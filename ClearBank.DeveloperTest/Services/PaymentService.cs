using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.PaymentValidators;
using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStoreFactory _dataStoreFactory;
        private readonly IReadOnlyDictionary<PaymentScheme, IPaymentValidator> _paymentValidators;

        public PaymentService(IAccountDataStoreFactory dataStoreFactory, IEnumerable<IPaymentValidator> paymentValidators)
        {
            _dataStoreFactory = dataStoreFactory ?? throw new ArgumentNullException(nameof(dataStoreFactory));
            _paymentValidators = paymentValidators?.ToDictionary(v => v.PaymentScheme) 
                ?? throw new ArgumentNullException(nameof(paymentValidators));
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStore = _dataStoreFactory.Create();
            var account = dataStore.GetAccount(request.DebtorAccountNumber);

            if (!_paymentValidators.TryGetValue(request.PaymentScheme, out var validator))
                return new MakePaymentResult { Success = false };

            if (!validator.IsValid(account, request))
                return new MakePaymentResult { Success = false };

            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);

            return new MakePaymentResult { Success = true };
        }
    }
}
