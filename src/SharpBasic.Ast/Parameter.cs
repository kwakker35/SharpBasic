namespace SharpBasic.Ast;

public record Parameter(string Name, string TypeName, SourceLocation Location) :
  AstNode(Location);