using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public record ParseStatementSuccess(Statement Statement): ParseStatementResult;