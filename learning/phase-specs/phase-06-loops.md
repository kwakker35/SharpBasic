# Phase 6 — Loops: FOR / NEXT and WHILE / WEND

## Goal

Both loop types work, including `STEP` for `FOR` and negative steps for counting down.

## Honest difficulty

Moderate. Loops are satisfying to implement once conditionals work. The block parsing pattern from Phase 5 applies directly. `STEP` adds one expression and a direction check.

## What you'll build

- `ForStatement(Variable, From, To, Step?, Body)` AST node
- `WhileStatement(Condition, Body)` AST node
- Evaluator extended to handle both loop types
- Loop guard logic: count up when step is positive, count down when step is negative

## Key concepts

**`FOR` and `WHILE` share the block parsing pattern from Phase 5.** The body is a list of statements read until a terminator token (`NEXT` or `WEND`). The difference is what triggers re-evaluation: `FOR` uses a counter, `WHILE` re-evaluates a condition.

**`FOR` loop guard direction.** The default step is `1`. When step is positive, the loop continues while `counter <= limit`. When step is negative, the loop continues while `counter >= limit`. Without this directional check, `FOR i = 10 TO 1 STEP -1` loops forever.

**`NEXT` may optionally name the variable.** `NEXT i` is valid; `NEXT` alone is also valid. If a name is present, it must match the loop variable — a mismatch is a parse error.

**`WHILE` condition is re-evaluated before every iteration.** It must evaluate to `Boolean`. A non-boolean condition is a runtime error: `"WHILE condition must evaluate to a boolean value."` See `docs/language-spec-v1.md` §7.3.

**Loop variable scoping.** The `FOR` loop counter is declared in the current scope on entry to the loop. It persists after the loop completes — this is consistent with classic BASIC behaviour.

## New tokens

None — `For`, `To`, `Step`, `Next`, `While`, `Wend` were defined in Phase 1.

## New AST nodes

```csharp
public record ForStatement(
    string Variable,
    Expression From,
    Expression To,
    Expression? Step,
    IReadOnlyList<Statement> Body) : Statement;

public record WhileStatement(
    Expression Condition,
    IReadOnlyList<Statement> Body) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_ForLoop_PrintsRange()
{
    Run("FOR i = 1 TO 3\n  PRINT i\nNEXT").Should().Be("1\n2\n3");
}

[Fact]
public void Evaluate_ForLoop_WithStep()
{
    Run("FOR i = 0 TO 10 STEP 3\n  PRINT i\nNEXT").Should().Be("0\n3\n6\n9");
}

[Fact]
public void Evaluate_ForLoop_CountsDown()
{
    Run("FOR i = 3 TO 1 STEP -1\n  PRINT i\nNEXT").Should().Be("3\n2\n1");
}

[Fact]
public void Evaluate_ForLoop_WithNamedNext()
{
    Run("FOR i = 1 TO 3\n  PRINT i\nNEXT i").Should().Be("1\n2\n3");
}

[Fact]
public void Evaluate_WhileLoop_ExecutesWhileTrue()
{
    var code = "LET n = 1\nWHILE n <= 3\n  PRINT n\n  LET n = n + 1\nWEND";
    Run(code).Should().Be("1\n2\n3");
}

[Fact]
public void Evaluate_WhileLoop_NeverExecutesIfFalseInitially()
{
    Run("WHILE FALSE\n  PRINT \"never\"\nWEND").Should().Be("");
}
```

## Gotchas

- **Default step is `1.0`, not `1`.** The internal accumulator is a `double`. The counter is stored as `IntValue((int)i)` each iteration. Sub-integer step values are silently truncated in the symbol table. Use integer values for `STEP`. See `theory/pitfalls.md` and `docs/language-spec-v1.md` §7.2.
- **Direction check required for `STEP`.** Without checking step direction, a descending loop runs forever because `counter <= limit` is always false from the start.
- **`WHILE` condition type check.** As with `IF`, a non-boolean condition should produce a clear runtime error, not a silent failure.

## End state

```bash
dotnet test   # all loop tests green
```

```
> FOR i = 1 TO 5
>   PRINT i
> NEXT
1
2
3
4
5
> LET n = 3
> WHILE n > 0
>   PRINT n
>   LET n = n - 1
> WEND
3
2
1
```

## What comes next

Phase 7 — SUBs and FUNCTIONs. Read `theory/call-frames.md` before starting. The SymbolTable parent chain from Phase 3 becomes load-bearing here.
