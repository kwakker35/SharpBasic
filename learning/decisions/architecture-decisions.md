# Architecture Decisions

Ten decisions that shaped SharpBASIC. For each: what was chosen, what was rejected, and the one key reason.

---

## 1. AST nodes as abstract records

**Chosen:** AST nodes are an `abstract record` hierarchy. Every node type is a positional record — immutable, pattern-matchable, with structural equality for free.

**Rejected:** Classes with virtual methods and an explicit Visitor interface (the textbook approach).

**Reason:** C# 10+ pattern matching in switch expressions makes the Visitor pattern unnecessary ceremony. `abstract record Statement` with `record PrintStatement(Expression Value) : Statement` is four tokens instead of a class, interface, and two method bodies. The evaluator dispatches on type via switch expressions — concise, readable, and impossible to forget a case with an exhaustive match.

---

## 2. Tree-walking interpreter, not bytecode VM

**Chosen:** A tree-walking evaluator that directly executes AST nodes.

**Rejected:** Compiling to bytecode and running a virtual machine.

**Reason:** The learning goal is lexing, parsing, and AST design. A tree-walker lets you get to a running language quickly and keeps every stage independently understandable. A bytecode VM adds significant complexity (instruction encoding, a separate compiler pass, register or stack management) that obscures the fundamentals at this stage. The bytecode VM is the subject of the next project.

---

## 3. Pratt parsing for expressions (Phase 4)

**Chosen:** Top-down operator precedence (Pratt parsing) for the expression parser.

**Rejected:** A recursive descent expression parser with explicit precedence levels (separate methods for addition, multiplication, etc.).

**Reason:** Pratt parsing is extensible — adding a new operator means adding one entry to a binding power table and one parse function. The recursive descent alternative requires restructuring the call hierarchy every time precedence changes. Pratt is harder to understand initially but pays for itself by Phase 5 when logical operators arrive with their own precedence.

Read `theory/pratt-parsing.md` before Phase 4.

---

## 4. End-token dispatch for compound terminators

**Chosen:** Compound terminators (`END IF`, `END SUB`, `END FUNCTION`) are parsed as two-token sequences — `END` followed by a contextual token.

**Rejected:** Single-token terminators (`ENDIF`, `ENDSUB`).

**Reason:** This matches the language surface (`END IF`, `END SUB`) exactly, and it means the parser can dispatch on the `END` token and then peek at what follows to disambiguate. The alternative — single tokens — would require adding three more reserved words to the keyword table and diverges from idiomatic BASIC syntax.

---

## 5. RuntimeError introduced early, not deferred to Phase 9

**Chosen:** `RuntimeError` (or equivalent exception type) was introduced from the evaluator's first iteration — Phase 3 — as the mechanism for propagating errors upward.

**Rejected:** Deferred error handling — return `null` or default values until Phase 9 adds proper diagnostics.

**Reason:** Silent failures are the hardest bugs to diagnose. If the evaluator returns `null` on bad input, nothing breaks visibly — the program just produces wrong output. A `RuntimeError` with a meaningful message fails loudly, which is the correct behaviour for a learning project where every unexpected result should be immediately explainable. Phase 9 improves the error surface, but the infrastructure was there from Phase 3.

---

## 6. SymbolTable parent chain — introduced in Phase 3

**Chosen:** `SymbolTable` carries a `Parent` reference from Phase 3. `Get` walks up the chain. `Set` always writes to the current scope.

**Rejected:** Adding the parent chain in Phase 7 when functions arrive.

**Reason:** Phase 7 builds directly on the parent chain — every function call creates a new `SymbolTable` with the caller's table as parent. If the chain isn't in place and tested before Phase 7, Phase 7 breaks in ways that are hard to diagnose. Introducing it in Phase 3 costs almost nothing (one nullable field, a small change to `Get`) and means Phase 7 is an extension of existing behaviour rather than a retrofit.

---

## 7. TDD throughout — tests before implementation always

**Chosen:** Every phase follows: write failing tests → implement → refactor → verify. Tests are written in the same session as the feature, before a line of implementation exists.

**Rejected:** Implementing first, writing tests to cover existing behaviour.

**Reason:** Compiler stages are among the most naturally testable code you will write. Input in, specific output out. Tests written before implementation are a design tool — they force you to state precisely what the feature should do before you commit to how. Tests written after implementation tend to mirror the implementation's assumptions rather than test them. The xUnit + FluentAssertions suite is a first-class deliverable alongside the code.

---

## 8. Token carries line and column from Phase 1

**Chosen:** `Token` is a `readonly record struct` with `Line` and `Column` fields populated by the lexer from the first iteration.

**Rejected:** Adding position tracking in Phase 9 when diagnostics are introduced.

**Reason:** Retrofitting position tracking to an existing lexer and threading it through the AST and evaluator is expensive and error-prone. The position information has to come from somewhere — and the only place that has it is the lexer, character by character. Adding it from the start costs almost nothing and means Phase 9 can focus on the diagnostic surface rather than plumbing.

---

## 9. No third-party NuGet packages beyond xUnit and FluentAssertions

**Chosen:** xUnit + FluentAssertions for tests. No other packages.

**Rejected:** Parser combinator libraries, source generator tooling, Roslyn for error reporting, any other convenience packages.

**Reason:** The learning value is in building things from first principles. A parser combinator library would abstract away exactly the part of the project that is worth understanding. Keeping the dependency surface minimal also means the project compiles and runs anywhere a .NET 10 SDK is installed — no package restore surprises, no version conflicts.

---

## 10. Fixed scope — 10 phases, no additions

**Chosen:** SharpBASIC ends at Phase 10. Every idea that arose during the build was either in scope or explicitly deferred to the next project.

**Rejected:** A rolling scope where interesting ideas get added as they arise.

**Reason:** A project without a defined end point does not end. File I/O, compile-to-executable, a VS Code extension, a bytecode VM — all of these came up during planning and all of them were deferred. The discipline of saying "that belongs to Grob, not SharpBASIC" kept the project completable. A clean finish at Phase 10 is more valuable than a richer but unfinished language.

---

## 11. CONST — global only, literal values only

**Chosen:** `CONST` is global-only and accepts only literal values — integers, floats, strings, and booleans. No expressions, no function calls.

**Rejected:** Allowing `CONST` inside a SUB/FUNCTION, or accepting arbitrary expressions (e.g. `CONST PI = 4 * ATN(1)`).

**Reason:** Constants are global names by design. Placing a `CONST` inside a local scope creates confusing shadowing and reduces the diagnostic value of "this name is immutable everywhere". Literal-only restriction means a constant is guaranteed to have no side effects and be fully knowable at a glance. The restriction is enforced at runtime with a clear error message.

---

## 12. SET GLOBAL — explicit keyword for global mutation

**Chosen:** A dedicated `SET GLOBAL name = expression` statement that explicitly writes to the global scope from inside a SUB or FUNCTION.

**Rejected:** Allowing `LET` to optionally write to an outer scope, or a reference/output-parameter mechanism.

**Reason:** The scope rule "LET always writes local" is a load-bearing invariant that makes programs easier to reason about. Silently violating it would make bugs very hard to find. `SET GLOBAL` makes the intent fully explicit and visible — you can grep the file for `SET GLOBAL` to find every global mutation. The cost is one extra keyword; the benefit is clarity.

---

## 13. SELECT CASE — first-match, no fall-through

**Chosen:** The first matching `CASE` clause executes and the block exits. No fall-through to subsequent clauses.

**Rejected:** C-style fall-through where execution continues to the next case unless `BREAK` is written explicitly.

**Reason:** Fall-through is a well-documented source of bugs. First-match semantics are the norm in modern pattern-matching languages (F# `match`, Swift `switch`). Classic BASIC never had fall-through in `ON … GOSUB` forms either — this is consistent with the language heritage and eliminates an entire class of errors.

---

## 14. 2D array syntax — `[rows][cols]` double brackets, not `(r, c)` parentheses

**Chosen:** `DIM name[rows][cols]` and element access `name[r][c]` — two separate bracket pairs.

**Rejected:** `DIM name(rows, cols)` and `name(r, c)` — parentheses with a comma-separated pair as seen in some early BASICs.

**Reason:** `name(arg1, arg2)` already parses as a `CallExpression` in the parser. At the point the parser sees `name(`, it cannot yet know whether what follows is a function call or a 2D array access. `[r][c]` is unambiguous — `[` cannot begin an argument list, so it can only mean indexing. It is also naturally consistent with the 1D `name[index]` syntax: 2D is just two sequential index operations.

---

## 15. CHR$ — the only way to embed a double-quote in a string

**Chosen:** `CHR$(34)` as the standard workaround for embedding a double-quote character, since string literals have no escape sequence support.

**Rejected:** Adding an escape sequence syntax (e.g. `\"` or doubling `""`) to string literals.

**Reason:** Escape sequences require changes in the lexer (handling `\"` inside a quoted string without treating it as the closing delimiter), in the AST (representing escape sequences), and everywhere strings are printed. `CHR$()` solves the problem by composing existing mechanisms — the built-in registry and integer-to-character conversion. Zero parser complexity added; the workaround is explicit and memorable.
