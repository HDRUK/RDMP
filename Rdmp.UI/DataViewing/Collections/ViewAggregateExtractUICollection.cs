// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using FAnsi.Discovery.QuerySyntax;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Curation.Data.Aggregation;
using Rdmp.Core.Curation.Data.Dashboarding;
using Rdmp.Core.Providers;
using Rdmp.Core.QueryBuilding;
using Rdmp.UI.AutoComplete;
using ReusableLibraryCode.DataAccess;

namespace Rdmp.UI.DataViewing.Collections
{
    /// <summary>
    /// Builds a query based on a <see cref="AggregateConfiguration"/> (either a sample of data in a graph or matching a cohort
    /// (<see cref="Rdmp.Core.Curation.Data.Aggregation.AggregateConfiguration.IsCohortIdentificationAggregate"/>)
    /// </summary>
    public class ViewAggregateExtractUICollection : PersistableObjectCollection,IViewSQLAndResultsCollection
    {
        public bool UseQueryCache { get; set; }

        public ViewAggregateExtractUICollection()
        {
        }

        public ViewAggregateExtractUICollection(AggregateConfiguration config):this()
        {
            DatabaseObjects.Add(config);
        }

        public IEnumerable<DatabaseEntity> GetToolStripObjects()
        {
            if (UseQueryCache)
            {
                var cache = GetCacheServer();
                if (cache != null)
                   yield return cache;
            }
        }
        
        private ExternalDatabaseServer GetCacheServer()
        {
            var cic = AggregateConfiguration.GetCohortIdentificationConfigurationIfAny();

            if (cic != null && cic.QueryCachingServer_ID != null)
                return cic.QueryCachingServer;

            return null;
        }


        public IDataAccessPoint GetDataAccessPoint()
        {
            var dim = AggregateConfiguration.AggregateDimensions.FirstOrDefault();

            //the aggregate has no dimensions
            if (dim == null)
            {
                var table = AggregateConfiguration.ForcedJoins.FirstOrDefault();
                if(table == null)
                    throw new Exception("AggregateConfiguration '" + AggregateConfiguration +"' has no AggregateDimensions and no TableInfo forced joins, we do not know where/what table to run the query on");

                return table;
            }

            return dim.ColumnInfo.TableInfo;
        }

        public string GetSql()
        {
            string sql = "";
            var ac = AggregateConfiguration;

            if (ac.IsCohortIdentificationAggregate)
            {
                var cic = ac.GetCohortIdentificationConfigurationIfAny();
                var globals = cic.GetAllParameters();

                var builder = new CohortQueryBuilder(ac, globals,null);
                
                if(UseQueryCache)
                    builder.CacheServer = GetCacheServer();

                sql = builder.GetDatasetSampleSQL(100);
            }
            else
            {
                var builder = ac.GetQueryBuilder();
                sql = builder.SQL;
            }

            return sql;
        }

        public string GetTabName()
        {
            return "View Top 100 " + AggregateConfiguration;
        }

        public void AdjustAutocomplete(AutoCompleteProvider autoComplete)
        {
            if(AggregateConfiguration != null)
                autoComplete.Add(AggregateConfiguration);
        }

        AggregateConfiguration AggregateConfiguration { get
        {
            return DatabaseObjects.OfType<AggregateConfiguration>().SingleOrDefault();
        } }

        public IQuerySyntaxHelper GetQuerySyntaxHelper()
        {
            var a = AggregateConfiguration;
            return a != null?a.GetQuerySyntaxHelper():null;
        }
    }
}
