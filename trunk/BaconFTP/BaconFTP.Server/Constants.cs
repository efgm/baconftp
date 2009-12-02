using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Server
{
    internal enum Codes
    {
        Okay = 200
    }

    internal class Constants
    {
        internal static int DefaultFtpPort
        {
            get { return 21; }
        }

        internal static string WelcomeMessage
        {
            get { return ((int)Codes.Okay) + " Welcome.\n"; }
        }
    }
}
