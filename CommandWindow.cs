﻿//-------------------------------------------------------------------
// Copyright © 2019 Kindel Systems, LLC
// http://www.kindel.com
// charlie@kindel.com
// 
// Published under the MIT License.
// Source on GitHub: https://github.com/tig/mcec  
//-------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32.Security;
using WindowsInput.Native;

namespace MCEControl {
    public partial class CommandWindow : Form {
        private log4net.ILog log4;

        public CommandWindow() {
            log4 = log4net.LogManager.GetLogger("MCEControl");
            InitializeComponent();
        }
        
        private void CommandWindow_Load(object sender, EventArgs e) {
            Icon = MainWindow.Instance.Icon;

            var orderedKeys = MainWindow.Instance.CmdTable.Keys.Cast<string>().OrderBy(c => c);
            foreach (string key in orderedKeys) {
                Command cmd = MainWindow.Instance.CmdTable[key];
                var item = new ListViewItem(cmd.Key);
                Match match = Regex.Match(cmd.GetType().ToString(), @"MCEControl\.([A-za-z]+)Command");
                item.SubItems.Add(match.Groups[1].Value);
                item.SubItems.Add(cmd.ToString());
                listCmds.Items.Add(item);
            }
        }

        private void CommandWindow_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                this.Hide();
                MainWindow.Instance.Settings.ShowCommandWindow = false;
                e.Cancel = true;
            }
        }

        private void listCmds_ItemActivate(object sender, EventArgs e) {

        }

        private void buttonSendChars_Click(object sender, EventArgs e) {
            textBoxSendCommand.Text = String.Format("chars:{0}", textBoxChars.Text);
            Send();
        }

        private void buttonSend_Click(object sender, EventArgs e) {
            log4.Debug("buttonSend_Click");
            Send();
        }

        private void Send() {
            Logger.Instance.Log4.Info("Sending Command: " + textBoxSendCommand.Text);
            MainWindow.Instance.SendLine(textBoxSendCommand.Text);
        }

        private void listCmds_DoubleClick(object sender, EventArgs e) {
            log4.Debug("listCmds_DoubleClick");
            textBoxSendCommand.Text = listCmds.SelectedItems[0].Text;
            Send();
        }

    }
}
