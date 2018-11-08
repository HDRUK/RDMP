﻿using ResearchDataManagementPlatform.Menus;
using WeifenLuo.WinFormsUI.Docking;

namespace ResearchDataManagementPlatform
{
    partial class RDMPMainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RDMPMainForm));
            this._rdmpTopMenuStrip1 = new ResearchDataManagementPlatform.Menus.RDMPTopMenuStrip();
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.SuspendLayout();
            // 
            // _rdmpTopMenuStrip1
            // 
            this._rdmpTopMenuStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this._rdmpTopMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this._rdmpTopMenuStrip1.Name = "_rdmpTopMenuStrip1";
            this._rdmpTopMenuStrip1.Size = new System.Drawing.Size(1300, 53);
            this._rdmpTopMenuStrip1.TabIndex = 3;
            // 
            // dockPanel1
            // 
            this.dockPanel1.AutoSize = true;
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 53);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(1300, 732);
            this.dockPanel1.TabIndex = 4;
            this.dockPanel1.Text = "label1";
            // 
            // RDMPMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 785);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this._rdmpTopMenuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RDMPMainForm";
            this.Text = "RDMPMainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RDMPMainForm_FormClosed);
            this.Load += new System.EventHandler(this.RDMPMainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RDMPTopMenuStrip _rdmpTopMenuStrip1;
        private DockPanel dockPanel1;
    }
}

