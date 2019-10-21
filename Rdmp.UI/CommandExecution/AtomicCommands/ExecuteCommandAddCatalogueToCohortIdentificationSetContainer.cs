// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using Rdmp.Core.CommandExecution.Combining;
using Rdmp.Core.Curation.Data.Aggregation;
using Rdmp.Core.Curation.Data.Cohort;
using Rdmp.UI.ItemActivation;

namespace Rdmp.UI.CommandExecution.AtomicCommands
{
    public class ExecuteCommandAddCatalogueToCohortIdentificationSetContainer : BasicUICommandExecution
    {
        private readonly CatalogueCombineable _catalogueCombineable;
        private readonly CohortAggregateContainer _targetCohortAggregateContainer;

        private ExecuteCommandAddAggregateConfigurationToCohortIdentificationSetContainer _postImportCommand;

        public bool SkipMandatoryFilterCreation { get; set; }

        public AggregateConfiguration AggregateCreatedIfAny
        {
            get
            {
                if (_postImportCommand == null)
                    return null;

                return _postImportCommand.AggregateCreatedIfAny;
            }
        }

        public ExecuteCommandAddCatalogueToCohortIdentificationSetContainer(IActivateItems activator,CatalogueCombineable catalogueCombineable, CohortAggregateContainer targetCohortAggregateContainer) : base(activator)
        {
            _catalogueCombineable = catalogueCombineable;
            _targetCohortAggregateContainer = targetCohortAggregateContainer;

            if(!catalogueCombineable.ContainsAtLeastOneExtractionIdentifier)
                SetImpossible("Catalogue " + catalogueCombineable.Catalogue + " does not contain any IsExtractionIdentifier columns");
        }

        public override void Execute()
        {
            base.Execute();

            
            var cmd = _catalogueCombineable.GenerateAggregateConfigurationFor(Activator,_targetCohortAggregateContainer,!SkipMandatoryFilterCreation);
            if(cmd != null)
            {
                _postImportCommand = 
                    new ExecuteCommandAddAggregateConfigurationToCohortIdentificationSetContainer(Activator,cmd, _targetCohortAggregateContainer)
                        {DoNotClone = true};
                _postImportCommand.Execute();
            }
        }
    }
}