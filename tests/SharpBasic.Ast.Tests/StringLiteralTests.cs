using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class StringLiteralTests
{
    [Fact]
    public void StringLiteral_Holds_Value()
    {
        var exp1 = new StringLiteralExpression("Hello, World!", new SourceLocation(1, 1));

        Assert.Equal("Hello, World!", exp1.Value);
    }

}