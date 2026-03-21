using System.Reflection;
using SharpBasic.Ast;
using SharpBasic.Evaluation;
using SharpBasic.Lexing;
using SharpBasic.Parsing;

if (args.Length == 1)
    return RunFile(args[0]);

if (args.Length > 1)
{
    Console.Error.WriteLine("Usage: sharpbasic [file.bas]");
    return 1;
}

return RunInteractive();

static int Execute(string source, SymbolTable table)
{
    var parseResult = new Parser(new Lexer(source).Tokenise()).Parse();

    if (parseResult is ParseFailure pf)
    {
        foreach (var diagnostic in pf.Diagnostics)
            Console.Error.WriteLine(diagnostic.ToString());
        return 1;
    }

    var evalResult = new Evaluator(((ParseSuccess)parseResult).Program, table).Evaluate();
    if (evalResult is EvalFailure ef)
    {
        foreach (var diagnostic in ef.Diagnostics)
            Console.Error.WriteLine(diagnostic.ToString());
        return 1;
    }

    return 0;
}

static int RunFile(string path)
{
    if (!File.Exists(path))
    {
        Console.Error.WriteLine($"Error: file not found: {path}");
        return 1;
    }
    if (!path.EndsWith(".bas", StringComparison.OrdinalIgnoreCase))
    {
        Console.Error.WriteLine("Error: expected a .bas file");
        return 1;
    }

    return Execute(File.ReadAllText(path), new SymbolTable());
}

static int RunInteractive()
{
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.1.0";
    var table = new SymbolTable();
    Console.WriteLine($"SharpBASIC {version}");
    Console.WriteLine("(c) 2026 Chris Grove (@kwakker35)  MIT Licence");
    Console.WriteLine("Type HELP for commands, EXIT to quit.");
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (input is null || input.ToUpperInvariant() == "EXIT")
            break;

        Execute(input, table);
    }
    return 0;
}
