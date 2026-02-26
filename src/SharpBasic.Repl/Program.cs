using SharpBasic.Lexer;
using SharpBasic.Parser;
using SharpBasic.Evaluator;

Console.WriteLine("Welcome to SharpBASIC");
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input is null || input.ToUpperInvariant() == "EXIT") break;

    var lexer = new Lexer(input);
    var tokens = lexer.Tokenise();
    var parser = new Parser(tokens);
    var parseResult = parser.Parse();

    if (parseResult is ParseFailure pf)
    {
        Console.WriteLine("One or more parse errors found:");
        foreach (var err in pf.Errors)
        {
            Console.WriteLine($"{err.Exception.Message} at Line:{err.Line}, Column:{err.Col}");
        }
        continue;
    }
    else
    {
        var ps = (ParseSuccess)parseResult;
        var evaluator = new Evaluator(ps.Program);
        var evalResult = evaluator.Evaluate();
        if (evalResult is EvalFailure ef)
        {
            Console.WriteLine("One or more evaluation errors found:");
            foreach (var err in ef.Errors)
            {
                Console.WriteLine($"{err.Exception.Message} at Line:{err.Line}, Column:{err.Col}");
            }
            continue;
        }
        else if (evalResult is EvalSuccess es && es.Value is not VoidValue)
        {
            //do someting later with other value types?
        }
    }

}
