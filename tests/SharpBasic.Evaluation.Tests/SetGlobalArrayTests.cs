using SharpBasic.Ast;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// Tests for SET GLOBAL array indexing:
///   SET GLOBAL arr[i] = value
///   SET GLOBAL arr[r][c] = value
/// </summary>
public class SetGlobalArrayTests
{
  // ------------------------------------------------------------------
  // 1D array
  // ------------------------------------------------------------------

  [Fact]
  public void SetGlobal_1DArray_UpdatesElementFromSub()
  {
    var source = """
            DIM scores[5] AS INTEGER
            SUB Fill()
                SET GLOBAL scores[0] = 10
                SET GLOBAL scores[1] = 20
                SET GLOBAL scores[4] = 99
            END SUB
            CALL Fill()
            PRINT scores[0]
            PRINT scores[1]
            PRINT scores[4]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("10\n20\n99", output);
  }

  [Fact]
  public void SetGlobal_1DArray_UpdatesElementUsingExpressionIndex()
  {
    var source = """
            DIM vals[4] AS INTEGER
            LET idx = 2
            SUB SetIt()
                SET GLOBAL vals[idx] = 77
            END SUB
            CALL SetIt()
            PRINT vals[2]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("77", output);
  }

  [Fact]
  public void SetGlobal_1DArray_DoesNotAffectOtherSlots()
  {
    var source = """
            DIM arr[3] AS INTEGER
            LET arr[0] = 1
            LET arr[1] = 2
            LET arr[2] = 3
            SUB OverwriteMiddle()
                SET GLOBAL arr[1] = 99
            END SUB
            CALL OverwriteMiddle()
            PRINT arr[0]
            PRINT arr[1]
            PRINT arr[2]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("1\n99\n3", output);
  }

  [Fact]
  public void SetGlobal_1DArray_OutOfBounds_ReturnsError()
  {
    var source = """
            DIM tiny[3] AS INTEGER
            SUB Blow()
                SET GLOBAL tiny[10] = 1
            END SUB
            CALL Blow()
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void SetGlobal_1DArray_UndeclaredName_ReturnsError()
  {
    var source = """
            SUB Blow()
                SET GLOBAL ghost[0] = 1
            END SUB
            CALL Blow()
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  // ------------------------------------------------------------------
  // 2D array
  // ------------------------------------------------------------------

  [Fact]
  public void SetGlobal_2DArray_UpdatesElementFromSub()
  {
    var source = """
            DIM grid[3][3] AS INTEGER
            SUB Fill()
                SET GLOBAL grid[0][0] = 1
                SET GLOBAL grid[1][2] = 7
                SET GLOBAL grid[2][1] = 5
            END SUB
            CALL Fill()
            PRINT grid[0][0]
            PRINT grid[1][2]
            PRINT grid[2][1]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("1\n7\n5", output);
  }

  [Fact]
  public void SetGlobal_2DArray_UpdatesElementUsingExpressionIndices()
  {
    var source = """
            DIM map[4][4] AS INTEGER
            LET r = 2
            LET c = 3
            SUB Place()
                SET GLOBAL map[r][c] = 42
            END SUB
            CALL Place()
            PRINT map[2][3]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("42", output);
  }

  [Fact]
  public void SetGlobal_2DArray_DoesNotAffectOtherCells()
  {
    var source = """
            DIM mat[2][2] AS INTEGER
            LET mat[0][0] = 1
            LET mat[0][1] = 2
            LET mat[1][0] = 3
            LET mat[1][1] = 4
            SUB Overwrite()
                SET GLOBAL mat[0][1] = 99
            END SUB
            CALL Overwrite()
            PRINT mat[0][0]
            PRINT mat[0][1]
            PRINT mat[1][0]
            PRINT mat[1][1]
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("1\n99\n3\n4", output);
  }

  [Fact]
  public void SetGlobal_2DArray_OutOfBounds_ReturnsError()
  {
    var source = """
            DIM small[2][2] AS INTEGER
            SUB Blow()
                SET GLOBAL small[5][5] = 1
            END SUB
            CALL Blow()
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void SetGlobal_2DArray_UndeclaredName_ReturnsError()
  {
    var source = """
            SUB Blow()
                SET GLOBAL ghost[0][0] = 1
            END SUB
            CALL Blow()
            """;
    var result = RunHelper.RunResult(source);
    Assert.IsType<EvalFailure>(result);
  }

  // ------------------------------------------------------------------
  // Regression: scalar SET GLOBAL must still work
  // ------------------------------------------------------------------

  [Fact]
  public void SetGlobal_Scalar_StillWorksAfterChange()
  {
    var source = """
            LET x = 0
            SUB Assign()
                SET GLOBAL x = 55
            END SUB
            CALL Assign()
            PRINT x
            """;
    var output = RunHelper.Run(source);
    Assert.Equal("55", output);
  }
}
