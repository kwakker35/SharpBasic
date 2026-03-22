# Phase 4 — Expressions & Arithmetic

> Read `theory/pratt-parsing.md` before starting this phase.  
> This is the first significant difficulty spike. Take the time.

## Goal

`PRINT 2 + 2 * 3` outputs `8`. Operator precedence is correct. `PRINT (2 + 2) * 3` outputs `12`.

## Honest difficulty

Hard. The Pratt parsing algorithm is the conceptual wall of this project. It is elegant and compact, but the insight — that operator precedence can be encoded as binding powers in a loop rather than a call hierarchy — takes time to land. Read the theory note first. Implement slowly. Test every step.

## What you'll build

- `BinaryExpression(Left, Operator, Right)` AST node
- `UnaryExpression(Operator, Operand)` AST node
- `GroupingExpression(Inner)` AST node (or handle parentheses directly in `ParsePrimary`)
- Pratt parser replacing the naive expression parser
- Arithmetic: `+`, `-`, `*`, `/`, `MOD`
- String concatenation: `&`
- Unary: `-` (negation), `NOT`
- Comparison operators: `=`, `<>`, `<`, `>`, `<=`, `>=`
- Evaluator extended to handle `BinaryExpression` and `UnaryExpression`

## Key concepts

**Binding powers control precedence.** Each operator has a numeric binding power. Higher BP binds tighter. The Pratt loop continues consuming operators as long as the next operator's BP exceeds the current minimum. See `theory/pratt-parsing.md` for the binding power table and a worked trace.

**Prefix vs infix position.** A `-` at the start of an expression (`-5`, `-n`) is unary negation — prefix position. A `-` between two expressions (`a - b`) is subtraction — infix position. The same token, different parse rules. `ParsePrimary` handles prefix position; the Pratt loop handles infix position.

**Integer / Float promotion.** Two `Integer` operands produce `Integer` unless the result is not a whole number (division). An `Integer` operand mixed with a `Float` operand promotes to `Float`. `10 / 4` produces `2.5` as `Float`.

**`&` for string concatenation.** The `+` operator does not concatenate strings. `&` calls `.ToString()` on both sides, so any type can be concatenated with any other. `"Count: " & 42` produces `"Count: 42"`.

**Binding power of `&`.** `&` has the same binding power as `+` and `-` (10). This means `"x=" & 1 + 2` evaluates as `"x=" & 3` → `"x=3"`. See `theory/pitfalls.md`.

## New tokens

None — all tokens were defined in Phase 1.

## New AST nodes

```csharp
public record BinaryExpression(Expression Left, Token Operator, Expression Right) : Expression;
public record UnaryExpression(Token Operator, Expression Operand) : Expression;
```

## Test examples

```csharp
[Fact]
public void Evaluate_Addition_ReturnsCorrectResult()
{
    Run("PRINT 2 + 3").Should().Be("5");
}

[Fact]
public void Evaluate_PrecedenceMultiplicationBeforeAddition()
{
    Run("PRINT 2 + 3 * 4").Should().Be("14");
}

[Fact]
public void Evaluate_Parentheses_OverridePrecedence()
{
    Run("PRINT (2 + 3) * 4").Should().Be("20");
}

[Fact]
public void Evaluate_Division_ProducesFloat()
{
    Run("PRINT 10 / 4").Should().Be("2.5");
}

[Fact]
public void Evaluate_Modulo_ReturnsRemainder()
{
    Run("PRINT 10 MOD 3").Should().Be("1");
}

[Fact]
public void Evaluate_StringConcatenation_CombinesValues()
{
    Run("LET name = \"World\"\nPRINT \"Hello, \" & name & \"!\"").Should().Be("Hello, World!");
}

[Fact]
public void Evaluate_UnaryMinus_OnVariable()
{
    Run("LET n = 5\nPRINT -n").Should().Be("-5");
}

[Fact]
public void Evaluate_ComparisonEqual_ReturnsBoolean()
{
    Run("PRINT 3 = 3").Should().Be("True");
    Run("PRINT 3 = 4").Should().Be("False");
}
```

## Gotchas

- **Unary minus must handle expressions, not just literals.** `-n` and `-(a + b)` must work. See `theory/pitfalls.md`.
- **`Advance()` after `ParsePrimary` must not fire for call expressions** (relevant from Phase 7 onwards, but the asymmetry is worth establishing cleanly now). See `theory/pitfalls.md`.
- **`&` binding power equals `+` and `-`.** See `theory/pitfalls.md`.
- **Dividing two integers.** `10 / 4` should return `2.5` (`Float`), not `2` (`Integer`). Check that the evaluator promotes correctly.
- **Add `AND`, `OR`, and `NOT` in this phase or Phase 5.** They are just two more rows in the binding power table. Adding them here means `IF` in Phase 5 works with realistic conditions immediately. See `theory/pitfalls.md` — Lesson D1.

## End state

```bash
dotnet test   # all expression tests green
```

```
> PRINT 2 + 2 * 3
8
> PRINT (2 + 2) * 3
12
> PRINT 10 / 4
2.5
> PRINT "Hello" & ", " & "World"
Hello, World
```

## What comes next

Phase 5 — `IF / THEN / ELSE / END IF`. The expression parser built here handles all conditions.
