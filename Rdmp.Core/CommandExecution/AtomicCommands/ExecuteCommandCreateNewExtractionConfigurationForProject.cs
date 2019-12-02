// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using Rdmp.Core.DataExport.Data;
using Rdmp.Core.Icons.IconProvision;
using ReusableLibraryCode.Icons.IconProvision;

namespace Rdmp.Core.CommandExecution.AtomicCommands
{
    public class ExecuteCommandCreateNewExtractionConfigurationForProject:BasicCommandExecution,IAtomicCommand
    {
        private readonly Project _project;

        public ExecuteCommandCreateNewExtractionConfigurationForProject(IBasicActivateItems activator,Project project) : base(activator)
        {
            _project = project;
        }

        public override string GetCommandHelp()
        {
            return "Starts a new extraction for the project containing one or more datasets linked against a given cohort";
        }

        public override Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.ExtractionConfiguration, OverlayKind.Add);
        }

        public override void Execute()
        {
            base.Execute();

            var newConfig = new ExtractionConfiguration(BasicActivator.RepositoryLocator.DataExportRepository, _project);

            //refresh the project
            Publish(_project);
            Activate(newConfig);
        }
    }
}
