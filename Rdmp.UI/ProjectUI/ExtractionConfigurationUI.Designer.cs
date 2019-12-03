﻿using Rdmp.UI.ChecksUI;
using Rdmp.UI.LocationsMenu.Ticketing;

namespace Rdmp.UI.ProjectUI
{
    partial class ExtractionConfigurationUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label4 = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.tbCreated = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tcRelease = new TicketingControlUI();
            this.tcRequest = new TicketingControlUI();
            this.tbID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pChooseExtractionPipeline = new System.Windows.Forms.Panel();
            this.gbCohortRefreshing = new System.Windows.Forms.GroupBox();
            this.ragSmiley1Refresh = new RAGSmiley();
            this.pChooseCohortRefreshPipeline = new System.Windows.Forms.Panel();
            this.btnClearCic = new System.Windows.Forms.Button();
            this.pbCic = new System.Windows.Forms.PictureBox();
            this.cbxCohortIdentificationConfiguration = new SuggestComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.gbCohortRefreshing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCic)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(133, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Username:";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(197, 6);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.ReadOnly = true;
            this.tbUsername.Size = new System.Drawing.Size(158, 20);
            this.tbUsername.TabIndex = 7;
            // 
            // tbCreated
            // 
            this.tbCreated.Location = new System.Drawing.Point(408, 5);
            this.tbCreated.Name = "tbCreated";
            this.tbCreated.ReadOnly = true;
            this.tbCreated.Size = new System.Drawing.Size(158, 20);
            this.tbCreated.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(360, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Created:";
            // 
            // tcRelease
            // 
            this.tcRelease.AutoSize = true;
            this.tcRelease.Location = new System.Drawing.Point(313, 42);
            this.tcRelease.Name = "tcRelease";
            this.tcRelease.Size = new System.Drawing.Size(300, 52);
            this.tcRelease.TabIndex = 46;
            this.tcRelease.TicketText = "";
            // 
            // tcRequest
            // 
            this.tcRequest.AutoSize = true;
            this.tcRequest.Location = new System.Drawing.Point(7, 42);
            this.tcRequest.Name = "tcRequest";
            this.tcRequest.Size = new System.Drawing.Size(300, 52);
            this.tcRequest.TabIndex = 46;
            this.tcRequest.TicketText = "";
            // 
            // tbID
            // 
            this.tbID.Location = new System.Drawing.Point(31, 6);
            this.tbID.Name = "tbID";
            this.tbID.ReadOnly = true;
            this.tbID.Size = new System.Drawing.Size(96, 20);
            this.tbID.TabIndex = 48;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "ID:";
            // 
            // pChooseExtractionPipeline
            // 
            this.pChooseExtractionPipeline.Location = new System.Drawing.Point(107, 360);
            this.pChooseExtractionPipeline.Name = "pChooseExtractionPipeline";
            this.pChooseExtractionPipeline.Size = new System.Drawing.Size(726, 37);
            this.pChooseExtractionPipeline.TabIndex = 49;
            // 
            // gbCohortRefreshing
            // 
            this.gbCohortRefreshing.Controls.Add(this.ragSmiley1Refresh);
            this.gbCohortRefreshing.Controls.Add(this.pChooseCohortRefreshPipeline);
            this.gbCohortRefreshing.Controls.Add(this.btnClearCic);
            this.gbCohortRefreshing.Controls.Add(this.pbCic);
            this.gbCohortRefreshing.Controls.Add(this.cbxCohortIdentificationConfiguration);
            this.gbCohortRefreshing.Controls.Add(this.label1);
            this.gbCohortRefreshing.Location = new System.Drawing.Point(7, 250);
            this.gbCohortRefreshing.Name = "gbCohortRefreshing";
            this.gbCohortRefreshing.Size = new System.Drawing.Size(743, 104);
            this.gbCohortRefreshing.TabIndex = 50;
            this.gbCohortRefreshing.TabStop = false;
            this.gbCohortRefreshing.Text = "Cohort Refreshing";
            // 
            // ragSmiley1Refresh
            // 
            this.ragSmiley1Refresh.AlwaysShowHandCursor = false;
            this.ragSmiley1Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ragSmiley1Refresh.BackColor = System.Drawing.Color.Transparent;
            this.ragSmiley1Refresh.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ragSmiley1Refresh.Location = new System.Drawing.Point(658, 44);
            this.ragSmiley1Refresh.Name = "ragSmiley1Refresh";
            this.ragSmiley1Refresh.Size = new System.Drawing.Size(25, 25);
            this.ragSmiley1Refresh.TabIndex = 52;
            // 
            // pChooseCohortRefreshPipeline
            // 
            this.pChooseCohortRefreshPipeline.Location = new System.Drawing.Point(17, 55);
            this.pChooseCohortRefreshPipeline.Name = "pChooseCohortRefreshPipeline";
            this.pChooseCohortRefreshPipeline.Size = new System.Drawing.Size(635, 38);
            this.pChooseCohortRefreshPipeline.TabIndex = 50;
            // 
            // btnClearCic
            // 
            this.btnClearCic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearCic.Location = new System.Drawing.Point(658, 15);
            this.btnClearCic.Name = "btnClearCic";
            this.btnClearCic.Size = new System.Drawing.Size(41, 23);
            this.btnClearCic.TabIndex = 4;
            this.btnClearCic.Text = "clear";
            this.btnClearCic.UseVisualStyleBackColor = true;
            this.btnClearCic.Click += new System.EventHandler(this.btnClearCic_Click);
            // 
            // pbCic
            // 
            this.pbCic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCic.Location = new System.Drawing.Point(705, 17);
            this.pbCic.Name = "pbCic";
            this.pbCic.Size = new System.Drawing.Size(21, 21);
            this.pbCic.TabIndex = 3;
            this.pbCic.TabStop = false;
            // 
            // cbxCohortIdentificationConfiguration
            // 
            this.cbxCohortIdentificationConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxCohortIdentificationConfiguration.FilterRule = null;
            this.cbxCohortIdentificationConfiguration.FormattingEnabled = true;
            this.cbxCohortIdentificationConfiguration.Location = new System.Drawing.Point(227, 17);
            this.cbxCohortIdentificationConfiguration.Name = "cbxCohortIdentificationConfiguration";
            this.cbxCohortIdentificationConfiguration.PropertySelector = null;
            this.cbxCohortIdentificationConfiguration.Size = new System.Drawing.Size(425, 21);
            this.cbxCohortIdentificationConfiguration.SuggestBoxHeight = 96;
            this.cbxCohortIdentificationConfiguration.SuggestListOrderRule = null;
            this.cbxCohortIdentificationConfiguration.TabIndex = 1;
            this.cbxCohortIdentificationConfiguration.SelectedIndexChanged += new System.EventHandler(this.cbxCohortIdentificationConfiguration_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cohort Identification Configuration (Query):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "Description:";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(6, 113);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(744, 105);
            this.tbDescription.TabIndex = 53;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbDescription);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tbUsername);
            this.panel1.Controls.Add(this.gbCohortRefreshing);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.pChooseExtractionPipeline);
            this.panel1.Controls.Add(this.tbCreated);
            this.panel1.Controls.Add(this.tbID);
            this.panel1.Controls.Add(this.tcRequest);
            this.panel1.Controls.Add(this.tcRelease);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(852, 492);
            this.panel1.TabIndex = 54;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 371);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 54;
            this.label3.Text = "Extraction Pipeline:";
            // 
            // ExtractionConfigurationUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panel1);
            this.Name = "ExtractionConfigurationUI";
            this.Size = new System.Drawing.Size(852, 492);
            this.gbCohortRefreshing.ResumeLayout(false);
            this.gbCohortRefreshing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCic)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.TextBox tbCreated;
        private System.Windows.Forms.Label label5;
        private TicketingControlUI tcRequest;
        private TicketingControlUI tcRelease;
        private System.Windows.Forms.TextBox tbID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pChooseExtractionPipeline;
        private System.Windows.Forms.GroupBox gbCohortRefreshing;
        private System.Windows.Forms.PictureBox pbCic;
        private SuggestComboBox cbxCohortIdentificationConfiguration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClearCic;
        private System.Windows.Forms.Panel pChooseCohortRefreshPipeline;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDescription;
        private RAGSmiley ragSmiley1Refresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
    }
}
