# Phase 8 — Arrays, DIM, and Bounds Checking

## Goal

`DIM scores[10] AS INTEGER`, `LET scores[3] = 42`, and `PRINT scores[3]` all work. Out-of-bounds access produces a clear error.

## Honest difficulty

Moderate. Arrays are mechanical once Phase 7 is solid. The new syntax is small; the evaluator change is straightforward.

## What you'll build

- `DimStatement(Name, Size, TypeName)` AST node
- `ArrayAccessExpression(Name, Index)` AST node
- `ArrayAssignStatement(Name, Index, Value)` AST node
- Runtime array storage (a fixed-size typed array in the symbol table)
- Bounds checking with a descriptive error message
- Zero-initialisation on declaration

## Key concepts

**Arrays use square bracket syntax.** `DIM scores[10] AS INTEGER` declares a 10-element integer array. `LET scores[3] = 42` assigns element 3. `PRINT scores[3]` reads element 3. The square brackets distinguish array access from parenthesised expressions. See `docs/language-spec-v1.md` §5.

**Arrays are zero-indexed.** Valid indices are `0` to `size - 1`. An out-of-bounds access or assignment produces a runtime error, not a silent failure or crash. The error message includes the index, the valid range, and the array name.

**Arrays are zero-initialised.** On declaration, every element is set to the zero value for its type: `0` for `INTEGER`, `0.0` for `FLOAT`, `FALSE` for `BOOLEAN`, `""` for `STRING`.

**`DIM` when the name already exists is an error.** Re-declaring an array with `DIM` when a variable or array with that name already exists in the current scope produces a runtime error. See `docs/language-spec-v1.md` §5.1.

**The type annotation is mandatory.** `DIM scores[10] AS INTEGER` — the `AS TYPE` clause is not optional. The type constrains what can be stored. Assigning the wrong type produces a runtime error: `"Supplied value is not an INTEGER as defined for the array: scores."` See `docs/language-spec-v1.md` §5.3.

**`LET` is required for array assignment.** `LET scores[3] = 42` — not `scores[3] = 42`. The `LET` keyword is mandatory, consistent with variable assignment.

## New tokens

`LeftBracket`, `RightBracket` — these should have been defined in Phase 1. If they were not, add them now.

## New AST nodes

```csharp
public record DimStatement(
    string Name,
    int Size,
    string TypeName) : Statement;

public record ArrayAccessExpression(
    string Name,
    Expression Index) : Expression;

public record ArrayAssignStatement(
    string Name,
    Expression Index,
    Expression Value) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_DimAndAccess_ReturnsZeroInitialised()
{
    Run("DIM scores[5] AS INTEGER\nPRINT scores[0]").Should().Be("0");
}

[Fact]
public void Evaluate_ArrayAssignAndRead_RoundTrips()
{
    Run("DIM scores[5] AS INTEGER\nLET scores[3] = 42\nPRINT scores[3]").Should().Be("42");
}

[Fact]
public void Evaluate_ArrayOutOfBounds_ReportsError()
{
    var act = () => Run("DIM arr[5] AS INTEGER\nLET arr[5] = 1");
    act.Should().Throw<RuntimeError>().WithMessage("*5*arr*");
}

[Fact]
public void Evaluate_ArrayLoop_PrintsAllElements()
{
    var code = """
        DIM primes[5] AS INTEGER
        LET primes[0] = 2
        LET primes[1] = 3
        LET primes[2] = 5
        LET primes[3] = 7
        LET primes[4] = 11
        FOR i = 0 TO 4
            PRINT primes[i]
        NEXT
        """;
    Run(code).Should().Be("2\n3\n5\n7\n11");
}

[Fact]
public void Evaluate_DimRedeclaration_ReportsError()
{
    var act = () => Run("LET x = 1\nDIM x[5] AS INTEGER");
    act.Should().Throw<RuntimeError>().WithMessage("*x*already exists*");
}

[Fact]
public void Evaluate_ArrayWrongType_ReportsError()
{
    var act = () => Run("DIM arr[5] AS INTEGER\nLET arr[0] = \"hello\"");
    act.Should().Throw<RuntimeError>().WithMessage("*INTEGER*arr*");
}
```

## Gotchas

- **Square bracket tokens.** If `LeftBracket` and `RightBracket` were not defined in Phase 1, the lexer has no rule for `[` and `]`. They produce `Unknown` tokens and the parser fails silently.
- **Bounds error message.** The error message includes the supplied index, the valid range (0 to size−1), and the array name. The upper bound in the error is the declared `size`, which is the exclusive limit. See `docs/language-spec-v1.md` §5.3 for the exact wording.
- **Accessing a non-array as an array.** `LET x = 5` then `PRINT x[0]` should produce: `"Identifier: x is not an Array."` This is distinct from an out-of-bounds access.
- **Variable shadowing.** `DIM` in a nested scope (inside a function) does not affect the outer scope. The re-declaration check applies only within the current scope.

## End state

```bash
dotnet test   # all array tests green
```

```
> DIM scores[5] AS INTEGER
> LET scores[0] = 95
> LET scores[1] = 87
> PRINT scores[0]
95
> PRINT scores[1]
87
```

## What comes next

Phase 9 — Error handling and diagnostics. The error infrastructure started in Phase 3 gets a proper surface: line/column reporting, structured diagnostics, and error recovery in the parser.
