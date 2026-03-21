using SharpBasic.Ast;
using SharpBasic.Lexing;
using SharpBasic.Parsing;

namespace SharpBasic.Evaluation.Tests;

public static class RunHelper
{
    public static string Run(string source)
    {
        var currentOutputMethod = Console.Out;
        var writer = new StringWriter();
        Console.SetOut(writer);

        var tokens = new Lexer(source).Tokenise();
        var parseResult = new Parser(tokens).Parse();
        if (parseResult is ParseSuccess ps)
            new Evaluator(ps.Program).Evaluate();

        Console.SetOut(currentOutputMethod);
        return writer.ToString().Trim().ReplaceLineEndings("\n");
    }

    public static EvalResult RunResult(string source)
    {
        var tokens = new Lexer(source).Tokenise();
        var parseResult = new Parser(tokens).Parse();
        if (parseResult is ParseSuccess ps)
            return new Evaluator(ps.Program).Evaluate();
        return new EvalFailure([new Diagnostic(0, 0, "Parse failed", DiagnosticSeverity.Error)]);
    }

    public static string RunWithInput(string source, string simulatedInput)
    {
        var currentIn = Console.In;
        var currentOut = Console.Out;
        var writer = new StringWriter();
        Console.SetIn(new StringReader(simulatedInput));
        Console.SetOut(writer);

        var tokens = new Lexer(source).Tokenise();
        var parseResult = new Parser(tokens).Parse();
        if (parseResult is ParseSuccess ps)
            new Evaluator(ps.Program).Evaluate();

        Console.SetIn(currentIn);
        Console.SetOut(currentOut);
        return writer.ToString().Trim().ReplaceLineEndings("\n");
    }
}