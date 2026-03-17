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

    [Fact]
    public void Evaluator_Calls_Sub_With_No_Params()
    {
        var source = "CALL Greet()\nSUB Greet()\nPRINT \"Hello\"\nEND SUB";
        var output = RunHelper.Run(source);
        Assert.Equal("Hello", output);
    }

    [Fact]
    public void Evaluator_Calls_Sub_With_Params()
    {
        var source = "CALL Greet(\"Alice\")\nSUB Greet(name As String)\nPRINT name\nEND SUB";
        var output = RunHelper.Run(source);
        Assert.Equal("Alice", output);
    }

    // --- Phase 8: Arrays ---

    [Fact]
    public void Evaluator_Dim_Declares_Array_In_Symbol_Table()
    {
        var source = "DIM scores[3] As Integer";
        var result = RunHelper.RunResult(source);
        Assert.IsType<EvalSuccess>(result);
    }

    [Fact]
    public void Evaluator_Can_Assign_And_Read_Array_Element()
    {
        var source = "DIM scores[3] As Integer\nLET scores[0] = 42\nPRINT scores[0]";
        var output = RunHelper.Run(source);
        Assert.Equal("42", output);
    }

    [Fact]
    public void Evaluator_Array_Elements_Default_To_Zero()
    {
        var source = "DIM scores[3] As Integer\nPRINT scores[1]";
        var output = RunHelper.Run(source);
        Assert.Equal("0", output);
    }

    [Fact]
    public void Evaluator_Array_Out_Of_Bounds_Returns_EvalFailure()
    {
        var source = "DIM scores[3] As Integer\nLET scores[5] = 99";
        var result = RunHelper.RunResult(source);
        Assert.IsType<EvalFailure>(result);
    }

    [Fact]
    public void Evaluator_Array_Type_Mismatch_Returns_EvalFailure()
    {
        var source = "DIM scores[3] As Integer\nLET scores[0] = \"hello\"";
        var result = RunHelper.RunResult(source);
        Assert.IsType<EvalFailure>(result);
    }

    [Fact]
    public void Evaluator_Array_Can_Be_Used_In_Expression()
    {
        var source = "DIM nums[2] As Integer\nLET nums[0] = 10\nLET nums[1] = 20\nLET total = nums[0] + nums[1]\nPRINT total";
        var output = RunHelper.Run(source);
        Assert.Equal("30", output);
    }

    [Fact]
    public void Evaluator_Array_Can_Be_Iterated_With_For_Loop()
    {
        var source = """
            DIM nums[3] As Integer
            LET nums[0] = 1
            LET nums[1] = 2
            LET nums[2] = 3
            FOR i = 0 TO 2
            PRINT nums[i]
            NEXT i
            """;
        var output = RunHelper.Run(source);
        Assert.Equal("1\n2\n3", output);
    }

    [Fact]
    public void Evaluator_Sub_Locals_Do_Not_Leak_To_Caller()
    {
        var source = "CALL SetX()\nPRINT x\nSUB SetX()\nLET x = 42\nEND SUB";
        var result = RunHelper.RunResult(source);
        Assert.IsType<EvalFailure>(result);
    }

    [Fact]
    public void Evaluator_Calls_Function_And_Uses_Return_Value()
    {
        var source = "LET result = Add(1, 2)\nPRINT result\nFUNCTION Add(a As Integer, b As Integer) As Integer\nRETURN a + b\nEND FUNCTION";
        var output = RunHelper.Run(source);
        Assert.Equal("3", output);
    }

    [Fact]
    public void Evaluator_Sub_Hoisted_Can_Be_Called_Before_Declaration()
    {
        var source = "CALL Greet()\nSUB Greet()\nPRINT \"Hi\"\nEND SUB";
        var output = RunHelper.Run(source);
        Assert.Equal("Hi", output);
    }

    [Fact]
    public void Evaluator_Function_Hoisted_Can_Be_Called_Before_Declaration()
    {
        var source = "LET result = Double(5)\nPRINT result\nFUNCTION Double(n As Integer) As Integer\nRETURN n + n\nEND FUNCTION";
        var output = RunHelper.Run(source);
        Assert.Equal("10", output);
    }

    [Fact]
    public void Evaluator_Recursive_Fibonacci_Returns_Correct_Value()
    {
        var source = """
            LET result = Fib(10)
            PRINT result
            FUNCTION Fib(n As Integer) As Integer
            IF n <= 1 THEN
            RETURN n
            END IF
            RETURN Fib(n - 1) + Fib(n - 2)
            END FUNCTION
            """;
        var output = RunHelper.Run(source);
        Assert.Equal("55", output);
    }

    [Fact]
    public void RunHelper_Mod_Integer_Operands()
    {
        var output = RunHelper.Run("PRINT 10 MOD 3");
        Assert.Equal("1", output);
    }

    [Fact]
    public void RunHelper_Mod_Float_Left_Operand_Truncated()
    {
        // 7.5 → 7, 7 MOD 2 = 1
        var output = RunHelper.Run("PRINT 7.5 MOD 2");
        Assert.Equal("1", output);
    }

    [Fact]
    public void RunHelper_Mod_Both_Float_Operands_Truncated()
    {
        // 7.5 → 7, 2.9 → 2, 7 MOD 2 = 1
        var output = RunHelper.Run("PRINT 7.5 MOD 2.9");
        Assert.Equal("1", output);
    }

    [Fact]
    public void RunHelper_Rem_Comment_Line_Is_Ignored()
    {
        var output = RunHelper.Run("REM this should be ignored\nPRINT \"ok\"");
        Assert.Equal("ok", output);
    }

    [Fact]
    public void RunHelper_Rem_Comment_Does_Not_Affect_Variables()
    {
        var source = "LET x = 42\nREM x is forty two\nPRINT x";
        var output = RunHelper.Run(source);
        Assert.Equal("42", output);
    }
    // --- Phase 9: Diagnostics ---

    [Fact]
    public void EvalFailure_Diagnostics_Contains_Line_And_Col()
    {
        var tokens = new SharpBasic.Lexing.Lexer("PRINT x").Tokenise();
        var parseResult = new SharpBasic.Parsing.Parser(tokens).Parse();
        var ps = Assert.IsType<SharpBasic.Parsing.ParseSuccess>(parseResult);
        var result = new Evaluator(ps.Program).Evaluate();
        var failure = Assert.IsType<EvalFailure>(result);

        Assert.NotEmpty(failure.Diagnostics);
        var d = failure.Diagnostics[0];
        Assert.Equal(DiagnosticSeverity.Error, d.Severity);
        Assert.True(d.Line >= 0);
        Assert.True(d.Col >= 0);
    }

    [Fact]
    public void EvalFailure_Diagnostics_Message_Is_Not_Empty()
    {
        var tokens = new SharpBasic.Lexing.Lexer("PRINT x").Tokenise();
        var parseResult = new SharpBasic.Parsing.Parser(tokens).Parse();
        var ps = Assert.IsType<SharpBasic.Parsing.ParseSuccess>(parseResult);
        var result = new Evaluator(ps.Program).Evaluate();
        var failure = Assert.IsType<EvalFailure>(result);

        Assert.NotEmpty(failure.Diagnostics);
        Assert.False(string.IsNullOrWhiteSpace(failure.Diagnostics[0].Message));
    }

    [Fact]
    public void EvalFailure_Diagnostic_ToString_Includes_Line_Col_And_Error()
    {
        var tokens = new SharpBasic.Lexing.Lexer("PRINT x").Tokenise();
        var parseResult = new SharpBasic.Parsing.Parser(tokens).Parse();
        var ps = Assert.IsType<SharpBasic.Parsing.ParseSuccess>(parseResult);
        var result = new Evaluator(ps.Program).Evaluate();
        var failure = Assert.IsType<EvalFailure>(result);
        var formatted = failure.Diagnostics[0].ToString();

        Assert.Contains("Error", formatted);
        Assert.Contains("Line", formatted);
        Assert.Contains("Col", formatted);
    }

    // --- Phase 9.5: Logical operators & unary expressions ---

    [Fact]
    public void Evaluator_True_Literal_Prints_True()
    {
        var output = RunHelper.Run("PRINT TRUE");
        Assert.Equal("True", output);
    }

    [Fact]
    public void Evaluator_False_Literal_Prints_False()
    {
        var output = RunHelper.Run("PRINT FALSE");
        Assert.Equal("False", output);
    }

    [Fact]
    public void Evaluator_Let_Bool_Variable_Holds_True()
    {
        var output = RunHelper.Run("LET x = TRUE\nPRINT x");
        Assert.Equal("True", output);
    }

    [Fact]
    public void Evaluator_And_True_True_Returns_True()
    {
        var output = RunHelper.Run("PRINT TRUE AND TRUE");
        Assert.Equal("True", output);
    }

    [Fact]
    public void Evaluator_And_True_False_Returns_False()
    {
        var output = RunHelper.Run("PRINT TRUE AND FALSE");
        Assert.Equal("False", output);
    }

    [Fact]
    public void Evaluator_Or_False_False_Returns_False()
    {
        var output = RunHelper.Run("PRINT FALSE OR FALSE");
        Assert.Equal("False", output);
    }

    [Fact]
    public void Evaluator_Or_False_True_Returns_True()
    {
        var output = RunHelper.Run("PRINT FALSE OR TRUE");
        Assert.Equal("True", output);
    }

    [Fact]
    public void Evaluator_Not_True_Returns_False()
    {
        var output = RunHelper.Run("PRINT NOT TRUE");
        Assert.Equal("False", output);
    }

    [Fact]
    public void Evaluator_Not_False_Returns_True()
    {
        var output = RunHelper.Run("PRINT NOT FALSE");
        Assert.Equal("True", output);
    }

    [Fact]
    public void Evaluator_And_Binds_Tighter_Than_Or()
    {
        // TRUE OR FALSE AND FALSE = TRUE OR (FALSE AND FALSE) = TRUE OR FALSE = TRUE
        var output = RunHelper.Run("PRINT TRUE OR FALSE AND FALSE");
        Assert.Equal("True", output);
    }

    [Fact]
    public void Evaluator_Not_In_If_Condition()
    {
        var source = "IF NOT FALSE THEN\nPRINT \"yes\"\nEND IF";
        var output = RunHelper.Run(source);
        Assert.Equal("yes", output);
    }

    [Fact]
    public void Evaluator_Unary_Minus_On_Variable_Negates_Value()
    {
        var output = RunHelper.Run("LET x = 5\nPRINT -x");
        Assert.Equal("-5", output);
    }

    [Fact]
    public void Evaluator_Unary_Minus_On_Float_Variable_Negates_Value()
    {
        var output = RunHelper.Run("LET x = 3.5\nPRINT -x");
        Assert.Equal("-3.5", output);
    }
}