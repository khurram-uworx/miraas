namespace MiraasWeb.Domain;

/// <summary>
/// Result of validating an inheritance case.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates if the inheritance case is valid.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// List of validation errors if any.
    /// </summary>
    public List<string> Errors { get; }

    public ValidationResult(bool isValid = true, List<string>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }

    public static ValidationResult Success()
    {
        return new ValidationResult(true);
    }

    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult(false, new List<string>(errors));
    }

    public static ValidationResult Failure(List<string> errors)
    {
        return new ValidationResult(false, errors);
    }

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public override string ToString()
    {
        if (IsValid)
            return "Validation passed.";

        return $"Validation failed with {Errors.Count} error(s):\n" + 
               string.Join("\n", Errors.Select(e => $"- {e}"));
    }
}
