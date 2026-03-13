namespace SharpBasic.Ast;

public record ArrayAssignStatement(string Name,
                                    Expression Index,
                                    Expression Value,
                                    SourceLocation Location)
    : Statement(Location);