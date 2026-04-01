using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// Tests that LET arr[i] = val inside a SUB or FUNCTION respects scope rules.
/// Arrays must be DIM'd in the current scope to be writable via LET.
/// Writing to a global array from inside a SUB/FUNCTION requires SET GLOBAL.
/// </summary>
public class LetArrayScopeTests
{
  // ------------------------------------------------------------------
  // 1D arrays — SUB scope
  // ------------------------------------------------------------------

  [Fact]
  public void Let_Array_In_Sub_Fails_When_Array_Is_Global()
  {
    var source = """
            DIM scores[3] AS INTEGER
            LET scores[0] = 10
            SUB Mutate()
                LET scores[0] = 99
            END SUB
            CALL Mutate()
            PRINT scores[0]
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Let_Array_In_Sub_Works_When_Array_Is_Local()
  {
    var source = """
            SUB ShowLocal()
                DIM local[3] AS INTEGER
                LET local[0] = 42
                PRINT local[0]
            END SUB
            CALL ShowLocal()
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("42", output);
  }

  [Fact]
  public void Let_Array_In_Sub_Does_Not_Mutate_Global()
  {
    var source = """
            DIM scores[3] AS INTEGER
            LET scores[0] = 10
            SUB Mutate()
                LET scores[0] = 99
            END SUB
            CALL Mutate()
            PRINT scores[0]
            """;
    // Should be EvalFailure — but even if implementation chose to
    // silently ignore the write, the global must remain unchanged.
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void SetGlobal_Array_In_Sub_Still_Works()
  {
    var source = """
            DIM scores[3] AS INTEGER
            LET scores[0] = 10
            SUB Mutate()
                SET GLOBAL scores[0] = 99
            END SUB
            CALL Mutate()
            PRINT scores[0]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("99", output);
  }

  [Fact]
  public void Let_Array_At_Global_Scope_Still_Works()
  {
    var source = """
            DIM nums[3] AS INTEGER
            LET nums[0] = 1
            LET nums[1] = 2
            LET nums[2] = 3
            PRINT nums[0]
            PRINT nums[1]
            PRINT nums[2]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("1\n2\n3", output);
  }

  // ------------------------------------------------------------------
  // 1D arrays — FUNCTION scope
  // ------------------------------------------------------------------

  [Fact]
  public void Let_Array_In_Function_Fails_When_Array_Is_Global()
  {
    var source = """
            DIM vals[3] AS INTEGER
            LET vals[0] = 5
            FUNCTION Mutate() AS INTEGER
                LET vals[0] = 99
                RETURN 0
            END FUNCTION
            LET dummy = Mutate()
            PRINT vals[0]
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Let_Array_In_Function_Works_When_Array_Is_Local()
  {
    var source = """
            FUNCTION MakeAndRead() AS INTEGER
                DIM local[3] AS INTEGER
                LET local[0] = 77
                RETURN local[0]
            END FUNCTION
            PRINT MakeAndRead()
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("77", output);
  }

  // ------------------------------------------------------------------
  // 2D arrays — SUB scope
  // ------------------------------------------------------------------

  [Fact]
  public void Let_2D_Array_In_Sub_Fails_When_Array_Is_Global()
  {
    var source = """
            DIM grid[3][3] AS INTEGER
            LET grid[0][0] = 1
            SUB Mutate()
                LET grid[1][1] = 99
            END SUB
            CALL Mutate()
            PRINT grid[1][1]
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Let_2D_Array_In_Sub_Works_When_Array_Is_Local()
  {
    var source = """
            SUB ShowLocal()
                DIM grid[2][2] AS INTEGER
                LET grid[0][1] = 42
                PRINT grid[0][1]
            END SUB
            CALL ShowLocal()
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("42", output);
  }

  [Fact]
  public void SetGlobal_2D_Array_In_Sub_Still_Works()
  {
    var source = """
            DIM grid[3][3] AS INTEGER
            SUB Fill()
                SET GLOBAL grid[1][1] = 55
            END SUB
            CALL Fill()
            PRINT grid[1][1]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("55", output);
  }

  // ------------------------------------------------------------------
  // Nested calls
  // ------------------------------------------------------------------

  [Fact]
  public void Let_Array_In_Nested_Sub_Fails_When_Array_Is_Global()
  {
    var source = """
            DIM data[5] AS INTEGER
            LET data[0] = 10
            SUB Inner()
                LET data[0] = 99
            END SUB
            SUB Outer()
                CALL Inner()
            END SUB
            CALL Outer()
            PRINT data[0]
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void SetGlobal_Array_In_Nested_Sub_Works()
  {
    var source = """
            DIM data[5] AS INTEGER
            LET data[0] = 10
            SUB Inner()
                SET GLOBAL data[0] = 99
            END SUB
            SUB Outer()
                CALL Inner()
            END SUB
            CALL Outer()
            PRINT data[0]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("99", output);
  }

  // ------------------------------------------------------------------
  // Reading global arrays from a SUB is still allowed
  // ------------------------------------------------------------------

  [Fact]
  public void Reading_Global_Array_From_Sub_Still_Works()
  {
    var source = """
            DIM vals[3] AS INTEGER
            LET vals[0] = 42
            SUB ShowIt()
                PRINT vals[0]
            END SUB
            CALL ShowIt()
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("42", output);
  }

  [Fact]
  public void Reading_Global_2D_Array_From_Sub_Still_Works()
  {
    var source = """
            DIM grid[2][2] AS INTEGER
            LET grid[0][1] = 77
            SUB ShowIt()
                PRINT grid[0][1]
            END SUB
            CALL ShowIt()
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("77", output);
  }
}
