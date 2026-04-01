namespace SharpBasic.Ast;

public record Dim2dStatement(
    string Name,
    string TypeName,
    Expression RowsExpr,
    Expression ColsExpr,
    SourceLocation Location)
    : Statement(Location);
