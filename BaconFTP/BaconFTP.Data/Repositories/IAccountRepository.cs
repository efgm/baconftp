using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconFTP.Data.Repositories
{
    public interface IAccountRepository
    {
        Account[] GetAll();
        Account GetByUsername(string username);

        void Add(Account item);
        void Edit(Account item);
        void Remove(Account item);
    }
}
