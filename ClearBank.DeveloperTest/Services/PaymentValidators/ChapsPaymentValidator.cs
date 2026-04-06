using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentValidators
{
    public class ChapsPaymentValidator : IPaymentRule
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Chaps;

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
