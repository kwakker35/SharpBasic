namespace SharpBasic.Ast;

public readonly record struct Token(TokenType Type, string Value, int Line, int Column)
{
  public bool Equals(Token other) => Type == other.Type && Value == other.Value;
  public override int GetHashCode() => HashCode.Combine(Type, Value);
}