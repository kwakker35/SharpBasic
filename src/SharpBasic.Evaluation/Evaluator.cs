using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public class Evaluator(Program _program)
{
    public EvalResult Evaluate()
    {
        var errors = new List<EvalError>();

        foreach(Statement stmt in _program.Statements)
        {
            var result = EvaluateStatement(stmt);

            if(result is EvalFailure failure)
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
            _ => new EvalFailure([new EvalError(
                                new InvalidOperationException(
                                $"Unknown statement type: {stmt.GetType().Name}"),
                                stmt.Location?.Line ?? 0,
                                stmt.Location?.Col ?? 0)])
        };
    }

    private EvalResult EvaluatePrintStatement(PrintStatement p)
    {
        var result = EvaluateExpression(p.Value);
        if (result is EvalFailure)
            return result;
        if (result is EvalSuccess es && es.Value is StringValue sv)
            Console.WriteLine(sv.V);
        return new EvalSuccess(new VoidValue());
    }


    private EvalResult EvaluateExpression(Expression expr)
    {
        return expr switch
        {
            StringLiteralExpression sl => new EvalSuccess(new StringValue(sl.Value)),
            _ => new EvalFailure([new EvalError(
                                new InvalidOperationException(
                                $"Unknown expression type: {expr.GetType().Name}"),
                                expr.Location?.Line ?? 0,
                                expr.Location?.Col ?? 0)])
        };
    }
}