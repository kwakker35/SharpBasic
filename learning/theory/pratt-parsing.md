# Pratt Parsing

> Read this before starting Phase 4.

---

## Why expressions are hard

By the end of Phase 3 you can parse `LET x = 42` and `PRINT x`. The parser for those is straightforward recursive descent — you know exactly what tokens to expect in what order.

Expressions break that model. `2 + 3 * 4` should produce `14`, not `20`. The parser must understand that `*` binds more tightly than `+`. And `AND` should bind more tightly than `OR`, but less tightly than `=`. And `NOT` is unary and should bind most tightly of all.

The naive solution is one parsing function per precedence level — `ParseOr` calls `ParseAnd`, which calls `ParseComparison`, which calls `ParseAddition`, which calls `ParseMultiplication`, which calls `ParseUnary`, which calls `ParsePrimary`. Seven levels of call. Adding one new operator potentially reshuffles the entire call hierarchy.

This gets unmanageable fast. There is a better way.

---

## What Pratt parsing is

Pratt parsing (top-down operator precedence) assigns every operator a number — its *binding power* — and uses a single recursive function that stops recursing when it encounters an operator with binding power lower than the current minimum.

The key terms:
- **Binding power (BP):** A number representing how tightly an operator holds its operands. `*` has higher BP than `+`.
- **Prefix position:** A token that appears at the start of an expression (`-x`, `NOT flag`, a literal, an identifier, `(`).
- **Infix position:** A token that appears between two sub-expressions (`+`, `*`, `AND`, `=`).

Every token that can appear in prefix position has a prefix parse function. Every token that can appear in infix position has an infix parse function and a binding power.

---

## The key insight

The Pratt parser loops, not recurses. It parses a left-hand side, then checks whether the next token has high enough binding power to "take" that left-hand side as its left operand. If yes, it continues. If no, it returns.

```csharp
private Expression? ParseExpression(int minBp = 0)
{
    Expression? left = ParsePrimary(); // prefix position
    if (left is null) return null;

    while (true)
    {
        var op = Current;
        int bp = BindingPower(op.Type);
        if (bp <= minBp) break;          // not strong enough to continue

        Advance();
        Expression? right = ParseExpression(bp); // infix position, higher minimum
        if (right is null) return null;

        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

That is the entire Pratt loop. `BindingPower` is a switch expression returning a number for each operator type and 0 for everything else.

---

## How it maps to the code — trace through `2 + 3 * 4`

Tokens: `IntegerLiteral(2)`, `Plus`, `IntegerLiteral(3)`, `Star`, `IntegerLiteral(4)`, `Eof`

1. `ParseExpression(minBp: 0)` is called.
2. `ParsePrimary()` returns `IntegerLiteral(2)`. Current is now `Plus`.
3. `BindingPower(Plus)` returns `10`. `10 > 0` → continue.
4. Advance past `Plus`. Call `ParseExpression(minBp: 10)` for the right side.
   - `ParsePrimary()` returns `IntegerLiteral(3)`. Current is now `Star`.
   - `BindingPower(Star)` returns `20`. `20 > 10` → continue.
   - Advance past `Star`. Call `ParseExpression(minBp: 20)` for the right side.
     - `ParsePrimary()` returns `IntegerLiteral(4)`. Current is now `Eof`.
     - `BindingPower(Eof)` returns `0`. `0 <= 20` → break. Return `IntegerLiteral(4)`.
   - `left` becomes `BinaryExpression(3, *, 4)`. Current is `Eof`.
   - `BindingPower(Eof)` returns `0`. `0 <= 10` → break. Return `BinaryExpression(3, *, 4)`.
5. `left` becomes `BinaryExpression(2, +, BinaryExpression(3, *, 4))`.
6. `BindingPower(Eof)` returns `0`. `0 <= 0` → break. Return the expression.

Result: `2 + (3 * 4)` — correct.

---

## SharpBASIC binding powers

```csharp
private static int BindingPower(TokenType type) => type switch
{
    TokenType.Or                 => 2,
    TokenType.And                => 3,
    TokenType.Equals
    | TokenType.NotEquals
    | TokenType.LessThan
    | TokenType.GreaterThan
    | TokenType.LessThanOrEqual
    | TokenType.GreaterThanOrEqual => 5,
    TokenType.Plus
    | TokenType.Minus
    | TokenType.Ampersand        => 10,
    TokenType.Star
    | TokenType.Slash
    | TokenType.Mod              => 20,
    _                            => 0
};
```

`&` (string concatenation) has the same binding power as `+` and `-`. That means `"x=" & 1 + 2` is `"x=" & 3` → `"x=3"`, not `"x=12"`. This is correct — `+` binds tighter. Be aware of it when constructing output strings.

---

## Prefix position — unary operators and primary expressions

`ParsePrimary()` handles tokens in prefix position. It needs to handle:
- Integer literals, float literals, string literals, boolean literals
- Identifiers (variables, function calls)
- Parenthesised expressions: `(` → parse inner expression → expect `)`
- Unary minus: `-` → parse inner expression with high minimum BP
- `NOT`: unary boolean negation

The unary minus case is a common source of confusion. `ParsePrimary` sees `-`, advances, and calls `ParseExpression(minBp: 15)` — a minimum high enough that only multiplication-class operators bind tighter. The result is wrapped in a `UnaryExpression`.

**Gotcha:** If unary minus only handles literal tokens (`-5`, `-3.14`) rather than arbitrary expressions, negating a variable (`-n`) or a subexpression (`-(a + b)`) will silently fail. Implement the full prefix case from the start. See `theory/pitfalls.md`.

---

## Common mistakes on a first implementation

**Not advancing after `ParsePrimary` returns for non-call expressions.** The convention is that `ParsePrimary` returns with `Current` pointing at the token it parsed. For a literal, that means `Current` is still the literal token — you must advance past it before entering the Pratt loop. For a call expression that consumed everything through `)`, you must not advance again. Track which cases self-advance and which do not. See `theory/pitfalls.md` — Bug 5 in Phase 7 is exactly this failure mode, present from Phase 4.

**Returning 0 from `BindingPower` for operators you haven't implemented yet.** A missing entry doesn't cause an error — the loop just stops. You end up with an expression that silently ignores everything to the right. Always verify new operators with tests that confirm the full expression evaluates correctly.

**Forgetting that `NOT` is prefix, not infix.** It has no binding power in the infix table. It belongs in `ParsePrimary` as a prefix rule.

---

## Further reading

- *Crafting Interpreters* — Robert Nystrom, Chapter 6. The definitive treatment. Nystrom's version uses the terms "prefix" and "infix parselets" but the algorithm is identical.
- "Pratt Parsers: Expression Parsing Made Easy" — Bob Nystrom's blog post (journal.stuffwithstuff.com). Shorter than the book chapter and directly applicable.
