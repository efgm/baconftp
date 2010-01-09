using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaconFTP.Data.Configuration;

namespace BaconFTP.ConfigurationManager
{
    public partial class ConfigurationManager : Form
    {
        public ConfigurationManager()
        {
            InitializeComponent();
            LoadConfigurationFile();
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
    }
}
