using MiraasWeb.Abstractions;

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
        var zero = Fraction.Zero;
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
        var fraction2 = Fraction.Zero;

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
        Assert.That(Fraction.Half.ToDecimal(), Is.EqualTo(0.5m));
        Assert.That(Fraction.Quarter.ToDecimal(), Is.EqualTo(0.25m));
        Assert.That(Fraction.Eighth.ToDecimal(), Is.EqualTo(0.125m));
        // rounding
        Assert.That(Math.Round(Fraction.Third.ToDecimal(), 6), Is.EqualTo(0.333333m));
        Assert.That(Math.Round(Fraction.Sixth.ToDecimal(), 6), Is.EqualTo(0.166667m));
        Assert.That(Math.Round(Fraction.TwoThirds.ToDecimal(), 6), Is.EqualTo(0.666667m));
    }

    #endregion

    #region Real Inheritance Scenarios

    [Test]
    public void InheritanceScenario_WifeAndFather_CorrectShares()
    {
        // Wife: 1/8, Father: 1/6 (when deceased has children)
        var wife = Fraction.Eighth;
        var father = Fraction.Sixth;
        var total = wife + father;

        Assert.That(total.Numerator, Is.EqualTo(7));
        Assert.That(total.Denominator, Is.EqualTo(24));
    }

    [Test]
    public void InheritanceScenario_HusbandNoChildren_GetHalf()
    {
        // Husband gets 1/2 when wife leaves no descendants
        var husband = Fraction.Half;

        Assert.That(husband.ToDecimal(), Is.EqualTo(0.5m));
        Assert.That(husband.ToPercentage(), Is.EqualTo(50m));
    }

    [Test]
    public void InheritanceScenario_HusbandWithChildren_GetsQuarter()
    {
        // Husband gets 1/4 when wife leaves descendants
        var husband = Fraction.Quarter;

        Assert.That(husband.ToDecimal(), Is.EqualTo(0.25m));
    }

    [Test]
    public void InheritanceScenario_TwoDaughtersNoSons_GetTwoThirds()
    {
        // Two or more daughters without sons get 2/3
        var daughters = Fraction.TwoThirds;

        Assert.That(daughters.Numerator, Is.EqualTo(2));
        Assert.That(daughters.Denominator, Is.EqualTo(3));
    }

    [Test]
    public void InheritanceScenario_AwlCase_SumExceedsOne()
    {
        // Awl scenario: Husband (1/4) + Mother (1/6) + Two Daughters (2/3)
        // This creates an "Awl" (عول) situation where total > 1
        var husband = Fraction.Quarter;      // 3/12
        var mother = Fraction.Sixth;       // 2/12
        var daughters = Fraction.TwoThirds;    // 8/12

        var total = husband + mother + daughters;

        // Total should be 13/12 (greater than 1 - requires Awl adjustment)
        Assert.That(total.Numerator, Is.EqualTo(13));
        Assert.That(total.Denominator, Is.EqualTo(12));
        Assert.That(total > Fraction.One, Is.True);

        // This test documents the Awl case - the engine must handle this!
    }

    [Test]
    public void InheritanceScenario_ParentsBothAlive_CorrectShares()
    {
        // Father: 1/6, Mother: 1/6 (when deceased has children)
        var father = Fraction.Sixth;
        var mother = Fraction.Sixth;
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
        var daughterShare = Fraction.Third;
        var sonShare = Fraction.TwoThirds;

        Assert.That(sonShare, Is.EqualTo(daughterShare * 2));
        Assert.That(sonShare + daughterShare, Is.EqualTo(Fraction.One));
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

    #region IComparable Tests

    [Test]
    public void CompareTo_LessThan_ReturnsNegative()
    {
        var fraction1 = new Fraction(1, 4);
        var fraction2 = new Fraction(1, 2);

        Assert.That(fraction1.CompareTo(fraction2), Is.LessThan(0));
    }

    [Test]
    public void CompareTo_GreaterThan_ReturnsPositive()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(1, 4);

        Assert.That(fraction1.CompareTo(fraction2), Is.GreaterThan(0));
    }

    [Test]
    public void CompareTo_Equal_ReturnsZero()
    {
        var fraction1 = new Fraction(1, 2);
        var fraction2 = new Fraction(2, 4);

        Assert.That(fraction1.CompareTo(fraction2), Is.EqualTo(0));
    }

    [Test]
    public void CompareTo_Zero_ReturnsCorrectSign()
    {
        var zero = Fraction.Zero;
        var positive = new Fraction(1, 4);
        var negative = new Fraction(-1, 4);

        Assert.That(zero.CompareTo(positive), Is.LessThan(0));
        Assert.That(zero.CompareTo(negative), Is.GreaterThan(0));
        Assert.That(zero.CompareTo(zero), Is.EqualTo(0));
    }

    [Test]
    public void CompareTo_WorksWithGreaterThanOrEqualTo()
    {
        // This test verifies that IComparable works with NUnit's Is.GreaterThanOrEqualTo
        var half = Fraction.Half;
        var quarter = Fraction.Quarter;
        var third = Fraction.Third;

        Assert.That(half, Is.GreaterThanOrEqualTo(quarter));
        Assert.That(half, Is.GreaterThanOrEqualTo(half));
        Assert.That(third, Is.GreaterThanOrEqualTo(quarter));
    }

    [Test]
    public void CompareTo_WorksWithLessThanOrEqualTo()
    {
        var quarter = Fraction.Quarter;
        var half = Fraction.Half;
        var third = Fraction.Third;

        Assert.That(quarter, Is.LessThanOrEqualTo(half));
        Assert.That(quarter, Is.LessThanOrEqualTo(quarter));
        Assert.That(quarter, Is.LessThanOrEqualTo(third));
    }

    [Test]
    public void CompareTo_IslamicFractions_CorrectOrdering()
    {
        // Test ordering of all Islamic fractions
        var eighth = Fraction.Eighth;
        var sixth = Fraction.Sixth;
        var quarter = Fraction.Quarter;
        var third = Fraction.Third;
        var half = Fraction.Half;
        var twoThirds = Fraction.TwoThirds;

        Assert.That(eighth.CompareTo(sixth), Is.LessThan(0));
        Assert.That(sixth.CompareTo(quarter), Is.LessThan(0));
        Assert.That(quarter.CompareTo(third), Is.LessThan(0));
        Assert.That(third.CompareTo(half), Is.LessThan(0));
        Assert.That(half.CompareTo(twoThirds), Is.LessThan(0));
    }

    [Test]
    public void CompareTo_NegativeFractions_CorrectOrdering()
    {
        var negative = new Fraction(-1, 2);
        var zero = Fraction.Zero;
        var positive = new Fraction(1, 2);

        Assert.That(negative.CompareTo(zero), Is.LessThan(0));
        Assert.That(zero.CompareTo(positive), Is.LessThan(0));
        Assert.That(negative.CompareTo(positive), Is.LessThan(0));
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

    #region Factory Methods

    [Test]
    public void Factory_Zero_ReturnsZeroFraction()
    {
        var zero = Fraction.Zero;
        Assert.That(zero.Numerator, Is.EqualTo(0));
        Assert.That(zero.Denominator, Is.EqualTo(1));
        Assert.That(zero, Is.EqualTo(new Fraction(0, 1)));
    }

    [Test]
    public void Factory_One_ReturnsOneFraction()
    {
        var one = Fraction.One;
        Assert.That(one.Numerator, Is.EqualTo(1));
        Assert.That(one.Denominator, Is.EqualTo(1));
        Assert.That(one, Is.EqualTo(new Fraction(1, 1)));
    }

    [Test]
    public void Factory_Whole_PositiveValue_ReturnsCorrectFraction()
    {
        var whole = Fraction.Whole(5);
        Assert.That(whole.Numerator, Is.EqualTo(5));
        Assert.That(whole.Denominator, Is.EqualTo(1));
        Assert.That(whole, Is.EqualTo(new Fraction(5, 1)));
    }

    [Test]
    public void Factory_Whole_NegativeValue_ReturnsCorrectFraction()
    {
        var whole = Fraction.Whole(-3);
        Assert.That(whole.Numerator, Is.EqualTo(-3));
        Assert.That(whole.Denominator, Is.EqualTo(1));
        Assert.That(whole, Is.EqualTo(new Fraction(-3, 1)));
    }

    [Test]
    public void Factory_Whole_Zero_ReturnsZero()
    {
        var whole = Fraction.Whole(0);
        Assert.That(whole, Is.EqualTo(Fraction.Zero));
    }

    [Test]
    public void Factory_Whole_One_ReturnsOne()
    {
        var whole = Fraction.Whole(1);
        Assert.That(whole, Is.EqualTo(Fraction.One));
    }

    [Test]
    public void Factory_Zero_CanBeUsedInArithmetic()
    {
        var fraction = new Fraction(1, 4);
        var result = fraction + Fraction.Zero;
        Assert.That(result, Is.EqualTo(fraction));
    }

    [Test]
    public void Factory_One_CanBeUsedInArithmetic()
    {
        var fraction = new Fraction(1, 2);
        var result = fraction + Fraction.One;
        Assert.That(result, Is.EqualTo(new Fraction(3, 2)));
    }

    [Test]
    public void Factory_Whole_CanBeUsedInArithmetic()
    {
        var fraction = new Fraction(1, 4);
        var whole = Fraction.Whole(2);
        var result = fraction + whole;
        Assert.That(result, Is.EqualTo(new Fraction(9, 4)));
    }

    #endregion

    #region Islamic Inheritance Fraction Factory Methods

    [Test]
    public void Factory_Half_ReturnsCorrectFraction()
    {
        var half = Fraction.Half;
        Assert.That(half.Numerator, Is.EqualTo(1));
        Assert.That(half.Denominator, Is.EqualTo(2));
        Assert.That(half, Is.EqualTo(new Fraction(1, 2)));
        Assert.That(half.ToDecimal(), Is.EqualTo(0.5m));
    }

    [Test]
    public void Factory_Third_ReturnsCorrectFraction()
    {
        var third = Fraction.Third;
        Assert.That(third.Numerator, Is.EqualTo(1));
        Assert.That(third.Denominator, Is.EqualTo(3));
        Assert.That(third, Is.EqualTo(new Fraction(1, 3)));
    }

    [Test]
    public void Factory_Quarter_ReturnsCorrectFraction()
    {
        var quarter = Fraction.Quarter;
        Assert.That(quarter.Numerator, Is.EqualTo(1));
        Assert.That(quarter.Denominator, Is.EqualTo(4));
        Assert.That(quarter, Is.EqualTo(new Fraction(1, 4)));
        Assert.That(quarter.ToDecimal(), Is.EqualTo(0.25m));
    }

    [Test]
    public void Factory_Sixth_ReturnsCorrectFraction()
    {
        var sixth = Fraction.Sixth;
        Assert.That(sixth.Numerator, Is.EqualTo(1));
        Assert.That(sixth.Denominator, Is.EqualTo(6));
        Assert.That(sixth, Is.EqualTo(new Fraction(1, 6)));
    }

    [Test]
    public void Factory_Eighth_ReturnsCorrectFraction()
    {
        var eighth = Fraction.Eighth;
        Assert.That(eighth.Numerator, Is.EqualTo(1));
        Assert.That(eighth.Denominator, Is.EqualTo(8));
        Assert.That(eighth, Is.EqualTo(new Fraction(1, 8)));
        Assert.That(eighth.ToDecimal(), Is.EqualTo(0.125m));
    }

    [Test]
    public void Factory_TwoThirds_ReturnsCorrectFraction()
    {
        var twoThirds = Fraction.TwoThirds;
        Assert.That(twoThirds.Numerator, Is.EqualTo(2));
        Assert.That(twoThirds.Denominator, Is.EqualTo(3));
        Assert.That(twoThirds, Is.EqualTo(new Fraction(2, 3)));
    }

    [Test]
    public void Factory_AllQuranicShares_CanBeUsedInArithmetic()
    {
        // Test that all factory methods work in arithmetic operations
        var total = Fraction.Half + Fraction.Third + Fraction.Quarter;
        Assert.That(total.Numerator, Is.EqualTo(13));
        Assert.That(total.Denominator, Is.EqualTo(12));
    }

    [Test]
    public void Factory_IslamicFractions_SumToCorrectValues()
    {
        // Common Islamic inheritance scenario: Wife (1/8) + Father (1/6)
        var wife = Fraction.Eighth;
        var father = Fraction.Sixth;
        var total = wife + father;

        Assert.That(total.Numerator, Is.EqualTo(7));
        Assert.That(total.Denominator, Is.EqualTo(24));
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
