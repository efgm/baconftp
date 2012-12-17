using System.Net;
using System.Net.Sockets;

namespace BaconFTP.Server
{
    internal class FtpClient
    {
        internal NetworkStream Stream { get; private set; }
        internal TcpClient TcpClientObject { get; private set; }
        internal EndPoint EndPoint { get; private set; }

        internal string Username { get; set; }
        internal string Password { get; set; }

        internal FtpClient(TcpClient clientObject)
        {
            TcpClientObject = clientObject;
            Stream = clientObject.GetStream();
            EndPoint = clientObject.Client.RemoteEndPoint;
        }

        internal void CloseConnection()
        {
            TcpClientObject.Close();
        }
    }
}
