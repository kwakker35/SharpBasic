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
