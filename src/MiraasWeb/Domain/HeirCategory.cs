namespace MiraasWeb.Domain;

/// <summary>
/// Classifies heirs based on their share allocation method in Islamic inheritance law.
/// 
/// FixedShare (Ashab al-Furudh): Receive prescribed fractions from the Qur'an.
/// Residuary (Asabah): Receive remainder after fixed shares are allocated.
/// Both: Can be either fixed share or residuary depending on other heirs present.
/// Blocked (Mahjoob): Completely blocked by stronger heirs.
/// </summary>
public enum HeirCategory
{
    FixedShare,
    Residuary,
    Both,
    Blocked
}
