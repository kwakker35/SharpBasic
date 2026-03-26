using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Parsing.Tests;

/// <summary>
/// Tests for parser error paths: malformed constructs that must produce ParseFailure.
/// </summary>
public class ParserErrorTests
{
    // --- FOR / NEXT mismatch ---

    [Fact]
    public void Parser_For_Next_Mismatched_Variable_Produces_ParseFailure()
    {
        // FOR i = 1 TO 5
        //     PRINT i
        // NEXT j   ← wrong variable name
        var tokens = new List<Token>
        {
            new(TokenType.For,         "", 1, 1),
            new(TokenType.Identifier,  "i", 1, 5),
            new(TokenType.Eq,          "", 1, 7),
            new(TokenType.IntLiteral,  "1", 1, 9),
            new(TokenType.To,          "", 1, 11),
            new(TokenType.IntLiteral,  "5", 1, 14),
            new(TokenType.NewLine,     "", 1, 15),
            new(TokenType.Print,       "", 2, 1),
            new(TokenType.Identifier,  "i", 2, 7),
            new(TokenType.NewLine,     "", 2, 8),
            new(TokenType.Next,        "", 3, 1),
            new(TokenType.Identifier,  "j", 3, 6),  // mismatch: i vs j
            new(TokenType.Eof,         "", 3, 7),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    // --- END followed by wrong keyword ---

    [Fact]
    public void Parser_If_Closed_With_End_Sub_Produces_ParseFailure()
    {
        // IF 1 = 1 THEN
        //     PRINT "yes"
        // END SUB   ← should be END IF
        var tokens = new List<Token>
        {
            new(TokenType.If,          "", 1, 1),
            new(TokenType.IntLiteral,  "1", 1, 4),
            new(TokenType.Eq,          "", 1, 6),
            new(TokenType.IntLiteral,  "1", 1, 8),
            new(TokenType.Then,        "", 1, 10),
            new(TokenType.NewLine,     "", 1, 14),
            new(TokenType.Print,       "", 2, 1),
            new(TokenType.StringLiteral, "yes", 2, 7),
            new(TokenType.NewLine,     "", 2, 12),
            new(TokenType.End,         "", 3, 1),
            new(TokenType.Sub,         "", 3, 5),  // END SUB instead of END IF
            new(TokenType.Eof,         "", 3, 8),
        };

        var result = new Parser(tokens).Parse();
        Assert.IsType<ParseFailure>(result);
    }

    [Fact]
    public void Parser_Sub_Closed_With_End_Function_Produces_ParseFailure()
    {
        // SUB Greet()
        //     PRINT "Hi"
        // END FUNCTION   ← should be END SUB
        var tokens = new List<Token>
        {
            new(TokenType.Sub,         "", 1, 1),
            new(TokenType.Identifier,  "Greet", 1, 5),
            new(TokenType.LParen,      "", 1, 10),
            new(TokenType.RParen,      "", 1, 11),
            new(TokenType.NewLine,     "", 1, 12),
            new(TokenType.Print,       "", 2, 1),
            new(TokenType.StringLiteral, "Hi", 2, 7),
            new(TokenType.NewLine,     "", 2, 11),
            new(TokenType.End,         "", 3, 1),
            new(TokenType.Function,    "", 3, 5),  // END FUNCTION instead of END SUB
            new(TokenType.Eof,         "", 3, 13),
        };

        var result = new Parser(tokens).Parse();
        Assert.IsType<ParseFailure>(result);
    }

    [Fact]
    public void Parser_Function_Closed_With_End_Sub_Produces_ParseFailure()
    {
        // FUNCTION Add(a As Integer) As Integer
        //     RETURN 1
        // END SUB   ← should be END FUNCTION
        var tokens = new List<Token>
        {
            new(TokenType.Function,    "", 1, 1),
            new(TokenType.Identifier,  "Add", 1, 10),
            new(TokenType.LParen,      "", 1, 13),
            new(TokenType.Identifier,  "a", 1, 14),
            new(TokenType.As,          "", 1, 16),
            new(TokenType.Integer,     "", 1, 19),
            new(TokenType.RParen,      "", 1, 26),
            new(TokenType.As,          "", 1, 28),
            new(TokenType.Integer,     "", 1, 31),
            new(TokenType.NewLine,     "", 1, 38),
            new(TokenType.Return,      "", 2, 1),
            new(TokenType.IntLiteral,  "1", 2, 8),
            new(TokenType.NewLine,     "", 2, 9),
            new(TokenType.End,         "", 3, 1),
            new(TokenType.Sub,         "", 3, 5),  // END SUB instead of END FUNCTION
            new(TokenType.Eof,         "", 3, 8),
        };

        var result = new Parser(tokens).Parse();
        Assert.IsType<ParseFailure>(result);
    }

    // --- Unsupported parameter type ---

    [Fact]
    public void Parser_Sub_With_Unsupported_Param_Type_Produces_ParseFailure()
    {
        // SUB Bad(x As Foo)
        // END SUB
        // 'Foo' is not a valid type keyword — it lexes as Identifier
        var tokens = new List<Token>
        {
            new(TokenType.Sub,         "", 1, 1),
            new(TokenType.Identifier,  "Bad", 1, 5),
            new(TokenType.LParen,      "", 1, 8),
            new(TokenType.Identifier,  "x", 1, 9),
            new(TokenType.As,          "", 1, 11),
            new(TokenType.Identifier,  "Foo", 1, 14), // unsupported type
            new(TokenType.RParen,      "", 1, 17),
            new(TokenType.NewLine,     "", 1, 18),
            new(TokenType.End,         "", 2, 1),
            new(TokenType.Sub,         "", 2, 5),
            new(TokenType.Eof,         "", 2, 8),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.Contains(failure.Diagnostics, d =>
            d.Severity == DiagnosticSeverity.Error &&
            d.Message.Contains("Expected TYPE after AS"));
    }

    [Fact]
    public void Parser_Function_With_Unsupported_Param_Type_Produces_ParseFailure()
    {
        // FUNCTION Bad(x As Foo) As Integer
        // END FUNCTION
        var tokens = new List<Token>
        {
            new(TokenType.Function,    "", 1, 1),
            new(TokenType.Identifier,  "Bad", 1, 10),
            new(TokenType.LParen,      "", 1, 13),
            new(TokenType.Identifier,  "x", 1, 14),
            new(TokenType.As,          "", 1, 16),
            new(TokenType.Identifier,  "Foo", 1, 19), // unsupported type
            new(TokenType.RParen,      "", 1, 22),
            new(TokenType.As,          "", 1, 24),
            new(TokenType.Integer,     "", 1, 27),
            new(TokenType.Eof,         "", 1, 34),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.Contains(failure.Diagnostics, d =>
            d.Severity == DiagnosticSeverity.Error &&
            d.Message.Contains("Expected TYPE after AS"));
    }

    [Fact]
    public void Parser_Function_With_Unsupported_Return_Type_Produces_ParseFailure()
    {
        // FUNCTION Bad() As Foo
        // END FUNCTION
        var tokens = new List<Token>
        {
            new(TokenType.Function,    "", 1, 1),
            new(TokenType.Identifier,  "Bad", 1, 10),
            new(TokenType.LParen,      "", 1, 13),
            new(TokenType.RParen,      "", 1, 14),
            new(TokenType.As,          "", 1, 16),
            new(TokenType.Identifier,  "Foo", 1, 19), // unsupported return type
            new(TokenType.NewLine,     "", 1, 22),
            new(TokenType.End,         "", 2, 1),
            new(TokenType.Function,    "", 2, 5),
            new(TokenType.Eof,         "", 2, 13),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.Contains(failure.Diagnostics, d =>
            d.Severity == DiagnosticSeverity.Error &&
            d.Message.Contains("Expected"));
    }

    // --- DIM with float size — parser now accepts any expression, evaluator rejects non-integers ---
    // (This test is intentionally removed. Float bounds now parse successfully and are
    //  rejected at evaluation time with a clear EvalFailure. See EvaluatorTests for coverage.)

    // --- Unexpected token at statement level ---

    [Fact]
    public void Parser_Unexpected_Token_At_Statement_Level_Produces_ParseFailure()
    {
        // A bare operator token at the start of a statement is not valid
        var tokens = new List<Token>
        {
            new(TokenType.Plus, "+", 1, 1),
            new(TokenType.Eof,  "", 1, 2),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    // --- CONST declaration errors ---

    [Fact]
    public void Parser_Const_Inside_Sub_Produces_ParseFailure()
    {
        // CONST inside a SUB body must be rejected at parse time
        // SUB Greet()
        //   CONST X = 5
        // END SUB
        var tokens = new List<Token>
    {
      new(TokenType.Sub,         "",      1, 1),
      new(TokenType.Identifier,  "Greet", 1, 5),
      new(TokenType.LParen,      "",      1, 10),
      new(TokenType.RParen,      "",      1, 11),
      new(TokenType.NewLine,     "",      1, 12),
      new(TokenType.Const,       "",      2, 3),
      new(TokenType.Identifier,  "X",     2, 9),
      new(TokenType.Eq,          "",      2, 11),
      new(TokenType.IntLiteral,  "5",     2, 13),
      new(TokenType.NewLine,     "",      2, 14),
      new(TokenType.End,         "",      3, 1),
      new(TokenType.Sub,         "",      3, 5),
      new(TokenType.Eof,         "",      3, 8),
    };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_Const_Inside_Function_Produces_ParseFailure()
    {
        // FUNCTION Double(n AS INTEGER) AS INTEGER
        //   CONST Scale = 2
        // END FUNCTION
        var tokens = new List<Token>
    {
      new(TokenType.Function,    "",        1, 1),
      new(TokenType.Identifier,  "Double",  1, 10),
      new(TokenType.LParen,      "",        1, 16),
      new(TokenType.Identifier,  "n",       1, 17),
      new(TokenType.As,          "",        1, 19),
      new(TokenType.Integer,     "",        1, 22),
      new(TokenType.RParen,      "",        1, 29),
      new(TokenType.As,          "",        1, 31),
      new(TokenType.Integer,     "",        1, 34),
      new(TokenType.NewLine,     "",        1, 41),
      new(TokenType.Const,       "",        2, 3),
      new(TokenType.Identifier,  "Scale",   2, 9),
      new(TokenType.Eq,          "",        2, 15),
      new(TokenType.IntLiteral,  "2",       2, 17),
      new(TokenType.NewLine,     "",        2, 18),
      new(TokenType.End,         "",        3, 1),
      new(TokenType.Function,    "",        3, 5),
      new(TokenType.Eof,         "",        3, 13),
    };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_Const_With_Missing_Identifier_Produces_ParseFailure()
    {
        // CONST = 5   ← missing name
        var tokens = new List<Token>
    {
      new(TokenType.Const,      "",  1, 1),
      new(TokenType.Eq,         "",  1, 7),
      new(TokenType.IntLiteral, "5", 1, 9),
      new(TokenType.Eof,        "",  1, 10),
    };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_Const_With_Missing_Equals_Produces_ParseFailure()
    {
        // CONST X 5   ← missing =
        var tokens = new List<Token>
    {
      new(TokenType.Const,      "",  1, 1),
      new(TokenType.Identifier, "X", 1, 7),
      new(TokenType.IntLiteral, "5", 1, 9),
      new(TokenType.Eof,        "",  1, 10),
    };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_Const_With_Expression_Rhs_Produces_ParseFailure()
    {
        // CONST X = 1 + 2   ← only literals allowed on RHS
        var tokens = new List<Token>
    {
      new(TokenType.Const,      "",  1, 1),
      new(TokenType.Identifier, "X", 1, 7),
      new(TokenType.Eq,         "",  1, 9),
      new(TokenType.IntLiteral, "1", 1, 11),
      new(TokenType.Plus,       "+", 1, 13),
      new(TokenType.IntLiteral, "2", 1, 15),
      new(TokenType.Eof,        "",  1, 16),
    };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    // --- SELECT CASE errors ---

    [Fact]
    public void Parser_Select_Without_Case_Keyword_Produces_ParseFailure()
    {
        // SELECT x   ← missing CASE between SELECT and subject
        var tokens = new List<Token>
        {
            new(TokenType.Select,     "",  1, 1),
            new(TokenType.Identifier, "x", 1, 8),
            new(TokenType.Eof,        "",  1, 9),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_SelectCase_Missing_EndSelect_Produces_ParseFailure()
    {
        // SELECT CASE x
        //   CASE 1
        //     PRINT "one"
        // ← no END SELECT
        var tokens = new List<Token>
        {
            new(TokenType.Select,      "",    1, 1),
            new(TokenType.Case,        "",    1, 8),
            new(TokenType.Identifier,  "x",   1, 13),
            new(TokenType.NewLine,     "",    1, 14),
            new(TokenType.Case,        "",    2, 3),
            new(TokenType.IntLiteral,  "1",   2, 8),
            new(TokenType.NewLine,     "",    2, 9),
            new(TokenType.Print,       "",    3, 5),
            new(TokenType.StringLiteral, "one", 3, 11),
            new(TokenType.NewLine,     "",    3, 16),
            new(TokenType.Eof,         "",    4, 1),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_SelectCase_CaseElse_Not_Last_Produces_ParseFailure()
    {
        // SELECT CASE x
        //   CASE ELSE
        //     PRINT "other"
        //   CASE 1           ← CASE after CASE ELSE is illegal
        //     PRINT "one"
        // END SELECT
        var tokens = new List<Token>
        {
            new(TokenType.Select,      "",    1, 1),
            new(TokenType.Case,        "",    1, 8),
            new(TokenType.Identifier,  "x",   1, 13),
            new(TokenType.NewLine,     "",    1, 14),
            new(TokenType.Case,        "",    2, 3),
            new(TokenType.Else,        "",    2, 8),
            new(TokenType.NewLine,     "",    2, 13),
            new(TokenType.Print,       "",    3, 5),
            new(TokenType.StringLiteral, "other", 3, 11),
            new(TokenType.NewLine,     "",    3, 18),
            new(TokenType.Case,        "",    4, 3),
            new(TokenType.IntLiteral,  "1",   4, 8),
            new(TokenType.NewLine,     "",    4, 9),
            new(TokenType.Print,       "",    5, 5),
            new(TokenType.StringLiteral, "one", 5, 11),
            new(TokenType.NewLine,     "",    5, 16),
            new(TokenType.End,         "",    6, 1),
            new(TokenType.Select,      "",    6, 5),
            new(TokenType.Eof,         "",    6, 11),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    // --- SET GLOBAL errors ---

    [Fact]
    public void Parser_Set_Without_Global_Produces_ParseFailure()
    {
        // SET x = 42  ← missing GLOBAL keyword
        var tokens = new List<Token>
        {
            new(TokenType.Set,        "",   1, 1),
            new(TokenType.Identifier, "x",  1, 5),
            new(TokenType.Eq,         "",   1, 7),
            new(TokenType.IntLiteral, "42", 1, 9),
            new(TokenType.Eof,        "",   1, 11),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_SetGlobal_Missing_Identifier_Produces_ParseFailure()
    {
        // SET GLOBAL = 42  ← missing identifier
        var tokens = new List<Token>
        {
            new(TokenType.Set,        "",   1, 1),
            new(TokenType.Global,     "",   1, 5),
            new(TokenType.Eq,         "",   1, 12),
            new(TokenType.IntLiteral, "42", 1, 14),
            new(TokenType.Eof,        "",   1, 16),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_SetGlobal_Missing_Eq_Produces_ParseFailure()
    {
        // SET GLOBAL x 42  ← missing =
        var tokens = new List<Token>
        {
            new(TokenType.Set,        "",   1, 1),
            new(TokenType.Global,     "",   1, 5),
            new(TokenType.Identifier, "x",  1, 12),
            new(TokenType.IntLiteral, "42", 1, 14),
            new(TokenType.Eof,        "",   1, 16),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    // --- SLEEP parse errors ---

    [Fact]
    public void Parser_Sleep_Missing_LParen_Produces_ParseFailure()
    {
        // SLEEP 100  ← missing ( before argument
        var tokens = new List<Token>
        {
            new(TokenType.Sleep,      "",    1, 1),
            new(TokenType.IntLiteral, "100", 1, 7),
            new(TokenType.Eof,        "",    1, 10),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_Sleep_Empty_Parens_Produces_ParseFailure()
    {
        // SLEEP()  ← missing expression inside
        var tokens = new List<Token>
        {
            new(TokenType.Sleep,  "", 1, 1),
            new(TokenType.LParen, "", 1, 6),
            new(TokenType.RParen, "", 1, 7),
            new(TokenType.Eof,    "", 1, 8),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    [Fact]
    public void Parser_Sleep_Missing_RParen_Produces_ParseFailure()
    {
        // SLEEP(100  ← no closing )
        var tokens = new List<Token>
        {
            new(TokenType.Sleep,      "",    1, 1),
            new(TokenType.LParen,     "",    1, 6),
            new(TokenType.IntLiteral, "100", 1, 7),
            new(TokenType.Eof,        "",    1, 10),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.NotEmpty(failure.Diagnostics);
        Assert.Equal(DiagnosticSeverity.Error, failure.Diagnostics[0].Severity);
    }

    // --- DIM type errors ---

    [Fact]
    public void Parser_Dim_InvalidType_1D_Produces_ParseFailure()
    {
        // DIM x[5] AS Foo  ← Foo is not a valid type keyword
        var tokens = new List<Token>
        {
            new(TokenType.Dim,        "",    1, 1),
            new(TokenType.Identifier, "x",   1, 5),
            new(TokenType.LBracket,   "",    1, 6),
            new(TokenType.IntLiteral, "5",   1, 7),
            new(TokenType.RBracket,   "",    1, 8),
            new(TokenType.As,         "",    1, 10),
            new(TokenType.Identifier, "Foo", 1, 13),
            new(TokenType.Eof,        "",    1, 16),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.Contains(failure.Diagnostics, d =>
            d.Severity == DiagnosticSeverity.Error &&
            d.Message.Contains("Expected TYPE after AS"));
    }

    [Fact]
    public void Parser_Dim_InvalidType_2D_Produces_ParseFailure()
    {
        // DIM m[2][3] AS Foo  ← Foo is not a valid type keyword
        var tokens = new List<Token>
        {
            new(TokenType.Dim,        "",    1, 1),
            new(TokenType.Identifier, "m",   1, 5),
            new(TokenType.LBracket,   "",    1, 6),
            new(TokenType.IntLiteral, "2",   1, 7),
            new(TokenType.RBracket,   "",    1, 8),
            new(TokenType.LBracket,   "",    1, 9),
            new(TokenType.IntLiteral, "3",   1, 10),
            new(TokenType.RBracket,   "",    1, 11),
            new(TokenType.As,         "",    1, 13),
            new(TokenType.Identifier, "Foo", 1, 16),
            new(TokenType.Eof,        "",    1, 19),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.Contains(failure.Diagnostics, d =>
            d.Severity == DiagnosticSeverity.Error &&
            d.Message.Contains("Expected TYPE after AS"));
    }

    // --- INPUT parse errors ---

    [Fact]
    public void Parser_Input_Literal_Instead_Of_Variable_Produces_ParseFailure()
    {
        // INPUT 42  ← a literal is not a valid assignment target for INPUT
        var tokens = new List<Token>
        {
            new(TokenType.Input,      "",   1, 1),
            new(TokenType.IntLiteral, "42", 1, 7),
            new(TokenType.Eof,        "",   1, 9),
        };

        var result = new Parser(tokens).Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        Assert.Contains(failure.Diagnostics, d =>
            d.Severity == DiagnosticSeverity.Error &&
            d.Message.Contains("INDENTIFIER"));
    }
}
