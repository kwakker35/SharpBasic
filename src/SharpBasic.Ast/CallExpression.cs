namespace SharpBasic.Ast;

public record CallExpression(string Name,
                            IReadOnlyList<Expression> Arguments,
                            SourceLocation Location) :
              Expression(Location);