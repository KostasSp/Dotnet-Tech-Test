using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentValidators
{
    public class FasterPaymentsValidator : IPaymentRule
    {
        public PaymentScheme PaymentScheme => PaymentScheme.FasterPayments;

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
