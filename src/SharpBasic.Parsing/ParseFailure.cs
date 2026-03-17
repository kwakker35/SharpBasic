using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public record ParseFailure(IReadOnlyList<Diagnostic> Diagnostics) : ParseResult;