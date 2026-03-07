namespace SharpBasic.Evaluation;

public record IntValue(int V) : Value
{
  public override string ToString() => V.ToString();
}