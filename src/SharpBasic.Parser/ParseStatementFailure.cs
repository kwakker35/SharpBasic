namespace SharpBasic.Parser;

public record ParseStatementFailure(ParseStatementError Error): ParseStatementResult;