using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace BaconFTP.Data.Configuration
{
    internal static class Const
    {
        internal static string ServerDirectoryElement = "server_directory";
        internal static string ServerConfigurationFilename = "server_config.xml";
        internal static string DefaultPortElement = "default_port";
        internal static string ConfigurationRootElement = "baconftp_server_config";
        internal static string ServerDirectoryName = "baconftpd";
    }

    public static class ServerConfiguration
    {
        private static string _pathToXmlFile = Const.ServerConfigurationFilename;
        private static XDocument _configXmlFile = XDocument.Load(GetConfigurationFile());
        private static XElement _root = _configXmlFile.Root;        

        #region Interface

        public static string ServerDirectoryPath
        {
            get { return GetValueFrom(Const.ServerDirectoryElement); }
            set { SetValue(Const.ServerDirectoryElement, value); }
        }

        public static int DefaultPort
        {
            get { return Convert.ToInt32(GetValueFrom(Const.DefaultPortElement)); }
            set { SetValue(Const.DefaultPortElement, value.ToString()); }
        }

        #endregion

        #region Implementation

        private static string GetConfigurationFile()
        {
            if (!File.Exists(_pathToXmlFile))
                GenerateConfigurationFile();

            return _pathToXmlFile;

        }

        private static string GetValueFrom(string element)
        {
            return (from d in _root.Elements()
                    where d.Name == element
                    select d).Single().Value;
        }

        private static void SetValue(string element, string value)
        {
            _root.Element(element).Value = value;
            _configXmlFile.Save(_pathToXmlFile);
        }

        private static void GenerateConfigurationFile()
        {
            (new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(Const.ConfigurationRootElement,
                             new XElement(Const.DefaultPortElement, 21),
                             new XElement(Const.ServerDirectoryElement, CreateDefaultServerFolder().FullName)
                             )
                           )
             ).Save(_pathToXmlFile);
        }

        private static DirectoryInfo CreateDefaultServerFolder()
        {
            return Directory.CreateDirectory(Const.ServerDirectoryName);
        }

        #endregion
    }
}
