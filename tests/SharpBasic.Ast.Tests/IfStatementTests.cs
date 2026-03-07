using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class IfStatementTests
{
  private readonly SourceLocation loc = new(1, 1);

  [Fact]
  public void IfStatements_With_Identical_Value_Are_Equal()
  {
    var condition = new BinaryExpression(
      new IntLiteralExpression(1, loc),
      new Token(TokenType.Eq, "", loc.Line, loc.Col),
      new IntLiteralExpression(1, loc),
      loc
    );

    var then = new List<Statement>
    {
      new PrintStatement(
        new StringLiteralExpression("Hello, World!", loc),
        loc
      )
    };

    var stmt1 = new IfStatement(condition, then, null, loc);
    var stmt2 = new IfStatement(condition, then, null, loc);

    Assert.Equal(stmt1, stmt2);
  }

  [Fact]
  public void LetStatements_With_Same_Value_Are_Equal()
  {
    var condition1 = new BinaryExpression(
      new IntLiteralExpression(1, loc),
      new Token(TokenType.Eq, "", loc.Line, loc.Col),
      new IntLiteralExpression(1, loc),
      loc
    );

    var then1 = new List<Statement>
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

    var then2 = new List<Statement>
    {
      new PrintStatement(
        new StringLiteralExpression("Hello, World!", loc),
        loc
      )
    };

    var stmt1 = new IfStatement(condition1, then1, null, loc);
    var stmt2 = new IfStatement(condition2, then2, null, loc);

    Assert.Equal(stmt1.Condition, stmt2.Condition);
    Assert.Equal(stmt1.ThenBlock.Count, stmt2.ThenBlock.Count);
    Assert.Equal(stmt1.Location, stmt2.Location);
  }

  [Fact]
  public void LetStatements_With_Different_Value_Are_Not_Equal()
  {
    var condition1 = new BinaryExpression(
      new IntLiteralExpression(1, loc),
      new Token(TokenType.Eq, "", loc.Line, loc.Col),
      new IntLiteralExpression(1, loc),
      loc
    );

    var then1 = new List<Statement>
    {
      new PrintStatement(
        new StringLiteralExpression("Well Now, what a pickle we are in!", loc),
        loc
      )
    };

    var condition2 = new BinaryExpression(
      new IntLiteralExpression(1, loc),
      new Token(TokenType.Eq, "", loc.Line, loc.Col),
      new IntLiteralExpression(1, loc),
      loc
    );

    var then2 = new List<Statement>
    {
      new PrintStatement(
        new StringLiteralExpression("Hello, World!", loc),
        loc
      )
    };

    var else2 = new List<Statement>
    {
      new PrintStatement(
        new StringLiteralExpression("Goodbye", loc),
        loc
      )
    };


    var stmt1 = new IfStatement(condition1, then1, null, loc);
    var stmt2 = new IfStatement(condition2, then2, null, loc);
    var stmt3 = new IfStatement(condition2, then2, else2, loc);

    Assert.NotEqual(stmt1, stmt2);
    Assert.NotEqual(stmt1, stmt3);
    Assert.NotEqual(stmt2, stmt3);
  }
}