using System;
using System.Data;
using System.Data.SqlClient;
using CatalogueLibrary.DataFlowPipeline;
using DataLoadEngine.Job;
using ReusableLibraryCode.DatabaseHelpers.Discovery;
using ReusableLibraryCode.Progress;

namespace DataLoadEngine.Migration
{
    /// <summary>
    /// Migrates from STAGING to LIVE a single table (with a MigrationColumnSet).  This is an UPSERT (new replaces old) operation achieved (in SQL) with MERGE and 
    /// UPDATE (based on primary key).  Both tables must be on the same server.  A MERGE sql statement will be created using LiveMigrationQueryHelper and executed
    /// within a transaction.
    /// </summary>
    public class OverwriteMigrationStrategy : DatabaseMigrationStrategy
    {
        public OverwriteMigrationStrategy(string sourceDatabaseName, IManagedConnection managedConnection)
            : base(sourceDatabaseName, managedConnection)
        {
        }

        public override void MigrateTable(IDataLoadJob job, MigrationColumnSet columnsToMigrate, int dataLoadInfoID, GracefulCancellationToken cancellationToken, ref int inserts, ref int updates)
        {
            var sourceTable = String.Format("[{0}]..[{1}]", _sourceDatabaseName, columnsToMigrate.SourceTableName);
            var destTable = String.Format("[{0}]..[{1}]", columnsToMigrate.DestinationDatabase, columnsToMigrate.DestinationTableName);

            var queryHelper = new LiveMigrationQueryHelper(columnsToMigrate, dataLoadInfoID);
            string mergeQuery;
            try
            {
                mergeQuery = queryHelper.BuildMergeQuery(sourceTable, destTable);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(String.Format("Could not build merge query to migrate from {0} to {1}", sourceTable, destTable), e);
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            var con = (SqlConnection) _managedConnection.ManagedTransaction.Connection;
            var trans = (SqlTransaction)_managedConnection.ManagedTransaction.Transaction;

            var cmd = new SqlCommand(mergeQuery, con,trans ) { CommandType = CommandType.Text, CommandTimeout = Timeout };
            job.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Merge query: " + Environment.NewLine + mergeQuery));

            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    switch (reader[0].ToString())
                    {
                        case "INSERT":
                            inserts++;
                            break;
                            // we ignore updates here, these require separate logic
                    }
                }
                reader.Close();

                var updateQuery = CreateUpdateQuery(columnsToMigrate, dataLoadInfoID);
                job.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Update query:" + Environment.NewLine + updateQuery));

                var updateCmd = new SqlCommand(updateQuery, con, trans)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = Timeout
                };
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    updates = (int) updateCmd.ExecuteScalar();
                    //var updateTask = updateCmd.ExecuteScalarAsync(cancellationToken.CreateLinkedSource().Token);
                    //updateTask.Wait();
                    //updates = (int) updateTask.Result;
                }
                catch (SqlException e)
                {
                    job.OnNotify(this, new NotifyEventArgs(ProgressEventType.Error, "Did not successfully perform the update queries: " + updateQuery, e));
                    throw new Exception("Did not successfully perform the update queries: " + updateQuery + " - " + e);
                }
            }
            catch (OperationCanceledException)
            {
                throw; // have to catch and rethrow this because of the catch-all below
            }
            catch (Exception e)
            {
                job.OnNotify(this, new NotifyEventArgs(ProgressEventType.Error, "Failed to migrate " + sourceTable + " to " + destTable, e));
                throw new Exception("Failed to migrate " + sourceTable + " to " + destTable + ": " + e);
            }
        }
    }
}