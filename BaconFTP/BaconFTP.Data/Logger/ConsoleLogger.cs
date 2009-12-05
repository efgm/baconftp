using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Data.Logger
{
    public class ConsoleLogger : ILogger
    {
        #region ILogger Members

        public void Write(string message)
        {
            //para evitar que varios threads escriban al mismo tiempo
            lock (this)
            {
                Console.WriteLine("** " + DateTime.Now + ": " + message + " **");
            }                
        }

        #endregion
    }
}
