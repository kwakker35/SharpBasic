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

}