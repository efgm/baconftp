using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using BaconFTP.Data;
using BaconFTP.Data.Repositories;
using BaconFTP.Data.Logger;
using System.Net.Sockets;
using System.Net;
using System.Security;

namespace BaconFTP.Server
{
    internal class FtpProtocolInterpreter : IFtpProtocol
    {
        private readonly FtpClient _client;
        private readonly IAccountRepository _accRepo = new AccountRepository();
        private readonly ILogger _logger;
        private string _currentWorkingDirectory;
        private int _dataPort;
        private string _transferType;

        internal FtpProtocolInterpreter(FtpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;

            _currentWorkingDirectory = "/";
        }

        #region IFtpProtocol Members

        //valida el usuario y da acceso
        public bool PerformHandShake()
        {
            SendMessageToClient(Const.WelcomeMessage);

            try { GetClientUsername(); }
            catch { return false; }

            if (!String.IsNullOrEmpty(_client.Username))
                return _client.Username == Const.AnonymousUser ? 
                    AuthenticateAnonymousUser() : AuthenticateUser();

            return false;

        }

        //escuchar infinitamente los comandos del cliente conectado.
        public void ListenForCommands()
        {
            while (true)
            {
                try
                {
                    ClientCommand cmd = GetCommandFromClient();

                    if (AreEqual(cmd.Command, Const.QuitCommand)) { HandleQuitCommand(); break; }

                    else if (AreEqual(cmd.Command, Const.UserCommand)) HandleUserCommand();

                    else if (AreEqual(cmd.Command, Const.SystCommand)) HandleSystCommand();

                    else if (AreEqual(cmd.Command, Const.CwdCommand)) HandleCwdCommand(cmd.Arguments);

                    else if (AreEqual(cmd.Command, Const.CdupCommand)) HandleCdupCommand();

                    else if (AreEqual(cmd.Command, Const.PwdCommand)) HandlePwdCommand();

                    else if (AreEqual(cmd.Command, Const.PasvCommand)) HandlePasvCommand();

                    else if (AreEqual(cmd.Command, Const.TypeCommand)) HandleTypeCommand(cmd.Arguments.First());

                    else if (AreEqual(cmd.Command, Const.ListCommand)) HandleListCommand();

                    else if (AreEqual(cmd.Command, Const.RetrCommand)) HandleRetrCommand(cmd.Arguments);

                    else if (AreEqual(cmd.Command, Const.StorCommand)) HandleStorCommand(cmd.Arguments);

                    else if (AreEqual(cmd.Command, Const.DeleCommand)) HandleDeleCommand(cmd.Arguments);

                    else if (AreEqual(cmd.Command, Const.MkdCommand)) HandleMkdCommand(cmd.Arguments);

                    else if (AreEqual(cmd.Command, Const.RmdCommand)) HandleRmdCommand(cmd.Arguments);

                    else if (AreEqual(cmd.Command, Const.NoopCommand)) HandleNoopCommand();

                    else SendMessageToClient(Const.UnknownCommandErrorMessage);
                }
                catch { continue; }
            }
        }

        #endregion

        #region Implementation

        //compara 2 strings ignorando mayúsculas y minúsculas
        private bool AreEqual(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.CurrentCultureIgnoreCase);
        }

        private void SendMessageToClient(string message)
        {
            _client.Stream.Write(Encode(message), 0, message.Length);
        }

        //devuelve el nombre de usuario del cliente
        private void GetClientUsername()
        {
            ClientCommand cmd = GetCommandFromClient();

            if (cmd.Command == Const.UserCommand)
                _client.Username = cmd.Arguments.First();         
        }
        
        //devuelve el comando que se recibio del cliente
        private ClientCommand GetCommandFromClient()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _client.Stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string cmdString = Decode(buffer).Substring(0, bytesRead).Replace("\r\n", String.Empty);

                var argList = new List<string>();

                foreach (string arg in cmdString.Split(' ').Skip(1))
                    argList.Add(arg);

                return new ClientCommand(cmdString.Split(' ').First(), argList);
            }
            else return null;
            
        }

        private void GetAnonymousPasswordAndValidate()
        {
            ClientCommand cmd = GetCommandFromClient();

            if (cmd == null)
                throw new Exception("Expecting PASS command..");

            if (cmd.Command == Const.PassCommand)
            {
                _client.Password = cmd.Arguments.First();

                SendMessageToClient(Const.UserLoggedInMessage);
            }
            else
                throw new Exception("Expecting PASS command..");
        }

        private void GetPasswordFromUser()
        {
            ClientCommand cmd = GetCommandFromClient();

            if (cmd == null)
                throw new Exception("Expecting PASS command..");

            if (cmd.Command == Const.PassCommand)
                _client.Password = cmd.Arguments.First();
            else
                throw new Exception("Expecting PASS command..");
        }

        private byte[] Encode(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private string Decode(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        private int GenerateDataPort()
        {
            int port = (new Random()).Next(1024, 65536);
            return PortIsAvailable(port) ? port : GenerateDataPort();
        }

        private bool PortIsAvailable(int port)
        {
            try
            {
                (new TcpClient(new IPEndPoint(IPAddress.Any, port))).Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region CommandHandling

        private void HandleQuitCommand()
        {
            SendMessageToClient(Const.ServerClosingConnectionMessage);

            FtpServer.CloseConnection(_client);
        }

        private void HandleUserCommand()
        {
            SendMessageToClient(Const.UserLoggedInMessage);
        }

        private void HandleSystCommand()
        {
            SendMessageToClient(Const.SystemDescriptionMessage);
        }

        private void HandleCwdCommand(IList<string> args) 
        {
            string directory = JoinArguments(args);

            if (!directory.Contains("/"))
                directory = _currentWorkingDirectory + "/" + directory;

            directory = directory.Replace("//", "/");

            if (Directory.Exists(FtpServer.GetRealPath(directory)))
            {
                _currentWorkingDirectory = directory;

                SendMessageToClient(Const.ChangeWorkingDirectoryMessage);
            }
            else
                SendMessageToClient(Const.SyntaxErrorInParametersMessage);
        }

        private void HandleCdupCommand() 
        {
            string newWorkingDir = _currentWorkingDirectory.Replace(
                                        (new DirectoryInfo(FtpServer.GetRealPath(_currentWorkingDirectory))).Name,
                                        string.Empty);

            _currentWorkingDirectory = IsRootDirectory(newWorkingDir) ? 
                                       newWorkingDir : newWorkingDir.Substring(0, newWorkingDir.Length - 1);

            SendMessageToClient(Const.ChangeToParentDirectoryMessage);    
        }

        private void HandlePwdCommand()
        {
            SendMessageToClient(Const.CurrentWorkingDirectoryMessage(_currentWorkingDirectory));
        }

        private void HandlePasvCommand()
        {
            _dataPort = GenerateDataPort();

            /* formulita para sacar el octeto 1 del puerto para el reply = (port - (port % 256)) / 256
             * para el octeto2 = port % 256
             */
            StringBuilder sb = new StringBuilder();

            string[] serverIp = GetServerIpAddress();

            string pasvReply = (int)Codes.PassiveMode +
                               String.Format(" Entering Passive Mode ({0},{1},{2},{3},{4},{5}).\r\n",
                                             serverIp[0], serverIp[1], serverIp[2], serverIp[3],
                                             (_dataPort - (_dataPort % 256)) / 256,
                                             _dataPort % 256);

            SendMessageToClient(pasvReply);
        }
        
        private void HandleTypeCommand(string type)
        {
            _transferType = type;
            SendMessageToClient(Const.TransferTypeSetToMessage(type));
        }

        private void HandleListCommand()
        {
            var dtp = new FtpDataTransferProcess(_client, _logger, _dataPort, 
                                                 _transferType, _currentWorkingDirectory);

            new Thread(dtp.SendDirectoryListing).Start(_currentWorkingDirectory);
        }

        private void HandleRetrCommand(IList<string> args)
        {
            string file = JoinArguments(args);

            var dtp = new FtpDataTransferProcess(_client, _logger, _dataPort, 
                                                 _transferType, _currentWorkingDirectory);
            string path = FtpServer.GetRealPath(_currentWorkingDirectory + "/" + file);
            
            if (File.Exists(path))
                new Thread(dtp.SendFileToClient).Start(path);
            else
                SendMessageToClient(Const.SyntaxErrorInParametersMessage);
        }

        private void HandleStorCommand(IList<string> args)
        {
            string file = JoinArguments(args);

            var dtp = new FtpDataTransferProcess(_client, _logger, _dataPort,
                                                 _transferType, _currentWorkingDirectory);

            new Thread(dtp.GetFileFromClient).Start(file);

        }

        private void HandleDeleCommand(IList<string> args)
        {
            string file = JoinArguments(args);
            string path = FtpServer.GetRealPath(_currentWorkingDirectory + "/" + file);

            if (File.Exists(path))
            {
                try
                {
                    new FileInfo(path).Delete();
                    SendMessageToClient(Const.FileOperationOkayMessage);
                }
                catch (IOException)
                {
                    SendMessageToClient(Const.CannotDeleteFileMessage);
                }
                catch (SecurityException)
                {
                    SendMessageToClient(Const.NoPermissionToDeleteFileMessage);
                }
            }
            else
                SendMessageToClient(Const.SyntaxErrorInParametersMessage);
            
        }

        private void HandleMkdCommand(IList<string> args)
        {
            string directory = JoinArguments(args);
            string path = FtpServer.GetRealPath(_currentWorkingDirectory + "/" + directory);

            if (!Directory.Exists(path))
            {
                try
                {
                    new DirectoryInfo(path).Create();
                    SendMessageToClient(Const.DirectoryCreatedMessage);
                }
                catch (IOException)
                {
                    SendMessageToClient(Const.CannotCreateDirectoryMessage);
                }
            }
            else
                SendMessageToClient(Const.DirectoryAlreadyExistsMessage);
        }

        private void HandleRmdCommand(IList<string> args)
        {
            string directory = JoinArguments(args);
            string path = FtpServer.GetRealPath(_currentWorkingDirectory + "/" + directory);

            if (Directory.Exists(path))
            {
                try
                {
                    new DirectoryInfo(path).Delete();
                    SendMessageToClient(Const.DirectoryRemovedMessage);
                }
                catch (IOException)
                {
                    SendMessageToClient(Const.CannotDeleteDirectoryMessage);
                }
                catch (SecurityException)
                {
                    SendMessageToClient(Const.NoPermissionToDeleteDirectoryMessage);
                }
            }
            else
                SendMessageToClient(Const.SyntaxErrorInParametersMessage);

        }

        private void HandleNoopCommand()
        {
            SendMessageToClient(Const.CommandOkayMessage);
        }

        internal string[] GetServerIpAddress()
        {
            return ClientIsLocal(_client) ? 
                _client.TcpClientObject.Client.LocalEndPoint.ToString().Split(':').First().Split('.') 
                :
                FtpServer.ServerIP.ToString().Split('.');
        }

        private bool ClientIsLocal(FtpClient client)
        {
            //sacar cada parte del ip por separado y meterlo en el arreglo
            string[] clientIp = client.EndPoint.ToString().Split(':').First().Split('.');

            return ((clientIp[0] == "192" && clientIp[1] == "168") ||
                    (clientIp[0] == "10" && clientIp[1] == "0")    ||
                    (clientIp[0] == "172" && clientIp[1] == "16")  ||
                    (clientIp[0] == "127" && clientIp[1] == "0")   || 
                    (clientIp[0] == "169" && clientIp[1] == "254")) ? true : false;
        }

        //si el archivo tiene espacios, juntar todo en una variable.
        private string JoinArguments(IList<string> args)
        {
            if (args.Count() > 1)
            {
                string joined = args.First();
                foreach (string a in args.Skip(1))
                    joined += " " + a;

                return joined;
            }
            else
                return args.First();            
        }

        private bool IsRootDirectory(string directory)
        {
            return directory == "/" ? true : false;
        }

        #endregion //CommandHandling

        #region Authentication

        private bool AuthenticateAnonymousUser()
        {
            try
            {
                SendMessageToClient(Const.AnonymousUserAllowedMessage);
                GetAnonymousPasswordAndValidate();

                _logger.Write("Logged in as \'{0}\' from {1}",
                               _client.Username, _client.EndPoint);
            }
            catch { return false; }
            return true;
        }

        private bool AuthenticateUser()
        {
            try
            {
                SendMessageToClient(Const.UserOkNeedPasswordMessage);
                GetPasswordFromUser();

                Account user = _accRepo.GetByUsername(_client.Username);

                if (user != null && user.Password == _client.Password)
                {
                    SendMessageToClient(Const.UserLoggedInMessage);

                    _logger.Write("Logged in as \'{0}\' from {1}.", 
                                  _client.Username, _client.EndPoint);
                    return true;
                }
                else
                {
                    SendMessageToClient(Const.LoginFailedMessage);

                    _logger.Write("Login attempt from \'{0}\' as {1} failed.", 
                                  _client.EndPoint, _client.Username);
                    return false;
                }                
            }
            catch { return false; }
        }

        #endregion //Authentication

        #endregion //Implementation
    }
}
