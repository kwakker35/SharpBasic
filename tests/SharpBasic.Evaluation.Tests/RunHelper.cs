using SharpBasic.Lexing;
using SharpBasic.Parsing;

namespace SharpBasic.Evaluation.Tests;

public static class RunHelper
{
    public static string Run(string source)
    {
        var writer = new StringWriter();
        Console.SetOut(writer);

        var tokens = new Lexer(source).Tokenise();
        var parseResult = new Parser(tokens).Parse();
        if(parseResult is ParseSuccess ps)
            new Evaluator(ps.Program).Evaluate();

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
        return writer.ToString().Trim();
    }
}