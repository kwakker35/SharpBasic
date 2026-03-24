namespace SharpBasic.Ast;

public record SelectCaseStatement(
    Expression Subject,
    IReadOnlyList<CaseClause> Cases,
    CaseClause? Else,
    SourceLocation Location)
    : Statement(Location);