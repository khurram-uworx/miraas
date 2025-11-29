namespace MiraasWeb.Domain;

/// <summary>
/// Represents a blocking rule (Hijab) in Islamic inheritance.
/// A blocker heir prevents a blocked heir from inheriting.
/// </summary>
public class BlockingRule
{
    public RelationType Blocker { get; }
    public RelationType Blocked { get; }

    public BlockingRule(RelationType blocker, RelationType blocked)
    {
        Blocker = blocker;
        Blocked = blocked;
    }

    public override string ToString()
    {
        return $"{Blocker} blocks {Blocked}";
    }
}

/// <summary>
/// Engine that applies blocking rules to determine which heirs are blocked from inheritance.
/// Based on Islamic inheritance law (Hijab al-Nuqsan and Hijab al-Khas).
/// </summary>
public class BlockingRuleEngine
{
    readonly List<BlockingRule> rules;

    public BlockingRuleEngine()
    {
        rules = new List<BlockingRule>();
        initializeBlockingRules();
    }

    /// <summary>
    /// Initializes all blocking rules based on Islamic inheritance law.
    /// </summary>
    void initializeBlockingRules()
    {
        // Father blocks grandfather (son blocks grandfather)
        rules.Add(new BlockingRule(RelationType.Son, RelationType.Grandfather));
        rules.Add(new BlockingRule(RelationType.Father, RelationType.Grandfather));

        // Grandfather blocks uncles (but not modeled here yet)

        // Son blocks grandson (son of son)
        rules.Add(new BlockingRule(RelationType.Son, RelationType.SonOfSon));

        // Father blocks brothers
        rules.Add(new BlockingRule(RelationType.Father, RelationType.FullBrother));
        rules.Add(new BlockingRule(RelationType.Father, RelationType.FullSister));
        rules.Add(new BlockingRule(RelationType.Father, RelationType.ConsanguineBrother));
        rules.Add(new BlockingRule(RelationType.Father, RelationType.ConsanguineSister));

        // Son blocks brothers and sisters
        rules.Add(new BlockingRule(RelationType.Son, RelationType.FullBrother));
        rules.Add(new BlockingRule(RelationType.Son, RelationType.FullSister));
        rules.Add(new BlockingRule(RelationType.Son, RelationType.ConsanguineBrother));
        rules.Add(new BlockingRule(RelationType.Son, RelationType.ConsanguineSister));
        rules.Add(new BlockingRule(RelationType.Son, RelationType.UterineBrother));
        rules.Add(new BlockingRule(RelationType.Son, RelationType.UterineSister));

        // Son of Son blocks brothers, sisters, and uterine siblings (as descendant)
        rules.Add(new BlockingRule(RelationType.SonOfSon, RelationType.FullBrother));
        rules.Add(new BlockingRule(RelationType.SonOfSon, RelationType.FullSister));
        rules.Add(new BlockingRule(RelationType.SonOfSon, RelationType.ConsanguineBrother));
        rules.Add(new BlockingRule(RelationType.SonOfSon, RelationType.ConsanguineSister));
        rules.Add(new BlockingRule(RelationType.SonOfSon, RelationType.UterineBrother));
        rules.Add(new BlockingRule(RelationType.SonOfSon, RelationType.UterineSister));

        // Daughter blocks granddaughter (daughter of son) only when no son exists
        // This is handled in the engine logic, not as a simple rule

        // Father blocks maternal grandmother
        rules.Add(new BlockingRule(RelationType.Father, RelationType.GrandmotherMaternal));

        // Mother blocks maternal grandmother
        rules.Add(new BlockingRule(RelationType.Mother, RelationType.GrandmotherMaternal));

        // Father blocks paternal grandmother
        rules.Add(new BlockingRule(RelationType.Father, RelationType.GrandmotherPaternal));

        // Grandfather blocks paternal grandmother
        rules.Add(new BlockingRule(RelationType.Grandfather, RelationType.GrandmotherPaternal));

        // Granddaughter (of son) blocks uterine siblings
        // Complex rule - handled in engine

        // Full sister blocks consanguine sister in certain contexts
        // Handled in engine
    }

    /// <summary>
    /// Applies complex blocking rules that depend on multiple conditions.
    /// </summary>
    void applyComplexBlockingRules(InheritanceCase case_, HashSet<RelationType> blocked)
    {
        // If daughter exists (no son), daughter blocks daughter of son
        if (case_.HasHeir(RelationType.Daughter) && !case_.HasHeir(RelationType.Son))
        {
            blocked.Add(RelationType.DaughterOfSon);
        }

        // Daughter of son blocks uterine siblings in certain contexts
        if (case_.HasHeir(RelationType.DaughterOfSon) && !case_.HasHeir(RelationType.Son))
        {
            blocked.Add(RelationType.UterineBrother);
            blocked.Add(RelationType.UterineSister);
        }

        // Son of son blocks grandson through another son
        // (complex - would need more detail)

        // If grandchildren exist, grandparents are blocked
        bool hasGrandchildren = case_.HasHeir(RelationType.SonOfSon) ||
                               case_.HasHeir(RelationType.DaughterOfSon);
        if (hasGrandchildren && case_.HasHeir(RelationType.Father))
        {
            // Father takes precedence, grandparents are blocked
            blocked.Add(RelationType.Grandfather);
            blocked.Add(RelationType.GrandmotherMaternal);
            blocked.Add(RelationType.GrandmotherPaternal);
        }
    }

    /// <summary>
    /// Applies blocking rules to determine blocked heirs.
    /// </summary>
    public List<RelationType> GetBlockedHeirs(InheritanceCase case_)
    {
        var blocked = new HashSet<RelationType>();

        foreach (var rule in rules)
        {
            if (case_.HasHeir(rule.Blocker))
            {
                blocked.Add(rule.Blocked);
            }
        }

        // Additional complex blocking logic
        applyComplexBlockingRules(case_, blocked);

        return blocked.ToList();
    }

    /// <summary>
    /// Checks if a specific heir is blocked.
    /// </summary>
    public bool IsBlocked(RelationType heirType, InheritanceCase case_)
    {
        return GetBlockedHeirs(case_).Contains(heirType);
    }
}
