namespace SharpBasic.Evaluation;

public record ArrayValue(Value[] Items, string ElementTypeName) : Value
{
  public override string TypeName => $"Array<{ElementTypeName}>";
  public override string ToString() => $"Array[{Items.Length}]";
}