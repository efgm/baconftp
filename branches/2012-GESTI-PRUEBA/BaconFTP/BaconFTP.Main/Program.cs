using System;

using BaconFTP.Server;

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
