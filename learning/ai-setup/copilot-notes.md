# Copilot Notes

## The rule

**Copilot is a hint engine, not a code generator. You write the code.**

Copilot continues a pattern you established. It sees what you are typing and suggests what might come next, based on the code you have already written. That is a useful property — once you have written the first `case` of a switch expression, Copilot may correctly suggest the shape of the next one. But it can only suggest continuations of patterns. You establish the patterns. You write the first version of every new construct. You read every suggestion before accepting it.

The temptation is to open Copilot Chat or inline chat and describe what you want. Do not. If you are describing what you want Copilot to write, you are outsourcing the implementation. Close the chat. Write it yourself.

Copilot Chat and inline chat are explicitly off-limits for implementation work. They produce code you have not thought through. That code becomes a gap in your understanding that compounds across phases.

---

## Where Copilot is genuinely useful

### Token type enums

Once you have written the first few `TokenType` enum values yourself and Copilot has seen the pattern, it will suggest subsequent values correctly. Read every suggestion. Verify that it belongs to the current phase — Copilot has no knowledge of phase boundaries and will suggest Phase 7 tokens during Phase 3 if the naming pattern suggests them.

### AST node records

After you have written the first `abstract record` and one or two concrete nodes, Copilot will suggest the shape of subsequent nodes. The risk is low once you understand what each node represents. Still read before accepting — a node with the wrong fields or the wrong base type compiles silently and produces wrong behaviour.

### Switch expression arms

Write the first arm yourself. Once Copilot has seen the pattern (`NodeType n => EvaluateNodeType(n)`), it will suggest subsequent arms correctly. The first arm is always yours.

### xUnit test structure

Copilot will suggest test method skeletons — `[Fact]` attribute, method signature, `var` declarations — once it has seen the pattern. The assertion values are always yours. Never accept a Copilot-suggested assertion value without verifying it is correct.

---

## Where Copilot goes wrong

### Scope creep

Copilot has no knowledge of phase boundaries. In Phase 3, it will suggest patterns that require Phase 7 features — function calls, call frames, scoped symbol tables. In Phase 5, it will suggest error handling patterns that belong in Phase 9. Recognise these suggestions and reject them. Accepting them introduces code you do not yet understand and creates dependencies between phases that complicate debugging.

### Error handling defaults

Copilot defaults to `try/catch (Exception)` for error handling. SharpBASIC uses an explicit error propagation model (`RuntimeError`, structured `Diagnostic` types) and `ReturnException` for control flow. Copilot's defaults are wrong for this architecture and will cause serious problems in Phase 9 if accepted uncritically. A broad `catch (Exception)` will swallow `ReturnException` — recursion and early returns will silently fail. See `theory/pitfalls.md`.

### Test coverage

Copilot writes the happy path. It will suggest a test for `2 + 3 = 5` but not for `-n` (unary minus on a variable), `(a + b) * c` (grouped expressions), or `Fib(n - 1) + Fib(n - 2)` (two call expressions in the same expression). Edge cases are your responsibility. See `theory/pitfalls.md` — Lessons D1, D2, and Bug 5 — for the consequences of missing edge case tests.

---

## The CLAUDE.md file

CLAUDE.md is a file placed in the repository root that GitHub Copilot reads automatically when it has access to the project. It provides context that constrains Copilot's suggestions — telling it what the project is, what phase you are in, and what is explicitly out of scope.

Create this file at the start of the project and update it as phases complete.

### Suggested content

```markdown
# SharpBASIC
A BASIC-inspired tree-walking interpreter in C# .NET 10.

## Current phase
[Update as phases complete — e.g., "Phase 4 — Expressions and Pratt parsing"]

## Architecture
- Lexer → Parser → Evaluator pipeline
- AST nodes are abstract records
- Evaluator uses switch expressions and pattern matching
- SymbolTable maps string names to IValue, with parent chain for scoping
- TDD throughout — xUnit + FluentAssertions

## Scope
10 phases. No bytecode VM. No language-level file I/O. No compile-to-executable.
Introduce tokens and AST nodes only when the current phase requires them.

## Key constraints
- C# .NET 10 only
- No NuGet packages beyond xUnit and FluentAssertions
- TDD always — failing tests before implementation
- ReturnException is the mechanism for RETURN unwinding — do not catch it broadly
- SymbolTable.Set always writes to the current scope — never walks the parent chain on write
```

### What the CLAUDE.md file does

It does not guarantee Copilot respects every constraint. It shifts the distribution of suggestions toward the patterns you have described. You still read every suggestion. The file is a signal, not a fence.

Update the `## Current phase` line at the start of each new phase. This shifts Copilot's context and reduces suggestions from the wrong phase.

---

## The decision to make

At some point in Phases 4 or 7 you will face a moment where the correct implementation is not immediately obvious, Copilot is suggesting something plausible, and opening Chat would produce a working answer in ten seconds.

Close Chat. Read what you have. Understand what it is doing. Look at the failing test — what does it expect, what are you producing, where is the gap?

That moment is the learning. It is not a problem to be solved efficiently. It is the point of the project.
