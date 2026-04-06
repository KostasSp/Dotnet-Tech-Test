using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.PaymentValidators;
using ClearBank.DeveloperTest.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace ClearBank.DeveloperTest.Tests;

public class PaymentServiceTests
{
    [Fact]
    public void MakePayment_ReturnsFailure_WhenNoRuleExistsForPaymentScheme()
    {
        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock.Setup(x => x.GetAccount("111")).Returns(new Account());

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock.Setup(x => x.Create()).Returns(dataStoreMock.Object);

        var ruleMock = new Mock<IPaymentValidator>();
        ruleMock.SetupGet(x => x.PaymentScheme).Returns(PaymentScheme.Chaps);

        var sut = new PaymentService(factoryMock.Object, new[] { ruleMock.Object });

        var result = sut.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            PaymentScheme = PaymentScheme.Bacs
        });

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    #region BACS
    [Fact]
    public void MakePayment_ReturnsFailure_WhenAccountDoesNotAllowBacs()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Bacs
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock.Setup(x => x.GetAccount("111")).Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock.Setup(x => x.Create()).Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new BacsPaymentValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }


    [Fact]
    public void MakePayment_ReturnsSuccess_WhenAccountAllowsBacs()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Bacs
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock.Setup(x => x.GetAccount("111")).Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock.Setup(x => x.Create()).Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new BacsPaymentValidator() });

        var result = sut.MakePayment(request);

        Assert.True(result.Success);
        Assert.Equal(90, account.Balance);
        dataStoreMock.Verify(x => x.UpdateAccount(account), Times.Once);
    }
    #endregion

    #region CHAPS
    [Fact]
    public void MakePayment_ReturnsFailure_WhenAccountIsNull_ForChaps()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Chaps
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns((Account)null);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new ChapsPaymentValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ReturnsFailure_WhenAccountDoesNotAllowChaps()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Chaps
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new ChapsPaymentValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ReturnsFailure_WhenAccountStatusIsNotLive_ForChaps()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Chaps
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Disabled
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new ChapsPaymentValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ReturnsSuccess_WhenAccountAllowsChaps_AndStatusIsLive()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Chaps
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new ChapsPaymentValidator() });

        var result = sut.MakePayment(request);

        Assert.True(result.Success);
        Assert.Equal(90, account.Balance);
        dataStoreMock.Verify(x => x.UpdateAccount(account), Times.Once);
    }
    #endregion

    #region FasterPayments
    [Fact]
    public void MakePayment_ReturnsFailure_WhenAccountIsNull_ForFasterPayments()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns((Account)null);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new FasterPaymentsValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ReturnsFailure_WhenAccountDoesNotAllowFasterPayments()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new FasterPaymentsValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ReturnsFailure_WhenBalanceIsLessThanAmount_ForFasterPayments()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 101,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new FasterPaymentsValidator() });

        var result = sut.MakePayment(request);

        Assert.False(result.Success);
        dataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ReturnsSuccess_WhenAccountAllowsFasterPayments_AndBalanceIsSufficient()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
        };

        var dataStoreMock = new Mock<IAccountDataStore>();
        dataStoreMock
            .Setup(x => x.GetAccount("111"))
            .Returns(account);

        var factoryMock = new Mock<IAccountDataStoreFactory>();
        factoryMock
            .Setup(x => x.Create())
            .Returns(dataStoreMock.Object);

        var sut = new PaymentService(
            factoryMock.Object,
            new IPaymentValidator[] { new FasterPaymentsValidator() });

        var result = sut.MakePayment(request);

        Assert.True(result.Success);
        Assert.Equal(90, account.Balance);
        dataStoreMock.Verify(x => x.UpdateAccount(account), Times.Once);
    }
    #endregion

    #region Factory tests
    [Fact]
    public void MakePayment_UsesDataStoreReturnedByFactory()
    {
        var dataStore = new Mock<IAccountDataStore>();
        var factory = new Mock<IAccountDataStoreFactory>();
        var validator = new Mock<IPaymentValidator>();

        factory.Setup(x => x.Create()).Returns(dataStore.Object);

        var account = new Account
        {
            AccountNumber = "111",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        };

        dataStore.Setup(x => x.GetAccount("111")).Returns(account);
        validator.SetupGet(x => x.PaymentScheme).Returns(PaymentScheme.Bacs);
        validator.Setup(x => x.IsValid(account, It.IsAny<MakePaymentRequest>())).Returns(true);

        var sut = new PaymentService(factory.Object, new[] { validator.Object });

        sut.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "111",
            Amount = 10,
            PaymentScheme = PaymentScheme.Bacs
        });

        factory.Verify(x => x.Create(), Times.Once);
        dataStore.Verify(x => x.GetAccount("111"), Times.Once);
    }


    [Fact]
    public void Create_ReturnsBackupAccountDataStore_WhenDataStoreTypeIsBackup()
    {
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["DataStoreType"]).Returns("Backup");

        var sut = new AccountDataStoreFactory(configurationMock.Object);

        var result = sut.Create();

        Assert.IsType<BackupAccountDataStore>(result);
    }

    [Fact]
    public void Create_ReturnsAccountDataStore_WhenDataStoreTypeIsNotBackup()
    {
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["DataStoreType"]).Returns("Primary");

        var sut = new AccountDataStoreFactory(configurationMock.Object);

        var result = sut.Create();

        Assert.IsType<AccountDataStore>(result);
    }

    [Fact]
    public void Create_ReturnsAccountDataStore_WhenDataStoreTypeIsMissing()
    {
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["DataStoreType"]).Returns((string)null);

        var sut = new AccountDataStoreFactory(configurationMock.Object);

        var result = sut.Create();

        Assert.IsType<AccountDataStore>(result);
    }
    #endregion
}