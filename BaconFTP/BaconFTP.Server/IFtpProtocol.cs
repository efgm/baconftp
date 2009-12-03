using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Server
{
    internal interface IFtpProtocol
    {
        void PerformHandShake();
        void ListenForCommands();
    }
}
