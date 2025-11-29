using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class FractionTests
{
    [Test]
    public void Constructor_ValidValues_CreatesNormalizedFraction()
    {
        var fraction = new Fraction(2, 4);
        Assert.That(fraction.Numerator, Is.EqualTo(1));
        Assert.That(fraction.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_AlreadyNormalized_StaysNormalized()
    {
        var fraction = new Fraction(1, 2);
        Assert.That(fraction.Numerator, Is.EqualTo(1));
        Assert.That(fraction.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_ZeroNumerator_NormalizesToZeroOverOne()
    {
        var fraction = new Fraction(0, 5);
        Assert.That(fraction.Numerator, Is.EqualTo(0));
        Assert.That(fraction.Denominator, Is.EqualTo(1));
    }

    [Test]
    public void Constructor_NegativeDenominator_MovesSignToNumerator()
    {
        var fraction = new Fraction(1, -2);
        Assert.That(fraction.Numerator, Is.EqualTo(-1));
        Assert.That(fraction.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_ZeroDenominator_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Fraction(1, 0));
    }

    [Test]
    public void Addition_SimpleCase_CorrectSum()
    {
        var fraction1 = new Fraction(1, 4);
        var fraction2 = new Fraction(1, 4);
        var result = fraction1 + fraction2;
        
        Assert.That(result.Numerator, Is.EqualTo(1));
        Assert.That(result.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void Addition_DifferentDenominators_CorrectSum()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(1, 3);
        var result = fraction1 + fraction2;
        
        Assert.That(result.Numerator, Is.EqualTo(5));
        Assert.That(result.Denominator, Is.EqualTo(6));
    }

    [Test]
    public void Subtraction_SimpleCase_CorrectDifference()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(1, 4);
        var result = fraction1 - fraction2;
        
        Assert.That(result.Numerator, Is.EqualTo(1));
        Assert.That(result.Denominator, Is.EqualTo(4));
    }

    [Test]
    public void Multiplication_SimpleCase_CorrectProduct()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(2, 3);
        var result = fraction1 * fraction2;
        
        Assert.That(result.Numerator, Is.EqualTo(1));
        Assert.That(result.Denominator, Is.EqualTo(3));
    }

    [Test]
    public void Division_SimpleCase_CorrectQuotient()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(1, 4);
        var result = fraction1 / fraction2;
        
        Assert.That(result.Numerator, Is.EqualTo(2));
        Assert.That(result.Denominator, Is.EqualTo(1));
    }

    [Test]
    public void Division_ByZeroFraction_ThrowsException()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(0, 1);
        
        Assert.Throws<ArgumentException>(() => _ = fraction1 / fraction2);
    }

    [Test]
    public void ScalarMultiplication_CorrectProduct()
    {
        var fraction = new Fraction(1, 4);
        var result = fraction * 3;
        
        Assert.That(result.Numerator, Is.EqualTo(3));
        Assert.That(result.Denominator, Is.EqualTo(4));
    }

    [Test]
    public void ScalarDivision_CorrectQuotient()
    {
        var fraction = new Fraction(1, 2);
        var result = fraction / 2;
        
        Assert.That(result.Numerator, Is.EqualTo(1));
        Assert.That(result.Denominator, Is.EqualTo(4));
    }

    [Test]
    public void ToDecimal_CorrectConversion()
    {
        var fraction = new Fraction(1, 4);
        Assert.That(fraction.ToDecimal(), Is.EqualTo(0.25m));
    }

    [Test]
    public void ToPercentage_CorrectConversion()
    {
        var fraction = new Fraction(1, 4);
        Assert.That(fraction.ToPercentage(), Is.EqualTo(25m));
    }

    [Test]
    public void Equality_SameFraction_IsEqual()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(2, 4);
        
        Assert.That(fraction1 == fraction2, Is.True);
        Assert.That(fraction1, Is.EqualTo(fraction2));
    }

    [Test]
    public void Equality_DifferentFractions_IsNotEqual()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(1, 3);
        
        Assert.That(fraction1 == fraction2, Is.False);
        Assert.That(fraction1 != fraction2, Is.True);
    }

    [Test]
    public void Comparison_LessThan_CorrectResult()
    {
        var fraction1 = new Fraction(1, 4);
        var fraction2 = new Fraction(1, 2);
        
        Assert.That(fraction1 < fraction2, Is.True);
        Assert.That(fraction2 < fraction1, Is.False);
    }

    [Test]
    public void Comparison_GreaterThan_CorrectResult()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(1, 4);
        
        Assert.That(fraction1 > fraction2, Is.True);
        Assert.That(fraction2 > fraction1, Is.False);
    }

    [Test]
    public void ToString_SimpleFraction_CorrectFormat()
    {
        var fraction = new Fraction(1, 4);
        Assert.That(fraction.ToString(), Is.EqualTo("1/4"));
    }

    [Test]
    public void ToString_IntegerFraction_ReturnsInteger()
    {
        var fraction = new Fraction(4, 1);
        Assert.That(fraction.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void ToMixedNumber_ProperFraction_ReturnsFraction()
    {
        var fraction = new Fraction(1, 4);
        Assert.That(fraction.ToMixedNumber(), Is.EqualTo("1/4"));
    }

    [Test]
    public void ToMixedNumber_ImproperFraction_ReturnsProperFormat()
    {
        var fraction = new Fraction(5, 2);
        Assert.That(fraction.ToMixedNumber(), Is.EqualTo("2 1/2"));
    }

    [Test]
    public void ToMixedNumber_WholeNumber_ReturnsInteger()
    {
        var fraction = new Fraction(8, 2);
        Assert.That(fraction.ToMixedNumber(), Is.EqualTo("4"));
    }

    [Test]
    public void IslamicFraction_OneFourth_CorrectValue()
    {
        var fraction = new Fraction(1, 4);
        Assert.That(fraction.ToDecimal(), Is.EqualTo(0.25m));
    }

    [Test]
    public void IslamicFraction_OneEighth_CorrectValue()
    {
        var fraction = new Fraction(1, 8);
        Assert.That(fraction.ToDecimal(), Is.EqualTo(0.125m));
    }

    [Test]
    public void IslamicFraction_TwoThirds_CorrectValue()
    {
        var fraction = new Fraction(2, 3);
        Assert.That(fraction.ToDecimal(), Is.EqualTo((decimal)2 / 3));
    }

    [Test]
    public void ComplexCalculation_InheritanceScenario()
    {
        // Wife: 1/8, Father: 1/6
        var wife = new Fraction(1, 8);
        var father = new Fraction(1, 6);
        var total = wife + father;

        Assert.That(total.Numerator, Is.EqualTo(7));
        Assert.That(total.Denominator, Is.EqualTo(24));
    }
}
