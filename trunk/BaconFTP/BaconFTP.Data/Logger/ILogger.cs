using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Data.Logger
{
    public interface ILogger
    {
        void Write(string message);
    }
}
