using SharpBasic.Ast;

namespace SharpBasic.Lexer;

public class Lexer
{
    private readonly string _input;

    public Lexer(string input)
    {
        _input = input;
    }

    public IReadOnlyList<Token> Tokenise()
    {
        var tokens = new List<Token>();

        var tokenType = _input.ToUpper() switch
        {
            "PRINT" => TokenType.Print,
            _ => TokenType.Unknown
        };

        tokens.Add(new Token(tokenType, _input, 1, 1));
        
        return tokens;
    }
}