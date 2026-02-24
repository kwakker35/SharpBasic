using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Parser.Tests;

public class ParserTests
{
    [Fact]
    public void Parser_Correctly_Gnerates_Hello_World_Program()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Print,"",1,1),
            new(TokenType.StringLiteral,"Hello World!",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var program = parser.Parse();

        Assert.NotNull(program);
        Assert.Equal(1, program.Statements.Count);
        var stmt = Assert.IsType<PrintStatement>(program.Statements[0]);
        var expr = Assert.IsType<StringLiteralExpression>(stmt.Value);
        Assert.Equal("Hello World!", expr.Value);
    }
}