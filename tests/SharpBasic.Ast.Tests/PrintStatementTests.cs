using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class PrintStatementTests
{
    [Fact]
    public void PrintStatements_With_Identical_Value_Are_Equal()
    {
        var expression = new StringLiteralExpression("Hello, World!");

        var stmt1 = new PrintStatement(expression);
        var stmt2 = new PrintStatement(expression);

        Assert.Equal(stmt1, stmt2);
    }

    [Fact]
    public void PrintStatements_With_Same_Value_Are_Equal()
    {
        var exp1 = new StringLiteralExpression("Hello, World!");
        var exp2 = new StringLiteralExpression("Hello, World!");

        var stmt1 = new PrintStatement(exp1);
        var stmt2 = new PrintStatement(exp2);

        Assert.Equal(stmt1, stmt2);
    }

    [Fact]
    public void PrintStatements_With_Different_Value_Are_Not_Equal()
    {
        var exp1 = new StringLiteralExpression("Hello, World!");
        var exp2 = new StringLiteralExpression("Good Bye");

        var stmt1 = new PrintStatement(exp1);
        var stmt2 = new PrintStatement(exp2);

        Assert.NotEqual(stmt1, stmt2);
    }
}