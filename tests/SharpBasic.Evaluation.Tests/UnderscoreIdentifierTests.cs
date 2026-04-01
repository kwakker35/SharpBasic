using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Evaluation.Tests;

/// <summary>
/// End-to-end tests for underscore support in identifiers.
/// Covers valid patterns, invalid patterns, and boundary conditions.
/// </summary>
public class UnderscoreIdentifierTests
{
  // --- Valid CONST with underscore ---

  [Fact]
  public void Underscore_Const_SingleUnderscore_Prints()
  {
    var result = RunHelper.Run("CONST MAX_WIDTH = 80\nPRINT MAX_WIDTH");
    Assert.Equal("80", result);
  }

  [Fact]
  public void Underscore_Const_MultiSegment_Prints()
  {
    var result = RunHelper.Run("CONST ITEM_GOLD_BAG = 11\nPRINT ITEM_GOLD_BAG");
    Assert.Equal("11", result);
  }

  [Fact]
  public void Underscore_Const_ShortSegments_Prints()
  {
    var result = RunHelper.Run("CONST DIR_N = 1\nCONST DIR_S = 2\nPRINT DIR_N & \" \" & DIR_S");
    Assert.Equal("1 2", result);
  }

  // --- Valid LET with underscore ---

  [Fact]
  public void Underscore_Let_SingleUnderscore_Prints()
  {
    var result = RunHelper.Run("LET start_time = 42\nPRINT start_time");
    Assert.Equal("42", result);
  }

  [Fact]
  public void Underscore_Let_MultiSegments_Prints()
  {
    var result = RunHelper.Run("LET x_y_z = 99\nPRINT x_y_z");
    Assert.Equal("99", result);
  }

  [Fact]
  public void Underscore_Let_AlphanumericSegments_Prints()
  {
    var result = RunHelper.Run("LET a1_b2_c3 = 7\nPRINT a1_b2_c3");
    Assert.Equal("7", result);
  }

  // --- Multi-word constant pattern (primary use case) ---

  [Fact]
  public void Underscore_MultipleConsts_PrintMultiple()
  {
    var source = """
            CONST FRAME_WIDTH = 80
            CONST SCREEN_HEIGHT = 30
            CONST CONTENT_ROWS = 20
            CONST MAX_INVENTORY = 4
            CONST ITEM_HEALING_POTION = 1
            CONST ITEM_LUCKY_CHARM = 2
            CONST ITEM_GOLD_BAG = 11
            PRINT FRAME_WIDTH & " " & SCREEN_HEIGHT & " " & CONTENT_ROWS
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("80 30 20", result);
  }

  // --- Underscore in SUB and FUNCTION names ---

  [Fact]
  public void Underscore_SubName_CallsCorrectly()
  {
    var source = """
            SUB print_result(val AS INTEGER)
                PRINT val
            END SUB

            CALL print_result(42)
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("42", result);
  }

  [Fact]
  public void Underscore_FunctionName_ReturnsCorrectly()
  {
    var source = """
            FUNCTION get_max(a AS INTEGER, b AS INTEGER) AS INTEGER
                IF a > b THEN
                    RETURN a
                END IF
                RETURN b
            END FUNCTION

            PRINT get_max(3, 7)
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("7", result);
  }

  // --- Invalid identifier patterns — must produce EvalFailure ---

  [Fact]
  public void Underscore_Leading_ReturnsFailure()
  {
    var result = RunHelper.RunResult("LET _name = 1");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Underscore_Trailing_ReturnsFailure()
  {
    var result = RunHelper.RunResult("LET done_ = 1");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Underscore_Consecutive_ReturnsFailure()
  {
    var result = RunHelper.RunResult("LET x__y = 1");
    Assert.IsType<EvalFailure>(result);
  }

  [Fact]
  public void Underscore_BareUnderscore_ReturnsFailure()
  {
    var result = RunHelper.RunResult("LET _ = 1");
    Assert.IsType<EvalFailure>(result);
  }

  // --- Boundary: built-in $ functions unaffected ---

  [Fact]
  public void Underscore_Boundary_UpperDollarStillWorks()
  {
    var result = RunHelper.Run("PRINT UPPER$(\"hello\")");
    Assert.Equal("HELLO", result);
  }

  [Fact]
  public void Underscore_Boundary_LeftDollarStillWorks()
  {
    var result = RunHelper.Run("PRINT LEFT$(\"abcdef\", 3)");
    Assert.Equal("abc", result);
  }

  [Fact]
  public void Underscore_Boundary_MidDollarStillWorks()
  {
    var result = RunHelper.Run("PRINT MID$(\"hello\", 2, 3)");
    Assert.Equal("ell", result);
  }

  // --- Boundary: existing identifiers without underscores unaffected ---

  [Fact]
  public void Underscore_Boundary_PlainIdentifiers_Unaffected()
  {
    var result = RunHelper.Run("LET myVar = 10\nLET camelCase = 20\nPRINT myVar & \" \" & camelCase");
    Assert.Equal("10 20", result);
  }

  // --- Boundary: keywords still recognised ---

  [Fact]
  public void Underscore_Boundary_KeywordsUnaffected()
  {
    var source = """
            LET count = 0
            WHILE count < 3
                LET count = count + 1
            WEND
            PRINT count
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("3", result);
  }

  // --- Underscore in array names ---

  [Fact]
  public void Underscore_ArrayName_DimAndAccess()
  {
    var source = """
            DIM item_codes[5] AS INTEGER
            LET item_codes[0] = 11
            LET item_codes[1] = 7
            PRINT item_codes[0] & " " & item_codes[1]
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("11 7", result);
  }

  // --- Underscore as a parameter name ---

  [Fact]
  public void Underscore_ParameterName_SubReceivesCorrectly()
  {
    var source = """
            SUB show_value(item_id AS INTEGER)
                PRINT item_id
            END SUB

            CALL show_value(99)
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("99", result);
  }

  [Fact]
  public void Underscore_ParameterName_FunctionReceivesAndReturnsCorrectly()
  {
    var source = """
            FUNCTION double_it(input_val AS INTEGER) AS INTEGER
                RETURN input_val * 2
            END FUNCTION

            PRINT double_it(6)
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("12", result);
  }

  // --- Underscore identifier as FOR loop counter ---

  [Fact]
  public void Underscore_ForLoopCounter_IteratesCorrectly()
  {
    var source = """
            LET total = 0
            FOR loop_i = 1 TO 3
                LET total = total + loop_i
            NEXT loop_i
            PRINT total
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("6", result);
  }

  // --- Underscore identifier read in arithmetic expression (RHS) ---

  [Fact]
  public void Underscore_IdentifierReadInArithmetic()
  {
    var source = """
            LET base_val = 10
            LET step_size = 3
            LET result = base_val + step_size * 2
            PRINT result
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("16", result);
  }

  [Fact]
  public void Underscore_ConstReadInArithmetic()
  {
    var source = """
            CONST MAX_SCORE = 100
            CONST PENALTY = 15
            LET final_score = MAX_SCORE - PENALTY
            PRINT final_score
            """;
    var result = RunHelper.Run(source);
    Assert.Equal("85", result);
  }
}
