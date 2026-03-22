# Phase 7 — Subroutines & Functions

> Read `theory/call-frames.md` before starting this phase.  
> The SymbolTable parent chain must be in place before Phase 7 begins.

## Goal

Define and call `SUB` and `FUNCTION`. Parameters work. Recursion works.

## Honest difficulty

Hard. This is the second significant difficulty spike. Managing scope correctly — parameters in a local scope, recursive calls sharing declaration tables, return values plumbing back to the caller — involves several moving parts that interact. The test suite written here will expose every assumption about scoping.

## What you'll build

- `SubDeclaration(Name, Parameters, Body)` AST node
- `FunctionDeclaration(Name, Parameters, ReturnType, Body)` AST node
- `CallStatement(Name, Arguments)` AST node
- `CallExpression(Name, Arguments)` AST node
- `ReturnStatement(Value?)` AST node
- `Parameter(Name, Type)` record
- Declaration hoisting — all `SUB` and `FUNCTION` declarations registered before any statement executes
- Call mechanics — new `SymbolTable` scope per call, parameters defined as locals
- Child evaluator receiving the parent's declaration dictionaries
- `ReturnException` (or equivalent) for unwinding the call stack on `RETURN`

## Key concepts

**Hoisting.** All `SUB` and `FUNCTION` declarations are registered before any top-level statement executes. This allows a function to be called before it is declared in the source file. Implement `HoistDeclarations()` as a separate pass over the program.

**Parameters become the first locals.** When a SUB or FUNCTION is called, a new `SymbolTable` is created with the caller's scope as parent. Parameters are defined in this new scope before the body executes. Any `LET` inside the body writes to this local scope only.

**Child evaluator must receive the parent's declaration tables.** This is the most common serious mistake in Phase 7. A child evaluator with empty `_subs` and `_functions` dictionaries cannot find any declared functions — including itself for recursion. Pass the parent's dictionaries by reference. See `theory/pitfalls.md` — Bug 6.

**`RETURN` uses an exception for control flow.** The return statement must unwind the current call frame immediately, regardless of how deeply nested in loops or conditionals it is. In a tree-walking interpreter, throwing a `ReturnException` (caught by the call site in the evaluator) is the standard and correct approach. This is not abuse of exceptions — this is modelling language semantics in the implementation. See `decisions/architecture-decisions.md` — Decision 5.

**`CallExpression` in expression position.** `LET result = Add(3, 4)` requires a `CallExpression` that produces a value. This is distinct from `CALL Add(3, 4)` at statement level. Both must be implemented. See `theory/pitfalls.md` — Bug 4 and Bug 5.

**Sub and function names are case-sensitive.** `CALL Greet()` and `CALL greet()` are different subs. Declare and call with consistent casing. See `docs/language-spec-v1.md` §8.

## New tokens

None — `Sub`, `Function`, `Return`, `As`, `Call`, `End` were defined in Phase 1.

## New AST nodes

```csharp
public record Parameter(string Name, string TypeName);

public record SubDeclaration(
    string Name,
    IReadOnlyList<Parameter> Parameters,
    IReadOnlyList<Statement> Body) : Statement;

public record FunctionDeclaration(
    string Name,
    IReadOnlyList<Parameter> Parameters,
    string ReturnType,
    IReadOnlyList<Statement> Body) : Statement;

public record CallStatement(
    string Name,
    IReadOnlyList<Expression> Arguments) : Statement;

public record CallExpression(
    string Name,
    IReadOnlyList<Expression> Arguments) : Expression;

public record ReturnStatement(Expression? Value) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_SubWithParameter_PrintsCorrectly()
{
    var code = "SUB SayHello(who AS STRING)\n  PRINT \"Hello, \" & who\nEND SUB\nCALL SayHello(\"Alice\")";
    Run(code).Should().Be("Hello, Alice");
}

[Fact]
public void Evaluate_FunctionReturnsValue()
{
    var code = "FUNCTION Add(a AS INTEGER, b AS INTEGER) AS INTEGER\n  RETURN a + b\nEND FUNCTION\nPRINT Add(3, 4)";
    Run(code).Should().Be("7");
}

[Fact]
public void Evaluate_RecursiveFibonacci_ReturnsCorrectValue()
{
    var code = """
        FUNCTION Fib(n AS INTEGER) AS INTEGER
            IF n <= 1 THEN
                RETURN n
            END IF
            RETURN Fib(n - 1) + Fib(n - 2)
        END FUNCTION
        PRINT Fib(10)
        """;
    Run(code).Should().Be("55");
}

[Fact]
public void Evaluate_FunctionDoesNotMutateCallerScope()
{
    var code = "LET a = 99\nSUB Mutate(a AS INTEGER)\n  LET a = 0\nEND SUB\nCALL Mutate(1)\nPRINT a";
    Run(code).Should().Be("99");
}

[Fact]
public void Evaluate_FunctionDeclaredAfterCall_HoistingWorks()
{
    var code = "PRINT Double(5)\nFUNCTION Double(n AS INTEGER) AS INTEGER\n  RETURN n * 2\nEND FUNCTION";
    Run(code).Should().Be("10");
}

[Fact]
public void Evaluate_SubCanReadOuterScope()
{
    var code = "LET x = 10\nSUB ShowX()\n  PRINT x\nEND SUB\nCALL ShowX()";
    Run(code).Should().Be("10");
}
```

## Gotchas

- **Child evaluator must receive the parent's declaration dictionaries.** See `theory/pitfalls.md` — Bug 6. This is the most impactful mistake in Phase 7.
- **`ParseCallExpression` in expression position.** Calls in expression position (`LET result = Add(1, 2)`) require a separate parser path from `CALL` at statement level. See `theory/pitfalls.md` — Bug 4.
- **`Advance()` after `ParsePrimary` must not fire for call expressions.** See `theory/pitfalls.md` — Bug 5.
- **`ReturnException` must propagate cleanly.** If anything in the evaluator catches exceptions broadly (`try { } catch (Exception) { }`), `ReturnException` will be silently swallowed. Check every try/catch in the evaluator for over-broad catching.
- **CRLF in raw string literals.** Multi-line raw string tests on Windows produce `\r\n`. Ensure the lexer normalises these. See `theory/pitfalls.md` — Bug 1.

## End state

```bash
dotnet test   # all function and sub tests green, including recursive Fibonacci
```

```
> FUNCTION Add(a AS INTEGER, b AS INTEGER) AS INTEGER
>   RETURN a + b
> END FUNCTION
> PRINT Add(3, 4)
7
```

## What comes next

Phase 8 — Arrays. The heavy lifting is done. Arrays are mechanical once Phase 7 is solid.
