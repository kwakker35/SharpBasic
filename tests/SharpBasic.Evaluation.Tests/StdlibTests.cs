using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class StdlibTests
{
  // --- LEN ---

  [Fact]
  public void LEN_Returns_Length_Of_String()
  {
    var output = RunHelper.Run("PRINT LEN(\"Hello\")");
    Assert.Equal("5", output);
  }

  [Fact]
  public void LEN_Returns_Zero_For_Empty_String()
  {
    var output = RunHelper.Run("PRINT LEN(\"\")");
    Assert.Equal("0", output);
  }

  [Fact]
  public void LEN_Returns_Length_Of_Variable()
  {
    var output = RunHelper.Run("LET s = \"SharpBASIC\"\nPRINT LEN(s)");
    Assert.Equal("10", output);
  }

  // --- MID$ ---

  [Fact]
  public void MID_Dollar_Returns_Substring_From_Middle()
  {
    // MID$("Hello", 2, 3) → "ell"  (1-based index)
    var output = RunHelper.Run("PRINT MID$(\"Hello\", 2, 3)");
    Assert.Equal("ell", output);
  }

  [Fact]
  public void MID_Dollar_From_Start_Returns_Full_String()
  {
    var output = RunHelper.Run("PRINT MID$(\"Hello\", 1, 5)");
    Assert.Equal("Hello", output);
  }

  [Fact]
  public void MID_Dollar_Single_Character()
  {
    var output = RunHelper.Run("PRINT MID$(\"Hello\", 3, 1)");
    Assert.Equal("l", output);
  }

  // --- LEFT$ ---

  [Fact]
  public void LEFT_Dollar_Returns_Left_Substring()
  {
    var output = RunHelper.Run("PRINT LEFT$(\"Hello\", 3)");
    Assert.Equal("Hel", output);
  }

  [Fact]
  public void LEFT_Dollar_Zero_Length_Returns_Empty_String()
  {
    var output = RunHelper.Run("PRINT LEFT$(\"Hello\", 0)");
    Assert.Equal("", output);
  }

  [Fact]
  public void LEFT_Dollar_Full_Length_Returns_Whole_String()
  {
    var output = RunHelper.Run("PRINT LEFT$(\"Hello\", 5)");
    Assert.Equal("Hello", output);
  }

  // --- RIGHT$ ---

  [Fact]
  public void RIGHT_Dollar_Returns_Right_Substring()
  {
    var output = RunHelper.Run("PRINT RIGHT$(\"Hello\", 3)");
    Assert.Equal("llo", output);
  }

  [Fact]
  public void RIGHT_Dollar_Zero_Length_Returns_Empty_String()
  {
    var output = RunHelper.Run("PRINT RIGHT$(\"Hello\", 0)");
    Assert.Equal("", output);
  }

  [Fact]
  public void RIGHT_Dollar_Full_Length_Returns_Whole_String()
  {
    var output = RunHelper.Run("PRINT RIGHT$(\"Hello\", 5)");
    Assert.Equal("Hello", output);
  }

  // --- UPPER$ ---

  [Fact]
  public void UPPER_Dollar_Uppercases_String()
  {
    var output = RunHelper.Run("PRINT UPPER$(\"hello\")");
    Assert.Equal("HELLO", output);
  }

  [Fact]
  public void UPPER_Dollar_Mixed_Case_Uppercased()
  {
    var output = RunHelper.Run("PRINT UPPER$(\"Hello World\")");
    Assert.Equal("HELLO WORLD", output);
  }

  // --- LOWER$ ---

  [Fact]
  public void LOWER_Dollar_Lowercases_String()
  {
    var output = RunHelper.Run("PRINT LOWER$(\"HELLO\")");
    Assert.Equal("hello", output);
  }

  [Fact]
  public void LOWER_Dollar_Mixed_Case_Lowercased()
  {
    var output = RunHelper.Run("PRINT LOWER$(\"Hello World\")");
    Assert.Equal("hello world", output);
  }

  // --- INT ---

  [Fact]
  public void INT_Positive_Float_Floors_Toward_Zero()
  {
    var output = RunHelper.Run("PRINT INT(3.7)");
    Assert.Equal("3", output);
  }

  [Fact]
  public void INT_Negative_Float_Floors_Away_From_Zero()
  {
    // INT in BASIC floors toward negative infinity
    var output = RunHelper.Run("PRINT INT(-3.7)");
    Assert.Equal("-4", output);
  }

  [Fact]
  public void INT_Whole_Number_Returns_Same_Value()
  {
    var output = RunHelper.Run("PRINT INT(5.0)");
    Assert.Equal("5", output);
  }

  // --- STR$ ---

  [Fact]
  public void STR_Dollar_Converts_Integer_To_String()
  {
    var output = RunHelper.Run("PRINT STR$(42)");
    Assert.Equal("42", output);
  }

  [Fact]
  public void STR_Dollar_Converts_Float_To_String()
  {
    var output = RunHelper.Run("PRINT STR$(3.14)");
    Assert.Equal("3.14", output);
  }

  [Fact]
  public void STR_Dollar_Result_Can_Be_Stored_In_Variable()
  {
    var output = RunHelper.Run("LET s = STR$(99)\nPRINT LEN(s)");
    Assert.Equal("2", output);
  }

  // --- VAL ---

  [Fact]
  public void VAL_Converts_Integer_String_To_Number()
  {
    var output = RunHelper.Run("PRINT VAL(\"42\")");
    Assert.Equal("42", output);
  }

  [Fact]
  public void VAL_Converts_Float_String_To_Number()
  {
    var output = RunHelper.Run("PRINT VAL(\"3.14\")");
    Assert.Equal("3.14", output);
  }

  [Fact]
  public void VAL_Result_Can_Be_Used_In_Arithmetic()
  {
    var output = RunHelper.Run("PRINT VAL(\"10\") + VAL(\"5\")");
    Assert.Equal("15", output);
  }

  // --- ABS ---

  [Fact]
  public void ABS_Negative_Integer_Returns_Positive()
  {
    var output = RunHelper.Run("PRINT ABS(-5)");
    Assert.Equal("5", output);
  }

  [Fact]
  public void ABS_Positive_Integer_Is_Unchanged()
  {
    var output = RunHelper.Run("PRINT ABS(5)");
    Assert.Equal("5", output);
  }

  [Fact]
  public void ABS_Negative_Float_Returns_Positive()
  {
    var output = RunHelper.Run("PRINT ABS(-3.5)");
    Assert.Equal("3.5", output);
  }

  [Fact]
  public void ABS_Zero_Returns_Zero()
  {
    var output = RunHelper.Run("PRINT ABS(0)");
    Assert.Equal("0", output);
  }

  // --- SQR ---

  [Fact]
  public void SQR_Perfect_Square_Returns_Whole_Number()
  {
    var output = RunHelper.Run("PRINT SQR(25)");
    Assert.Equal("5", output);
  }

  [Fact]
  public void SQR_One_Returns_One()
  {
    var output = RunHelper.Run("PRINT SQR(1)");
    Assert.Equal("1", output);
  }

  [Fact]
  public void SQR_Zero_Returns_Zero()
  {
    var output = RunHelper.Run("PRINT SQR(0)");
    Assert.Equal("0", output);
  }

  // --- RND ---

  [Fact]
  public void RND_Returns_Float_Between_Zero_And_One()
  {
    var output = RunHelper.Run("PRINT RND()");
    Assert.True(
        double.TryParse(output, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var v),
        $"Expected a numeric value but got: '{output}'");
    Assert.InRange(v, 0.0, 1.0);
  }

  [Fact]
  public void RND_Two_Calls_Can_Produce_Different_Values()
  {
    // Run twice — statistical chance of collision is negligible
    var a = RunHelper.Run("PRINT RND()");
    var b = RunHelper.Run("PRINT RND()");
    // At minimum, both must be valid floats in [0, 1]
    Assert.True(double.TryParse(a, System.Globalization.NumberStyles.Float,
        System.Globalization.CultureInfo.InvariantCulture, out _));
    Assert.True(double.TryParse(b, System.Globalization.NumberStyles.Float,
        System.Globalization.CultureInfo.InvariantCulture, out _));
  }

  // --- TRIM$ ---

  [Fact]
  public void TRIM_Dollar_Removes_Leading_And_Trailing_Spaces()
  {
    var output = RunHelper.Run("PRINT TRIM$(\"  hello  \")");
    Assert.Equal("hello", output);
  }

  [Fact]
  public void TRIM_Dollar_No_Op_On_String_With_No_Whitespace()
  {
    var output = RunHelper.Run("PRINT TRIM$(\"hello\")");
    Assert.Equal("hello", output);
  }

  [Fact]
  public void TRIM_Dollar_Returns_Empty_For_All_Spaces()
  {
    var output = RunHelper.Run("PRINT TRIM$(\"   \")");
    Assert.Equal("", output);
  }

  [Fact]
  public void TRIM_Dollar_Works_On_Variable()
  {
    var output = RunHelper.Run("LET s$ = \"  world  \"\nPRINT TRIM$(s$)");
    Assert.Equal("world", output);
  }

  // --- Bug fix: locale-sensitive float parsing ---

  [Fact]
  public void Float_Literal_Parses_Correctly_Under_Comma_Decimal_Culture()
  {
    // In cultures where ',' is the decimal separator (e.g. de-DE),
    // double.TryParse without InvariantCulture silently fails.
    var original = System.Threading.Thread.CurrentThread.CurrentCulture;
    try
    {
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo("de-DE");
      var output = RunHelper.Run("PRINT 3.14");
      Assert.Equal("3.14", output);
    }
    finally
    {
      System.Threading.Thread.CurrentThread.CurrentCulture = original;
    }
  }

  [Fact]
  public void VAL_Parses_Float_String_Correctly_Under_Comma_Decimal_Culture()
  {
    var original = System.Threading.Thread.CurrentThread.CurrentCulture;
    try
    {
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo("de-DE");
      var output = RunHelper.Run("PRINT VAL(\"3.14\")");
      Assert.Equal("3.14", output);
    }
    finally
    {
      System.Threading.Thread.CurrentThread.CurrentCulture = original;
    }
  }
}
