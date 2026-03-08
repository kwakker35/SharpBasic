using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class WhileStatementTests
{
    private readonly SourceLocation loc = new(1, 1);

    [Fact]
    public void WhileStatements_With_Identical_Value_Are_Equal()
    {
        var condition = new BinaryExpression(
          new IntLiteralExpression(1, loc),
          new Token(TokenType.Eq, "", loc.Line, loc.Col),
          new IntLiteralExpression(1, loc),
          loc
        );

        var body = new List<Statement>
        {
            new PrintStatement(
                new StringLiteralExpression("Hello, World!", loc),
                loc
            )
        };

        var stmt1 = new WhileStatement(condition, body, loc);
        var stmt2 = new WhileStatement(condition, body, loc);

        Assert.Equal(stmt1, stmt2);
    }

    [Fact]
    public void WhileStatements_With_Same_Value_Are_Equal()
    {
        var condition1 = new BinaryExpression(
          new IntLiteralExpression(1, loc),
          new Token(TokenType.Eq, "", loc.Line, loc.Col),
          new IntLiteralExpression(1, loc),
          loc
        );

        var body1 = new List<Statement>
        {
            new PrintStatement(
                new StringLiteralExpression("Hello, World!", loc),
                loc
            )
        };

        var condition2 = new BinaryExpression(
          new IntLiteralExpression(1, loc),
          new Token(TokenType.Eq, "", loc.Line, loc.Col),
          new IntLiteralExpression(1, loc),
          loc
        );

        var body2 = new List<Statement>
        {
            new PrintStatement(
                new StringLiteralExpression("Hello, World!", loc),
                loc
            )
        };

        var stmt1 = new WhileStatement(condition1, body1, loc);
        var stmt2 = new WhileStatement(condition2, body2, loc);

        Assert.Equal(stmt1.Condition, stmt2.Condition);
        Assert.Equal(stmt1.Body.Count, stmt2.Body.Count);
        Assert.Equal(stmt1.Location, stmt2.Location);
    }

    [Fact]
    public void WhileStatements_With_Different_Value_Are_Not_Equal()
    {
        var condition1 = new BinaryExpression(
          new IntLiteralExpression(1, loc),
          new Token(TokenType.Eq, "", loc.Line, loc.Col),
          new IntLiteralExpression(1, loc),
          loc
        );

        var body1 = new List<Statement>
        {
            new PrintStatement(
                new StringLiteralExpression("Well Now, what a pickle we are in!", loc),
                loc
            )
        };

        var condition2 = new BinaryExpression(
          new IdentifierExpression("X", loc),
          new Token(TokenType.Lt, "", loc.Line, loc.Col),
          new IntLiteralExpression(5, loc),
          loc
        );

        var body2 = new List<Statement>
        {
            new PrintStatement(
                new StringLiteralExpression("Hello, World!", loc),
                loc
            ),
            new PrintStatement(
                new StringLiteralExpression("Hello, World - Again!", loc),
                loc
            )
        };


        var stmt1 = new WhileStatement(condition1, body1, loc);
        var stmt2 = new WhileStatement(condition2, body2, loc);
        var stmt3 = new WhileStatement(condition1, body2, loc);

        Assert.NotEqual(stmt1.Condition, stmt2.Condition);
        Assert.NotEqual(stmt2.Condition, stmt3.Condition);
    }
}