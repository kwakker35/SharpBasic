using System.Text;
using SharpBasic.Ast;

namespace SharpBasic.Lexing;

public class Lexer
{
    private readonly string _source;
    private int _pos;

    public Lexer(string source)
    {
        _source = source;
    }

    private char Current => _pos < _source.Length ? _source[_pos] : '\0';
    private char Peek() => _pos + 1 < _source.Length ? _source[_pos + 1] : '\0';
    private void Advance() => _pos++;

    public IReadOnlyList<Token> Tokenise()
    {
        var tokens = new List<Token>();
        StringBuilder token = new();
        _pos = 0;

        while (_pos < _source.Length)
        {
            switch (Current)
            {
                case ' ':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    break;
                case '"':
                    tokens.Add(GetStringLiteral());
                    break;
                case '\n':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.NewLine, "", 1, _pos));
                    break;
                case '+':
                    tokens.Add(new Token(TokenType.Plus, "", 1, _pos));
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.Minus, "", 1, _pos));
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.Multiply, "", 1, _pos));
                    break;
                case '/':
                    tokens.Add(new Token(TokenType.Divide, "", 1, _pos));
                    break;
                case '(':
                    tokens.Add(new Token(TokenType.LParen, "", 1, _pos));
                    break;
                case ')':
                    tokens.Add(new Token(TokenType.RParen, "", 1, _pos));
                    break;
                default:
                    if (char.IsDigit(Current))
                    {
                        tokens.Add(GetNumberLiteral());
                    }
                    else
                    {
                        token.Append(Current);
                    }

                    break;
            }
            Advance();
        }

        if (token.Length > 0)
            tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));

        tokens.Add(new Token(TokenType.Eof, "", 1, _pos));
        return tokens;
    }

    private TokenType GetTokenType(string token)
    {
        return token.ToUpper() switch
        {
            "PRINT" => TokenType.Print,
            "LET" => TokenType.Let,
            "=" => TokenType.Eq,
            _ => token.All(c => char.IsAsciiLetterOrDigit(c)) ? TokenType.Identifier : TokenType.Unknown
        };
    }

    private Token GetStringLiteral()
    {
        StringBuilder lit = new();
        Advance(); //skip opening "
        while (Current != '"' && Current != '\0')
        {
            lit.Append(Current);
            Advance();
        }
        Advance(); //to skip closing "

        return new Token(TokenType.StringLiteral, lit.ToString(), 1, _pos);
    }

    /// <summary>
    /// Can return either Int or Float literal
    /// </summary>
    /// <returns></returns>
    private Token GetNumberLiteral()
    {
        StringBuilder lit = new();

        while (char.IsDigit(Current) || Current == '.' && Current != '\0' && Peek() != ')')
        {
            lit.Append(Current);
            Advance();
        }

        _pos--; // step back so outer Advance() re-lands on the delimiter

        var numStr = lit.ToString();

        return numStr.Contains('.') ?
         new Token(TokenType.FloatLiteral, lit.ToString(), 1, _pos) :
         new Token(TokenType.IntLiteral, lit.ToString(), 1, _pos);
    }
}