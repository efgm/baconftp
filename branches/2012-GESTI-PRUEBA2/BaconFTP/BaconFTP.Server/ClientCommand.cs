using System.Collections.Generic;

namespace BaconFTP.Server
{
    internal class ClientCommand
    {
        public string Command { get; private set; }
        public IList<string> Arguments { get; private set; }

        internal ClientCommand(string command, IList<string> args)
        {
            Command = command;
            Arguments = args;
        }

    }
}
