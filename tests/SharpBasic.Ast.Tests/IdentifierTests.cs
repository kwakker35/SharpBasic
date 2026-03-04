using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class IdentifierTests
{
    [Fact]
    public void IdentifierExpression_Holds_Value()
    {
        var exp1 = new IdentifierExpression("X", new SourceLocation(1, 1));

        Assert.Equal("X", exp1.Name);
    }

}