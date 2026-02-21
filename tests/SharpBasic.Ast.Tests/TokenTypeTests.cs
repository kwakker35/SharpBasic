using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class TokenTypeTests
{
    [Theory]
    [InlineData(TokenType.Print, "Print")]
    [InlineData(TokenType.StringLiteral, "StringLiteral")]
    [InlineData(TokenType.NewLine, "NewLine")]
    [InlineData(TokenType.Eof, "Eof")]
    [InlineData(TokenType.Unknown, "Unknown")]
    public void TokenType_ToString_Returns_Expected_Type(TokenType tokenType, string expectedType)
    {
        Assert.Equal(expectedType, tokenType.ToString());
    }
}