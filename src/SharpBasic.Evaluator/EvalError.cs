using SharpBasic.Evaluator;

public record EvalError(Exception Exception, int Line, int Col);