namespace SharpBasic.Ast;

public record SleepStatement(Expression Milliseconds, SourceLocation Location)
    : Statement(Location);
