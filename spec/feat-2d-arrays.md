# Feature Spec: 2D Arrays

**Branch:** `feat/2d-arrays`  
**Priority:** Medium — needed for game map storage and 2D grid logic  
**Status:** ⬜ Not started

---

## Problem

The game needs a 2D map grid. 1D arrays alone require manual index arithmetic
(`rooms(row * COLS + col)`). Native 2D array syntax is cleaner and prevents bugs.

```basic
DIM map(5, 8) AS INTEGER        ' 5 rows, 8 cols
LET map(2, 3) = 42
PRINT map(2, 3)
```

---

## Syntax

```basic
' Declaration
DIM name(rows, cols) AS TypeName

' Read access
PRINT arr(r, c)

' Write access
LET arr(r, c) = value
```

- Both indices are zero-based (consistent with existing 1D arrays)
- Only rectangular arrays (no jagged)
- Type names: `INTEGER`, `FLOAT`, `STRING`, `BOOL` (same as 1D)
- No multi-dimensional arrays beyond 2D in this phase

---

## Components Affected

| Layer | File(s) | Change |
|-------|---------|--------|
| AST | new `Dim2dStatement.cs` | Declaration node |
| AST | new `Array2dAccessExpression.cs` | Read index node |
| AST | new `Array2dAssignStatement.cs` | Write index node |
| AST | `ArrayValue.cs` (existing) | Add `Cols` property (0 = 1D) |
| Parser | `Parser.cs` | Detect `DIM name(expr, expr)` — two index exprs |
| Parser | `Parser.cs` | `array(expr, expr)` in expressions → `Array2dAccessExpression` |
| Parser | `Parser.cs` | `LET name(expr, expr) = expr` → `Array2dAssignStatement` |
| Evaluator | `Evaluator.cs` | `EvaluateDim2dStatement`, `EvaluateArray2dAssign`, `EvaluateArray2dAccess` |

No lexer changes required — `(`, `,`, `)` are already tokenised.

---

## AST Design

```csharp
// Dim2dStatement.cs
record Dim2dStatement(
    string Name,
    string TypeName,
    Expression Rows,
    Expression Cols,
    SourceLocation? Location
) : Statement(Location);

// Array2dAccessExpression.cs
record Array2dAccessExpression(
    string Name,
    Expression RowIndex,
    Expression ColIndex,
    SourceLocation? Location
) : Expression(Location);

// Array2dAssignStatement.cs
record Array2dAssignStatement(
    string Name,
    Expression RowIndex,
    Expression ColIndex,
    Expression Value,
    SourceLocation? Location
) : Statement(Location);
```

---

## Storage Model

Use flat `Value[]` with **row-major stride**:

```
index = row * cols + col
```

This stores the existing `ArrayValue` record with `Cols > 0` to signal 2D.

```csharp
// Proposed addition to ArrayValue:
record ArrayValue(Value[] Items, string ElementTypeName, int Cols = 0) : Value;
```

`Cols == 0` → 1D (backward-compatible).  
`Cols >  0` → 2D; flat length = `rows * cols`.

Bounds checking: `row < 0 || row >= rows || col < 0 || col >= cols` → runtime error.
`rows = Items.Length / Cols` (derived, not stored separately).

---

## Parser Disambiguation

The parser already handles `array(expr)` for 1D. With 2D, it must detect
a comma inside the index list:

```
name ( expr )        → 1D access / assign
name ( expr , expr ) → 2D access / assign
```

At the point of parsing the first `expr`, if the next token is `,`, it's 2D.
Apply the same disambiguation in `DIM` parsing:

```
DIM name ( expr )        → DimStatement (1D)
DIM name ( expr , expr ) → Dim2dStatement (2D)
```

---

## Tests Required

1. `Dim2d_DeclaresWithCorrectSize`
2. `Array2d_AssignAndRead_AtKnownIndex`
3. `Array2d_StoresIndependentRows`
4. `Array2d_ThrowsError_OnRowOutOfBounds`
5. `Array2d_ThrowsError_OnColOutOfBounds`
6. `Array2d_WorksWithStringType`
7. `Array1d_StillWorksAfterChange` (regression — `Cols == 0` path unchanged)

---

## TDD Cycle Reminder

1. Write failing test → Red
2. Minimum code to pass → Green
3. Refactor
4. Merge to `main`
