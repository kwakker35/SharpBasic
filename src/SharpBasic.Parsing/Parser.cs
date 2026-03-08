using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public class Parser(IReadOnlyList<Token> tokens)
{
    private readonly IReadOnlyList<Token> _tokens = tokens;
    private int _pos = 0;

    List<ParseError> errors = [];

    private Token Eof = new(TokenType.Eof, "", 1, 1);

    private Token Current => _pos < _tokens.Count ? _tokens[_pos] : Eof;

    private Token Peek() => _pos + 1 < _tokens.Count ? _tokens[_pos + 1] : Eof;

    private void Advance() => _pos++;

    public ParseResult Parse()
    {
        List<Statement> statements = [];

        while (Current.Type != TokenType.Eof)
        {
            ParseStatement(statements);
        }

        return errors.Count > 0
            ? new ParseFailure(errors)
            : new ParseSuccess(new Program(statements));
    }
    private void ParseStatement(List<Statement> target)
    {
        switch (Current.Type)
        {
            case TokenType.NewLine:
                Advance(); //consume token
                break;
            case TokenType.Print:
                var ppsRes = ParsePrintStatement();
                if (ppsRes is ParseStatementSuccess ps)
                {
                    target.Add(ps.Statement);
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
                    target.Add(ls.Statement);
                }
                else if (plsRes is ParseStatementFailure lf)
                {
                    errors.Add(new ParseError(lf.Error.Exception, lf.Error.Line, lf.Error.Col));
                }
                break;
            case TokenType.If:
                var pisRes = ParseIfStatement();
                if (pisRes is ParseStatementSuccess ifs)
                {
                    target.Add(ifs.Statement);
                }
                else if (pisRes is ParseStatementFailure lf)
                {
                    errors.Add(new ParseError(lf.Error.Exception, lf.Error.Line, lf.Error.Col));
                }
                break;
            case TokenType.While:
                var pwsRes = ParseWhileStatement();
                if (pwsRes is ParseStatementSuccess ws)
                {
                    target.Add(ws.Statement);
                }
                else if (pwsRes is ParseStatementFailure lf)
                {
                    errors.Add(new ParseError(lf.Error.Exception, lf.Error.Line, lf.Error.Col));
                }
                break;
            default:
                errors.Add(
                    new ParseError(
                        new InvalidOperationException(
                            $"Unexpected token '{Current.Value}' ({Current.Type})"
                        ),
                        Current.Line,
                        Current.Column
                    )
                );
                Advance();
                break;
        }
    }
    private ParseStatementResult ParsePrintStatement()
    {
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //Consume PRINT

        var expr = ParseExpression();

        if (expr is null)
        {
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected expression after PRINT but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line, Current.Column
            );
            return new ParseStatementFailure(err);
        }

        return new ParseStatementSuccess(new PrintStatement(expr, loc));
    }

    private ParseStatementResult ParseWhileStatement()
    {
        var wLoc = new SourceLocation(Current.Line, Current.Column);
        List<Statement> body = [];

        Advance(); //consume WHILE

        var condition = ParseExpression();

        if (condition is null)
        {
            Advance();
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected condition after WHILE but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        //break on ELSE or END
        while (Current.Type is not TokenType.Wend &&
                Current.Type is not TokenType.Eof)
        {
            ParseStatement(body);
        }

        Advance(); //consume WEND

        return new ParseStatementSuccess(
            new WhileStatement(
                condition,
                body,
                wLoc
                )
        );
    }

    private ParseStatementResult ParseIfStatement()
    {
        var ifLoc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume IF

        var condition = ParseExpression();

        if (condition is null)
        {
            Advance();
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected condition after IF but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        List<Statement> thenBlock = [];
        List<Statement> elseBlock = [];

        Advance(); //consume Then
        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        //break on ELSE or END
        while (Current.Type is not TokenType.Else &&
                Current.Type is not TokenType.End &&
                Current.Type is not TokenType.Eof)
        {
            ParseStatement(thenBlock);
        }

        //ELSE Block
        if (Current.Type is TokenType.Else &&
                Current.Type is not TokenType.Eof)
        {
            Advance(); //consume Else
            if (Current.Type is TokenType.NewLine)
                Advance(); //consume NewLine

            //break on END
            while (Current.Type is not TokenType.End)
            {
                ParseStatement(elseBlock);
            }
        }

        //expecting END IF
        if (Current.Type is TokenType.End && Peek().Type is not TokenType.If)
        {
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected IF after END but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        Advance(); //consume END
        Advance(); //consume IF

        return new ParseStatementSuccess(
            new IfStatement(
                condition,
                thenBlock,
                elseBlock.Count > 0 ? elseBlock : null,
                ifLoc
                )
        );
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
                    $"Expected Identifier after LET but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
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
                    $"Expected = after LET <identifier> but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        Advance(); //consume =

        var expr = ParseExpression();

        if (expr is null)
        {
            Advance();
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected value after LET <identifier> = but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        return new ParseStatementSuccess(
            new LetStatement(
                new Token(TokenType.Identifier, ident, identLoc.Line, identLoc.Col),
                expr,
                letLoc
            )
        );
    }

    private Expression? ParseExpression(int minBindingPower = 0)
    {
        var loc = new SourceLocation(Current.Line, Current.Column);

        //Parse the left-hand side (a "prefix" - literal, identifier or grouped expr)
        Expression? left;

        if (Current.Type == TokenType.LParen)
        {
            Advance(); // consume (
            left = ParseExpression(0); //parse inner expression with reset binding power
            if (Current.Type != TokenType.RParen)
                return null; //unmatched paren - error
            Advance(); //consume )
        }
        else
        {
            left = ParsePrimary();
            if (left is null) return null;
            Advance(); //consume the primary token
        }

        //Pratt loop - keep consuming operators while they bind tighter than minBindingPower
        while (true)
        {
            var bp = BindingPower(Current.Type);
            if (bp <= minBindingPower) break;

            var op = Current;
            Advance(); //consume operator
            var right = ParseExpression(bp); // recurse with THIS operator's binding power
            if (right is null) return null;

            left = new BinaryExpression(left!, op, right, loc);
        }

        return left;

    }

    private Expression? ParsePrimary()
    {
        var loc = new SourceLocation(Current.Line, Current.Column);

        if (Current.Type == TokenType.StringLiteral)
            return new StringLiteralExpression(Current.Value, loc);

        if (Current.Type == TokenType.IntLiteral && int.TryParse(Current.Value, out var i))
            return new IntLiteralExpression(i, loc);

        if (Current.Type == TokenType.FloatLiteral && double.TryParse(Current.Value, out var f))
            return new FloatLiteralExpression(f, loc);

        if (Current.Type == TokenType.Identifier)
            return new IdentifierExpression(Current.Value, loc);

        return null;
    }

    private static int BindingPower(TokenType type) => type switch
    {
        TokenType.Eq or TokenType.NotEq or TokenType.Lt or
            TokenType.Gt or TokenType.LtEq or TokenType.GtEq => 5,
        TokenType.Plus or TokenType.Minus => 10,
        TokenType.Multiply or TokenType.Divide => 20,
        _ => 0
    };
}
