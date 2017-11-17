﻿using System.Drawing;
using CatalogueLibrary.Data.Remoting;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using ReusableUIComponents.CommandExecution.AtomicCommands;
using ReusableUIComponents.Icons.IconProvision;

namespace CatalogueManager.CommandExecution.AtomicCommands
{
    public class ExecuteCommandCreateNewRemoteRDMP : BasicUICommandExecution, IAtomicCommand
    {
        public ExecuteCommandCreateNewRemoteRDMP(IActivateItems activator) : base(activator)
        {
        }

        public override void Execute()
        {
            base.Execute();
            var remote = new RemoteRDMP(Activator.RepositoryLocator.CatalogueRepository);
            Publish(remote);
        }

        public Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.RemoteRDMP, OverlayKind.Add);
        }
    }
}