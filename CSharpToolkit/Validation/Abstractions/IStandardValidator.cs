namespace CSharpToolkit.Validation.Abstractions {
    using Utilities;
    using Validation;
    public interface IStandardValidator {
        OperationResult ValidateRequiredFieldPresent(string field);
        OperationResult ValidateOnlyValidCharactersPresent(string field);
        OperationResult ValidateEmailAddress(string field);
        OperationResult ValidateNumericRange(RangeDataOrder<long> order);
    }
}
