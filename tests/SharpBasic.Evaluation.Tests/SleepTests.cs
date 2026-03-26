using System.Diagnostics;
using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class SleepTests
{
  // --- Happy path ---

  [Fact]
  public void Sleep_Zero_Is_Valid_No_Op()
  {
    var output = RunHelper.Run("SLEEP(0)\nPRINT \"ok\"");
    Assert.Equal("ok", output);
  }

  [Fact]
  public void Sleep_Positive_Milliseconds_Completes_And_Continues()
  {
    var output = RunHelper.Run("SLEEP(100)\nPRINT \"done\"");
    Assert.Equal("done", output);
  }

  [Fact]
  public void Sleep_Positive_Milliseconds_Actually_Sleeps()
  {
    var sw = Stopwatch.StartNew();
    RunHelper.Run("SLEEP(100)");
    sw.Stop();
    Assert.True(sw.ElapsedMilliseconds >= 80,
        $"Expected at least 80ms elapsed, got {sw.ElapsedMilliseconds}ms");
  }

  [Fact]
  public void Sleep_With_Variable_Argument()
  {
    var output = RunHelper.Run("LET ms = 50\nSLEEP(ms)\nPRINT \"ok\"");
    Assert.Equal("ok", output);
  }

  [Fact]
  public void Sleep_With_Expression_Argument()
  {
    var output = RunHelper.Run("SLEEP(50 * 2)\nPRINT \"ok\"");
    Assert.Equal("ok", output);
  }

  [Fact]
  public void Sleep_With_Const_Argument()
  {
    var output = RunHelper.Run("CONST DELAY = 50\nSLEEP(DELAY)\nPRINT \"ok\"");
    Assert.Equal("ok", output);
  }

  // --- Error cases ---

  [Fact]
  public void Sleep_Negative_Value_Returns_Diagnostic()
  {
    var result = RunHelper.RunResult("SLEEP(-1)");
    Assert.IsType<EvalFailure>(result);
    var failure = (EvalFailure)result;
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("non-negative"));
  }

  [Fact]
  public void Sleep_Float_Argument_Returns_Diagnostic()
  {
    var result = RunHelper.RunResult("SLEEP(1.5)");
    Assert.IsType<EvalFailure>(result);
    var failure = (EvalFailure)result;
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  [Fact]
  public void Sleep_String_Argument_Returns_Diagnostic()
  {
    var result = RunHelper.RunResult("SLEEP(\"hello\")");
    Assert.IsType<EvalFailure>(result);
    var failure = (EvalFailure)result;
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  // --- Shadow protection ---

  [Fact]
  public void Sleep_Cannot_Be_Shadowed_As_Sub()
  {
    // SLEEP tokenises as TokenType.Sleep, not Identifier.
    // The parser will reject "SUB SLEEP(...)" with a parse error.
    var tokens = new SharpBasic.Lexing.Lexer("SUB SLEEP(ms AS INTEGER)\nPRINT ms\nEND SUB").Tokenise();
    var parseResult = new SharpBasic.Parsing.Parser(tokens).Parse();
    Assert.IsType<SharpBasic.Parsing.ParseFailure>(parseResult);
  }
}
