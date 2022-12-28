namespace Uqs.Arithmetic;

/// <summary>
/// Class to process divisions.
/// </summary>
public class Division
{
    /// <summary>
    /// Divide the <paramref name="dividend"/> by the <paramref name="divisor"/> and return the quotient.
    /// </summary>
    /// <param name="dividend">The dividend.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The quotient.</returns>
    public static decimal Divide(int dividend, int divisor)
    {
        decimal quotient = (decimal)dividend / divisor;
        return quotient;
    }
}
