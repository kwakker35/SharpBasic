namespace SharpBasic.Ast;

public record Dim2dStatement(
    string Name,
    string TypeName,
    int Rows,
    int Cols,
    SourceLocation Location)
    : Statement(Location);
