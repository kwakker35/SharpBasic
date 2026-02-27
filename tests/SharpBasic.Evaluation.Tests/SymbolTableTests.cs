using Xunit;

namespace SharpBasic.Evaluation.Tests;

public class SymbolTableTests()
{
    [Fact]
    public void Get_Returns_The_Correct_Value()
    {
        var table = new SymbolTable();
        var testValue = new StringValue("Chris");

        table.Set("Name", testValue);

        var output = table.Get("Name");

        Assert.NotNull(output);
        var slv = Assert.IsType<StringValue>(output);
        Assert.Equal(testValue,slv);
    }

    [Fact]
    public void Get_Returns_The_Latest_Value()
    {
        var table = new SymbolTable();
        var testValue = new StringValue("Chris");
        var testValue2 = new IntValue(69);

        table.Set("Name", testValue);
        table.Set("Name", testValue2);

        var output = table.Get("Name");

        Assert.NotNull(output);
        var iv = Assert.IsType<IntValue>(output);
        Assert.Equal(testValue2,iv);
    }

    [Fact]
    public void Get_Returns_Null_For_Undefined_Name()
    {
        var table = new SymbolTable();

        var output = table.Get("Name");

        Assert.Null(output);
    }
}