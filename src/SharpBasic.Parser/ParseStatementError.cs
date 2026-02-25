namespace SharpBasic.Parser;

public record ParseStatementError(Exception Exception, int Line, int Col);