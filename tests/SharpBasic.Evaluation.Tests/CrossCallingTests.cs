using Xunit;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// Tests for cross-calling between SUBs and FUNCTIONs — scenarios where one
/// subroutine type invokes another, verifying that the shared declaration tables
/// are threaded correctly through recursive evaluator instances.
/// </summary>
public class CrossCallingTests
{
  // --- SUB calling another SUB ---

  [Fact]
  public void Sub_Can_Call_Another_Sub()
  {
    var source = """
            CALL Outer()
            SUB Outer()
            CALL Inner()
            END SUB
            SUB Inner()
            PRINT "Inner"
            END SUB
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("Inner", output);
  }

  [Fact]
  public void Sub_Can_Call_Another_Sub_With_Arguments()
  {
    var source = """
            CALL PrintDouble(5)
            SUB PrintDouble(n As Integer)
            CALL Show(n + n)
            END SUB
            SUB Show(v As Integer)
            PRINT v
            END SUB
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("10", output);
  }

  // --- FUNCTION calling another FUNCTION ---

  [Fact]
  public void Function_Can_Call_Another_Function()
  {
    var source = """
            LET result = Double(3)
            PRINT result
            FUNCTION Double(n As Integer) As Integer
            RETURN Add(n, n)
            END FUNCTION
            FUNCTION Add(a As Integer, b As Integer) As Integer
            RETURN a + b
            END FUNCTION
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("6", output);
  }

  [Fact]
  public void Function_Can_Call_Another_Function_With_Multiple_Levels()
  {
    var source = """
            LET result = Triple(4)
            PRINT result
            FUNCTION Triple(n As Integer) As Integer
            RETURN Add(Double(n), n)
            END FUNCTION
            FUNCTION Double(n As Integer) As Integer
            RETURN n + n
            END FUNCTION
            FUNCTION Add(a As Integer, b As Integer) As Integer
            RETURN a + b
            END FUNCTION
            """;
    // Triple(4) = Add(Double(4), 4) = Add(8, 4) = 12
    var output = RunHelper.Run(source);
    Assert.Equal("12", output);
  }

  // --- SUB calling a FUNCTION ---

  [Fact]
  public void Sub_Can_Call_A_Function()
  {
    var source = """
            CALL Greet("World")
            SUB Greet(name As String)
            LET msg = Join("Hello", name)
            PRINT msg
            END SUB
            FUNCTION Join(a As String, b As String) As String
            RETURN a & " " & b
            END FUNCTION
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("Hello World", output);
  }

  // --- Declarations visible in both directions ---

  [Fact]
  public void Functions_Declared_After_Sub_Are_Still_Callable_From_Sub()
  {
    // Hoisting means all subs and functions are registered before any code runs.
    var source = """
            CALL Run()
            SUB Run()
            LET x = Square(3)
            PRINT x
            END SUB
            FUNCTION Square(n As Integer) As Integer
            RETURN n * n
            END FUNCTION
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("9", output);
  }

  [Fact]
  public void Subs_Declared_After_Function_Are_Still_Callable_From_Function()
  {
    var source = """
            LET result = Compute()
            PRINT result
            FUNCTION Compute() As Integer
            CALL PrintSomething()
            RETURN 42
            END FUNCTION
            SUB PrintSomething()
            PRINT "side-effect"
            END SUB
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("side-effect\n42", output);
  }

  // --- Scope isolation across calls ---

  [Fact]
  public void Variables_Set_In_Callee_Sub_Are_Not_Visible_In_Outer_Scope()
  {
    // 'local' is defined inside Second (called via First).
    // After both subs complete, the outer scope still has no 'local'.
    var source = """
            CALL First()
            PRINT local
            SUB First()
            CALL Second()
            END SUB
            SUB Second()
            LET local = 99
            END SUB
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }
}
