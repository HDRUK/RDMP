// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FAnsi.Discovery;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;
using Version = System.Version;

namespace MapsDirectlyToDatabaseTable.Versioning
{
    /// <summary>
    /// Creates new databases with a fixed (versioned) schema (determined by an <see cref="IPatcher"/>) into a database server (e.g. localhost\sqlexpress).
    /// </summary>
    public class MasterDatabaseScriptExecutor
    {
        private readonly string _server;
        private readonly string _database;

        private const string RoundhouseSchemaName ="RoundhousE";
        private const string RoundhouseVersionTable = "Version";
        private const string RoundhouseScriptsRunTable = "ScriptsRun";

        private readonly SqlConnectionStringBuilder _builder;
        private const string InitialDatabaseScriptName = @"Initial Database Setup";

        public MasterDatabaseScriptExecutor(string connectionString)
        {
            _builder = new SqlConnectionStringBuilder(connectionString);
            _server = _builder.DataSource;
            _database = _builder.InitialCatalog;
        }

        public MasterDatabaseScriptExecutor(string server, string database, string username, string password)
        {
            _server = server;
            _database = database;

            _builder = new SqlConnectionStringBuilder
            {
                DataSource = _server,
                InitialCatalog = _database,
                IntegratedSecurity = string.IsNullOrWhiteSpace(username),
            };

            if (!string.IsNullOrWhiteSpace(username))
            {
                _builder.UserID = username;
                _builder.Password = password;
            }
        }

        public MasterDatabaseScriptExecutor(DiscoveredServer server, string database):this(server.ExpectDatabase(database))
        {
            
        }

        public MasterDatabaseScriptExecutor(DiscoveredDatabase discoveredDatabase)
        {
            _builder = (SqlConnectionStringBuilder)discoveredDatabase.Server.Builder;
            _server = _builder.DataSource;
            _database = _builder.InitialCatalog;

            _builder = new SqlConnectionStringBuilder
            {
                DataSource = _server,
                InitialCatalog = _database,
                IntegratedSecurity = string.IsNullOrWhiteSpace(_builder.UserID),
            };

            if (!string.IsNullOrWhiteSpace(_builder.UserID))
            {
                _builder.UserID = _builder.UserID;
                _builder.Password = _builder.Password;
            }
        }

        public bool BinaryCollation { get; set; }

        public string CreateConnectionString(bool includeDatabaseInString = true)
        {
            if (!includeDatabaseInString)
            {
                var serverOnlyBuilder = new SqlConnectionStringBuilder(_builder.ConnectionString) {InitialCatalog = ""};
                return serverOnlyBuilder.ConnectionString;
            }

            return _builder.ConnectionString;
        }

        public bool CreateDatabase(string createTablesAndFunctionsSql, string initialVersionNumber, ICheckNotifier notifier)
        {
            try
            {
                // The _builder has InitialCatalog set which will cause the pre-database creation connection to fail, so create one which doesn't contain InitialCatalog
                var serverBuilder = new SqlConnectionStringBuilder(_builder.ConnectionString) { InitialCatalog = "" };

                DiscoveredServer server = new DiscoveredServer(serverBuilder);
                server.TestConnection();

                var db = server.ExpectDatabase(_database);

                if (db.Exists())//make sure database does not already exist
                {
                    bool createAnyway = notifier.OnCheckPerformed(new CheckEventArgs("Database already exists", CheckResult.Warning, null,"Attempt to create database inside existing database (will cause problems if the database is not empty)?"));

                    if(!createAnyway)
                        throw new Exception("User chose not continue");
                }
                else
                {
                    using (var con = server.GetConnection())//do it manually 
                    {
                        con.Open();
                        server.GetCommand("CREATE DATABASE " + _database + (BinaryCollation?" COLLATE Latin1_General_BIN2":""), con).ExecuteNonQuery();
                        notifier.OnCheckPerformed(new CheckEventArgs("Database " + _database + " created", CheckResult.Success, null));
                    }
                }
                
                SqlConnection.ClearAllPools();

                using (var con = db.Server.GetConnection())
                {
                    con.Open();

                    var cmd =  db.Server.GetCommand("CREATE SCHEMA " + RoundhouseSchemaName, con);
                    cmd.ExecuteNonQuery();

                    var sql = 
                    @"CREATE TABLE [RoundhousE].[ScriptsRun](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[version_id] [bigint] NULL,
	[script_name] [nvarchar](255) NULL,
	[text_of_script] [text] NULL,
	[text_hash] [nvarchar](512) NULL,
	[one_time_script] [bit] NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)
)

CREATE TABLE [RoundhousE].[Version](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[repository_path] [nvarchar](255) NULL,
	[version] [nvarchar](50) NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)
)
";

                    var cmd2 = db.Server.GetCommand(sql, con);
                    cmd2.ExecuteNonQuery();
                }

                RunSQL(db, createTablesAndFunctionsSql, InitialDatabaseScriptName);

                SetVersion(db,"Initial Setup", initialVersionNumber);

                notifier.OnCheckPerformed(new CheckEventArgs("Tables created", CheckResult.Success, null));

                notifier.OnCheckPerformed(new CheckEventArgs("Setup Completed successfully", CheckResult.Success, null));

                return true;
            }
            catch (Exception e)
            {
                notifier.OnCheckPerformed(new CheckEventArgs("Create failed", CheckResult.Fail, e));
                return false;
            }
        }

        private void RunSQL(DiscoveredDatabase db, string sql, string filename)
        {
            using (var con = db.Server.GetConnection())
            {
                con.Open();

                UsefulStuff.ExecuteBatchNonQuery(sql, con);

                string insert = @"
INSERT INTO [RoundhousE].[ScriptsRun]
           ([script_name],
           [text_of_script],
           [text_hash],
           [one_time_script],
           [entry_date],
           [modified_date],
           [entered_by])
     VALUES
          (@script_name,
           @text_of_script,
           @text_hash,
           @one_time_script,
           @entry_date,
           @modified_date,
           @entered_by)
";

                DateTime dt = DateTime.Now;

                var cmd2 = db.Server.GetCommand(insert, con);

                db.Server.AddParameterWithValueToCommand("@script_name",cmd2,filename);
                db.Server.AddParameterWithValueToCommand("@text_of_script",cmd2,sql);
                db.Server.AddParameterWithValueToCommand("@text_hash",cmd2,CalculateHash(sql));
                db.Server.AddParameterWithValueToCommand("@one_time_script",cmd2,1);
                db.Server.AddParameterWithValueToCommand("@entry_date",cmd2,dt);
                db.Server.AddParameterWithValueToCommand("@modified_date",cmd2,dt);
                db.Server.AddParameterWithValueToCommand("@entered_by",cmd2,Environment.UserName);

                cmd2.ExecuteNonQuery();
            }

        }
        
        public string CalculateHash(string input)
        {
            // step 1, calculate MD5 hash from input

            var hashProvider = SHA512.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            byte[] hash = hashProvider.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(i.ToString("X2"));

            return sb.ToString();

        }



        private void SetVersion(DiscoveredDatabase db, string name, string version)
        {
            var versionTable = db.ExpectTable(RoundhouseVersionTable,RoundhouseSchemaName);
            versionTable.Truncate();

            //repository_path	version	entry_date	modified_date	entered_by
            //Patching	2.6.0.1	2018-02-05 08:26:54.000	2018-02-05 08:26:54.000	DUNDEE\TZNind
            
            using(var con =  db.Server.GetConnection())
            {
                con.Open();

                var sql = "INSERT INTO " + versionTable.GetFullyQualifiedName() +
                          "(repository_path,version,entry_date,modified_date,entered_by) VALUES (@repository_path,@version,@entry_date,@modified_date,@entered_by)";


                var cmd = db.Server.GetCommand(sql, con);

                var dt = DateTime.Now;

                db.Server.AddParameterWithValueToCommand("@repository_path", cmd, name);
                db.Server.AddParameterWithValueToCommand("@version",cmd,version);
                db.Server.AddParameterWithValueToCommand("@entry_date",cmd, dt);
                db.Server.AddParameterWithValueToCommand("@modified_date",cmd,dt);
                db.Server.AddParameterWithValueToCommand("@entered_by", cmd, Environment.UserName);

                cmd.ExecuteNonQuery();
            }
                
        }

        public bool PatchDatabase(SortedDictionary<string, Patch> patches, ICheckNotifier notifier, Func<Patch, bool> patchPreviewShouldIRunIt, bool backupDatabase = true)
        {
            if(!patches.Any())
            {
                notifier.OnCheckPerformed(new CheckEventArgs("There are no patches to apply so skipping patching", CheckResult.Success,null));
                return true;
            }

            Version maxPatchVersion = patches.Values.Max(pat => pat.DatabaseVersionNumber);

            var db = new DiscoveredServer(_builder).GetCurrentDatabase();

            if (backupDatabase)
            {
                try
                {
                    notifier.OnCheckPerformed(new CheckEventArgs("About to backup database", CheckResult.Success, null));

                    db.CreateBackup("Full backup of " + _database);
            
                    notifier.OnCheckPerformed(new CheckEventArgs("Database backed up", CheckResult.Success, null));
                
                }
                catch (Exception e)
                {
                    notifier.OnCheckPerformed(new CheckEventArgs(
                        "Patching failed during setup and preparation (includes failures due to backup creation failures)",
                        CheckResult.Fail, e));
                    return false;
                }
            }
            

            try
            {
                int i = 0;
                foreach (KeyValuePair<string, Patch> patch in patches)
                {
                    i++;

                    bool shouldRun = patchPreviewShouldIRunIt(patch.Value);

                    if (shouldRun)
                    {

                        try
                        {
                            RunSQL(db, patch.Value.EntireScript, patch.Key);
                        }
                        catch(Exception e)
                        {
                            throw new Exception($"Failed to apply patch '{ patch.Key }'",e);
                        }
                        

                        notifier.OnCheckPerformed(new CheckEventArgs("Executed patch " + patch.Value, CheckResult.Success, null));
                    }
                    else
                        throw new Exception("User decided not to execute patch " + patch.Key + " aborting ");
                }

                UpdateVersionIncludingClearingLastVersion(db,notifier,maxPatchVersion);
                
                //all went fine
                notifier.OnCheckPerformed(new CheckEventArgs("All Patches applied, transaction committed", CheckResult.Success, null));
                
                return true;

            }
            catch (Exception e)
            {
                notifier.OnCheckPerformed(new CheckEventArgs("Error occurred during patching", CheckResult.Fail, e));
                return false;
            }
        }

        private void UpdateVersionIncludingClearingLastVersion(DiscoveredDatabase db,ICheckNotifier notifier, Version maxPatchVersion)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_builder.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmdClear = new SqlCommand("Delete from RoundhousE.Version", con);
                    cmdClear.ExecuteNonQuery();
                    con.Close();
                    notifier.OnCheckPerformed(new CheckEventArgs("successfully deleted old Version number from RoundhousE.Version", CheckResult.Success, null));
                }
            }
            catch (Exception e)
            {
                notifier.OnCheckPerformed(new CheckEventArgs(
                    "Could not clear previous version history (but will continue with versioning anyway) ",
                    CheckResult.Fail, e));
            }
            //increment the version number if there were any patches
            SetVersion(db,"Patching", maxPatchVersion.ToString());
            notifier.OnCheckPerformed(new CheckEventArgs("Updated database version to " + maxPatchVersion.ToString(), CheckResult.Success, null));
                
        }


        public Patch[] GetPatchesRun()
        { 
            List<Patch> toReturn = new List<Patch>();
            
            using (var con = new SqlConnection(CreateConnectionString()))
            {
                
                con.Open();

                SqlCommand cmd = new SqlCommand("Select * from " +RoundhouseSchemaName +"."+ RoundhouseScriptsRunTable, con);
                var r = cmd.ExecuteReader();

                while (r.Read())
                {
                    string text_of_script = r["text_of_script"] as string;
                    string script_name = r["script_name"] as string;
                    
                    if(string.IsNullOrWhiteSpace(script_name) || 
                        string.IsNullOrWhiteSpace(text_of_script) || 
                        script_name.Equals(InitialDatabaseScriptName))
                        continue;

                    Patch p = new Patch(script_name,text_of_script);
                    toReturn.Add(p);
                }
                
                con.Close();
            }
            return toReturn.ToArray();
        }

        /// <summary>
        /// Creates a new platform database and patches it
        /// </summary>
        /// <param name="patcher">Determines the SQL schema created</param>
        /// <param name="notifier">audit object, can be a new ThrowImmediatelyCheckNotifier if you aren't in a position to pass one</param>
        public void CreateAndPatchDatabase(IPatcher patcher, ICheckNotifier notifier)
        {
            string sql = Patch.GetInitialCreateScriptContents(patcher);

            //get everything in the /up/ folder that are .sql
            var patches = Patch.GetAllPatchesInAssembly(patcher);

            CreateDatabase(sql, "1.0.0.0", notifier);
            PatchDatabase(patches,notifier,(p)=>true);//apply all patches without question
        }
    }
}
