namespace SharpBasic.Evaluator;

public record EvalSuccess(Value? Value): EvalResult; // null = void/no value