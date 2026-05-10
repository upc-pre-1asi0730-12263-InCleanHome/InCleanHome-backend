namespace InCleanHome.API.Payments.Domain.Model.ValueObjects;

public static class PaymentMethodType
{
    public const string Cash         = "cash";
    public const string Card         = "card";
    public const string Yape         = "yape";
    public const string Plin         = "plin";
    public const string BankTransfer = "bank_transfer";

    public static readonly string[] All = { Cash, Card, Yape, Plin, BankTransfer };

    public static bool IsValid(string t) => All.Contains(t);
}
