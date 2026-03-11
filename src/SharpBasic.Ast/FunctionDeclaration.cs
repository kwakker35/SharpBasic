namespace SharpBasic.Ast;

public record FunctionDeclaration(string Name,
                              IReadOnlyList<Parameter> Parameters,
                              IReadOnlyList<Statement> Body,
                              string ReturnType,
                              SourceLocation Location) :
              Statement(Location);