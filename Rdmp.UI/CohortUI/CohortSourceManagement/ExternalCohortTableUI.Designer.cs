﻿using Rdmp.UI.SimpleControls;

namespace Rdmp.UI.CohortUI.CohortSourceManagement
{
    partial class ExternalCohortTableUI
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
            this.tbID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTableName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPrivateIdentifierField = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbReleaseIdentifierField = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbDefinitionTableForeignKeyField = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbDefinitionTableName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.serverDatabaseTableSelector1 = new ServerDatabaseTableSelector();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbID
            // 
            this.tbID.Location = new System.Drawing.Point(45, 3);
            this.tbID.Name = "tbID";
            this.tbID.ReadOnly = true;
            this.tbID.Size = new System.Drawing.Size(100, 20);
            this.tbID.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ID";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(45, 29);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(284, 20);
            this.tbName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(75, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "(How you want to refer to this cohort source)";
            // 
            // tbTableName
            // 
            this.tbTableName.Location = new System.Drawing.Point(136, 30);
            this.tbTableName.Name = "tbTableName";
            this.tbTableName.Size = new System.Drawing.Size(284, 20);
            this.tbTableName.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Cohort Table Name";
            // 
            // tbPrivateIdentifierField
            // 
            this.tbPrivateIdentifierField.Location = new System.Drawing.Point(137, 56);
            this.tbPrivateIdentifierField.Name = "tbPrivateIdentifierField";
            this.tbPrivateIdentifierField.Size = new System.Drawing.Size(284, 20);
            this.tbPrivateIdentifierField.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Private Identifier Field";
            // 
            // tbReleaseIdentifierField
            // 
            this.tbReleaseIdentifierField.Location = new System.Drawing.Point(137, 97);
            this.tbReleaseIdentifierField.Name = "tbReleaseIdentifierField";
            this.tbReleaseIdentifierField.Size = new System.Drawing.Size(284, 20);
            this.tbReleaseIdentifierField.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Release Identifier Field";
            // 
            // tbDefinitionTableForeignKeyField
            // 
            this.tbDefinitionTableForeignKeyField.Location = new System.Drawing.Point(192, 137);
            this.tbDefinitionTableForeignKeyField.Name = "tbDefinitionTableForeignKeyField";
            this.tbDefinitionTableForeignKeyField.Size = new System.Drawing.Size(284, 20);
            this.tbDefinitionTableForeignKeyField.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 140);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(165, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Definition Table Foreign Key Field";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tbTableName);
            this.groupBox1.Controls.Add(this.tbPrivateIdentifierField);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbReleaseIdentifierField);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbDefinitionTableForeignKeyField);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(7, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(892, 185);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "The Cohort Table (Where you store patient identifiers of cohorts you intend to re" +
    "lease data for)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(158, 160);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(367, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "(Column containing a value which links it to the other 2 tables recorded here)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(90, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(433, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "(Release identifiers which will be substituted 1 for 1 in replacement of the priv" +
    "ate identifiers)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(162, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(244, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "(Private Patient Identifiers that will not be released)";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.tbDefinitionTableName);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Location = new System.Drawing.Point(10, 380);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(889, 50);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cohort Definition Table (Where you store the names and descriptions of all your c" +
    "ohorts, must have a column called id which links against the \'Definition Table F" +
    "oreign Key Field\')";
            // 
            // tbDefinitionTableName
            // 
            this.tbDefinitionTableName.Location = new System.Drawing.Point(135, 19);
            this.tbDefinitionTableName.Name = "tbDefinitionTableName";
            this.tbDefinitionTableName.Size = new System.Drawing.Size(284, 20);
            this.tbDefinitionTableName.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(17, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Definition Table Name";
            // 
            // serverDatabaseTableSelector1
            // 
            this.serverDatabaseTableSelector1.AllowTableValuedFunctionSelection = false;
            this.serverDatabaseTableSelector1.AutoSize = true;
            this.serverDatabaseTableSelector1.Database = "";
            this.serverDatabaseTableSelector1.DatabaseType = FAnsi.DatabaseType.MicrosoftSQLServer;
            this.serverDatabaseTableSelector1.Location = new System.Drawing.Point(7, 68);
            this.serverDatabaseTableSelector1.Name = "serverDatabaseTableSelector1";
            this.serverDatabaseTableSelector1.Password = "";
            this.serverDatabaseTableSelector1.Server = "";
            this.serverDatabaseTableSelector1.Size = new System.Drawing.Size(623, 181);
            this.serverDatabaseTableSelector1.TabIndex = 10;
            this.serverDatabaseTableSelector1.Username = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbID);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.tbName);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.serverDatabaseTableSelector1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(902, 446);
            this.panel1.TabIndex = 11;
            // 
            // ExternalCohortTableUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ExternalCohortTableUI";
            this.Size = new System.Drawing.Size(902, 446);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTableName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbPrivateIdentifierField;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbReleaseIdentifierField;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbDefinitionTableForeignKeyField;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbDefinitionTableName;
        private System.Windows.Forms.Label label14;
        private ServerDatabaseTableSelector serverDatabaseTableSelector1;
        private System.Windows.Forms.Panel panel1;
    }
}
