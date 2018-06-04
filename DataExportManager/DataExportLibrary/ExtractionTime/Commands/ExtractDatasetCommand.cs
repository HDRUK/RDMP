﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatalogueLibrary.Data;
using CatalogueLibrary.QueryBuilding;
using CatalogueLibrary.Repositories;
using DataExportLibrary.Data.DataTables;
using DataExportLibrary.Data.LinkCreators;
using DataExportLibrary.Interfaces.Data;
using DataExportLibrary.Interfaces.Data.DataTables;
using DataExportLibrary.Interfaces.ExtractionTime;
using DataExportLibrary.Interfaces.ExtractionTime.Commands;
using DataExportLibrary.Interfaces.ExtractionTime.UserPicks;
using DataExportLibrary.Data;
using DataExportLibrary.Repositories;

namespace DataExportLibrary.ExtractionTime.Commands
{
    /// <summary>
    /// Command representing a desire to extract a given dataset in an ExtractionConfiguration through an extraction pipeline.  This includes bundled content 
    /// (Lookup tables, SupportingDocuments etc).  Also includes optional settings (e.g. IncludeValidation) etc.  You can realise the request by running the 
    /// QueryBuilder SQL. 
    /// </summary>
    public class ExtractDatasetCommand : ExtractCommand, IExtractDatasetCommand
    {
        public IRDMPPlatformRepositoryServiceLocator RepositoryLocator { get; private set; }

        public ISelectedDataSets SelectedDataSets { get; set; }

        private IExtractableDatasetBundle _datasetBundle;
        
        public IExtractableCohort ExtractableCohort { get; set; }

        public IExtractableDatasetBundle DatasetBundle
        {
            get { return _datasetBundle; }
            set
            {
                _datasetBundle = value; 

                if(value == null)
                    Catalogue = null;
                else
                    Catalogue = RepositoryLocator.CatalogueRepository.GetObjectByID<Catalogue>(value.DataSet.Catalogue_ID);
            }
        }

        public List<IColumn> ColumnsToExtract{get;set;} 
        public IHICProjectSalt Salt{get;set;}
        public bool IncludeValidation {get;set;} 
        
        public IExtractionDirectory Directory { get; set; }
        public ICatalogue Catalogue { get; private set; }

        public ISqlQueryBuilder QueryBuilder { get; set; }
        public ICumulativeExtractionResults CumulativeExtractionResults { get; set; }
        public List<ReleaseIdentifierSubstitution> ReleaseIdentifierSubstitutions { get; private set; }
        public List<IExtractionResults> ExtractionResults { get; private set; }
        public int TopX { get; set; }

        public ExtractDatasetCommand(IRDMPPlatformRepositoryServiceLocator repositoryLocator, IExtractionConfiguration configuration, IExtractableCohort extractableCohort, IExtractableDatasetBundle datasetBundle, List<IColumn> columnsToExtract, IHICProjectSalt salt, IExtractionDirectory directory, bool includeValidation = false, bool includeLookups = false):this(configuration,datasetBundle.DataSet)
        {
            RepositoryLocator = repositoryLocator;
            ExtractableCohort = extractableCohort;
            DatasetBundle = datasetBundle;
            ColumnsToExtract = columnsToExtract;
            Salt = salt;
            Directory = directory;
            IncludeValidation = includeValidation;
        }

        /// <summary>
        /// This version has less arguments because it goes back to the database and queries the configuration and explores who the cohort is etc, it will result in more database
        /// queries than the more explicit constructor
        /// </summary>
        /// <param name="repositoryLocator"></param>
        /// <param name="configuration"></param>
        /// <param name="datasetBundle"></param>
        /// <param name="includeValidation"></param>
        /// <param name="includeLookups"></param>
        public ExtractDatasetCommand(IRDMPPlatformRepositoryServiceLocator repositoryLocator, IExtractionConfiguration configuration, IExtractableDatasetBundle datasetBundle, bool includeValidation = false, bool includeLookups = false):this(configuration,datasetBundle.DataSet)
        {
            RepositoryLocator = repositoryLocator;
            
            RepositoryLocator = repositoryLocator;
            //ExtractableCohort = ExtractableCohort.GetExtractableCohortByID((int) configuration.Cohort_ID);
            ExtractableCohort = configuration.GetExtractableCohort();
            DatasetBundle = datasetBundle;
            ColumnsToExtract = new List<IColumn>(Configuration.GetAllExtractableColumnsFor(datasetBundle.DataSet));
            Salt = new HICProjectSalt(Project);
            Directory = new ExtractionDirectory(Project.ExtractionDirectory, configuration);
            IncludeValidation = includeValidation;
        }

        public static readonly ExtractDatasetCommand EmptyCommand = new ExtractDatasetCommand();


        private ExtractDatasetCommand(IExtractionConfiguration configuration, IExtractableDataSet dataset):base(configuration)
        {
            var selectedDataSets = configuration.SelectedDataSets.Where(ds => ds.ExtractableDataSet_ID == dataset.ID).ToArray();

            if (selectedDataSets.Length != 1)
                throw new Exception("Could not find 1 ISelectedDataSets for ExtractionConfiguration '" + configuration + "' | Dataset '" + dataset +"'");

            SelectedDataSets = selectedDataSets[0];

            ExtractionResults = new List<IExtractionResults>();
        }

        private ExtractDatasetCommand() : base(null)
        {
            
        }

        public void GenerateQueryBuilder()
        {
            List<ReleaseIdentifierSubstitution> substitutions;
            var host = new ExtractionQueryBuilder(RepositoryLocator.DataExportRepository);
            QueryBuilder = host.GetSQLCommandForFullExtractionSet(this,out substitutions);
            QueryBuilder.Sort = true;
            ReleaseIdentifierSubstitutions = substitutions;
        }

        public override string ToString()
        {
            if (this == EmptyCommand)
                return "EmptyCommand";

            return DatasetBundle.DataSet.ToString();
        }

        public override DirectoryInfo GetExtractionDirectory()
        {
            if (this == EmptyCommand)
                return new DirectoryInfo(Path.GetTempPath());

            return Directory.GetDirectoryForDataset(DatasetBundle.DataSet);
        }
        public override string DescribeExtractionImplementation()
        {
            return QueryBuilder.SQL;
        }
    }
}