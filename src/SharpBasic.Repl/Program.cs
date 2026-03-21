using System.Reflection;
using SharpBasic.Ast;
using SharpBasic.Evaluation;
using SharpBasic.Lexing;
using SharpBasic.Parsing;

if (args.Length == 1)
    return RunFile(args[0]);

if (args.Length > 1)
{
    Console.Error.WriteLine("Usage: sharpbasic [file.sbx]");
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

static void PrintHelp()
{
    Console.WriteLine();
    Console.WriteLine("REPL commands:");
    Console.WriteLine("  HELP    Show this help");
    Console.WriteLine("  EXIT    Quit the REPL");
    Console.WriteLine();
    Console.WriteLine("Built-in functions:");
    Console.WriteLine("  LEN(s)          Length of string");
    Console.WriteLine("  MID$(s,n,l)     Substring");
    Console.WriteLine("  LEFT$(s,n)      Left n characters");
    Console.WriteLine("  RIGHT$(s,n)     Right n characters");
    Console.WriteLine("  UPPER$(s)       Uppercase");
    Console.WriteLine("  LOWER$(s)       Lowercase");
    Console.WriteLine("  TRIM$(s)        Trim whitespace");
    Console.WriteLine("  STR$(n)         Number to string");
    Console.WriteLine("  VAL(s)          String to number");
    Console.WriteLine("  INT(n)          Floor to integer");
    Console.WriteLine("  ABS(n)          Absolute value");
    Console.WriteLine("  RND()           Random float 0..1");
    Console.WriteLine("  TYPENAME(v)     Type name of value");
    Console.WriteLine();
}

static int RunFile(string path)
{
    if (!File.Exists(path))
    {
        Console.Error.WriteLine($"Error: file not found: {path}");
        return 1;
    }
    if (!path.EndsWith(".sbx", StringComparison.OrdinalIgnoreCase))
    {
        Console.Error.WriteLine("Error: expected a .sbx file");
        return 1;
    }

    return Execute(File.ReadAllText(path), new SymbolTable());
}

static int RunInteractive()
{
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.1.0";
    var table = new SymbolTable();
    Console.WriteLine($"SharpBASIC {version}");
    Console.WriteLine("(c) 2026 Chris Grove  MIT Licence");
    Console.WriteLine("Type HELP for commands, EXIT to quit.");
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (input is null || input.ToUpperInvariant() == "EXIT")
            break;

        if (input.ToUpperInvariant() == "HELP")
        {
            PrintHelp();
            continue;
        }

        Execute(input, table);
    }
    return 0;
}
