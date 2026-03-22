# Phase 10 — Standard Library, INPUT, and File Runner

## Goal

A usable, complete language. `INPUT` reads from the console. Built-in functions cover the common string and numeric operations. Programs can be run from a file with `sharpbasic run hello.sbx`. SharpBASIC is done at the end of this phase.

## Honest difficulty

Easy. This phase is mostly additive — new built-in functions registered in the evaluator, an `INPUT` statement, and a file runner entry point. Nothing architectural. A satisfying finish.

## What you'll build

- Built-in string functions: `LEN`, `MID$`, `LEFT$`, `RIGHT$`, `TRIM$`, `UPPER$`, `LOWER$`
- Built-in numeric functions: `INT`, `ABS`, `SQR`, `RND`
- Built-in conversion functions: `STR$`, `VAL`
- Built-in diagnostic function: `TYPENAME`
- `INPUT` statement — with and without a prompt string
- File runner: `sharpbasic run program.sbx`
- REPL improvements: `.exit` command, multi-line input handling

## Key concepts

**Built-in functions are resolved before user-defined functions.** The evaluator checks the built-in registry first. Built-in names cannot be shadowed by user `FUNCTION` declarations — attempting to do so is a runtime error. See `docs/language-spec-v1.md` §10.

**`INPUT` always stores a `String`.** Regardless of what the user types, `INPUT` stores the result as `StringValue`. Use `VAL()` to convert to a number. See `docs/language-spec-v1.md` §11.

**`MID$` is 1-based.** `MID$("hello", 1, 3)` returns `"hel"`, not `"ell"`. This matches classic BASIC convention. See `docs/language-spec-v1.md` §10.

**`STR$` is culture-sensitive.** On systems with a decimal separator of `,`, `STR$(3.14)` returns `"3,14"`. Use `ToUpperInvariant()` / `ToLowerInvariant()` everywhere strings are compared against fixed constants. See `theory/pitfalls.md` — ToUpperInvariant note.

**No bounds checking in `MID$`, `LEFT$`, `RIGHT$`.** These functions do not validate that `length` or `n` are within range. Providing a length that exceeds the string length throws an unhandled C# exception. Document this as a known limitation. See `docs/language-spec-v1.md` §10.

**`RND()` returns a Float in `[0.0, 1.0)`.** Use `INT(RND() * n)` for a random integer in `[0, n)`.

## New tokens

None. Built-in function names are resolved by the evaluator as `Identifier` tokens, not as dedicated keywords.

## New AST nodes

```csharp
public record InputStatement(string? Prompt, string VariableName) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_Len_ReturnsStringLength()
{
    Run("PRINT LEN(\"hello\")").Should().Be("5");
}

[Fact]
public void Evaluate_Upper_ConvertsToUpperCase()
{
    Run("PRINT UPPER$(\"hello\")").Should().Be("HELLO");
}

[Fact]
public void Evaluate_MidDollar_UsesOneBasedIndexing()
{
    Run("PRINT MID$(\"hello\", 2, 3)").Should().Be("ell");
}

[Fact]
public void Evaluate_LeftDollar_ReturnsFirstNChars()
{
    Run("PRINT LEFT$(\"abcdef\", 3)").Should().Be("abc");
}

[Fact]
public void Evaluate_StrDollar_ConvertsIntToString()
{
    Run("PRINT STR$(42)").Should().Be("42");
}

[Fact]
public void Evaluate_Val_ParsesStringToInteger()
{
    Run("PRINT VAL(\"99\")").Should().Be("99");
}

[Fact]
public void Evaluate_Abs_ReturnsAbsoluteValue()
{
    Run("PRINT ABS(-42)").Should().Be("42");
}

[Fact]
public void Evaluate_Sqr_ReturnsSquareRoot()
{
    Run("PRINT SQR(16)").Should().Be("4");
}

[Fact]
public void Evaluate_Typename_ReturnsRuntimeTypeName()
{
    Run("PRINT TYPENAME(42)").Should().Be("Integer");
    Run("PRINT TYPENAME(\"x\")").Should().Be("String");
    Run("PRINT TYPENAME(TRUE)").Should().Be("Boolean");
}

[Fact]
public void Evaluate_BuiltinNameCannotBeShadowed()
{
    var act = () => Run("FUNCTION LEN(s AS STRING) AS INTEGER\n  RETURN 0\nEND FUNCTION");
    act.Should().Throw<RuntimeError>().WithMessage("*LEN*built in*");
}
```

## Gotchas

- **`ToUpperInvariant()` throughout.** Every comparison of a built-in name, keyword, or type name against a fixed constant must use `ToUpperInvariant()`. The Turkish locale breaks `ToUpper()`. See `theory/pitfalls.md`.
- **`INT` returns `Float` when given a `Float`.** `INT(3.9)` returns the `Float` value `3.0`, not the `Integer` `3`. See `docs/language-spec-v1.md` §10.
- **`MID$` is 1-based.** Test this explicitly — off-by-one errors on the index are easy to introduce.
- **`STR$` culture sensitivity.** Document this known limitation rather than fixing it — fixing it requires threading an invariant culture through the conversion, which is out of scope.

## End state

```bash
dotnet test   # all standard library tests green
dotnet run --project src/SharpBasic.Repl -- run samples/hello.sbx
Hello, World!
```

SharpBASIC is complete. Write a retrospective: what was harder than expected, what the test suite caught that manual testing would have missed, what you would design differently. That retrospective is the foundation of the next project.

## What comes next

SharpBASIC is done. The retrospective you write now informs the next project. The natural next step is Part III of *Crafting Interpreters* — implementing clox, a bytecode VM, in C. See `theory/reading-list.md`.
