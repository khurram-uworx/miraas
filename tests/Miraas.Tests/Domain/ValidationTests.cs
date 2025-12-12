using MiraasWeb.Abstractions;
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class ValidationTests
{
    [Test]
    public void Validate_ValidCase_ReturnsSuccess()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);

        Assert.That(result.Errors.Count, Is.EqualTo(0));
    }

    [Test]
    public void Validate_NullCase_ReturnsFail()
    {
        var validator = new Validator(null);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Validate_WifeCountTooHigh_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        var wife = new Wife();
        wife.Count = 5;
        inheritanceCase.AddHeir(wife);

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Count, Is.GreaterThan(0));
        Assert.That(result.Errors.Any(e => e.Contains("wives", StringComparison.OrdinalIgnoreCase)
        || e.Contains("wife", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_MaxWives_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(4));

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_TwoHusbands_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Husband());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Any(e => e.Contains("husband", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_OneHusband_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_MultipleValidHeirs_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(2));
        inheritanceCase.AddHeir(new Son(3));
        inheritanceCase.AddHeir(new Daughter(2));
        inheritanceCase.AddHeir(new Mother());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_NegativeHeirCount_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        var son = new Son(1);
        son.Count = -1;
        inheritanceCase.AddHeir(son);

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ZeroHeirCount_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        var son = new Son(1);
        son.Count = 0;
        inheritanceCase.AddHeir(son);

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Any(e => e.Contains("count", StringComparison.OrdinalIgnoreCase)
        || e.Contains("zero", StringComparison.OrdinalIgnoreCase)
        || e.Contains("positive", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_MaleDeceasedWithHusband_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        //Deceased must be female to have husband.
        Assert.That(result.Errors.Any(e => e.Contains("husband", StringComparison.OrdinalIgnoreCase)
        && e.Contains("female", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_FemaleDeceasedWithWife_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        //Deceased must be male to have wives. Found: 1 wives.
        Assert.That(result.Errors.Any(e => e.Contains("wives", StringComparison.OrdinalIgnoreCase)
        && e.Contains("male", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_MaleDeceasedWithWife_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_FemaleDeceasedWithHusband_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_MultipleFathers_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Father());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        //Cannot have multiple fathers. Found: {fathersCount}
        Assert.That(result.Errors.Any(e => e.Contains("fathers", StringComparison.OrdinalIgnoreCase)
        && e.Contains("multiple", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_MultipleMothers_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Mother());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        //Cannot have multiple mothers. Found: {motherCount}
        Assert.That(result.Errors.Any(e => e.Contains("mothers", StringComparison.OrdinalIgnoreCase)
        && (e.Contains("multiple", StringComparison.OrdinalIgnoreCase))), Is.True);
    }

    [Test]
    public void Validate_MultipleGrandfathers_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new Grandfather());

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.False);

        //Cannot have multiple grand fathers. Found: {grandFatherCount}
        Assert.That(result.Errors.Any(e => e.Contains("grand fathers", StringComparison.OrdinalIgnoreCase)
        && (e.Contains("multiple", StringComparison.OrdinalIgnoreCase))), Is.True);
    }

    [Test]
    public void Validate_EmptyHeirsList_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        // No heirs added - should be valid (Bayt-ul-Maal case)

        var validator = new Validator(inheritanceCase);
        var result = validator.Validate();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidationResult_Success_IsValid()
    {
        var result = ValidationResult.Success();

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Count, Is.EqualTo(0));
    }

    [Test]
    public void ValidationResult_Failure_IsInvalid()
    {
        var result = ValidationResult.Failure("Error 1", "Error 2");

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(2));
        Assert.That(result.Errors, Does.Contain("Error 1"));
        Assert.That(result.Errors, Does.Contain("Error 2"));
    }

    [Test]
    public void ValidationResult_AddError_ContainsError()
    {
        var result = ValidationResult.Success();
        result.AddError("New error");

        Assert.That(result.Errors, Does.Contain("New error"));
    }
}