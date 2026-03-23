using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public class Parser(IReadOnlyList<Token> tokens)
{
    private readonly IReadOnlyList<Token> _tokens = tokens;
    private int _pos = 0;
    private bool _inSubOrFunction = false;

    private readonly List<Diagnostic> _diagnostics = [];

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

        return _diagnostics.Count > 0
            ? new ParseFailure(_diagnostics)
            : new ParseSuccess(new Program(statements));
    }

    private static int BindingPower(TokenType type) => type switch
    {
        TokenType.Or => 2,
        TokenType.And => 3,
        TokenType.Eq or TokenType.NotEq or TokenType.Lt or
            TokenType.Gt or TokenType.LtEq or TokenType.GtEq => 5,
        TokenType.Plus or TokenType.Minus or TokenType.Ampersand => 10,
        TokenType.Multiply or TokenType.Divide or TokenType.Mod => 20,
        _ => 0
    };

    private Expression? ParsePrimary()
    {
        var loc = new SourceLocation(Current.Line, Current.Column);

        if (Current.Type == TokenType.StringLiteral)
            return new StringLiteralExpression(Current.Value, loc);

        if (Current.Type == TokenType.IntLiteral && int.TryParse(Current.Value, out var i))
            return new IntLiteralExpression(i, loc);

        if (Current.Type == TokenType.FloatLiteral &&
            double.TryParse(Current.Value,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var f))
            return new FloatLiteralExpression(f, loc);

        if (Current.Type == TokenType.True || Current.Type == TokenType.False)
            return new BoolLiteralExpression(Current.Type == TokenType.True, loc);

        if (Current.Type == TokenType.Identifier)
        {
            if (Peek().Type == TokenType.LParen)
                return ParseCallExpression();   // Foo(...)

            if (Peek().Type == TokenType.LBracket)
                return ParseArrayAccessExpression();   // Foo[...]

            var identExpr = new IdentifierExpression(Current.Value, loc);
            return identExpr;
        }

        return null;
    }

    private Expression? ParseExpression(int minBindingPower = 0)
    {
        ParseStatementFailure? err;
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
            if (Current.Type == TokenType.Minus || Current.Type == TokenType.Not)
            {
                var minusToken = Current;
                Advance(); // consume the unary operator
                var operand = ParseExpression(0);
                err = ExpectExpression(operand, "expression after unary operator");
                if (err is not null) return null;

                return new UnaryExpression(minusToken, operand!, loc);
            }
            else
            {
                left = ParsePrimary();
                if (left is null) return null;
                if (left is not CallExpression)
                    Advance(); //consume the primary token
            }
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
            case TokenType.Sub:
                AddStatement(target, ParseSubDeclaration());
                break;
            case TokenType.Function:
                AddStatement(target, ParseFunctionDeclaration());
                break;
            case TokenType.Return:
                AddStatement(target, ParseReturnStatement());
                break;
            case TokenType.Call:
                AddStatement(target, ParseCallStatement());
                break;
            case TokenType.Dim:
                AddStatement(target, ParseDimStatement());
                break;
            case TokenType.Const:
                AddStatement(target, ParseConstStatement());
                break;
            case TokenType.Input:
                AddStatement(target, ParseInputStatement());
                break;
            case TokenType.Select:
                AddStatement(target, ParseSelectCaseStatement());
                break;
            case TokenType.Set:
                AddStatement(target, ParseSetGlobalStatement());
                break;
            default:
                _diagnostics.Add(
                    new Diagnostic(
                        Current.Line,
                        Current.Column,
                        $"Unexpected token '{Current.Value}' ({Current.Type})",
                        DiagnosticSeverity.Error
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
            _diagnostics.Add(pf.Diagnostic);
        }
    }

    private ParseStatementResult ParseSetGlobalStatement()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance(); // consume SET

        err = ExpectToken(TokenType.Global, "after SET");
        if (err is not null) return err;
        Advance(); // consume GLOBAL

        err = ExpectToken(TokenType.Identifier, "identifier after SET GLOBAL");
        if (err is not null) return err;
        var ident = Current.Value;
        Advance(); // consume identifier

        err = ExpectToken(TokenType.Eq, "= after SET GLOBAL <identifier>");
        if (err is not null) return err;
        Advance(); // consume =

        var value = ParseExpression();
        err = ExpectExpression(value, "expression after SET GLOBAL <identifier> =");
        if (err is not null) return err;

        return new ParseStatementSuccess(new SetGlobalStatement(ident, value!, loc));
    }

    private ParseStatementResult ParseSelectCaseStatement()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance(); // consume SELECT

        // SELECT must be followed by CASE
        err = ExpectToken(TokenType.Case, "after SELECT");
        if (err is not null) return err;
        Advance(); // consume CASE

        var subject = ParseExpression();
        err = ExpectExpression(subject, "subject expression after SELECT CASE");
        if (err is not null) return err;

        if (Current.Type is TokenType.NewLine)
            Advance(); // consume NewLine

        List<CaseClause> cases = [];
        CaseClause? elseClause = null;

        while (Current.Type is TokenType.Case)
        {
            var caseLoc = new SourceLocation(Current.Line, Current.Column);
            Advance(); // consume CASE

            if (Current.Type is TokenType.Else)
            {
                // CASE ELSE
                Advance(); // consume ELSE
                if (Current.Type is TokenType.NewLine)
                    Advance(); // consume NewLine

                List<Statement> elseBody = [];
                while (Current.Type is not TokenType.Case &&
                       Current.Type is not TokenType.End &&
                       Current.Type is not TokenType.Eof)
                {
                    ParseStatement(elseBody);
                }

                elseClause = new CaseClause([], elseBody, caseLoc);

                // CASE ELSE must be last - error if another CASE follows
                if (Current.Type is TokenType.Case)
                {
                    return new ParseStatementFailure(
                        new Diagnostic(
                            Current.Line,
                            Current.Column,
                            $"CASE clause found after CASE ELSE at {Current.Line}:{Current.Column}",
                            DiagnosticSeverity.Error
                        )
                    );
                }

                break;
            }
            else
            {
                // regular CASE with one or more comma-separated values
                List<Expression> values = [];

                var val = ParseExpression();
                err = ExpectExpression(val, "value after CASE");
                if (err is not null) return err;
                values.Add(val!);

                while (Current.Type is TokenType.Comma)
                {
                    Advance(); // consume comma
                    val = ParseExpression();
                    err = ExpectExpression(val, "value after ','");
                    if (err is not null) return err;
                    values.Add(val!);
                }

                if (Current.Type is TokenType.NewLine)
                    Advance(); // consume NewLine

                List<Statement> body = [];
                while (Current.Type is not TokenType.Case &&
                       Current.Type is not TokenType.End &&
                       Current.Type is not TokenType.Eof)
                {
                    ParseStatement(body);
                }

                cases.Add(new CaseClause(values, body, caseLoc));
            }
        }

        // must end with END SELECT
        if (Current.Type is TokenType.Eof)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    $"Expected END SELECT but reached end of file",
                    DiagnosticSeverity.Error
                )
            );
        }

        if (Current.Type is not TokenType.End || Peek().Type is not TokenType.Select)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    $"Expected END SELECT but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error
                )
            );
        }

        Advance(); // consume END
        Advance(); // consume SELECT

        return new ParseStatementSuccess(
            new SelectCaseStatement(subject!, cases, elseClause, loc)
        );
    }

    private ParseStatementFailure? ExpectToken(TokenType expected, string context)
    {
        if (Current.Type != expected)
        {
            Advance();
            var err = new Diagnostic(
                Current.Line,
                Current.Column,
                 $"Expected {expected} {context} but got {Current.Type} at {Current.Line}:{Current.Column}",
                 DiagnosticSeverity.Error
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
            var err = new Diagnostic(
                Current.Line,
                Current.Column,
                $"Expected {context} but got {Current.Type} at {Current.Line}:{Current.Column}",
                DiagnosticSeverity.Error
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
                var errVar = new Diagnostic(
                Current.Line,
                Current.Column,
                $"Expected {loopVar.Value} but got {Current.Value} at {Current.Line}:{Current.Column}",
                DiagnosticSeverity.Error
            );
                return new ParseStatementFailure(errVar);
            }

            nextVar = Current;

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

    private ParseStatementResult ParseDimStatement()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);
        string name = string.Empty;
        int size = 0;
        string typeName = string.Empty;

        Advance(); //Consume DIM

        //expecting Identifer
        err = ExpectToken(TokenType.Identifier, "<Identifier> after DIM");
        if (err is not null) return err;

        name = Current.Value;
        Advance(); //consume Identifier

        //expecting [
        err = ExpectToken(TokenType.LBracket, "[ after <Identifier>");
        if (err is not null) return err;
        Advance(); //consume [

        //expecting Identifer
        err = ExpectToken(TokenType.IntLiteral, "Integer as array size.");
        if (err is not null) return err;

        var sizeValid = int.TryParse(Current.Value, out size);

        if (!sizeValid)
        {
            var errEnd = new Diagnostic(
            Current.Line,
            Current.Column,
            $"Expected TYPE after AS but got {Current.Type} at {Current.Line}:{Current.Column}",
            DiagnosticSeverity.Error
            );
            return new ParseStatementFailure(errEnd);
        }

        Advance(); //consume size expression

        //expecting ]
        err = ExpectToken(TokenType.RBracket, "] after array size");
        if (err is not null) return err;
        Advance(); //consume ]

        // 2D: if next token is [ it's DIM name[rows][cols]
        if (Current.Type == TokenType.LBracket)
        {
            Advance(); //consume [

            err = ExpectToken(TokenType.IntLiteral, "Integer as 2D column size.");
            if (err is not null) return err;

            var colsValid = int.TryParse(Current.Value, out int cols);
            if (!colsValid)
            {
                return new ParseStatementFailure(new Diagnostic(
                    Current.Line, Current.Column,
                    $"Expected integer column size but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error));
            }
            Advance(); //consume cols size

            err = ExpectToken(TokenType.RBracket, "] after 2D column size");
            if (err is not null) return err;
            Advance(); //consume ]

            err = ExpectToken(TokenType.As, "AS after 2D array size");
            if (err is not null) return err;
            Advance(); //consume AS

            if (Current.Type is not TokenType.Integer &&
                    Current.Type is not TokenType.Float &&
                    Current.Type is not TokenType.String &&
                    Current.Type is not TokenType.Boolean)
            {
                return new ParseStatementFailure(new Diagnostic(
                    Current.Line, Current.Column,
                    $"Expected TYPE after AS but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error));
            }

            typeName = Current.Type.ToString();
            Advance(); //consume type

            return new ParseStatementSuccess(new Dim2dStatement(name, typeName, size, cols, loc));
        }

        //expecting AS
        err = ExpectToken(TokenType.As, "AS after array size");
        if (err is not null) return err;
        Advance(); //consume As

        if (Current.Type is not TokenType.Integer &&
                Current.Type is not TokenType.Float &&
                Current.Type is not TokenType.String &&
                Current.Type is not TokenType.Boolean)
        {
            var errEnd = new Diagnostic(
            Current.Line,
            Current.Column,
            $"Expected TYPE after AS but got {Current.Type} at {Current.Line}:{Current.Column}",
            DiagnosticSeverity.Error
            );
            return new ParseStatementFailure(errEnd);
        }

        typeName = Current.Type.ToString();
        Advance(); //consume  type

        return new ParseStatementSuccess(
                                new DimStatement(
                                    name,
                                    typeName,
                                    size,
                                    loc
                                )
        );
    }

    private ParseStatementResult ParseConstStatement()
    {
        var loc = new SourceLocation(Current.Line, Current.Column);

        if (_inSubOrFunction)
        {
            Advance(); // consume CONST so the body loop can progress
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    "CONST declarations are not allowed inside SUB or FUNCTION.",
                    DiagnosticSeverity.Error
                )
            );
        }

        Advance(); //consume CONST

        var err = ExpectToken(TokenType.Identifier, "Identifier after CONST");
        if (err is not null) return err;

        var nameToken = Current;
        Advance(); //consume Identifier

        err = ExpectToken(TokenType.Eq, "= after CONST <Identifier>");
        if (err is not null) return err;
        Advance(); //consume =

        var valueExpr = ParseExpression();

        if (valueExpr is null)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    "Expected a literal value after CONST =.",
                    DiagnosticSeverity.Error
                )
            );
        }

        if (valueExpr is not IntLiteralExpression &&
            valueExpr is not FloatLiteralExpression &&
            valueExpr is not StringLiteralExpression &&
            valueExpr is not BoolLiteralExpression)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    loc.Line,
                    loc.Col,
                    "CONST value must be a literal (integer, float, string, or boolean).",
                    DiagnosticSeverity.Error
                )
            );
        }

        return new ParseStatementSuccess(new ConstStatement(nameToken, valueExpr, loc));
    }

    private Expression? ParseArrayAccessExpression()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);

        err = ExpectToken(TokenType.Identifier, "Identifier");
        if (err is not null) return null;
        var name = Current.Value;
        Advance(); //consume identifier

        err = ExpectToken(TokenType.LBracket, "[ after <Identifier>");
        if (err is not null) return null;
        Advance(); //consume [

        var expr = ParseExpression();

        err = ExpectExpression(expr, "expression in array access");
        if (err is not null) return null;

        // 2D: current = ], peek ahead — if next is [ it's name[row][col]
        if (Peek().Type == TokenType.LBracket)
        {
            Advance(); //consume ]
            Advance(); //consume [

            var colExpr = ParseExpression();

            err = ExpectExpression(colExpr, "column index in 2D array access");
            if (err is not null) return null;

            return new Array2dAccessExpression(name, expr, colExpr, loc);
            // caller (ParseExpression) will Advance() to consume the final ]
        }

        return new ArrayAccessExpression(
                name,
                expr,
                loc
        );
    }

    private Expression? ParseCallExpression()
    {
        ParseStatementFailure? err;

        var name = string.Empty;
        List<Expression> arguments = [];
        var loc = new SourceLocation(Current.Line, Current.Column);

        err = ExpectToken(TokenType.Identifier, "Identifier");
        if (err is not null) return null;

        name = Current.Value;
        Advance(); //consume function name

        err = ExpectToken(TokenType.LParen, "( after <Identifier>");
        if (err is not null) return null;
        Advance(); //consume (

        //loop though Aguments
        while (Current.Type is not TokenType.RParen)
        {
            var expr = ParseExpression();

            err = ExpectExpression(expr, "expression in arguments");
            if (err is not null) return null;

            if (Current.Type == TokenType.Comma)
                Advance(); //consume comma if it exsits

            arguments.Add(expr);
        }

        if (Current.Type is TokenType.RParen)
            Advance(); //consume )

        return new CallExpression(
                name,
                arguments,
                loc
        );
    }

    private ParseStatementResult ParseCallStatement()
    {
        ParseStatementFailure? err;

        var name = string.Empty;
        List<Expression> arguments = [];
        var loc = new SourceLocation(Current.Line, Current.Column);

        Advance(); //consume CALL

        err = ExpectToken(TokenType.Identifier, "Identifier after CALL");
        if (err is not null) return err;

        name = Current.Value;
        Advance(); //consume sub name

        err = ExpectToken(TokenType.LParen, "( after <Identifier>");
        if (err is not null) return err;
        Advance(); //consume (

        //loop though Aguments
        while (Current.Type is not TokenType.RParen)
        {
            var expr = ParseExpression();

            err = ExpectExpression(expr, "expression in arguments");
            if (err is not null) return err;

            if (Current.Type == TokenType.Comma)
                Advance(); //consume comma if it exsits

            arguments.Add(expr);
        }

        if (Current.Type is TokenType.RParen)
            Advance(); //consume )

        return new ParseStatementSuccess(
           new CallStatement(
                name,
                arguments,
                loc
               )
       );

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

    private ParseStatementResult ParseFunctionDeclaration()
    {
        ParseStatementFailure? err;

        var name = string.Empty;
        var returnType = string.Empty;
        List<Parameter> parameters = [];
        List<Statement> body = [];
        var loc = new SourceLocation(Current.Line, Current.Column);

        Advance(); //consume FUNCTION

        err = ExpectToken(TokenType.Identifier, "Identifier after FUNCTION");
        if (err is not null) return err;

        name = Current.Value;
        Advance(); //consume sub name

        err = ExpectToken(TokenType.LParen, "( after FUNCTION <Identifier>");
        if (err is not null) return err;
        Advance(); //consume (

        //loop though parameters
        while (Current.Type is not TokenType.RParen)
        {
            var paramName = string.Empty;
            var paramType = string.Empty;
            var paramLoc = new SourceLocation(Current.Line, Current.Column);

            err = ExpectToken(TokenType.Identifier, "Identifier at start of parameter");
            if (err is not null) return err;

            paramName = Current.Value;
            Advance(); //consume param name

            err = ExpectToken(TokenType.As, "As after parameter name");
            if (err is not null) return err;
            Advance(); //consume As

            if (Current.Type is not TokenType.Integer &&
                Current.Type is not TokenType.Float &&
                Current.Type is not TokenType.String &&
                Current.Type is not TokenType.Boolean)
            {
                return new ParseStatementFailure(
                    new Diagnostic(
                        Current.Line,
                        Current.Column,
                        $"Expected TYPE after AS but got {Current.Type} at {Current.Line}:{Current.Column}",
                        DiagnosticSeverity.Error
                    )
                );
            }

            paramType = Current.Type.ToString();
            Advance(); //consume param type, next char will be , or )

            if (Current.Type == TokenType.Comma)
                Advance(); //consume comma if it exsits

            parameters.Add(
                new Parameter(paramName, paramType, paramLoc)
            );
        }

        if (Current.Type is TokenType.RParen)
            Advance(); //consume )

        err = ExpectToken(TokenType.As, "AS after FUNCTION declaration");
        if (err is not null) return err;
        Advance(); //consume As

        if (Current.Type is not TokenType.Integer &&
                Current.Type is not TokenType.Float &&
                Current.Type is not TokenType.String &&
                Current.Type is not TokenType.Boolean)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    $"Expected return TYPE after AS but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error
                )
            );
        }

        returnType = Current.Type.ToString();
        Advance(); //consume param type

        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        //break on END
        _inSubOrFunction = true;
        while (Current.Type is not TokenType.End &&
                Current.Type is not TokenType.Eof)
        {
            ParseStatement(body);
        }
        _inSubOrFunction = false;

        //expecting END FUNCTION
        if (Current.Type is TokenType.End && Peek().Type is not TokenType.Function)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    $"Expected FUNCTION after END but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error
                )
            );
        }

        Advance(); //consume END
        Advance(); //consume SUB

        return new ParseStatementSuccess(
            new FunctionDeclaration(
                name,
                parameters,
                body,
                returnType,
                loc
                )
        );
    }

    private ParseStatementResult ParseSubDeclaration()
    {
        ParseStatementFailure? err;

        var name = string.Empty;
        List<Parameter> parameters = [];
        List<Statement> body = [];
        var loc = new SourceLocation(Current.Line, Current.Column);

        Advance(); //consume SUB

        err = ExpectToken(TokenType.Identifier, "Identifier after SUB");
        if (err is not null) return err;

        name = Current.Value;
        Advance(); //consume sub name

        err = ExpectToken(TokenType.LParen, "( after SUB <Identifier>");
        if (err is not null) return err;
        Advance(); //consume (

        //loop though parameters
        while (Current.Type is not TokenType.RParen)
        {
            var paramName = string.Empty;
            var paramType = string.Empty;
            var paramLoc = new SourceLocation(Current.Line, Current.Column);

            err = ExpectToken(TokenType.Identifier, "Identifier at start of parameter");
            if (err is not null) return err;

            paramName = Current.Value;
            Advance(); //consume param name

            err = ExpectToken(TokenType.As, "As after parameter name");
            if (err is not null) return err;
            Advance(); //consume As

            if (Current.Type is not TokenType.Integer &&
                Current.Type is not TokenType.Float &&
                Current.Type is not TokenType.String &&
                Current.Type is not TokenType.Boolean)
            {
                return new ParseStatementFailure(
                    new Diagnostic(
                        Current.Line,
                        Current.Column,
                        $"Expected TYPE after AS but got {Current.Type} at {Current.Line}:{Current.Column}",
                        DiagnosticSeverity.Error
                    )
                );
            }

            paramType = Current.Type.ToString();
            Advance(); //consume param type, next char will be , or )

            if (Current.Type == TokenType.Comma)
                Advance(); //consume comma if it exsits

            parameters.Add(
                new Parameter(paramName, paramType, paramLoc)
            );
        }

        if (Current.Type is TokenType.RParen)
            Advance(); //consume )

        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        //break on END
        _inSubOrFunction = true;
        while (Current.Type is not TokenType.End &&
                Current.Type is not TokenType.Eof)
        {
            ParseStatement(body);
        }
        _inSubOrFunction = false;

        //expecting END SUB
        if (Current.Type is TokenType.End && Peek().Type is not TokenType.Sub)
        {
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    $"Expected SUB after END but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error
                )
            );
        }

        Advance(); //consume END
        Advance(); //consume SUB

        return new ParseStatementSuccess(
            new SubDeclaration(
                name,
                parameters,
                body,
                loc
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
            return new ParseStatementFailure(
                new Diagnostic(
                    Current.Line,
                    Current.Column,
                    $"Expected IF after END but got {Current.Type} at {Current.Line}:{Current.Column}",
                    DiagnosticSeverity.Error
                )
            );
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

    private ParseStatementResult ParseReturnStatement()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);
        Advance(); //consume RETURN

        if (Current.Type is TokenType.NewLine ||
                Current.Type is TokenType.Eof)
        {
            if (Current.Type is TokenType.NewLine)
                Advance(); //consume NewLine

            // no expresstion so plain return statement
            return new ParseStatementSuccess(
                        new ReturnStatement(null, loc
                        ));
        }

        var retVal = ParseExpression();
        err = ExpectExpression(retVal, "expression after RETURN");
        if (err is not null) return err;

        if (Current.Type is TokenType.NewLine)
            Advance(); //consume NewLine

        // no expresstion so plain return statement
        return new ParseStatementSuccess(
                    new ReturnStatement(retVal, loc
                    ));

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

        if (Current.Type == TokenType.LBracket)
        {
            Advance(); //consume [

            var exprIndex = ParseExpression();

            err = ExpectExpression(exprIndex, "value after LET <identifier>[");
            if (err is not null) return err;

            err = ExpectToken(TokenType.RBracket, "] after array index expression");
            if (err is not null) return err;
            Advance();// consume ]

            // 2D: if next token is [ it's LET name[row][col] = value
            if (Current.Type == TokenType.LBracket)
            {
                Advance(); //consume [

                var exprColIndex = ParseExpression();

                err = ExpectExpression(exprColIndex, "column index after LET <identifier>[row][");
                if (err is not null) return err;

                err = ExpectToken(TokenType.RBracket, "] after 2D column index");
                if (err is not null) return err;
                Advance(); //consume ]

                err = ExpectToken(TokenType.Eq, "= after LET <identifier>[row][col]");
                if (err is not null) return err;
                Advance(); //consume =

                var exprValue2d = ParseExpression();

                err = ExpectExpression(exprValue2d, "value after LET <identifier>[row][col] =");
                if (err is not null) return err;

                return new ParseStatementSuccess(
                    new Array2dAssignStatement(
                        ident.Value,
                        exprIndex!,
                        exprColIndex!,
                        exprValue2d!,
                        letLoc
                    )
                );
            }

            err = ExpectToken(TokenType.Eq, "after LET <identifier>[]");
            if (err is not null) return err;

            Advance(); //consume =

            var exprValue = ParseExpression();

            err = ExpectExpression(exprValue, "value after LET <identifier> =");
            if (err is not null) return err;

            return new ParseStatementSuccess(
                new ArrayAssignStatement(
                    ident.Value,
                    exprIndex!,
                    exprValue!,
                    letLoc
                )
            );
        }
        else
        {

            err = ExpectToken(TokenType.Eq, "after LET <identifier>");
            if (err is not null) return err;

            Advance(); //consume =

            var expr = ParseExpression();

            err = ExpectExpression(expr, "value after LET <identifier> =");
            if (err is not null) return err;

            return new ParseStatementSuccess(
                new LetStatement(
                    ident,
                    expr!,
                    letLoc
                )
            );
        }
    }

    private ParseStatementResult ParseInputStatement()
    {
        ParseStatementFailure? err;
        var loc = new SourceLocation(Current.Line, Current.Column);
        string? prompt = null;

        Advance(); //consume INPUT

        if (Current.Type == TokenType.StringLiteral)
        {
            var promptExpr = ParseExpression();

            err = ExpectExpression(promptExpr, "prompt after INPUT");
            if (err is not null) return err;

            if (promptExpr is not StringLiteralExpression sle)
            {
                return new ParseStatementFailure(
                   new Diagnostic(
                       Current.Line,
                       Current.Column,
                       $"Expected STRING after INPUT but got {Current.Type} at {Current.Line}:{Current.Column}",
                       DiagnosticSeverity.Error
                   )
               );
            }

            prompt = sle.Value;

            err = ExpectToken(TokenType.Semicolon, "after INPUT prompt");
            if (err is not null) return err;

            Advance(); //consume Semicolon
        }

        var expr = ParseExpression();
        err = ExpectExpression(expr, "<Identifier> after INPUT");
        if (err is not null) return err;

        if (expr is not IdentifierExpression ie)
        {
            return new ParseStatementFailure(
                  new Diagnostic(
                      Current.Line,
                      Current.Column,
                      $"Expected INDENTIFIER after INPUT but got {Current.Type} at {Current.Line}:{Current.Column}",
                      DiagnosticSeverity.Error
                  )
              );
        }

        return new ParseStatementSuccess(
            new InputStatement(
                prompt,
                ie,
                loc
            )
        );
    }
}
