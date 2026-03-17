namespace SharpBasic.Ast;

public enum DiagnosticSeverity { Error = 0, Warning = 1 }

public record Diagnostic(int Line, int Col, string Message, DiagnosticSeverity Severity)
{
  public override string ToString()
  {
    return $"[Line {Line}, Col {Col}] {Severity}: {Message}";
  }
};