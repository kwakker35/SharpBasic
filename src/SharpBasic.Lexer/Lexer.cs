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


    public IReadOnlyList<Token> Tokenise()
    {
        var tokens = new List<Token>();
        StringBuilder token = new();
        _pos = 0;

        foreach(var ch in _source)
        {
            if(!char.IsWhiteSpace(ch))
            {
                token.Append(ch);
            } else {
                //char IS white space so end of a token
                tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                token = new();
            }
            _pos++;

            if(_pos == _source.Length)
            {
                tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
            }
        }

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