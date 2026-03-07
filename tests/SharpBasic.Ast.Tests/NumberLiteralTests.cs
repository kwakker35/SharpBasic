using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class NumberLiteralTests
{
    [Fact]
    public void IntLiteral_Holds_Value()
    {
        var exp1 = new IntLiteralExpression(42, new SourceLocation(1, 1));

        Assert.Equal(42, exp1.Value);
    }

    [Fact]
    public void FloatLiteral_Holds_Value()
    {
        var exp1 = new FloatLiteralExpression(3.14, new SourceLocation(1, 1));

        Assert.Equal(3.14, exp1.Value);
    }

}