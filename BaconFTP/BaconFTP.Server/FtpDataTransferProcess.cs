﻿using System.Text;
using BaconFTP.Data.Logger;
using System.Net.Sockets;
using System.Net;

namespace BaconFTP.Server
{
    internal class FtpDataTransferProcess
    {
        private readonly FtpClient _client;
        private readonly ILogger _logger;
        private readonly TcpListener _tcpListener = new TcpListener(IPAddress.Any, 3762);

        internal FtpDataTransferProcess(FtpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public void GetDirectoryListing()
        {
        }

        public void ListenForConnections()
        {
            #region ejemplo, no va asi

            string m1 = "150\r\n";
            string m2 = "226\r\n";

            _client.Stream.Write(Encoding.ASCII.GetBytes(m1), 0, m1.Length);

            _tcpListener.Start();
            TcpClient dataclient = _tcpListener.AcceptTcpClient();

            //var processInfo = new ProcessStartInfo("cmd", "/c dir C:");

            //processInfo.RedirectStandardOutput = true;
            //processInfo.UseShellExecute = false;
            //processInfo.CreateNoWindow = true;

            //var process = new Process();
            //process.StartInfo = processInfo;

            //process.Start();


            string output = "04-27-00 12:09PM <DIR> 16-dos-dateambiguous dir\n";
            output += "02-25-01 20:03 <DIR> dir2\n"; 

            //client.GetStream().Write(Encoding.ASCII.GetBytes(m1), 0, m1.Length);
            
            dataclient.GetStream().Write(Encoding.ASCII.GetBytes(output), 0, output.Length);

            dataclient.Close();
            _tcpListener.Stop();

            _client.Stream.Write(Encoding.ASCII.GetBytes(m2), 0, m2.Length);

            #endregion
        }

        #region Implementation

        private byte[] Encode(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private string Decode(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        private void SendDataToClient(string data)
        {

        }

        #endregion
    }
}
