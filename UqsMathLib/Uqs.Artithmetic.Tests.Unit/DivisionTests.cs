using Uqs.Arithmetic;

namespace Uqs.Artithmetic.Tests.Unit;

public class DivisionTests
{
    [Fact]
    public void Divide_DivisibleIntegers_WholeNumber()
    {
        int dividend = 10;
        int divisor = 5;
        decimal expectedQuotient = 2.00M;

        decimal actualQuotient = Division.Divide(dividend, divisor);

        Assert.Equal(expectedQuotient, actualQuotient);
    }

    [Fact]
    public void Divide_IndivisibleIntegers_DecimalNumber()
    {
        int dividend = 10;
        int divisor = 4;
        decimal expectedQuotient = 2.50M;

        decimal actualQuotient = Division.Divide(dividend, divisor);

        Assert.Equal(expectedQuotient, actualQuotient);
    }

    [Fact]
    public void Divide_ZeroDivisor_DivideByZeroException()
    {
        int dividend = 10;
        int divisor = 0;

        Exception exception = Record.Exception(() => Division.Divide(dividend, divisor));

        _ = Assert.IsType<DivideByZeroException>(exception);
    }

    [Theory]
    [InlineData(int.MaxValue, int.MinValue, -0.999999999534)]
    [InlineData(-int.MaxValue, int.MinValue, 0.999999999534)]
    [InlineData(int.MinValue, int.MaxValue, -1.000000000466)]
    [InlineData(int.MinValue, -int.MaxValue, 1.000000000466)]
    public void Divide_ExtremeInput_CorrectCalculation(int dividend, int divisor, decimal expectedQuotient)
    {

        decimal actualQuotient = Division.Divide(dividend, divisor);

        Assert.Equal(expectedQuotient, actualQuotient, 12);
    }
}
