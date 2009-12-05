using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Server
{
    internal interface IFtpProtocol
    {
        bool PerformHandShake();
        void ListenForCommands();
    }
}
