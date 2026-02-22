using System.Text;
using SharpBasic.Ast;

namespace SharpBasic.Lexer;

public class Lexer
{
    private readonly string _source;
    private  int _pos;

    public Lexer(string source)
    {
        _source = source;
    }

    private char Current => _pos < _source.Length ? _source[_pos]: '\0';
    private char Peek() => _pos + 1 < _source.Length ? _source[_pos + 1] : '\0';
    private void Advance() => _pos++;

    public IReadOnlyList<Token> Tokenise()
    {
        var tokens = new List<Token>();
        StringBuilder token = new();
        _pos = 0;

        while(_pos < _source.Length)
        {
            if(char.IsWhiteSpace(Current))
            {
                tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                token = new();
            }
            else
            {
                token.Append(Current);
            }
            Advance();
        }

        if(token.Length > 0)
            tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));

        return tokens;
    }

    private TokenType GetTokenType(string token)
    {
        return token.ToUpper() switch
        {
            "PRINT" => TokenType.Print,
            _ => TokenType.Unknown
        };
    }
}