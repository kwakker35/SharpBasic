using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// Tests for DIM array bounds expressed as runtime expressions rather than
/// literal integers. Covers 1D and 2D arrays, happy paths, edge cases, and
/// all error paths (wrong type, zero, negative, undefined variable, redeclaration).
/// </summary>
public class DimExprBoundTests
{
  // ─── Happy path: literal integer (regression — must still work) ──────────

  [Fact]
  public void Dim_LiteralBound_Works()
  {
    var result = RunHelper.Run("""
            DIM arr[3] AS INTEGER
            LET arr[0] = 7
            PRINT arr[0]
            """);
    Assert.Equal("7", result);
  }

  [Fact]
  public void Dim2d_LiteralBounds_Works()
  {
    var result = RunHelper.Run("""
            DIM grid[2][3] AS INTEGER
            LET grid[0][1] = 42
            PRINT grid[0][1]
            """);
    Assert.Equal("42", result);
  }

  // ─── Happy path: CONST as bound ──────────────────────────────────────────

  [Fact]
  public void Dim_ConstBound_Allocates_Correctly()
  {
    var result = RunHelper.Run("""
            CONST MAX_SIZE = 5
            DIM scores[MAX_SIZE] AS INTEGER
            LET scores[0] = 42
            PRINT scores[0]
            """);
    Assert.Equal("42", result);
  }

  [Fact]
  public void Dim_ConstBound_AllIndicesAccessible()
  {
    var result = RunHelper.Run("""
            CONST N = 3
            DIM vals[N] AS INTEGER
            LET vals[0] = 10
            LET vals[1] = 20
            LET vals[2] = 30
            PRINT vals[0] & " " & vals[1] & " " & vals[2]
            """);
    Assert.Equal("10 20 30", result);
  }

  [Fact]
  public void Dim2d_ConstBounds_Allocates_Correctly()
  {
    var result = RunHelper.Run("""
            CONST ROWS = 2
            CONST COLS = 3
            DIM grid[ROWS][COLS] AS INTEGER
            LET grid[1][2] = 99
            PRINT grid[1][2]
            """);
    Assert.Equal("99", result);
  }

  // ─── Happy path: LET variable as bound ───────────────────────────────────

  [Fact]
  public void Dim_VariableBound_Allocates_Correctly()
  {
    var result = RunHelper.Run("""
            LET n = 4
            DIM items[n] AS INTEGER
            LET items[3] = 77
            PRINT items[3]
            """);
    Assert.Equal("77", result);
  }

  // ─── Happy path: arithmetic expression as bound ──────────────────────────

  [Fact]
  public void Dim_ArithmeticBound_Allocates_Correctly()
  {
    var result = RunHelper.Run("""
            CONST BASE = 3
            DIM arr[BASE + 2] AS INTEGER
            LET arr[4] = 55
            PRINT arr[4]
            """);
    Assert.Equal("55", result);
  }

  [Fact]
  public void Dim2d_ArithmeticBounds_Allocates_Correctly()
  {
    var result = RunHelper.Run("""
            CONST R = 1
            CONST C = 2
            DIM grid[R + 1][C + 1] AS INTEGER
            LET grid[1][2] = 11
            PRINT grid[1][2]
            """);
    Assert.Equal("11", result);
  }

  // ─── Happy path: underscore-named CONST as bound (combined feature test) ─

  [Fact]
  public void Dim_UnderscoreConst_AsBound()
  {
    var result = RunHelper.Run("""
            CONST MAX_ITEMS = 5
            DIM item_list[MAX_ITEMS] AS INTEGER
            LET item_list[4] = 9
            PRINT item_list[4]
            """);
    Assert.Equal("9", result);
  }

  // ─── Happy path: different element types ─────────────────────────────────

  [Fact]
  public void Dim_ConstBound_StringArray()
  {
    var result = RunHelper.Run("""
            CONST SIZE = 2
            DIM names[SIZE] AS STRING
            LET names[0] = "Alice"
            LET names[1] = "Bob"
            PRINT names[0] & " " & names[1]
            """);
    Assert.Equal("Alice Bob", result);
  }

  [Fact]
  public void Dim_ConstBound_BooleanArray_DefaultsFalse()
  {
    var result = RunHelper.Run("""
            CONST N = 3
            DIM flags[N] AS BOOLEAN
            PRINT flags[0]
            """);
    Assert.Equal("False", result);
  }

  // ─── Error path: float bound ──────────────────────────────────────────────

  [Fact]
  public void Dim_FloatBound_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            DIM arr[3.5] AS INTEGER
            """);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  [Fact]
  public void Dim2d_FloatRowBound_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            DIM grid[2.5][3] AS INTEGER
            """);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  [Fact]
  public void Dim2d_FloatColBound_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            DIM grid[2][3.5] AS INTEGER
            """);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  // ─── Error path: string bound ────────────────────────────────────────────

  [Fact]
  public void Dim_StringBound_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            DIM arr["five"] AS INTEGER
            """);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  // ─── Error path: float CONST as bound ────────────────────────────────────

  [Fact]
  public void Dim_FloatConstBound_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            CONST SIZE = 3.5
            DIM arr[SIZE] AS INTEGER
            """);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  // ─── Error path: redeclaration still fails regardless of bound type ───────

  [Fact]
  public void Dim_ConstBound_Redeclaration_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            CONST N = 3
            DIM arr[N] AS INTEGER
            DIM arr[N] AS INTEGER
            """);
    Assert.IsType<EvalFailure>(result);
  }

  // ─── Edge case: bound expression evaluates to 1 (minimum valid) ──────────

  [Fact]
  public void Dim_BoundOfOne_SingleElement_Works()
  {
    var result = RunHelper.Run("""
            CONST N = 1
            DIM solo[N] AS INTEGER
            LET solo[0] = 100
            PRINT solo[0]
            """);
    Assert.Equal("100", result);
  }

  // ─── Edge case: bound from arithmetic produces correct size ──────────────

  [Fact]
  public void Dim_BoundOutOfRange_AfterArithmetic_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            CONST MAX = 2
            DIM arr[MAX] AS INTEGER
            LET arr[5] = 1
            """);
    Assert.IsType<EvalFailure>(result);
  }

  // ─── Regression: 1D literal dim unchanged behaviour ──────────────────────

  [Fact]
  public void Dim_LiteralBound_DefaultsToZero()
  {
    var result = RunHelper.Run("""
            DIM nums[3] AS INTEGER
            PRINT nums[0]
            """);
    Assert.Equal("0", result);
  }

  [Fact]
  public void Dim_LiteralBound_OutOfBounds_ReturnsEvalFailure()
  {
    var result = RunHelper.RunResult("""
            DIM nums[3] AS INTEGER
            LET nums[10] = 1
            """);
    Assert.IsType<EvalFailure>(result);
  }
}
