using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CatalogueLibrary.Triggers.Exceptions;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.DatabaseHelpers.Discovery;
using ReusableLibraryCode.DatabaseHelpers.Discovery.QuerySyntax;
using ReusableLibraryCode.DatabaseHelpers.Discovery.TypeTranslation;

namespace CatalogueLibrary.Triggers.Implementations
{
    public abstract class TriggerImplementer:ITriggerImplementer
    {
        protected readonly bool _createDataLoadRunIdAlso;
        
        protected readonly DiscoveredServer _server;
        protected readonly DiscoveredTable _table;
        protected readonly DiscoveredTable _archiveTable;
        protected readonly DiscoveredColumn[] _columns;
        protected readonly DiscoveredColumn[] _primaryKeys;

        public TriggerImplementer(DiscoveredTable table, bool createDataLoadRunIDAlso = true)
        {
            _server = table.Database.Server;
            _table = table;
            _archiveTable = _table.Database.ExpectTable(table.GetRuntimeName() + "_Archive");
            _columns = table.DiscoverColumns();
            _primaryKeys = _columns.Where(c => c.IsPrimaryKey).ToArray();
            
            _createDataLoadRunIdAlso = createDataLoadRunIDAlso;
        }

        public abstract void DropTrigger(out string problemsDroppingTrigger, out string thingsThatWorkedDroppingTrigger);

        public virtual string CreateTrigger(ICheckNotifier notifier, int timeout = 30)
        {
            if (!_primaryKeys.Any())
                throw new TriggerException("There must be at least 1 primary key");

            //if _Archive exists skip creating it
            bool skipCreatingArchive = _archiveTable.Exists();

            //check _Archive does not already exist
            foreach (string forbiddenColumnName in new[] { "hic_validTo", "hic_userID", "hic_status" })
                if (_columns.Any(c => c.GetRuntimeName().Equals(forbiddenColumnName, StringComparison.CurrentCultureIgnoreCase)))
                    throw new TriggerException("Table " + _table + " already contains a column called " + forbiddenColumnName + " this column is reserved for Archiving");

            bool b_mustCreate_validFrom = !_columns.Any(c => c.GetRuntimeName().Equals(SpecialFieldNames.ValidFrom, StringComparison.CurrentCultureIgnoreCase));
            bool b_mustCreate_dataloadRunId = !_columns.Any(c => c.GetRuntimeName().Equals(SpecialFieldNames.DataLoadRunID,StringComparison.CurrentCultureIgnoreCase)) && _createDataLoadRunIdAlso;

            //forces column order dataloadrunID then valid from (doesnt prevent these being in the wrong place in the record but hey ho - possibly not an issue anyway since probably the 3 values in the archive are what matters for order - see the Trigger which populates *,X,Y,Z where * is all columns in mane table
            if (b_mustCreate_dataloadRunId && !b_mustCreate_validFrom)
                throw new TriggerException("Cannot create trigger because table contains " + SpecialFieldNames.ValidFrom + " but not " + SpecialFieldNames.DataLoadRunID + " (ID must be placed before valid from in column order)");

            //must add validFrom outside of transaction if we want SMO to pick it up
            if (b_mustCreate_dataloadRunId)
                _table.AddColumn(SpecialFieldNames.DataLoadRunID, new DatabaseTypeRequest(typeof(int)), true, timeout);

            var syntaxHelper = _server.GetQuerySyntaxHelper();
            var dateTimeDatatype = syntaxHelper.TypeTranslater.GetSQLDBTypeForCSharpType(new DatabaseTypeRequest(typeof (DateTime)));
            var nowFunction = syntaxHelper.GetScalarFunctionSql(MandatoryScalarFunctions.GetTodaysDate);

            //must add validFrom outside of transaction if we want SMO to pick it up
            if (b_mustCreate_validFrom)
                _table.AddColumn(SpecialFieldNames.ValidFrom, string.Format(" {0} DEFAULT {1}", dateTimeDatatype, nowFunction), true, timeout);

            string sql = WorkOutArchiveTableCreationSQL(); 
            
            if (!skipCreatingArchive)
                using(var con = _server.GetConnection())
                {
                    con.Open();
                
                    var cmdCreateArchive = _server.GetCommand(sql, con);

                    cmdCreateArchive.ExecuteNonQuery();

                    _archiveTable.AddColumn("hic_validTo", new DatabaseTypeRequest(typeof(DateTime)), true, timeout);
                    _archiveTable.AddColumn("hic_userID", new DatabaseTypeRequest(typeof(string), 128), true, timeout);
                    _archiveTable.AddColumn("hic_status", new DatabaseTypeRequest(typeof(string), 1), true, timeout);
                }

            return sql;
        }

        

        private string WorkOutArchiveTableCreationSQL()
        {
            //script original table
            string createTableSQL = _table.ScriptTableCreation(true, true, true);

            string toReplaceTableName = Regex.Escape("CREATE TABLE " + _table.GetFullyQualifiedName());

            if (Regex.Matches(createTableSQL, toReplaceTableName).Count != 1)
                throw new Exception("Expected to find 1 occurrence of " + toReplaceTableName + " in the SQL " + createTableSQL);

            //rename table
            createTableSQL = Regex.Replace(createTableSQL, toReplaceTableName, "CREATE TABLE " + _archiveTable);

            string toRemoveIdentities = "IDENTITY\\(\\d+,\\d+\\)";

            //drop identity bit
            createTableSQL = Regex.Replace(createTableSQL, toRemoveIdentities, "");

            return createTableSQL;
        }
        public abstract TriggerStatus GetTriggerStatus();

        /// <summary>
        /// Returns true if the trigger exists and the method body of the trigger matches the expected method body.  This exists to handle
        /// the situation where a trigger is created on a table then the schema of the live table or the archive table is altered subsequently.
        /// 
        /// <para>The best way to implement this is to regenerate the trigger and compare it to the current code fetched from the ddl</para>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckUpdateTriggerIsEnabledAndHasExpectedBody()
        {
            //check server has trigger and it is on 
            TriggerStatus isEnabledSimple = GetTriggerStatus();

            if (isEnabledSimple == TriggerStatus.Disabled || isEnabledSimple == TriggerStatus.Missing)
                return false;
            
            CheckColumnDefinitionsMatchArchive();

            return true;
        }

        private void CheckColumnDefinitionsMatchArchive()
        {
            List<string> errors = new List<string>();

            var archiveTableCols = _archiveTable.DiscoverColumns().ToArray();

            foreach (DiscoveredColumn col in _columns)
            {
                var colInArchive = archiveTableCols.SingleOrDefault(c => c.GetRuntimeName().Equals(col.GetRuntimeName()));

                if (colInArchive == null)
                    errors.Add("Column " + col.GetRuntimeName() + " appears in Table '" + _table + "' but not in archive table '" + _archiveTable + "'");
                else
                    if (!AreCompatibleDatatypes(col.DataType, colInArchive.DataType))
                        errors.Add("Column " + col.GetRuntimeName() + " has data type '" + col.DataType + "' in '" + _table + "' but in Archive table '" + _archiveTable + "' it is defined as '" + colInArchive.DataType + "'");
            }

            if (errors.Any())
                throw new IrreconcilableColumnDifferencesInArchiveException("The following column mismatch errors were seen:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
        }

        private bool AreCompatibleDatatypes(DiscoveredDataType mainDataType, DiscoveredDataType archiveDataType)
        {
            var t1 = mainDataType.SQLType;
            var t2 = archiveDataType.SQLType;

            if (t1.Equals(t2, StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (t1.ToLower().Contains("identity"))
                return t1.ToLower().Replace("identity", "").Trim().Equals(t2.ToLower().Trim());

            return false;
        }

    }
}
