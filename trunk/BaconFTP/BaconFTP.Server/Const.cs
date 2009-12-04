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
        ServerClosingConnection = 221,
        UserLoggedIn = 230,
        
        UserOkay = 331,

        Error = 500,
    }

    internal class Const
    {
        #region Constants
        
        internal static int DefaultFtpPort 
        {
            get { return 21; }
        }

        internal static string AnonymousUser
        {
            get { return "anonymous"; }
        }

        #endregion //Constants

        #region Messages

        internal static string AnonymousUserAllowedMessage
        {
            get { return ((int)Codes.UserOkay) + " Anonymous user allowed, please enter your e-mail as your password.\n"; }
        }

        internal static string WelcomeMessage
        {
            get { return ((int)Codes.Okay) + " Welcome to BaconFTP Server.\n"; }
        }

        internal static string UserLoggedInMessage
        {
            get { return ((int)Codes.UserLoggedIn) + " Logged in.\n"; }
        }

        internal static string ServerClosingConnectionMessage
        {
            get { return ((int)Codes.ServerClosingConnection) + " Closing connection..\n"; }
        }

        internal static string UnknownCommandErrorMessage
        {
            get { return ((int)Codes.Error) + " Command not recognized.\n"; }
        }

        #endregion //Messages

        #region Commands

        internal static string UserCommand
        {
            get { return "USER"; }
        }

        internal static string PassCommand
        {
            get { return "PASS"; }
        }

        internal static string PwdCommand
        {
            get { return "PWD"; }
        }

        internal static string CwdCommand
        {
            get { return "CWD"; }
        }

        internal static string QuitCommand
        {
            get { return "QUIT"; }
        }
        
        internal static string CdupCommand
        {
            get { return "CDUP"; }
        }

        internal static string StorCommand
        {
            get { return "STOR"; }
        }

        internal static string RetrCommand
        {
            get { return "RETR"; }
        }

        internal static string MkdCommand
        {
            get { return "MKD"; }
        }

        internal static string RmdCommand
        {
            get { return "RMD"; }
        }

        internal static string PasvCommand
        {
            get { return "PASV"; }
        }

        internal static string ListCommand
        {
            get { return "LIST"; }
        }
        
        #endregion //Commands
    }
}
