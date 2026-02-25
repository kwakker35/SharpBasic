using SharpBasic.Ast;

namespace SharpBasic.Parser;

public record ParseStatementSuccess(Statement Statement): ParseStatementResult;