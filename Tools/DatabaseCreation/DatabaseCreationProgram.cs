﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CatalogueLibrary.Database;
using CatalogueLibrary.Repositories;
using CommandLine;
using MapsDirectlyToDatabaseTable.Versioning;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.DatabaseHelpers.Discovery;
using ReusableLibraryCode.DatabaseHelpers.Discovery.Microsoft;

namespace DatabaseCreation
{
    /// <summary>
    /// Creates the minimum set of databases required to get RDMP working (Catalogue and Data Export databases) with an optional prefix.  Also creates satellite
    /// Tier 2 databases (logging / dqe)
    /// </summary>
    public class DatabaseCreationProgram
    {
        public const string DefaultCatalogueDatabaseName = "Catalogue";
        public const string DefaultDataExportDatabaseName = "DataExport";
        public const string DefaultDQEDatabaseName = "DQE";
        public const string DefaultLoggingDatabaseName = "Logging";

        public static int Main(string[] args)
        {
            return UsefulStuff.GetParser().ParseArguments<DatabaseCreationProgramOptions>(args).MapResult(RunOptionsAndReturnExitCode, errs => 1);
        }

        private static int RunOptionsAndReturnExitCode(DatabaseCreationProgramOptions options)
        {
            
            var serverName = options.ServerName;
            var prefix = options.Prefix;

            Console.WriteLine("About to create on server '" + serverName + "' databases with prefix '" + prefix + "'");

            try
            {
                Create(serverName, prefix, DefaultCatalogueDatabaseName, typeof(Class1).Assembly, options.DropDatabases, options.BinaryCollation);
                Create(serverName, prefix, DefaultDataExportDatabaseName, typeof(DataExportLibrary.Database.Class1).Assembly, options.DropDatabases, options.BinaryCollation);

                var dqe = Create(serverName, prefix, DefaultDQEDatabaseName, typeof(DataQualityEngine.Database.Class1).Assembly, options.DropDatabases, options.BinaryCollation);
                var logging = Create(serverName, prefix, DefaultLoggingDatabaseName, typeof(HIC.Logging.Database.Class1).Assembly, options.DropDatabases, options.BinaryCollation);

                CatalogueRepository.SuppressHelpLoading = true;

                if (!options.SkipPipelines)
                {
                    var creator = new CataloguePipelinesAndReferencesCreation(new DatabaseCreationRepositoryFinder(serverName, prefix), logging, dqe);
                    creator.Create();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
            return 0;
        }

        private static SqlConnectionStringBuilder Create(string serverName, string prefix, string databaseName, Assembly assembly, bool dropFlag, bool binaryCollationFlag)
        {
            SqlConnection.ClearAllPools();

            var builder = GetBuilder(serverName, prefix, databaseName);
            DiscoveredDatabase db = new DiscoveredDatabase(new DiscoveredServer(builder), builder.InitialCatalog,new MicrosoftQuerySyntaxHelper());

            if (dropFlag && db.Exists())
            {
                Console.WriteLine("Dropping Database:" + builder.InitialCatalog);
                db.ForceDrop();
            }
            
            MasterDatabaseScriptExecutor executor = new MasterDatabaseScriptExecutor(builder.ConnectionString);
            executor.BinaryCollation = binaryCollationFlag;
            executor.CreateAndPatchDatabaseWithDotDatabaseAssembly(assembly,new AcceptAllCheckNotifier());
            Console.WriteLine("Created " + builder.InitialCatalog + " on server " + builder.DataSource);
            
            return builder;
        }

        public static SqlConnectionStringBuilder GetBuilder(string serverName,string prefix, string databaseName)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.IntegratedSecurity = true;
            builder.DataSource = serverName;
            builder.InitialCatalog = (prefix ?? "") + databaseName;
            return builder;
        }
    }
}
