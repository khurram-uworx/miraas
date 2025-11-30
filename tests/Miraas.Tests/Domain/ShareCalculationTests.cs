/*
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class ShareCalculationTests
{
    ShareRuleEngine engine;

    [SetUp]
    public void Setup()
    {
        engine = new ShareRuleEngine();
    }

    // Wife tests
    [Test]
    public void GetShare_WifeWithChildren_ReturnsOneEighth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());

        var share = engine.GetShare(RelationType.Wife, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(8));
    }

    [Test]
    public void GetShare_WifeWithoutChildren_ReturnsOneFourth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());

        var share = engine.GetShare(RelationType.Wife, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(4));
    }

    // Husband tests
    [Test]
    public void GetShare_HusbandWithChildren_ReturnsOneFourth()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Son());

        var share = engine.GetShare(RelationType.Husband, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(4));
    }

    [Test]
    public void GetShare_HusbandWithoutChildren_ReturnsOneHalf()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());

        var share = engine.GetShare(RelationType.Husband, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(2));
    }

    // Daughter tests
    [Test]
    public void GetShare_SingleDaughterNoSon_ReturnsOneHalf()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());

        var share = engine.GetShare(RelationType.Daughter, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void GetShare_MultipleDaughtersNoSon_ReturnsTwoThirds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter(2));

        var share = engine.GetShare(RelationType.Daughter, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(2));
        Assert.That(share.Denominator, Is.EqualTo(3));
    }

    [Test]
    public void GetShare_DaughterWithSon_ReturnsZero()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new Son());

        var share = engine.GetShare(RelationType.Daughter, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(0));
    }

    // Father tests
    [Test]
    public void GetShare_FatherWithChildren_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Son());

        var share = engine.GetShare(RelationType.Father, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void GetShare_FatherWithoutChildren_ReturnsZero()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());

        var share = engine.GetShare(RelationType.Father, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(0));
    }

    // Mother tests
    [Test]
    public void GetShare_MotherWithChildren_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Son());

        var share = engine.GetShare(RelationType.Mother, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void GetShare_MotherWithSiblings_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new FullBrother());

        var share = engine.GetShare(RelationType.Mother, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void GetShare_MotherNoChildrenNoSiblingsNoHusband_ReturnsOneThird()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());

        var share = engine.GetShare(RelationType.Mother, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(3));
    }

    // Grandmother tests
    [Test]
    public void GetShare_GrandmotherMaternal_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new GrandmotherMaternal());

        var share = engine.GetShare(RelationType.GrandmotherMaternal, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void GetShare_GrandmotherPaternal_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new GrandmotherPaternal());

        var share = engine.GetShare(RelationType.GrandmotherPaternal, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    // Grandfather tests
    [Test]
    public void GetShare_GrandfatherWithChildren_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new Son());

        var share = engine.GetShare(RelationType.Grandfather, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void GetShare_GrandfatherWithoutChildren_ReturnsZero()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());

        var share = engine.GetShare(RelationType.Grandfather, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(0));
    }

    // Uterine sibling tests
    [Test]
    public void GetShare_SingleUterineBrother_ReturnsOneSixth()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother());

        var share = engine.GetShare(RelationType.UterineBrother, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void GetShare_MultipleUterineSiblings_ReturnsOneThird()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother(2));

        var share = engine.GetShare(RelationType.UterineBrother, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(3));
    }

    [Test]
    public void GetShare_UterineBrotherAndSister_ShareOneThirdEqually()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother());
        inheritanceCase.AddHeir(new UterineSister());

        var brotherShare = engine.GetShare(RelationType.UterineBrother, inheritanceCase);
        var sisterShare = engine.GetShare(RelationType.UterineSister, inheritanceCase);

        Assert.That(brotherShare.Numerator, Is.EqualTo(1));
        Assert.That(brotherShare.Denominator, Is.EqualTo(3));
        Assert.That(sisterShare.Numerator, Is.EqualTo(1));
        Assert.That(sisterShare.Denominator, Is.EqualTo(3));
    }

    // Full sibling tests
    [Test]
    public void GetShare_SingleFullSisterNoDescendants_ReturnsOneHalf()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());

        var share = engine.GetShare(RelationType.FullSister, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void GetShare_MultipleFullSisters_ReturnsTwoThirds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister(2));

        var share = engine.GetShare(RelationType.FullSister, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(2));
        Assert.That(share.Denominator, Is.EqualTo(3));
    }

    // Daughter of Son tests
    [Test]
    public void GetShare_SingleDaughterOfSon_ReturnsOneHalf()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new DaughterOfSon());

        var share = engine.GetShare(RelationType.DaughterOfSon, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(1));
        Assert.That(share.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void GetShare_MultipleDaughtersOfSon_ReturnsTwoThirds()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new DaughterOfSon(2));

        var share = engine.GetShare(RelationType.DaughterOfSon, inheritanceCase);

        Assert.That(share.Numerator, Is.EqualTo(2));
        Assert.That(share.Denominator, Is.EqualTo(3));
    }
}
*/