namespace Uqs.Weather.Wrappers;

/// <summary>
/// A wrapper for DateTime.Now to impelement dependency injection.
/// </summary>
public interface INowWrapper
{
    /// <summary>
    /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer, expressed
    /// as the local time.
    /// </summary>
    /// <value>An object whose value is the current local date and time.</value>
    DateTime Now { get; }
}
