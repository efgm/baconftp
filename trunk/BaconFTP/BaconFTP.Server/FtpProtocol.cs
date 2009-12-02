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
            _client.Stream.Write(Encoding.ASCII.GetBytes(Constants.WelcomeMessage), 0, Constants.WelcomeMessage.Length);
        }

        #endregion
    }
}
