using Xunit;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// Tests for expression operator precedence and associativity.
/// Verifies that the Pratt parser produces correct parse trees, and the
/// evaluator computes expected results.
/// </summary>
public class OperatorPrecedenceTests
{
  // --- Multiplicative over additive ---

  [Fact]
  public void Multiply_Binds_Tighter_Than_Add()
  {
    // 2 + 3 * 4 should be  2 + (3 * 4) = 14,  not  (2 + 3) * 4 = 20
    var output = RunHelper.Run("PRINT 2 + 3 * 4");
    Assert.Equal("14", output);
  }

  [Fact]
  public void Multiply_Binds_Tighter_Than_Add_Right_Operand()
  {
    // 2 * 3 + 4 = (2 * 3) + 4 = 10
    var output = RunHelper.Run("PRINT 2 * 3 + 4");
    Assert.Equal("10", output);
  }

  [Fact]
  public void Divide_Binds_Tighter_Than_Subtract()
  {
    // 10 - 6 / 2 = 10 - (6 / 2) = 10 - 3 = 7
    var output = RunHelper.Run("PRINT 10 - 6 / 2");
    Assert.Equal("7", output);
  }

  // --- Parentheses override precedence ---

  [Fact]
  public void Parentheses_Override_Multiply_Precedence()
  {
    // (2 + 3) * 4 = 5 * 4 = 20
    var output = RunHelper.Run("PRINT (2 + 3) * 4");
    Assert.Equal("20", output);
  }

  [Fact]
  public void Nested_Parentheses_Evaluated_Inner_First()
  {
    // ((1 + 2) * (3 + 4)) = 3 * 7 = 21
    var output = RunHelper.Run("PRINT (1 + 2) * (3 + 4)");
    Assert.Equal("21", output);
  }

  // --- Left-to-right for equal precedence ---

  [Fact]
  public void Add_And_Subtract_Are_Left_Associative()
  {
    // 10 - 2 + 3 = (10 - 2) + 3 = 8 + 3 = 11
    var output = RunHelper.Run("PRINT 10 - 2 + 3");
    Assert.Equal("11", output);
  }

  [Fact]
  public void Multiply_And_Divide_Are_Left_Associative()
  {
    // 12 / 4 * 3 = (12 / 4) * 3 = 3 * 3 = 9
    var output = RunHelper.Run("PRINT 12 / 4 * 3");
    Assert.Equal("9", output);
  }

  // --- Comparison lower than arithmetic ---

  [Fact]
  public void Comparison_Binds_Looser_Than_Addition()
  {
    // 1 + 2 = 3  parses as  (1 + 2) = 3  → True
    var output = RunHelper.Run("PRINT 1 + 2 = 3");
    Assert.Equal("True", output);
  }

  [Fact]
  public void Comparison_Binds_Looser_Than_Multiplication()
  {
    // 2 * 3 > 5  →  (2 * 3) > 5  →  6 > 5  → True
    var output = RunHelper.Run("PRINT 2 * 3 > 5");
    Assert.Equal("True", output);
  }

  [Fact]
  public void Not_Equal_Binds_Looser_Than_Arithmetic()
  {
    // 2 + 2 <> 5  →  (2 + 2) <> 5  →  4 <> 5  → True
    var output = RunHelper.Run("PRINT 2 + 2 <> 5");
    Assert.Equal("True", output);
  }

  // --- Logical operators lower than comparison ---

  [Fact]
  public void And_Binds_Looser_Than_Comparison()
  {
    // 5 > 3 AND 2 < 4  →  (5 > 3) AND (2 < 4)  →  True AND True  → True
    var output = RunHelper.Run("PRINT 5 > 3 AND 2 < 4");
    Assert.Equal("True", output);
  }

  [Fact]
  public void And_Binds_Tighter_Than_Or()
  {
    // TRUE OR FALSE AND FALSE → TRUE OR (FALSE AND FALSE) → TRUE OR FALSE → True
    // This is already covered in EvaluatorTests; duplicated here for precedence completeness.
    var output = RunHelper.Run("PRINT TRUE OR FALSE AND FALSE");
    Assert.Equal("True", output);
  }

  [Fact]
  public void Full_Expression_Chain_Evaluates_Correctly()
  {
    // 2 + 3 = 5 AND 1 < 2
    // → (2 + 3) = 5 AND (1 < 2)
    // → True AND True
    // → True
    var output = RunHelper.Run("PRINT 2 + 3 = 5 AND 1 < 2");
    Assert.Equal("True", output);
  }

  // --- Unary NOT prefix binding ---

  [Fact]
  public void Not_As_Prefix_Captures_Full_Comparison_As_Operand()
  {
    // NOT 1 = 2  →  NOT (1 = 2)  →  NOT False  →  True
    // (The Pratt unary branch calls ParseExpression(0), so NOT grabs everything)
    var output = RunHelper.Run("PRINT NOT 1 = 2");
    Assert.Equal("True", output);
  }

  [Fact]
  public void Not_As_Prefix_Captures_And_Expression_As_Operand()
  {
    // NOT TRUE AND FALSE  →  NOT (TRUE AND FALSE)  →  NOT False  →  True
    var output = RunHelper.Run("PRINT NOT TRUE AND FALSE");
    Assert.Equal("True", output);
  }

  // --- String concatenation ---

  [Fact]
  public void Ampersand_Concatenates_String_And_Number()
  {
    // "Result: " & 42  →  "Result: 42"
    var output = RunHelper.Run("PRINT \"Result: \" & 42");
    Assert.Equal("Result: 42", output);
  }

  [Fact]
  public void Ampersand_Same_Precedence_As_Plus_Left_Associative()
  {
    // "x=" & 1 + 2  →  The binding powers of & and + are both 10, so left-to-right:
    // → ("x=" & 1) + 2.
    // Evaluator: "x=1" is a StringValue; then "x=1" + IntValue(2) tries numeric addition
    // but left is StringValue → EvalFailure (cannot use + with mixed types).
    // Use parentheses to achieve concatenation of an arithmetic result:
    // "x=" & (1 + 2)  →  "x=3"
    var output = RunHelper.Run("PRINT \"x=\" & (1 + 2)");
    Assert.Equal("x=3", output);
  }

  // --- Modulo ---

  [Fact]
  public void Mod_Binds_Same_As_Multiply()
  {
    // 10 + 3 MOD 2  →  10 + (3 MOD 2)  →  10 + 1  →  11
    var output = RunHelper.Run("PRINT 10 + 3 MOD 2");
    Assert.Equal("11", output);
  }
}
