namespace SharpBasic.Ast;

public record BinaryExpression(Expression Left,
                                Token Operator,
                                Expression Right,
                                SourceLocation Location)
    : Expression(Location);