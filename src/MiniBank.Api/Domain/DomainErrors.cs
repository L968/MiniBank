namespace MiniBank.Api.Domain;

internal static class DomainErrors
{
    internal static class User
    {
        public const string OnlyCommonUsersCanSendMoney = "Only Common users are allowed to send money.";
        public const string InsufficientBalance = "Insufficient balance.";
        public const string InvalidValue = "Invalid value.";
    }

    internal static class Transaction
    {
        public const string InvalidValue = "Invalid transaction value.";
        public const string OnlyPendingTransactionsCanBeProcessed = "Only pending transactions can be processed.";
        public const string OnlyPendingTransactionsCanFail = "Only pending transactions can fail.";
        public const string OnlyCompletedTransactionsCanBeReverted = "Only completed transactions can be reverted.";
        public const string SamePayerAndPayee = "The payer and the payee cannot be the same person.";
    }
}
