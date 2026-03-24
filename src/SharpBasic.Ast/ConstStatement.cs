namespace SharpBasic.Ast;

public record ConstStatement(Token Identifier, Expression Value, SourceLocation Location)
    : Statement(Location);
