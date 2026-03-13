using System.Runtime.Versioning;
using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public class Evaluator(
    Program _program,
    SymbolTable? table = null,
    Dictionary<string, SubDeclaration>? subs = null,
    Dictionary<string, FunctionDeclaration>? functions = null)
{
    private SymbolTable _table = table ?? new();
    private Dictionary<string, SubDeclaration> _subs = subs ?? new();
    private Dictionary<string, FunctionDeclaration> _functions = functions ?? new();
    private List<EvalError> errors = [];

    public EvalResult Evaluate()
    {
        HoistDeclarations();

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

    public void HoistDeclarations()
    {
        foreach (Statement stmt in _program.Statements)
        {
            if (stmt is SubDeclaration s)
            {
                _subs.Add(s.Name, s);
            }
            else if (stmt is FunctionDeclaration f)
            {
                _functions.Add(f.Name, f);
            }
        }
    }

    private EvalResult EvaluateStatement(Statement stmt)
    {
        return stmt switch
        {
            PrintStatement p => EvaluatePrintStatement(p),
            LetStatement l => EvaluateLetStatement(l),
            IfStatement i => EvaluateIfStatement(i),
            WhileStatement w => EvaluateWhileStatement(w),
            ForStatement f => EvaluateForStatement(f),
            SubDeclaration => new EvalSuccess(new VoidValue()),
            FunctionDeclaration => new EvalSuccess(new VoidValue()),
            CallStatement cs => EvaluateCallStatement(cs),
            ReturnStatement rs => EvaluateReturnStatement(rs),
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

    private EvalResult EvaluateReturnStatement(ReturnStatement stmt)
    {
        if (stmt.Value is null)
            throw new ReturnException(null);

        var result = EvaluateExpression(stmt.Value);
        if (result is EvalFailure) return result;

        throw new ReturnException(((EvalSuccess)result).Value);
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

    private EvalResult EvaluateForStatement(ForStatement stmt)
    {
        List<EvalError> localErrors = [];
        var startEx = EvaluateExpression(stmt.Start);
        var limitEx = EvaluateExpression(stmt.Limit);
        var stepEx = stmt.Step is null ?
                        new EvalSuccess(new FloatValue(1)) :
                        EvaluateExpression(stmt.Step);


        if (startEx is EvalFailure)
        {
            return new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException("Invalid START expression or value"),
                            stmt.Start.Location?.Line ?? 0,
                            stmt.Start.Location?.Col ?? 0
                        )
                    ]
                );
        }
        else if (limitEx is EvalFailure)
        {
            return new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException("Invalid LIMIT expression or value"),
                            stmt.Limit.Location?.Line ?? 0,
                            stmt.Limit.Location?.Col ?? 0
                        )
                    ]
                );
        }
        else if (stepEx is EvalFailure)
        {
            return new EvalFailure(
                    [
                        new EvalError(
                            new InvalidOperationException("Invalid STEP expression or value"),
                            stmt.Step?.Location?.Line ?? 0,
                            stmt.Step?.Location?.Col ?? 0
                        )
                    ]
                );
        }


        if (startEx is EvalSuccess startS &&
            limitEx is EvalSuccess limitS &&
            stepEx is EvalSuccess stepS)
        {
            // evaluated once before the loop
            double i = ToFloat(startS.Value);
            double limit = ToFloat(limitS.Value);
            double step = ToFloat(stepS.Value);

            _table.Set(stmt.LoopVar.Value, new IntValue((int)i));

            while (true)
            {
                // condition depends on step direction
                bool shouldRun = step > 0 ? i <= limit : i >= limit;
                if (!shouldRun) break;

                _table.Set(stmt.LoopVar.Value, new IntValue((int)i));

                foreach (Statement bodyStmt in stmt.Body)
                {
                    var bodyResult = EvaluateStatement(bodyStmt);

                    if (bodyResult is EvalFailure failure)
                    {
                        localErrors.AddRange(failure.Errors);
                    }
                }

                // increment
                i += step;
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

    private EvalResult EvaluateCallStatement(CallStatement stmt)
    {
        var subExists = _subs.TryGetValue(stmt.Name, out var sub);

        if (!subExists)
        {
            return new EvalFailure(
                        [
                            new EvalError(
                            new InvalidOperationException($"Sub: {stmt.Name} not found."),
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0
                        )
                        ]
                    );
        }

        var localSymbols = new SymbolTable(_table);

        for (int i = 0; i < sub!.Parameters.Count; i++)
        {
            var argResult = EvaluateExpression(stmt.Arguments[i]);  // evaluate in caller's scope
            if (argResult is EvalFailure) return argResult;
            localSymbols.Set(sub.Parameters[i].Name, ((EvalSuccess)argResult).Value!);
        }

        try
        {
            var funcEval = new Evaluator(
                                new Program(sub.Body),
                                localSymbols,
                                _subs,
                                _functions
                            ).Evaluate();
        }
        catch (ReturnException re)
        {
            //sucess hit return statement and swallow the return value
            return new EvalSuccess(new VoidValue());
        }
        catch (Exception e)
        {
            //unkown exception
            return new EvalFailure(
                        [
                            new EvalError(
                            e,
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0
                        )
                        ]
                    );
        }

        //no return statment - no problem!
        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateCallExpression(CallExpression expr)
    {
        var funcExists = _functions.TryGetValue(expr.Name, out var func);

        if (!funcExists)
        {
            return new EvalFailure(
                        [
                            new EvalError(
                            new InvalidOperationException($"Function: {expr.Name} not found."),
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                        ]
                    );
        }

        var localSymbols = new SymbolTable(_table);

        for (int i = 0; i < func!.Parameters.Count; i++)
        {
            var argResult = EvaluateExpression(expr.Arguments[i]);  // evaluate in caller's scope
            if (argResult is EvalFailure) return argResult;
            localSymbols.Set(func.Parameters[i].Name, ((EvalSuccess)argResult).Value!);
        }

        try
        {
            var funcEval = new Evaluator(
                                new Program(func.Body),
                                localSymbols,
                                _subs,
                                _functions
                            ).Evaluate();
        }
        catch (ReturnException re)
        {
            //sucess hit return statement 
            return new EvalSuccess(re.ReturnValue);
        }
        catch (Exception e)
        {
            //unkown exception
            return new EvalFailure(
                        [
                            new EvalError(
                            e,
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                        ]
                    );
        }

        //no return statement hit
        return new EvalFailure(
                        [
                            new EvalError(
                            new InvalidOperationException("Missing RETURN statement in FUNCTION."),
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0
                        )
                        ]
                    );

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

        // string concatenation
        if (expr.Operator.Type is TokenType.Ampersand)
        {
            return new EvalSuccess(new StringValue(
                (leftES.Value?.ToString() ?? "") + (rightES.Value?.ToString() ?? "")
            ));
        }

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

                return new EvalSuccess(new BoolValue(result));
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
            CallExpression ce => EvaluateCallExpression(ce),
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
