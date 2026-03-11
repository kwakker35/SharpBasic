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

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_While_Loop()
    {
        var output = RunHelper.Run("LET X = 1\nWHILE X < 5\nLET X = X + 1\nWEND\nPRINT X");
        Assert.NotNull(output);
        Assert.Equal("5", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Simple_While_Loop_No_Run()
    {
        var output = RunHelper.Run("LET X = 6\nWHILE X < 5\nLET X = X + 1\nWEND\nPRINT X");
        Assert.NotNull(output);
        Assert.Equal("6", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_String_Equality()
    {
        var output = RunHelper.Run("LET n = \"Alice\"\nIF n = \"Alice\" THEN\nPRINT \"yes\"\nEND IF");
        Assert.NotNull(output);
        Assert.Equal("yes", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_String_Inequality()
    {
        var output = RunHelper.Run("LET n = \"Bob\"\nIF n <> \"Alice\" THEN\nPRINT \"yes\"\nEND IF");
        Assert.NotNull(output);
        Assert.Equal("yes", output);
    }

    [Fact]
    public void Evaluator_Returns_EvalFailure_For_Unsupported_String_Operator()
    {
        var tokens = new SharpBasic.Lexing.Lexer("LET a = \"x\"\nLET b = \"y\"\nIF a < b THEN\nPRINT \"yes\"\nEND IF").Tokenise();
        var parseResult = new Parser(tokens).Parse();
        var ps = Assert.IsType<ParseSuccess>(parseResult);
        var result = new Evaluator(ps.Program).Evaluate();
        Assert.IsType<EvalFailure>(result);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_String_Concatination()
    {
        var output = RunHelper.Run("LET g = \"Hello\" & \" \" & \"World\"\nPRINT g");
        Assert.NotNull(output);
        Assert.Equal("Hello World", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_String_Concatination_With_Var()
    {
        var output = RunHelper.Run("LET x = \"Alice\"\nPRINT \"Hello: \" & x");
        Assert.NotNull(output);
        Assert.Equal("Hello: Alice", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_For_Next_No_Step()
    {
        var output = RunHelper.Run("FOR I = 1 TO 5\nPRINT I\nNEXT");
        Assert.NotNull(output);
        Assert.Equal("1\n2\n3\n4\n5", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_For_Next_With_Step()
    {
        var output = RunHelper.Run("FOR I = 1 TO 5 STEP 2\nPRINT I\nNEXT");
        Assert.NotNull(output);
        Assert.Equal("1\n3\n5", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_For_Next_With_Negative_Step()
    {
        var output = RunHelper.Run("FOR I = 5 TO 1 STEP -1\nPRINT I\nNEXT");
        Assert.NotNull(output);
        Assert.Equal("5\n4\n3\n2\n1", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_For_Next_Loop_Var_Visible_After()
    {
        var output = RunHelper.Run("FOR I = 1 TO 3\nPRINT I\nNEXT\nPRINT I");
        Assert.NotNull(output);
        Assert.Equal("1\n2\n3\n3", output);
    }

    [Fact]
    public void RunHelper_Generates_Correct_Output_For_Nested_For_Next_Loop()
    {
        var output = RunHelper.Run("FOR I = 1 TO 3\nFOR J = 1 TO 2\nPRINT I & \" \" & J\nNEXT J\nNEXT I");
        Assert.NotNull(output);
        Assert.Equal("1 1\n1 2\n2 1\n2 2\n3 1\n3 2", output);
    }
}