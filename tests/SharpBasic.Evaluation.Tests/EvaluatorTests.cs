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
}