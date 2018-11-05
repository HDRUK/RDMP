﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.Dashboarding;
using CatalogueManager.Collections;
using CatalogueManager.CommandExecution.AtomicCommands;
using CatalogueManager.DashboardTabs;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.Theme;
using CohortManager.Collections;
using DataExportManager.Collections;
using MapsDirectlyToDatabaseTable;
using ResearchDataManagementPlatform.WindowManagement.HomePane;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.Icons.IconProvision;
using ReusableUIComponents;

namespace ResearchDataManagementPlatform.WindowManagement.TopBar
{
    /// <summary>
    /// Allows you to access the main object collections that make up the RDMP.  These include 
    /// </summary>
    public partial class RDMPTaskBar : UserControl
    {
        private ToolboxWindowManager _manager;
        
        private const string CreateNewDashboard = "<<New Dashboard>>";
        private const string CreateNewLayout = "<<New Layout>>";

        public RDMPTaskBar()
        {
            InitializeComponent();
            BackColorProvider provider = new BackColorProvider();

            btnHome.Image = FamFamFamIcons.application_home;
            btnCatalogues.Image = CatalogueIcons.Catalogue;
            btnCatalogues.BackgroundImage = provider.GetBackgroundImage(btnCatalogues.Size, RDMPCollection.Catalogue);

            btnCohorts.Image = CatalogueIcons.CohortIdentificationConfiguration;
            btnCohorts.BackgroundImage = provider.GetBackgroundImage(btnCohorts.Size, RDMPCollection.Cohort);

            btnSavedCohorts.Image = CatalogueIcons.AllCohortsNode;
            btnSavedCohorts.BackgroundImage = provider.GetBackgroundImage(btnSavedCohorts.Size, RDMPCollection.SavedCohorts);

            btnDataExport.Image = CatalogueIcons.Project;
            btnDataExport.BackgroundImage = provider.GetBackgroundImage(btnDataExport.Size, RDMPCollection.DataExport);

            btnTables.Image = CatalogueIcons.TableInfo;
            btnTables.BackgroundImage = provider.GetBackgroundImage(btnTables.Size, RDMPCollection.Tables);

            btnLoad.Image = CatalogueIcons.LoadMetadata;
            btnLoad.BackgroundImage = provider.GetBackgroundImage(btnLoad.Size, RDMPCollection.DataLoad);
            
            btnFavourites.Image = CatalogueIcons.Favourite;

        }

        public void SetWindowManager(ToolboxWindowManager manager)
        {
            _manager = manager;
            btnDataExport.Enabled = manager.RepositoryLocator.DataExportRepository != null;
            
            ReCreateDropDowns();

            SetupToolTipText();
        }

        private void SetupToolTipText()
        {
            int maxCharactersForButtonTooltips = 200;

            try
            {
                var store = _manager.ContentManager.RepositoryLocator.CatalogueRepository.CommentStore;

                btnHome.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(HomeUI));
                btnCatalogues.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(CatalogueCollectionUI));
                btnCohorts.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(CohortIdentificationCollectionUI));
                btnSavedCohorts.ToolTipText =  store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(SavedCohortsCollectionUI));
                btnDataExport.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(DataExportCollectionUI));
                btnTables.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(TableInfoCollectionUI));
                btnLoad.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(LoadMetadataCollectionUI));
                btnFavourites.ToolTipText = store.GetTypeDocumentationIfExists(maxCharactersForButtonTooltips, typeof(FavouritesCollectionUI)); 

            }
            catch (Exception e)
            {
                _manager.ContentManager.GlobalErrorCheckNotifier.OnCheckPerformed(new CheckEventArgs("Failed to setup tool tips", CheckResult.Fail, e));
            }

        }

        private void ReCreateDropDowns()
        {
            CreateDropDown<DashboardLayout>(cbxDashboards, CreateNewDashboard);
            CreateDropDown<WindowLayout>(cbxLayouts, CreateNewLayout);
        }

        private void CreateDropDown<T>(ToolStripComboBox cbx, string createNewDashboard) where T:IMapsDirectlyToDatabaseTable, INamed
        {
            const int xPaddingForComboText = 10;

            if (cbx.ComboBox == null)
                throw new Exception("Expected combo box!");
            
            cbx.ComboBox.Items.Clear();

            var objects = _manager.RepositoryLocator.CatalogueRepository.GetAllObjects<T>();

            cbx.ComboBox.Items.Add("");

            //minimum size that it will be (same width as the combo box)
            int proposedComboBoxWidth = cbx.Width - xPaddingForComboText;

            foreach (T o in objects)
            {
                //add dropdown item
                cbx.ComboBox.Items.Add(o);

                //will that label be too big to fit in text box? if so expand the max width
                proposedComboBoxWidth = Math.Max(proposedComboBoxWidth, TextRenderer.MeasureText(o.Name, cbx.Font).Width);
            }

            cbx.DropDownWidth = Math.Min(400, proposedComboBoxWidth + xPaddingForComboText);
            cbx.ComboBox.SelectedItem = "";

            cbx.Items.Add(createNewDashboard);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            _manager.CloseAllToolboxes();
            _manager.CloseAllWindows();
            _manager.PopHome();
        }

        private void ToolboxButtonClicked(object sender, EventArgs e)
        {
            RDMPCollection collection = ButtonToEnum(sender);

            if (_manager.IsVisible(collection))
                _manager.Pop(collection);
            else
                _manager.Create(collection);
        }

        private RDMPCollection ButtonToEnum(object button)
        {
            RDMPCollection collectionToToggle;

            if (button == btnCatalogues)
                collectionToToggle = RDMPCollection.Catalogue;
            else
            if (button == btnCohorts)
                collectionToToggle = RDMPCollection.Cohort;
            else
            if (button == btnDataExport)
                collectionToToggle = RDMPCollection.DataExport;
            else
            if (button == btnTables)
                collectionToToggle = RDMPCollection.Tables;
            else
            if (button == btnLoad)
                collectionToToggle = RDMPCollection.DataLoad;
            else if (button == btnSavedCohorts)
                collectionToToggle = RDMPCollection.SavedCohorts;
            else if (button == btnFavourites)
                collectionToToggle = RDMPCollection.Favourites;
            else
                throw new ArgumentOutOfRangeException();

            return collectionToToggle;
        }

        
        private void cbx_DropDownClosed(object sender, EventArgs e)
        {
            var cbx = (ToolStripComboBox)sender;
            var toOpen = cbx.SelectedItem as INamed;

            if (ReferenceEquals(cbx.SelectedItem, CreateNewDashboard))
                AddNewDashboard();

            if (ReferenceEquals(cbx.SelectedItem, CreateNewLayout))
                AddNewLayout();

            if (toOpen != null)
            {
                var cmd = new ExecuteCommandActivate(_manager.ContentManager, toOpen);
                cmd.Execute();
            }

            btnDeleteLayout.Enabled = cbxLayouts.SelectedItem is DashboardLayout;
            btnDeleteDash.Enabled = cbxDashboards.SelectedItem is DashboardLayout;
        }

        private void AddNewLayout()
        {

            string xml = _manager.MainForm.GetCurrentLayoutXml();

            var dialog = new TypeTextOrCancelDialog("Layout Name", "Name", 100, null, false);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var layout = new WindowLayout(_manager.RepositoryLocator.CatalogueRepository, dialog.ResultText,xml);

                var cmd = new ExecuteCommandActivate(_manager.ContentManager, layout);
                cmd.Execute();

                ReCreateDropDowns();
            }
        }

        private void AddNewDashboard()
        {
            var dialog = new TypeTextOrCancelDialog("Dashboard Name", "Name", 100, null, false);
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                var dash = new DashboardLayout(_manager.RepositoryLocator.CatalogueRepository, dialog.ResultText);
                
                var cmd = new ExecuteCommandActivate(_manager.ContentManager, dash);
                cmd.Execute();

                ReCreateDropDowns();
            }
        }

        public void InjectButton(ToolStripButton button)
        {
            toolStrip1.Items.Add(button);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ToolStripComboBox cbx;
            if (sender == btnDeleteDash)
                cbx = cbxDashboards;
            else if (sender == btnDeleteLayout)
                cbx = cbxLayouts;
            else
                throw new Exception("Unexpected sender");

            var d = cbx.SelectedItem as IDeleteable;
            if (d != null)
            {
                _manager.ContentManager.DeleteWithConfirmation(this, d);
                ReCreateDropDowns();
            }
        }

    }
}
