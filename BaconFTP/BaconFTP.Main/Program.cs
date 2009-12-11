using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BaconFTP.Server;
using BaconFTP.Data.Repositories;
using BaconFTP.Data;
using System.Threading;

namespace BaconFTP.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FtpServer.Start();
            }
            catch (TypeInitializationException e)
            {
                Console.WriteLine(Const.FatalErrorFormatString, e.InnerException.Message);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(Const.FatalErrorFormatString, e.Message);
                Console.ReadKey();
            }
        }
    }
}
