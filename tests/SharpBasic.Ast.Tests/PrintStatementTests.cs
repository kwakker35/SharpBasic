using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class PrintStatementTests
{
    [Fact]
    public void PrintStatements_With_Identical_Value_Are_Equal()
    {
        var expression = new StringLiteralExpression("Hello, World!", new SourceLocation(1, 1));

        var stmt1 = new PrintStatement(expression, new SourceLocation(1, 1));
        var stmt2 = new PrintStatement(expression, new SourceLocation(1, 1));

        Assert.Equal(stmt1, stmt2);
    }

    [Fact]
    public void PrintStatements_With_Same_Value_Are_Equal()
    {
        var exp1 = new StringLiteralExpression("Hello, World!", new SourceLocation(1, 1));
        var exp2 = new StringLiteralExpression("Hello, World!", new SourceLocation(1, 1));

        var stmt1 = new PrintStatement(exp1, new SourceLocation(1, 1));
        var stmt2 = new PrintStatement(exp2, new SourceLocation(1, 1));

        Assert.Equal(stmt1, stmt2);
    }

    [Fact]
    public void PrintStatements_With_Different_Value_Are_Not_Equal()
    {
        var exp1 = new StringLiteralExpression("Hello, World!", new SourceLocation(1, 1));
        var exp2 = new StringLiteralExpression("Good Bye", new SourceLocation(1, 1));

        var stmt1 = new PrintStatement(exp1, new SourceLocation(1, 1));
        var stmt2 = new PrintStatement(exp2, new SourceLocation(1, 1));

        Assert.NotEqual(stmt1, stmt2);
    }
}