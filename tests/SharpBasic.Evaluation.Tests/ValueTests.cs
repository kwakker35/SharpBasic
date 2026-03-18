using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class ValueTests
{
  // --- TypeName property ---
  // Each Value subtype must expose a TypeName string for readable diagnostics.

  [Fact]
  public void StringValue_TypeName_Is_String()
  {
    Assert.Equal("String", new StringValue("hello").TypeName);
  }

  [Fact]
  public void IntValue_TypeName_Is_Integer()
  {
    Assert.Equal("Integer", new IntValue(42).TypeName);
  }

  [Fact]
  public void FloatValue_TypeName_Is_Float()
  {
    Assert.Equal("Float", new FloatValue(3.14).TypeName);
  }

  [Fact]
  public void BoolValue_TypeName_Is_Boolean()
  {
    Assert.Equal("Boolean", new BoolValue(true).TypeName);
  }

  [Fact]
  public void VoidValue_TypeName_Is_Void()
  {
    Assert.Equal("Void", new VoidValue().TypeName);
  }
}
