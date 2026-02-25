using SharpBasic.Ast;

namespace SharpBasic.Parser;

public class Parser(IReadOnlyList<Token> tokens)
{
    private readonly IReadOnlyList<Token> _tokens = tokens;
    private int _pos;
    private Token Eof =  new(TokenType.Eof,"",1,1);

    private Token Current => _pos < _tokens.Count ? _tokens[_pos] : Eof;
    private Token Peek() => _pos + 1 < _tokens.Count ? _tokens[_pos + 1] : Eof;
    private void Advance() => _pos++;

    public Program Parse()
    {
        var statements = new List<Statement>();
        _pos = 0;
        while(Current.Type != TokenType.Eof)
        {
            switch(Current.Type)
            {
                case TokenType.Print:
                    statements.Add(ParsePrintStatement());
                    break;
                default:
                    Advance();
                    break;
            }
        }

        return new Program(statements);
    }

    private PrintStatement ParsePrintStatement()
    {
        Advance(); //Consume PRINT

        if(Current.Type is not TokenType.StringLiteral)
            throw new InvalidOperationException(
                $"Expected StringLiteral after PRINT but got {Current.Type} at {Current.Line}:{Current.Column}");

        var value = Current.Value;
        Advance();
        return new PrintStatement(new StringLiteralExpression(value));

    }
}