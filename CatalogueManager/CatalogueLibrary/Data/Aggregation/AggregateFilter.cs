﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CatalogueLibrary.Checks;
using CatalogueLibrary.DataHelper;
using CatalogueLibrary.FilterImporting;
using CatalogueLibrary.FilterImporting.Construction;
using CatalogueLibrary.Repositories;
using MapsDirectlyToDatabaseTable;
using MapsDirectlyToDatabaseTable.Revertable;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;

namespace CatalogueLibrary.Data.Aggregation
{
    /// <summary>
    /// Sometimes you want to restrict the data that is Aggregated as part of an AggregateConfiguration.  E.g. you might want to only aggregate records loaded
    /// in the last 6 months.  To do this you would need to set a root AggregateFilterContainer on the AggregateConfiguration and then put in an appropriate
    /// AggregateFilter.  Each AggregateFilter can be associated with a given ColumnInfo this will ensure that it is included when it comes to JoinInfo time
    /// in QueryBuilding even if it is not a selected dimension (this allows you to for example aggregate the drug codes but filter by drug prescribed date even
    /// when the two fields are in different tables - that will be joined at QueryTime).
    /// 
    /// Each AggregateFilter can have a collection of AggregateFilterParameters which store SQL paramater values (along with descriptions for the user) that let you
    /// paramaterise (for the user) your AggregateFilter
    /// </summary>
    public class AggregateFilter : ConcreteFilter
    {
        #region Database Properties

        private int? _filterContainerID;
        private int? _clonedFromExtractionFilterID;

        public override int? ClonedFromExtractionFilter_ID
        {
            get { return _clonedFromExtractionFilterID; }
            set { SetField(ref _clonedFromExtractionFilterID , value); }
        }

        public override int? FilterContainer_ID
        {
            get { return _filterContainerID; }
            set { SetField(ref  _filterContainerID, value); }
        }

        private int? _associatedColumnInfoID;

        public int? AssociatedColumnInfo_ID
        {
            get { return _associatedColumnInfoID; }
            set { SetField(ref  _associatedColumnInfoID, value); }
        }
        #endregion

        #region Relationships
        [NoMappingToDatabase]
        public IEnumerable<AggregateFilterParameter> AggregateFilterParameters {
            get { return Repository.GetAllObjectsWithParent<AggregateFilterParameter>(this); }
        }

        public override ISqlParameter[] GetAllParameters()
        {
            return AggregateFilterParameters.ToArray();
        }

        [NoMappingToDatabase]
        public override IContainer FilterContainer { get { return FilterContainer_ID.HasValue? Repository.GetObjectByID<AggregateFilterContainer>(FilterContainer_ID.Value):null;}}

        #endregion


        public static int Name_MaxLength = -1;
        public static int Description_MaxLength = -1;
        

        public AggregateFilter(ICatalogueRepository repository, string name=null, AggregateFilterContainer container=null)
        {
            name = name ?? "New AggregateFilter" + Guid.NewGuid();
            
            repository.InsertAndHydrate(this,new Dictionary<string, object>
            {
                 {"Name", name},
                 {"FilterContainer_ID", container != null ? (object)container.ID : DBNull.Value}
            });
        }

        public AggregateFilter(ICatalogueRepository repository, DbDataReader r): base(repository, r)
        {
            WhereSQL = r["WhereSQL"] as string;
            Description = r["Description"] as string;
            Name = r["Name"] as string;
            IsMandatory = (bool)r["IsMandatory"];
            ClonedFromExtractionFilter_ID = ObjectToNullableInt(r["ClonedFromExtractionFilter_ID"]);

            object associatedColumnInfo_ID = r["AssociatedColumnInfo_ID"];
            if (associatedColumnInfo_ID != DBNull.Value)
                AssociatedColumnInfo_ID = int.Parse(associatedColumnInfo_ID.ToString());

            if (r["FilterContainer_ID"] != null && !string.IsNullOrWhiteSpace(r["FilterContainer_ID"].ToString()))
                FilterContainer_ID = int.Parse(r["FilterContainer_ID"].ToString());
            else
                FilterContainer_ID = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public override ColumnInfo GetColumnInfoIfExists()
        {
            if (AssociatedColumnInfo_ID != null)
                try
                {
                    return Repository.GetObjectByID<ColumnInfo>((int)AssociatedColumnInfo_ID);
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }

            return null;
        }

        public override IFilterFactory GetFilterFactory()
        {
            return new AggregateFilterFactory((ICatalogueRepository)Repository);
        }

        public override Catalogue GetCatalogue()
        {
            var agg = GetAggregate();

            if(agg == null)
                throw new Exception("Cannot determine the Catalogue for AggregateFilter " + this + " because GetAggregate returned null, possibly the Filter does not belong to any AggregateFilterContainer (i.e. it is an orphan?)");

            return agg.Catalogue;
        }

        public override void Check(ICheckNotifier notifier)
        {
            base.Check(notifier);

            var checker = new ClonedFilterChecker(this, ClonedFromExtractionFilter_ID, Repository);
            checker.Check(notifier);
        }

        public void MakeIntoAnOrphan()
        {
            FilterContainer_ID = null;
            SaveToDatabase();
        }

        public AggregateConfiguration GetAggregate()
        {
            if (FilterContainer_ID == null)
                return null;

            var container = Repository.GetObjectByID<AggregateFilterContainer>(FilterContainer_ID.Value);
            return container.GetAggregate();
        }
    }
}
