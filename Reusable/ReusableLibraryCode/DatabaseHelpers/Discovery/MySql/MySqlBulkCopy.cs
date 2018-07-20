using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace ReusableLibraryCode.DatabaseHelpers.Discovery.MySql
{
    public class MySqlBulkCopy : BulkCopy
    {
        public MySqlBulkCopy(DiscoveredTable targetTable, IManagedConnection connection) : base(targetTable, connection)
        {
            
        }

        /* //Old method that used LOAD DATA IN FILE LOCAL (MySqlBulkLoader).  This requires local infile to be enabled on the server (and is generally a pain) : https://dev.mysql.com/doc/refman/5.7/en/load-data-local.html
        public override int Upload(DataTable dt)
        {
            //for all columns not appearing in the DataTable provided
            DiscoveredColumn[] unmatchedColumns;

            var matchedColumns = GetMapping(dt.Columns.Cast<DataColumn>(), out unmatchedColumns);
            
            var unmatchedPks = unmatchedColumns.Where(c => c.IsPrimaryKey).ToArray();

            if (unmatchedPks.Any())
            {
                if (!unmatchedPks.All(pk=>pk.IsAutoIncrement))
                    throw new Exception("Primary key columns " + string.Join(",", unmatchedPks.Select(c => c.GetRuntimeName())) + " did not appear in the DataTable and are not IsAutoIncrement");
            }
            else
            {
                //MySqlBulkLoader does upsert and ignore but no Throw option, so we have to enforce primary keys in memory instead
                if (dt.PrimaryKey.Length == 0)
                    dt.PrimaryKey = matchedColumns.Where(kvp => kvp.Value.IsPrimaryKey).Select(kvp=>kvp.Key).ToArray();
            }

            //make the column names in the data table match the destination columns in case etc
            foreach (KeyValuePair<DataColumn, DiscoveredColumn> kvp in matchedColumns)
                if (!kvp.Key.ColumnName.Equals(kvp.Value.GetRuntimeName()))
                    kvp.Key.ColumnName = kvp.Value.GetRuntimeName();

            var loader = new MySqlBulkLoader((MySqlConnection)Connection.Connection);
            loader.TableName = "`" + TargetTable.GetRuntimeName() +"`";
            
            var tempFile = Path.GetTempFileName();
            loader.FieldTerminator = ",";
            loader.LineTerminator = "\r\n";
            loader.FieldQuotationCharacter = '"';

            loader.Expressions.Clear();
            loader.Columns.Clear();

            //use the system default
            foreach (var column in unmatchedColumns)
                loader.Expressions.Add(column + " = DEFAULT");
            
            loader.Columns.AddRange(dt.Columns.Cast<DataColumn>().Select(c=>"`"+c.ColumnName+"`"));

            var sw = new StreamWriter(tempFile);
            Rfc4180Writer.WriteDataTable(dt,sw,false, new MySqlQuerySyntaxHelper());
            sw.Flush();
            sw.Close();

            loader.FileName = tempFile;
            loader.Timeout = Timeout;
            try
            {
                return loader.Load();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
        */
                public override int Upload(DataTable dt)
        {
            var matchedColumns = GetMapping(dt.Columns.Cast<DataColumn>());

            MySqlCommand cmd = new MySqlCommand("", (MySqlConnection)Connection.Connection,(MySqlTransaction) Connection.Transaction);

            string commandPrefix = string.Format("INSERT INTO {0}({1}) VALUES ", TargetTable.GetFullyQualifiedName(),string.Join(",", matchedColumns.Values.Select(c => "`" + c.GetRuntimeName() + "`")));

            StringBuilder sb = new StringBuilder();
                
            int affected = 0;
            int row = 0;

            foreach(DataRow dr in dt.Rows)
            {
                sb.Append('(');

                sb.Append(string.Join(",", matchedColumns.Keys.Select(k => "@" + k.ColumnName + row)));

                foreach (var k in matchedColumns.Keys)
                    cmd.Parameters.Add(new MySqlParameter("@" + k.ColumnName + row, dr[k]));

                sb.AppendLine("),");
                row++;

                //don't let command get too long
                if (row%1000 == 0)
                {
                    cmd.CommandText = commandPrefix + sb.ToString().TrimEnd(',', '\r', '\n');
                    affected += cmd.ExecuteNonQuery();
                        
                    cmd.Parameters.Clear();
                    sb.Clear();
                }
            }

            //send final batch
            if(sb.Length > 0)
            {
                cmd.CommandText = commandPrefix + sb.ToString().TrimEnd(',', '\r', '\n');
                affected += cmd.ExecuteNonQuery();
                sb.Clear();
            }
               
            return affected;
            
        }
    }
}