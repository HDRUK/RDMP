// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using System.Windows.Forms;
using CatalogueManager.CommandExecution.AtomicCommands;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using DataExportLibrary.Data.DataTables;
using ReusableLibraryCode.CommandExecution.AtomicCommands;
using ReusableLibraryCode.Icons.IconProvision;

namespace DataExportManager.CommandExecution.AtomicCommands
{
    public class ExecuteCommandSetProjectExtractionDirectory : BasicUICommandExecution,IAtomicCommand
    {
        private readonly Project _project;

        public ExecuteCommandSetProjectExtractionDirectory(IActivateItems activator, Project project) : base(activator)
        {
            _project = project;
        }

        public override string GetCommandHelp()
        {
            return "Change the location on disk where extracted artifacts are put when you run extraction configurations of this project";
        }

        public override void Execute()
        {
            base.Execute();

            using (var ofd = new FolderBrowserDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _project.ExtractionDirectory = ofd.SelectedPath;
                    _project.SaveToDatabase();
                    Publish(_project);
                }
            }

        }

        public override Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.ExtractionDirectoryNode,OverlayKind.Edit);
        }
    }
}