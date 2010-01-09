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
        private readonly string _transferType;
        private readonly string _currentDirectory;

        internal FtpDataTransferProcess(FtpClient client, ILogger logger, 
                                        int dataPort, string transferType, string directory)
        {
            _client = client;
            _logger = logger;
            _dataPort = dataPort;
            _tcpListener = new TcpListener(IPAddress.Any, dataPort);
            _transferType = transferType == "I" ? "BINARY" : "ASCII";
            _currentDirectory = directory;
        }

        internal void SendDirectoryListing(object dir)
        {
            OpenDataConnection("LIST", dir);
        }

        internal void SendFileToClient(object file)
        {
            OpenDataConnection("RETR", file);
        }

        internal void GetFileFromClient(object file)
        {
            OpenDataConnection("STOR", file);
        }

        #region Implementation

        private void OpenDataConnection(string type, object file)
        {
            //hacer el lock para que no ocurran colisiones de 2 o mas clientes tratando de conectarse
            //al mismo puerto provisto por el comando PASV
            lock (this)
            {
                SendMessageToClient(Const.OpeningDataConnectionMessage(_transferType));                

                try
                {
                    _tcpListener.Start();

                    using (TcpClient dataClient = _tcpListener.AcceptTcpClient())
                    {
                        _logger.Write("Opening data connection with {0} on port {1}.", dataClient.Client.RemoteEndPoint, _dataPort);

                        if (type == "STOR")
                            GetFileFromClient(dataClient, file as string);
                        else if (type == "RETR")
                            SendFileToClient(dataClient, file as string);
                        else if (type == "LIST")
                        {
                            _logger.Write("Sending directory listing to {0}.", dataClient.Client.RemoteEndPoint);
                            SendDataToClient(dataClient, GenerateDirectoryList(file as string));
                        }

                        _logger.Write("Closing data connection with {0}.", dataClient.Client.RemoteEndPoint);
                    }
                }
                catch (SocketException)
                {
                    _tcpListener.Stop();
                    SendMessageToClient(Const.CannotOpenDataConnectionMessage);

                    return;
                }

                _tcpListener.Stop();
                SendMessageToClient(Const.TransferCompleteMessage);
            }
        }

        private string GenerateDirectoryList(string dir)
        {
            StringBuilder sb = new StringBuilder();
            DirectoryInfo di = new DirectoryInfo(FtpServer.GetRealPath(dir));

            DirectoryInfo[] directoriesList;
            FileInfo[] filesList;

            try
            {
                directoriesList = di.GetDirectories();
                filesList = di.GetFiles();
            }
            catch { return string.Empty; }
            
            if (directoriesList.Length > 0)
            {
                foreach (DirectoryInfo d in directoriesList)
                    sb.AppendLine(String.Format("{0}-{1}-{2} {3}:{4} <DIR> {5}", d.LastWriteTime.Month,
                                                d.LastWriteTime.Day, d.LastWriteTime.Year,
                                                d.LastWriteTime.Hour, d.LastWriteTime.Minute,
                                                d.Name));
            }

            if (filesList.Length > 0)
            {
                foreach (FileInfo f in filesList)
                    sb.AppendLine(String.Format("{0}-{1}-{2} {3}:{4} {5} {6}", f.LastWriteTime.Month,
                                                f.LastWriteTime.Day, f.LastWriteTime.Year,
                                                f.LastWriteTime.Hour, f.LastWriteTime.Minute,
                                                f.Length, f.Name));
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

        private void SendFileToClient(TcpClient dataClient, string file)
        {
            try
            {
                using (FileStream fs = new FileInfo(file).OpenRead())
                {
                    byte[] buffer = new byte[Const.BlockSize];
                    int bytesRead;
                    Stream clientStream = dataClient.GetStream();

                    _logger.Write("Starting transfer of file '{0}' with {1}.", file, dataClient.Client.RemoteEndPoint);

                    try
                    {
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                            clientStream.Write(buffer, 0, bytesRead);
                    }
                    catch (Exception e)
                    {
                        _logger.Write("Error: {0}.", e.Message);
                        SendMessageToClient(Const.DataConnectionErrorMessage);
                        return;
                    }

                    _logger.Write("File '{0}' successfully transfered to {1}.", file, dataClient.Client.RemoteEndPoint);
                }
            }
            catch (Exception e)
            {
                _logger.Write("Error: {0}.", e.Message);
                SendMessageToClient(Const.DataConnectionErrorMessage);
                return;
            }
        }

        private void GetFileFromClient(TcpClient dataClient, string file)
        {
            try
            {
                using (FileStream fs = new FileStream(FtpServer.GetRealPath(_currentDirectory + "/" + file),
                                                      FileMode.Create))
                {
                    byte[] buffer = new byte[Const.BlockSize];
                    int bytesRead;
                    Stream clientStream = dataClient.GetStream();

                    _logger.Write("Recieving file '{0}' from {1}.", file, dataClient.Client.RemoteEndPoint);

                    try
                    {
                        while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) != 0)
                            fs.Write(buffer, 0, bytesRead);
                    }
                    catch (Exception e)
                    {
                        _logger.Write("Error: {0}.", e.Message);
                        SendMessageToClient(Const.DataConnectionErrorMessage);
                        return;
                    }

                    _logger.Write("Successfully recieved File '{0}' from {1}.", file, dataClient.Client.RemoteEndPoint);
                }
            }
            catch (Exception e)
            {
                _logger.Write("Error: {0}.", e.Message);
                SendMessageToClient(Const.DataConnectionErrorMessage);
                return;
            }
        }

        private void SendMessageToClient(string message)
        {
            _client.Stream.Write(Encode(message), 0, message.Length);
        }

        #endregion
    }
}
