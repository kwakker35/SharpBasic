namespace SharpBasic.Ast;

public record DimStatement(string Name,
                            string TypeName,
                            int Size,
                            SourceLocation Location)
    : Statement(Location);