using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace BaconFTP.Server
{
    public class FtpServer
    {
        #region Properties
        
        private readonly TcpListener _tcpListener;
        private readonly Thread _listenerThread;
        private readonly List<FtpClient> _connectedClients = new List<FtpClient>();
        
        #endregion //Properties

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
            //por ahora nama pa proba, aqui iria instanciar el protocolo y k el resuelva
            FtpClient ftpClient = (FtpClient)client;

            FtpProtocol protocol = new FtpProtocol(ftpClient);

            protocol.PerformHandShake();

            byte[] buffer = new byte[1024];
                        
            ftpClient.Stream.Read(buffer, 0, 1024);            

            Console.WriteLine("Read: {0}\n", Encoding.ASCII.GetString(buffer));
        }

        #endregion //implementation
    }
}
