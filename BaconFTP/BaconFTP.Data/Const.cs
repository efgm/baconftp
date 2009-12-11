using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Data
{
    internal class Const
    {
        internal static string ClearTextForEncriptDecript
        {
            get { return "ENCRIPT-DECRIPT"; }
        }

        internal static string UnknownLoggingMethodExceptionMessage
        {
            get { return "Logging method unknown. Valid methods are 'file' and 'console'."; }
        }

        internal static string ServerPortElementNotFoundExceptionMessage
        {
            get { return "Server port element not found. Sintax: <port>port</port> where port is a number between 1 and 65536."; }
        }

        internal static string DirectoryElementNotFoundExceptionMessage
        {
            get { return "Directory element not found. Sintax: <directory>path</directory>"; }
        }

        internal static string LoggingElementNotFoundExceptionMessasge
        {
            get { return "Logging element not found. Sintax: <logging>method</logging> where method is either 'file' or 'console'."; }
        }

        internal static string ConfigurationFileEmptyExceptionMessage
        {
            get { return "The configuration file cannot be empty."; }
        }

    }
}
