using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Data.Exceptions
{
    public class LoggingMethodUnknownException : Exception
    {
        public LoggingMethodUnknownException()
            : base(Const.UnknownLoggingMethodExceptionMessage)
        { }            
    }

    public class ServerPortElementNotFoundException : Exception
    {
        public ServerPortElementNotFoundException()
            : base(Const.ServerPortElementNotFoundExceptionMessage)
        { }
    }

    public class DirectoryElementNotFoundException : Exception
    {
        public DirectoryElementNotFoundException()
            : base(Const.ServerPortElementNotFoundExceptionMessage)
        { }
    }

    public class LoggingElementNotFoundException : Exception
    {
        public LoggingElementNotFoundException()
            : base(Const.LoggingElementNotFoundExceptionMessasge)
        { }
    }

    public class ConfigurationFileIsEmptyException : Exception
    {
        public ConfigurationFileIsEmptyException()
            : base(Const.ConfigurationFileEmptyExceptionMessage)
        { }
    }
}
