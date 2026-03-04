using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class LetStatementTests
{
  [Fact]
  public void LetStatements_With_Identical_Value_Are_Equal()
  {
    var token = new Token(TokenType.Identifier, "X", 1, 1);
    var expression = new IdentifierExpression("Hello, World!", new SourceLocation(1, 1));

    var stmt1 = new LetStatement(token, expression, new SourceLocation(1, 1));
    var stmt2 = new LetStatement(token, expression, new SourceLocation(1, 1));

    Assert.Equal(stmt1, stmt2);
  }

  [Fact]
  public void LetStatements_With_Same_Value_Are_Equal()
  {
    var token1 = new Token(TokenType.Identifier, "X", 1, 1);
    var expression1 = new IdentifierExpression("Hello, World!", new SourceLocation(1, 1));
    var token2 = new Token(TokenType.Identifier, "X", 1, 1);
    var expression2 = new IdentifierExpression("Hello, World!", new SourceLocation(1, 1));

    var stmt1 = new LetStatement(token1, expression1, new SourceLocation(1, 1));
    var stmt2 = new LetStatement(token2, expression2, new SourceLocation(1, 1));

    Assert.Equal(stmt1, stmt2);
  }

  [Fact]
  public void LetStatements_With_Different_Value_Are_Not_Equal()
  {
    var token1 = new Token(TokenType.Identifier, "X", 1, 1);
    var expression1 = new IdentifierExpression("Hello, World!", new SourceLocation(1, 1));
    var token2 = new Token(TokenType.Identifier, "Y", 1, 1);
    var expression2 = new IdentifierExpression("Hello, World!", new SourceLocation(1, 1));
    var token3 = new Token(TokenType.Identifier, "X", 1, 1);
    var expression3 = new IdentifierExpression("Goodbye!", new SourceLocation(1, 1));

    var stmt1 = new LetStatement(token1, expression1, new SourceLocation(1, 1));
    var stmt2 = new LetStatement(token2, expression2, new SourceLocation(1, 1));
    var stmt3 = new LetStatement(token3, expression3, new SourceLocation(1, 1));

    Assert.NotEqual(stmt1, stmt2);
    Assert.NotEqual(stmt1, stmt3);
    Assert.NotEqual(stmt2, stmt3);
  }
}