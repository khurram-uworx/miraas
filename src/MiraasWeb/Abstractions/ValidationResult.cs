namespace MiraasWeb.Abstractions;

// public because of tests
public class ValidationResult
{
    public bool IsValid { get; }

    public List<string> Errors { get; }

    public ValidationResult(bool isValid = true, List<string>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }

    public void AddError(string error) =>
        Errors.Add(error);

    public override string ToString() =>
        IsValid
            ? "Validation passed."
            : $"Validation failed with {Errors.Count} error(s):\n" +
                string.Join("\n", Errors.Select(e => $"- {e}"));

    // factory methods
    public static ValidationResult Success() =>
        new ValidationResult(true);
    public static ValidationResult Failure(params string[] errors) =>
        new ValidationResult(false, new List<string>(errors));
    public static ValidationResult Failure(List<string> errors) =>
        new ValidationResult(false, errors);
}
