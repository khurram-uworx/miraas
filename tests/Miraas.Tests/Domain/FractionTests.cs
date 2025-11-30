using MiraasWeb.Abstractions;
using MiraasWeb.Domain;

namespace Miraas.Tests.Domain;

[TestFixture]
public class FractionTests
{
    #region Constructor Tests

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
    public void Constructor_BothNegative_NormalizesToPositive()
    {
        var fraction = new Fraction(-1, -2);
        Assert.That(fraction.Numerator, Is.EqualTo(1));
        Assert.That(fraction.Denominator, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_ZeroDenominator_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Fraction(1, 0));
    }

    #endregion

    #region Arithmetic Operations - Basic

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
    public void Addition_WithZero_ReturnsOriginal()
    {
        var fraction1 = new Fraction(1, 4);
        var zero = new Fraction(0, 1);
        var result = fraction1 + zero;

        Assert.That(result, Is.EqualTo(fraction1));
    }

    [Test]
    public void Addition_NegativeFractions_CorrectSum()
    {
        var fraction1 = new Fraction(-1, 4);
        var fraction2 = new Fraction(-1, 4);
        var result = fraction1 + fraction2;

        Assert.That(result.Numerator, Is.EqualTo(-1));
        Assert.That(result.Denominator, Is.EqualTo(2));
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
    public void Subtraction_ResultingInZero_CorrectResult()
    {
        var fraction1 = new Fraction(1, 4);
        var fraction2 = new Fraction(1, 4);
        var result = fraction1 - fraction2;

        Assert.That(result.Numerator, Is.EqualTo(0));
        Assert.That(result.Denominator, Is.EqualTo(1));
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
    public void Multiplication_ByZero_ReturnsZero()
    {
        var fraction = new Fraction(1, 4);
        var zero = new Fraction(0, 1);
        var result = fraction * zero;

        Assert.That(result.Numerator, Is.EqualTo(0));
        Assert.That(result.Denominator, Is.EqualTo(1));
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

    #endregion

    #region Islamic Fractions - All Six Quranic Shares

    [Test]
    public void IslamicFraction_OneHalf_CorrectValue()
    {
        var fraction = new Fraction(1, 2);
        Assert.That(fraction.ToDecimal(), Is.EqualTo(0.5m));
        Assert.That(fraction.ToPercentage(), Is.EqualTo(50m));
    }

    [Test]
    public void IslamicFraction_OneThird_CorrectValue()
    {
        var fraction = new Fraction(1, 3);
        var decimalValue = fraction.ToDecimal();

        // 1/3 = 0.333... (repeating)
        Assert.That(decimalValue, Is.EqualTo(1m / 3m).Within(0.0000001m));
        Assert.That(fraction.ToPercentage(), Is.EqualTo(100m / 3m).Within(0.0001m));
    }

    [Test]
    public void IslamicFraction_OneFourth_CorrectValue()
    {
        var fraction = new Fraction(1, 4);
        Assert.That(fraction.ToDecimal(), Is.EqualTo(0.25m));
        Assert.That(fraction.ToPercentage(), Is.EqualTo(25m));
    }

    [Test]
    public void IslamicFraction_OneSixth_CorrectValue()
    {
        var fraction = new Fraction(1, 6);
        var decimalValue = fraction.ToDecimal();

        // 1/6 = 0.1666... (repeating)
        Assert.That(decimalValue, Is.EqualTo(1m / 6m).Within(0.0000001m));
    }

    [Test]
    public void IslamicFraction_OneEighth_CorrectValue()
    {
        var fraction = new Fraction(1, 8);
        Assert.That(fraction.ToDecimal(), Is.EqualTo(0.125m));
        Assert.That(fraction.ToPercentage(), Is.EqualTo(12.5m));
    }

    [Test]
    public void IslamicFraction_TwoThirds_CorrectValue()
    {
        var fraction = new Fraction(2, 3);
        var decimalValue = fraction.ToDecimal();

        // 2/3 = 0.666... (repeating)
        Assert.That(decimalValue, Is.EqualTo(2m / 3m).Within(0.0000001m));
        Assert.That(fraction.ToPercentage(), Is.EqualTo(200m / 3m).Within(0.0001m));
    }

    [Test]
    public void AllSixIslamicFractions_CorrectValues()
    {
        Assert.That(new Fraction(1, 2).ToDecimal(), Is.EqualTo(0.5m));
        Assert.That(new Fraction(1, 4).ToDecimal(), Is.EqualTo(0.25m));
        Assert.That(new Fraction(1, 8).ToDecimal(), Is.EqualTo(0.125m));
        // rounding
        Assert.That(Math.Round(new Fraction(1, 3).ToDecimal(), 6), Is.EqualTo(0.333333m));
        Assert.That(Math.Round(new Fraction(1, 6).ToDecimal(), 6), Is.EqualTo(0.166667m));
        Assert.That(Math.Round(new Fraction(2, 3).ToDecimal(), 6), Is.EqualTo(0.666667m));
    }

    #endregion

    #region Real Inheritance Scenarios

    [Test]
    public void InheritanceScenario_WifeAndFather_CorrectShares()
    {
        // Wife: 1/8, Father: 1/6 (when deceased has children)
        var wife = new Fraction(1, 8);
        var father = new Fraction(1, 6);
        var total = wife + father;

        Assert.That(total.Numerator, Is.EqualTo(7));
        Assert.That(total.Denominator, Is.EqualTo(24));
    }

    [Test]
    public void InheritanceScenario_HusbandNoChildren_GetHalf()
    {
        // Husband gets 1/2 when wife leaves no descendants
        var husband = new Fraction(1, 2);

        Assert.That(husband.ToDecimal(), Is.EqualTo(0.5m));
        Assert.That(husband.ToPercentage(), Is.EqualTo(50m));
    }

    [Test]
    public void InheritanceScenario_HusbandWithChildren_GetsQuarter()
    {
        // Husband gets 1/4 when wife leaves descendants
        var husband = new Fraction(1, 4);

        Assert.That(husband.ToDecimal(), Is.EqualTo(0.25m));
    }

    [Test]
    public void InheritanceScenario_TwoDaughtersNoSons_GetTwoThirds()
    {
        // Two or more daughters without sons get 2/3
        var daughters = new Fraction(2, 3);

        Assert.That(daughters.Numerator, Is.EqualTo(2));
        Assert.That(daughters.Denominator, Is.EqualTo(3));
    }

    [Test]
    public void InheritanceScenario_AwlCase_SumExceedsOne()
    {
        // Awl scenario: Husband (1/4) + Mother (1/6) + Two Daughters (2/3)
        // This creates an "Awl" (عول) situation where total > 1
        var husband = new Fraction(1, 4);      // 3/12
        var mother = new Fraction(1, 6);       // 2/12
        var daughters = new Fraction(2, 3);    // 8/12

        var total = husband + mother + daughters;

        // Total should be 13/12 (greater than 1 - requires Awl adjustment)
        Assert.That(total.Numerator, Is.EqualTo(13));
        Assert.That(total.Denominator, Is.EqualTo(12));
        Assert.That(total > new Fraction(1, 1), Is.True);

        // This test documents the Awl case - the engine must handle this!
    }

    [Test]
    public void InheritanceScenario_ParentsBothAlive_CorrectShares()
    {
        // Father: 1/6, Mother: 1/6 (when deceased has children)
        var father = new Fraction(1, 6);
        var mother = new Fraction(1, 6);
        var parentsTotal = father + mother;

        Assert.That(parentsTotal.Numerator, Is.EqualTo(1));
        Assert.That(parentsTotal.Denominator, Is.EqualTo(3));
    }

    [Test]
    public void InheritanceScenario_MultipleWives_ShareDivision()
    {
        // If a man has 4 wives, they share 1/8 (when children exist)
        var totalWifeShare = new Fraction(1, 8);
        var perWife = totalWifeShare / 4;

        Assert.That(perWife.Numerator, Is.EqualTo(1));
        Assert.That(perWife.Denominator, Is.EqualTo(32));
    }

    [Test]
    public void InheritanceScenario_SonAndDaughter_TwoToOneRatio()
    {
        // If 1 son and 1 daughter share residue
        // Son gets 2x, daughter gets 1x, total = 3 parts
        var daughterShare = new Fraction(1, 3);
        var sonShare = new Fraction(2, 3);

        Assert.That(sonShare, Is.EqualTo(daughterShare * 2));
        Assert.That(sonShare + daughterShare, Is.EqualTo(new Fraction(1, 1)));
    }

    #endregion

    #region Conversion Tests

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
    public void ToDecimal_RepeatingDecimal_CorrectPrecision()
    {
        var fraction = new Fraction(1, 3);
        var result = fraction.ToDecimal();

        // C# decimal has 28-29 significant digits
        // 1/3 should be handled correctly
        Assert.That(result, Is.EqualTo(1m / 3m));
    }

    #endregion

    #region Equality and Comparison

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
    public void Equality_ZeroFractions_AreEqual()
    {
        var zero1 = new Fraction(0, 1);
        var zero2 = new Fraction(0, 5);

        Assert.That(zero1, Is.EqualTo(zero2));
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
    public void Comparison_NegativeFractions_CorrectOrdering()
    {
        var negative = new Fraction(-1, 4);
        var positive = new Fraction(1, 4);

        Assert.That(negative < positive, Is.True);
        Assert.That(positive > negative, Is.True);
    }

    #endregion

    #region String Representation

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
    public void ToString_Zero_ReturnsZero()
    {
        var fraction = new Fraction(0, 5);
        Assert.That(fraction.ToString(), Is.EqualTo("0"));
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

    #endregion

    #region Edge Cases and Stress Tests

    [Test]
    public void EdgeCase_VeryLargeNumerator_HandlesCorrectly()
    {
        // This tests if we can handle large values without overflow
        var fraction = new Fraction(1000000, 1);
        Assert.That(fraction.Numerator, Is.EqualTo(1000000));
        Assert.That(fraction.Denominator, Is.EqualTo(1));
    }

    [Test]
    public void EdgeCase_VeryLargeDenominator_HandlesCorrectly()
    {
        var fraction = new Fraction(1, 1000000);
        Assert.That(fraction.Numerator, Is.EqualTo(1));
        Assert.That(fraction.Denominator, Is.EqualTo(1000000));
    }

    [Test]
    public void EdgeCase_MultipleOperations_MaintainsNormalization()
    {
        var fraction = new Fraction(1, 2);
        fraction = fraction + new Fraction(1, 4);
        fraction = fraction - new Fraction(1, 8);

        // (1/2 + 1/4 - 1/8) = (4/8 + 2/8 - 1/8) = 5/8
        Assert.That(fraction.Numerator, Is.EqualTo(5));
        Assert.That(fraction.Denominator, Is.EqualTo(8));
    }

    #endregion
}
