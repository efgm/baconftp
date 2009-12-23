using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BaconFTP.Server
{
    internal enum Codes
    {   
        CommandNotImplemented = 120,
        Okay = 200,
        FileOkay = 202,
        CommandSuccessful = 250,
        ServerOffline = 421,
        SyntaxErrorInParameters = 501,
        DirectoryStatus = 212,
        FileStatus = 213,
        SystemDescrption = 215,
        ServerClosingConnection = 221,
        PassiveMode = 227,
        UserLoggedIn = 230,

        WorkingDirectory = 257,
        
        
        UserOkay = 331,
        NoUserAuthenticated = 530,

        Error = 500,
        NotLoggedIn = 530,
        FileNotFoundOrNoAccess = 550,
    }

    public class Const
    {
        #region Constants

        internal static string AnonymousUser
        {
            get { return "anonymous"; }
        }

        internal static string ServerVersion
        {
            get { return "0.1"; }
        }

        public static string FatalErrorFormatString
        {
            get { return "FATAL ERROR: Server could not start.\n\n{0}\n\nPress any key to exit."; }
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

        internal static string UserOkNeedPasswordMessage
        {
            get { return ((int)Codes.UserOkay) + " User name okay, need password.\n"; }
        }

        internal static string LoginFailedMessage
        {
            get { return ((int)Codes.NotLoggedIn) + " Login authentication failed.\n"; }
        }

        internal static string SystemDescriptionMessage
        {
            get { return ((int)Codes.SystemDescrption) + " " + Environment.OSVersion + "\n"; }
        }

        internal static string ChangeWorkingDirectoryMessage 
        {
            get { return ((int)Codes.CommandSuccessful).ToString() + "\n"; }
        }

        internal static string SyntaxErrorInParametersMessage 
        {
            get { return ((int)Codes.SyntaxErrorInParameters) + " Directory not found or missing parameter.\n"; }
        }

        internal static string CurrentWorkingDirectoryMessage(string directory)
        {
            return ((int)Codes.WorkingDirectory) + " '" + directory + "' is current working directory.\n";
        }

        internal static string PasvCommandReplyMessage
        {
            get
            {
                string[] serverIp = FtpServer.ServerIP.ToString().Split('.');

                return String.Format(((int)Codes.PassiveMode) +
                                     " Entering Passive Mode. ({0},{1},{2},{3},{4},{5}).\n", serverIp[0],
                                                                                             serverIp[1],
                                                                                             serverIp[2],
                                                                                             serverIp[3],
                                                                                             20,
                                                                                             30);
            }
        }
        #endregion //Messages

        #region Commands

        #region Access Controls Commands

        internal static string UserCommand
        {
            get { return "USER"; }
        }

        internal static string PassCommand
        {
            get { return "PASS"; }
        }

        internal static string AcctCommand
        {
            get { return "ACCT"; }
        }

        internal static string CwdCommand
        {
            get { return "CWD"; }
        }

        internal static string CdupCommand
        {
            get { return "CDUP"; }
        }

        internal static string QuitCommand
        {
            get { return "QUIT"; }
        }

        #endregion

        #region Transfer Parameters Commands

        internal static string PortCommand
        {
            get { return "PORT"; }
        }

        internal static string PasvCommand
        {
            get { return "PASV"; }
        }

        #endregion

        #region FTP Service Commands

        internal static string RetrCommand
        {
            get { return "RETR"; }
        }

        internal static string StorCommand
        {
            get { return "STOR"; }
        }

        internal static string StouCommand
        {
            get { return "STOU";}
        }

        internal static string AppeCommand
        {
            get { return "APPE"; }
        }

        internal static string RnfrCommand
        {
            get { return "RNFR"; }
        }

        internal static string RntoCommand 
        {
            get { return "RNTO"; }
        }

        internal static string DeleCommand 
        {
            get { return "DELE"; }
        }

        internal static string RmdCommand
        {
            get { return "RMD"; }
        }

        internal static string MkdCommand
        {
            get { return "MKD"; }
        }

        internal static string PwdCommand
        {
            get { return "PWD"; }
        }

        internal static string ListCommand
        {
            get { return "LIST"; }
        }

        internal static string NlistCommand 
        {
            get { return "NLST"; }
        }

        internal static string SystCommand
        {
            get { return "SYST"; }
        }

        internal static string HelpCommand
        {
            get { return "HELP"; }
        }

        internal static string NoopCommand
        {
            get { return "NOOP"; }
        }

        internal static string TypeCommand
        {
            get { return "TYPE"; }
        }

        #endregion

        #endregion //Commands
    }
}
