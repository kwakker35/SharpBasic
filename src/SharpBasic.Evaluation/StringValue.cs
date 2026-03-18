namespace SharpBasic.Evaluation;

public record StringValue(string V) : Value
{
  public override string ToString() => V.ToString();
  public override string TypeName => "String";
}