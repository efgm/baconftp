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

        public void PerformHandShake()
        {
            SendWelcomeMessageToClient();

            string username = GetClientUsername();

            if (username == Const.AnonymousUser)
            {
                SendAnonymousUserOkToClient();
                GetAnonymousPasswordAndValidate();
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

        //devuelve el nombre de usuario del cliente
        private string GetClientUsername()
        {
            ClientCommand cmd = GetCommandFromClient();

            return (cmd.Command == Const.UserCommand) ? cmd.Arguments.First() : null;
        }
        
        private ClientCommand GetCommandFromClient()
        {
            byte[] buffer = new byte[1024];
            StringBuilder recievedData = new StringBuilder();

            int bytesRead = _client.Stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string cmdString = recievedData.Append(Decode(buffer), 0, bytesRead).ToString().Replace(Environment.NewLine, String.Empty);

                List<string> argList = new List<string>();

                foreach (string arg in cmdString.Substring(1, cmdString.Length).Split(' '))
                    argList.Add(arg);

                return new ClientCommand(cmdString.Split(' ').First(), argList);
            }

            else
                return null;
            
        }

        private void GetAnonymousPasswordAndValidate()
        {
            string command = GetClientUsername();
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
