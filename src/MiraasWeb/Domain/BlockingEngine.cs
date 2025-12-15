namespace MiraasWeb.Domain;

class BlockingRule
{
    public RelationType Blocker { get; }
    public RelationType Blocked { get; }

    public BlockingRule(RelationType blocker, RelationType blocked)
    {
        Blocker = blocker;
        Blocked = blocked;
    }

    public override string ToString() =>
        $"{Blocker} blocks {Blocked}";
}

// public because of tests
public class BlockingEngine
{
    readonly List<BlockingRule> rules;

    public BlockingEngine()
    {
        rules =
        [
            new BlockingRule(RelationType.Son, RelationType.Grandfather),
            new BlockingRule(RelationType.Father, RelationType.Grandfather),
            
            new BlockingRule(RelationType.Son, RelationType.SonOfSon),
            
            new BlockingRule(RelationType.Father, RelationType.FullBrother),
            new BlockingRule(RelationType.Father, RelationType.FullSister),
            
            new BlockingRule(RelationType.Father, RelationType.ConsanguineBrother),
            new BlockingRule(RelationType.Father, RelationType.ConsanguineSister),

            new BlockingRule(RelationType.Son, RelationType.FullBrother),
            new BlockingRule(RelationType.Son, RelationType.FullSister),
            new BlockingRule(RelationType.Son, RelationType.ConsanguineBrother),
            new BlockingRule(RelationType.Son, RelationType.ConsanguineSister),
            new BlockingRule(RelationType.Son, RelationType.UterineBrother),
            new BlockingRule(RelationType.Son, RelationType.UterineSister),

            new BlockingRule(RelationType.SonOfSon, RelationType.FullBrother),
            new BlockingRule(RelationType.SonOfSon, RelationType.FullSister),
            new BlockingRule(RelationType.SonOfSon, RelationType.ConsanguineBrother),
            new BlockingRule(RelationType.SonOfSon, RelationType.ConsanguineSister),
            new BlockingRule(RelationType.SonOfSon, RelationType.UterineBrother),
            new BlockingRule(RelationType.SonOfSon, RelationType.UterineSister),

            new BlockingRule(RelationType.Father, RelationType.GrandmotherMaternal),

            new BlockingRule(RelationType.Mother, RelationType.GrandmotherMaternal),

            new BlockingRule(RelationType.Father, RelationType.GrandmotherPaternal),

            // full sister doesnt block uterine sister
            new BlockingRule(RelationType.FullSister, RelationType.ConsanguineSister),

            new BlockingRule(RelationType.Grandfather, RelationType.GrandmotherPaternal)
        ];
    }

    public List<RelationType> GetBlockedHeirs(InheritanceCase inheritanceCase)
    {
        var blocked = new HashSet<RelationType>();

        foreach (var rule in rules)
            if (inheritanceCase.HasHeir(rule.Blocker))
                blocked.Add(rule.Blocked);

        if (inheritanceCase.HasHeir(RelationType.Son))
        {
            blocked.Add(RelationType.SonOfSon);
            blocked.Add(RelationType.DaughterOfSon);
        }

        if (!inheritanceCase.HasHeir(RelationType.Son) && inheritanceCase.GetHeirCount(RelationType.Daughter) >= 2)
            blocked.Add(RelationType.DaughterOfSon);

        if (!inheritanceCase.HasHeir(RelationType.Son) && inheritanceCase.HasHeir(RelationType.DaughterOfSon))
        {
            blocked.Add(RelationType.UterineBrother);
            blocked.Add(RelationType.UterineSister);
        }

        if (inheritanceCase.DeceasedHasMaleDescendantsOrAscendants() || inheritanceCase.HasHeir(RelationType.FullBrother))
        {
            blocked.Add(RelationType.ConsanguineBrother);
            blocked.Add(RelationType.ConsanguineSister);
        }

        if (inheritanceCase.GetHeirCount(RelationType.FullSister) > 2)
            blocked.Add(RelationType.ConsanguineSister);

        if (inheritanceCase.DeceasedHasMaleDescendantsOrAscendants())
        {
            blocked.Add(RelationType.FullBrother);
            blocked.Add(RelationType.FullSister);
        }

        if (inheritanceCase.DeceasedHasDescendants() || inheritanceCase.DeceasedHasMaleDescendantsOrAscendants() /* father or grand father*/)
        {
            blocked.Add(RelationType.UterineBrother);
            blocked.Add(RelationType.UterineSister);
        }

        if (inheritanceCase.DeceasedHasGrandChildren() && inheritanceCase.HasHeir(RelationType.Father))
        {
            blocked.Add(RelationType.Grandfather);
            blocked.Add(RelationType.GrandmotherMaternal);
            blocked.Add(RelationType.GrandmotherPaternal);
        }

        foreach (var b in blocked.ToArray())
            if (!inheritanceCase.HasHeir(b)) blocked.Remove(b);

        return blocked.ToList();
    }

    public bool IsBlocked(RelationType heirType, InheritanceCase inheritanceCase) =>
        GetBlockedHeirs(inheritanceCase).Contains(heirType);
}
