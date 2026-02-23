using SharpBasic.Ast;
using SharpBasic.Lexer;
using Xunit;

namespace SharpBasic.Lexer.Tests;

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

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.NewLine, tokens[1].Type);
        Assert.Equal(TokenType.Print, tokens[2].Type);
    }
}