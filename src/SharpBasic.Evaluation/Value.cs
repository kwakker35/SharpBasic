namespace SharpBasic.Evaluation;

public abstract record Value
{
  public abstract override string ToString();
  public abstract string TypeName { get; }
};
