// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Windows.Forms;
using Rdmp.Core.Curation.Data.Aggregation;
using Rdmp.Core.DataExport.Data;
using Rdmp.Core.Icons.IconProvision;
using Rdmp.UI.CommandExecution.AtomicCommands;
using Rdmp.UI.ProjectUI.Graphs;
using Rdmp.UI.SimpleDialogs;


namespace Rdmp.UI.Menus
{
    class SelectedDataSetsMenu : RDMPContextMenuStrip
    {
        private readonly SelectedDataSets _selectedDataSet;
        private IExtractionConfiguration _extractionConfiguration;

        public SelectedDataSetsMenu(RDMPContextMenuStripArgs args, SelectedDataSets selectedDataSet): base(args, selectedDataSet)
        {
            _selectedDataSet = selectedDataSet;
            _extractionConfiguration = _selectedDataSet.ExtractionConfiguration;

            Add(new ExecuteCommandExecuteExtractionConfiguration(_activator, selectedDataSet));

            Add(new ExecuteCommandRelease(_activator).SetTarget(selectedDataSet));

            /////////////////// Extraction Graphs //////////////////////////////
            var graphs = new ToolStripMenuItem("View Extraction Graphs", CatalogueIcons.Graph);
            
            var cata = selectedDataSet.ExtractableDataSet.Catalogue;
            
            // If the Catalogue has been deleted, don't build Catalogue specific menu items
            if(cata == null)
                return;

            var availableGraphs = cata.AggregateConfigurations.Where(a => !a.IsCohortIdentificationAggregate).ToArray();

            foreach (AggregateConfiguration ac in availableGraphs)
                graphs.DropDownItems.Add(ac.ToString(), CatalogueIcons.Graph, (s, e) => GenerateExtractionGraphs(ac));

            if(availableGraphs.Length > 1)
                graphs.DropDownItems.Add("All", null, (s,e)=>GenerateExtractionGraphs(availableGraphs));

            //must have graphs available and must have a cohort
            graphs.Enabled = graphs.DropDownItems.Count > 0 && _extractionConfiguration.Cohort_ID != null;
            Items.Add(graphs);
            ////////////////////////////////////////////////////////////////////
            
            
            Add(new ExecuteCommandViewThenVsNowSql(_activator, selectedDataSet));

            Add(new ExecuteCommandOpenExtractionDirectory(_activator, selectedDataSet));
        }

        private void GenerateExtractionGraphs(params AggregateConfiguration[] graphsToExecute)
        {
            try
            {
                foreach (AggregateConfiguration graph in graphsToExecute)
                {
                    var args = new ExtractionAggregateGraphObjectCollection(_selectedDataSet, graph);
                    new ExecuteCommandExecuteExtractionAggregateGraph(_activator,args).Execute();
                }

                
            }
            catch (Exception exception)
            {
                ExceptionViewer.Show(exception);
            }
        }
    }
}