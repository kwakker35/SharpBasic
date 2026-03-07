namespace SharpBasic.Ast;

public record FloatLiteralExpression(double Value, SourceLocation Location)
    : Expression(Location);