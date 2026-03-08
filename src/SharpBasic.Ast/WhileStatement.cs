namespace SharpBasic.Ast;

public record WhileStatement(Expression Condition,
                            List<Statement> Body,
                            SourceLocation Location)
    : Statement(Location);