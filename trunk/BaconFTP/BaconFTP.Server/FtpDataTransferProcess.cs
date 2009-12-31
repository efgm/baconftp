using System.Text;
using BaconFTP.Data.Logger;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;

namespace BaconFTP.Server
{
    internal class FtpDataTransferProcess
    {
        private readonly FtpClient _client;
        private readonly ILogger _logger;
        private readonly TcpListener _tcpListener;
        private readonly int _dataPort;

        internal FtpDataTransferProcess(FtpClient client, ILogger logger, int dataPort)
        {
            _client = client;
            _logger = logger;
            _dataPort = dataPort;
            _tcpListener = new TcpListener(IPAddress.Any, dataPort);
        }

        internal void SendDirectoryListing(object dir)
        {
            SendMessageToClient(Const.OpeningDataConnectionMessage("BINARY"));

            _tcpListener.Start();
            TcpClient dataClient = _tcpListener.AcceptTcpClient();

            SendDataToClient(dataClient, GenerateDirectoryList(dir as string));

            dataClient.Close();
            _tcpListener.Stop();

            SendMessageToClient(Const.TransferCompleteMessage);
        }

        #region Implementation

        private string GenerateDirectoryList(string dir)
        {
            StringBuilder sb = new StringBuilder();
            DirectoryInfo di = new DirectoryInfo(FtpServer.GetRealPath(dir));

            foreach (DirectoryInfo d in di.GetDirectories())
            {
                sb.AppendLine(String.Format("{0}-{1}-{2} {3}:{4} <DIR> {5}", d.LastWriteTime.Month,
                                            d.LastWriteTime.Day, d.LastWriteTime.Year,
                                            d.LastWriteTime.Hour, d.LastWriteTime.Minute,
                                            d.Name));
            }

            return sb.ToString();
        }

        private byte[] Encode(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private string Decode(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        private void SendDataToClient(TcpClient dataClient, string data)
        {
            dataClient.GetStream().Write(Encode(data), 0, data.Length);
        }

        private void SendMessageToClient(string message)
        {
            _client.Stream.Write(Encode(message), 0, message.Length);
        }

        #endregion
    }
}
