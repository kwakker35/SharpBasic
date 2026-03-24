# Feature Spec: CHR$ Built-in Function

**Branch:** `feat/chr-builtin`  
**Priority:** Low-Medium — hard blocker for printing quoted speech in the game  
**Status:** ⬜ Not started  
**Estimated effort:** ~30 minutes

---

## Problem

The spec forbids escape sequences in string literals. There is no way to include a
literal double-quote character (`"`) inside a string. The game needs to print quoted
dialogue, e.g:

```
"You survived."
```

Without `CHR$`, this is impossible.

---

## Syntax

```basic
CHR$(n AS INTEGER) AS STRING
```

Returns the Unicode character with code point `n` as a single-character string.

```basic
PRINT CHR$(34)                            ' → "
PRINT CHR$(34) & "You survived." & CHR$(34)  ' → "You survived."
PRINT CHR$(72) & CHR$(101) & CHR$(108)   ' → Hel
```

---

## Components Affected

| Layer | File(s) | Change |
|-------|---------|--------|
| Evaluator | `Evaluator.cs` `CreateBuiltins()` | Add one entry: `["CHR$"]` |

No changes to Lexer, Parser, or AST. The `$` suffix in identifier names is already
handled by the Lexer (see `MID$`, `LEFT$`, `STR$` etc.).

---

## Implementation (single dict entry — sketch only)

```csharp
["CHR$"] = args =>
{
    var n = ((IntValue)args[0]).Value;
    return new StringValue(((char)n).ToString());
}
```

Follow the same argument type validation pattern used by `LEN`, `MID$` etc.

---

## Edge Cases

| Input | Expected |
|-------|----------|
| `CHR$(34)` | `"` (double-quote) |
| `CHR$(39)` | `'` (single-quote) |
| `CHR$(72)` | `H` |
| `CHR$(0)` | null char — acceptable, no special handling needed |
| `CHR$(-1)` | Runtime error or exception — follow existing built-in error pattern |

Game only needs printable ASCII (32–126) but full Unicode range is fine to support.

---

## Tests Required

1. `Chr_ReturnsCorrectCharacter_ForPrintableAscii` — e.g. `CHR$(72)` = `"H"`
2. `Chr_ReturnsDoubleQuote_ForCode34` — the critical game use case
3. `Chr_ReturnsCorrectCharacter_ForCode39` — single-quote
4. `Chr_ReturnsError_ForNegativeInput` — or however existing built-ins handle bad args

---

## TDD Cycle Reminder

1. Write failing test → Red
2. Add single dict entry → Green
3. No refactor needed — it's one line
4. Merge to `main`
