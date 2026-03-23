namespace SharpBasic.Ast;

public record Array2dAccessExpression(
    string Name,
    Expression RowIndex,
    Expression ColIndex,
    SourceLocation Location)
    : Expression(Location);
