using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Server
{
    internal class FtpProtocol : IFtpProtocol
    {
        private readonly FtpClient _client;

        public FtpProtocol(FtpClient client)
        {
            _client = client;
        }

        #region IFtpProtocol Members

        //valida el usuario y da acceso
        public void PerformHandShake()
        {
            SendWelcomeMessageToClient();

            string username = GetClientUsername();

            if (!String.IsNullOrEmpty(username))
            {
                if (username == Const.AnonymousUser)
                {
                    SendAnonymousUserOkToClient();
                    GetAnonymousPasswordAndValidate();
                }
            }
        }

        #endregion

        #region Implementation

        //le manda el mensaje de bienvenida del servidor al cliente
        private void SendWelcomeMessageToClient()
        {
            _client.Stream.Write(Encode(Const.WelcomeMessage), 0, Const.WelcomeMessage.Length);
        }

        //envia el msj de que el usuario anonymous esta habilitado y se puede logear con el
        private void SendAnonymousUserOkToClient()
        {
            _client.Stream.Write(Encode(Const.AnonymousUserAllowedMessage), 0, Const.AnonymousUserAllowedMessage.Length);
        }

        private void SendUserLoggedInMessage()
        {
            _client.Stream.Write(Encode(Const.UserLoggedInMessage), 0, Const.UserLoggedInMessage.Length);
        }

        //devuelve el nombre de usuario del cliente
        private string GetClientUsername()
        {
            ClientCommand cmd = GetCommandFromClient();

            if (cmd.Command == Const.UserCommand)
            {
                _client.Username = cmd.Arguments.First();
            }
            return (cmd.Command == Const.UserCommand) ? cmd.Arguments.First() : null;
        }
        
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

            }
        }

        private byte[] Encode(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private string Decode(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        #endregion
    }
}
