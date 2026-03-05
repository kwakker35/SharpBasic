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
        if (
            p.Value is StringLiteralExpression
            && result is EvalSuccess es
            && es.Value is StringValue sv
        )
            Console.WriteLine(sv.V);
        if (
            p.Value is IdentifierExpression
            && result is EvalSuccess esx
            && esx.Value is StringValue svx
        )
            Console.WriteLine(svx.V);

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

    private EvalResult EvaluateExpression(Expression expr)
    {
        return expr switch
        {
            StringLiteralExpression sl => new EvalSuccess(new StringValue(sl.Value)),
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
