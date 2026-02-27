using SharpBasic.Evaluation;

namespace SharpBasic.Evaluation;

public record EvalError(Exception Exception, int Line, int Col);