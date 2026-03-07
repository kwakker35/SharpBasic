using System.Collections;
using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public class Evaluator(Program _program, SymbolTable? table = null)
{
    private SymbolTable _table = table ?? new();

    public EvalResult Evaluate()
    {
        var errors = new List<EvalError>();

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

    private EvalResult EvaluateBinaryExpression(BinaryExpression expr)
    {
        var leftRes = EvaluateExpression(expr.Left);
        if (leftRes is EvalFailure) return leftRes;
        var left = ((EvalSuccess)leftRes).Value as IntValue;

        var rightRes = EvaluateExpression(expr.Right);
        if (rightRes is EvalFailure) return rightRes;
        var right = ((EvalSuccess)rightRes).Value as IntValue;

        if (expr.Operator.Type is TokenType.Divide
                && right.V == 0)
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

        int? result = expr.Operator.Type switch
        {
            TokenType.Plus => left.V + right.V,
            TokenType.Minus => left.V - right.V,
            TokenType.Multiply => left.V * right.V,
            TokenType.Divide => left.V / right.V,
            _ => null
        };

        if (result is null)
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

        return new EvalSuccess(new IntValue(result.Value));
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
