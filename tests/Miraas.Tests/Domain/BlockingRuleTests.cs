/*
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class BlockingRuleTests
{
    BlockingRuleEngine engine;

    [SetUp]
    public void Setup()
    {
        engine = new BlockingRuleEngine();
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_GrandfatherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Grandfather());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.Grandfather));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_GrandfatherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Grandfather());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.Grandfather));
    }

    [Test]
    public void GetBlockedHeirs_NoFatherNoSon_GrandfatherNotBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Grandfather));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_BrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_SisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new FullSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullSister));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_BrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_NoSonNoFather_BrotherNotBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_SonOfSonBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new SonOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.SonOfSon));
    }

    [Test]
    public void GetBlockedHeirs_DaughterOnlyNoDaughterOfSon_DaughterOfSonBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.DaughterOfSon));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_MaternalGrandmotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new GrandmotherMaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.GrandmotherMaternal));
    }

    [Test]
    public void GetBlockedHeirs_MotherPresent_MaternalGrandmotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new GrandmotherMaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.GrandmotherMaternal));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_PaternalGrandmotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new GrandmotherPaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.GrandmotherPaternal));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_PaternalGrandmotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new GrandmotherPaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.GrandmotherPaternal));
    }

    [Test]
    public void IsBlocked_FatherBlocksGrandfather_ReturnsTrue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Grandfather());

        bool isBlocked = engine.IsBlocked(RelationType.Grandfather, inheritanceCase);

        Assert.That(isBlocked, Is.True);
    }

    [Test]
    public void IsBlocked_NoBlocker_ReturnsFalse()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());

        bool isBlocked = engine.IsBlocked(RelationType.Grandfather, inheritanceCase);

        Assert.That(isBlocked, Is.False);
    }

    [Test]
    public void GetBlockedHeirs_ConsanguineSiblings_BlockedByFather()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new ConsanguineBrother());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    [Test]
    public void GetBlockedHeirs_UterineSiblings_BlockedBySonOfSon()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineBrother));
    }

    [Test]
    public void GetBlockedHeirs_MultipleBlockers_AllApplied()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new Grandfather());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
        Assert.That(blocked, Does.Contain(RelationType.FullSister));
        Assert.That(blocked, Does.Contain(RelationType.SonOfSon));
        Assert.That(blocked, Does.Contain(RelationType.Grandfather));
    }
}
*/