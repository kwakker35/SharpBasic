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
    [InlineData(TokenType.Let, "Let")]
    [InlineData(TokenType.Eq, "Eq")]
    [InlineData(TokenType.Identifier, "Identifier")]
    [InlineData(TokenType.If, "If")]
    [InlineData(TokenType.Then, "Then")]
    [InlineData(TokenType.Else, "Else")]
    [InlineData(TokenType.End, "End")]
    [InlineData(TokenType.While, "While")]
    [InlineData(TokenType.Wend, "Wend")]
    [InlineData(TokenType.For, "For")]
    [InlineData(TokenType.To, "To")]
    [InlineData(TokenType.Step, "Step")]
    [InlineData(TokenType.Next, "Next")]
    [InlineData(TokenType.NextVar, "NextVar")]
    public void TokenType_ToString_Returns_Expected_Type(TokenType tokenType, string expectedType)
    {
        Assert.Equal(expectedType, tokenType.ToString());
    }
}