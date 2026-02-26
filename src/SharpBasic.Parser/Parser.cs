using SharpBasic.Ast;

namespace SharpBasic.Parser;

public class Parser(IReadOnlyList<Token> tokens)
{
    private readonly IReadOnlyList<Token> _tokens = tokens;
    private int _pos;
    private Token Eof = new(TokenType.Eof, "", 1, 1);

    private Token Current => _pos < _tokens.Count ? _tokens[_pos] : Eof;
    private Token Peek() => _pos + 1 < _tokens.Count ? _tokens[_pos + 1] : Eof;
    private void Advance() => _pos++;

    public ParseResult Parse()
    {
        var statements = new List<Statement>();
        var errors = new List<ParseError>();

        _pos = 0;
        while (Current.Type != TokenType.Eof)
        {
            switch (Current.Type)
            {
                case TokenType.Print:
                    var result = ParsePrintStatement();
                    if (result is ParseStatementSuccess s)
                    {
                        statements.Add(s.Statement);
                    }
                    else if (result is ParseStatementFailure f)
                    {
                        errors.Add(new ParseError(f.Error.Exception, f.Error.Line, f.Error.Col));
                    }
                    break;
                default:
                    errors.Add(new ParseError(
                        new InvalidOperationException($"Unexpected token '{Current.Value}' ({Current.Type})"),
                        Current.Line, Current.Column));
                    Advance();
                    break;
            }
        }

        return errors.Count > 0 ?
                new ParseFailure(errors) :
                new ParseSuccess(new Program(statements));
    }

    private ParseStatementResult ParsePrintStatement()
    {
        Advance(); //Consume PRINT

        if (Current.Type is not TokenType.StringLiteral)
        {
            Advance();
            var err = new ParseStatementError(
                            new InvalidOperationException(
                            $"Expected StringLiteral after PRINT but got {Current.Type} at {Current.Line}:{Current.Column}")
                            , Current.Line, Current.Column
                        );
            return new ParseStatementFailure(err);
        }

        var value = Current.Value;
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance();
        return new ParseStatementSuccess(
                    new PrintStatement(
                    new StringLiteralExpression(value, loc), loc
                    ));

    }
}