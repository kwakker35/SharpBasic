namespace SharpBasic.Parsing;

public record ParseStatementError(Exception Exception, int Line, int Col);