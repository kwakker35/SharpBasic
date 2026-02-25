using SharpBasic.Ast;

namespace SharpBasic.Evaluator;

public class Evaluator(Program _program)
{
    public EvalResult Evaluate()
    {
        foreach(Statement stmt in _program.Statements)
        {
            var result = EvaluateStatement(stmt);
        }

        return new EvalSuccess(new NumberValue(12));
    }

    private EvalResult EvaluateStatement(Statement stmt)
    {
        var expr = new StringLiteralExpression("hello");
        return new EvalSuccess(EvaluateExpression(expr));
    }

    private Value EvaluateExpression(Expression expr)
    {
        return new StringValue(expr.ToString());
    }
}