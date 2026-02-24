using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class PrintStatementTests
{
    [Fact]
    public void PrintStatements_With_Same_Value_Are_Equal()
    {
        var expression = new StringLiteralExpression("Hello, World!");

        var stmt1 = new PrintStatementTests(expression); 
        var stmt2 = new PrintStatementTests(expression); 

        Assert.Equal(stmt1,stmt2);
    } 
}