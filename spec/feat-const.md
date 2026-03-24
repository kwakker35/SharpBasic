# Feature Spec: CONST Declaration

**Branch:** `feat/const-declaration`  
**Priority:** Medium — protects game integrity, prevents silent corruption  
**Status:** ⬜ Not started

---

## Problem

The game uses many named constants (direction codes, item codes, room numbers,
combat thresholds, array sizes). Currently declared with `LET`, which makes them
mutable. An accidental `LET DIR_N = 3` anywhere silently corrupts navigation.

---

## Syntax

```basic
CONST MAX_INVENTORY = 4
CONST DIR_NORTH = 1
CONST WELCOME = "Welcome to the game"
CONST PI = 3.14159
```

- `CONST` declares a name and binds it to a value at declaration time
- Type is inferred from the literal (INTEGER, FLOAT, STRING)
- Only literal values on the RHS (no expressions: `CONST X = 1 + 2` is NOT valid)
- Constants are global — no local `CONST` inside a SUB
- Attempting to reassign a constant via `LET` or `SET GLOBAL` produces a runtime error

---

## Components Affected

| Layer | File(s) | Change |
|-------|---------|--------|
| Lexer | `TokenType.cs` | Add `Const` token |
| Lexer | `Lexer.cs` `GetTokenType` | Map `"CONST"` → `TokenType.Const` |
| AST | new `ConstDeclaration.cs` | `record ConstDeclaration(string Name, Expression Value, SourceLocation? Location)` |
| Parser | `Parser.cs` | Detect `Const` token → parse identifier + `=` + literal expression |
| Evaluator | `Evaluator.cs` | `EvaluateConstDeclaration` — store in `_table` and register name in `_consts` |
| SymbolTable | `SymbolTable.cs` | Add `HashSet<string> _consts` on root; `SetConst` method; guard in `Set` |

---

## SymbolTable Changes

```csharp
// Const names live only on the root SymbolTable
private readonly HashSet<string> _consts = new(StringComparer.OrdinalIgnoreCase);

public void SetConst(string name, Value value)
{
    _consts.Add(name);
    _store[name] = value;
}

public bool IsConst(string name) =>
    _consts.Contains(name) || (parent?.IsConst(name) ?? false);
```

The evaluator checks `_table.Root.IsConst(name)` before any `Set` call for
`LET` and `SET GLOBAL` — returns error if true.

---

## Parser Note

RHS must be a literal only. The parser should reject non-literal expressions at
parse time with a clear error. Accepted: `IntLiteralExpression`, `FloatLiteralExpression`,
`StringLiteralExpression`, `BoolLiteralExpression`. Anything else → `ParseFailure`.

---

## Tests Required

1. `Const_DeclaresAndReadsCorrectly_Integer`
2. `Const_DeclaresAndReadsCorrectly_String`
3. `Const_DeclaresAndReadsCorrectly_Float`
4. `Const_ReturnsError_WhenReassignedWithLet`
5. `Const_ReturnsError_WhenReassignedWithSetGlobal`
6. `Const_ReturnsError_WhenDeclaredInsideSub` (if parser-time enforcement chosen)
7. `Const_ReturnsParseError_WhenRhsIsExpression` (e.g. `CONST X = 1 + 2`)

---

## TDD Cycle Reminder

1. Write failing test → Red
2. Minimum code to pass → Green
3. Refactor
4. Merge to `main`
