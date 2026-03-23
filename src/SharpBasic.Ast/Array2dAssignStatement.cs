namespace SharpBasic.Ast;

public record Array2dAssignStatement(
    string Name,
    Expression RowIndex,
    Expression ColIndex,
    Expression Value,
    SourceLocation Location)
    : Statement(Location);
