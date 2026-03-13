namespace SharpBasic.Evaluation;

public class ReturnException(Value? value) : Exception()
{
  public Value? ReturnValue { get; } = value;
}