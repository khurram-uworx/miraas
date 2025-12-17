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
}
