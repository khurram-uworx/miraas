namespace MiraasWeb.Domain;

public enum Gender { Male, Female }

// public because of tests
public class DeceasedPerson
{
    public Gender Gender { get; set; }

    public DeceasedPerson(Gender gender = Gender.Male)
    {
        Gender = gender;
    }

    public override string ToString() =>
        $"Deceased Person Gender: {Gender}";
}
