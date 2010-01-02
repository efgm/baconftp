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

namespace BaconFTP.Server
{
    internal class FtpProtocolInterpreter : IFtpProtocol
    {
        private readonly FtpClient _client;
        private readonly IAccountRepository _accRepo = new AccountRepository();
        private readonly ILogger _logger;
        private string _currentWorkingDirectory;
        private int _dataPort;

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

            GetClientUsername();

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

                    if (cmd.Command == Const.QuitCommand) { HandleQuitCommand(); break; }

                    else if (cmd.Command == Const.UserCommand) HandleUserCommand();

                    else if (cmd.Command == Const.SystCommand) HandleSystCommand();

                    else if (cmd.Command == Const.CwdCommand) HandleCwdCommand(cmd.Arguments.First());

                    else if (cmd.Command == Const.CdupCommand) HandleCdupCommand();

                    else if (cmd.Command == Const.PwdCommand) HandlePwdCommand();

                    else if (cmd.Command == Const.PasvCommand) HandlePasvCommand();

                    else if (cmd.Command == Const.TypeCommand) HandleTypeCommand();

                    else if (cmd.Command == Const.ListCommand) HandleListCommand();                   

                    else SendMessageToClient(Const.UnknownCommandErrorMessage);
                }
                catch { continue; }
            }
        }

        #endregion

        #region Implementation

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

            if (cmd.Command == Const.PassCommand)
            {
                _client.Password = cmd.Arguments.First();

                SendMessageToClient(Const.UserLoggedInMessage);
            }
        }

        private void GetPasswordFromUser()
        {
            ClientCommand cmd = GetCommandFromClient();

            if (cmd.Command == Const.PassCommand)            
                _client.Password = cmd.Arguments.First();
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

        private void HandleCwdCommand(string directory) 
        {
            if (Directory.Exists(FtpServer.GetRealPath(directory)))
            {
                _currentWorkingDirectory += directory;
                _currentWorkingDirectory = _currentWorkingDirectory.Replace("//", "/");
                SendMessageToClient(Const.ChangeWorkingDirectoryMessage + _currentWorkingDirectory);
            }
            else
            {
                SendMessageToClient(Const.SyntaxErrorInParametersMessage);
            }
        }

        private void HandleCdupCommand() 
        {
            //string actualDirectoryName = (new DirectoryInfo(FtpServer.GetRealPath(_currentWorkingDirectory))).Name;

            string newWorkingDir = _currentWorkingDirectory.Replace(
                                        (new DirectoryInfo(FtpServer.GetRealPath(_currentWorkingDirectory))).Name,
                                        string.Empty);

            if (newWorkingDir.Length == 1)
                HandleCwdCommand(newWorkingDir);
            else
                HandleCwdCommand(newWorkingDir.Substring(newWorkingDir.Length - 1));
                
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

            string pasvReply = (int)Codes.PassiveMode +
                               String.Format(" Entering Passive Mode ({0},{1},{2},{3},{4},{5}).\r\n",
                                             127, 0, 0, 1,
                                             (_dataPort - (_dataPort % 256)) / 256,
                                             _dataPort % 256);

            SendMessageToClient(pasvReply);
        }

        private void HandleTypeCommand()
        {
            //temporal
            SendMessageToClient("200 Type set to I.\n");
        }

        private void HandleListCommand()
        {
            var dtp = new FtpDataTransferProcess(_client, _logger, _dataPort);

            new Thread(dtp.SendDirectoryListing).Start(_currentWorkingDirectory);
        }

        #endregion //CommandHandling

        #region Authentication

        private bool AuthenticateAnonymousUser()
        {
            SendMessageToClient(Const.AnonymousUserAllowedMessage);
            GetAnonymousPasswordAndValidate();

            _logger.Write("Logged in as \'{0}\' from {1}", 
                           _client.Username, _client.EndPoint);
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
