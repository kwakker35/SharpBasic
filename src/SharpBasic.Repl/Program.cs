using SharpBasic.Lexer;
using SharpBasic.Ast;

bool exitRepl = false;

Console.WriteLine("Welcome to SharpBASIC");
while(!exitRepl)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input is null) break;
    if(input.ToUpperInvariant() != "EXIT")
    {
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenise();
        if(tokens.Count > 1 && tokens[0].Type == TokenType.Print)
        {
            Console.WriteLine(tokens[1].Value);
        }
    }
    else
    {
        exitRepl = true;
    }
}
