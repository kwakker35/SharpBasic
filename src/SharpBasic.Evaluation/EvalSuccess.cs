using SharpBasic.Evaluation;

public record EvalSuccess(Value? Value): EvalResult; // null = void/no value