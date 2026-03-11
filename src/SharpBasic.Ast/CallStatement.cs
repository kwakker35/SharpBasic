namespace SharpBasic.Ast;

public record CallStatement(string Name,
                            IReadOnlyList<Expression> Arguments,
                            SourceLocation Location) :
              Statement(Location);