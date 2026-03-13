namespace SharpBasic.Ast;

public record ArrayAccessExpression(string Name,
                                    Expression Index,
                                    SourceLocation Location)
    : Expression(Location);