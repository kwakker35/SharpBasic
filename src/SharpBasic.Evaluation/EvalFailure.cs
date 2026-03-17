using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public record EvalFailure(IReadOnlyList<Diagnostic> Diagnostics) : EvalResult;