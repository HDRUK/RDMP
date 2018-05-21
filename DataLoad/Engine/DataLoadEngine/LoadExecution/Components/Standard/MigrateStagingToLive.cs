using System.Collections.Generic;
using System.Linq;
using CatalogueLibrary;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.DataLoad;
using CatalogueLibrary.DataFlowPipeline;
using DataLoadEngine.DatabaseManagement.EntityNaming;
using DataLoadEngine.Job;
using DataLoadEngine.LoadProcess;
using DataLoadEngine.Migration;
using HIC.Logging;
using ReusableLibraryCode.Progress;

namespace DataLoadEngine.LoadExecution.Components.Standard
{
    /// <summary>
    /// DLE component resonsible for merging records in the STAGING database into the LIVE database table(s) during a Data Load Engine execution.  The actual
    /// implementation of migrating records done by MigrationHost and MigrationConfiguration.
    /// </summary>
    public class MigrateStagingToLive : DataLoadComponent
    {
        private readonly IList<ICatalogue> _cataloguesToLoad;
        private readonly HICDatabaseConfiguration _databaseConfiguration;
        private readonly HICLoadConfigurationFlags _loadConfigurationFlags;
        
        public MigrateStagingToLive(IList<ICatalogue> cataloguesToLoad, HICDatabaseConfiguration databaseConfiguration, HICLoadConfigurationFlags loadConfigurationFlags)
        {
            _cataloguesToLoad = cataloguesToLoad;
            _databaseConfiguration = databaseConfiguration;
            _loadConfigurationFlags = loadConfigurationFlags;
            
            Description = "Migrate Staging to Live";
            SkipComponent = !_loadConfigurationFlags.DoMigrateFromStagingToLive;
        }

        public override ExitCodeType Run(IDataLoadJob job, GracefulCancellationToken cancellationToken)
        {
            if (Skip(job)) return ExitCodeType.Error;

            //if(_migrationHost != null)
            //    throw new Exception("Load stage already started once");

            // After the user-defined load process, the framework handles the insert into staging and resolves any conflicts
            var stagingDbInfo = _databaseConfiguration.DeployInfo[LoadBubble.Staging];
            var liveDbInfo = _databaseConfiguration.DeployInfo[LoadBubble.Live];
            
            job.OnNotify(this,new NotifyEventArgs(ProgressEventType.Information, "Migrating '" + stagingDbInfo + "' to '" + liveDbInfo + "'"));

            var migrationConfig = new MigrationConfiguration(stagingDbInfo, LoadBubble.Staging, LoadBubble.Live, _databaseConfiguration.DatabaseNamer);
            var migrationHost = new MigrationHost(_cataloguesToLoad.ToList(), stagingDbInfo, liveDbInfo, _databaseConfiguration, migrationConfig);
            migrationHost.Migrate(_loadConfigurationFlags, job, cancellationToken);

            return ExitCodeType.Success;
        }

        public override void LoadCompletedSoDispose(ExitCodeType exitCode,IDataLoadEventListener postLoadEventListener)
        {
        }
    }
}