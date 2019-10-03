﻿using System;
using System.Drawing;
using System.IO;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.Repositories;
using ReusableLibraryCode.Icons.IconProvision;

namespace Rdmp.Core.CommandExecution
{
    class SayHello:BasicCommandExecution,IAtomicCommand
    {
        public SayHello(IRDMPPlatformRepositoryServiceLocator locator, DirectoryInfo dir)
        {
            
        }
        public override void Execute()
        {
            base.Execute();

            Console.WriteLine("hi");
        }

        public Image GetImage(IIconProvider iconProvider)
        {
            return null;
        }
    }
}
