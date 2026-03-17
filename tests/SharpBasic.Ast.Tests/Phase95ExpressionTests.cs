using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class Phase95ExpressionTests
{
  [Fact]
  public void BoolLiteralExpression_Holds_True_Value()
  {
    var expr = new BoolLiteralExpression(true, new SourceLocation(1, 1));
    Assert.True(expr.Value);
  }

  [Fact]
  public void BoolLiteralExpression_Holds_False_Value()
  {
    var expr = new BoolLiteralExpression(false, new SourceLocation(1, 1));
    Assert.False(expr.Value);
  }

  [Fact]
  public void UnaryExpression_Holds_Not_Operator_And_Operand()
  {
    var op = new Token(TokenType.Not, "NOT", 1, 1);
    var operand = new BoolLiteralExpression(true, new SourceLocation(1, 1));
    var expr = new UnaryExpression(op, operand, new SourceLocation(1, 1));

    Assert.Equal(TokenType.Not, expr.Operator.Type);
    Assert.Equal(operand, expr.Operand);
  }

  [Fact]
  public void UnaryExpression_Holds_Minus_Operator_And_Operand()
  {
    var op = new Token(TokenType.Minus, "-", 1, 1);
    var operand = new IdentifierExpression("x", new SourceLocation(1, 1));
    var expr = new UnaryExpression(op, operand, new SourceLocation(1, 1));

    Assert.Equal(TokenType.Minus, expr.Operator.Type);
    Assert.Equal(operand, expr.Operand);
  }
}
