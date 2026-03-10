using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public class Evaluator(Program _program, SymbolTable? table = null)
{
    private SymbolTable _table = table ?? new();
    private List<EvalError> errors = [];

    public EvalResult Evaluate()
    {
        foreach (Statement stmt in _program.Statements)
        {
            var result = EvaluateStatement(stmt);

            if (result is EvalFailure failure)
            {
                errors.AddRange(failure.Errors);
            }
        }

        return errors.Count > 0 ? new EvalFailure(errors) : new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateStatement(Statement stmt)
    {
        return stmt switch
        {
            PrintStatement p => EvaluatePrintStatement(p),
            LetStatement l => EvaluateLetStatement(l),
            IfStatement i => EvaluateIfStatement(i),
            WhileStatement w => EvaluateWhileStatement(w),
            _
                => new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException(
                                $"Unknown statement type: {stmt.GetType().Name}"
                            ),
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0
                        )
                    ]
                )
        };
    }

    private EvalResult EvaluatePrintStatement(PrintStatement p)
    {
        var result = EvaluateExpression(p.Value);
        if (result is EvalFailure)
            return result;

        if (result is EvalSuccess es)
            Console.WriteLine(es.Value?.ToString() ?? string.Empty);

        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateLetStatement(LetStatement l)
    {
        var ident = l.Identifier.Value;
        var valueRes = EvaluateExpression(l.Value);

        if (valueRes is EvalFailure)
            return valueRes;

        if (valueRes is EvalSuccess es && es.Value is not null)
        {
            _table.Set(ident, es.Value);
            return new EvalSuccess(new VoidValue());
        }

        return new EvalFailure(
            [
                new EvalError(
                    new ArgumentNullException($"{l.Identifier.Value}"),
                    l.Location?.Line ?? 0,
                    l.Location?.Col ?? 0
                )
            ]
        );
    }

    private EvalResult EvaluateIfStatement(IfStatement stmt)
    {
        var result = EvaluateExpression(stmt.Condition);

        if (result is EvalFailure) return result;

        bool value = false;
        List<EvalError> localErrors = [];

        if (result is EvalSuccess es && es.Value is BoolValue bv)
        {
            value = bv.V;
        }
        else
        {
            return new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException("IF condition must evaluate to a boolean value."),
                            stmt.Condition.Location?.Line ?? 0,
                            stmt.Condition.Location?.Col ?? 0
                        )
                    ]
                );
        }


        if (value)
        {
            foreach (Statement thenStmt in stmt.ThenBlock)
            {
                var thenResult = EvaluateStatement(thenStmt);

                if (thenResult is EvalFailure failure)
                {
                    localErrors.AddRange(failure.Errors);
                }
            }
        }
        else if (stmt.ElseBlock is not null)
        {
            foreach (Statement elseStmt in stmt.ElseBlock)
            {
                var elseResult = EvaluateStatement(elseStmt);

                if (elseResult is EvalFailure failure)
                {
                    localErrors.AddRange(failure.Errors);
                }
            }
        }

        return localErrors.Count > 0 ?
                new EvalFailure(localErrors) :
                new EvalSuccess(new VoidValue());

    }

    private EvalResult EvaluateWhileStatement(WhileStatement stmt)
    {
        List<EvalError> localErrors = [];

        while (true)
        {
            bool loop = false;
            var result = EvaluateExpression(stmt.Condition);

            if (result is EvalFailure) return result;

            if (result is EvalSuccess es && es.Value is BoolValue bv)
            {
                loop = bv.V;
            }
            else
            {
                return new EvalFailure(
                        [
                            new EvalError(
                            new InvalidOperationException("WHILE condition must evaluate to a boolean value."),
                            stmt.Condition.Location?.Line ?? 0,
                            stmt.Condition.Location?.Col ?? 0
                        )
                        ]
                    );
            }

            if (!loop) break;

            foreach (Statement bodyStmt in stmt.Body)
            {
                var bodyResult = EvaluateStatement(bodyStmt);

                if (bodyResult is EvalFailure failure)
                {
                    localErrors.AddRange(failure.Errors);
                }
            }
        }

        return localErrors.Count > 0 ?
                new EvalFailure(localErrors) :
                new EvalSuccess(new VoidValue());
    }

    private static double ToFloat(Value v) => v switch
    {
        IntValue iv => iv.V,
        FloatValue fv => fv.V,
        _ => throw new InvalidOperationException($"Cannot convert {v.GetType().Name} to float")
    };

    private EvalResult EvaluateBinaryExpression(BinaryExpression expr)
    {
        if (expr.Operator.Type is not (TokenType.Plus or
                                        TokenType.Minus or
                                        TokenType.Multiply or
                                        TokenType.Divide or
                                        TokenType.Eq or
                                        TokenType.NotEq or
                                        TokenType.Lt or
                                        TokenType.Gt or
                                        TokenType.LtEq or
                                        TokenType.GtEq or
                                        TokenType.Ampersand))
        {
            return new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException(
                                $"Unknown statement type: {expr.Operator.GetType().Name}"
                            ),
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                    ]
                );
        }

        var leftRes = EvaluateExpression(expr.Left);
        if (leftRes is EvalFailure) return leftRes;
        var leftES = (EvalSuccess)leftRes;
        var rightRes = EvaluateExpression(expr.Right);
        if (rightRes is EvalFailure) return rightRes;
        var rightES = (EvalSuccess)rightRes;

        if (leftES.Value is StringValue lsv && rightES.Value is StringValue rsv)
        {
            //string comparison
            if (expr.Operator.Type is TokenType.Eq or
                                        TokenType.NotEq)
            {
                bool result = expr.Operator.Type switch
                {
                    TokenType.Eq => lsv == rsv,
                    TokenType.NotEq => lsv != rsv,
                    _ => throw new InvalidOperationException("Unreachable")
                };

                return new EvalSuccess(new BoolValue(true));
            } // string concatenation
            else if (expr.Operator.Type is TokenType.Ampersand)
            {
                string result = string.Concat(lsv, rsv);
                return new EvalSuccess(new StringValue(result));
            }
            else
            {
                return new EvalFailure(
                    [
                        new EvalError(
                        new InvalidOperationException($"Unknown Expression: {expr.GetType()}"),
                        expr.Location?.Line ?? 0,
                        expr.Location?.Col ?? 0
                    )
                    ]
                );
            }
        }

        var isFloat = leftES.Value is FloatValue || rightES.Value is FloatValue;

        double left = ToFloat(leftES.Value);
        double right = ToFloat(rightES.Value);

        if (expr.Operator.Type is TokenType.Divide
                && right == 0)
        {
            return new EvalFailure(
                [
                    new EvalError(
                        new DivideByZeroException(),
                        expr.Location?.Line ?? 0,
                        expr.Location?.Col ?? 0
                    )
                ]
            );
        }

        //Arithmetic
        if (expr.Operator.Type is TokenType.Plus or
                                        TokenType.Minus or
                                        TokenType.Multiply or
                                        TokenType.Divide)
        {
            double result = expr.Operator.Type switch
            {
                TokenType.Plus => left + right,
                TokenType.Minus => left - right,
                TokenType.Multiply => left * right,
                TokenType.Divide => left / right,
                _ => throw new InvalidOperationException("Unreachable")
            };

            Value output = isFloat ? new FloatValue(result) :
                    double.IsInteger(result) ?
                        new IntValue((int)result) :
                        new FloatValue(result);

            return new EvalSuccess(output);
        } //Boolean
        else if (expr.Operator.Type is TokenType.Eq or
                                        TokenType.NotEq or
                                        TokenType.Lt or
                                        TokenType.Gt or
                                        TokenType.LtEq or
                                        TokenType.GtEq)
        {
            bool result = expr.Operator.Type switch
            {
                TokenType.Eq => left == right,
                TokenType.NotEq => left != right,
                TokenType.Lt => left < right,
                TokenType.Gt => left > right,
                TokenType.LtEq => left <= right,
                TokenType.GtEq => left >= right,
                _ => throw new InvalidOperationException("Unreachable")
            };

            return new EvalSuccess(new BoolValue(result));
        }

        return new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException($"Unknown Expression: {expr.GetType()}"),
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                    ]
                );
    }

    private EvalResult EvaluateExpression(Expression expr)
    {
        return expr switch
        {
            StringLiteralExpression sl => new EvalSuccess(new StringValue(sl.Value)),
            IntLiteralExpression il => new EvalSuccess(new IntValue(il.Value)),
            FloatLiteralExpression fl => new EvalSuccess(new FloatValue(fl.Value)),
            BinaryExpression be => EvaluateBinaryExpression(be),
            IdentifierExpression id when _table.Get(id.Name) is { } val => new EvalSuccess(val),
            IdentifierExpression id
                => new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException($"Unknown Identifier: {id.Name}"),
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                    ]
                ),
            _
                => new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException($"Unknown Expression: {expr.GetType()}"),
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                    ]
                )
        };
    }
}
