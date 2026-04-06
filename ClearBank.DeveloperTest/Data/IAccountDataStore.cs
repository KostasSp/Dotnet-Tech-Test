using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Data
{
    public interface IAccountDataStore
    {
        public Account GetAccount(string accountNumber);
        public void UpdateAccount(Account account);
    }
}
