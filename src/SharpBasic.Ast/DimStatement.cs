namespace SharpBasic.Ast;

public record DimStatement(string Name,
                            string TypeName,
                            Expression SizeExpr,
                            SourceLocation Location)
    : Statement(Location);