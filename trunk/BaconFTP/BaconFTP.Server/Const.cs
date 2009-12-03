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
        FileStatus = 213,
        
        UserOkay = 331,
    }

    internal class Const
    {
        internal static int DefaultFtpPort 
        {
            get { return 21; }
        }

        internal static string AnonymousUser
        {
            get { return "anonymous"; }
        }

        #region Messages

        internal static string AnonymousUserAllowedMessage
        {
            get { return ((int)Codes.UserOkay) + " Anonymous user allowed, please enter your e-mail as your password.\n"; }
        }

        internal static string WelcomeMessage
        {
            get { return ((int)Codes.Okay) + " Welcome to BaconFTP Server.\n"; }
        }

        #endregion

        #region Commands

        internal static string UserCommand
        {
            get { return "USER"; }
        }

        internal static string Passcommand
        {
            get { return "PASS"; }
        }

        #endregion
    }
}
