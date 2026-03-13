# SharpBASIC — Lessons Learned

A living record of bugs discovered, root causes, and fixes. Useful as a learning reference for anyone following this project's build path.

---

## Phase 7 — Subroutines & Functions

### Bug 1 — Lexer: CRLF line endings not handled

**Symptom:** Tests using C# raw string literals (`"""..."""`) failed to parse entirely. All tokens appeared on line 1; newlines tokenised as `Unknown`.

**Root cause:** The lexer handled `'\n'` but not `'\r'`. C# raw string literals on Windows produce `\r\n` (CRLF). The `\r` fell through to `default:` in the switch, got buffered into the current token, then when `\n` arrived it flushed an `Unknown` token containing the `\r`. Regular string tests used explicit `"\n"` — no `\r` — so they passed silently.

**Fix:** Normalise line endings in the `Lexer` constructor before any tokenising:

```csharp
_source = source.Replace("\r\n", "\n").Replace("\r", "\n");
```

**Lesson:** Always normalise input at the system boundary (the constructor). Never assume source strings have Unix line endings — files, raw string literals, and clipboard pastes on Windows all produce CRLF.

---

### Bug 2 — Lexer: Operator cases didn't flush the token buffer

**Symptom:** `Greet(` tokenised as just `LParen` — the `Greet` identifier was silently dropped.

**Root cause:** The `' '` and `'"'` cases had an `if (token.Length > 0)` flush guard before emitting their token. None of the operator/punctuation cases (`+`, `-`, `(`, `)`, etc.) did. So when the lexer hit `(` mid-word, it emitted `LParen` and discarded the buffered text.

**Fix:** Add the same buffer-flush block to every operator and punctuation case.

**Lesson:** Any character that terminates a word must flush the pending buffer before emitting its own token.

---

### Bug 3 — Lexer: `GetStringLiteral` double-advance

**Symptom:** The character immediately after a closing `"` was skipped. e.g. `)` in `Greet("Alice")` was dropped.

**Root cause:** `GetStringLiteral` called `Advance()` to step past the closing `"` before returning. The outer `while` loop then called `Advance()` again at the top of the next iteration — skipping whichever character followed the `"`.

**Fix:** Remove the final `Advance()` inside `GetStringLiteral`. The outer loop's advance lands on the correct next character.

---

### Bug 4 — Parser: `ParseCallExpression` missing entirely

**Symptom:** `LET result = Add(1, 2)` — `Add` parsed as an `IdentifierExpression`; `(1, 2)` was dangling garbage.

**Root cause:** `ParseCallStatement` handled `CALL Foo(...)` as a statement, but there was no equivalent for `Foo(...)` in expression position (RHS of `LET`, argument to `PRINT`, etc.). The `ParsePrimary` identifier branch had a `// Phase 7: peek ahead` comment but never implemented the peek.

**Fix:** Added `ParseCallExpression` returning `Expression?`, called from `ParsePrimary` when an `Identifier` is immediately followed by `LParen`:

```csharp
if (Current.Type == TokenType.Identifier)
{
    if (Peek().Type == TokenType.LParen)
        return ParseCallExpression();

    var identExpr = new IdentifierExpression(Current.Value, loc);
    return identExpr;
}
```

---

### Bug 5 — Parser: Unconditional `Advance()` after `ParsePrimary` consumed operators

**Symptom:** `Fib(n - 1) + Fib(n - 2)` — the `+` was silently eaten, making the expression return only `Fib(n - 1)`. Single-call tests passed because the extra `Advance()` consumed a harmless newline.

**Root cause:** `ParseExpression` always called `Advance()` after `ParsePrimary()`. For literals and identifiers this is correct — they leave `Current` pointing at the token they represent. But `ParseCallExpression` already consumed everything through `)`, so the extra `Advance()` stepped over the following `+` operator.

**Fix:** Only advance when `ParsePrimary` returned something other than a `CallExpression`:

```csharp
left = ParsePrimary();
if (left is null) return null;
if (left is not CallExpression)
    Advance(); // call expressions self-advance; others do not
```

---

### Bug 6 — Evaluator: Child evaluator had empty declaration dictionaries

**Symptom:** Recursive calls (`Fib` calling `Fib`) silently returned `EvalFailure` — the child evaluator couldn't find the function.

**Root cause:** `new Evaluator(Program, localSymbols)` created a fresh `_subs = new()` and `_functions = new()`. When the child called `HoistDeclarations()` on the function body, it found no declarations (the body only contains `IF`, `RETURN`, etc.). Recursive calls then failed the `TryGetValue` lookup and returned a wrapped `EvalFailure`, which the parent silently discarded.

**Fix:** Pass the parent's dictionaries directly to the child evaluator:

```csharp
new Evaluator(new Program(func.Body), localSymbols, _subs, _functions).Evaluate()
```

`Dictionary<K,V>` is a reference type — the child shares the exact same lookup tables as the parent. No chaining needed.

**Lesson:** When spawning child evaluators for function calls, always propagate shared state (declarations, built-ins) explicitly. Fresh instances start blind.

---

## Diagnostic Techniques

### When `RunHelper.Run()` produces `""` with no exception

This means either (a) parse failed silently, or (b) evaluator produced `EvalFailure` which was silently discarded. Work inward:

1. **Suspect parse first.** Add a `ParseFailure` branch and log its errors. This catches the majority of silent failures.

```csharp
if (parseResult is ParseFailure pf)
    foreach (var e in pf.Errors)
        // log e.Exception.Message, e.Line, e.Col
```

2. **If parse succeeds, instrument `EvaluatePrintStatement`.** Log what `EvaluateExpression(p.Value)` returns — is it `EvalSuccess` or `EvalFailure`?

3. **Write diagnostics to a file, not `Console.Error`.** xUnit captures both stdout and stderr from the test process. `File.AppendAllText(@"C:\Temp\diag.txt", ...)` bypasses capture entirely.
