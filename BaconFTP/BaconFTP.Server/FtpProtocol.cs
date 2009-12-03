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

        public void SendWelcomeMessageToClient()
        {
            _client.Stream.Write(Encode(Constants.WelcomeMessage), 0, Constants.WelcomeMessage.Length);
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

        #endregion
    }
}
