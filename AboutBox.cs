﻿//-------------------------------------------------------------------
// Copyright � 2018 Kindel Systems, LLC
// http://www.kindel.com
// charlie@kindel.com
// 
// Published under the MIT License.
// Source control on SourceForge 
//    http://sourceforge.net/projects/mcecontroller/
//-------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Forms;
using MCEControl.Properties;

namespace MCEControl {
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class AboutBox : Form {
        private Label _labelTitle;
        private Button _buttonOk;
        private LinkLabel _linkLabelMceController;
        private LinkLabel _linkLabelKindelSystems;
        private Label _labelSummary;
        private PictureBox _pictureBox1;
        private Label labelVersion;
        private Label _label1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        public AboutBox() {
            //
            // Required for Windows Form Designer support
            //
            // https://www.sgrottel.de/?p=1581&lang=en
            Font = System.Drawing.SystemFonts.DefaultFont;

            InitializeComponent();

            labelVersion.Text = Resources.MCE_Controller_Version_label + Application.ProductVersion;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this._labelTitle = new System.Windows.Forms.Label();
            this._linkLabelMceController = new System.Windows.Forms.LinkLabel();
            this._buttonOk = new System.Windows.Forms.Button();
            this._linkLabelKindelSystems = new System.Windows.Forms.LinkLabel();
            this._labelSummary = new System.Windows.Forms.Label();
            this._pictureBox1 = new System.Windows.Forms.PictureBox();
            this._label1 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _labelTitle
            // 
            this._labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._labelTitle.Location = new System.Drawing.Point(216, 16);
            this._labelTitle.Name = "_labelTitle";
            this._labelTitle.Size = new System.Drawing.Size(280, 41);
            this._labelTitle.TabIndex = 0;
            this._labelTitle.Text = "MCE Controller";
            // 
            // _linkLabelMceController
            // 
            this._linkLabelMceController.Location = new System.Drawing.Point(216, 144);
            this._linkLabelMceController.Name = "_linkLabelMceController";
            this._linkLabelMceController.Size = new System.Drawing.Size(280, 16);
            this._linkLabelMceController.TabIndex = 3;
            this._linkLabelMceController.TabStop = true;
            this._linkLabelMceController.Tag = "https://github.com/tig/mcec/blob/master/license.md";
            this._linkLabelMceController.Text = "License Agreement";
            this._linkLabelMceController.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelMceControllerLinkClicked);
            // 
            // _buttonOk
            // 
            this._buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonOk.Location = new System.Drawing.Point(416, 184);
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.Size = new System.Drawing.Size(80, 32);
            this._buttonOk.TabIndex = 0;
            this._buttonOk.Text = "OK";
            this._buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // _linkLabelKindelSystems
            // 
            this._linkLabelKindelSystems.AutoSize = true;
            this._linkLabelKindelSystems.Location = new System.Drawing.Point(216, 88);
            this._linkLabelKindelSystems.Name = "_linkLabelKindelSystems";
            this._linkLabelKindelSystems.Size = new System.Drawing.Size(145, 13);
            this._linkLabelKindelSystems.TabIndex = 1;
            this._linkLabelKindelSystems.TabStop = true;
            this._linkLabelKindelSystems.Tag = "http://www.kindel.com";
            this._linkLabelKindelSystems.Text = "© 2019 Kindel Systems, LLC.";
            this._linkLabelKindelSystems.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelCharlieLinkClicked);
            // 
            // _labelSummary
            // 
            this._labelSummary.Location = new System.Drawing.Point(216, 112);
            this._labelSummary.Name = "_labelSummary";
            this._labelSummary.Size = new System.Drawing.Size(272, 32);
            this._labelSummary.TabIndex = 2;
            this._labelSummary.Text = "MCE Controller is distributed as freeware and published as open source under the " +
    "MIT License.";
            // 
            // _pictureBox1
            // 
            this._pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("_pictureBox1.Image")));
            this._pictureBox1.InitialImage = null;
            this._pictureBox1.Location = new System.Drawing.Point(8, 16);
            this._pictureBox1.Name = "_pictureBox1";
            this._pictureBox1.Size = new System.Drawing.Size(192, 160);
            this._pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pictureBox1.TabIndex = 5;
            this._pictureBox1.TabStop = false;
            // 
            // _label1
            // 
            this._label1.AutoSize = true;
            this._label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._label1.Location = new System.Drawing.Point(56, 176);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(96, 12);
            this._label1.TabIndex = 7;
            this._label1.Text = "Icon by Guillen Design";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(216, 64);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(78, 13);
            this.labelVersion.TabIndex = 7;
            this.labelVersion.Text = "Version a.b.c.d";
            // 
            // AboutBox
            // 
            this.AcceptButton = this._buttonOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this._buttonOk;
            this.ClientSize = new System.Drawing.Size(507, 228);
            this.ControlBox = false;
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this._label1);
            this.Controls.Add(this._pictureBox1);
            this.Controls.Add(this._labelSummary);
            this.Controls.Add(this._linkLabelKindelSystems);
            this.Controls.Add(this._buttonOk);
            this.Controls.Add(this._linkLabelMceController);
            this.Controls.Add(this._labelTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void ButtonOkClick(object sender, EventArgs e) {
            Close();
        }

        private void LinkLabelMceControllerLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(_linkLabelMceController.Tag.ToString());
        }

        private void LinkLabelCharlieLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(_linkLabelKindelSystems.Tag.ToString());
        }
    }
}
