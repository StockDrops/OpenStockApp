namespace OpenStockApp.Contracts;

internal interface IValidationRule
{
    string ValidationMessage { get; set; }
    bool IsValid { get; set; }
}
