using System.Data.Common;
using CatalogueLibrary.Data;
using CatalogueLibrary.Repositories;
using DataExportLibrary.Data.DataTables;
using DataExportLibrary.Interfaces.Data.DataTables;
using ReusableLibraryCode.DataAccess;

namespace DataExportLibrary.DataRelease
{
    /// <summary>
    /// Determines whether a given ExtractableDataSet in an ExtractionConfiguration, as generated by an Extracion To DB pipeline is ready for Release. 
    /// This includes making sure that the current configuration in the database matches the extracted tables that are destined for release.
    /// </summary>
    public class MsSqlExtractionReleasePotential : ReleasePotential
    {
        public MsSqlExtractionReleasePotential(IRDMPPlatformRepositoryServiceLocator repositoryLocator, IExtractionConfiguration configuration, IExtractableDataSet dataSet) : base(repositoryLocator, configuration, dataSet)
        {
            
        }

        protected override Releaseability GetSpecificAssessment()
        {
            var externalServerId = int.Parse(ExtractionResults.DestinationDescription.Split('|')[0]);
            var externalServer = _repositoryLocator.CatalogueRepository.GetObjectByID<ExternalDatabaseServer>(externalServerId);
            var dbName = ExtractionResults.DestinationDescription.Split('|')[1];
            var tblName = ExtractionResults.DestinationDescription.Split('|')[2];
            var server = DataAccessPortal.GetInstance().ExpectServer(externalServer, DataAccessContext.DataExport, setInitialDatabase: false);
            var database = server.ExpectDatabase(dbName);
            if (!database.Exists())
            {
                return Releaseability.ExtractFilesMissing;
            }
            var foundTable = database.ExpectTable(tblName);
            if (!foundTable.Exists())
            {
                return Releaseability.ExtractFilesMissing;
            }

            // TODO: Table can be polluted, how to check?? CHECK FOR SPURIOUS TABLES?
            
            return Releaseability.Undefined;
        }
    }
}