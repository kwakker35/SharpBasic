using SharpBasic.Evaluator;
using SharpBasic.Parser;

namespace SharpBasic.Evaluator.Tests;

public static class RunHelper
{
    public static string Run(string source)
    {
        var writer = new StringWriter();
        Console.SetOut(writer);

        var tokens = new SharpBasic.Lexer.Lexer(source).Tokenise();
        var parseResult = new SharpBasic.Parser.Parser(tokens).Parse();
        if(parseResult is ParseSuccess ps)
            new Evaluator(ps.Program).Evaluate();

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
        return writer.ToString().Trim();
    }
}