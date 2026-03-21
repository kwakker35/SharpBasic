using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class InputTests
{
  [Fact]
  public void Input_Bare_Assigns_User_Input_To_Variable()
  {
    // INPUT name$  — user types "Alice"
    var source = "INPUT name$\nPRINT name$";
    var output = RunHelper.RunWithInput(source, "Alice");
    Assert.Equal("Alice", output);
  }

  [Fact]
  public void Input_With_Prompt_Prints_Prompt_With_Question_Mark()
  {
    // INPUT "Enter name"; name$  — prompt should appear as "Enter name? "
    var source = "INPUT \"Enter name\"; name$\nPRINT name$";
    var output = RunHelper.RunWithInput(source, "Bob");
    Assert.Equal("Enter name? Bob", output);
  }

  [Fact]
  public void Input_Assigns_Numeric_String_To_Variable()
  {
    var source = "INPUT age$\nPRINT age$";
    var output = RunHelper.RunWithInput(source, "42");
    Assert.Equal("42", output);
  }

  [Fact]
  public void Input_Can_Be_Used_In_Expression_After_Val()
  {
    // INPUT num$; use VAL to convert and do arithmetic
    var source = "INPUT num$\nLET result = VAL(num$) + 10\nPRINT result";
    var output = RunHelper.RunWithInput(source, "5");
    Assert.Equal("15", output);
  }
}
