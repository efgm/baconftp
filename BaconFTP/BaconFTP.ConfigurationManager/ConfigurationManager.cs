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

namespace BaconFTP.ConfigurationManager
{
    public partial class ConfigurationManager : Form
    {
        private IAccountRepository _accountRepository = new AccountRepository();

        public ConfigurationManager()
        {
            InitializeComponent();

            LoadConfigurationFile();
            LoadUsers();
            LoadLoggingMethods();
        }

        private void LoadUsers()
        {
            cbUsers.DataSource = _accountRepository.GetAll();            
        }

        private void LoadLoggingMethods()
        {
            cbLoggingMethods.DataSource = new string[] { "Console", "File" };
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
        }

        private void ShowError(string message, params object[] args)
        {
            MessageBox.Show(String.Format(message, args), 
                            "Error parsing the configuration file.", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

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
    }
}
