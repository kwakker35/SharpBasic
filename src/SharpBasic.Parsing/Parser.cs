using SharpBasic.Ast;

namespace SharpBasic.Parsing;

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
                    var ppsRes = ParsePrintStatement();
                    if (ppsRes is ParseStatementSuccess ps)
                    {
                        statements.Add(ps.Statement);
                    }
                    else if (ppsRes is ParseStatementFailure pf)
                    {
                        errors.Add(new ParseError(pf.Error.Exception, pf.Error.Line, pf.Error.Col));
                    }
                    break;
                case TokenType.Let:
                    var plsRes = ParseLetStatement();
                    if (plsRes is ParseStatementSuccess ls)
                    {
                        statements.Add(ls.Statement);
                    }
                    else if (plsRes is ParseStatementFailure lf)
                    {
                        errors.Add(new ParseError(lf.Error.Exception, lf.Error.Line, lf.Error.Col));
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

    private ParseStatementResult ParseLetStatement()
    {
        var letLoc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume LET

        if (Current.Type is not TokenType.Identifier)
        {
            Advance();
            var err = new ParseStatementError(
                            new InvalidOperationException(
                            $"Expected Identifier after LET but got {Current.Type} at {Current.Line}:{Current.Column}")
                            , Current.Line, Current.Column
                        );
            return new ParseStatementFailure(err);
        }

        var ident = Current.Value;
        var identLoc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume Identifier

        if (Current.Type is not TokenType.Eq)
        {
            Advance();
            var err = new ParseStatementError(
                            new InvalidOperationException(
                            $"Expected = after LET <identifier> but got {Current.Type} at {Current.Line}:{Current.Column}")
                            , Current.Line, Current.Column
                        );
            return new ParseStatementFailure(err);
        }

        Advance(); //consume =

        if (Current.Type is not TokenType.StringLiteral)
        {
            Advance();
            var err = new ParseStatementError(
                            new InvalidOperationException(
                            $"Expected StringLiteral after LET <identifier> = but got {Current.Type} at {Current.Line}:{Current.Column}")
                            , Current.Line, Current.Column
                        );
            return new ParseStatementFailure(err);
        }

        var value = Current.Value;
        var valueLoc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume value

        return new ParseStatementSuccess(
            new LetStatement(
                new Token(TokenType.Identifier, ident, identLoc.Line, identLoc.Col),
                new StringLiteralExpression(value, valueLoc),
                letLoc
            )
        );

    }
}