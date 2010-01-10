using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaconFTP.Data.Configuration;
using BaconFTP.Data.Repositories;
using System.Text.RegularExpressions;
using System.IO;

namespace BaconFTP.ConfigurationManager
{
    public partial class ConfigurationManager : Form
    {
        private readonly IAccountRepository _accountRepository = new AccountRepository();

        public ConfigurationManager()
        {
            InitializeComponent();

            LoadLoggingMethods();
            LoadConfigurationFile();
            LoadUsers();
            
        }

        #region Event Handlers

        private void btnBrowseServerPath_Click(object sender, EventArgs e)
        {
            switch (folderBrowserDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    tbServerDirPath.Text = folderBrowserDialog1.SelectedPath;
                    break;

                case DialogResult.Cancel:
                    break;
            }
        }

        private void btnSaveConfiguration_Click(object sender, EventArgs e)
        {
            SaveServerConfiguration();
        }

        private void aboutBaconFTPServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenAboutDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void openHelpFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHelpFile();
        }

        #endregion //Event handlers

        #region Implementation

        private void LoadUsers()
        {
            cbUsers.DataSource = _accountRepository.GetAll();
        }

        private void LoadLoggingMethods()
        {
            cbLoggingMethods.DataSource = new string[] { "console", "file" };
        }

        private void LoadConfigurationFile()
        {
            try
            {
                ServerConfiguration.Parse();
            }
            catch (TypeInitializationException e)
            {
                ShowError("{0}\n\n Generating a new configuration file using default values.", e.InnerException.Message);
                ServerConfiguration.GenerateConfigurationFile();
            }
            catch (Exception e)
            {
                ShowError("{0}\n\n Generating a new configuration file using default values.", e.Message);
                ServerConfiguration.GenerateConfigurationFile();
            }

            SetValues();
        }

        private void SetValues()
        {
            tbServerDirPath.Text = ServerConfiguration.ServerDirectoryPath;
            tbServerPort.Text = ServerConfiguration.ServerPort.ToString();
            cbLoggingMethods.SelectedItem = (object)ServerConfiguration.Logger;
        }

        private void ShowError(string title, string message, params object[] args)
        {
            MessageBox.Show(String.Format(message, args),
                            title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowError(string message, params object[] args)
        {
            ShowError("Error parsing the configuration file.", message, args);
        }

        private void ShowInfo(string title, string message, params object[] args)
        {
            MessageBox.Show(String.Format(message, args),
                            title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveServerConfiguration()
        {
            try
            {
                ValidateData();
            }
            catch (Exception e)
            {
                ShowError("Error", e.Message);
                return;
            }

            SaveConfiguration();
        }

        private void ValidateData()
        {
            if (!Directory.Exists(tbServerDirPath.Text))
                throw new Exception("Please enter a valid server directory");

            if (Convert.ToInt32(tbServerPort.Text) < 1 && Convert.ToInt32(tbServerPort.Text) > 65535)
                throw new Exception("Please enter a valid port number (from 1 to 65535)");

            if (cbLoggingMethods.SelectedIndex == -1)
                throw new Exception("Please select one logging method from the list.");
        }

        private void SaveConfiguration()
        {
            ServerConfiguration.ServerDirectoryPath = tbServerDirPath.Text;
            ServerConfiguration.ServerPort = Convert.ToInt32(tbServerPort.Text);
            ServerConfiguration.Logger = cbLoggingMethods.SelectedItem.ToString();

            ShowInfo("Server configuration has been saved.", "Your server configuration has been saved.");
        }

        private void OpenHelpFile()
        {
            throw new NotImplementedException();
        }

        private void OpenAboutDialog()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
