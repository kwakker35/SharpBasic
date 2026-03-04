namespace SharpBasic.Ast;

public record IdentifierExpression(string Name, SourceLocation Location)
    : Expression(Location);