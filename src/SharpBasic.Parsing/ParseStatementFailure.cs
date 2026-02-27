namespace SharpBasic.Parsing;

public record ParseStatementFailure(ParseStatementError Error): ParseStatementResult;