namespace MiraasWeb.Abstractions;

public struct Fraction : IEquatable<Fraction>, IComparable<Fraction>
{
    public int Numerator { get; private set; }
    public int Denominator { get; private set; }

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
            throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        Numerator = numerator;
        Denominator = denominator;
        normalize();
    }

    void normalize()
    {
        if (Numerator == 0)
        {
            Denominator = 1;
            return;
        }

        int gcd = Fraction.gcd(Math.Abs(Numerator), Math.Abs(Denominator));
        Numerator /= gcd;
        Denominator /= gcd;
    }

    static int gcd(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public decimal ToDecimal() =>
        (decimal)Numerator / Denominator;

    public decimal ToPercentage() =>
        ToDecimal() * 100;

    public bool Equals(Fraction other) =>
        this == other;

    public override bool Equals(object? obj) =>
        obj is Fraction fraction && Equals(fraction);

    public override int GetHashCode() =>
        HashCode.Combine(Numerator, Denominator);

    public int CompareTo(Fraction other)
    {
        // Compare using decimal values for accuracy
        decimal thisValue = ToDecimal();
        decimal otherValue = other.ToDecimal();

        if (thisValue < otherValue)
            return -1;
        if (thisValue > otherValue)
            return 1;
        return 0;
    }

    public override string ToString()
    {
        if (Denominator == 1)
            return Numerator.ToString();

        return $"{Numerator}/{Denominator}";
    }

    public string ToMixedNumber()
    {
        if (Numerator < Denominator)
            return ToString();

        int whole = Numerator / Denominator;
        int remainder = Numerator % Denominator;

        if (remainder == 0)
            return whole.ToString();

        return $"{whole} {remainder}/{Denominator}";
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int numerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
        int denominator = a.Denominator * b.Denominator;
        return new Fraction(numerator, denominator);
    }

    public static Fraction operator -(Fraction a, Fraction b)
    {
        int numerator = a.Numerator * b.Denominator - b.Numerator * a.Denominator;
        int denominator = a.Denominator * b.Denominator;
        return new Fraction(numerator, denominator);
    }

    public static Fraction operator *(Fraction a, Fraction b) =>
        new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.Numerator == 0)
            throw new ArgumentException("Cannot divide by zero fraction.");

        return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
    }

    public static Fraction operator *(Fraction a, int scalar) =>
        new Fraction(a.Numerator * scalar, a.Denominator);

    public static Fraction operator /(Fraction a, int scalar)
    {
        if (scalar == 0)
            throw new ArgumentException("Cannot divide by zero.");

        return new Fraction(a.Numerator, a.Denominator * scalar);
    }

    public static bool operator ==(Fraction a, Fraction b) =>
        a.Numerator == b.Numerator && a.Denominator == b.Denominator;

    public static bool operator !=(Fraction a, Fraction b) =>
        !(a == b);

    public static bool operator <(Fraction a, Fraction b) =>
        a.ToDecimal() < b.ToDecimal();

    public static bool operator <=(Fraction a, Fraction b) =>
        a.ToDecimal() <= b.ToDecimal();

    public static bool operator >(Fraction a, Fraction b) =>
        a.ToDecimal() > b.ToDecimal();

    public static bool operator >=(Fraction a, Fraction b) =>
        a.ToDecimal() >= b.ToDecimal();

    // Factory methods for common values
    public static Fraction Zero => new Fraction(0, 1);

    public static Fraction One => new Fraction(1, 1);

    public static Fraction Whole(int value) => new Fraction(value, 1);

    // Factory methods for Islamic inheritance fractions (Quranic shares)
    public static Fraction Half => new Fraction(1, 2);        // 1/2

    public static Fraction Third => new Fraction(1, 3);      // 1/3

    public static Fraction Quarter => new Fraction(1, 4);     // 1/4

    public static Fraction Sixth => new Fraction(1, 6);       // 1/6

    public static Fraction Eighth => new Fraction(1, 8);      // 1/8

    public static Fraction TwoThirds => new Fraction(2, 3);   // 2/3
}
