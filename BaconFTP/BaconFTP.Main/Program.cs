using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BaconFTP.Server;
using BaconFTP.Data.Repositories;
using BaconFTP.Data;

namespace BaconFTP.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            //prueba
            new FtpServer(123).Start();
        }
    }
}
