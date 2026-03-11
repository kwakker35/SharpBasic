# SharpBASIC — Code Review Notes & Copilot Update Instructions

> Generated from architecture review session, March 2026.
> Based on direct review of Evaluator.cs and Parser.cs.
> Instruct Copilot to action these before continuing with new phases.

---

## Context

These notes come from an external architecture review of the current codebase.
The reviewer had full project context including goals, phase plan, and ambitions
for Language 2 (Grob). Action these items in order of priority before proceeding
with FOR/NEXT or Phase 7.

---

## Priority 1 — Fix Before FOR/NEXT

### 1a. SymbolTable needs a parent chain for scope support

The current SymbolTable is a single flat dictionary. This will break in Phase 7
when SUBs and FUNCTIONs require their own scope. Retrofit this now while the
codebase is small — it is much harder to do after Phase 7 is built on top of it.

**Required change:**

```csharp
public class SymbolTable(SymbolTable? parent = null)
{
    private readonly Dictionary<string, Value> _store = new();

    public Value? Get(string name) =>
        _store.TryGetValue(name, out var val) ? val : parent?.Get(name);

    public void Set(string name, Value value) => _store[name] = value;
}
```

- `Get` walks up the parent chain — inner scopes can read outer variables
- `Set` always writes to the current scope — no accidental outer mutation
- When calling a SUB or FUNCTION: `new SymbolTable(currentScope)`
- When returning: discard the child scope, parent is restored automatically

**Write tests for this before changing the implementation:**
- Variable in outer scope readable from inner scope
- Variable declared in inner scope not visible in outer scope after return
- Inner scope variable with same name as outer shadows correctly

---

### 1b. BinaryExpression evaluator will crash on string comparisons

`EvaluateBinaryExpression` calls `ToFloat` on both operands unconditionally.
This means `IF name = "Alice"` will throw at runtime when string comparison
is attempted.

**Required change — add a string branch before the numeric path:**

```csharp
// Handle string equality before attempting numeric conversion
if (leftES.Value is StringValue ls && rightES.Value is StringValue rs)
{
    bool result = expr.Operator.Type switch
    {
        TokenType.Eq    => ls.V == rs.V,
        TokenType.NotEq => ls.V != rs.V,
        _ => throw new InvalidOperationException(
            $"Operator {expr.Operator.Type} not supported for strings")
    };
    return new EvalSuccess(new BoolValue(result));
}
```

**Write tests before implementing:**
- `IF name = "Alice"` evaluates correctly
- `IF name <> "Bob"` evaluates correctly  
- `IF "hello" + " " + "world" = "hello world"` evaluates correctly

---

### 1c. String concatenation operator `&` not wired up

The `&` operator is defined in TokenType and the Lexer produces it, but
`EvaluateBinaryExpression` does not handle it. This means string concatenation
silently fails or errors.

**Required change — add to the string branch:**

```csharp
if (leftES.Value is StringValue lsv && rightES.Value is StringValue rsv
    && expr.Operator.Type is TokenType.Ampersand)
{
    return new EvalSuccess(new StringValue(lsv.V + rsv.V));
}
```

**Write tests before implementing:**
- `LET greeting = "Hello" & " " & "World"` produces `"Hello World"`
- `PRINT "Value: " & x` where x is a string variable works correctly

---

## Priority 2 — Fix Before Phase 7

### 2a. Parser does not validate THEN token in IF statement

`ParseIfStatement` advances past what should be the THEN token without checking
that it actually is THEN. Malformed input is silently consumed.

**Required change:**

```csharp
// After parsing the condition, validate THEN is present
if (Current.Type is not TokenType.Then)
{
    return new ParseStatementFailure(new ParseStatementError(
        new InvalidOperationException(
            $"Expected THEN after IF condition but got {Current.Type} at {Current.Line}:{Current.Column}"
        ),
        Current.Line, Current.Column
    ));
}
Advance(); // consume THEN — now validated
```

---

### 2b. ParseStatement switch has repeated boilerplate — extract helper

Every case in `ParseStatement` repeats the same pattern match and error handling.
This will become a maintenance burden as Phase 6 and 7 add more statement types.

**Suggested helper:**

```csharp
private void AddStatement(ParseStatementResult result, List<Statement> target)
{
    if (result is ParseStatementSuccess s)
        target.Add(s.Statement);
    else if (result is ParseStatementFailure f)
        errors.Add(new ParseError(f.Error.Exception, f.Error.Line, f.Error.Col));
}
```

Each switch case then becomes:
```csharp
case TokenType.Print:
    AddStatement(ParsePrintStatement(), statements);
    break;
```

---

## Priority 3 — Design Decisions for FOR/NEXT

### 3a. Loop variable scope — decide consciously

FOR/NEXT mutates a loop variable across iterations. Decide now whether:

- **Option A** — loop variable is a regular variable in current scope, visible
  after the loop ends with its final value (classic BASIC behaviour — recommended
  for SharpBASIC authenticity)
- **Option B** — loop variable is scoped to the loop, not visible after NEXT

Whichever is chosen, document it and write a test that explicitly verifies the
behaviour so it cannot accidentally change later.

### 3b. Negative STEP termination condition

When STEP is negative (counting down), the loop continuation condition flips:
- Positive STEP: continue while `i <= To`
- Negative STEP: continue while `i >= To`

A naive implementation using only `i <= To` will produce an infinite loop for
negative STEP. Write a test for `FOR i = 10 TO 1 STEP -1` before implementing.

---

## Phase 7 Advance Warning — Call Expression Parsing

When Phase 7 arrives, `ParsePrimary` will need to handle call expressions.
After parsing an identifier, peek at the next token — if it is `(`, parse
an argument list rather than returning a plain `IdentifierExpression`.

The scaffold to prepare now (no implementation needed yet, just awareness):

```csharp
if (Current.Type == TokenType.Identifier)
{
    var identExpr = new IdentifierExpression(Current.Value, loc);
    // Phase 7: if Peek().Type == TokenType.LParen, parse as CallExpression
    return identExpr;
}
```

---

## What Is Working Well — No Changes Needed

- `EvalResult` discriminated union pattern is correct and clean
- Error propagation through the evaluator is consistent
- Location tracking on all AST nodes is excellent — Phase 9 will be easy
- Pattern matching dispatch in `EvaluateStatement` and `EvaluateExpression`
  is idiomatic and will extend cleanly
- Block termination strategy in the parser is clear and readable
- Pratt loop implementation is correct — left associativity falls out naturally

---

*Action Priority 1 items before writing any new statement types.*
*Action Priority 2 items before starting Phase 7.*
*All changes should follow the established TDD pattern — tests before implementation.*
