namespace SharpBasic.Evaluation;

public record FloatValue(double V) : Value
{
  public override string ToString() => V.ToString();
}