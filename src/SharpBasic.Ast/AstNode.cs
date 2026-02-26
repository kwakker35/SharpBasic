namespace SharpBasic.Ast;

public abstract record AstNode(SourceLocation? Location = null)
{
    public virtual bool Equals(AstNode? other) =>
        other is not null && other.GetType() == GetType();

    public override int GetHashCode() => GetType().GetHashCode();
}