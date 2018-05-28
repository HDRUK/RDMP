using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using CatalogueLibrary.Data;
using CatalogueLibrary.Triggers;
using CatalogueManager.Collections;
using CatalogueManager.ItemActivation;
using CatalogueManager.TestsAndSetup.ServicePropogation;
using DataLoadEngine.Migration;
using DataQualityEngine.Reports;
using RDMPAutomationService.Options;
using ReusableLibraryCode.Progress;
using ReusableUIComponents;

namespace CatalogueManager.DataQualityUIs
{
    /// <summary>
    /// Form for performing Data Quality Engine executions on a chosen Catalogue. Opening the form will trigger a series of pre run checks and once these have successfully completed you
    /// can then begin the execution by clicking the Start Execution button.
    /// 
    /// <para>While the execution is happening you can view the progress on the right hand side.</para>
    /// 
    /// <para>To view the results of the execution Right Click on the relevant catalogue and select View DQE Results.</para>
    /// </summary>
    public partial class DQEExecutionControl : DQEExecutionControl_Design
    {
        private Catalogue _catalogue;

        public DQEExecutionControl()
        {
            InitializeComponent();
            
            AssociatedCollection = RDMPCollection.Catalogue;
            checkAndExecuteUI1.CommandGetter += CommandGetter;
        }

        private RDMPCommandLineOptions CommandGetter(CommandLineActivity commandLineActivity)
        {
            return new DqeOptions() { Catalogue = _catalogue.ID, Command = commandLineActivity };
        }

        public override void SetDatabaseObject(IActivateItems activator, Catalogue databaseObject)
        {
            base.SetDatabaseObject(activator, databaseObject);
            _catalogue = databaseObject;
            checkAndExecuteUI1.SetItemActivator(activator);
        }

        public override string GetTabName()
        {
            return "DQE Execution:" + base.GetTabName();
        }
    }

    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<DQEExecutionControl_Design, UserControl>))]
    public abstract class DQEExecutionControl_Design : RDMPSingleDatabaseObjectControl<Catalogue>
    {
    }
}
