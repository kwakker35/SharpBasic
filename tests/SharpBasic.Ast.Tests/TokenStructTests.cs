using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class TokenStructTests
{

    public static IEnumerable<object[]> GetTokenVariations()
    {
        var baseline = new Token(TokenType.Print, "Hello, World!", 1, 1);

        yield return new object[] { baseline, new Token(TokenType.StringLiteral, "Hello, World!", 1, 1) };
        yield return new object[] { baseline, new Token(TokenType.Print, "Goodbye, World!", 1, 1) };
        yield return new object[] { baseline, new Token(TokenType.Print, "Hello, World!", 2, 1) };
        yield return new object[] { baseline, new Token(TokenType.Print, "Hello, World!", 1, 2) };
    }

    [Fact]
    public void TokenStruct_Should_Have_Expected_Values()
    {
        //Arrange
        var expectedType = TokenType.Print;
        var expectedValue = "Hello, World!";
        var expectedLine = 1;
        var expectedColumn = 1;

        var token = new Token(
                            expectedType, 
                            expectedValue, 
                            expectedLine, 
                            expectedColumn);

        //Assert
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(expectedValue, token.Value);
        Assert.Equal(expectedLine, token.Line);
        Assert.Equal(expectedColumn, token.Column);
    }

    [Fact]
    public void Tokens_With_Same_Value_Should_Be_Equal()
    {
        //Arrange
        var token1 = new Token(TokenType.Print, "Hello, World!", 1, 1);
        var token2 = new Token(TokenType.Print, "Hello, World!", 1, 1);

        //Assert
        Assert.Equal(token1, token2);
    }

    [Theory]
    [MemberData(nameof(GetTokenVariations))]
    public void Tokens_With_Different_Values_Should_Not_Be_Equal(
        Token baseline, 
        Token variation)
    {
        //Assert
        Assert.NotEqual(baseline, variation);
    }

    [Fact]
    public void TokenStruct_Can_Be_Copied_Using_With()
    {
        //Arrange
        var token = new Token(TokenType.Print, "Hello, World!", 1, 1);
        var newToken = token with { Value = "Goodbye, World!" };
        //Assert
        Assert.True(newToken.Value == "Goodbye, World!");       
    }

}