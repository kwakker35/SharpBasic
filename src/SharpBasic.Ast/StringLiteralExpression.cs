namespace SharpBasic.Ast;

public record StringLiteralExpression(string Value, SourceLocation Location)
    : Expression(Location);