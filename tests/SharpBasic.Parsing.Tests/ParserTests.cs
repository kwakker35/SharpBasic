using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Parsing.Tests;

public class ParserTests
{
    [Fact]
    public void Parser_Correctly_Generates_Hello_World_Program()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Print,"",1,1),
            new(TokenType.StringLiteral,"Hello World!",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;


        Assert.NotNull(program);
        Assert.Equal(1, program.Statements.Count);
        var stmt = Assert.IsType<PrintStatement>(program.Statements[0]);
        var expr = Assert.IsType<StringLiteralExpression>(stmt.Value);
        Assert.Equal("Hello World!", expr.Value);
    }

    [Fact]
    public void Parser_Genrates_ParseError_When_Invalid_Token_Follows_Print()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Print,"",1,1),
            new(TokenType.NewLine,"",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        var errors = failure.Errors;

        Assert.Equal(1, errors.Count);
        Assert.IsType<InvalidOperationException>(errors[0].Exception);
    }

    [Fact]
    public void Single_Eof_Token_Generates_Empty_Program()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.NotNull(result);
        Assert.NotNull(program);
        Assert.Empty(program.Statements);
    }

}