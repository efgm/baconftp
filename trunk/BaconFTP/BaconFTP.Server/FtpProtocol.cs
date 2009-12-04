using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BaconFTP.Data;
using BaconFTP.Data.Repositories;

namespace BaconFTP.Server
{
    internal class FtpProtocol : IFtpProtocol
    {
        private readonly FtpClient _client;
        private const IAccountRepository _accountRepository = new AccountRepository();

        public FtpProtocol(FtpClient client)
        {
            _client = client;
        }

        #region IFtpProtocol Members

        //valida el usuario y da acceso
        public void PerformHandShake()
        {
            SendMessageToClient(Const.WelcomeMessage);

            GetClientUsername();

            if (!String.IsNullOrEmpty(_client.Username))
            {
                if (_client.Username == Const.AnonymousUser)
                    AuthenticateAnonymousUser();

                else
                    AuthenticateUser();
            }
        }

        //escuchar infinitamente los comandos del cliente conectado.
        public void ListenForCommands()
        {
            while (true)
            {
                ClientCommand cmd = GetCommandFromClient();

                if (cmd.Command == Const.QuitCommand)
                {
                    HandleQuitCommand();
                    break;
                }

                else
                    SendMessageToClient(Const.UnknownCommandErrorMessage);
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
            {
                _client.Username = cmd.Arguments.First();
            }
            return (cmd.Command == Const.UserCommand) ? cmd.Arguments.First() : null;
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

            _client.CloseConnection();
        }


        #endregion //CommandHandling

        #region Authentication

        private void AuthenticateAnonymousUser()
        {
            SendMessageToClient(Const.AnonymousUserAllowedMessage);
            GetAnonymousPasswordAndValidate();
        }

        private bool AuthenticateUser()
        {
            try
            {
                if (_accountRepository.GetByUsername(_client.Username).Password == _client.Password)
                    return true;
            }
            catch { return false; }

            return false;
        }

        #endregion //Authentication



        #endregion //Implementation
    }
}
