namespace SharpBasic.Ast;

public record PrintStatement(Expression Value, SourceLocation Location)
    : Statement(Location);