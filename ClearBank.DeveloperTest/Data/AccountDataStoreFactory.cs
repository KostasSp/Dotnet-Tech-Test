using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace ClearBank.DeveloperTest.Data
{
    public class AccountDataStoreFactory : IAccountDataStoreFactory
    {
        private readonly IConfiguration _configuration;

        public AccountDataStoreFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IAccountDataStore Create()
        {
            var dataStoreType = _configuration["DataStoreType"];
            return dataStoreType == "Backup"
                ? new BackupAccountDataStore()
                : new AccountDataStore();
        }
    }
}
