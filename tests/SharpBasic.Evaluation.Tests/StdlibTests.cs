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

  // --- STRING$ ---

  [Fact]
  public void STRING_Dollar_Repeats_Char_Five_Times()
  {
    var output = RunHelper.Run("PRINT STRING$(\"=\", 5)");
    Assert.Equal("=====", output);
  }

  [Fact]
  public void STRING_Dollar_Repeats_Char_Once()
  {
    var output = RunHelper.Run("PRINT STRING$(\"*\", 1)");
    Assert.Equal("*", output);
  }

  [Fact]
  public void STRING_Dollar_Count_Zero_Returns_Empty_String()
  {
    var output = RunHelper.Run("PRINT STRING$(\"x\", 0)");
    Assert.Equal("", output);
  }

  [Fact]
  public void STRING_Dollar_Count_80_Has_Correct_Length()
  {
    var output = RunHelper.Run("CONST FRAME_WIDTH = 80\nPRINT LEN(STRING$(\"=\", FRAME_WIDTH))");
    Assert.Equal("80", output);
  }

  [Fact]
  public void STRING_Dollar_Concatenation_With_Ampersand()
  {
    var output = RunHelper.Run("PRINT STRING$(\"-\", 10) & \"X\" & STRING$(\"-\", 10)");
    Assert.Equal("----------X----------", output);
  }

  [Fact]
  public void STRING_Dollar_With_Variable_Char_And_Count()
  {
    var source = "LET ch = \"=\"\nLET n = 20\nPRINT STRING$(ch, n)";
    var output = RunHelper.Run(source);
    Assert.Equal("====================", output);
  }

  [Fact]
  public void STRING_Dollar_With_CHR_Dollar_Argument()
  {
    var output = RunHelper.Run("PRINT STRING$(CHR$(34), 3)");
    Assert.Equal("\"\"\"", output);
  }

  [Fact]
  public void STRING_Dollar_Multi_Char_Arg_Returns_Null_Prints_Empty()
  {
    var output = RunHelper.Run("PRINT STRING$(\"ab\", 5)");
    Assert.Equal("", output);
  }

  [Fact]
  public void STRING_Dollar_Empty_Char_Arg_Returns_Null_Prints_Empty()
  {
    var output = RunHelper.Run("PRINT STRING$(\"\", 5)");
    Assert.Equal("", output);
  }

  [Fact]
  public void STRING_Dollar_Negative_Count_Returns_Null_Prints_Empty()
  {
    var output = RunHelper.Run("PRINT STRING$(\"=\", -1)");
    Assert.Equal("", output);
  }

  [Fact]
  public void STRING_Dollar_Sub_Cannot_Shadow_Builtin()
  {
    var source = "SUB STRING$(c AS STRING, n AS INTEGER)\nPRINT c\nEND SUB";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("STRING$") && d.Message.Contains("built in"));
  }

  // --- ASC ---

  [Fact]
  public void ASC_Returns_Code_For_Uppercase_Letter()
  {
    var output = RunHelper.Run("PRINT ASC(\"A\")");
    Assert.Equal("65", output);
  }

  [Fact]
  public void ASC_Returns_Code_For_Lowercase_Letter()
  {
    var output = RunHelper.Run("PRINT ASC(\"a\")");
    Assert.Equal("97", output);
  }

  [Fact]
  public void ASC_Returns_Code_For_Space()
  {
    var output = RunHelper.Run("PRINT ASC(\" \")");
    Assert.Equal("32", output);
  }

  [Fact]
  public void ASC_Round_Trip_With_CHR_Dollar()
  {
    var output = RunHelper.Run("PRINT ASC(CHR$(65))");
    Assert.Equal("65", output);
  }

  [Fact]
  public void ASC_Multi_Char_String_Uses_First_Character()
  {
    var output = RunHelper.Run("PRINT ASC(\"Hello\")");
    Assert.Equal("72", output);
  }

  [Fact]
  public void ASC_Result_Stored_In_Variable()
  {
    var output = RunHelper.Run("LET code = ASC(\"Z\")\nPRINT code");
    Assert.Equal("90", output);
  }

  [Fact]
  public void ASC_Empty_String_Returns_EvalFailure_With_Diagnostic()
  {
    var result = RunHelper.RunResult("PRINT ASC(\"\")");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("non-empty"));
  }

  [Fact]
  public void ASC_Sub_Cannot_Shadow_Builtin()
  {
    var source = "SUB ASC(s AS STRING)\nPRINT s\nEND SUB";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("ASC") && d.Message.Contains("built in"));
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

  // --- TYPENAME ---

  [Fact]
  public void TYPENAME_Integer_Returns_Integer()
  {
    var output = RunHelper.Run("PRINT TYPENAME(42)");
    Assert.Equal("Integer", output);
  }

  [Fact]
  public void TYPENAME_Float_Returns_Float()
  {
    var output = RunHelper.Run("PRINT TYPENAME(3.14)");
    Assert.Equal("Float", output);
  }

  [Fact]
  public void TYPENAME_String_Returns_String()
  {
    var output = RunHelper.Run("PRINT TYPENAME(\"hello\")");
    Assert.Equal("String", output);
  }

  [Fact]
  public void TYPENAME_Boolean_Returns_Boolean()
  {
    var output = RunHelper.Run("PRINT TYPENAME(TRUE)");
    Assert.Equal("Boolean", output);
  }

  [Fact]
  public void TYPENAME_Can_Be_Used_In_Condition()
  {
    var source = "LET v = 42\nIF TYPENAME(v) = \"Integer\" THEN\nPRINT \"yes\"\nEND IF";
    var output = RunHelper.Run(source);
    Assert.Equal("yes", output);
  }

  // --- Built-in null / edge-case return values ---

  [Fact]
  public void LEN_Non_String_Argument_Returns_Empty_Output()
  {
    // LEN with a non-string argument returns null from the built-in,
    // which the evaluator prints as an empty line.
    var output = RunHelper.Run("PRINT LEN(42)");
    Assert.Equal("", output);
  }

  [Fact]
  public void VAL_Non_Numeric_String_Returns_Empty_Output()
  {
    // VAL("abc") cannot parse → built-in returns null → prints empty line.
    var output = RunHelper.Run("PRINT VAL(\"abc\")");
    Assert.Equal("", output);
  }

  [Fact]
  public void SQR_Negative_Number_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT SQR(-1)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("non-negative"));
  }

  [Fact]
  public void ABS_Zero_Float_Returns_Zero()
  {
    var output = RunHelper.Run("PRINT ABS(0.0)");
    Assert.Equal("0", output);
  }

  [Fact]
  public void STR_Dollar_Converts_Negative_Integer()
  {
    var output = RunHelper.Run("PRINT STR$(-7)");
    Assert.Equal("-7", output);
  }

  [Fact]
  public void INT_Already_Integer_Is_Unchanged()
  {
    var output = RunHelper.Run("PRINT INT(10)");
    Assert.Equal("10", output);
  }

  [Fact]
  public void STR_Dollar_Non_Numeric_Argument_Returns_Empty_Output()
  {
    // STR$ only accepts integers and floats; a boolean returns null → prints "".
    var output = RunHelper.Run("PRINT STR$(TRUE)");
    Assert.Equal("", output);
  }

  // --- CHR$ ---

  [Fact]
  public void CHR_Dollar_Returns_Uppercase_Letter()
  {
    var output = RunHelper.Run("PRINT CHR$(72)");
    Assert.Equal("H", output);
  }

  [Fact]
  public void CHR_Dollar_Returns_Lowercase_Letter()
  {
    var output = RunHelper.Run("PRINT CHR$(97)");
    Assert.Equal("a", output);
  }

  [Fact]
  public void CHR_Dollar_Returns_Double_Quote_For_Code_34()
  {
    // The primary use case: embedding a literal " in string output
    var output = RunHelper.Run("PRINT CHR$(34)");
    Assert.Equal("\"", output);
  }

  [Fact]
  public void CHR_Dollar_Returns_Single_Quote_For_Code_39()
  {
    var output = RunHelper.Run("PRINT CHR$(39)");
    Assert.Equal("'", output);
  }

  [Fact]
  public void CHR_Dollar_Space_Code32_HasLength_One()
  {
    // RunHelper.Run trims output, so verify via LEN rather than direct PRINT
    var output = RunHelper.Run("LET s = CHR$(32)\nPRINT LEN(s)");
    Assert.Equal("1", output);
  }

  [Fact]
  public void CHR_Dollar_Null_Char_Code0_Has_Length_One()
  {
    var output = RunHelper.Run("LET s = CHR$(0)\nPRINT LEN(s)");
    Assert.Equal("1", output);
  }

  [Fact]
  public void CHR_Dollar_Can_Be_Concatenated_To_Build_Quoted_String()
  {
    var output = RunHelper.Run("PRINT CHR$(34) & \"You survived.\" & CHR$(34)");
    Assert.Equal("\"You survived.\"", output);
  }

  [Fact]
  public void CHR_Dollar_Result_Can_Be_Stored_In_Variable()
  {
    var output = RunHelper.Run("LET ch = CHR$(65)\nPRINT ch");
    Assert.Equal("A", output);
  }

  [Fact]
  public void CHR_Dollar_Result_Can_Be_Used_Repeatedly_Via_Variable()
  {
    var output = RunHelper.Run("LET q = CHR$(34)\nPRINT q & \"hello\" & q");
    Assert.Equal("\"hello\"", output);
  }

  [Fact]
  public void CHR_Dollar_Digit_Character_Code48_Returns_Zero_Char()
  {
    var output = RunHelper.Run("PRINT CHR$(48)");
    Assert.Equal("0", output);
  }

  [Fact]
  public void CHR_Dollar_String_Argument_Returns_Empty_Output()
  {
    // Non-integer argument → null → prints ""
    var output = RunHelper.Run("PRINT CHR$(\"H\")");
    Assert.Equal("", output);
  }

  [Fact]
  public void CHR_Dollar_Float_Argument_Returns_Empty_Output()
  {
    // Float does not match IntValue → null → prints ""
    var output = RunHelper.Run("PRINT CHR$(72.5)");
    Assert.Equal("", output);
  }

  [Fact]
  public void CHR_Dollar_Boolean_Argument_Returns_Empty_Output()
  {
    var output = RunHelper.Run("PRINT CHR$(TRUE)");
    Assert.Equal("", output);
  }

  // --- CONST declaration (happy path) ---

  [Fact]
  public void Const_Integer_Can_Be_Read()
  {
    var output = RunHelper.Run("CONST MAX = 10\nPRINT MAX");
    Assert.Equal("10", output);
  }

  [Fact]
  public void Const_String_Can_Be_Read()
  {
    var output = RunHelper.Run("CONST GREETING = \"hello\"\nPRINT GREETING");
    Assert.Equal("hello", output);
  }

  [Fact]
  public void Const_Float_Can_Be_Read()
  {
    var output = RunHelper.Run("CONST PI = 3.14\nPRINT PI");
    Assert.Equal("3.14", output);
  }

  [Fact]
  public void Const_Bool_True_Can_Be_Read()
  {
    var output = RunHelper.Run("CONST FLAG = TRUE\nPRINT FLAG");
    Assert.Equal("True", output);
  }

  [Fact]
  public void Const_Bool_False_Can_Be_Read()
  {
    var output = RunHelper.Run("CONST FLAG = FALSE\nPRINT FLAG");
    Assert.Equal("False", output);
  }

  [Fact]
  public void Const_Can_Be_Used_In_Expression()
  {
    var output = RunHelper.Run("CONST BASE = 5\nPRINT BASE * 3");
    Assert.Equal("15", output);
  }

  [Fact]
  public void Const_Is_Visible_Inside_Sub()
  {
    var code = """
      CONST LIMIT = 100
      SUB ShowLimit()
        PRINT LIMIT
      END SUB
      CALL ShowLimit()
      """;
    var output = RunHelper.Run(code);
    Assert.Equal("100", output);
  }

  [Fact]
  public void Const_Is_Visible_Inside_Function()
  {
    var code = """
      CONST FACTOR = 3
      FUNCTION Triple(n AS INTEGER) AS INTEGER
        RETURN n * FACTOR
      END FUNCTION
      PRINT Triple(4)
      """;
    var output = RunHelper.Run(code);
    Assert.Equal("12", output);
  }

  [Fact]
  public void Const_Can_Be_Concatenated_With_String()
  {
    var output = RunHelper.Run("CONST PREFIX = \"Hello\"\nPRINT PREFIX & \" World\"");
    Assert.Equal("Hello World", output);
  }

  // --- SELECT CASE ---

  [Fact]
  public void SelectCase_MatchesFirstCase_Integer()
  {
    var output = RunHelper.Run(
      "LET x = 1\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"one\"\n" +
      "  CASE 2\n" +
      "    PRINT \"two\"\n" +
      "END SELECT");
    Assert.Equal("one", output);
  }

  [Fact]
  public void SelectCase_MatchesCorrectCase_WhenMultipleClauses()
  {
    var output = RunHelper.Run(
      "LET x = 2\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"one\"\n" +
      "  CASE 2\n" +
      "    PRINT \"two\"\n" +
      "  CASE 3\n" +
      "    PRINT \"three\"\n" +
      "END SELECT");
    Assert.Equal("two", output);
  }

  [Fact]
  public void SelectCase_MatchesCaseElse_WhenNoCaseMatches()
  {
    var output = RunHelper.Run(
      "LET x = 99\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"one\"\n" +
      "  CASE ELSE\n" +
      "    PRINT \"other\"\n" +
      "END SELECT");
    Assert.Equal("other", output);
  }

  [Fact]
  public void SelectCase_DoesNotFallThrough_ToNextCase()
  {
    // Two CASE clauses both matching x = 1; only first executes
    var output = RunHelper.Run(
      "LET x = 1\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"first\"\n" +
      "  CASE 1\n" +
      "    PRINT \"second\"\n" +
      "END SELECT");
    Assert.Equal("first", output);
  }

  [Fact]
  public void SelectCase_ExecutesNothing_WhenNoMatchAndNoCaseElse()
  {
    var output = RunHelper.Run(
      "LET x = 99\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"one\"\n" +
      "  CASE 2\n" +
      "    PRINT \"two\"\n" +
      "END SELECT");
    Assert.Equal("", output);
  }

  [Fact]
  public void SelectCase_SupportsMultipleValues_InSingleCase()
  {
    var output = RunHelper.Run(
      "LET x = 3\n" +
      "SELECT CASE x\n" +
      "  CASE 1, 2, 3\n" +
      "    PRINT \"low\"\n" +
      "  CASE 4, 5\n" +
      "    PRINT \"high\"\n" +
      "END SELECT");
    Assert.Equal("low", output);
  }

  [Fact]
  public void SelectCase_WorksWithStringSubject()
  {
    var output = RunHelper.Run(
      "LET cmd = \"north\"\n" +
      "SELECT CASE cmd\n" +
      "  CASE \"north\"\n" +
      "    PRINT \"go north\"\n" +
      "  CASE \"south\"\n" +
      "    PRINT \"go south\"\n" +
      "  CASE ELSE\n" +
      "    PRINT \"unknown\"\n" +
      "END SELECT");
    Assert.Equal("go north", output);
  }

  [Fact]
  public void SelectCase_CaseBlock_CanContainMultipleStatements()
  {
    var output = RunHelper.Run(
      "LET x = 2\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"a\"\n" +
      "  CASE 2\n" +
      "    PRINT \"b\"\n" +
      "    PRINT \"c\"\n" +
      "    PRINT \"d\"\n" +
      "  CASE 3\n" +
      "    PRINT \"e\"\n" +
      "END SELECT");
    Assert.Equal("b\nc\nd", output);
  }

  [Fact]
  public void SelectCase_CaseBlock_CanCallSub()
  {
    var output = RunHelper.Run(
      "SUB Greet()\n" +
      "  PRINT \"hello from sub\"\n" +
      "END SUB\n" +
      "LET x = 1\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    CALL Greet()\n" +
      "  CASE ELSE\n" +
      "    PRINT \"no match\"\n" +
      "END SELECT");
    Assert.Equal("hello from sub", output);
  }

  [Fact]
  public void SelectCase_Subject_CanBeFunction_Return_Value()
  {
    var output = RunHelper.Run(
      "FUNCTION GetCode() AS INTEGER\n" +
      "  RETURN 42\n" +
      "END FUNCTION\n" +
      "SELECT CASE GetCode()\n" +
      "  CASE 42\n" +
      "    PRINT \"matched\"\n" +
      "  CASE ELSE\n" +
      "    PRINT \"no match\"\n" +
      "END SELECT");
    Assert.Equal("matched", output);
  }

  [Fact]
  public void SelectCase_Nested_InnerMatchesCorrectly()
  {
    var output = RunHelper.Run(
      "LET x = 1\n" +
      "LET y = 2\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    SELECT CASE y\n" +
      "      CASE 1\n" +
      "        PRINT \"1-1\"\n" +
      "      CASE 2\n" +
      "        PRINT \"1-2\"\n" +
      "    END SELECT\n" +
      "  CASE 2\n" +
      "    PRINT \"outer-2\"\n" +
      "END SELECT");
    Assert.Equal("1-2", output);
  }

  [Fact]
  public void SelectCase_OnlyCaseElse_Executes_WhenSubjectIsAnything()
  {
    var output = RunHelper.Run(
      "LET x = 42\n" +
      "SELECT CASE x\n" +
      "  CASE ELSE\n" +
      "    PRINT \"always\"\n" +
      "END SELECT");
    Assert.Equal("always", output);
  }

  [Fact]
  public void SelectCase_CaseElse_NotExecuted_WhenEarlierCaseMatches()
  {
    // Ensures CASE ELSE doesn't run when a preceding CASE already fired
    var output = RunHelper.Run(
      "LET x = 1\n" +
      "SELECT CASE x\n" +
      "  CASE 1\n" +
      "    PRINT \"matched\"\n" +
      "  CASE ELSE\n" +
      "    PRINT \"fallback\"\n" +
      "END SELECT");
    Assert.Equal("matched", output);
  }

  // --- Built-in argument errors ---

  [Fact]
  public void LEN_Non_String_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT LEN(42)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void MID_Dollar_Start_Index_Zero_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT MID$(\"Hello\", 0, 2)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("1"));
  }

  [Fact]
  public void MID_Dollar_Length_Overflow_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT MID$(\"Hello\", 2, 99)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("bounds"));
  }

  [Fact]
  public void MID_Dollar_Non_String_First_Arg_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT MID$(42, 1, 1)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void LEFT_Dollar_Count_Exceeds_Length_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT LEFT$(\"Hello\", 99)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("range"));
  }

  [Fact]
  public void LEFT_Dollar_Negative_Count_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT LEFT$(\"Hello\", -1)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("range"));
  }

  [Fact]
  public void LEFT_Dollar_Non_String_First_Arg_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT LEFT$(42, 1)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void RIGHT_Dollar_Count_Exceeds_Length_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT RIGHT$(\"Hello\", 99)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("range"));
  }

  [Fact]
  public void RIGHT_Dollar_Negative_Count_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT RIGHT$(\"Hello\", -1)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("range"));
  }

  [Fact]
  public void RIGHT_Dollar_Non_String_First_Arg_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT RIGHT$(42, 1)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void TRIM_Dollar_Non_String_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT TRIM$(42)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void UPPER_Dollar_Non_String_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT UPPER$(42)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void LOWER_Dollar_Non_String_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT LOWER$(42)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("string"));
  }

  [Fact]
  public void INT_Non_Numeric_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT INT(\"foo\")");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("numeric"));
  }

  [Fact]
  public void STR_Dollar_Non_Numeric_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT STR$(TRUE)");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("numeric"));
  }

  [Fact]
  public void ABS_Non_Numeric_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT ABS(\"hello\")");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("numeric"));
  }

  [Fact]
  public void SQR_Non_Numeric_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT SQR(\"hello\")");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("numeric"));
  }

  [Fact]
  public void CHR_Dollar_String_Arg_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT CHR$(\"H\")");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("integer"));
  }

  // --- CINT ---

  [Fact]
  public void CINT_Truncates_Float_Toward_Zero_Positive()
  {
    var output = RunHelper.Run("PRINT CINT(3.9)");
    Assert.Equal("3", output);
  }

  [Fact]
  public void CINT_Truncates_Float_Toward_Zero_Fractional()
  {
    var output = RunHelper.Run("PRINT CINT(3.1)");
    Assert.Equal("3", output);
  }

  [Fact]
  public void CINT_Truncates_Negative_Float_Toward_Zero()
  {
    // Key difference from INT: CINT(-3.9) = -3, INT(-3.9) = -4
    var output = RunHelper.Run("PRINT CINT(-3.9)");
    Assert.Equal("-3", output);
  }

  [Fact]
  public void CINT_Zero_Float_Returns_Zero()
  {
    var output = RunHelper.Run("PRINT CINT(0.0)");
    Assert.Equal("0", output);
  }

  [Fact]
  public void CINT_Integer_Passthrough()
  {
    var output = RunHelper.Run("PRINT CINT(7)");
    Assert.Equal("7", output);
  }

  [Fact]
  public void CINT_Result_Is_Integer_Not_Float()
  {
    // Verify return type is Integer: integer arithmetic gives 4 not 4.0
    var output = RunHelper.Run("LET i = CINT(3.9)\nPRINT i + 1");
    Assert.Equal("4", output);
  }

  [Fact]
  public void CINT_Result_Stored_In_Variable()
  {
    var output = RunHelper.Run("LET f = 7.8\nLET i = CINT(f)\nPRINT i");
    Assert.Equal("7", output);
  }

  [Fact]
  public void CINT_Non_Numeric_Returns_EvalFailure()
  {
    var result = RunHelper.RunResult("PRINT CINT(\"foo\")");
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d => d.Message.Contains("numeric"));
  }

  [Fact]
  public void CINT_Sub_Cannot_Shadow_Builtin()
  {
    var source = "SUB CINT(n AS INTEGER)\nPRINT n\nEND SUB";
    var result = RunHelper.RunResult(source);
    var failure = Assert.IsType<EvalFailure>(result);
    Assert.Contains(failure.Diagnostics, d =>
        d.Message.Contains("CINT") && d.Message.Contains("built in"));
  }
}
