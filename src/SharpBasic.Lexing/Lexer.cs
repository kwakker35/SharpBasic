using System.Text;
using SharpBasic.Ast;

namespace SharpBasic.Lexing;

public class Lexer
{
    private readonly string _source;
    private int _pos;

    public Lexer(string source)
    {
        _source = source.Replace("\r\n", "\n").Replace("\r", "\n");
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
                        if (token.ToString() == "REM")
                        {
                            ConsumeComment();
                            token = new();
                            break;
                        }

                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    break;
                case '"':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
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
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Plus, "", 1, _pos));
                    break;
                case '-':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Minus, "", 1, _pos));
                    break;
                case '*':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Multiply, "", 1, _pos));
                    break;
                case '/':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Divide, "", 1, _pos));
                    break;
                case '(':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.LParen, "", 1, _pos));
                    break;
                case ')':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.RParen, "", 1, _pos));
                    break;
                case '[':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.LBracket, "", 1, _pos));
                    break;
                case ']':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.RBracket, "", 1, _pos));
                    break;
                case '<':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    if (Peek() == '=') { Advance(); tokens.Add(new Token(TokenType.LtEq, "", 1, _pos)); }
                    else if (Peek() == '>') { Advance(); tokens.Add(new Token(TokenType.NotEq, "", 1, _pos)); }
                    else tokens.Add(new Token(TokenType.Lt, "", 1, _pos));
                    break;
                case '>':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    if (Peek() == '=') { Advance(); tokens.Add(new Token(TokenType.GtEq, "", 1, _pos)); }
                    else tokens.Add(new Token(TokenType.Gt, "", 1, _pos));
                    break;
                case '&':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Ampersand, "", 1, _pos));
                    break;
                case ',':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Comma, "", 1, _pos));
                    break;
                default:
                    if (char.IsDigit(Current))
                    {
                        if (token.Length > 0)
                        {
                            tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), 1, _pos));
                            token = new();
                        }
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

    private void ConsumeComment()
    {
        //consume everything up to next \n
        while (true)
        {
            if (Peek() == '\n' || Peek() == '\0')
                break;

            Advance();
        }
    }

    private static TokenType GetTokenType(string token)
    {
        return token.ToUpper() switch
        {
            "PRINT" => TokenType.Print,
            "LET" => TokenType.Let,
            "=" => TokenType.Eq,
            "IF" => TokenType.If,
            "THEN" => TokenType.Then,
            "ELSE" => TokenType.Else,
            "END" => TokenType.End,
            "WHILE" => TokenType.While,
            "WEND" => TokenType.Wend,
            "FOR" => TokenType.For,
            "TO" => TokenType.To,
            "STEP" => TokenType.Step,
            "NEXT" => TokenType.Next,
            "FUNCTION" => TokenType.Function,
            "SUB" => TokenType.Sub,
            "RETURN" => TokenType.Return,
            "CALL" => TokenType.Call,
            "AS" => TokenType.As,
            "AND" => TokenType.And,
            "OR" => TokenType.Or,
            "NOT" => TokenType.Not,
            "INTEGER" => TokenType.Integer,
            "FLOAT" => TokenType.Float,
            "STRING" => TokenType.String,
            "BOOLEAN" => TokenType.Boolean,
            "TRUE" => TokenType.True,
            "FALSE" => TokenType.False,
            "DIM" => TokenType.Dim,
            "MOD" => TokenType.Mod,
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