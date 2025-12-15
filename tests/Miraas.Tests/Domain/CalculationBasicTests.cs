using MiraasWeb.Abstractions;
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class CalculationBasicTests
{
    CalculationEngine engine;

    [SetUp]
    public void Setup()
    {
        engine = new CalculationEngine();
    }

    [Test]
    public void Calculate_SingleSon_SonGetsAll()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs.Count, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Result.Fraction, Is.EqualTo(Fraction.One));
    }

    [Test]
    public void Calculate_SingleDaughter_DaughterGetsAll()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs[0].Result.Fraction, Is.EqualTo(Fraction.One));
    }

    [Test]
    public void Calculate_WifeAndSon_CorrectShares()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs.Count, Is.EqualTo(2));

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var son = result.Heirs.First(h => h.Relation == RelationType.Son);

        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth));

        Assert.That(son.Result.Fraction, Is.EqualTo(new Fraction(7, 8)));
    }

    [Test]
    public void Calculate_WifeAndDaughter_CorrectShares()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth));
        Assert.That(daughter.Result.Fraction, Is.EqualTo(new Fraction(7, 8)));
    }

    [Test]
    public void Calculate_TwoWivesAndSon_WivesShareEqually()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(2));
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wife.Count, Is.EqualTo(2));
    }

    [Test]
    public void Calculate_SonAndDaughter_SonGetsTwiceAsDaughtersShare()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var son = result.Heirs.First(h => h.Relation == RelationType.Son);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        var sonShare = son.Result.Fraction.ToDecimal();
        var daughterShare = daughter.Result.Fraction.ToDecimal();

        Assert.That(sonShare, Is.EqualTo(daughterShare * 2).Within(0.001m));
    }

    [Test]
    public void Calculate_FatherAndSon_FatherGetsSixth()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.Sixth));
        // son gets 5/6
    }

    [Test]
    public void Calculate_FatherAlone_FatherGetsAll()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.One));
    }

    [Test]
    public void Calculate_MotherAndSon_MotherGetsSixth()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.Sixth));
        // son gets 5/6
    }

    [Test]
    public void Calculate_MotherAlone_MotherGetsAll()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.One));
    }

    [Test]
    public void Calculate_FatherAndGrandfather_GrandfatherBlocked()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Grandfather());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs.Count, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Relation, Is.EqualTo(RelationType.Father));
    }

    [Test]
    public void Calculate_SonAndBrother_BrotherBlocked()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new FullBrother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs.Count, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Relation, Is.EqualTo(RelationType.Son));
    }

    [Test]
    public void Calculate_ComplexScenario_CorrectDistribution()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Daughter(2));
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs.Count, Is.EqualTo(3));

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var daughters = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth));
        Assert.That(mother.Result.Fraction, Is.GreaterThanOrEqualTo(Fraction.Sixth));
        Assert.That(Fraction.One - wife.Result.Fraction - mother.Result.Fraction, Is.EqualTo(daughters.Result.Fraction));
    }

    [Test]
    public void Calculate_UterineSiblings_CorrectShare()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother(2));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var utBrother = result.Heirs.First(h => h.Relation == RelationType.UterineBrother);
        Assert.That(utBrother.Result.Fraction, Is.EqualTo(Fraction.One)); // 1/3 + Radd
    }

    [Test]
    public void Calculate_TotalFractionEqualsOne()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One));
    }

    [Test]
    public void Calculate_ResultHasExpectedProperties()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs, Is.Not.Null);
        Assert.That(result.Heirs.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Calculate_NoHeirs_ReturnsFail()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.False);

        Assert.That(result.ErrorMessage?.Contains("heirs"), Is.True);
    }

    [Test]
    public void Calculate_FiveWives_ReturnsFail()
    {
        var deceased = new DeceasedPerson(GenderType.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        var wife = new Wife();
        wife.Count = 5;
        inheritanceCase.AddHeir(wife);

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.False);
    }
}
