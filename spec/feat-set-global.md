# Feature Spec: SET GLOBAL

**Branch:** `feat/set-global`  
**Priority:** CRITICAL — architectural blocker for The Sunken Crown  
**Status:** ⬜ Not started

---

## Problem

`SymbolTable.Set` always writes to the current scope's `_store` unconditionally.
`LET` inside a SUB/FUNCTION creates a local variable and never mutates the caller's
scope or the global scope. This makes game state mutation from SUBs impossible.

`SymbolTable.Get` already walks the chain (reads from nearest scope that has the name).
`Set` must remain unchanged — shadowing is intentional and correct.

---

## Decision

Add `SET GLOBAL varname = expression` as a new statement.  
`LET` behaviour is unchanged. `SET GLOBAL` bypasses local scope and writes directly
to the root (global) `SymbolTable`.

**Rejected alternatives:**
- Option A (walk-up Set): rejected — breaks shadowing which is intentional
- Option B (GLOBAL declaration): rejected — Python's footgun, silent bugs if declaration forgotten
- Ruby `$` sigil: rejected — conflicts with existing `$` suffix on string built-ins (`MID$`, `CHR$` etc.)

---

## Syntax

```basic
SET GLOBAL varname = expression
```

---

## Semantics

| Scenario | Behaviour |
|----------|-----------|
| Inside SUB/FUNCTION, name exists in global scope | Writes value to root SymbolTable |
| Inside SUB/FUNCTION, name does NOT exist in global scope | Runtime error: `'varname' does not exist in global scope` |
| At top level (outside any SUB/FUNCTION) | Runtime error: `'SET GLOBAL' is not valid outside a SUB or FUNCTION` |
| `LET` inside SUB/FUNCTION | Unchanged — always creates/assigns in current local scope |

Reading (`Get`) is unchanged — still walks the chain from current scope upward.

---

## Components Affected

| Layer | File(s) | Change |
|-------|---------|--------|
| Lexer | `TokenType.cs` | Add `Set`, `Global` tokens |
| Lexer | `Lexer.cs` `GetTokenType` | Map `"SET"` → `TokenType.Set`, `"GLOBAL"` → `TokenType.Global` |
| AST | new `SetGlobalStatement.cs` | `record SetGlobalStatement(string Identifier, Expression Value, SourceLocation? Location)` |
| Parser | `Parser.cs` | Detect `Set` token → consume `Global` → parse identifier + `=` + expression |
| Evaluator | `Evaluator.cs` | `EvaluateSetGlobalStatement` — guard top-level, call `_table.Root.Set(...)` after existence check |
| SymbolTable | `SymbolTable.cs` | Add `public bool IsGlobal => parent is null;` and `public SymbolTable Root => parent?.Root ?? this;` |

---

## SymbolTable Changes (minimal)

```csharp
// Add to SymbolTable:
public bool IsGlobal => parent is null;
public SymbolTable Root => parent?.Root ?? this;
```

`Set` is NOT changed. It remains: `public void Set(string name, Value value) => _store[name] = value;`

---

## Evaluator Logic (sketch — do not implement from this)

```
EvaluateSetGlobalStatement(stmt):
  1. If _table.IsGlobal → EvalFailure: SET GLOBAL not valid outside SUB/FUNCTION
  2. var root = _table.Root
  3. If root.Get(stmt.Identifier) is null → EvalFailure: 'name' does not exist in global scope
  4. Evaluate RHS expression
  5. root.Set(stmt.Identifier, value)
  6. Return EvalSuccess(VoidValue)
```

---

## Tests Required (write these FIRST — Red before Green)

1. `SetGlobal_UpdatesGlobalVariable_WhenCalledFromSub` — basic happy path
2. `SetGlobal_UpdatesGlobalVariable_WhenCalledFromNestedSub` — A calls B, B uses SET GLOBAL
3. `SetGlobal_ReturnsError_WhenVariableNotInGlobalScope`
4. `SetGlobal_ReturnsError_WhenUsedAtTopLevel`
5. `Let_DoesNotMutateGlobal_WhenUsedInsideSub` — regression: shadowing still works

---

## TDD Cycle Reminder

1. Write failing test → Red
2. Minimum code to pass → Green
3. Refactor
4. Merge to `main`
