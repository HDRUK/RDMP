// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.DataExport.Data;
using Rdmp.Core.Icons.IconProvision;
using Rdmp.Core.Providers;
using Rdmp.UI.CommandExecution.AtomicCommands;
using Rdmp.UI.ProjectUI;
using ReusableLibraryCode;
using ReusableLibraryCode.Icons.IconProvision;

namespace Rdmp.UI.Menus
{
    [System.ComponentModel.DesignerCategory("")]
    class ExtractionConfigurationMenu:RDMPContextMenuStrip
    {
        public ExtractionConfigurationMenu(RDMPContextMenuStripArgs args, ExtractionConfiguration extractionConfiguration)
            : base( args,extractionConfiguration)
        {
            Items.Add("Edit", null, (s, e) => _activator.Activate<ExtractionConfigurationUI, ExtractionConfiguration>(extractionConfiguration));
                                    
            ///////////////////Change Cohorts//////////////
            
            Add(new ExecuteCommandRelease(_activator).SetTarget(extractionConfiguration));

            Add(new ExecuteCommandChooseCohort(_activator, extractionConfiguration));

            Add(new ExecuteCommandViewLogs(_activator,extractionConfiguration));
            
            /////////////////Add Datasets/////////////
            Add(new ExecuteCommandAddDatasetsToConfiguration(_activator,extractionConfiguration));

            Add(new ExecuteCommandAddPackageToConfiguration(_activator, extractionConfiguration));
            
            Add(new ExecuteCommandGenerateReleaseDocument(_activator, extractionConfiguration));          
            
            Add(new ExecuteCommandViewSqlParameters(_activator,extractionConfiguration));

            if (extractionConfiguration.IsReleased)
                Add(new ExecuteCommandUnfreezeExtractionConfiguration(_activator, extractionConfiguration));
            else
                Add(new ExecuteCommandFreezeExtractionConfiguration(_activator, extractionConfiguration));

            Add(new ExecuteCommandCloneExtractionConfiguration(_activator, extractionConfiguration));

            Add(new ExecuteCommandRefreshExtractionConfigurationsCohort(_activator, extractionConfiguration));

            ReBrandActivateAs("Extract...", RDMPConcept.ExtractionConfiguration, OverlayKind.Execute);

            
        }

    }
}
