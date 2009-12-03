using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Server
{
    internal enum Codes
    {   
        CommandNotImplemented = 120,
        Okay = 200,
        FileOkay = 202,
        DirectoryStatus = 212,
        FileStatus = 213
    }

    internal class Constants
    {
        internal static int DefaultFtpPort
        {
            get { return 21; }
        }

        internal static string WelcomeMessage
        {
            get { return ((int)Codes.Okay) + " Welcome to BaconFTP Server.\n"; }
        }
    }
}
