using MiraasWeb.Abstractions;

namespace MiraasWeb.Domain;

public enum RelationType
{
    // Direct Descendants
    Son, Daughter, SonOfSon, DaughterOfSon,

    // Ascendants
    Father, Mother, Grandfather, GrandmotherMaternal, GrandmotherPaternal,

    // Spouse
    Husband, Wife,

    // same father and mothe)
    FullBrother, FullSister,

    // same father, different mother
    ConsanguineBrother, ConsanguineSister,

    // same mother, different father
    UterineBrother, UterineSister
}

public class ShareResult
{
    public Fraction Fraction { get; set; } = Fraction.Zero;
    public decimal Percentage => Fraction.ToPercentage();
    public decimal Amount { get; set; }
    public string Explanation { get; set; } = string.Empty; // Citation etc
    public bool IsBlocked { get; set; }

    public ShareResult(Fraction fraction)
    {
        Fraction = fraction;
    }

    public ShareResult() { }

    public override string ToString()
    {
        if (this.IsBlocked) return "Blocked";
        if (this.Fraction == Fraction.Zero) return "Zero";

        if (!string.IsNullOrEmpty(this.Explanation) && this.Amount > decimal.Zero)
            return $"{this.Fraction} {this.Explanation} [{this.Amount}]";
        else if (!string.IsNullOrEmpty(this.Explanation))
            return $"{this.Fraction} {this.Explanation}";
        else if (this.Amount > decimal.Zero)
            return $"{this.Fraction} [{this.Amount}]";
        else
            return this.Fraction.ToString();
    }
}


// public because of tests
public abstract class Heir
{
    ShareResult shareResult = new();

    public abstract GenderType Gender { get; }
    public abstract RelationType Relation { get; }

    public int Count { get; set; } = 1;

    public ShareResult Result => this.shareResult;

    protected Heir(int count = 1)
    {
        if (count < 1)
            throw new ArgumentException("Count must be at least 1.", nameof(count));

        Count = count;
    }

    public override string ToString() =>
        this.shareResult.Fraction > Fraction.Zero
        ? string.Join(Environment.NewLine,
            this.Count > 0 ? $"Relation: {this.Relation} [{this.Count}]" : $"Relation: {this.Relation}",
            $"Share: {this.shareResult}")
        : this.Count > 0 ? $"Relation: {this.Relation} [{this.Count}]" : $"Relation: {this.Relation}";

    public void AddShare(ShareResult value)
    {
        var oldResult = this.shareResult;
        this.shareResult = value;

        if (oldResult.Fraction.Numerator > 0)
        {
            this.shareResult.Amount += oldResult.Amount;
            this.shareResult.Fraction += oldResult.Fraction;
            if (!string.IsNullOrWhiteSpace(oldResult.Explanation)) this.shareResult.Explanation = $"{oldResult.Explanation}, {this.shareResult.Explanation}";
        }
    }

    public string RelationTypeToString() =>
        Relation switch
        {
            RelationType.DaughterOfSon => "Grand Son",
            RelationType.SonOfSon => "Grand Daughter",
            RelationType.FullSister => "Full Sister",
            RelationType.FullBrother => "Full Brother",
            RelationType.ConsanguineBrother => "Consanguine Brother",
            RelationType.ConsanguineSister => "Consanguine Sister",
            RelationType.UterineBrother => "Uterine Brother",
            RelationType.UterineSister => "Uterine Sister",
            RelationType.GrandmotherPaternal => "Grandmother (Paternal)",
            RelationType.GrandmotherMaternal => "Grandmother (Maternal)",

            _ => Relation.ToString()
        };
}

// Direct Descendants
public class Son : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.Son;

    public Son(int count = 1) : base(count) { }
}

public class Daughter : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.Daughter;

    public Daughter(int count = 1) : base(count) { }
}

public class SonOfSon : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.SonOfSon;

    public SonOfSon(int count = 1) : base(count) { }
}

public class DaughterOfSon : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.DaughterOfSon;

    public DaughterOfSon(int count = 1) : base(count) { }
}

// Ascendants
public class Father : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.Father;

    public Father() : base(1) { }
}

public class Mother : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.Mother;

    public Mother() : base(1) { }
}

public class Grandfather : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.Grandfather;

    public Grandfather() : base(1) { }
}

public class GrandmotherMaternal : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.GrandmotherMaternal;

    public GrandmotherMaternal() : base(1) { }
}

public class GrandmotherPaternal : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.GrandmotherPaternal;

    public GrandmotherPaternal() : base(1) { }
}

// Spouse
public class Husband : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.Husband;

    public Husband(int count = 1) : base(count) { }
}

public class Wife : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.Wife;

    public Wife(int count = 1) : base(count) { }
}

// Full Siblings
public class FullBrother : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.FullBrother;

    public FullBrother(int count = 1) : base(count) { }
}

public class FullSister : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.FullSister;

    public FullSister(int count = 1) : base(count) { }
}

// same father, different mother
public class ConsanguineBrother : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.ConsanguineBrother;

    public ConsanguineBrother(int count = 1) : base(count) { }
}

public class ConsanguineSister : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.ConsanguineSister;

    public ConsanguineSister(int count = 1) : base(count) { }
}

// same mother, different father
public class UterineBrother : Heir
{
    public override GenderType Gender => GenderType.Male;
    public override RelationType Relation => RelationType.UterineBrother;

    public UterineBrother(int count = 1) : base(count) { }
}

public class UterineSister : Heir
{
    public override GenderType Gender => GenderType.Female;
    public override RelationType Relation => RelationType.UterineSister;

    public UterineSister(int count = 1) : base(count) { }
}
