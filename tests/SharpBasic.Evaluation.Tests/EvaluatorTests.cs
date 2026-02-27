using SharpBasic.Ast;
using SharpBasic.Parsing;
using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class EvaluatorTests
{
    [Fact]
    public void Evaluator_Generates_EvalSuccess_For_Valid_Program()
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
    public void RunHelper_Generates_Correct_Output()
    {
        var output = RunHelper.Run("PRINT \"Hello World!\"");
        Assert.NotNull(output);
        Assert.Equal("Hello World!", output);
    }
}