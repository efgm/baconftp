﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using BaconFTP.Data.Logger;

namespace BaconFTP.Server
{
    public class FtpServer
    {
        #region Fields
        
        private readonly TcpListener _tcpListener;
        private readonly Thread _listenerThread;
        private readonly List<FtpClient> _connectedClients = new List<FtpClient>();
        
        //por ahora, despues hay que implementar uno para hacerlo en un archivo.
        private readonly ILogger _logger = new ConsoleLogger();
        
        #endregion //Fields

        #region Constructor(s)

        public FtpServer()
            : this(Const.DefaultFtpPort)
        { }

        public FtpServer(int port)
            : this(IPAddress.Any, port)
        { }

        public FtpServer(IPAddress ipAddress, int port)
        {
            _tcpListener = new TcpListener(ipAddress, port);
            _listenerThread = new Thread(this.ListenForConnections);
        }

        #endregion //Constructor(s)

        #region Interface

        public void Start()
        {
            _listenerThread.Start();
        }

        #endregion //Interface

        #region Implementation

        private void ListenForConnections()
        {
            _tcpListener.Start();

            while (true)
            {
                FtpClient ftpClient = new FtpClient(_tcpListener.AcceptTcpClient());

                _connectedClients.Add(ftpClient);

                new Thread(new ParameterizedThreadStart(this.HandleClientConnection)).Start(ftpClient);                
            }
        }

        private void HandleClientConnection(object client)
        {
            FtpClient ftpClient = (FtpClient)client;

            FtpProtocol protocol = new FtpProtocol(ftpClient, _logger);

            if (protocol.PerformHandShake())
                protocol.ListenForCommands();
            else
                CloseConnection(ftpClient);
        }

        private void CloseConnection(FtpClient client)
        {
            _connectedClients.Remove(client);
            client.CloseConnection();
        }

        #endregion //implementation
    }
}
