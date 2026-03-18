namespace SharpBasic.Evaluation;

public record VoidValue : Value
{
  public override string ToString() => string.Empty;
  public override string TypeName => "Void";
}