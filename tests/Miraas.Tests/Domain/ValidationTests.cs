using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class ValidationTests
{
    InheritanceValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new InheritanceValidator();
    }

    [Test]
    public void Validate_ValidCase_ReturnsSuccess()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());

        var result = validator.Validate(inheritanceCase);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Count, Is.EqualTo(0));
    }

    [Test]
    public void Validate_NullCase_ReturnsFail()
    {
        var result = validator.Validate(null);

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

        var result = validator.Validate(inheritanceCase);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.GreaterThan(0));
        Assert.That(result.Errors.Any(e => e.Contains("wives", StringComparison.OrdinalIgnoreCase) || 
                                           e.Contains("wife", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_MaxWives_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(4));

        var result = validator.Validate(inheritanceCase);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_TwoHusbands_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Husband());

        var result = validator.Validate(inheritanceCase);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.Contains("husband", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_OneHusband_Succeeds()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());

        var result = validator.Validate(inheritanceCase);

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

        var result = validator.Validate(inheritanceCase);

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

        var result = validator.Validate(inheritanceCase);

        Assert.That(result.IsValid, Is.False);
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
