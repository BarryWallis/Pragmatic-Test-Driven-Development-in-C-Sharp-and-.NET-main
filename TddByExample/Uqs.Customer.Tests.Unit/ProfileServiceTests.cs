namespace Uqs.Customer.Tests.Unit;

public class ProfileServiceTests
{
    [Fact]
    public void ChangeUsername_NullUsername_ArgumentNullException()
    {
        // Arrange
        ProfileService sut = new();

        // Act
        Exception e = Record.Exception(() => sut.ChangeUsername(null!));

        // Assert
        ArgumentNullException ex = Assert.IsType<ArgumentNullException>(e);
        Assert.Equal("username", ex.ParamName);
    }

    [Theory]
    [InlineData("AnameOf8", true)]
    [InlineData("NameOfChar12", true)]
    [InlineData("AnameofChar13", false)]
    [InlineData("NameOf7", false)]
    [InlineData("", false)]
    public void ChangeUsername_VariousLengthUsernames_ArgumentExceptionIfInvalid(string username, bool isValid)
    {
        // Arrange
        ProfileService sut = new();

        // Act
        Exception e = Record.Exception(() => sut.ChangeUsername(username));

        // Assert
        if (isValid)
        {
            Assert.Null(e);
        }
        else
        {
            ArgumentException ex = Assert.IsType<ArgumentException>(e);
            Assert.Equal("username", ex.ParamName);
        }
    }

    [Theory]
    [InlineData("Letter_123", true)]
    [InlineData("!The_Start", false)]
    [InlineData("InThe@Middle", false)]
    [InlineData("WithDollar$", false)]
    [InlineData("Space 123", false)]
    public void ChangeUsername_InvalidCharValidation_ArgumentExceptionIfInvalid(string username, bool isValid)
    {
        // Arrange
        ProfileService sut = new();

        // Act
        Exception e = Record.Exception(() => sut.ChangeUsername(username));

        // Assert
        if (isValid)
        {
            Assert.Null(e);
        }
        else
        {
            ArgumentException ex = Assert.IsType<ArgumentException>(e);
            Assert.Equal("username", ex.ParamName);
        }
    }
}
