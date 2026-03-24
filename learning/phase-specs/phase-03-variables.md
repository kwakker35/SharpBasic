# Phase 3 — Variables & Assignment

## Goal

`LET name = "Alice"` followed by `PRINT name` outputs `Alice`.

## Honest difficulty

Moderate. The symbol table is straightforward. The important work is establishing the scope rules and parent chain correctly — they are load-bearing in Phase 7.

## What you'll build

- `LetStatement` AST node
- `IdentifierExpression` AST node
- `IntegerLiteralExpression` and `FloatLiteralExpression` nodes
- `BooleanLiteralExpression` node
- `SymbolTable` — maps variable names to values, with a parent chain for scoping
- `IValue` interface (or equivalent) and concrete value types: `StringValue`, `IntValue`, `FloatValue`, `BoolValue`
- Evaluator extended to handle `LetStatement` and `IdentifierExpression`

## Key concepts

**The symbol table is a dictionary of names to values, with a parent.** `Get(name)` walks up the parent chain. `Set(name, value)` always writes to the current scope. This asymmetry — read up, write local — is the entire scoping model. Introduce the `Parent` pointer now even though it is only exercised in Phase 7. The cost is one nullable field. See `decisions/architecture-decisions.md` — Decision 6.

**Values are typed.** SharpBASIC has no implicit type coercion. `IntValue`, `FloatValue`, `StringValue`, and `BoolValue` are distinct types. Mixed arithmetic between `Integer` and `Float` promotes to `Float`. Mixed operations between other types are runtime errors.

**`LET` is mandatory for assignment.** There is no bare assignment (`x = 5`). The `LET` keyword is required. The equals sign in `LET x = 5` is not a comparison — the parser must distinguish this from the comparison operator `=` used in conditions.

**Variables do not need to be declared before use.** The first `LET` for a name creates it. Re-assigning with `LET` replaces the value. There is no type enforcement on re-assignment.

**v1 additions built on top of Phase 3.** Two features arrived in the pre-release feature set that extend the variable system:
- `CONST name = literal` — a global, immutable binding. The constant is stored in the symbol table with a write-protect flag. Re-assigning with `LET` is a runtime error. Constants are global only and accept only literal values.
- `SET GLOBAL name = expression` — the only way to write to the global scope from inside a SUB or FUNCTION. Complements the Phase 3 scope rule ("LET always writes local") by making global mutations explicit and greppable.

## New tokens

None — all tokens were defined in Phase 1.

## New AST nodes

```csharp
public record LetStatement(string Name, Expression Value) : Statement;
public record IdentifierExpression(string Name) : Expression;
public record IntegerLiteralExpression(int Value) : Expression;
public record FloatLiteralExpression(double Value) : Expression;
public record BooleanLiteralExpression(bool Value) : Expression;
```

## Test examples

```csharp
[Fact]
public void Evaluate_LetThenPrint_OutputsValue()
{
    Run("LET x = \"World\"\nPRINT x").Should().Be("World");
}

[Fact]
public void Evaluate_LetInteger_StoresAndReturns()
{
    Run("LET n = 42\nPRINT n").Should().Be("42");
}

[Fact]
public void Evaluate_LetReassign_UpdatesValue()
{
    Run("LET x = 1\nLET x = 2\nPRINT x").Should().Be("2");
}

[Fact]
public void Evaluate_UndefinedVariable_ReportsError()
{
    var act = () => Run("PRINT unknown");
    act.Should().Throw<RuntimeError>()
        .WithMessage("*unknown*");
}

[Fact]
public void Evaluate_BooleanLiteral_PrintsCorrectly()
{
    Run("LET flag = TRUE\nPRINT flag").Should().Be("True");
}
```

## Gotchas

- **Introduce the parent chain now.** Even though Phase 3 only uses a flat global scope, the `SymbolTable` constructor should accept a nullable `parent` parameter from the start. Testing `Get` with an explicit parent scope in Phase 3 validates the chain before Phase 7 depends on it.
- **`Set` always writes locally.** Confirm this explicitly in a test: set a variable in an outer scope, create a child scope, call `Set` with the same name, verify the outer scope is unchanged.
- **The `=` token is used for both assignment and equality.** In `LET x = 5`, the `=` is assignment. In `IF x = 5 THEN`, it is equality comparison. The parser disambiguates by context — after `LET <identifier>`, the `=` is assignment; in expression position, it is a comparison operator.

## End state

```bash
dotnet test   # all variable tests green
```

```
> LET name = "Alice"
> PRINT name
Alice
> LET x = 10
> PRINT x
10
```

## What comes next

Phase 4 — arithmetic, operator precedence, and Pratt parsing. Read `theory/pratt-parsing.md` before starting.
