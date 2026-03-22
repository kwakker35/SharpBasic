# Pitfalls and Lessons Learned

> Every entry here cost real time during the build.  
> Knowing it in advance won't remove the struggle —  
> but it will stop you hitting the same walls twice.

---

## Lexer and tokenisation

### CRLF line endings silently break tests written with raw string literals

**What goes wrong:** Tests using C# raw string literals (`"""..."""`) fail to parse. All tokens appear on line 1; newlines tokenise as `Unknown`.

**Why:** C# raw string literals on Windows produce `\r\n` (CRLF). If your lexer only handles `\n`, the `\r` falls through to the `default` case, gets buffered into the current token, then when `\n` arrives it flushes an `Unknown` token containing the carriage return. Tests using explicit `"\n"` strings pass silently because they have no `\r`.

**Fix:** Normalise line endings in the `Lexer` constructor before any tokenising:

```csharp
_source = source.Replace("\r\n", "\n").Replace("\r", "\n");
```

Do this at the system boundary — the constructor — before any tokenising happens.

**Cross-reference:** `phase-specs/phase-01-lexer.md`

---

### Operator cases must flush the token buffer

**What goes wrong:** An identifier immediately before a punctuation character is silently dropped. `Greet(` tokenises as just `LParen` — the `Greet` identifier disappears.

**Why:** The space and string-literal cases have a buffer-flush guard (`if (token.Length > 0)`) before emitting their token. Operator and punctuation cases commonly do not. When the lexer hits `(` mid-word, it emits `LParen` and discards whatever was buffered.

**Fix:** Every character that terminates a word must flush the pending buffer before emitting its own token. This applies to every operator and punctuation case, not just the ones you discover are broken.

**Cross-reference:** `phase-specs/phase-01-lexer.md`

---

### `GetStringLiteral` advancing one character too many

**What goes wrong:** The character immediately after a closing `"` is skipped. `)` in `Greet("Alice")` is dropped, causing a parse failure.

**Why:** `GetStringLiteral` calls `Advance()` to step past the closing `"`. The outer loop then calls `Advance()` again at the top of the next iteration, skipping whatever followed the `"`.

**Fix:** Remove the final `Advance()` from inside the string literal helper. The outer loop's advance at the start of the next iteration lands on the correct next character.

**Cross-reference:** `phase-specs/phase-01-lexer.md`

---

### Use `ToUpperInvariant()` not `ToUpper()` for keyword and builtin comparisons

**What goes wrong:** On a machine configured with a Turkish locale, `"len".ToUpper()` returns `"LEN"` on most systems but `"İEN"` (with a dotted capital I, U+0130) on Turkish systems. Built-in function lookup and keyword recognition fail silently on those machines.

**Why:** `String.ToUpper()` is culture-sensitive. The Turkish locale has different case rules for the letter I.

**Fix:** Use `ToUpperInvariant()` and `ToLowerInvariant()` everywhere you compare against fixed string constants (keywords, built-in names, type names). These methods always use invariant culture rules regardless of locale.

```csharp
// For display — culture-sensitive is fine
Console.WriteLine(value.ToString());

// For comparison — always use invariant
_builtins.TryGetValue(expr.Name.ToUpperInvariant(), ...)
stmt.TypeName.ToUpperInvariant() switch { "INTEGER" => ... }
```

The rule of thumb: `ToUpper()` for display, `ToUpperInvariant()` for comparison.

**Cross-reference:** `phase-specs/phase-10-stdlib.md`

---

## Parsing and AST

### `ParseCallExpression` must be called from `ParsePrimary` for expression-position calls

**What goes wrong:** `LET result = Add(1, 2)` — `Add` parses as an `IdentifierExpression` and `(1, 2)` is dangling garbage. Function calls only work at statement level (`CALL Foo(...)`), not in expressions.

**Why:** `ParseCallStatement` handles `CALL Foo(...)` as a statement. Without a corresponding `ParseCallExpression`, identifiers followed by `(` in expression position are not recognised as calls.

**Fix:** In `ParsePrimary`, after recognising an `Identifier` token, peek at the next token. If it is `LParen`, dispatch to `ParseCallExpression` rather than returning a bare `IdentifierExpression`.

```csharp
if (Current.Type == TokenType.Identifier)
{
    if (Peek().Type == TokenType.LParen)
        return ParseCallExpression();

    return new IdentifierExpression(Current.Value, loc);
}
```

**Cross-reference:** `phase-specs/phase-07-functions.md`

---

### `Advance()` after `ParsePrimary` must not fire for call expressions

**What goes wrong:** `Fib(n - 1) + Fib(n - 2)` — the `+` is silently consumed. The expression returns only `Fib(n - 1)`. Single-call tests pass because the extra advance consumes a harmless newline.

**Why:** `ParseExpression` calls `Advance()` after `ParsePrimary()`. For literals and identifiers this is correct — they leave `Current` pointing at the token they represent, and you must advance past it. But `ParseCallExpression` already consumed everything through `)`. The extra advance then steps over the following operator.

**Fix:** Advance only when `ParsePrimary` returned something other than a call expression:

```csharp
left = ParsePrimary();
if (left is null) return null;
if (left is not CallExpression)
    Advance();
```

This is a subtle asymmetry that does not surface until you have multiple call expressions in the same expression.

**Cross-reference:** `phase-specs/phase-07-functions.md`

---

## Expressions and operator precedence

### `AND` / `OR` / `NOT` belong in Phase 5, not Phase 7

**What goes wrong:** The first realistic `IF` statement you write — `IF x > 0 AND x < 10 THEN` — does not work. `AND` is either not lexed, not parsed, or not evaluated.

**Why:** It is tempting to defer `AND`, `OR`, and `NOT` until Phase 7 when the token enum is expanding anyway. But by Phase 5, `IF` without logical operators is artificially limited to single conditions. Any real conditional needs them.

**Fix:** Add `AND`, `OR`, and `NOT` in Phase 5 alongside `IF`. They are two more rows in `BindingPower()` (the Pratt loop handles them for free) and one prefix rule for `NOT`. The work is small. The payoff — being able to write realistic conditionals immediately — is large.

**Cross-reference:** `phase-specs/phase-05-conditionals.md`

---

### Unary minus must handle expressions, not just literals

**What goes wrong:** `-5` and `-3.14` work. `-n` and `-(a + b)` silently fail. The parser returns `null` and the expression produces no output or an error.

**Why:** It is easy to implement unary minus only for the literal case — the test `LET x = -5` passes and the phase feels complete. But the prefix unary minus case must fall through to `ParsePrimary` and handle any expression, not just literals.

**Fix:** In the `ParsePrimary` unary minus branch, call `ParseExpression` with a sufficiently high minimum binding power, not a literal-specific parse:

```csharp
case TokenType.Minus:
    Advance();
    var operand = ParseExpression(minBp: 15); // binds tighter than + or -
    return new UnaryExpression(UnaryOp.Negate, operand);
```

Add a test for `-n` and `-(a + b)` to the Phase 4 test suite to catch this before moving on.

**Cross-reference:** `phase-specs/phase-04-expressions.md`

---

### `&` has the same binding power as `+` and `-`

**What this means:** `"x=" & 1 + 2` evaluates as `"x=" & 3` → `"x=3"`, not `"x=12"`. The `+` binds tighter than `&`.

**Why this matters:** When constructing output strings, unexpected precedence produces wrong output with no error. `PRINT "Total: " & a + b` adds `a` to `b` first, then concatenates — which is usually wrong.

**Fix:** Use parentheses when the intent could be ambiguous: `PRINT "Total: " & (a + b)` and `PRINT "Total: " & a & " and " & b`.

**Cross-reference:** `phase-specs/phase-04-expressions.md`, `docs/language-spec-v1.md` §6.6

---

## Control flow

### `FOR` loop counter is stored as `IntValue` even when step is a float

**What goes wrong:** A `FOR` loop with a float step value produces confusing counter values when read back as an integer.

**Why:** The loop counter is stored as `IntValue((int)i)` every iteration even when the internal accumulator is a `double`. Sub-integer step values are silently truncated in the symbol table.

**Fix:** Use integer values for `STEP` in `FOR` loops. If sub-integer stepping is needed, use a `WHILE` loop with a float accumulator variable.

**Cross-reference:** `phase-specs/phase-06-loops.md`, `docs/language-spec-v1.md` §7.2

---

### `REM` recognition requires a token boundary after the word

**What goes wrong:** A source file ending with the bare text `REM` and no trailing newline or space causes the lexer to flush `REM` as an `Identifier`. The parser reports an error.

**Fix:** Always end source files with a trailing newline. The lexer requires a space, newline, or another token boundary after `REM` to recognise it as a comment.

**Cross-reference:** `phase-specs/phase-01-lexer.md`, `docs/language-spec-v1.md` §2.4

---

## Functions and scope

### Child evaluators must receive the parent's declaration dictionaries

**What goes wrong:** Recursive calls to a function silently return failure. `Fib(n - 1)` inside `Fib` fails to find `Fib`. Non-recursive calls to other top-level functions from inside a function also fail.

**Why:** Creating a child evaluator with `new Evaluator(body, localScope)` gives it fresh, empty `_subs` and `_functions` dictionaries. When the child calls `HoistDeclarations()` on the function body, it finds no declarations. All subsequent lookups fail silently.

**Fix:** Pass the parent's dictionaries to the child evaluator explicitly:

```csharp
new Evaluator(new Program(func.Body), localScope, _subs, _functions).Evaluate()
```

`Dictionary<K,V>` is a reference type — the child and parent share the same lookup tables.

**Cross-reference:** `phase-specs/phase-07-functions.md`

---

### Function and sub names are case-sensitive at call sites

**What goes wrong:** `CALL greet()` fails to find a sub declared as `SUB Greet()`.

**Why:** SharpBASIC identifiers are case-sensitive. Sub and function names are identifiers. The keyword `CALL` is case-insensitive; the name after it is not.

**Fix:** Declare and call with consistent casing throughout. The convention used in the spec samples is `PascalCase` for sub and function names.

**Cross-reference:** `phase-specs/phase-07-functions.md`, `docs/language-spec-v1.md` §8

---

### `STR$` uses the system locale for float formatting

**What goes wrong:** `STR$(3.14)` returns `"3,14"` (comma as decimal separator) on machines configured with a European locale.

**Why:** `STR$` converts a float via C# `double.ToString()` without an explicit culture argument. Systems with a decimal separator of `,` produce different output from systems with `.`.

**Fix:** Avoid relying on `STR$` for float-to-string conversion in portable code. `PRINT` and `&` use invariant culture internally and are not affected.

**Cross-reference:** `phase-specs/phase-10-stdlib.md`, `docs/language-spec-v1.md` §10

---

## Error handling and diagnostics

### Silent `EvalFailure` is indistinguishable from empty output

**What goes wrong:** A test that calls `Run(code)` and expects output receives `""`. No exception. No message. The test fails with a misleading assertion.

**Why:** An `EvalFailure` from deep in the evaluator is returned up the call chain and eventually produces an empty string rather than throwing. `ParseFailure` has the same behaviour if not explicitly handled.

**Diagnosis approach:**
1. Check parse first. Add explicit handling for `ParseFailure` and log its errors. Most silent failures are parse failures.
2. If parse succeeds, instrument `EvaluatePrintStatement`. Log what `EvaluateExpression` returns — is it `EvalSuccess` or `EvalFailure`?
3. Write diagnostics to a file (`File.AppendAllText(...)`) rather than `Console.Error`. xUnit captures both stdout and stderr from the test process. A file bypasses capture entirely.

**Cross-reference:** `phase-specs/phase-09-diagnostics.md`

---

## General — applies across all phases

### "Can I write a realistic program?" is a phase-completion test

A phase that passes its own test suite but fails when writing a realistic program is not complete. The gaps above — missing `AND`/`OR`, unary minus on variables — were not in the initial test lists and only surfaced when writing real programs.

Add a mandatory capstone snippet to each phase: a short, realistic program that exercises every feature introduced. If it does not run correctly, the phase is not done.

Examples:
- Phase 4 capstone: `LET result = -n * 2` — verifies unary minus on a variable.
- Phase 5 capstone: `IF x > 0 AND x < 10 THEN PRINT "in range"` — verifies logical operators.

---

### Token and semantics are separate concerns

Adding a token to the `TokenType` enum feels like done. It is not. Every new token also requires:
- A lexer case to recognise it
- A parser binding power entry and/or a parse function
- An evaluator case to execute it
- At least one test for each

Track these four concerns explicitly for every new language feature. Tokens added without parser or evaluator wiring sit silently broken until a test exposes them.
