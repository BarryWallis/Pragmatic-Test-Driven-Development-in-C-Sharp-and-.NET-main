namespace Uqs.Weather.Wrappers;

/// <summary>
/// A wrapper for DateTime.Now to impelement dependency injection.
/// </summary>
public class NowWrapper : INowWrapper
{
    /// <inheritdoc/>
    public DateTime Now => DateTime.Now;
}
