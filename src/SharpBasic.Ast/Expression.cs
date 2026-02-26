namespace SharpBasic.Ast;

public abstract record Expression(SourceLocation Location): AstNode(Location);