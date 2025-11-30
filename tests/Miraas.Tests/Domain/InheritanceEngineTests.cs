/*
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class InheritanceEngineTests
{
    InheritanceEngine engine;

    [SetUp]
    public void Setup()
    {
        engine = new InheritanceEngine();
    }

    // Basic scenarios
    [Test]
    public void Calculate_SingleSon_SonGetsAll()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs.Count, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Result.Fraction.Denominator, Is.EqualTo(1));
    }

    [Test]
    public void Calculate_SingleDaughter_DaughterGetsHalf()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs[0].Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Result.Fraction.Denominator, Is.EqualTo(2));
    }

    // Classic scenario: Wife and Son
    [Test]
    public void Calculate_WifeAndSon_CorrectShares()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs.Count, Is.EqualTo(2));
        
        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var son = result.Heirs.First(h => h.Relation == RelationType.Son);

        // Wife: 1/8
        Assert.That(wife.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(wife.Result.Fraction.Denominator, Is.EqualTo(8));

        // Son: 7/8 (residue)
        Assert.That(son.Result.Fraction.Numerator, Is.EqualTo(7));
        Assert.That(son.Result.Fraction.Denominator, Is.EqualTo(8));
    }

    // Classic scenario: Wife and Daughter
    [Test]
    public void Calculate_WifeAndDaughter_CorrectShares()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        // Wife: 1/8 (daughter is a descendant)
        Assert.That(wife.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(wife.Result.Fraction.Denominator, Is.EqualTo(8));

        // Daughter: 1/2
        Assert.That(daughter.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(daughter.Result.Fraction.Denominator, Is.EqualTo(2));
    }

    // Multiple wives scenario
    [Test]
    public void Calculate_TwoWivesAndSon_WivesShareEqually()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(2));
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        
        // Each wife gets 1/16 (1/8 divided by 2)
        Assert.That(wife.Count, Is.EqualTo(2));
    }

    // Son and Daughter scenario (2:1 ratio)
    [Test]
    public void Calculate_SonAndDaughter_SonGetsTwiceAsDaughtersShare()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var son = result.Heirs.First(h => h.Relation == RelationType.Son);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        // Son should get 2x daughter's share
        var sonShare = son.Result.Fraction.ToDecimal();
        var daughterShare = daughter.Result.Fraction.ToDecimal();
        
        Assert.That(sonShare, Is.EqualTo(daughterShare * 2).Within(0.001m));
    }

    // Father with son
    [Test]
    public void Calculate_FatherAndSon_FatherGetsSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);

        // Father: 1/6
        Assert.That(father.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(father.Result.Fraction.Denominator, Is.EqualTo(6));
    }

    // Father without sons
    [Test]
    public void Calculate_FatherAlone_FatherGetsAll()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        
        // Father is residuary, gets all
        Assert.That(father.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(father.Result.Fraction.Denominator, Is.EqualTo(1));
    }

    // Mother with children
    [Test]
    public void Calculate_MotherAndSon_MotherGetsSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        // Mother: 1/6
        Assert.That(mother.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(mother.Result.Fraction.Denominator, Is.EqualTo(6));
    }

    // Mother alone
    [Test]
    public void Calculate_MotherAlone_MotherGetsThird()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        // Mother: 1/3
        Assert.That(mother.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(mother.Result.Fraction.Denominator, Is.EqualTo(3));
    }

    // Blocking scenario: Father blocks Grandfather
    [Test]
    public void Calculate_FatherAndGrandfather_GrandfatherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Grandfather());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs.Count, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Relation, Is.EqualTo(RelationType.Father));
    }

    // Blocking scenario: Son blocks Brother
    [Test]
    public void Calculate_SonAndBrother_BrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new FullBrother());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs.Count, Is.EqualTo(1));
        Assert.That(result.Heirs[0].Relation, Is.EqualTo(RelationType.Son));
    }

    // Empty case
    [Test]
    public void Calculate_NoHeirs_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.False);
        Assert.That(result.ErrorMessage?.Contains("heirs"), Is.True);
    }

    // Invalid case: Too many wives
    [Test]
    public void Calculate_FiveWives_ReturnsFail()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        var wife = new Wife();
        wife.Count = 5;
        inheritanceCase.AddHeir(wife);

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.False);
    }

    // Complex scenario: Wife, 2 Daughters, Mother
    [Test]
    public void Calculate_ComplexScenario_CorrectDistribution()
    {
        var deceased = new DeceasedPerson(Gender.Male);
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

        // Wife: 1/8 (with daughters as descendants), Daughters: 2/3, Mother: 1/6
        Assert.That(wife.Result.Fraction, Is.EqualTo(new Fraction(1, 8)));
        Assert.That(daughters.Result.Fraction, Is.EqualTo(new Fraction(2, 3)));
        Assert.That(mother.Result.Fraction, Is.EqualTo(new Fraction(1, 6)));
    }

    // Uterine siblings scenario
    [Test]
    public void Calculate_UterineSiblings_CorrectShare()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother(2));

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);

        var utBrother = result.Heirs.First(h => h.Relation == RelationType.UterineBrother);

        // 2 uterine brothers share 1/3 equally
        Assert.That(utBrother.Result.Fraction.Numerator, Is.EqualTo(1));
        Assert.That(utBrother.Result.Fraction.Denominator, Is.EqualTo(3));
    }

    // Total fraction verification
    [Test]
    public void Calculate_TotalFractionEqualsOne()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.TotalFraction.Numerator, Is.EqualTo(1));
        Assert.That(result.TotalFraction.Denominator, Is.EqualTo(1));
    }

    // Calculation result verification
    [Test]
    public void Calculate_ResultHasExpectedProperties()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.Heirs, Is.Not.Null);
        Assert.That(result.Heirs.Count, Is.GreaterThan(0));
    }
}
*/