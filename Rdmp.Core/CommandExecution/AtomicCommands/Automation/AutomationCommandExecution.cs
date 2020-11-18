// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using CommandLine;
using MapsDirectlyToDatabaseTable;
using Rdmp.Core.CommandLine.Options;
using Rdmp.Core.Startup;

namespace Rdmp.Core.CommandExecution.AtomicCommands.Automation
{
    public abstract class AutomationCommandExecution : BasicCommandExecution
    {
        protected readonly Func<RDMPCommandLineOptions> CommandGetter;
        public readonly string AutomationServiceExecutable = EnvironmentInfo.IsLinux ? "rdmp" : "rdmp.exe";

        private TableRepository _cataTableRepo;
        private TableRepository _dataExportTableRepo;


        protected AutomationCommandExecution(IBasicActivateItems activator, Func<RDMPCommandLineOptions> commandGetter) : base(activator)
        {
            CommandGetter = commandGetter;

            _cataTableRepo = activator.RepositoryLocator.CatalogueRepository as TableRepository;
            _dataExportTableRepo = activator.RepositoryLocator.DataExportRepository as TableRepository;

            if (_cataTableRepo == null || _dataExportTableRepo == null)
                SetImpossible("Current repositories are not TableRepository");
        }

        protected string GetCommandText()
        {
            Parser p = new Parser();
            var options = CommandGetter();

            PopulateConnectionStringOptions(options);

            return AutomationServiceExecutable + " " + p.FormatCommandLine(options);
        }

        private void PopulateConnectionStringOptions(RDMPCommandLineOptions options)
        {
            if (BasicActivator == null)
                return;

            if (string.IsNullOrWhiteSpace(options.CatalogueConnectionString))
                options.CatalogueConnectionString = _cataTableRepo.ConnectionStringBuilder.ConnectionString;

            if (string.IsNullOrWhiteSpace(options.DataExportConnectionString))
                options.DataExportConnectionString = _dataExportTableRepo.ConnectionStringBuilder.ConnectionString;
        }
    }
}