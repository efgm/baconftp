using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Data
{
    public class Account
    {
        public Guid AccountID { get; internal set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Account()
        {
            AccountID = Guid.NewGuid();
        }

        public Account(string username, string password)
            : this()         
        {
            Username = username;
            Password = password;
        }
    }
}
