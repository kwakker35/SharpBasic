using SharpBasic.Ast;
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
        Assert.Equal(testValue, slv);
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
        Assert.Equal(testValue2, iv);
    }

    [Fact]
    public void Get_Returns_Null_For_Undefined_Name()
    {
        var table = new SymbolTable();

        var output = table.Get("Name");

        Assert.Null(output);
    }

    [Fact]
    public void Variable_In_Outer_Scope_Readable_From_Inner_Scope()
    {
        var parent = new SymbolTable();
        var child = new SymbolTable(parent);

        parent.Set("x", new StringValue("Hello, World!"));

        var value = child.Get("x");

        Assert.NotNull(value);
        var retVal = Assert.IsType<StringValue>(value);
        Assert.Equal("Hello, World!", retVal.V);
    }

    [Fact]
    public void Variable_In_Inner_Scope_Not_Visible_From_Outer_Scope()
    {
        var parent = new SymbolTable();
        var child = new SymbolTable(parent);

        child.Set("x", new StringValue("Hello, World!"));

        var value = parent.Get("x");

        Assert.Null(value);
    }

    [Fact]
    public void Variable_In_Inner_Scope_Shadows_Varible_From_Outer_Scope()
    {
        var parent = new SymbolTable();
        var child = new SymbolTable(parent);
        parent.Set("x", new StringValue("Hello, World!"));
        child.Set("x", new StringValue("Goodbye"));

        var pv = parent.Get("x");
        var cv = child.Get("x");

        Assert.NotNull(pv);
        var retValP = Assert.IsType<StringValue>(pv);
        Assert.Equal("Hello, World!", retValP.V);

        Assert.NotNull(cv);
        var retValC = Assert.IsType<StringValue>(cv);
        Assert.Equal("Goodbye", retValC.V);

    }
}