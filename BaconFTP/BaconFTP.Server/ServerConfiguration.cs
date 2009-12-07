using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BaconFTP.Server
{
    class ServerConfiguration
    {
        public ServerConfiguration()
        {
            //Set current working directory
            Const.CurrentWorkingDirectory = Directory.GetCurrentDirectory();
        }
    }
}
