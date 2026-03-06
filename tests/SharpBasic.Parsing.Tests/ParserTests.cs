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
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<PrintStatement>(program.Statements[0]);
        var expr = Assert.IsType<StringLiteralExpression>(stmt.Value);
        Assert.Equal("Hello World!", expr.Value);
    }

    [Fact]
    public void Parser_Generates_ParseError_When_Invalid_Token_Follows_Print()
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

        Assert.Single(errors);
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

    [Fact]
    public void Parser_Correctly_Generates_Let_Program_String()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Let,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.StringLiteral,"Hello World!",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;


        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<LetStatement>(program.Statements[0]);
        Assert.Equal("X", stmt.Identifier.Value);
        var expr = Assert.IsType<StringLiteralExpression>(stmt.Value);
        Assert.Equal("Hello World!", expr.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Let_Program_Int()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Let,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"42",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;


        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<LetStatement>(program.Statements[0]);
        Assert.Equal("X", stmt.Identifier.Value);
        var expr = Assert.IsType<IntLiteralExpression>(stmt.Value);
        Assert.Equal(42, expr.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Let_Program_Float()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Let,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.FloatLiteral,"3.14",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;


        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<LetStatement>(program.Statements[0]);
        Assert.Equal("X", stmt.Identifier.Value);
        var expr = Assert.IsType<FloatLiteralExpression>(stmt.Value);
        Assert.Equal(3.14f, expr.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Let_Program_Binary_Paren()
    {
        // LET x = 1 + 2 * 3  should parse as  1 + (2 * 3)
        var tokens = new List<Token>
        {
            new(TokenType.Let,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Plus,"",1,1),
            new(TokenType.LParen,"",1,1),
            new(TokenType.IntLiteral,"2",1,1),
            new(TokenType.Multiply,"",1,1),
            new(TokenType.IntLiteral,"3",1,1),
            new(TokenType.RParen,"",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;


        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<LetStatement>(program.Statements[0]);

        var binExpr = Assert.IsType<BinaryExpression>(stmt.Value);

        //LEFT side
        var left = Assert.IsType<IntLiteralExpression>(binExpr.Left);
        Assert.Equal(1, left.Value);

        //Operator
        Assert.Equal(TokenType.Plus, binExpr.Operator.Type);

        //RIGHT side
        var right = Assert.IsType<BinaryExpression>(binExpr.Right);
        var l2 = Assert.IsType<IntLiteralExpression>(right.Left);
        Assert.Equal(2, l2.Value);
        Assert.Equal(TokenType.Multiply, right.Operator.Type);
        var r2 = Assert.IsType<IntLiteralExpression>(right.Right);
        Assert.Equal(3, r2.Value);

    }

    [Fact]
    public void Parser_Correctly_Generates_Let_Program_Binary_No_Paren()
    {
        // LET x = 1 + 2 * 3  should parse as  1 + (2 * 3)
        var tokens = new List<Token>
        {
            new(TokenType.Let,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Plus,"",1,1),
            new(TokenType.IntLiteral,"2",1,1),
            new(TokenType.Multiply,"",1,1),
            new(TokenType.IntLiteral,"3",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;


        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<LetStatement>(program.Statements[0]);

        var binExpr = Assert.IsType<BinaryExpression>(stmt.Value);

        //LEFT side
        var left = Assert.IsType<IntLiteralExpression>(binExpr.Left);
        Assert.Equal(1, left.Value);

        //Operator
        Assert.Equal(TokenType.Plus, binExpr.Operator.Type);

        //RIGHT side
        var right = Assert.IsType<BinaryExpression>(binExpr.Right);
        var l2 = Assert.IsType<IntLiteralExpression>(right.Left);
        Assert.Equal(2, l2.Value);
        Assert.Equal(TokenType.Multiply, right.Operator.Type);
        var r2 = Assert.IsType<IntLiteralExpression>(right.Right);
        Assert.Equal(3, r2.Value);

    }

    [Fact]
    public void Parser_Generates_ParseError_When_Invalid_Token_Follows_Let()
    {
        var tokens = new List<Token>
        {
            new(TokenType.Let,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.NewLine,"",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var failure = Assert.IsType<ParseFailure>(result);
        var errors = failure.Errors;

        Assert.Single(errors);
        Assert.IsType<InvalidOperationException>(errors[0].Exception);
    }
}