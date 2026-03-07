namespace SharpBasic.Evaluation;

public record VoidValue : Value
{
  public override string ToString() => string.Empty;
}