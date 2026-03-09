namespace LiveSplit.SonicFrontiers;

/// <summary>
/// Custom extension methods used in this autosplitter
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Checks is a specific bit inside a byte value is set or not
    /// </summary>
    /// <param name="value">The byte value in which to perform the check</param>
    /// <param name="bitPos">The bit position. Can range from 0 to 7: any value outside this range will make the function automatically return false.</param>
    /// <returns></returns>
    public static bool BitCheck(this byte value, byte bitPos)
    {
        return bitPos >= 0 && bitPos <= 7 && (value & (1 << bitPos)) != 0;
    }
}