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
}