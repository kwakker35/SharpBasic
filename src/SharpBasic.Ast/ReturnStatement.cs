namespace SharpBasic.Ast;

public record ReturnStatement(Expression? Value, SourceLocation Location) :
              Statement(Location);