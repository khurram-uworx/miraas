using System.Text;

namespace MiraasWeb.Domain;

// public because of tests
public class InheritanceCase
{
    List<Heir> heirs = new();

    public DeceasedPerson Deceased { get; }
    public IEnumerable<Heir> Heirs => heirs;
    public decimal? EstateValue { get; set; }

    public InheritanceCase(DeceasedPerson deceased)
    {
        Deceased = deceased ?? throw new ArgumentNullException(nameof(deceased));
    }

    public override string ToString()
    {
        var sb = new StringBuilder(this.Deceased.ToString());

        if ((this.EstateValue ?? 0) > 0)
        {
            sb.Append(Environment.NewLine);
            sb.Append($"Estate Value: {this.EstateValue}");
        }

        foreach(var heir in heirs)
        {
            sb.AppendLine(Environment.NewLine);
            sb.Append(heir.ToString());
        }

        return sb.ToString();
    }

    public void AddHeir(Heir heir)
    {
        if (heir == null)
            throw new ArgumentNullException(nameof(heir));

        heirs.Add(heir);
    }

    public void AddHeirs(params Heir[] heirs)
    {
        foreach (var heir in heirs)
            AddHeir(heir);
    }

    public int GetHeirCount(RelationType relationType) =>
        heirs
            .Where(h => h.Relation == relationType)
            .Sum(h => h.Count);

    public bool HasHeir(RelationType relationType) =>
        heirs.Any(h => h.Relation == relationType);

    public IEnumerable<Heir> GetHeirsByType(RelationType relationType) =>
        heirs.Where(h => h.Relation == relationType);
}
