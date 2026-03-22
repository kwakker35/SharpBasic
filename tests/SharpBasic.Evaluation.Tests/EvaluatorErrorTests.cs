using Xunit;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// Tests for evaluator runtime error paths: constructs that parse successfully
/// but must produce EvalFailure at runtime.
/// </summary>
public class EvaluatorErrorTests
{
  // --- Arithmetic errors ---

  [Fact]
  public void Evaluator_Divide_By_Zero_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT 10 / 0");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("divide by zero"));
  }

  [Fact]
  public void Evaluator_Divide_By_Zero_Via_Variable_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("LET d = 0\nPRINT 10 / d");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("divide by zero"));
  }

  // --- String operator errors ---

  [Fact]
  public void Evaluator_Plus_On_Two_Strings_Returns_EvalFailure()
  {
    // & is the string concat operator; + between two strings is not allowed
    var result = RunHelper.RunResult("PRINT \"hello\" + \"world\"");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Minus_On_Two_Strings_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("LET a = \"x\"\nLET b = \"y\"\nPRINT a - b");
    Assert.IsType<EvalFailure>(result);
  }

  // --- Unary operator errors ---

  [Fact]
  public void Evaluator_Unary_Minus_On_String_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("LET s = \"hello\"\nPRINT -s");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Unary_Not_On_Integer_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("LET n = 5\nPRINT NOT n");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Unary_Minus_On_Bool_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT -TRUE");
    Assert.IsType<EvalFailure>(result);
  }

  // --- Control flow condition errors ---

  [Fact]
  public void Evaluator_If_With_Integer_Condition_Returns_EvalFailure()
  {
    // IF condition requires a boolean; integer is not accepted
    var source = "IF 42 THEN\nPRINT \"yes\"\nEND IF";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("boolean"));
  }

  [Fact]
  public void Evaluator_While_With_Integer_Condition_Returns_EvalFailure()
  {
    var source = "WHILE 1\nLET x = 1\nWEND";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("boolean"));
  }

  // --- Undefined sub / function ---

  [Fact]
  public void Evaluator_Call_Undefined_Sub_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("CALL Missing()");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("Missing") && d.Message.Contains("not found"));
  }

  [Fact]
  public void Evaluator_Call_Undefined_Function_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("LET x = Missing()");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("Missing") && d.Message.Contains("not found"));
  }

  // --- Function missing RETURN ---

  [Fact]
  public void Evaluator_Function_Missing_Return_Returns_EvalFailure()
  {
    var source = """
            LET x = NoReturn()
            FUNCTION NoReturn() As Integer
            LET y = 1
            END FUNCTION
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("RETURN") || d.Message.Contains("return"));
  }

  // --- DIM re-declaration ---

  [Fact]
  public void Evaluator_Dim_Redeclaration_Returns_EvalFailure()
  {
    var source = "DIM arr[3] As Integer\nDIM arr[3] As Integer";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("already exists") || d.Message.Contains("redefin"));
  }

  // --- Arrays ---

  [Fact]
  public void Evaluator_Array_Access_On_Non_Array_Variable_Returns_EvalFailure()
  {
    var source = "LET x = 5\nPRINT x[0]";
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Negative_Array_Index_Returns_EvalFailure()
  {
    var source = "DIM arr[3] As Integer\nPRINT arr[-1]";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("outside of the range"));
  }

  [Fact]
  public void Evaluator_Float_Array_Receives_Integer_Returns_EvalFailure()
  {
    // Float array should not accept an Integer value
    var source = "DIM fArr[2] As Float\nLET fArr[0] = 99";
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  // --- Built-in / user name collision ---

  [Fact]
  public void Evaluator_Sub_Named_Same_As_Builtin_Returns_EvalFailure()
  {
    // Hoisting must reject a SUB named after a built-in function
    var source = "CALL LEN(\"test\")\nSUB LEN()\nEND SUB";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("LEN") && d.Message.Contains("built in"));
  }

  [Fact]
  public void Evaluator_Function_Named_Same_As_Builtin_Returns_EvalFailure()
  {
    var source = """
            LET x = LEN("test")
            FUNCTION LEN(s As String) As Integer
            RETURN 42
            END FUNCTION
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("LEN") && d.Message.Contains("built in"));
  }

  // --- Unknown identifier ---

  [Fact]
  public void Evaluator_Unknown_Identifier_In_Expression_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("LET x = undefined + 1");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("undefined") || d.Message.Contains("Unknown Identifier"));
  }
}
