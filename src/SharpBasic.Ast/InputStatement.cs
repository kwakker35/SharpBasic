namespace SharpBasic.Ast;

public record InputStatement(string? Prompt,
                            IdentifierExpression Target,
                            SourceLocation Location)
    : Statement(Location);