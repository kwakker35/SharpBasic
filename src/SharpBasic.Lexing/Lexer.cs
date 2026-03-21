using System.Text;
using SharpBasic.Ast;

namespace SharpBasic.Lexing;

public class Lexer
{
    private readonly string _source;
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    public Lexer(string source)
    {
        _source = source.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private char Current => _pos < _source.Length ? _source[_pos] : '\0';
    private char Peek() => _pos + 1 < _source.Length ? _source[_pos + 1] : '\0';
    private void Advance()
    {
        if (_pos < _source.Length && _source[_pos] == '\n')
        {
            _line++;
            _col = 1;
        }
        else
        {
            _col++;
        }
        _pos++;
    }

    public IReadOnlyList<Token> Tokenise()
    {
        var startCol = _pos;
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

                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    break;
                case '"':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(GetStringLiteral());
                    break;
                case '\n':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.NewLine, "", _line, _col));
                    break;
                case '+':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Plus, "", _line, _col));
                    break;
                case '-':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Minus, "", _line, _col));
                    break;
                case '*':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Multiply, "", _line, _col));
                    break;
                case '/':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Divide, "", _line, _col));
                    break;
                case '(':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.LParen, "", _line, _col));
                    break;
                case ')':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.RParen, "", _line, _col));
                    break;
                case '[':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.LBracket, "", _line, _col));
                    break;
                case ']':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.RBracket, "", _line, _col));
                    break;
                case '<':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    if (Peek() == '=') { Advance(); tokens.Add(new Token(TokenType.LtEq, "", _line, _col)); }
                    else if (Peek() == '>') { Advance(); tokens.Add(new Token(TokenType.NotEq, "", _line, _col)); }
                    else tokens.Add(new Token(TokenType.Lt, "", _line, _col));
                    break;
                case '>':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    if (Peek() == '=') { Advance(); tokens.Add(new Token(TokenType.GtEq, "", _line, _col)); }
                    else tokens.Add(new Token(TokenType.Gt, "", _line, _col));
                    break;
                case '&':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Ampersand, "", _line, _col)); ;
                    break;
                case ',':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Comma, "", _line, _col));
                    break;
                case ';':
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                        token = new();
                    }
                    tokens.Add(new Token(TokenType.Semicolon, "", _line, _col));
                    break;
                default:
                    if (char.IsDigit(Current))
                    {
                        if (token.Length > 0)
                        {
                            tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));
                            token = new();
                        }
                        tokens.Add(GetNumberLiteral());
                    }
                    else if (Current == '$' && !char.IsAsciiLetterOrDigit(Peek()))
                    {
                        token.Append(Current);
                    }
                    else if (Current == '$' && char.IsAsciiLetterOrDigit(Peek()))
                    {
                        tokens.Add(new Token(TokenType.Unknown, "", _line, _col));
                        token = new();
                    }
                    else
                    {
                        if (token.Length == 0)
                            startCol = _col;

                        token.Append(Current);
                    }

                    break;
            }
            Advance();
        }

        if (token.Length > 0)
            tokens.Add(new Token(GetTokenType(token.ToString()), token.ToString(), _line, startCol));

        tokens.Add(new Token(TokenType.Eof, "", _line, _col));
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
        return token.ToUpperInvariant() switch
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
            "INPUT" => TokenType.Input,
            _ => token.All(c => char.IsAsciiLetterOrDigit(c)) || token.EndsWith('$') ?
                        TokenType.Identifier : TokenType.Unknown
        };
    }

    private Token GetStringLiteral()
    {
        var startCol = _col;
        StringBuilder lit = new();
        Advance(); //skip opening "
        while (Current != '"' && Current != '\0')
        {
            lit.Append(Current);
            Advance();
        }

        return new Token(TokenType.StringLiteral, lit.ToString(), _line, startCol);
    }

    /// <summary>
    /// Can return either Int or Float literal
    /// </summary>
    /// <returns></returns>
    private Token GetNumberLiteral()
    {
        var startCol = _col;
        StringBuilder lit = new();

        while (char.IsDigit(Current) || Current == '.' && Current != '\0' && Peek() != ')')
        {
            lit.Append(Current);
            Advance();
        }

        _pos--; // step back so outer Advance() re-lands on the delimiter

        var numStr = lit.ToString();

        return numStr.Contains('.') ?
         new Token(TokenType.FloatLiteral, lit.ToString(), _line, startCol) :
         new Token(TokenType.IntLiteral, lit.ToString(), _line, startCol);
    }
}