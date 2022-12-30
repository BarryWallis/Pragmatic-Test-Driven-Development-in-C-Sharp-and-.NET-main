using CommunityToolkit.Diagnostics;

namespace Uqs.Customer;

/// <summary>
/// Provide services to manage user profiles.
/// </summary>
public class ProfileService
{
#pragma warning disable CA1822 // Mark members as static
    /// <summary>
    /// Change the user's name.
    /// </summary>
    /// <param name="username">The new user name.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="username"/> cannot be <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="username"/> must be between 8 and 12 characters inclusive.
    /// </exception>
    public void ChangeUsername(string username)
    {
        Guard.IsNotNull(username);
        Guard.HasSizeGreaterThanOrEqualTo(username, 8);
        Guard.HasSizeLessThanOrEqualTo(username, 12);
        Guard.IsTrue(username.All(c => char.IsLetterOrDigit(c) || c == '_'), nameof(username));
    }
#pragma warning restore CA1822 // Mark members as static
}
