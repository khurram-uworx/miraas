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

    #region Grandfather Blocking Tests

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

    #endregion

    #region Son Blocks Siblings - All Types

    [Test]
    public void GetBlockedHeirs_SonPresent_FullBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_FullSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new FullSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullSister));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_ConsanguineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_ConsanguineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_UterineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonPresent_UterineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new UterineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineSister));
    }

    #endregion

    #region Father Blocks Siblings - All Types

    [Test]
    public void GetBlockedHeirs_FatherPresent_FullBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_FullSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new FullSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullSister));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_ConsanguineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_ConsanguineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_UterineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineBrother));
    }

    [Test]
    public void GetBlockedHeirs_FatherPresent_UterineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new UterineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineSister));
    }

    #endregion

    #region SonOfSon Blocks Siblings - All Types

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_FullBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_FullSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new FullSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullSister));
    }

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_ConsanguineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_ConsanguineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_UterineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineBrother));
    }

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_UterineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new UterineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineSister));
    }

    #endregion

    #region Grandfather Blocks Siblings (When Father Absent)

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_FullBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_FullSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new FullSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.FullSister));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_ConsanguineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_ConsanguineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_UterineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineBrother));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherPresent_UterineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new UterineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.UterineSister));
    }

    #endregion

    #region Full Siblings Block Consanguine Siblings

    [Test]
    public void GetBlockedHeirs_FullBrotherPresent_ConsanguineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
    }

    [Test]
    public void GetBlockedHeirs_FullBrotherPresent_ConsanguineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    [Test]
    public void GetBlockedHeirs_FullSisterPresent_ConsanguineBrotherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
    }

    [Test]
    public void GetBlockedHeirs_FullSisterPresent_ConsanguineSisterBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new ConsanguineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
    }

    #endregion

    #region Son Blocks Grandchildren

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
    public void GetBlockedHeirs_SonPresent_DaughterOfSonBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.DaughterOfSon));
    }

    #endregion

    #region Daughter Blocks DaughterOfSon

    [Test]
    public void GetBlockedHeirs_OneDaughterOnly_DaughterOfSonBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter { Count = 1 });
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.DaughterOfSon));
    }

    [Test]
    public void GetBlockedHeirs_TwoDaughters_DaughterOfSonBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter { Count = 2 });
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.DaughterOfSon));
    }

    #endregion

    #region SonOfSon Blocks DaughterOfSon

    [Test]
    public void GetBlockedHeirs_SonOfSonPresent_DaughterOfSonBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.DaughterOfSon));
    }

    #endregion

    #region Grandmother Blocking Rules

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
    public void GetBlockedHeirs_MotherOnly_PaternalGrandmotherNotBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new GrandmotherPaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.GrandmotherPaternal));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherOnly_MaternalGrandmotherNotBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new GrandmotherMaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.GrandmotherMaternal));
    }

    #endregion

    #region Negative Tests - No Blocking Scenarios

    [Test]
    public void GetBlockedHeirs_NoSonNoFather_FullBrotherNotBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.FullBrother));
    }

    [Test]
    public void GetBlockedHeirs_OnlyWife_NothingBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Is.Empty);
    }

    [Test]
    public void GetBlockedHeirs_OnlyDaughter_NothingBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Is.Empty);
    }

    [Test]
    public void GetBlockedHeirs_MotherAndHusband_NothingBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Husband());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Is.Empty);
    }

    [Test]
    public void GetBlockedHeirs_SonAndDaughter_NeitherBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Son));
        Assert.That(blocked, Does.Not.Contain(RelationType.Daughter));
    }

    #endregion

    #region IsBlocked Method Tests

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
    public void IsBlocked_FullBrotherBlocksConsanguineBrother_ReturnsTrue()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new ConsanguineBrother());

        bool isBlocked = engine.IsBlocked(RelationType.ConsanguineBrother, inheritanceCase);

        Assert.That(isBlocked, Is.True);
    }

    [Test]
    public void IsBlocked_WifeNeverBlocked_ReturnsFalse()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Wife());

        bool isBlocked = engine.IsBlocked(RelationType.Wife, inheritanceCase);

        Assert.That(isBlocked, Is.False);
    }

    #endregion

    #region Complex Multi-Blocker Scenarios

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

    [Test]
    public void GetBlockedHeirs_ComplexSiblingHierarchy_ProperBlocking()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new FullSister());
        inheritanceCase.AddHeir(new ConsanguineBrother());
        inheritanceCase.AddHeir(new ConsanguineSister());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Full siblings block consanguine siblings
        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));
        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));

        // Uterine siblings are not blocked by full/consanguine (no descendants/ascendants)
        Assert.That(blocked, Does.Not.Contain(RelationType.UterineBrother));
    }

    [Test]
    public void GetBlockedHeirs_AllGrandparentsAndDescendants_CorrectBlocking()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new GrandmotherMaternal());
        inheritanceCase.AddHeir(new GrandmotherPaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Contain(RelationType.Grandfather));
        Assert.That(blocked, Does.Contain(RelationType.GrandmotherMaternal));
        Assert.That(blocked, Does.Contain(RelationType.GrandmotherPaternal));
        Assert.That(blocked, Does.Not.Contain(RelationType.Father));
        Assert.That(blocked, Does.Not.Contain(RelationType.Mother));
    }

    [Test]
    public void GetBlockedHeirs_SonAndFatherPresent_AllSiblingsAndGrandparentsBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new ConsanguineSister());
        inheritanceCase.AddHeir(new UterineBrother());
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new GrandmotherPaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // All siblings blocked by Son OR Father
        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));
        Assert.That(blocked, Does.Contain(RelationType.UterineBrother));

        // Grandparents blocked by Father
        Assert.That(blocked, Does.Contain(RelationType.Grandfather));
        Assert.That(blocked, Does.Contain(RelationType.GrandmotherPaternal));
    }

    [Test]
    public void GetBlockedHeirs_OnlyGrandchildren_NoneBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new SonOfSon());
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // When only grandchildren exist with no direct children, neither is blocked
        Assert.That(blocked, Does.Not.Contain(RelationType.SonOfSon));
        Assert.That(blocked, Does.Not.Contain(RelationType.DaughterOfSon));
    }

    #endregion

    #region Edge Cases

    [Test]
    public void GetBlockedHeirs_DaughterWithoutSon_SiblingsNotBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new FullSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Daughter alone does NOT block siblings (only Son blocks them)
        Assert.That(blocked, Does.Not.Contain(RelationType.FullBrother));
        Assert.That(blocked, Does.Not.Contain(RelationType.FullSister));
    }

    [Test]
    public void GetBlockedHeirs_OnlyUterineSiblings_NoneBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new UterineBrother());
        inheritanceCase.AddHeir(new UterineSister());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Uterine siblings don't block each other
        Assert.That(blocked, Is.Empty);
    }

    [Test]
    public void GetBlockedHeirs_MotherAndDaughter_NoGrandmotherBlocking()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new GrandmotherMaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Mother blocks maternal grandmother
        Assert.That(blocked, Does.Contain(RelationType.GrandmotherMaternal));

        // Daughter doesn't block anything here
        Assert.That(blocked, Does.Not.Contain(RelationType.Daughter));
        Assert.That(blocked, Does.Not.Contain(RelationType.Mother));
    }

    [Test]
    public void GetBlockedHeirs_GrandfatherWithoutFather_OnlySiblingsBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Grandfather());
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new ConsanguineSister());
        inheritanceCase.AddHeir(new GrandmotherMaternal());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Grandfather blocks siblings
        Assert.That(blocked, Does.Contain(RelationType.FullBrother));
        Assert.That(blocked, Does.Contain(RelationType.ConsanguineSister));

        // Grandfather does NOT block maternal grandmother
        Assert.That(blocked, Does.Not.Contain(RelationType.GrandmotherMaternal));
    }

    [Test]
    public void GetBlockedHeirs_MultipleDaughters_DaughterOfSonStillBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter { Count = 3 });
        inheritanceCase.AddHeir(new DaughterOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Multiple daughters (taking 2/3) block daughter of son
        Assert.That(blocked, Does.Contain(RelationType.DaughterOfSon));
    }

    [Test]
    public void GetBlockedHeirs_AllSiblingTypes_FullBlocksConsanguineOnly()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new FullBrother());
        inheritanceCase.AddHeir(new ConsanguineBrother());
        inheritanceCase.AddHeir(new UterineBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        // Full blocks consanguine
        Assert.That(blocked, Does.Contain(RelationType.ConsanguineBrother));

        // Full does NOT block uterine (different rules)
        Assert.That(blocked, Does.Not.Contain(RelationType.UterineBrother));

        // Full brother is not blocked
        Assert.That(blocked, Does.Not.Contain(RelationType.FullBrother));
    }

    #endregion

    #region Spouse Never Blocked Tests

    [Test]
    public void GetBlockedHeirs_HusbandWithAllHeirs_NeverBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Female);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Husband());
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Husband));
    }

    [Test]
    public void GetBlockedHeirs_WifeWithAllHeirs_NeverBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Wife));
    }

    #endregion

    #region Parents Never Blocked Tests

    [Test]
    public void GetBlockedHeirs_FatherWithAllDescendants_NeverBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new SonOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Father));
    }

    [Test]
    public void GetBlockedHeirs_MotherWithAllDescendants_NeverBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new SonOfSon());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Mother));
    }

    #endregion

    #region Direct Children Never Blocked Tests

    [Test]
    public void GetBlockedHeirs_SonWithAllHeirs_NeverBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Son());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Son));
    }

    [Test]
    public void GetBlockedHeirs_DaughterWithAllHeirs_NeverBlocked()
    {
        var deceased = new DeceasedPerson(Gender.Male);
        var inheritanceCase = new InheritanceCase(deceased);
        inheritanceCase.AddHeir(new Daughter());
        inheritanceCase.AddHeir(new Father());
        inheritanceCase.AddHeir(new Mother());
        inheritanceCase.AddHeir(new Wife());
        inheritanceCase.AddHeir(new FullBrother());

        var blocked = engine.GetBlockedHeirs(inheritanceCase);

        Assert.That(blocked, Does.Not.Contain(RelationType.Daughter));
    }

    #endregion
}