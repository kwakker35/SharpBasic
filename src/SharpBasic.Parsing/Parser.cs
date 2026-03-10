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

    private static int BindingPower(TokenType type) => type switch
    {
        TokenType.Eq or TokenType.NotEq or TokenType.Lt or
            TokenType.Gt or TokenType.LtEq or TokenType.GtEq => 5,
        TokenType.Plus or TokenType.Minus or TokenType.Ampersand => 10,
        TokenType.Multiply or TokenType.Divide => 20,
        _ => 0
    };

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
        {
            var identExpr = new IdentifierExpression(Current.Value, loc);
            // Phase 7: peek ahead — if next token is (, parse call expression
            return identExpr;
        }

        return null;
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

    private void ParseStatement(List<Statement> target)
    {
        switch (Current.Type)
        {
            case TokenType.NewLine:
                Advance(); //consume token
                break;
            case TokenType.Print:
                AddStatement(target, ParsePrintStatement());
                break;
            case TokenType.Let:
                AddStatement(target, ParseLetStatement());
                break;
            case TokenType.If:
                AddStatement(target, ParseIfStatement());
                break;
            case TokenType.While:
                AddStatement(target, ParseWhileStatement());
                break;
            case TokenType.For:
                AddStatement(target, ParseForStatement());
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

    private void AddStatement(List<Statement> target, ParseStatementResult ppsRes)
    {
        if (ppsRes is ParseStatementSuccess ps)
        {
            target.Add(ps.Statement);
        }
        else if (ppsRes is ParseStatementFailure pf)
        {
            errors.Add(new ParseError(pf.Error.Exception, pf.Error.Line, pf.Error.Col));
        }
    }

    private ParseStatementFailure? ExpectToken(TokenType expected, string context)
    {
        if (Current.Type != expected)
        {
            Advance();
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected {expected} {context} but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        //success
        return null;
    }

    private ParseStatementFailure? ExpectExpression(Expression? expected, string context)
    {
        if (expected is null)
        {
            Advance();
            var err = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected {context} but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(err);
        }

        //success
        return null;
    }

    private ParseStatementResult ParseForStatement()
    {
        ParseStatementFailure? err;
        Expression? stepExpression = null; //optional
        Token? nextVar = null; //optional
        List<Statement> body = [];
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //Consume FOR

        //expecting LoopVar Identifier
        err = ExpectToken(TokenType.Identifier, "after FOR");
        if (err is not null) return err;

        var loopVar = Current;
        Advance(); //consume Identifier

        //expecting =
        err = ExpectToken(TokenType.Eq, "after Loop Variable Indendifier");
        if (err is not null) return err;

        Advance(); //consume =

        var startExpr = ParseExpression();

        //expecting Start expression
        err = ExpectExpression(startExpr, "Start expression after =");
        if (err is not null) return err;

        //expecting To
        err = ExpectToken(TokenType.To, "after Start expression");
        if (err is not null) return err;

        Advance(); //consume To

        var limitExpr = ParseExpression();

        //expecting Limit expression
        err = ExpectExpression(limitExpr, "Limit expression after To");
        if (err is not null) return err;

        //optional STEP
        if (Current.Type is TokenType.Step)
        {
            Advance(); //consume STEP
            stepExpression = ParseExpression();

            //expecting STEP expression
            err = ExpectExpression(stepExpression, "value after STEP");
            if (err is not null) return err;
        }

        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        //break on NEXT
        while (Current.Type is not TokenType.Next &&
                Current.Type is not TokenType.Eof)
        {
            ParseStatement(body);
        }

        Advance(); //consume NEXT

        if (Current.Type is TokenType.Identifier)
        {
            if (Current != loopVar)
            {
                var errVar = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected {loopVar.Value} but got {Current.Value} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
                return new ParseStatementFailure(errVar);
            }

            nextVar = new Token(TokenType.NextVar,
                                Current.Value,
                                Current.Line,
                                Current.Column);

            Advance();// consume Identifer after NEXT
        }

        return new ParseStatementSuccess(
            new ForStatement(
                loopVar,
                startExpr,
                limitExpr,
                stepExpression,
                nextVar,
                body,
                loc
                )
        );
    }

    private ParseStatementResult ParsePrintStatement()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //Consume PRINT

        var expr = ParseExpression();

        err = ExpectExpression(expr, "expression after PRINT");
        if (err is not null) return err;

        return new ParseStatementSuccess(new PrintStatement(expr, loc));
    }

    private ParseStatementResult ParseWhileStatement()
    {
        ParseStatementFailure? err;
        var wLoc = new SourceLocation(Current.Line, Current.Column);
        List<Statement> body = [];

        Advance(); //consume WHILE

        var condition = ParseExpression();
        err = ExpectExpression(condition, "condition after WHILE");
        if (err is not null) return err;

        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        //break on WEND
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
        ParseStatementFailure? err;
        var ifLoc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume IF

        var condition = ParseExpression();

        err = ExpectExpression(condition, "condition after IF");
        if (err is not null) return err;

        List<Statement> thenBlock = [];
        List<Statement> elseBlock = [];

        //expecting THEN
        err = ExpectToken(TokenType.Then, "after condition");
        if (err is not null) return err;

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
            var errEnd = new ParseStatementError(
                new InvalidOperationException(
                    $"Expected IF after END but got {Current.Type} at {Current.Line}:{Current.Column}"
                ),
                Current.Line,
                Current.Column
            );
            return new ParseStatementFailure(errEnd);
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
        ParseStatementFailure? err;
        var letLoc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume LET

        err = ExpectToken(TokenType.Identifier, "after LET");
        if (err is not null) return err;

        var ident = Current;
        Advance(); //consume Identifier

        err = ExpectToken(TokenType.Eq, "after LET <identifier>");
        if (err is not null) return err;

        Advance(); //consume =

        var expr = ParseExpression();

        err = ExpectExpression(expr, "value after LET <identifier> =");
        if (err is not null) return err;

        return new ParseStatementSuccess(
            new LetStatement(
                ident,
                expr,
                letLoc
            )
        );
    }

}
