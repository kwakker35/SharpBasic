namespace SharpBasic.Evaluation;

public record BoolValue(bool V) : Value
{
  public override string ToString() => V.ToString();
}
