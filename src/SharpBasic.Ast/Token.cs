namespace SharpBasic.Ast;

public readonly record struct Token(TokenType Type, string Value, int Line, int Column);