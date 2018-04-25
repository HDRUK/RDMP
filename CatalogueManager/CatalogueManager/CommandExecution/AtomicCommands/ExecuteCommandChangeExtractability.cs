﻿using System.Drawing;
using System.Linq;
using CatalogueLibrary.Data;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using DataExportLibrary.Data.DataTables;
using DataExportLibrary.Providers;
using ReusableLibraryCode.CommandExecution.AtomicCommands;
using ReusableLibraryCode.Icons.IconProvision;

namespace CatalogueManager.CommandExecution.AtomicCommands
{
    public class ExecuteCommandChangeExtractability:BasicUICommandExecution,IAtomicCommand
    {
        private readonly Catalogue _catalogue;
        private bool _isExtractable;

        public ExecuteCommandChangeExtractability(IActivateItems activator, Catalogue catalogue) : base(activator)
        {
            _catalogue = catalogue;
            if(!catalogue.GetIsExtractabilityKnown())
            {
                SetImpossible("We don't know whether Catalogue is extractable or not (possibly no Data Export database is available)");
                return;
            }

            _isExtractable = catalogue.GetIsExtractable();
        }

        public override string GetCommandName()
        {
            return _isExtractable?"Mark Not Extractable":"Mark Extractable";
        }

        public Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.ExtractableDataSet, _isExtractable?OverlayKind.Delete:OverlayKind.Add);
        }

        public override void Execute()
        {
            base.Execute();

            if (_isExtractable)
            {
                var extractabilityRecord = ((DataExportChildProvider) Activator.CoreChildProvider).ExtractableDataSets.Single(ds => ds.Catalogue_ID == _catalogue.ID);
                extractabilityRecord.DeleteInDatabase();
                Publish(_catalogue);
            }
            else
            {
                 new ExtractableDataSet(Activator.RepositoryLocator.DataExportRepository, _catalogue);
                Publish(_catalogue);

            }
        }
    }
}
