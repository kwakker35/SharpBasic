using SharpBasic.Ast;
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

  // --- SUB/FUNCTION body failure propagation ---

  [Fact]
  public void Evaluator_Sub_Body_Failure_Propagates_To_Caller()
  {
    var code = """
      SUB BadSub()
        PRINT 10 / 0
      END SUB
      CALL BadSub()
      """;
    var result = RunHelper.RunResult(code);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Function_Body_Failure_Propagates_To_Caller()
  {
    var code = """
      FUNCTION BadFunc() AS INTEGER
        RETURN 10 / 0
      END FUNCTION
      PRINT BadFunc()
      """;
    var result = RunHelper.RunResult(code);
    Assert.IsType<EvalFailure>(result);
  }

  // --- CONST reassignment errors ---

  [Fact]
  public void Evaluator_Let_Reassigns_Const_Returns_EvalFailure()
  {
    // CONST X = 5 then LET X = 10 must be a runtime error
    var result = RunHelper.RunResult("CONST X = 5\nLET X = 10");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Let_Reassigns_String_Const_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("CONST GREETING = \"hello\"\nLET GREETING = \"world\"");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Let_Reassigns_Float_Const_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("CONST PI = 3.14\nLET PI = 3.0");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Let_Reassigns_Bool_Const_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("CONST FLAG = TRUE\nLET FLAG = FALSE");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Const_Error_Message_Names_The_Constant()
  {
    var result = RunHelper.RunResult("CONST MAX = 10\nLET MAX = 99");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("MAX", StringComparison.OrdinalIgnoreCase));
  }

  [Fact]
  public void Evaluator_Const_Redefinition_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("CONST X = 10\nCONST X = 5");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Evaluator_Const_Redefinition_Error_Message_Names_The_Constant()
  {
    var result = RunHelper.RunResult("CONST LIMIT = 100\nCONST LIMIT = 200");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("LIMIT", StringComparison.OrdinalIgnoreCase));
  }

  // --- Argument count mismatch ---

  [Fact]
  public void Evaluator_Call_Sub_Too_Few_Args_Returns_EvalFailure()
  {
    var source = """
            SUB Add(a AS INTEGER, b AS INTEGER)
              PRINT a + b
            END SUB
            CALL Add(1)
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("Add") && d.Message.Contains("2") && d.Message.Contains("1"));
  }

  [Fact]
  public void Evaluator_Call_Sub_Too_Many_Args_Returns_EvalFailure()
  {
    var source = """
            SUB Greet(name AS STRING)
              PRINT "Hello " & name
            END SUB
            CALL Greet("Alice", "extra")
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("Greet") && d.Message.Contains("1") && d.Message.Contains("2"));
  }

  [Fact]
  public void Evaluator_Call_Function_Too_Few_Args_Returns_EvalFailure()
  {
    var source = """
            FUNCTION Multiply(a AS INTEGER, b AS INTEGER) AS INTEGER
              RETURN a * b
            END FUNCTION
            PRINT Multiply(5)
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("Multiply") && d.Message.Contains("2") && d.Message.Contains("1"));
  }

  [Fact]
  public void Evaluator_Call_Function_Too_Many_Args_Returns_EvalFailure()
  {
    var source = """
            FUNCTION Double(n AS INTEGER) AS INTEGER
              RETURN n * 2
            END FUNCTION
            PRINT Double(3, 99)
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("Double") && d.Message.Contains("1") && d.Message.Contains("2"));
  }

  [Fact]
  public void Evaluator_Call_Sub_No_Params_With_Args_Returns_EvalFailure()
  {
    var source = """
            SUB NoArgs()
              PRINT "hi"
            END SUB
            CALL NoArgs(1)
            """;
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("NoArgs") && d.Message.Contains("0") && d.Message.Contains("1"));
  }
}
