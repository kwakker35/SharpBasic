using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;


public class BinaryExpressionTests()
{
  [Fact]
  public void BinararyExpressions_With_Identical_Value_Are_Equal()
  {
    var left = new IntLiteralExpression(30, new SourceLocation(1, 1));
    var op = new Token(TokenType.Plus, "", 1, 1);
    var right = new IntLiteralExpression(12, new SourceLocation(1, 1));

    var stmt1 = new BinaryExpression(left, op, right, new SourceLocation(1, 1));
    var stmt2 = new BinaryExpression(left, op, right, new SourceLocation(1, 1));

    Assert.Equal(stmt1, stmt2);
  }

  [Fact]
  public void BinararyExpressions_With_Same_Value_Are_Equal()
  {
    var left1 = new IntLiteralExpression(30, new SourceLocation(1, 1));
    var op1 = new Token(TokenType.Plus, "", 1, 1);
    var right1 = new IntLiteralExpression(12, new SourceLocation(1, 1));

    var left2 = new IntLiteralExpression(30, new SourceLocation(1, 1));
    var op2 = new Token(TokenType.Plus, "", 1, 1);
    var right2 = new IntLiteralExpression(12, new SourceLocation(1, 1));

    var stmt1 = new BinaryExpression(left1, op1, right1, new SourceLocation(1, 1));
    var stmt2 = new BinaryExpression(left2, op2, right2, new SourceLocation(1, 1));

    Assert.Equal(stmt1, stmt2);
  }

  [Fact]
  public void BinararyExpressions_With_Different_Value_Are_Not_Equal()
  {
    var left1 = new IntLiteralExpression(30, new SourceLocation(1, 1));
    var op1 = new Token(TokenType.Plus, "", 1, 1);
    var right1 = new IntLiteralExpression(12, new SourceLocation(1, 1));

    var left2 = new IntLiteralExpression(60, new SourceLocation(1, 1));
    var op2 = new Token(TokenType.Plus, "", 1, 1);
    var right2 = new IntLiteralExpression(9, new SourceLocation(1, 1));

    var stmt1 = new BinaryExpression(left1, op1, right1, new SourceLocation(1, 1));
    var stmt2 = new BinaryExpression(left2, op2, right2, new SourceLocation(1, 1));

    Assert.NotEqual(stmt1, stmt2);
  }
}