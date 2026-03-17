using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public record ParseStatementFailure(Diagnostic Diagnostic) : ParseStatementResult;