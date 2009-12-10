using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BaconFTP.Data.Logger
{
    public class FileLogger : ILogger
    {
        private const string _logFile = "baconftpd.log";
        
        #region ILogger Members

        public void Write(string message, params object[] args)
        {
            lock (this)
            {
                using (StreamWriter sw = File.AppendText(_logFile))
                {
                    sw.WriteLine("** " + DateTime.Now + ": " + String.Format(message, args) + " **");
                }
            }
        }

        #endregion
    }
}
