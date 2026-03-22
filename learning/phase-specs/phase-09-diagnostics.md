# Phase 9 — Error Handling & Diagnostics

## Goal

Errors are clear, located (line and column), and structured throughout the pipeline. The parser recovers after an error and collects multiple diagnostics rather than stopping at the first.

## Honest difficulty

Moderate. The concepts are not difficult, but the work touches every stage of the pipeline. The line/column information was collected from Phase 1 — this phase surfaces it properly.

## What you'll build

- `Diagnostic` type with `Line`, `Column`, `Message`, and `Severity`
- `ParseError` and `RuntimeError` hierarchy hardened to include `Diagnostic`
- `Result<T, IReadOnlyList<Diagnostic>>` (or equivalent) return type throughout the pipeline
- Parser error recovery — continue after an error, collect multiple diagnostics
- Meaningful error messages for all existing error cases, with source location

## Key concepts

**Errors should be precise and located.** The format from `docs/language-spec-v1.md` §13 is the target:

```
[Line 3, Col 7] Error: Undefined variable 'nme'. Did you mean 'name'?
[Line 5, Col 1] Error: Type mismatch — expected INTEGER, got STRING.
[Line 8, Col 12] Error: Array index 15 out of bounds (size 10).
```

Line and column were tracked in `Token` from Phase 1. Every AST node should carry the token (or position) from which it was parsed so that runtime errors can point to source locations.

**Parser error recovery.** A production parser does not stop at the first syntax error — it attempts to recover and continue, collecting all errors so the user sees them at once. The simplest recovery strategy is synchronisation: on an error, advance tokens until you find a known "safe" position (a newline or the start of a new statement keyword) and resume parsing from there.

**Result types vs exceptions.** Phase 9 considers making the pipeline return `Result<T>` (a discriminated union of success and failure) rather than throwing. The trade-off: `Result<T>` forces callers to handle errors explicitly; exceptions propagate automatically. SharpBASIC uses exceptions for runtime errors (including `ReturnException` for control flow) and may use `Result<T>` at the parse phase boundary. The key decision is consistent application — mixing both patterns in the same stage is harder to reason about than committing to one.

**The REPL continues after an error.** When the REPL encounters a runtime error on a line, it reports the error and waits for the next line. The symbol table is preserved — variables set before the error are still accessible. The file runner exits with code `1` on any error.

## New tokens

None.

## New AST nodes

None — diagnostics are infrastructure, not language features.

## New types

```csharp
public enum DiagnosticSeverity { Error, Warning }

public record Diagnostic(int Line, int Column, string Message, DiagnosticSeverity Severity = DiagnosticSeverity.Error)
{
    public override string ToString() => $"[Line {Line}, Col {Column}] {Severity}: {Message}";
}
```

## Test examples

```csharp
[Fact]
public void Parse_UnknownToken_ReportsDiagnosticWithLocation()
{
    var result = Parse("LET x = @invalid");
    result.Should().BeOfType<ParseFailure>()
        .Which.Diagnostics.Should().ContainSingle()
        .Which.Column.Should().BeGreaterThan(0);
}

[Fact]
public void Evaluate_UndefinedVariable_ReportsLocationInError()
{
    var act = () => Run("LET x = 1\nPRINT unknown");
    act.Should().Throw<RuntimeError>()
        .Where(e => e.Line == 2);
}

[Fact]
public void Evaluate_ArrayOutOfBounds_ReportsDescriptiveError()
{
    var act = () => Run("DIM arr[5] AS INTEGER\nLET arr[10] = 1");
    act.Should().Throw<RuntimeError>()
        .WithMessage("*10*0-5*arr*");
}

[Fact]
public void Parse_MultipleErrors_AllCollected()
{
    var result = Parse("LET = 1\nLET y = @\nPRINT z");
    result.Should().BeOfType<ParseFailure>()
        .Which.Diagnostics.Should().HaveCountGreaterThan(1);
}
```

## Gotchas

- **Silent `EvalFailure` is indistinguishable from empty output.** If your evaluator returns a failure type rather than throwing, and the caller does not check the return value, the program silently produces no output. Add explicit failure handling to every call site. See `theory/pitfalls.md`.
- **Write diagnostics to a file during debugging.** xUnit captures both stdout and stderr. Use `File.AppendAllText` for diagnostic output when debugging test failures. See `theory/pitfalls.md`.
- **Parser recovery is optional but valuable.** Collecting multiple errors in one run is useful for the user. Synchronisation-based recovery (advance to the next newline or statement keyword on error) is the standard minimal approach.

## End state

```bash
dotnet test   # all diagnostic tests green
```

```
> PRINT unknown
[Line 1, Col 7] Error: Unknown Identifier: unknown
```

## What comes next

Phase 10 — Standard library, `INPUT`, and the file runner. The satisfying finish.
