using SharpBasic.Lexer;
using SharpBasic.Ast;

Console.WriteLine("Welcome to SharpBASIC");
while(true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input is null || input.ToUpperInvariant() == "EXIT") break;

    var lexer = new Lexer(input);
    var tokens = lexer.Tokenise();
    if(tokens.Count > 1 && tokens[0].Type == TokenType.Print)
    {
        Console.WriteLine(tokens[1].Value);
    }
    else
    {
        Console.WriteLine("Unknown Command");
    }
}
