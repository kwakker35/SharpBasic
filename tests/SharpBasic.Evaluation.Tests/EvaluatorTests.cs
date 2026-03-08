using SharpBasic.Ast;
using SharpBasic.Parsing;
using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class EvaluatorTests
{
    [Fact]
    public void Evaluator_Generates_EvalSuccess_For_Valid_Print_Program()
    {
        var input = new Program(
            [
                new PrintStatement(new StringLiteralExpression(
                    "Hello World!", new SourceLocation(1, 1)), new SourceLocation(1, 1))
            ]);

        var evaluator = new Evaluator(input);
        var result = evaluator.Evaluate();

        Assert.NotNull(result);
        Assert.IsType<EvalSuccess>(result);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Print()
    {
        var output = RunHelper.Run("PRINT \"Hello World!\"");
        Assert.NotNull(output);
        Assert.Equal("Hello World!", output);
    }

    [Fact]
    public void Evaluator_Generates_EvalSuccess_For_Valid_Let_Only_Program()
    {
        var input = new Program(
            [
                new LetStatement(new Token(TokenType.Identifier,"X",1,1),
                new StringLiteralExpression(
                    "Hello World!", new SourceLocation(1, 1)
                    ),
                new SourceLocation(1, 1))
            ]);

        var evaluator = new Evaluator(input);
        var result = evaluator.Evaluate();

        Assert.NotNull(result);
        Assert.IsType<EvalSuccess>(result);
    }

    [Fact]
    public void Evaluator_Generates_EvalFailure_For_Unassigned_Identifier()
    {
        var input = new Program(
           [
               new PrintStatement(new IdentifierExpression(
                    "Y", new SourceLocation(1, 1)), new SourceLocation(1, 1))
           ]);

        var evaluator = new Evaluator(input);
        var result = evaluator.Evaluate();

        Assert.IsType<EvalFailure>(result);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Let()
    {
        var output = RunHelper.Run("LET X = \"Hello World!\"\nPRINT X");
        Assert.NotNull(output);
        Assert.Equal("Hello World!", output);
    }

    [Fact]
    public void Evaluator_Generates_Correct_Value_Simple_Add()
    {
        var loc = new SourceLocation(1, 1);
        var id = new Token(TokenType.Identifier, "X", 1, 1);
        var expr = new BinaryExpression(
                    new IntLiteralExpression(1, loc),
                    new Token(TokenType.Plus, "", 1, 1),
                    new IntLiteralExpression(2, loc),
                    loc
        );

        var input = new Program(
            [
                new LetStatement(id,expr,loc)
            ]);

        var evaluator = new Evaluator(input);
        var result = evaluator.Evaluate();

        Assert.NotNull(result);
        Assert.IsType<EvalSuccess>(result);
    }

    [Fact]
    public void Evaluator_Generates_Correct_Value_Simple_Divide()
    {
        var loc = new SourceLocation(1, 1);
        var id = new Token(TokenType.Identifier, "X", 1, 1);
        var expr = new BinaryExpression(
                    new IntLiteralExpression(10, loc),
                    new Token(TokenType.Divide, "", 1, 1),
                    new IntLiteralExpression(2, loc),
                    loc
        );

        var input = new Program(
            [
                new LetStatement(id,expr,loc)
            ]);

        var evaluator = new Evaluator(input);
        var result = evaluator.Evaluate();

        Assert.NotNull(result);
        Assert.IsType<EvalSuccess>(result);
    }

    [Fact]
    public void Evaluator_Generates_Correct_Value_Complex_Sum()
    {
        var loc = new SourceLocation(1, 1);
        var id = new Token(TokenType.Identifier, "X", 1, 1);
        var rExpr = new BinaryExpression(
                    new IntLiteralExpression(2, loc),
                    new Token(TokenType.Multiply, "", 1, 1),
                    new IntLiteralExpression(3, loc),
                    loc
        );

        var expr = new BinaryExpression(
                    new IntLiteralExpression(1, loc),
                    new Token(TokenType.Plus, "", 1, 1),
                    rExpr,
                    loc
        );

        var input = new Program(
            [
                new LetStatement(id,expr,loc),
                new PrintStatement(new IdentifierExpression("X", loc), loc)
            ]);

        var evaluator = new Evaluator(input);
        var result = evaluator.Evaluate();

        Assert.NotNull(result);
        Assert.IsType<EvalSuccess>(result);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_Sum_Int()
    {
        var output = RunHelper.Run("LET X = 1 + 2\nPRINT X");
        Assert.NotNull(output);
        Assert.Equal("3", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_Sum_Float_Ends_In_Zero()
    {
        var output = RunHelper.Run("LET X = 1.5 + 2.5\nPRINT X");
        Assert.NotNull(output);
        Assert.Equal("4", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_Sum_Float_Ends_In_NonZero()
    {
        var output = RunHelper.Run("LET X = 1.7 + 2.5\nPRINT X");
        Assert.NotNull(output);
        Assert.Equal("4.2", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_If_Then()
    {
        var output = RunHelper.Run("IF 1 = 1 THEN\nPRINT \"yes\"\nEND IF");
        Assert.NotNull(output);
        Assert.Equal("yes", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_If_Then_Else_Then_Branch()
    {
        var output = RunHelper.Run("IF 1 = 1 THEN\nPRINT \"yes\"\nELSE\nPRINT \"no\"\nEND IF");
        Assert.NotNull(output);
        Assert.Equal("yes", output);
    }
    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_If_Then_Else_Else_Branch()
    {
        var output = RunHelper.Run("IF 1 = 2 THEN\nPRINT \"yes\"\nELSE\nPRINT \"no\"\nEND IF");
        Assert.NotNull(output);
        Assert.Equal("no", output);
    }
}