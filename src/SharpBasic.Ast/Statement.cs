namespace SharpBasic.Ast;

public abstract record Statement(SourceLocation Location): AstNode(Location);