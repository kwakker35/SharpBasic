namespace SharpBasic.Evaluation;

public record ArrayValue(Value[] Items, string TypeName) : Value;