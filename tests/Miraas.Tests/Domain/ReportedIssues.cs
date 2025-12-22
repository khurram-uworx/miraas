using MiraasWeb.Abstractions;
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class ReportedIssues
{
    CalculationEngine engine = new();

    [Test]
    public void Issue3_ConsanguineBrother_ShouldGetViaRadd()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new ConsanguineSister());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var sister = result.Heirs.First(h => h.Relation == RelationType.FullSister);
        var conBrother = result.Heirs.First(h => h.Relation == RelationType.ConsanguineBrother);
        var conSister = result.Heirs.First(h => h.Relation == RelationType.ConsanguineSister);
        Assert.That(sister.Result.Fraction, Is.EqualTo(Fraction.Half), "Consanguine sister should get 1/2");
        Assert.That(conBrother.Result.Fraction, Is.EqualTo(Fraction.Third), "Consanguine brother should get 1/3");
        Assert.That(conSister.Result.Fraction, Is.EqualTo(Fraction.One - Fraction.Half - Fraction.Third), "Consanguine sister should get rest of it");
    }

    #region Spouse as Residual Heir Tests

    [Test]
    public void SpouseAsResiduary_WifeAlone_ShouldGetAllWithResiduary()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.One), 
            "Wife as only heir should get 100% (1/4 fixed share + 3/4 residuary)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total fraction should equal 1");
    }

    [Test]
    public void SpouseAsResiduary_HusbandAlone_ShouldGetAllWithResiduary()
    {
        var deceased = new DeceasedPerson(GenderType.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        Assert.That(husband.Result.Fraction, Is.EqualTo(Fraction.One), 
            "Husband as only heir should get 100% (1/2 fixed share + 1/2 residuary)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total fraction should equal 1");
    }

    [Test]
    public void SpouseAsResiduary_MultipleWivesAlone_ShouldShareAllWithResiduary()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(2));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wives = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wives.Result.Fraction, Is.EqualTo(Fraction.One), 
            "Multiple wives as only heirs should share 100% (1/4 fixed share + 3/4 residuary, divided equally)");
        Assert.That(wives.Count, Is.EqualTo(2), "Wife count should be 2");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total fraction should equal 1");
    }

    [Test]
    public void SpouseAsResiduary_ThreeWivesAlone_ShouldShareAllWithResiduary()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(3));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wives = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wives.Result.Fraction, Is.EqualTo(Fraction.One), 
            "Multiple wives as only heirs should share 100% total");
        Assert.That(wives.Count, Is.EqualTo(3), "Wife count should be 3");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total fraction should equal 1");
    }

    [Test]
    public void SpouseNotResiduary_WifeWithMother_WifeStaysFixed()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Quarter), 
            "Wife should get fixed 1/4 share (NOT residuary since mother exists)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void SpouseNotResiduary_WifeWithFather_WifeStaysFixed()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Father());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var father = result.Heirs.First(h => h.Relation == RelationType.Father);

        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Quarter), 
            "Wife should get fixed 1/4 share (NOT residuary since father exists)");
        Assert.That(father.Result.Fraction, Is.EqualTo(new Fraction(3, 4)), 
            "Father should get residuary 3/4");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void SpouseNotResiduary_HusbandWithFather_HusbandStaysFixed()
    {
        var deceased = new DeceasedPerson(GenderType.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Father());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        var father = result.Heirs.First(h => h.Relation == RelationType.Father);

        Assert.That(husband.Result.Fraction, Is.EqualTo(Fraction.Half), 
            "Husband should get fixed 1/2 share (NOT residuary since father exists)");
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.Half), 
            "Father should get residuary 1/2");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void SpouseNotResiduary_HusbandWithMother_HusbandStaysFixed()
    {
        var deceased = new DeceasedPerson(GenderType.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        Assert.That(husband.Result.Fraction, Is.EqualTo(Fraction.Half), 
            "Husband should get fixed 1/2 share (NOT residuary since mother exists)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void SpouseNotResiduary_WifeWithSon_WifeStaysFixed()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth), 
            "Wife should get fixed 1/8 share (NOT residuary since son exists)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void SpouseNotResiduary_HusbandWithDaughter_HusbandStaysFixed()
    {
        var deceased = new DeceasedPerson(GenderType.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        Assert.That(husband.Result.Fraction, Is.EqualTo(Fraction.Quarter), 
            "Husband should get fixed 1/4 share (NOT residuary since daughter exists)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    #endregion
}
