// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.Curation.Data.Governance;
using Rdmp.UI.Icons.IconProvision;
using Rdmp.UI.ItemActivation;
using ReusableLibraryCode.Icons.IconProvision;

namespace Rdmp.UI.CommandExecution.AtomicCommands
{
    class ExecuteCommandCreateNewGovernancePeriod:BasicUICommandExecution,IAtomicCommand
    {
        public ExecuteCommandCreateNewGovernancePeriod(IActivateItems activator) : base(activator)
        {
        }

        public override Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.GovernancePeriod, OverlayKind.Add);
        }

        public override void Execute()
        {
            base.Execute();

            var period = new GovernancePeriod(Activator.RepositoryLocator.CatalogueRepository);
            Publish(period);
            Emphasise(period);
            Activate(period);
        }
    }
}
