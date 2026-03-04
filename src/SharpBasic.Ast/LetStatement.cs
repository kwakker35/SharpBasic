namespace SharpBasic.Ast;

public record LetStatement(Token Identifier, Expression Value, SourceLocation Location)
    : Statement(Location);