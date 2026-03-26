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
    Console.WriteLine("Commands:  HELP  EXIT");
    Console.WriteLine();
    Console.WriteLine("Syntax:");
    Console.WriteLine("  LET x AS INTEGER = 42           Variable declaration");
    Console.WriteLine("  LET x = x + 1                   Assignment");
    Console.WriteLine("  CONST PI = 3.14159              Named constant");
    Console.WriteLine("  PRINT \"hello \" & x              Output");
    Console.WriteLine("  INPUT \"Name? \"; name            Input");
    Console.WriteLine("  IF x > 0 THEN ... END IF        Conditional");
    Console.WriteLine("  IF x > 0 THEN ... ELSE ... END IF");
    Console.WriteLine("  SELECT CASE op                  Multi-way branch");
    Console.WriteLine("    CASE \"+\" : ...  CASE ELSE : ...");
    Console.WriteLine("  END SELECT");
    Console.WriteLine("  FOR i = 1 TO 10 ... NEXT i      Counted loop");
    Console.WriteLine("  FOR i = 10 TO 1 STEP -1 ... NEXT i");
    Console.WriteLine("  WHILE x > 0 ... WEND            Conditional loop");
    Console.WriteLine("  DIM a[5] AS INTEGER             1D array (0-indexed)");
    Console.WriteLine("  DIM m[3][3] AS INTEGER          2D array");
    Console.WriteLine("  SUB name(p AS INTEGER)          Subroutine");
    Console.WriteLine("  END SUB  /  CALL name(arg)");
    Console.WriteLine("  FUNCTION f(n AS INTEGER) AS INTEGER   Function");
    Console.WriteLine("  END FUNCTION  /  RETURN value");
    Console.WriteLine("  REM comment                     Comment (inline or full-line)");
    Console.WriteLine();
    Console.WriteLine("Built-in functions:");
    Console.WriteLine("  LEN(s)  MID$(s,n,l)  LEFT$(s,n)  RIGHT$(s,n)");
    Console.WriteLine("  UPPER$(s)  LOWER$(s)  TRIM$(s)  CHR$(n)  STRING$(c,n)  ASC(s)");
    Console.WriteLine("  STR$(n)  VAL(s)  INT(n)  CINT(n)  ABS(n)  SQR(n)  RND()  TYPENAME(v)");
    Console.WriteLine("  MAX(a,b)  MIN(a,b)  CLAMP(n,min,max)");
    Console.WriteLine("Statement built-ins:");
    Console.WriteLine("  SLEEP(ms)");
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
