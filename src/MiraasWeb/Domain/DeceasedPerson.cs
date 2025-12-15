namespace MiraasWeb.Domain;

public enum GenderType { Male, Female }

// public because of tests
public class DeceasedPerson
{
    public GenderType Gender { get; set; }

    public DeceasedPerson(GenderType gender = GenderType.Male)
    {
        Gender = gender;
    }

    public override string ToString() =>
        $"Deceased Person Gender: {Gender}";
}
