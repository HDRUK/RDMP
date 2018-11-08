using CatalogueLibrary;
using CatalogueLibrary.DataFlowPipeline;
using DataLoadEngine.Job;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.DatabaseHelpers.Discovery;
using ReusableLibraryCode.Progress;

namespace DataLoadEngine.Attachers
{
    /// <summary>
    /// A Class which will run during Data Load Engine execution and result in the creation or population of a RAW database, the database may or not require 
    /// to already exist (e.g. MDFAttacher would expect it not to exist but AnySeparatorFileAttacher would require the tables/databases already exist).
    /// </summary>
    public abstract class Attacher : IAttacher
    {
        protected DiscoveredDatabase _dbInfo;

        public abstract ExitCodeType Attach(IDataLoadJob job, GracefulCancellationToken cancellationToken);

        public IHICProjectDirectory HICProjectDirectory { get; set; }
        
        public bool RequestsExternalDatabaseCreation { get; private set; }

        public virtual void Initialize(IHICProjectDirectory hicProjectDirectory, DiscoveredDatabase dbInfo)
        {
            HICProjectDirectory = hicProjectDirectory;
            _dbInfo = dbInfo;
        }
        
        protected Attacher(bool requestsExternalDatabaseCreation)
        {
            RequestsExternalDatabaseCreation = requestsExternalDatabaseCreation;
        }

        public abstract void Check(ICheckNotifier notifier);

        
        public abstract void LoadCompletedSoDispose(ExitCodeType exitCode,IDataLoadEventListener postLoadEventListener);
    }
}