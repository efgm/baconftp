using System;

namespace BaconFTP.Server
{
    internal enum Codes
    {   
        CommandNotImplemented = 120,
        DataConnection = 150,

        Okay = 200,
        FileOkay = 202,
        TransferComplete = 226,
        CommandSuccessful = 250,        

        DirectoryStatus = 212,
        FileStatus = 213,
        SystemDescrption = 215,
        ServerClosingConnection = 221,
        PassiveMode = 227,
        UserLoggedIn = 230,
        WorkingDirectory = 257,        
        
        UserOkay = 331,

        ServerOffline = 421,
        CannotOpenDataConnection = 425,
        DataConnectionError = 426,
        CantPerformOperation = 450,

        Error = 500,
        SyntaxErrorInParameters = 501,
        NotLoggedIn = 530,
        NoUserAuthenticated = 530,
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

        //enviar/recibir los bytes en bloques de 4kb.
        public static int BlockSize
        {
            get { return 4096; }
        }

        #endregion //Constants

        #region Messages

        internal static string AnonymousUserAllowedMessage
        {
            get { return ((int)Codes.UserOkay) + " Anonymous user allowed, please enter your e-mail as your password.\r\n"; }
        }

        internal static string WelcomeMessage
        {
            get { return ((int)Codes.Okay) + " Welcome to BaconFTP Server.\r\n"; }
        }

        internal static string UserLoggedInMessage
        {
            get { return ((int)Codes.UserLoggedIn) + " Logged in.\r\n"; }
        }

        internal static string ServerClosingConnectionMessage
        {
            get { return ((int)Codes.ServerClosingConnection) + " Closing connection..\r\n"; }
        }

        internal static string UnknownCommandErrorMessage
        {
            get { return ((int)Codes.Error) + " Command not recognized.\r\n"; }
        }

        internal static string UserOkNeedPasswordMessage
        {
            get { return ((int)Codes.UserOkay) + " User name okay, need password.\r\n"; }
        }

        internal static string LoginFailedMessage
        {
            get { return ((int)Codes.NotLoggedIn) + " Login authentication failed.\r\n"; }
        }

        internal static string SystemDescriptionMessage
        {
            get { return ((int)Codes.SystemDescrption) + " " + "Windows_NT" + "\r\n"; }
        }

        internal static string ChangeWorkingDirectoryMessage 
        {
            get { return ((int)Codes.CommandSuccessful) + " CWD command successful.\r\n"; }
        }

        internal static string ChangeToParentDirectoryMessage
        {
            get { return ((int)Codes.CommandSuccessful) + " CDUP command successful.\r\n"; }
        }

        internal static string CommandOkayMessage
        {
            get { return ((int)Codes.CommandSuccessful) + " Command okay.\r\n"; }
        }

        internal static string SyntaxErrorInParametersMessage 
        {
            get { return ((int)Codes.SyntaxErrorInParameters) + " Directory/file not found or missing parameter.\r\n"; }
        }

        internal static string TransferCompleteMessage
        {
            get { return ((int)Codes.TransferComplete) + " Transfer complete.\r\n"; }
        }

        internal static string FileOperationOkayMessage
        {
            get { return ((int)Codes.CommandSuccessful) + " Requested file action okay, completed.\r\n"; }
        }

        internal static string CannotDeleteFileMessage
        {
            get { return ((int)Codes.CantPerformOperation) + " Can't delete file.\r\n"; }
        }

        internal static string CannotDeleteDirectoryMessage
        {
            get { return ((int)Codes.CantPerformOperation) + " Can't delete directory.\r\n"; }
        }

        internal static string NoPermissionToDeleteFileMessage
        {
            get { return ((int)Codes.CantPerformOperation) + " No permission to delete file.\r\n"; }
        }

        internal static string NoPermissionToDeleteDirectoryMessage
        {
            get { return ((int)Codes.CantPerformOperation) + " No permission to delete directory.\r\n"; }
        }

        internal static string CurrentWorkingDirectoryMessage(string directory)
        {
            //si el directorio tiene espacios, devuelvelo entre " ".
            return (directory.Split(' ').Length > 1) ?
                ((int)Codes.WorkingDirectory) + " \"" + directory + "\" is current working directory.\r\n" :
                ((int)Codes.WorkingDirectory) + " " + directory + " is current working directory.\r\n";
        }

        internal static string OpeningDataConnectionMessage(string type)
        {
            return ((int)Codes.DataConnection) + String.Format(" Opening {0} mode data connection.\r\n", type);
        }

        internal static string TransferTypeSetToMessage(string type)
        {
            return ((int)Codes.CommandSuccessful) + " Type set to " + type + ".\r\n";
        }

        internal static string DirectoryAlreadyExistsMessage
        {
            get { return ((int)Codes.FileNotFoundOrNoAccess) + " Directory already exists.\r\n"; }
        }

        internal static string DirectoryCreatedMessage
        {
            get { return ((int)Codes.CommandSuccessful) + " Directory created.\r\n"; }
        }

        internal static string CannotCreateDirectoryMessage
        {
            get { return ((int)Codes.FileNotFoundOrNoAccess) + " Cannot create directory.\r\n."; }
        }

        internal static string DataConnectionErrorMessage
        {
            get { return ((int)Codes.DataConnectionError) + " Data connection error.\r\n"; }
        }

        internal static string CannotOpenDataConnectionMessage
        {
            get { return ((int)Codes.CannotOpenDataConnection) + " Cannot open the data connection.\r\n"; }
        }

        internal static string DirectoryRemovedMessage
        {
            get { return ((int)Codes.CommandSuccessful) + " Directory removed.\r\n"; }
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

        #region FTP Service Commands

        internal static string PasvCommand
        {
            get { return "PASV"; }
        }

        internal static string RetrCommand
        {
            get { return "RETR"; }
        }

        internal static string StorCommand
        {
            get { return "STOR"; }
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
