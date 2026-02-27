using SharpBasic.Evaluation;

public record EvalFailure(IReadOnlyList<EvalError> Errors): EvalResult;