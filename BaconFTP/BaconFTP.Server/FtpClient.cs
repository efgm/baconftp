using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace BaconFTP.Server
{
    internal class FtpClient
    {
        public NetworkStream Stream { get; private set; }
        public TcpClient TcpClientObject { get; private set; }

        internal FtpClient(TcpClient clientObject)
        {
            TcpClientObject = clientObject;
            Stream = clientObject.GetStream();
        }
    }
}
