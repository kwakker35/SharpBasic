using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Helpers;
using SharpBasic.Ast;
using SharpBasic.Lexing;
using Xunit;

namespace SharpBasic.Lexing.Tests;

public class LexerTests
{

    [Fact]
    public void Lex_PrintKeyword_ReturnsCorrectToken()
    {
        //Arange
        var input = "PRINT";
        var lexer = new Lexer(input);

        //Act
        var token = lexer.Tokenise();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(TokenType.Print, token[0].Type);
    }

    [Fact]
    public void Lex_Print_ReturnsCorrectTokens_When_Space_Is_Missing()
    {
        //Arange
        var input = "PRINT\"hello\"";
        var lexer = new Lexer(input);

        //Act
        var tokens = lexer.Tokenise();

        //Assert
        Assert.NotNull(tokens);
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[1].Type);
        Assert.Equal(TokenType.Eof, tokens[2].Type);
    }

    [Fact]
    public void Lex_LetKeyword_ReturnsCorrectToken()
    {
        //Arange
        var input = "LET";
        var lexer = new Lexer(input);

        //Act
        var token = lexer.Tokenise();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(TokenType.Let, token[0].Type);
    }

    [Fact]
    public void Lex_EqKeyword_ReturnsCorrectToken()
    {
        //Arange
        var input = "=";
        var lexer = new Lexer(input);

        //Act
        var token = lexer.Tokenise();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(TokenType.Eq, token[0].Type);
    }

    [Fact]
    public void Lex_Identifier_ReturnsCorrectToken()
    {
        //Arange
        var input = "x";
        var lexer = new Lexer(input);

        //Act
        var token = lexer.Tokenise();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(TokenType.Identifier, token[0].Type);
        Assert.Equal("x", token[0].Value);
    }

    [Fact]
    public void Lex_Full_Let_Sequence_ReturnsCorrectTokens()
    {
        //Arange
        var input = "LET x = \"Alice\"";
        var lexer = new Lexer(input);

        //Act
        var tokens = lexer.Tokenise();

        //Assert
        Assert.NotNull(tokens);
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
        Assert.Equal(TokenType.Eq, tokens[2].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[3].Type);
        Assert.Equal("Alice", tokens[3].Value);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void Lex_NonKeyword_Word_ReturnsIdentifierToken()
    {
        //Arange
        var input = "WIBBLE";
        var lexer = new Lexer(input);

        //Act
        var token = lexer.Tokenise();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(TokenType.Identifier, token[0].Type);
    }

    [Fact]
    public void Lex_UnrecognisedCharacter_ReturnsUnknownToken()
    {
        //Arange
        var input = "@";
        var lexer = new Lexer(input);

        //Act
        var token = lexer.Tokenise();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(TokenType.Unknown, token[0].Type);
    }

    [Fact]
    public void Lex_StringLiteral_ReturnCorrectTokenAndValue()
    {
        var input = "\"Hello, World!\"";
        var lexer = new Lexer(input);

        var token = lexer.Tokenise();

        Assert.NotNull(token);
        Assert.Equal(TokenType.StringLiteral, token[0].Type);
        Assert.Equal("Hello, World!", token[0].Value);
    }

    [Fact]
    public void Lex_NewLine_Emmited()
    {
        var input = "PRINT\nPRINT";
        var lexer = new Lexer(input);

        var tokens = lexer.Tokenise();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.NewLine, tokens[1].Type);
        Assert.Equal(TokenType.Print, tokens[2].Type);
        Assert.Equal(TokenType.Eof, tokens[3].Type);
    }

    [Fact]
    public void Lex_Eof_IsLastToken()
    {
        var input = "PRINT";
        var lexer = new Lexer(input);

        var tokens = lexer.Tokenise();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Fact]
    public void Lex_Mulitple_Spaces_Skipped()
    {
        var input = "PRINT  ##";

        var lexer = new Lexer(input);

        var tokens = lexer.Tokenise();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.Unknown, tokens[1].Type);
        Assert.Equal(TokenType.Eof, tokens[2].Type);
    }

    [Fact]
    public void Lex_Integer_Returns_Correct_Tokens_And_Value()
    {
        var input = "42";

        var lexer = new Lexer(input);
        var tokens = lexer.Tokenise();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.IntLiteral, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Fact]
    public void Lex_Float_Returns_Correct_Tokens_And_Value()
    {
        var input = "3.14";

        var lexer = new Lexer(input);
        var tokens = lexer.Tokenise();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.FloatLiteral, tokens[0].Type);
        Assert.Equal("3.14", tokens[0].Value);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Fact]
    public void Lex_Simple_Sum_Returns_Correct_Tokens_And_Value()
    {
        var input = "1 + 2";

        var lexer = new Lexer(input);
        var tokens = lexer.Tokenise();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.IntLiteral, tokens[0].Type);
        Assert.Equal("1", tokens[0].Value);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[2].Type);
        Assert.Equal("2", tokens[2].Value);
        Assert.Equal(TokenType.Eof, tokens[3].Type);
    }

    [Fact]
    public void Lex_Bracketed_Simple_Sum_Returns_Correct_Tokens_And_Value()
    {
        var input = "(10 * 3)";

        var lexer = new Lexer(input);
        var tokens = lexer.Tokenise();

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.LParen, tokens[0].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[1].Type);
        Assert.Equal("10", tokens[1].Value);
        Assert.Equal(TokenType.Multiply, tokens[2].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[3].Type);
        Assert.Equal("3", tokens[3].Value);
        Assert.Equal(TokenType.RParen, tokens[4].Type);
        Assert.Equal(TokenType.Eof, tokens[5].Type);
    }

    [Theory]
    [InlineData("IF", TokenType.If)]
    [InlineData("THEN", TokenType.Then)]
    [InlineData("ELSE", TokenType.Else)]
    [InlineData("END", TokenType.End)]
    [InlineData("=", TokenType.Eq)]
    [InlineData("<", TokenType.Lt)]
    [InlineData(">", TokenType.Gt)]
    [InlineData("WHILE", TokenType.While)]
    [InlineData("WEND", TokenType.Wend)]
    [InlineData("FOR", TokenType.For)]
    [InlineData("TO", TokenType.To)]
    [InlineData("STEP", TokenType.Step)]
    [InlineData("NEXT", TokenType.Next)]
    [InlineData("FUNCTION", TokenType.Function)]
    [InlineData("SUB", TokenType.Sub)]
    [InlineData("RETURN", TokenType.Return)]
    [InlineData("CALL", TokenType.Call)]
    [InlineData("AS", TokenType.As)]
    [InlineData("AND", TokenType.And)]
    [InlineData("OR", TokenType.Or)]
    [InlineData("NOT", TokenType.Not)]
    [InlineData("INTEGER", TokenType.Integer)]
    [InlineData("FLOAT", TokenType.Float)]
    [InlineData("STRING", TokenType.String)]
    [InlineData("BOOLEAN", TokenType.Boolean)]
    [InlineData("TRUE", TokenType.True)]
    [InlineData("FALSE", TokenType.False)]
    [InlineData(",", TokenType.Comma)]
    public void Lexer_Tokenises_Single_Token(string input, TokenType expected)
    {
        var tokens = new Lexer(input).Tokenise();
        Assert.Equal(expected, tokens[0].Type);
    }

    [Theory]
    [InlineData("<>", TokenType.NotEq)]
    [InlineData("<=", TokenType.LtEq)]
    [InlineData(">=", TokenType.GtEq)]
    public void Lexer_Tokenises_Double_Token(string input, TokenType expected)
    {
        var tokens = new Lexer(input).Tokenise();
        Assert.Equal(expected, tokens[0].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Multiple_Token_If_Correctly()
    {
        var input = "IF X > 5 THEN";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.If, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("X", tokens[1].Value);
        Assert.Equal(TokenType.Gt, tokens[2].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[3].Type);
        Assert.Equal("5", tokens[3].Value);
        Assert.Equal(TokenType.Then, tokens[4].Type);
        Assert.Equal(TokenType.Eof, tokens[5].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Ampersand_Correctly()
    {
        var input = "\"hello\" & \"world\"";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.StringLiteral, tokens[0].Type);
        Assert.Equal("hello", tokens[0].Value);
        Assert.Equal(TokenType.Ampersand, tokens[1].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[2].Type);
        Assert.Equal("world", tokens[2].Value);
        Assert.Equal(TokenType.Eof, tokens[3].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Sub_Signature_No_Params()
    {
        // SUB Greet()
        var input = "SUB Greet()";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Sub, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Greet", tokens[1].Value);
        Assert.Equal(TokenType.LParen, tokens[2].Type);
        Assert.Equal(TokenType.RParen, tokens[3].Type);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    // --- Phase 8: Arrays ---

    [Fact]
    public void Lexer_Tokenises_Dim_Keyword()
    {
        var tokens = new Lexer("DIM").Tokenise();
        Assert.Equal(TokenType.Dim, tokens[0].Type);
    }

    [Fact]
    public void Lexer_Tokenises_LBracket()
    {
        var tokens = new Lexer("[").Tokenise();
        Assert.Equal(TokenType.LBracket, tokens[0].Type);
    }

    [Fact]
    public void Lexer_Tokenises_RBracket()
    {
        var tokens = new Lexer("]").Tokenise();
        Assert.Equal(TokenType.RBracket, tokens[0].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Dim_Statement_Sequence()
    {
        // DIM scores[10] As Integer
        var tokens = new Lexer("DIM scores[10] As Integer").Tokenise();

        // DIM, scores, [, 10, ], As, Integer, Eof
        Assert.Equal(8, tokens.Count);
        Assert.Equal(TokenType.Dim, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("scores", tokens[1].Value);
        Assert.Equal(TokenType.LBracket, tokens[2].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[3].Type);
        Assert.Equal("10", tokens[3].Value);
        Assert.Equal(TokenType.RBracket, tokens[4].Type);
        Assert.Equal(TokenType.As, tokens[5].Type);
        Assert.Equal(TokenType.Integer, tokens[6].Type);
        Assert.Equal(TokenType.Eof, tokens[7].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Array_Access_Sequence()
    {
        // scores[2]
        var tokens = new Lexer("scores[2]").Tokenise();

        // scores, [, 2, ], Eof
        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("scores", tokens[0].Value);
        Assert.Equal(TokenType.LBracket, tokens[1].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[2].Type);
        Assert.Equal(TokenType.RBracket, tokens[3].Type);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Sub_Signature_With_Params()
    {
        // SUB Add(a As Integer, b As Integer)
        var input = "SUB Add(a As Integer, b As Integer)";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(12, tokens.Count);
        Assert.Equal(TokenType.Sub, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Add", tokens[1].Value);
        Assert.Equal(TokenType.LParen, tokens[2].Type);
        Assert.Equal(TokenType.Identifier, tokens[3].Type);
        Assert.Equal("a", tokens[3].Value);
        Assert.Equal(TokenType.As, tokens[4].Type);
        Assert.Equal(TokenType.Integer, tokens[5].Type);
        Assert.Equal(TokenType.Comma, tokens[6].Type);
        Assert.Equal(TokenType.Identifier, tokens[7].Type);
        Assert.Equal("b", tokens[7].Value);
        Assert.Equal(TokenType.As, tokens[8].Type);
        Assert.Equal(TokenType.Integer, tokens[9].Type);
        Assert.Equal(TokenType.RParen, tokens[10].Type);
        Assert.Equal(TokenType.Eof, tokens[11].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Function_Signature_With_Return_Type()
    {
        // FUNCTION Add(a As Integer) As Integer
        var input = "FUNCTION Add(a As Integer) As Integer";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(10, tokens.Count);
        Assert.Equal(TokenType.Function, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Add", tokens[1].Value);
        Assert.Equal(TokenType.LParen, tokens[2].Type);
        Assert.Equal(TokenType.Identifier, tokens[3].Type);
        Assert.Equal("a", tokens[3].Value);
        Assert.Equal(TokenType.As, tokens[4].Type);
        Assert.Equal(TokenType.Integer, tokens[5].Type);
        Assert.Equal(TokenType.RParen, tokens[6].Type);
        Assert.Equal(TokenType.As, tokens[7].Type);
        Assert.Equal(TokenType.Integer, tokens[8].Type);
        Assert.Equal(TokenType.Eof, tokens[9].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Call_Statement()
    {
        // CALL Greet("Alice")
        var input = "CALL Greet(\"Alice\")";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.Call, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Greet", tokens[1].Value);
        Assert.Equal(TokenType.LParen, tokens[2].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[3].Type);
        Assert.Equal("Alice", tokens[3].Value);
        Assert.Equal(TokenType.RParen, tokens[4].Type);
        Assert.Equal(TokenType.Eof, tokens[5].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Return_With_Expression()
    {
        // RETURN a + b
        var input = "RETURN a + b";
        var tokens = new Lexer(input).Tokenise();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Return, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("a", tokens[1].Value);
        Assert.Equal(TokenType.Plus, tokens[2].Type);
        Assert.Equal(TokenType.Identifier, tokens[3].Type);
        Assert.Equal("b", tokens[3].Value);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Mod_Keyword()
    {
        var tokens = new Lexer("MOD").Tokenise();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Mod, tokens[0].Type);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Fact]
    public void Lexer_Tokenises_Mod_Expression()
    {
        // 10 MOD 3
        var tokens = new Lexer("10 MOD 3").Tokenise();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.IntLiteral, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(TokenType.Mod, tokens[1].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[2].Type);
        Assert.Equal("3", tokens[2].Value);
        Assert.Equal(TokenType.Eof, tokens[3].Type);
    }

    [Fact]
    public void Lexer_Skips_Rem_Comment_Line()
    {
        // REM comment followed by a real statement
        var tokens = new Lexer("REM this is a comment\nPRINT \"hi\"").Tokenise();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.NewLine, tokens[0].Type);
        Assert.Equal(TokenType.Print, tokens[1].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[2].Type);
        Assert.Equal(TokenType.Eof, tokens[3].Type);
    }

    [Fact]
    public void Lexer_Skips_Rem_Comment_At_End_Of_Input()
    {
        var tokens = new Lexer("REM just a comment").Tokenise();

        Assert.Equal(1, tokens.Count);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    // --- Phase 9: Line & Column Tracking ---

    [Fact]
    public void Lexer_Token_On_First_Line_Has_Line_1()
    {
        var tokens = new Lexer("PRINT \"hello\"").Tokenise();

        Assert.Equal(1, tokens[0].Line); // PRINT
        Assert.Equal(1, tokens[1].Line); // "hello"
    }

    [Fact]
    public void Lexer_Token_On_Second_Line_Has_Line_2()
    {
        // PRINT "hello"\nPRINT "world"
        var tokens = new Lexer("PRINT \"hello\"\nPRINT \"world\"").Tokenise();

        // tokens: PRINT(1), "hello"(1), NewLine(1), PRINT(2), "world"(2), Eof
        Assert.Equal(1, tokens[0].Line); // first PRINT
        Assert.Equal(2, tokens[3].Line); // second PRINT
    }

    [Fact]
    public void Lexer_Token_Column_Starts_At_1()
    {
        var tokens = new Lexer("PRINT \"hello\"").Tokenise();

        Assert.Equal(1, tokens[0].Column); // PRINT starts at col 1
    }

    [Fact]
    public void Lexer_Token_Column_Is_Correct_After_Space()
    {
        // "PRINT \"hello\""
        // P=1, R=2, I=3, N=4, T=5, space=6, "=7
        var tokens = new Lexer("PRINT \"hello\"").Tokenise();

        Assert.Equal(7, tokens[1].Column); // string literal starts at col 7
    }

    [Fact]
    public void Lexer_Column_Resets_To_1_After_NewLine()
    {
        var tokens = new Lexer("PRINT \"a\"\nLET x = 1").Tokenise();

        // NewLine token, then LET should be col 1 on line 2
        var letToken = tokens.First(t => t.Type == TokenType.Let);
        Assert.Equal(2, letToken.Line);
        Assert.Equal(1, letToken.Column);
    }

    [Fact]
    public void Lexer_Tracks_Line_Across_Multiple_Newlines()
    {
        var tokens = new Lexer("LET x = 1\nLET y = 2\nPRINT x").Tokenise();

        var printToken = tokens.First(t => t.Type == TokenType.Print);
        Assert.Equal(3, printToken.Line);
    }

    // --- Phase 10: Standard Library ---

    [Theory]
    [InlineData("MID$")]
    [InlineData("LEFT$")]
    [InlineData("RIGHT$")]
    [InlineData("UPPER$")]
    [InlineData("LOWER$")]
    [InlineData("STR$")]
    public void Lexer_Dollar_Suffix_Builtin_Lexes_As_Identifier(string name)
    {
        var tokens = new Lexer(name).Tokenise();

        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(name, tokens[0].Value);
    }

    [Theory]
    [InlineData("LEN")]
    [InlineData("INT")]
    [InlineData("VAL")]
    [InlineData("ABS")]
    [InlineData("SQR")]
    [InlineData("RND")]
    public void Lexer_No_Dollar_Builtin_Lexes_As_Identifier(string name)
    {
        var tokens = new Lexer(name).Tokenise();

        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(name, tokens[0].Value);
    }

    [Fact]
    public void Lexer_Dollar_Suffix_In_Call_Expression_Produces_Correct_Sequence()
    {
        // MID$("Hello", 2, 3)
        var tokens = new Lexer("MID$(\"Hello\", 2, 3)").Tokenise();

        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("MID$", tokens[0].Value);
        Assert.Equal(TokenType.LParen, tokens[1].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[2].Type);
        Assert.Equal("Hello", tokens[2].Value);
        Assert.Equal(TokenType.Comma, tokens[3].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[4].Type);
        Assert.Equal(TokenType.Comma, tokens[5].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[6].Type);
        Assert.Equal(TokenType.RParen, tokens[7].Type);
    }

    [Fact]
    public void Lex_InputKeyword_ReturnsCorrectToken()
    {
        var tokens = new Lexer("INPUT").Tokenise();
        Assert.Equal(TokenType.Input, tokens[0].Type);
    }

    [Fact]
    public void Lex_Semicolon_ReturnsCorrectToken()
    {
        var tokens = new Lexer(";").Tokenise();
        Assert.Equal(TokenType.Semicolon, tokens[0].Type);
    }

    [Fact]
    public void Lex_Input_Bare_ReturnsCorrectTokens()
    {
        // INPUT name$
        var tokens = new Lexer("INPUT name$").Tokenise();
        Assert.Equal(TokenType.Input, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("name$", tokens[1].Value);
    }

    [Fact]
    public void Lex_Input_WithPrompt_ReturnsCorrectTokens()
    {
        // INPUT "Enter name"; name$
        var tokens = new Lexer("INPUT \"Enter name\"; name$").Tokenise();
        Assert.Equal(TokenType.Input, tokens[0].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[1].Type);
        Assert.Equal("Enter name", tokens[1].Value);
        Assert.Equal(TokenType.Semicolon, tokens[2].Type);
        Assert.Equal(TokenType.Identifier, tokens[3].Type);
        Assert.Equal("name$", tokens[3].Value);
    }

    // --- Bug fix: = operator without surrounding spaces ---

    [Fact]
    public void Lex_Eq_Without_Surrounding_Spaces_Produces_Eq_Token()
    {
        // Bug: '=' had no dedicated case in the lexer switch, so LET x=5
        // accumulated 'x=' into the buffer and produced TokenType.Unknown.
        var input = "LET x=5";
        var lexer = new Lexer(input);

        var tokens = lexer.Tokenise();

        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
        Assert.Equal(TokenType.Eq, tokens[2].Type);
        Assert.Equal(TokenType.IntLiteral, tokens[3].Type);
        Assert.Equal("5", tokens[3].Value);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void Lex_Assignment_Without_Spaces_Produces_No_Unknown_Tokens()
    {
        var tokens = new Lexer("LET x=5").Tokenise();
        Assert.DoesNotContain(tokens, t => t.Type == TokenType.Unknown);
    }

}