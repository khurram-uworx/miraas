namespace MiraasWeb.Domain;

/// <summary>
/// Represents the deceased person whose estate is being divided.
/// </summary>
public class DeceasedPerson
{
    /// <summary>
    /// The gender of the deceased (affects certain inheritance rules).
    /// </summary>
    public Gender Gender { get; set; }

    public DeceasedPerson(Gender gender = Gender.Male)
    {
        Gender = gender;
    }
}
