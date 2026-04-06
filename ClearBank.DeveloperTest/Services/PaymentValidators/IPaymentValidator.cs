using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentValidators
{
    public interface IPaymentValidator
    {
        PaymentScheme PaymentScheme { get; }
        bool IsValid(Account account, MakePaymentRequest request);
    }
}
