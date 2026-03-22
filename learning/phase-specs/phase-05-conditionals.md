# Phase 5 — Conditionals: IF / THEN / ELSE / END IF

## Goal

Structured conditionals work, including `ELSE` branches and nesting.

## Honest difficulty

Moderate. The concepts build directly on Phase 4. The parser extension is mechanical — read tokens until you hit `ELSE` or `END IF`. Nesting works for free once the recursive structure is in place.

## What you'll build

- `IfStatement(Condition, ThenBlock, ElseBlock?)` AST node
- Block parsing — read a list of statements until a terminator token is found
- Evaluator extended to handle `IfStatement`
- `AND`, `OR`, `NOT` fully wired if not added in Phase 4

## Key concepts

**`THEN` introduces a block, not a single statement.** SharpBASIC has no single-line `IF`. After `THEN`, the parser reads statements until it encounters `ELSE` or `END IF`. The same block-reading logic handles the `ELSE` branch.

**`END IF` is two tokens.** The parser sees `END` and must peek at the next token to know it is `END IF` and not `END SUB` or `END FUNCTION`. Establish this pattern cleanly in Phase 5 — it repeats in Phases 7 and 8.

**There is no `ELSEIF`.** Multiple branches are handled by nesting `IF` blocks inside `ELSE` blocks. The grammar is simple; the indentation makes it readable.

**Conditions must evaluate to `Boolean`.** Applying a non-boolean value as an `IF` condition is a runtime error. The evaluator should check this explicitly and report a clear message.

**`AND`, `OR`, and `NOT` must be fully wired by the end of this phase.** A realistic `IF` condition needs them. If they were not added in Phase 4, add them now — two rows in the binding power table and one prefix rule for `NOT`. See `theory/pitfalls.md` — Lesson D1.

## New tokens

None — `If`, `Then`, `Else`, `End`, `And`, `Or`, `Not` were defined in Phase 1.

## New AST nodes

```csharp
public record IfStatement(
    Expression Condition,
    IReadOnlyList<Statement> ThenBlock,
    IReadOnlyList<Statement>? ElseBlock) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_IfTrue_ExecutesThenBlock()
{
    Run("IF 1 = 1 THEN\n  PRINT \"yes\"\nEND IF").Should().Be("yes");
}

[Fact]
public void Evaluate_IfFalse_SkipsThenBlock()
{
    Run("IF 1 = 2 THEN\n  PRINT \"yes\"\nEND IF").Should().Be("");
}

[Fact]
public void Evaluate_IfFalse_ExecutesElseBlock()
{
    var code = "IF 1 = 2 THEN\n  PRINT \"yes\"\nELSE\n  PRINT \"no\"\nEND IF";
    Run(code).Should().Be("no");
}

[Fact]
public void Evaluate_NestedIf_WorksCorrectly()
{
    var code = """
        LET x = 7
        IF x > 5 THEN
            IF x > 10 THEN
                PRINT "big"
            ELSE
                PRINT "medium"
            END IF
        ELSE
            PRINT "small"
        END IF
        """;
    Run(code).Should().Be("medium");
}

[Fact]
public void Evaluate_CompoundConditionWithAnd()
{
    var code = "LET x = 7\nIF x > 0 AND x < 10 THEN\n  PRINT \"in range\"\nEND IF";
    Run(code).Should().Be("in range");
}

[Fact]
public void Evaluate_NotOperator()
{
    var code = "LET flag = FALSE\nIF NOT flag THEN\n  PRINT \"not false\"\nEND IF";
    Run(code).Should().Be("not false");
}
```

## Gotchas

- **`AND`, `OR`, `NOT` must be wired.** The first realistic test you write will use one of them. If they are missing, the test fails with a silent parse error. See `theory/pitfalls.md`.
- **Condition type check.** If the condition evaluates to something other than `BoolValue`, report a descriptive runtime error: `"IF condition must evaluate to a boolean value."` See `docs/language-spec-v1.md` §7.1.
- **`END IF` is two tokens.** Your block parser stops when it encounters the `End` token followed by `If`. If it stops on `End` alone, subsequent `END SUB` and `END FUNCTION` will also terminate `IF` blocks incorrectly.

## End state

```bash
dotnet test   # all conditional tests green
```

```
> IF 5 > 3 THEN
>   PRINT "yes"
> END IF
yes
> IF 1 = 2 THEN
>   PRINT "yes"
> ELSE
>   PRINT "no"
> END IF
no
```

## What comes next

Phase 6 — `FOR / NEXT` and `WHILE / WEND`. The block parsing pattern established here applies directly to loop bodies.
