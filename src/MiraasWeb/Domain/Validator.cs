using MiraasWeb.Abstractions;

namespace MiraasWeb.Domain;

// public because of tests
public class Validator
{
    readonly List<string> errors = new();
    readonly InheritanceCase inheritanceCase;

    public Validator(InheritanceCase inheritanceCase)
    {
        this.inheritanceCase = inheritanceCase; // passing null is ok, we will validate it as a failure
    }

    void validateHeirs()
    {
        foreach (var heir in inheritanceCase.Heirs)
        {
            if (heir.Count < 1)
                errors.Add($"{heir.Relation} count must be at least 1.");
            else if (heir.Count < 0)
                errors.Add($"{heir.Relation} count cannot be negative.");
        }
    }

    void validateWives()
    {
        int wifeCount = inheritanceCase.GetHeirCount(RelationType.Wife);

        if (wifeCount > 0 && inheritanceCase.Deceased.Gender != Gender.Male)
            errors.Add($"Deceased must be male to have wives. Found: {wifeCount} wives.");
        else if (wifeCount > 4)
            errors.Add($"Cannot have more than 4 wives. Found: {wifeCount}");
    }

    void validateHusband()
    {
        int husbandCount = inheritanceCase.GetHeirCount(RelationType.Husband);

        if (husbandCount > 0 && inheritanceCase.Deceased.Gender != Gender.Female)
            errors.Add($"Deceased must be female to have husband.");
        else if (husbandCount > 1)
            errors.Add($"Cannot have more than 1 husband. Found: {husbandCount}");
    }

    void validateParents()
    {
        int fatherCount = inheritanceCase.GetHeirCount(RelationType.Father);
        int motherCount = inheritanceCase.GetHeirCount(RelationType.Mother);

        if (fatherCount > 1)
            errors.Add($"Cannot have more than 1 father. Found: {fatherCount}");

        if (motherCount > 1)
            errors.Add($"Cannot have more than 1 mother. Found: {motherCount}");
    }

    void validateGrandParents()
    {
        int fatherCount = inheritanceCase.GetHeirCount(RelationType.Grandfather);
        int paternalMotherCount = inheritanceCase.GetHeirCount(RelationType.GrandmotherPaternal);
        int maternalMotherCount = inheritanceCase.GetHeirCount(RelationType.GrandmotherMaternal);

        if (fatherCount > 1)
            errors.Add($"Cannot have more than 1 grand father. Found: {fatherCount}");

        if (paternalMotherCount > 1)
            errors.Add($"Cannot have more than 1 paternal grand mother. Found: {paternalMotherCount}");

        if (maternalMotherCount > 1)
            errors.Add($"Cannot have more than 1 maternal grand mother. Found: {maternalMotherCount}");
    }

    void validateAscendants()
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

    public ValidationResult Validate()
    {
        if (inheritanceCase == null)
            return ValidationResult.Failure("Inheritance case cannot be null.");

        validateHeirs();
        validateWives();
        validateHusband();
        validateParents();
        validateGrandParents();
        validateAscendants();

        if (errors.Count > 0)
            return ValidationResult.Failure(errors);

        return ValidationResult.Success();
    }
}
