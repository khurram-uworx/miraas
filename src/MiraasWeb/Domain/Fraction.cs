namespace MiraasWeb.Domain;

/// <summary>
/// Represents a rational number (fraction) for precise inheritance calculations.
/// Automatically normalizes to lowest terms.
/// </summary>
public struct Fraction : IEquatable<Fraction>
{
    public int Numerator { get; private set; }
    public int Denominator { get; private set; }

    /// <summary>
    /// Creates a new fraction and normalizes it.
    /// </summary>
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

    /// <summary>
    /// Normalizes the fraction to its lowest terms.
    /// </summary>
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

    /// <summary>
    /// Calculates the Greatest Common Divisor.
    /// </summary>
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

    /// <summary>
    /// Converts fraction to decimal.
    /// </summary>
    public decimal ToDecimal()
    {
        return (decimal)Numerator / Denominator;
    }

    /// <summary>
    /// Converts fraction to percentage.
    /// </summary>
    public decimal ToPercentage()
    {
        return ToDecimal() * 100;
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

    public static Fraction operator *(Fraction a, Fraction b)
    {
        return new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
    }

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.Numerator == 0)
            throw new ArgumentException("Cannot divide by zero fraction.");
        
        return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
    }

    public static Fraction operator *(Fraction a, int scalar)
    {
        return new Fraction(a.Numerator * scalar, a.Denominator);
    }

    public static Fraction operator /(Fraction a, int scalar)
    {
        if (scalar == 0)
            throw new ArgumentException("Cannot divide by zero.");
        
        return new Fraction(a.Numerator, a.Denominator * scalar);
    }

    public static bool operator ==(Fraction a, Fraction b)
    {
        return a.Numerator == b.Numerator && a.Denominator == b.Denominator;
    }

    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }

    public static bool operator <(Fraction a, Fraction b)
    {
        return a.ToDecimal() < b.ToDecimal();
    }

    public static bool operator <=(Fraction a, Fraction b)
    {
        return a.ToDecimal() <= b.ToDecimal();
    }

    public static bool operator >(Fraction a, Fraction b)
    {
        return a.ToDecimal() > b.ToDecimal();
    }

    public static bool operator >=(Fraction a, Fraction b)
    {
        return a.ToDecimal() >= b.ToDecimal();
    }

    public bool Equals(Fraction other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is Fraction fraction && Equals(fraction);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Numerator, Denominator);
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
}
