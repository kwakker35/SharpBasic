namespace SharpBasic.Ast;

public record SetGlobalStatement(string Identifier, Expression Value, SourceLocation Location)
    : Statement(Location);
