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
    public void Lex_UnkownKeyword_ReturnsCorrectToken()
    {
        //Arange
        var input = "UNKNOWN";
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
        var input  ="\"Hello, World!\"";
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
    private void Lex_Eof_IsLastToken()
    {
        var input ="PRINT";
        var lexer = new Lexer(input);

        var tokens = lexer.Tokenise();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Fact]
    private void Lex_Mulitple_Spaces_Skipped()
    {
        var input = "PRINT  HELLO";

         var lexer = new Lexer(input);

        var tokens = lexer.Tokenise();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.Unknown, tokens[1].Type);
        Assert.Equal(TokenType.Eof, tokens[2].Type);
    }
}