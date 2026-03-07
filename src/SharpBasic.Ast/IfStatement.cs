namespace SharpBasic.Ast;

public record IfStatement(Expression Condition,
                            List<Statement> ThenBlock,
                            List<Statement>? ElseBlock,
                            SourceLocation Location)
    : Statement(Location);