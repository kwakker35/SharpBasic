namespace SharpBasic.Ast;

public record FloatLiteralExpression(float Value, SourceLocation Location)
    : Expression(Location);