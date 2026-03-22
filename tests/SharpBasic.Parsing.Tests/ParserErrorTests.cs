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

  // --- DIM with float size (non-integer) ---

  [Fact]
  public void Parser_Dim_With_Float_Literal_Size_Produces_ParseFailure()
  {
    // DIM scores[3.5] As Integer   ← size must be an integer literal
    var tokens = new List<Token>
        {
            new(TokenType.Dim,          "", 1, 1),
            new(TokenType.Identifier,   "scores", 1, 5),
            new(TokenType.LBracket,     "", 1, 11),
            new(TokenType.FloatLiteral, "3.5", 1, 12), // float, not int
            new(TokenType.RBracket,     "", 1, 15),
            new(TokenType.As,           "", 1, 17),
            new(TokenType.Integer,      "", 1, 20),
            new(TokenType.Eof,          "", 1, 27),
        };

    var result = new Parser(tokens).Parse();
    Assert.IsType<ParseFailure>(result);
  }

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
}
