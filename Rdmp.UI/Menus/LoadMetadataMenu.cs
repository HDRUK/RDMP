// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Windows.Forms;
using MapsDirectlyToDatabaseTable;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.Curation.Data.DataLoad;
using Rdmp.Core.Icons.IconProvision;
using Rdmp.UI.CommandExecution.AtomicCommands;
using Rdmp.UI.CommandExecution.AtomicCommands.Sharing;
using Rdmp.UI.Icons.IconProvision;
using ReusableLibraryCode.Icons.IconProvision;

namespace Rdmp.UI.Menus
{
    [System.ComponentModel.DesignerCategory("")]
    class LoadMetadataMenu:RDMPContextMenuStrip
    {
        public LoadMetadataMenu(RDMPContextMenuStripArgs args, LoadMetadata loadMetadata) : base(args, loadMetadata)
        {
            Add(new ExecuteCommandViewLoadMetadataLogs(_activator).SetTarget(loadMetadata));

            Add(new ExecuteCommandViewLoadDiagram(_activator,loadMetadata));

            Add(new ExecuteCommandEditLoadMetadataDescription(_activator, loadMetadata));

            Add(new ExecuteCommandExportObjectsToFileUI(_activator, new IMapsDirectlyToDatabaseTable[] {loadMetadata}));

            Items.Add(new ToolStripSeparator());

            Add(new ExecuteCommandOverrideRawServer(_activator, loadMetadata));
            Add(new ExecuteCommandCreateNewLoadMetadata(_activator));
            
            ReBrandActivateAs("Check and Execute",RDMPConcept.LoadMetadata,OverlayKind.Execute);
        }
    }
}
