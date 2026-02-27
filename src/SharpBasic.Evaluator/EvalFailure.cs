namespace SharpBasic.Evaluator;

public record EvalFailure(IReadOnlyList<EvalError> Errors): EvalResult;