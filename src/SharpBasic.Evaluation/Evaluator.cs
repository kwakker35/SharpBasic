using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public class Evaluator(Program _program)
{
    private SymbolTable _table = new();

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

    private EvalResult EvaluateLetStatement(LetStatement l)
    {
        var identRes = EvaluateToken(l.Identifier);
        var valueRes = EvaluateExpression(l.Value);

        if (identRes is EvalFailure)
            return identRes;
        if (valueRes is EvalFailure)
            return valueRes;

        if (identRes is EvalSuccess id
            && id.Value is StringValue svi
            && valueRes is EvalSuccess es)
        {
            _table.Set(svi.V, es.Value);
        }

        return new EvalSuccess(new VoidValue());

    }


    private EvalResult EvaluateExpression(Expression expr)
    {
        return expr switch
        {
            StringLiteralExpression sl => new EvalSuccess(new StringValue(sl.Value)),
            IdentifierExpression id => new EvalSuccess(_table.Get(id.Name)),
            _ => new EvalFailure([new EvalError(
                                new InvalidOperationException(
                                $"Unknown expression type: {expr.GetType().Name}"),
                                expr.Location?.Line ?? 0,
                                expr.Location?.Col ?? 0)])
        };
    }

    private EvalResult EvaluateToken(Token token)
    {
        return token.Type switch
        {
            TokenType.Identifier => new EvalSuccess(new StringValue(token.Value)),
            _ => new EvalFailure([new EvalError(
                                new InvalidOperationException(
                                $"Unknown token type: {token.GetType().Name}"),
                                token.Line,
                                token.Column)])
        };
    }
}