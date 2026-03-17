using SharpBasic.Ast;
using SharpBasic.Evaluation;
using SharpBasic.Lexing;
using SharpBasic.Parsing;

var table = new SymbolTable();
Console.WriteLine("Welcome to SharpBASIC");
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input is null || input.ToUpperInvariant() == "EXIT")
        break;

    var lexer = new Lexer(input);
    var tokens = lexer.Tokenise();
    var parser = new Parser(tokens);
    var parseResult = parser.Parse();

    if (parseResult is ParseFailure pf)
    {
        Console.WriteLine("One or more parse errors found:");
        foreach (var diagnostic in pf.Diagnostics)
        {
            Console.WriteLine(diagnostic.ToString());
        }
        continue;
    }
    else
    {
        var ps = (ParseSuccess)parseResult;
        var evaluator = new Evaluator(ps.Program, table);
        var evalResult = evaluator.Evaluate();
        if (evalResult is EvalFailure ef)
        {
            Console.WriteLine("One or more evaluation errors found:");
            foreach (var diagnostic in ef.Diagnostics)
            {
                Console.WriteLine(diagnostic.ToString());
            }
            continue;
        }
        else if (evalResult is EvalSuccess es && es.Value is not VoidValue)
        {
            //do someting later with other value types?
        }
    }
}
