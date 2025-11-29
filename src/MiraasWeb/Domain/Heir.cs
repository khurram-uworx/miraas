namespace MiraasWeb.Domain;

/// <summary>
/// Base class for all heirs in Islamic inheritance.
/// </summary>
public abstract class Heir
{
    /// <summary>
    /// The relationship type of this heir to the deceased.
    /// </summary>
    public abstract RelationType Relation { get; }

    /// <summary>
    /// The number of heirs of this type (e.g., 2 daughters).
    /// </summary>
    public int Count { get; set; } = 1;

    /// <summary>
    /// The calculated share result for this heir.
    /// </summary>
    public ShareResult Result { get; set; } = new();

    /// <summary>
    /// Gets the category of this heir (FixedShare, Residuary, Both, Blocked).
    /// </summary>
    public abstract HeirCategory Category { get; }

    protected Heir(int count = 1)
    {
        if (count < 1)
            throw new ArgumentException("Count must be at least 1.", nameof(count));
        
        Count = count;
    }
}

// Direct Descendants
public class Son : Heir
{
    public override RelationType Relation => RelationType.Son;
    public override HeirCategory Category => HeirCategory.Residuary;

    public Son(int count = 1) : base(count) { }
}

public class Daughter : Heir
{
    public override RelationType Relation => RelationType.Daughter;
    public override HeirCategory Category => HeirCategory.Both;

    public Daughter(int count = 1) : base(count) { }
}

public class SonOfSon : Heir
{
    public override RelationType Relation => RelationType.SonOfSon;
    public override HeirCategory Category => HeirCategory.Residuary;

    public SonOfSon(int count = 1) : base(count) { }
}

public class DaughterOfSon : Heir
{
    public override RelationType Relation => RelationType.DaughterOfSon;
    public override HeirCategory Category => HeirCategory.Both;

    public DaughterOfSon(int count = 1) : base(count) { }
}

// Ascendants
public class Father : Heir
{
    public override RelationType Relation => RelationType.Father;
    public override HeirCategory Category => HeirCategory.Both;

    public Father() : base(1) { }
}

public class Mother : Heir
{
    public override RelationType Relation => RelationType.Mother;
    public override HeirCategory Category => HeirCategory.Both;

    public Mother() : base(1) { }
}

public class Grandfather : Heir
{
    public override RelationType Relation => RelationType.Grandfather;
    public override HeirCategory Category => HeirCategory.Both;

    public Grandfather() : base(1) { }
}

public class GrandmotherMaternal : Heir
{
    public override RelationType Relation => RelationType.GrandmotherMaternal;
    public override HeirCategory Category => HeirCategory.FixedShare;

    public GrandmotherMaternal() : base(1) { }
}

public class GrandmotherPaternal : Heir
{
    public override RelationType Relation => RelationType.GrandmotherPaternal;
    public override HeirCategory Category => HeirCategory.FixedShare;

    public GrandmotherPaternal() : base(1) { }
}

// Spouse
public class Husband : Heir
{
    public override RelationType Relation => RelationType.Husband;
    public override HeirCategory Category => HeirCategory.Both;

    public Husband() : base(1) { }
}

public class Wife : Heir
{
    public override RelationType Relation => RelationType.Wife;
    public override HeirCategory Category => HeirCategory.FixedShare;

    public Wife(int count = 1) : base(count) { }
}

// Full Siblings
public class FullBrother : Heir
{
    public override RelationType Relation => RelationType.FullBrother;
    public override HeirCategory Category => HeirCategory.Both;

    public FullBrother(int count = 1) : base(count) { }
}

public class FullSister : Heir
{
    public override RelationType Relation => RelationType.FullSister;
    public override HeirCategory Category => HeirCategory.Both;

    public FullSister(int count = 1) : base(count) { }
}

// Consanguine Siblings (same father, different mother)
public class ConsanguineBrother : Heir
{
    public override RelationType Relation => RelationType.ConsanguineBrother;
    public override HeirCategory Category => HeirCategory.Both;

    public ConsanguineBrother(int count = 1) : base(count) { }
}

public class ConsanguineSister : Heir
{
    public override RelationType Relation => RelationType.ConsanguineSister;
    public override HeirCategory Category => HeirCategory.Both;

    public ConsanguineSister(int count = 1) : base(count) { }
}

// Uterine Siblings (same mother, different father)
public class UterineBrother : Heir
{
    public override RelationType Relation => RelationType.UterineBrother;
    public override HeirCategory Category => HeirCategory.FixedShare;

    public UterineBrother(int count = 1) : base(count) { }
}

public class UterineSister : Heir
{
    public override RelationType Relation => RelationType.UterineSister;
    public override HeirCategory Category => HeirCategory.FixedShare;

    public UterineSister(int count = 1) : base(count) { }
}
