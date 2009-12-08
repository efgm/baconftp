using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace BaconFTP.Data.Configuration
{
    internal static struct Const
    {
        internal static string ServerDirectoryElement = "server_directory";
        internal static string ServerConfigurationFilename = "server_config.xml";
        internal static string DefaultPortElement = "default_port";
        internal static string ConfigurationRootElement = "baconftp_server_config";
    }

    public static class ServerConfiguration
    {
        private static string _pathToXmlFile = Const.ServerConfigurationFilename;
        private static XDocument _configXmlFile = XDocument.Load(GetConfigurationFile());
        private static XElement _root = _configXmlFile.Root;        

        #region Interface

        public static string GetServerDirectoryPath()
        {
            return (from d in _root.Elements()
                    where d.Name == Const.ServerDirectoryElement
                    select d).Single().Value;
        }

        #endregion

        #region Implementation

        private static string GetConfigurationFile()
        {
            if (!File.Exists(_pathToXmlFile))
                GenerateConfigurationFile();

            return _pathToXmlFile;

        }

        private static void GenerateConfigurationFile()
        {
            (new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("baconftp_server_config", 
                             new XElement("default_port", 21),
                             new XElement("server_directory", CreateDefaultServerFolder())
                             )
                           )
             ).Save(_pathToXmlFile);
        }

        private static string CreateDefaultServerFolder()
        {
            const string dir = "baconftpd";
            if (Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        #endregion
    }
}
