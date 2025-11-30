namespace MiraasWeb.Domain;

/// <summary>
/// Validates inheritance cases against Islamic inheritance law rules.
/// </summary>
public class InheritanceValidator
{
    /// <summary>
    /// Validates basic heir properties.
    /// </summary>
    void validateHeirs(InheritanceCase inheritanceCase, List<string> errors)
    {
        foreach (var heir in inheritanceCase.Heirs)
        {
            if (heir.Count < 1)
                errors.Add($"{heir.Relation} count must be at least 1.");

            if (heir.Count < 0)
                errors.Add($"{heir.Relation} count cannot be negative.");
        }
    }

    /// <summary>
    /// Validates wife count (max 4 per Islamic law).
    /// </summary>
    void validateWives(InheritanceCase inheritanceCase, List<string> errors)
    {
        int wifeCount = inheritanceCase.GetHeirCount(RelationType.Wife);
        if (wifeCount > 0 && inheritanceCase.Deceased.Gender != Gender.Male)
            errors.Add($"Deceased must be male to have wives. Found: {wifeCount} wives.");
        if (wifeCount > 4)
            errors.Add($"Cannot have more than 4 wives. Found: {wifeCount}");
    }

    /// <summary>
    /// Validates husband count (max 1 per Islamic law).
    /// </summary>
    void validateHusband(InheritanceCase inheritanceCase, List<string> errors)
    {
        int husbandCount = inheritanceCase.GetHeirCount(RelationType.Husband);
        if (husbandCount > 0 && inheritanceCase.Deceased.Gender != Gender.Female)
            errors.Add($"Deceased must be female to have husband.");
        if (husbandCount > 1)
            errors.Add($"Cannot have more than 1 husband. Found: {husbandCount}");
    }

    /// <summary>
    /// Validates ascendants configuration.
    /// </summary>
    void validateAscendants(InheritanceCase inheritanceCase, List<string> errors)
    {
        int fatherCount = inheritanceCase.GetHeirCount(RelationType.Father);
        int motherCount = inheritanceCase.GetHeirCount(RelationType.Mother);
        int grandFatherCount = inheritanceCase.GetHeirCount(RelationType.Grandfather);
        if (fatherCount > 1)
            errors.Add($"Cannot have multiple fathers. Found: {fatherCount}");
        if (motherCount > 1)
            errors.Add($"Cannot have multiple mothers. Found: {motherCount}");
        if (grandFatherCount > 1)
            errors.Add($"Cannot have multiple grand fathers. Found: {grandFatherCount}");
    }

    /// <summary>
    /// Validates an inheritance case for validity.
    /// </summary>
    public ValidationResult Validate(InheritanceCase inheritanceCase)
    {
        if (inheritanceCase == null)
            return ValidationResult.Failure("Inheritance case cannot be null.");

        var errors = new List<string>();

        validateHeirs(inheritanceCase, errors);
        validateWives(inheritanceCase, errors);
        validateHusband(inheritanceCase, errors);
        validateAscendants(inheritanceCase, errors);

        if (errors.Count > 0)
            return ValidationResult.Failure(errors);

        return ValidationResult.Success();
    }
}
