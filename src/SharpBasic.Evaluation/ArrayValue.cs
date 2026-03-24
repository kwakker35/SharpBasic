namespace SharpBasic.Evaluation;

public record ArrayValue(Value[] Items, string ElementTypeName, int Cols = 0) : Value
{
  public override string TypeName => $"Array<{ElementTypeName}>";
  public override string ToString() => Cols > 0
      ? $"Array[{Items.Length / Cols}][{Cols}]"
      : $"Array[{Items.Length}]";
}