namespace SharpBasic.Ast;

public record CaseClause(
    IReadOnlyList<Expression> Values,
    IReadOnlyList<Statement> Body,
    SourceLocation Location)
    : AstNode(Location);