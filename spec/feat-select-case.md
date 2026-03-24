# Feature Spec: SELECT CASE Statement

**Branch:** `feat/select-case`  
**Priority:** High — required for game location routing and command parsing  
**Status:** ⬜ Not started

---

## Problem

The game dispatches on room IDs, player commands, and menu choices using long
`IF / ELSEIF / ELSEIF / ...` chains. SharpBASIC currently has no multi-way
branch statement. `SELECT CASE` would make dispatch readable and maintainable.

---

## Syntax

```basic
SELECT CASE expr
    CASE value1
        ' statements
    CASE value2, value3
        ' statements
    CASE ELSE
        ' default statements
END SELECT
```

### Rules

- `expr` is any expression — typically an integer or string variable
- `CASE` label is one or more comma-separated values (literals or simple expressions)
- Cases are evaluated top-to-bottom; first match wins — **no fall-through**
- `CASE ELSE` is optional; if present, must be the last clause
- No `GOTO`, no `BREAK` — block ends at the next `CASE` or `END SELECT`
- Nested `SELECT CASE` is legal

---

## Components Affected

| Layer | File(s) | Change |
|-------|---------|--------|
| Lexer | `TokenType.cs` | Add `Select`, `Case` tokens |
| Lexer | `Lexer.cs` `GetTokenType` | Map `"SELECT"` and `"CASE"` |
| AST | new `SelectCaseStatement.cs` | See AST design below |
| AST | new `CaseClause.cs` | See AST design below |
| Parser | `Parser.cs` | `ParseSelectCaseStatement` |
| Evaluator | `Evaluator.cs` | `EvaluateSelectCaseStatement` |

---

## AST Design

```csharp
// CaseClause.cs
// Values is null for CASE ELSE
record CaseClause(
    IReadOnlyList<Expression>? Values,  // null = CASE ELSE
    IReadOnlyList<Statement> Body,
    SourceLocation? Location
) : AstNode(Location);

// SelectCaseStatement.cs
record SelectCaseStatement(
    Expression Subject,
    IReadOnlyList<CaseClause> Cases,
    SourceLocation? Location
) : Statement(Location);
```

`CaseClause.Values == null` signals `CASE ELSE`.

---

## Parser Notes

High-level parse loop for `ParseSelectCaseStatement`:

1. Consume `SELECT` token
2. Consume `CASE` token
3. Parse `Subject` expression (to end of line)
4. Loop:
   - Consume `CASE` token
   - If next token is `ELSE` → consume it, parse body until `END SELECT`, emit `CASE ELSE` with `Values = null`, close
   - Otherwise parse comma-separated value list
   - Parse body statements until next `CASE` or `END SELECT`
5. Consume `END SELECT` (two tokens: `End` then `Select` — or a combined token; see note below)

> **Note on `END SELECT`**: If the lexer emits `End` and `Select` as separate tokens,
> the parser reads them as a two-token compound terminator — same pattern as `END IF`
> and `END FUNCTION`. No new combined token needed.

---

## Evaluator Notes

```
1. Evaluate Subject → subjectValue
2. For each CaseClause in Cases:
   a. If Values is null (CASE ELSE) → execute body, return
   b. For each value expression in Values:
      - Evaluate → caseValue
      - If subjectValue == caseValue → execute body, return
3. No match and no CASE ELSE → no-op (fall out of SELECT CASE silently)
```

Equality uses the same `==` semantics as `BinaryExpression` evaluation.

---

## Tests Required

1. `SelectCase_MatchesFirstCase_Integer`
2. `SelectCase_MatchesCorrectCase_WhenMultipleClauses`
3. `SelectCase_MatchesCaseElse_WhenNoCaseMatches`
4. `SelectCase_DoesNotFallThrough_ToNextCase`
5. `SelectCase_ExecutesNothing_WhenNoMatchAndNoCaseElse`
6. `SelectCase_SupportsMultipleValues_InSingleCase` (e.g. `CASE 1, 2, 3`)
7. `SelectCase_WorksWithStringSubject`
8. `SelectCase_Nested_WorksCorrectly`

---

## TDD Cycle Reminder

1. Write failing test → Red
2. Minimum code to pass → Green
3. Refactor
4. Merge to `main`
