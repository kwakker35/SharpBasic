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
        Assert.Equal(3.14, expr.Value);
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

    [Fact]
    public void Parser_Correctly_Generates_If_Then_End_Simple()
    {
        // IF 1 = 1 THEN
        //     PRINT "yes"
        // END IF

        var tokens = new List<Token>
        {
            new(TokenType.If,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Then,"",1,1),
            new(TokenType.Print,"",1,1),
            new(TokenType.StringLiteral,"yes",1,1),
            new(TokenType.End,"",1,1),
            new(TokenType.If,"",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<IfStatement>(program.Statements[0]);

        var binExpr = Assert.IsType<BinaryExpression>(stmt.Condition);

        //LEFT side
        var left = Assert.IsType<IntLiteralExpression>(binExpr.Left);
        Assert.Equal(1, left.Value);

        //Operator
        Assert.Equal(TokenType.Eq, binExpr.Operator.Type);

        //RIGHT side
        var right = Assert.IsType<IntLiteralExpression>(binExpr.Right);
        Assert.Equal(1, right.Value);

        //THEN block
        var thenBlock = Assert.IsType<List<Statement>>(stmt.ThenBlock);
        Assert.Equal(1, thenBlock.Count);
        var thenStmt = Assert.IsType<PrintStatement>(thenBlock[0]);
        var strLit = Assert.IsType<StringLiteralExpression>(thenStmt.Value);
        Assert.Equal("yes", strLit.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_If_Then_Else_End_Simple()
    {
        // IF 1 = 1 THEN
        //     PRINT "yes"
        // ELSE
        //      PRINT "no"
        // END IF

        var tokens = new List<Token>
        {
            new(TokenType.If,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Then,"",1,1),
            new(TokenType.Print,"",1,1),
            new(TokenType.StringLiteral,"yes",1,1),
            new(TokenType.Else,"",1,1),
            new(TokenType.Print,"",1,1),
            new(TokenType.StringLiteral,"no",1,1),
            new(TokenType.End,"",1,1),
            new(TokenType.If,"",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<IfStatement>(program.Statements[0]);

        var binExpr = Assert.IsType<BinaryExpression>(stmt.Condition);

        //LEFT side
        var left = Assert.IsType<IntLiteralExpression>(binExpr.Left);
        Assert.Equal(1, left.Value);

        //Operator
        Assert.Equal(TokenType.Eq, binExpr.Operator.Type);

        //RIGHT side
        var right = Assert.IsType<IntLiteralExpression>(binExpr.Right);
        Assert.Equal(1, right.Value);

        //THEN block
        var thenBlock = Assert.IsType<List<Statement>>(stmt.ThenBlock);
        Assert.Equal(1, thenBlock.Count);
        var thenStmt = Assert.IsType<PrintStatement>(thenBlock[0]);
        var strLit = Assert.IsType<StringLiteralExpression>(thenStmt.Value);
        Assert.Equal("yes", strLit.Value);

        //ELSE block
        Assert.NotNull(stmt.ElseBlock);
        var elseBlock = Assert.IsType<List<Statement>>(stmt.ElseBlock);
        Assert.Equal(1, elseBlock.Count);
        var elseStmt = Assert.IsType<PrintStatement>(elseBlock[0]);
        var strLit2 = Assert.IsType<StringLiteralExpression>(elseStmt.Value);
        Assert.Equal("no", strLit2.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_While_Loop_Simple()
    {
        // WHILE (1 = 1)
        //     PRINT "Hello, World!"
        // WEND

        var tokens = new List<Token>
        {
            new(TokenType.While,"",1,1),
            new(TokenType.LParen,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.RParen,"",1,1),
            new(TokenType.NewLine,"",1,1),
            new(TokenType.Print,"",1,1),
            new(TokenType.StringLiteral,"Hello, World!",1,1),
            new(TokenType.NewLine,"",1,1),
            new(TokenType.Wend,"",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<WhileStatement>(program.Statements[0]);

        var binExpr = Assert.IsType<BinaryExpression>(stmt.Condition);

        //LEFT side
        var left = Assert.IsType<IntLiteralExpression>(binExpr.Left);
        Assert.Equal(1, left.Value);

        //Operator
        Assert.Equal(TokenType.Eq, binExpr.Operator.Type);

        //RIGHT side
        var right = Assert.IsType<IntLiteralExpression>(binExpr.Right);
        Assert.Equal(1, right.Value);

        //BODY block
        var bodyBlock = Assert.IsType<List<Statement>>(stmt.Body);
        Assert.Equal(1, bodyBlock.Count);
        var thenStmt = Assert.IsType<PrintStatement>(bodyBlock[0]);
        var strLit = Assert.IsType<StringLiteralExpression>(thenStmt.Value);
        Assert.Equal("Hello, World!", strLit.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_For_Next_With_Step_And_Explicit_Next()
    {
        // FOR X = 1 TO 10 STEP 2
        //     PRINT X
        // NEXT X

        var tokens = new List<Token>
        {
            new(TokenType.For,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eq,"",1,1),
            new(TokenType.IntLiteral,"1",1,1),
            new(TokenType.To,"",1,1),
            new(TokenType.IntLiteral,"10",1,1),
            new(TokenType.Step,"",1,1),
            new(TokenType.IntLiteral,"2",1,1),
            new(TokenType.NewLine,"",1,1),
            new(TokenType.Print,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.NewLine,"",1,1),
            new(TokenType.Next,"",1,1),
            new(TokenType.Identifier,"X",1,1),
            new(TokenType.Eof,"",1,1)
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.NotNull(program);
        Assert.Single(program.Statements);
        var stmt = Assert.IsType<ForStatement>(program.Statements[0]);

        Assert.Equal(TokenType.Identifier, stmt.LoopVar.Type);
        Assert.Equal("X", stmt.LoopVar.Value);

        //Start
        var start = Assert.IsType<IntLiteralExpression>(stmt.Start);
        Assert.Equal(1, start.Value);

        //Limit
        var limit = Assert.IsType<IntLiteralExpression>(stmt.Limit);
        Assert.Equal(10, limit.Value);

        //Step
        var step = Assert.IsType<IntLiteralExpression>(stmt.Step);
        Assert.Equal(2, step.Value);

        //BODY
        var body = Assert.IsType<List<Statement>>(stmt.Body);
        Assert.Equal(1, body.Count);
        var printStmt = Assert.IsType<PrintStatement>(body[0]);
        var id = Assert.IsType<IdentifierExpression>(printStmt.Value);
        Assert.Equal("X", id.Name);
    }

    [Fact]
    public void Parser_Correctly_Generates_Sub_Declaration_No_Params_Empty_Body()
    {
        // SUB Greet()
        // END SUB
        var tokens = new List<Token>
        {
            new(TokenType.Sub, "", 1, 1),
            new(TokenType.Identifier, "Greet", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.NewLine, "", 1, 1),
            new(TokenType.End, "", 2, 1),
            new(TokenType.Sub, "", 2, 1),
            new(TokenType.Eof, "", 2, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.Single(program.Statements);
        var stmt = Assert.IsType<SubDeclaration>(program.Statements[0]);
        Assert.Equal("Greet", stmt.Name);
        Assert.Empty(stmt.Parameters);
        Assert.Empty(stmt.Body);
    }

    [Fact]
    public void Parser_Correctly_Generates_Sub_Declaration_With_Params()
    {
        // SUB Add(a As Integer, b As Integer)
        // END SUB
        var tokens = new List<Token>
        {
            new(TokenType.Sub, "", 1, 1),
            new(TokenType.Identifier, "Add", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.Identifier, "a", 1, 1),
            new(TokenType.As, "", 1, 1),
            new(TokenType.Integer, "", 1, 1),
            new(TokenType.Comma, "", 1, 1),
            new(TokenType.Identifier, "b", 1, 1),
            new(TokenType.As, "", 1, 1),
            new(TokenType.Integer, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.NewLine, "", 1, 1),
            new(TokenType.End, "", 2, 1),
            new(TokenType.Sub, "", 2, 1),
            new(TokenType.Eof, "", 2, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.Single(program.Statements);
        var stmt = Assert.IsType<SubDeclaration>(program.Statements[0]);
        Assert.Equal("Add", stmt.Name);
        Assert.Equal(2, stmt.Parameters.Count);
        Assert.Equal("a", stmt.Parameters[0].Name);
        Assert.Equal("Integer", stmt.Parameters[0].TypeName);
        Assert.Equal("b", stmt.Parameters[1].Name);
        Assert.Equal("Integer", stmt.Parameters[1].TypeName);
    }

    [Fact]
    public void Parser_Correctly_Generates_Sub_Declaration_With_Body()
    {
        // SUB Greet()
        //     PRINT "Hello"
        // END SUB
        var tokens = new List<Token>
        {
            new(TokenType.Sub, "", 1, 1),
            new(TokenType.Identifier, "Greet", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.NewLine, "", 1, 1),
            new(TokenType.Print, "", 2, 1),
            new(TokenType.StringLiteral, "Hello", 2, 1),
            new(TokenType.NewLine, "", 2, 1),
            new(TokenType.End, "", 3, 1),
            new(TokenType.Sub, "", 3, 1),
            new(TokenType.Eof, "", 3, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.Single(program.Statements);
        var stmt = Assert.IsType<SubDeclaration>(program.Statements[0]);
        Assert.Equal("Greet", stmt.Name);
        Assert.Single(stmt.Body);
        var print = Assert.IsType<PrintStatement>(stmt.Body[0]);
        var str = Assert.IsType<StringLiteralExpression>(print.Value);
        Assert.Equal("Hello", str.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Function_Declaration_With_Return_Type()
    {
        // FUNCTION Add(a As Integer) As Integer
        // END FUNCTION
        var tokens = new List<Token>
        {
            new(TokenType.Function, "", 1, 1),
            new(TokenType.Identifier, "Add", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.Identifier, "a", 1, 1),
            new(TokenType.As, "", 1, 1),
            new(TokenType.Integer, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.As, "", 1, 1),
            new(TokenType.Integer, "", 1, 1),
            new(TokenType.NewLine, "", 1, 1),
            new(TokenType.End, "", 2, 1),
            new(TokenType.Function, "", 2, 1),
            new(TokenType.Eof, "", 2, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.Single(program.Statements);
        var stmt = Assert.IsType<FunctionDeclaration>(program.Statements[0]);
        Assert.Equal("Add", stmt.Name);
        Assert.Equal("Integer", stmt.ReturnType);
        Assert.Single(stmt.Parameters);
        Assert.Equal("a", stmt.Parameters[0].Name);
        Assert.Equal("Integer", stmt.Parameters[0].TypeName);
    }

    // --- Phase 8: Arrays ---

    [Fact]
    public void Parser_Correctly_Generates_DimStatement()
    {
        // DIM scores[10] As Integer
        var tokens = new List<Token>
        {
            new(TokenType.Dim, "", 1, 1),
            new(TokenType.Identifier, "scores", 1, 1),
            new(TokenType.LBracket, "", 1, 1),
            new(TokenType.IntLiteral, "10", 1, 1),
            new(TokenType.RBracket, "", 1, 1),
            new(TokenType.As, "", 1, 1),
            new(TokenType.Integer, "", 1, 1),
            new(TokenType.Eof, "", 1, 1)
        };

        var result = new Parser(tokens).Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var stmt = Assert.IsType<DimStatement>(success.Program.Statements[0]);

        Assert.Equal("scores", stmt.Name);
        Assert.Equal(10, stmt.Size);
        Assert.Equal("Integer", stmt.TypeName);
    }

    [Fact]
    public void Parser_Correctly_Generates_ArrayAccessExpression_In_Let()
    {
        // LET x = scores[2]
        var tokens = new List<Token>
        {
            new(TokenType.Let, "", 1, 1),
            new(TokenType.Identifier, "x", 1, 1),
            new(TokenType.Eq, "", 1, 1),
            new(TokenType.Identifier, "scores", 1, 1),
            new(TokenType.LBracket, "", 1, 1),
            new(TokenType.IntLiteral, "2", 1, 1),
            new(TokenType.RBracket, "", 1, 1),
            new(TokenType.Eof, "", 1, 1)
        };

        var result = new Parser(tokens).Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var stmt = Assert.IsType<LetStatement>(success.Program.Statements[0]);
        var expr = Assert.IsType<ArrayAccessExpression>(stmt.Value);

        Assert.Equal("scores", expr.Name);
        Assert.IsType<IntLiteralExpression>(expr.Index);
    }

    [Fact]
    public void Parser_Correctly_Generates_Array_Assignment_In_Let()
    {
        // LET scores[0] = 42
        var tokens = new List<Token>
        {
            new(TokenType.Let, "", 1, 1),
            new(TokenType.Identifier, "scores", 1, 1),
            new(TokenType.LBracket, "", 1, 1),
            new(TokenType.IntLiteral, "0", 1, 1),
            new(TokenType.RBracket, "", 1, 1),
            new(TokenType.Eq, "", 1, 1),
            new(TokenType.IntLiteral, "42", 1, 1),
            new(TokenType.Eof, "", 1, 1)
        };

        var result = new Parser(tokens).Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var stmt = Assert.IsType<ArrayAssignStatement>(success.Program.Statements[0]);

        Assert.Equal("scores", stmt.Name);
        Assert.IsType<IntLiteralExpression>(stmt.Index);
        Assert.IsType<IntLiteralExpression>(stmt.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Return_Statement_No_Value()
    {
        // SUB Greet()
        //     RETURN
        // END SUB
        var tokens = new List<Token>
        {
            new(TokenType.Sub, "", 1, 1),
            new(TokenType.Identifier, "Greet", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.NewLine, "", 1, 1),
            new(TokenType.Return, "", 2, 1),
            new(TokenType.NewLine, "", 2, 1),
            new(TokenType.End, "", 3, 1),
            new(TokenType.Sub, "", 3, 1),
            new(TokenType.Eof, "", 3, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        var sub = Assert.IsType<SubDeclaration>(program.Statements[0]);
        var ret = Assert.IsType<ReturnStatement>(sub.Body[0]);
        Assert.Null(ret.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Return_Statement_With_Expression()
    {
        // FUNCTION GetOne() As Integer
        //     RETURN 1
        // END FUNCTION
        var tokens = new List<Token>
        {
            new(TokenType.Function, "", 1, 1),
            new(TokenType.Identifier, "GetOne", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.As, "", 1, 1),
            new(TokenType.Integer, "", 1, 1),
            new(TokenType.NewLine, "", 1, 1),
            new(TokenType.Return, "", 2, 1),
            new(TokenType.IntLiteral, "1", 2, 1),
            new(TokenType.NewLine, "", 2, 1),
            new(TokenType.End, "", 3, 1),
            new(TokenType.Function, "", 3, 1),
            new(TokenType.Eof, "", 3, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        var fn = Assert.IsType<FunctionDeclaration>(program.Statements[0]);
        var ret = Assert.IsType<ReturnStatement>(fn.Body[0]);
        var lit = Assert.IsType<IntLiteralExpression>(ret.Value);
        Assert.Equal(1, lit.Value);
    }

    [Fact]
    public void Parser_Correctly_Generates_Call_Statement_No_Args()
    {
        // CALL Greet()
        var tokens = new List<Token>
        {
            new(TokenType.Call, "", 1, 1),
            new(TokenType.Identifier, "Greet", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.Eof, "", 1, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.Single(program.Statements);
        var stmt = Assert.IsType<CallStatement>(program.Statements[0]);
        Assert.Equal("Greet", stmt.Name);
        Assert.Empty(stmt.Arguments);
    }

    [Fact]
    public void Parser_Correctly_Generates_Call_Statement_With_Args()
    {
        // CALL Add(1, 2)
        var tokens = new List<Token>
        {
            new(TokenType.Call, "", 1, 1),
            new(TokenType.Identifier, "Add", 1, 1),
            new(TokenType.LParen, "", 1, 1),
            new(TokenType.IntLiteral, "1", 1, 1),
            new(TokenType.Comma, "", 1, 1),
            new(TokenType.IntLiteral, "2", 1, 1),
            new(TokenType.RParen, "", 1, 1),
            new(TokenType.Eof, "", 1, 1),
        };

        var parser = new Parser(tokens);
        var result = parser.Parse();
        var success = Assert.IsType<ParseSuccess>(result);
        var program = success.Program;

        Assert.Single(program.Statements);
        var stmt = Assert.IsType<CallStatement>(program.Statements[0]);
        Assert.Equal("Add", stmt.Name);
        Assert.Equal(2, stmt.Arguments.Count);
        Assert.IsType<IntLiteralExpression>(stmt.Arguments[0]);
        Assert.IsType<IntLiteralExpression>(stmt.Arguments[1]);
    }
}