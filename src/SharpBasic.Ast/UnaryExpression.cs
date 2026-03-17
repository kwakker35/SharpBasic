namespace SharpBasic.Ast;

public record UnaryExpression(Token Operator, Expression Operand, SourceLocation Location)
    : Expression(Location);