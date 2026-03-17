namespace SharpBasic.Ast;

public record BoolLiteralExpression(bool Value, SourceLocation Location)
    : Expression(Location);