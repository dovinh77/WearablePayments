namespace WearablePayments.API.Models;

public record AddCardRequest(
    string Pan,           // full PAN — we'll mask it before storing
    string CardholderName,
    string ExpiryMonth,
    string ExpiryYear,
    string Network);      // "Visa", "Mastercard", "Amex"
