namespace SharpBasic.Ast;

public record SubDeclaration(string Name,
                              IReadOnlyList<Parameter> Parameters,
                              IReadOnlyList<Statement> Body,
                              SourceLocation Location) :
              Statement(Location);