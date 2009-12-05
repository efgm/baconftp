using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace BaconFTP.Data.Repositories
{
    //rep para manejar el xml de las cuentas
    public class AccountRepository : IAccountRepository
    {
        private readonly XDocument _accountsXmlFile;
        private readonly XElement _root;
        private const string _pathToXmlFile = "accounts.xml";

        public AccountRepository()
        {
            if (!File.Exists(_pathToXmlFile))
                GenerateAccountsFile();

            _accountsXmlFile = XDocument.Load(_pathToXmlFile);
            _root = _accountsXmlFile.Root;
        }

        public Account GetByUsername(string username)
        {
            try
            {
                return (from a in _root.Elements()
                        where a.Attribute("username").Value == username
                        select new Account
                        {
                            AccountID = new Guid(a.Attribute("id").Value),
                            Username = username,
                            Password = a.Attribute("password").Value                            
                        }).Single();
            }
            catch { return null; }
        }

        public void Add(Account a)
        {
            _root.Add(new XElement("account",
                                   new XAttribute("id", a.AccountID),
                                   new XAttribute("username", a.Username),
                                   new XAttribute("password", a.Password)
                                   )
                     );

            _accountsXmlFile.Save(_pathToXmlFile);
        }

        private void GenerateAccountsFile()
        {
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                              new XElement("accounts"));

            xml.Save(_pathToXmlFile);
        }
    }
}
