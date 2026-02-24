namespace SharpBasic.Ast;

public record Program(IReadOnlyList<Statement> Statements): AstNode;