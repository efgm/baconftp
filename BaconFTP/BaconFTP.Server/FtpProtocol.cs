using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using BaconFTP.Data;
using BaconFTP.Data.Repositories;
using BaconFTP.Data.Logger;
using BaconFTP.Data.Configuration;

namespace BaconFTP.Server
{
    internal class FtpProtocol : IFtpProtocol
    {
        private readonly FtpClient _client;
        private readonly IAccountRepository _accRepo = new AccountRepository();
        private readonly ILogger _logger;
        private string _currentWorkingDirectory;

        public FtpProtocol(FtpClient client, ILogger logger)
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

                    if (cmd.Command == Const.QuitCommand){ HandleQuitCommand(); break; }

                    if (cmd.Command == Const.UserCommand) HandleUserCommand();
                    
                    if (cmd.Command == Const.SystCommand) HandleSystCommand();

                    if (cmd.Command == Const.CwdCommand) HandleCwdCommand(cmd.Arguments);

                    if (cmd.Command == Const.CdupCommand) HandleCdupCommand();

                    if (cmd.Command == Const.PwdCommand) HandlePwdCommand();

                    if (cmd.Command == Const.PasvCommand) HandlePasvCommand();

                    if (cmd.Command == Const.TypeCommand) HandleTypeCommand();

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
                string cmdString = Decode(buffer).Substring(0, bytesRead).Replace(Environment.NewLine, String.Empty);

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
            string directory = args.First();

            if (Directory.Exists(FtpServer.GetRealPath(directory)))
            {
                _currentWorkingDirectory += directory;
                SendMessageToClient(Const.ChangeWorkingDirectoryMessage + _currentWorkingDirectory);

                _logger.Write("Current Working Directory changed to: " + directory);
            }
            else
            {
                SendMessageToClient(Const.SyntaxErrorInParametersMessage);
            }
        }

        private void HandleCdupCommand() 
        {
            _currentWorkingDirectory = "/";  //new DirectoryInfo(Const._currentWorkingDirectory).Parent.Name;
            SendMessageToClient(Const.ChangeWorkingDirectoryMessage + _currentWorkingDirectory);
            
            _logger.Write("Current Working Directory changed to Parent Directory: " + _currentWorkingDirectory);
        }

        private void HandlePwdCommand()
        {
            SendMessageToClient(Const.CurrentWorkingDirectoryMessage(_currentWorkingDirectory));
        }

        private void HandlePasvCommand()
        {
            SendMessageToClient(Const.PasvCommandReplyMessage);
        }

        private void HandleTypeCommand()
        {
            SendMessageToClient("200 Type set to I.\n");
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
