namespace SharpBasic.Evaluation;

public record FloatValue(double V) : Value
{
  public override string ToString() =>
    V.ToString(System.Globalization.CultureInfo.InvariantCulture);
  public override string TypeName => "Float";
}