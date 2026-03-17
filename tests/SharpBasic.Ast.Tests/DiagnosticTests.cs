using SharpBasic.Ast;
using Xunit;

namespace SharpBasic.Ast.Tests;

public class DiagnosticTests
{
  [Fact]
  public void Diagnostic_Stores_Line_Col_And_Message()
  {
    var d = new Diagnostic(3, 7, "Undefined variable 'nme'.", DiagnosticSeverity.Error);

    Assert.Equal(3, d.Line);
    Assert.Equal(7, d.Col);
    Assert.Equal("Undefined variable 'nme'.", d.Message);
    Assert.Equal(DiagnosticSeverity.Error, d.Severity);
  }

  [Fact]
  public void Diagnostic_ToString_Formats_Correctly()
  {
    var d = new Diagnostic(3, 7, "Undefined variable 'nme'.", DiagnosticSeverity.Error);

    Assert.Equal("[Line 3, Col 7] Error: Undefined variable 'nme'.", d.ToString());
  }

  [Fact]
  public void Diagnostic_Warning_Formats_Correctly()
  {
    var d = new Diagnostic(1, 1, "Unused variable 'x'.", DiagnosticSeverity.Warning);

    Assert.Equal("[Line 1, Col 1] Warning: Unused variable 'x'.", d.ToString());
  }

  [Fact]
  public void DiagnosticSeverity_Has_Error_And_Warning()
  {
    Assert.Equal(0, (int)DiagnosticSeverity.Error);
    Assert.Equal(1, (int)DiagnosticSeverity.Warning);
  }
}
