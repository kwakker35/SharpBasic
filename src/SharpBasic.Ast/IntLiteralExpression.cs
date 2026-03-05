namespace SharpBasic.Ast;

public record IntLiteralExpression(int Value, SourceLocation Location)
    : Expression(Location);