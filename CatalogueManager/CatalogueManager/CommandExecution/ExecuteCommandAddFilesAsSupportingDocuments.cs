﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CatalogueLibrary.Data;
using CatalogueLibrary.Repositories;
using CatalogueManager.CommandExecution.AtomicCommands;
using CatalogueManager.ItemActivation;
using CatalogueManager.Refreshing;
using CatalogueManager.Copying;
using CatalogueManager.Copying.Commands;
using ReusableLibraryCode.CommandExecution;

namespace CatalogueManager.CommandExecution
{
    public class ExecuteCommandAddFilesAsSupportingDocuments : BasicUICommandExecution
    {
        private readonly FileCollectionCommand _fileCollectionCommand;
        private readonly Catalogue _targetCatalogue;

        public ExecuteCommandAddFilesAsSupportingDocuments(IActivateItems activator, FileCollectionCommand fileCollectionCommand, Catalogue targetCatalogue) : base(activator)
        {
            _fileCollectionCommand = fileCollectionCommand;
            _targetCatalogue = targetCatalogue;
            var allExisting = targetCatalogue.GetAllSupportingDocuments(FetchOptions.AllGlobalsAndAllLocals);

            foreach (var doc in allExisting)
            {
                string filename = doc.GetFileName();
                
                if(filename == null)
                    continue;

                var collisions = _fileCollectionCommand.Files.FirstOrDefault(f => f.Name.Equals(filename));
                
                if(collisions != null)
                    SetImpossible("File '" + collisions.Name +"' is already a SupportingDocument (ID=" + doc.ID + " - '"+doc.Name+"')");
            }
        }

        public override void Execute()
        {
            base.Execute();

            foreach (var f in _fileCollectionCommand.Files)
            {
                var doc = new SupportingDocument((ICatalogueRepository)_targetCatalogue.Repository, _targetCatalogue, f.Name);
                doc.URL = new Uri(f.FullName); 
                doc.SaveToDatabase();
            }

            Publish(_targetCatalogue);
        }
    }
}