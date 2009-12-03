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

            string test = GetClientUsername();
        }



        #endregion

        #region Implementation

        private byte[] Encode(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private string Decode(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        private void SendWelcomeMessageToClient()
        {
            _client.Stream.Write(Encode(Constants.WelcomeMessage), 0, Constants.WelcomeMessage.Length);
        }

        //devuelve el nombre de usuario del cliente
        private string GetClientUsername()
        {
            string command = GetCommandFromClient();

            return (!String.IsNullOrEmpty(command) && command.Split(' ').First() == "USER") ? 
                command.Split(' ')[1] : String.Empty;
        }
        
        //lee los bytes que lee del cliente y los devuelve en un string.
        private string GetCommandFromClient()
        {
            byte[] buffer = new byte[1024];
            StringBuilder recievedData = new StringBuilder();

            int bytesRead = _client.Stream.Read(buffer, 0, buffer.Length);

            return (bytesRead > 0) ? recievedData.Append(Decode(buffer), 0, bytesRead).ToString() : String.Empty;
        }

        #endregion
    }
}
