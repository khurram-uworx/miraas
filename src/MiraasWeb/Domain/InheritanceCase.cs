namespace MiraasWeb.Domain;

/// <summary>
/// Aggregate root for an inheritance case.
/// Contains the deceased person and all heirs, representing the complete scenario for calculation.
/// </summary>
public class InheritanceCase
{
    /// <summary>
    /// The deceased person whose estate is being divided.
    /// </summary>
    public DeceasedPerson Deceased { get; }

    /// <summary>
    /// The list of heirs in this inheritance case.
    /// </summary>
    public List<Heir> Heirs { get; }

    /// <summary>
    /// The total estate value (optional, for monetary calculations).
    /// </summary>
    public decimal? EstateValue { get; set; }

    public InheritanceCase(DeceasedPerson deceased)
    {
        Deceased = deceased ?? throw new ArgumentNullException(nameof(deceased));
        Heirs = new List<Heir>();
    }

    /// <summary>
    /// Adds an heir to the case.
    /// </summary>
    public void AddHeir(Heir heir)
    {
        if (heir == null)
            throw new ArgumentNullException(nameof(heir));

        Heirs.Add(heir);
    }

    /// <summary>
    /// Adds multiple heirs to the case.
    /// </summary>
    public void AddHeirs(params Heir[] heirs)
    {
        foreach (var heir in heirs)
        {
            AddHeir(heir);
        }
    }

    /// <summary>
    /// Gets the count of heirs of a specific relation type.
    /// </summary>
    public int GetHeirCount(RelationType relationType)
    {
        return Heirs
            .Where(h => h.Relation == relationType)
            .Sum(h => h.Count);
    }

    /// <summary>
    /// Checks if any heir of a specific relation type exists.
    /// </summary>
    public bool HasHeir(RelationType relationType)
    {
        return Heirs.Any(h => h.Relation == relationType);
    }

    /// <summary>
    /// Gets all heirs of a specific relation type.
    /// </summary>
    public List<Heir> GetHeirsByType(RelationType relationType)
    {
        return Heirs.Where(h => h.Relation == relationType).ToList();
    }

    /// <summary>
    /// Gets all heirs that belong to a specific category.
    /// </summary>
    public List<Heir> GetHeirsByCategory(HeirCategory category)
    {
        return Heirs.Where(h => h.Category == category).ToList();
    }
}
