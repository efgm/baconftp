using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace BaconFTP.Server
{
    public class FtpClient
    {
        public NetworkStream Stream { get; private set; }
        public TcpClient TcpClientObject { get; private set; }
        public EndPoint EndPoint { get; private set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public FtpClient(TcpClient clientObject)
        {
            TcpClientObject = clientObject;
            Stream = clientObject.GetStream();
            EndPoint = clientObject.Client.RemoteEndPoint;
        }

        public void CloseConnection()
        {
            TcpClientObject.Close();
        }
    }
}
