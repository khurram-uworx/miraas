using MiraasWeb.Abstractions;
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class CalculationAdvancedTests
{
    CalculationEngine engine;

    [SetUp]
    public void Setup()
    {
        engine = new CalculationEngine();
    }

    [Test]
    public void Calculate_SingleDaughter_GetsAllViaRadd()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        Assert.That(daughter.Result.Fraction, Is.EqualTo(Fraction.One), "Single daughter should get 100% via Radd");
    }

    [Test]
    public void Calculate_TwoDaughters_GetAllViaRadd()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter(2));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var daughters = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        Assert.That(daughters.Result.Fraction, Is.EqualTo(Fraction.One), "Two daughters should get 100% via Radd");
    }

    [Test]
    public void Calculate_MotherAlone_GetsAllViaRadd()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.One), "Mother alone should get 100% via Radd");
    }

    [Test]
    public void Calculate_WifeAndDaughter_DaughterGetsResidue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth));
        Assert.That(daughter.Result.Fraction, Is.EqualTo(new Fraction(7, 8)), "Daughter should get residue via Radd");
        Assert.That(daughter.Result.Fraction, Is.GreaterThan(wife.Result.Fraction), "Daughter should receive more than wife");
    }

    [Test]
    public void Calculate_HusbandAndMother_MotherGetsThirdOfResidue()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        Assert.That(husband.Result.Fraction, Is.EqualTo(Fraction.Half));
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.Half), "Mother should get 1/3 of residue after husband's share, then Radd");
        Assert.That(husband.Result.Fraction, Is.EqualTo(mother.Result.Fraction), "Husband and mother should receive equal shares");
    }

    [Test]
    public void Calculate_WifeAndMotherAndDaughter_NoRaddForSpouse()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth), "Wife should not receive Radd");

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        Assert.That(mother.Result.Fraction, Is.GreaterThan(wife.Result.Fraction), "Mother should receive more than wife via Radd");
        Assert.That(daughter.Result.Fraction, Is.GreaterThan(wife.Result.Fraction), "Daughter should receive more than wife via Radd");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should still equal 1");
    }

    [Test]
    public void Calculate_FatherMotherAndDaughterOfSon_CorrectShares()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var granddaughter = result.Heirs.First(h => h.Relation == RelationType.DaughterOfSon);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        var father = result.Heirs.First(h => h.Relation == RelationType.Father);

        Assert.That(granddaughter.Result.Fraction, Is.EqualTo(Fraction.Half), "Daughter of son should get 1/2");
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.Sixth), "Mother should get 1/6");
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.Third), "Father should get 1/3 (1/6 fixed + residue)");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void Calculate_WifeTwoDaughtersAndMother_RaddAllocatesExcludingSpouse()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Daughter(2));
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var daughters = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        // Expected: wife 1/8, daughters combined 7/10, mother 7/40 after Radd (spouse excluded)
        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth), "Wife should get fixed 1/8");
        Assert.That(daughters.Result.Fraction, Is.EqualTo(new Fraction(7, 10)), "Daughters combined should get 7/10 after Radd");
        Assert.That(mother.Result.Fraction, Is.EqualTo(new Fraction(7, 40)), "Mother should get 7/40 after Radd");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void Calculate_WifeFatherAndMother_OneThirdRaddForMother()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);

        var wifeExpected = Fraction.Quarter;
        var motherExpected = (Fraction.One - wifeExpected) / 3;
        var fatherExpected = Fraction.One - wifeExpected - motherExpected; // Technically Father 1/6 + Resiude after Mother

        Assert.That(wife.Result.Fraction, Is.EqualTo(wifeExpected), "Wife should get fixed 1/4");
        Assert.That(mother.Result.Fraction, Is.EqualTo(motherExpected), "Mother should get 1/4"); // 1/6 + 1/3 * 3/4 
        Assert.That(father.Result.Fraction, Is.EqualTo(fatherExpected), "Father should get 1/2");
        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total should equal 1");
    }

    [Test]
    public void Calculate_FatherAndDaughter_FatherGetsFixedPlusResidue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        Assert.That(daughter.Result.Fraction, Is.EqualTo(Fraction.Half));
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.Half), "Father gets 1/6 fixed + 1/3 residue");
        Assert.That(daughter.Result.Fraction, Is.EqualTo(father.Result.Fraction), "Daughter and father should receive equal shares");
    }

    [Test]
    public void Calculate_DaughterAndSonOfSon_SonOfSonGetsResidue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new SonOfSon());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        var sonOfSon = result.Heirs.First(h => h.Relation == RelationType.SonOfSon);

        Assert.That(daughter.Result.Fraction, Is.EqualTo(Fraction.Half));
        Assert.That(sonOfSon.Result.Fraction, Is.EqualTo(Fraction.Half));
        Assert.That(daughter.Result.Fraction, Is.EqualTo(sonOfSon.Result.Fraction), "Daughter and son of son should receive equal shares");
    }

    [Test]
    public void Calculate_DaughterAndDaughterOfSon_DaughterOfSonGetsComplement()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        var daughterOfSon = result.Heirs.First(h => h.Relation == RelationType.DaughterOfSon);

        Assert.That(daughter.Result.Fraction, Is.GreaterThanOrEqualTo(Fraction.Half));
        Assert.That(daughterOfSon.Result.Fraction, Is.GreaterThanOrEqualTo(Fraction.Sixth), "Daughter of son gets 1/6 complement + Radd share");
        Assert.That(daughter.Result.Fraction, Is.GreaterThan(daughterOfSon.Result.Fraction), "Daughter should receive more than daughter of son");
    }

    [Test]
    public void Calculate_DaughterAndFullSister_FullSisterGetsResidue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new FullSister());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        var fullSister = result.Heirs.First(h => h.Relation == RelationType.FullSister);

        Assert.That(daughter.Result.Fraction, Is.EqualTo(Fraction.Half));
        Assert.That(fullSister.Result.Fraction, Is.EqualTo(Fraction.Half), "Full sister as residuary when daughter exists");
        Assert.That(daughter.Result.Fraction, Is.EqualTo(fullSister.Result.Fraction), "Daughter and full sister should receive equal shares");
    }

    [Test]
    public void Calculate_MultipleWives_ShareEqually()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife(3));
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var wives = result.Heirs.First(h => h.Relation == RelationType.Wife);
        Assert.That(wives.Result.Fraction, Is.EqualTo(Fraction.Eighth), "Total wife share should be 1/8");
        Assert.That(wives.Count, Is.EqualTo(3));
    }

    [Test]
    public void Calculate_MultipleGrandmothers_ShareSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new GrandmotherMaternal());
        inheritanceCase.AddHeir(new GrandmotherPaternal());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var grandmotherM = result.Heirs.First(h => h.Relation == RelationType.GrandmotherMaternal);
        var grandmotherP = result.Heirs.First(h => h.Relation == RelationType.GrandmotherPaternal);

        // Total grandmother share is 1/6, each gets 1/12 (1/6 ÷ 2)
        var expectedShare = new Fraction(1, 12);
        Assert.That(grandmotherM.Result.Fraction, Is.EqualTo(expectedShare));
        Assert.That(grandmotherP.Result.Fraction, Is.EqualTo(expectedShare));
        Assert.That(grandmotherM.Result.Fraction, Is.EqualTo(grandmotherP.Result.Fraction), "Both grandmothers should receive equal shares");
        Assert.That(grandmotherM.Result.Fraction, Is.LessThan(Fraction.Sixth), "Each grandmother should receive less than 1/6");
    }

    [Test]
    public void Calculate_GrandfatherWithSiblings_GrandfatherCompetes()
    {
        // Grandfather competing with siblings - complex Islamic rule
        // For now, test that grandfather gets at least 1/6
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new FullBrother());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var grandfather = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.Grandfather);
        Assert.That(grandfather, Is.Not.Null, "Grandfather should not be blocked by siblings");
        // Grandfather should get at least 1/6 in competition with siblings
        Assert.That(grandfather.Result.Fraction, Is.GreaterThanOrEqualTo(Fraction.Sixth));
        Assert.That(grandfather.Result.Fraction, Is.LessThanOrEqualTo(Fraction.One), "Grandfather should not receive more than 100%");
    }

    [Test]
    public void Calculate_UterineSiblingsWithDescendants_ShouldBeBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.Heirs.Count, Is.EqualTo(1), "Uterine brother should be blocked by son");
        Assert.That(result.Heirs[0].Relation, Is.EqualTo(RelationType.Son));
    }

    [Test]
    public void Calculate_GrandfatherAndDaughter_GrandfatherGetsFixedPlusResidue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var grandfather = result.Heirs.First(h => h.Relation == RelationType.Grandfather);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        Assert.That(daughter.Result.Fraction, Is.EqualTo(Fraction.Half));
        Assert.That(grandfather.Result.Fraction, Is.EqualTo(Fraction.Half), "Grandfather gets 1/6 fixed + 1/3 residue with female descendants");
        Assert.That(daughter.Result.Fraction, Is.EqualTo(grandfather.Result.Fraction), "Daughter and grandfather should receive equal shares");
    }

    [Test]
    public void Calculate_SonOfSonAndDaughterOfSon_TwoToOneRatio()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var sonOfSon = result.Heirs.First(h => h.Relation == RelationType.SonOfSon);
        var daughterOfSon = result.Heirs.First(h => h.Relation == RelationType.DaughterOfSon);

        var expectedSonShare = daughterOfSon.Result.Fraction * 2;
        Assert.That(sonOfSon.Result.Fraction, Is.EqualTo(expectedSonShare), "Son of son should get twice daughter of son's share");
        Assert.That(sonOfSon.Result.Fraction, Is.GreaterThan(daughterOfSon.Result.Fraction), "Son of son should receive more than daughter of son");
    }

    [Test]
    public void Calculate_HusbandTwoDaughters_RaddScenario()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Daughter(2));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total shares should equal 100%");

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        var daughters = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        Assert.That(husband.Result.Fraction, Is.EqualTo(Fraction.Quarter), "Husband should keep fixed 1/4 (excluded from Radd)");
        Assert.That(daughters.Result.Fraction, Is.EqualTo(new Fraction(3, 4)), "Daughters should get 2/3 + 1/12 residue via Radd");
    }

    [Test]
    public void Calculate_AwlScenario_HusbandMotherTwoDaughters_ProportionalReduction()
    {
        // Husband (1/4) + Mother (1/6) + 2 Daughters (2/3)
        // Total = 6/24 + 4/24 + 16/24 = 26/24 = 13/12 (Awl!)
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Daughter(2));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total shares should equal 100% after Awl");

        var husband = result.Heirs.First(h => h.Relation == RelationType.Husband);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        var daughters = result.Heirs.First(h => h.Relation == RelationType.Daughter);

        // Original: Husband 1/4 (3/12), Mother 1/6 (2/12), Daughters 2/3 (8/12) = 13/12
        // After Awl: Husband 3/13, Mother 2/13, Daughters 8/13
        var expectedHusbandShare = new Fraction(3, 13);
        var expectedMotherShare = new Fraction(2, 13);
        var expectedDaughtersShare = new Fraction(8, 13);

        Assert.That(husband.Result.Fraction, Is.EqualTo(expectedHusbandShare), "Husband share should be proportionally reduced in Awl");
        Assert.That(mother.Result.Fraction, Is.EqualTo(expectedMotherShare), "Mother share should be proportionally reduced in Awl");
        Assert.That(daughters.Result.Fraction, Is.EqualTo(expectedDaughtersShare), "Daughters share should be proportionally reduced in Awl");
    }

    [Test]
    public void Calculate_TwoDaughtersAndTwoDaughtersOfSon_DaughtersOfSonBlocked()
    {
        // RULES.md Section 7: When 2+ daughters exist (already have 2/3), daughters of son are blocked
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter(2));
        inheritanceCase.AddHeir(new DaughterOfSon(2));

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        // Daughters of son should be blocked
        var daughtersOfSon = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.DaughterOfSon);
        Assert.That(daughtersOfSon, Is.Null, "Daughters of son should be blocked when 2+ daughters exist");

        var daughters = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        Assert.That(daughters.Result.Fraction, Is.EqualTo(Fraction.One), "Daughters should get all via Radd");
    }

    [Test]
    public void Calculate_GrandmotherWithMother_GrandmotherBlocked()
    {
        // RULES.md Section 10: Grandmother only inherits if mother is not alive
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new GrandmotherMaternal());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        // Grandmother should be blocked by mother
        var grandmother = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.GrandmotherMaternal);
        Assert.That(grandmother, Is.Null, "Grandmother should be blocked when mother exists");

        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.Sixth), "Mother should get 1/6");
    }

    [Test]
    public void Calculate_GrandfatherWithFather_GrandfatherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new Daughter());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        var grandfather = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.Grandfather);

        Assert.That(grandfather, Is.Null, "Grandfather should be blocked when father exists");
        Assert.That(father.Result.Fraction, Is.GreaterThan(Fraction.Zero), "Father should inherit");
    }

    [Test]
    public void Calculate_ConsanguineSisterWithFullSister_ConsanguineBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var fullSister = result.Heirs.First(h => h.Relation == RelationType.FullSister);
        var consanguineSister = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.ConsanguineSister);

        Assert.That(fullSister.Result.Fraction, Is.EqualTo(Fraction.One), "Full sister should get all via Radd");
        Assert.That(consanguineSister, Is.Null, "Consanguine sister should be blocked when full sister exists");
    }

    [Test]
    public void Calculate_FullSisterWithSon_FullSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new Son());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var fullSister = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.FullSister);
        var son = result.Heirs.First(h => h.Relation == RelationType.Son);

        Assert.That(fullSister, Is.Null, "Full sister should be blocked when son exists");
        Assert.That(son.Result.Fraction, Is.EqualTo(Fraction.One), "Son should get all as residuary");
    }

    [Test]
    public void Calculate_FullSisterWithFather_FullSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new Father());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var fullSister = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.FullSister);
        var father = result.Heirs.First(h => h.Relation == RelationType.Father);

        Assert.That(fullSister, Is.Null, "Full sister should be blocked when father exists");
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.One), "Father should get all as residuary");
    }

    [Test]
    public void Calculate_UterineSiblingWithFather_UterineBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother());
        inheritanceCase.AddHeir(new Father());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        var uterineBrother = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.UterineBrother);

        Assert.That(uterineBrother, Is.Null, "Uterine brother should be blocked when father exists");
        Assert.That(father.Result.Fraction, Is.EqualTo(Fraction.One), "Father should get all");
    }

    [Test]
    public void Calculate_ComplexScenario_WifeFatherMotherDaughterFullSister()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new FullSister());

        var result = engine.Calculate(inheritanceCase);
        Assert.That(result.IsSuccessful, Is.True);

        Assert.That(result.TotalFraction, Is.EqualTo(Fraction.One), "Total shares should equal 100%");

        var wife = result.Heirs.First(h => h.Relation == RelationType.Wife);
        var father = result.Heirs.First(h => h.Relation == RelationType.Father);
        var mother = result.Heirs.First(h => h.Relation == RelationType.Mother);
        var daughter = result.Heirs.First(h => h.Relation == RelationType.Daughter);
        var fullSister = result.Heirs.FirstOrDefault(h => h.Relation == RelationType.FullSister);

        Assert.That(wife.Result.Fraction, Is.EqualTo(Fraction.Eighth), "Wife gets 1/8");
        Assert.That(mother.Result.Fraction, Is.EqualTo(Fraction.Sixth), "Mother gets 1/6");
        Assert.That(daughter.Result.Fraction, Is.EqualTo(Fraction.Half), "Daughter gets 1/2");
        
        // Father and Full Sister compete for residue (if full sister not blocked)
        // OR Full sister might be blocked by father's presence
        // This tests the interaction between multiple rules
        Assert.That(father.Result.Fraction, Is.GreaterThan(Fraction.Zero), "Father should inherit");
    }
}
