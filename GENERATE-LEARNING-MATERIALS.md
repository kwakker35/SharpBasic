# Generate Learning Materials

> Instructions for GitHub Copilot.
> Create the complete `learning/` folder structure for the SharpBASIC repository.
> Write every file exactly as specified below. Do not summarise, paraphrase,
> or interpret — write the content verbatim to each file path.

---

## Before you start — read these source files

Read all of the following before writing any files:

- `docs/language-spec-v1.md`
- `docs/lessons-learned.md`

These are referenced by the content below. Do not generate content
from them — use them only to verify that SharpBASIC syntax examples
are correct before writing each file to disk.

---

## File: `learning/README.md`

Write the following content verbatim to this path:

# SharpBASIC — Learning Materials

SharpBASIC is a complete BASIC-inspired tree-walking interpreter built in C# .NET 10 across 10 deliberate phases. It was built as a learning project, and these materials are a first-class deliverable alongside the code.

---

## What you can do with this

- **Follow the journey from scratch.** The phase specs guide you through building the same interpreter, phase by phase, using the same AI-assisted workflow. Every decision is documented. Every pitfall is recorded.
- **Fork the repo and use the AI prompts.** The `ai-setup/` folder contains the exact Claude system prompt and Copilot guidance used during the build, stripped of personal context and ready to adapt.
- **Study it as a reference.** The phase specs, architecture decisions, and theory notes form a coherent account of how a tree-walking interpreter is actually built — not how one could theoretically be built.
- **Contribute improvements.** If you find an error, a missing edge case, or a lesson worth recording, a pull request is welcome.

---

## How this folder is organised

| Folder | Contents |
|---|---|
| `ai-setup/` | Claude system prompt, how to use AI tools correctly, Copilot guidance |
| `phase-specs/` | One spec per phase — goal, concepts, test examples, gotchas, end state |
| `decisions/` | The 10 key architectural decisions with reasoning |
| `theory/` | Deeper dives into Pratt parsing, call frames, pitfalls, and reading list |

---

## Where to start

1. Read `decisions/architecture-decisions.md` to understand the shape of the project before any code exists.
2. Read `theory/pratt-parsing.md` before Phase 4. It will save you significant time.
3. Read `theory/call-frames.md` before Phase 7. The concept needs to land before the implementation begins.
4. Work through `phase-specs/phase-01-lexer.md` and follow the spec for each phase in order.
5. Set up the AI tooling using `ai-setup/claude-system-prompt.md` and `ai-setup/how-to-use-with-ai.md`.

The specs reference `theory/pitfalls.md` throughout. That document records everything that actually went wrong during the build. Read the relevant sections before each phase, not after.

---

## A note on these materials

These materials are free to adapt and use. A link back to the original repository is appreciated but not required.

The code is the authority. Where any document conflicts with the running interpreter or `docs/language-spec-v1.md`, the spec wins.

---

## File: `learning/decisions/architecture-decisions.md`

Write the following content verbatim to this path:

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

## File: `learning/theory/pratt-parsing.md`

Write the following content verbatim to this path:

# Pratt Parsing

> Read this before starting Phase 4.

---

## Why expressions are hard

By the end of Phase 3 you can parse `LET x = 42` and `PRINT x`. The parser for those is straightforward recursive descent — you know exactly what tokens to expect in what order.

Expressions break that model. `2 + 3 * 4` should produce `14`, not `20`. The parser must understand that `*` binds more tightly than `+`. And `AND` should bind more tightly than `OR`, but less tightly than `=`. And `NOT` is unary and should bind most tightly of all.

The naive solution is one parsing function per precedence level — `ParseOr` calls `ParseAnd`, which calls `ParseComparison`, which calls `ParseAddition`, which calls `ParseMultiplication`, which calls `ParseUnary`, which calls `ParsePrimary`. Seven levels of call. Adding one new operator potentially reshuffles the entire call hierarchy.

This gets unmanageable fast. There is a better way.

---

## What Pratt parsing is

Pratt parsing (top-down operator precedence) assigns every operator a number — its *binding power* — and uses a single recursive function that stops recursing when it encounters an operator with binding power lower than the current minimum.

The key terms:
- **Binding power (BP):** A number representing how tightly an operator holds its operands. `*` has higher BP than `+`.
- **Prefix position:** A token that appears at the start of an expression (`-x`, `NOT flag`, a literal, an identifier, `(`).
- **Infix position:** A token that appears between two sub-expressions (`+`, `*`, `AND`, `=`).

Every token that can appear in prefix position has a prefix parse function. Every token that can appear in infix position has an infix parse function and a binding power.

---

## The key insight

The Pratt parser loops, not recurses. It parses a left-hand side, then checks whether the next token has high enough binding power to "take" that left-hand side as its left operand. If yes, it continues. If no, it returns.

```csharp
private Expression? ParseExpression(int minBp = 0)
{
    Expression? left = ParsePrimary(); // prefix position
    if (left is null) return null;

    while (true)
    {
        var op = Current;
        int bp = BindingPower(op.Type);
        if (bp <= minBp) break;          // not strong enough to continue

        Advance();
        Expression? right = ParseExpression(bp); // infix position, higher minimum
        if (right is null) return null;

        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

That is the entire Pratt loop. `BindingPower` is a switch expression returning a number for each operator type and 0 for everything else.

---

## How it maps to the code — trace through `2 + 3 * 4`

Tokens: `IntegerLiteral(2)`, `Plus`, `IntegerLiteral(3)`, `Star`, `IntegerLiteral(4)`, `Eof`

1. `ParseExpression(minBp: 0)` is called.
2. `ParsePrimary()` returns `IntegerLiteral(2)`. Current is now `Plus`.
3. `BindingPower(Plus)` returns `10`. `10 > 0` → continue.
4. Advance past `Plus`. Call `ParseExpression(minBp: 10)` for the right side.
   - `ParsePrimary()` returns `IntegerLiteral(3)`. Current is now `Star`.
   - `BindingPower(Star)` returns `20`. `20 > 10` → continue.
   - Advance past `Star`. Call `ParseExpression(minBp: 20)` for the right side.
     - `ParsePrimary()` returns `IntegerLiteral(4)`. Current is now `Eof`.
     - `BindingPower(Eof)` returns `0`. `0 <= 20` → break. Return `IntegerLiteral(4)`.
   - `left` becomes `BinaryExpression(3, *, 4)`. Current is `Eof`.
   - `BindingPower(Eof)` returns `0`. `0 <= 10` → break. Return `BinaryExpression(3, *, 4)`.
5. `left` becomes `BinaryExpression(2, +, BinaryExpression(3, *, 4))`.
6. `BindingPower(Eof)` returns `0`. `0 <= 0` → break. Return the expression.

Result: `2 + (3 * 4)` — correct.

---

## SharpBASIC binding powers

```csharp
private static int BindingPower(TokenType type) => type switch
{
    TokenType.Or                 => 2,
    TokenType.And                => 3,
    TokenType.Equals
    | TokenType.NotEquals
    | TokenType.LessThan
    | TokenType.GreaterThan
    | TokenType.LessThanOrEqual
    | TokenType.GreaterThanOrEqual => 5,
    TokenType.Plus
    | TokenType.Minus
    | TokenType.Ampersand        => 10,
    TokenType.Star
    | TokenType.Slash
    | TokenType.Mod              => 20,
    _                            => 0
};
```

`&` (string concatenation) has the same binding power as `+` and `-`. That means `"x=" & 1 + 2` is `"x=" & 3` → `"x=3"`, not `"x=12"`. This is correct — `+` binds tighter. Be aware of it when constructing output strings.

---

## Prefix position — unary operators and primary expressions

`ParsePrimary()` handles tokens in prefix position. It needs to handle:
- Integer literals, float literals, string literals, boolean literals
- Identifiers (variables, function calls)
- Parenthesised expressions: `(` → parse inner expression → expect `)`
- Unary minus: `-` → parse inner expression with high minimum BP
- `NOT`: unary boolean negation

The unary minus case is a common source of confusion. `ParsePrimary` sees `-`, advances, and calls `ParseExpression(minBp: 15)` — a minimum high enough that only multiplication-class operators bind tighter. The result is wrapped in a `UnaryExpression`.

**Gotcha:** If unary minus only handles literal tokens (`-5`, `-3.14`) rather than arbitrary expressions, negating a variable (`-n`) or a subexpression (`-(a + b)`) will silently fail. Implement the full prefix case from the start. See `theory/pitfalls.md`.

---

## Common mistakes on a first implementation

**Not advancing after `ParsePrimary` returns for non-call expressions.** The convention is that `ParsePrimary` returns with `Current` pointing at the token it parsed. For a literal, that means `Current` is still the literal token — you must advance past it before entering the Pratt loop. For a call expression that consumed everything through `)`, you must not advance again. Track which cases self-advance and which do not. See `theory/pitfalls.md` — Bug 5 in Phase 7 is exactly this failure mode, present from Phase 4.

**Returning 0 from `BindingPower` for operators you haven't implemented yet.** A missing entry doesn't cause an error — the loop just stops. You end up with an expression that silently ignores everything to the right. Always verify new operators with tests that confirm the full expression evaluates correctly.

**Forgetting that `NOT` is prefix, not infix.** It has no binding power in the infix table. It belongs in `ParsePrimary` as a prefix rule.

---

## Further reading

- *Crafting Interpreters* — Robert Nystrom, Chapter 6. The definitive treatment. Nystrom's version uses the terms "prefix" and "infix parselets" but the algorithm is identical.
- "Pratt Parsers: Expression Parsing Made Easy" — Bob Nystrom's blog post (journal.stuffwithstuff.com). Shorter than the book chapter and directly applicable.

---

## File: `learning/theory/call-frames.md`

Write the following content verbatim to this path:

# Call Frames and the Call Stack

> Read this before starting Phase 7.

---

## What the problem is — why a flat symbol table breaks

After Phase 3, variables live in a `SymbolTable` — a dictionary of name-to-value pairs. That works for global variables. It breaks the moment you add functions.

Consider:

```
FUNCTION Add(a AS INTEGER, b AS INTEGER) AS INTEGER
    RETURN a + b
END FUNCTION

LET a = 99
PRINT Add(3, 4)
PRINT a
```

If `a` and `b` are written directly into the global symbol table, the call to `Add(3, 4)` overwrites the global `a`. After the call, `PRINT a` outputs `3`, not `99`.

The fix is a separate scope for each function call — a scope that ceases to exist when the call returns, and that does not interfere with variables in the caller.

---

## What a call frame is

A call frame is the data that exists for the duration of one function call:
- Which function is executing
- The local variables and parameters for this call
- Where to return to when the call finishes

In SharpBASIC, a call frame is represented by a `SymbolTable` instance created at call time, with the caller's `SymbolTable` as its parent.

```csharp
// When calling a function:
var localScope = new SymbolTable(parent: callerScope);
// Define parameters in localScope
localScope.Define("a", new IntValue(3));
localScope.Define("b", new IntValue(4));
// Execute the function body against localScope
var childEvaluator = new Evaluator(functionBody, localScope, _subs, _functions);
```

When the call returns, `localScope` goes out of scope. The caller's scope is unchanged.

---

## The call stack as a concrete data structure

The call stack is the chain of active call frames — one frame per function call that has not yet returned.

In SharpBASIC's tree-walking interpreter, the call stack is implicit: the C# call stack tracks active recursive calls to `Evaluate`, and each call frame corresponds to one stack frame of the C# runtime.

This is one of the meaningful differences between a tree-walking interpreter and a bytecode VM. In a VM, the call stack is explicit — an array of `CallFrame` structs managed by the VM itself. In a tree-walker, C# does the work for you.

What this means in practice: the recursion depth of your SharpBASIC programs is bounded by the C# stack depth. For the interpreter's purposes, this is acceptable. It is one of the trade-offs accepted in `decisions/architecture-decisions.md`.

---

## How local variables work — scoped to a frame, not global names

Parameters become the first local variables of a call frame. Any `LET` statement inside the function writes to the local `SymbolTable`, not to the caller's.

```csharp
// SymbolTable.Set always writes to the current scope
public void Set(string name, IValue value)
{
    _values[name] = value; // always local
}

// SymbolTable.Get walks up the parent chain
public IValue? Get(string name)
{
    if (_values.TryGetValue(name, out var value)) return value;
    return Parent?.Get(name);
}
```

This means:
- A function can **read** a variable from the caller's scope (the parent chain allows it).
- A function **cannot write to** a variable in the caller's scope — `Set` always writes locally.

This is the expected and correct behaviour. See `docs/language-spec-v1.md` §4.2 for the formal statement.

---

## What a stack trace actually is

A stack trace is a printout of every active call frame, from the innermost (most recent) outward. Each line is: the name of the function and where in it execution is currently paused.

When you see a stack trace from a C# exception, it is the C# runtime printing its own call stack. When SharpBASIC throws a `RuntimeError` inside a deeply recursive call, the C# stack trace includes your evaluator frames — which is one reason the tree-walking approach makes Phase 9 diagnostics simpler to implement.

---

## Scope and the parent chain — why SymbolTable needs a Parent pointer

The parent chain was introduced in Phase 3. By Phase 7, it is load-bearing.

When a function is called:
1. Create a new `SymbolTable` with `parent = callerScope`.
2. Define parameters in the new scope.
3. Execute the function body in the new scope.
4. When `RETURN` is encountered, unwind back to the caller.

Without the parent pointer, the function body cannot read global variables or caller-scope variables. With it, the function body has full read access to everything in any enclosing scope.

The asymmetry — read access up the chain, write access only to the current scope — is deliberate. It prevents functions from mutating caller state, which is almost always the wrong behaviour.

---

## Common mistakes

**Passing fresh, empty declaration dictionaries to the child evaluator.** This is the exact bug that caused Phase 7's most serious failure (see `theory/pitfalls.md` — Bug 6). When spawning a child evaluator for a function call, pass the parent's `_subs` and `_functions` dictionaries by reference. A fresh empty dictionary means recursive calls — and any call to a function defined at the top level — will fail silently.

```csharp
// Wrong — child evaluator cannot see any declared functions or subs
new Evaluator(body, localScope)

// Correct — child shares the same declaration tables
new Evaluator(body, localScope, _subs, _functions)
```

**Calling Set on the wrong scope.** If `Set` walks the parent chain and writes to the first scope that contains the name (like Python's `nonlocal`), functions mutate caller variables. SharpBASIC's `Set` always writes to the current scope — confirm this explicitly in your implementation.

**Not hoisting declarations before execution.** All `SUB` and `FUNCTION` declarations must be registered before any top-level statement executes. Otherwise, a function declared after the call that invokes it will not be found. Implement `HoistDeclarations()` as a separate pass over the program before `Evaluate` runs.

**Parent chain depth and scope leaks.** If the parent chain is set incorrectly — for example, pointing to a scope that was already discarded — reads from that scope will either fail silently or return stale values. Test recursive functions (Fibonacci is the canonical case) and verify that each recursive call sees its own parameters, not those of an earlier call.

---

## File: `learning/theory/reading-list.md`

Write the following content verbatim to this path:

# Reading List

Ordered by when to read it, not by importance. All of these are worth reading. The primary reference is non-negotiable.

---

## Primary reference

**Crafting Interpreters — Robert Nystrom**

The book that this project orbits. Part I and Part II (jlox) cover a tree-walking interpreter for a language called Lox — structurally similar to SharpBASIC but in Java. Part III (clox) implements a bytecode VM in C.

Chapters 1–4 give the big picture. Reading them before Phase 1 is worth the time even if you don't read another word until Phase 4.

The physical book is the recommended format — the diagrams reproduce well in print and it is the kind of book you will return to. The full text is available free online at craftinginterpreters.com.

---

## Phases 1–3

**"Let's Build a Simple Interpreter" — Ruslan Spivak**  
ruslanspivak.com — a step-by-step series in Python. The concepts translate directly to C#. Good supplementary reading for the lexer and early parser phases if the book's approach doesn't immediately click.

**The Super Tiny Compiler — James Kyle**  
github.com/jamiebuilds/the-super-tiny-compiler  
A minimal compiler in ~200 lines of JavaScript. Useful for seeing the full pipeline in a single file, which helps when the pipeline is spread across five C# projects.

**Computerphile — Parsing videos**  
YouTube. Search "Computerphile compiler" or "Computerphile parsing". Good visual intuition for what the parser is doing before the code makes it concrete.

---

## Phase 4 — Pratt parsing

**Crafting Interpreters — Chapter 6**  
Nystrom's treatment of Pratt parsing. Read `theory/pratt-parsing.md` first, then Chapter 6 for the full account.

**"Pratt Parsers: Expression Parsing Made Easy" — Bob Nystrom**  
journal.stuffwithstuff.com  
A shorter, standalone article by the same author. More directly applicable than the book chapter for a first implementation. Recommended.

---

## General compiler theory

**Roslyn source — github.com/dotnet/roslyn**  
The C# compiler itself, written in C#. Too large to study directly, but useful for error handling and AST design inspiration in the later phases. The diagnostic model in particular is worth looking at before Phase 9.

---

## What comes after SharpBASIC

The natural next step is Part III of *Crafting Interpreters* — implementing clox, a bytecode VM for Lox, in C. This is a different discipline from the tree-walking interpreter: manual memory management, an explicit value stack, call frames as data structures, and a garbage collector.

clox is worth implementing fully — not reading and skimming, but typing every line. The concepts it builds (stack-based VM, bytecode compilation, closures, GC) are the foundation of the Grob project. Working through clox before designing Grob means every key architectural decision in Grob's VM will be informed by having built one already.

After clox, if systems programming appeals: a bare-metal x86 kernel in C and NASM assembly is a coherent next project. The mental models — stack, instruction pointer, memory addresses — transfer directly from the VM work.

---

## File: `learning/theory/pitfalls.md`

Write the following content verbatim to this path:

# Pitfalls and Lessons Learned

> Every entry here cost real time during the build.  
> Knowing it in advance won't remove the struggle —  
> but it will stop you hitting the same walls twice.

---

## Lexer and tokenisation

### CRLF line endings silently break tests written with raw string literals

**What goes wrong:** Tests using C# raw string literals (`"""..."""`) fail to parse. All tokens appear on line 1; newlines tokenise as `Unknown`.

**Why:** C# raw string literals on Windows produce `\r\n` (CRLF). If your lexer only handles `\n`, the `\r` falls through to the `default` case, gets buffered into the current token, then when `\n` arrives it flushes an `Unknown` token containing the carriage return. Tests using explicit `"\n"` strings pass silently because they have no `\r`.

**Fix:** Normalise line endings in the `Lexer` constructor before any tokenising:

```csharp
_source = source.Replace("\r\n", "\n").Replace("\r", "\n");
```

Do this at the system boundary — the constructor — before any tokenising happens.

**Cross-reference:** `phase-specs/phase-01-lexer.md`

---

### Operator cases must flush the token buffer

**What goes wrong:** An identifier immediately before a punctuation character is silently dropped. `Greet(` tokenises as just `LParen` — the `Greet` identifier disappears.

**Why:** The space and string-literal cases have a buffer-flush guard (`if (token.Length > 0)`) before emitting their token. Operator and punctuation cases commonly do not. When the lexer hits `(` mid-word, it emits `LParen` and discards whatever was buffered.

**Fix:** Every character that terminates a word must flush the pending buffer before emitting its own token. This applies to every operator and punctuation case, not just the ones you discover are broken.

**Cross-reference:** `phase-specs/phase-01-lexer.md`

---

### `GetStringLiteral` advancing one character too many

**What goes wrong:** The character immediately after a closing `"` is skipped. `)` in `Greet("Alice")` is dropped, causing a parse failure.

**Why:** `GetStringLiteral` calls `Advance()` to step past the closing `"`. The outer loop then calls `Advance()` again at the top of the next iteration, skipping whatever followed the `"`.

**Fix:** Remove the final `Advance()` from inside the string literal helper. The outer loop's advance at the start of the next iteration lands on the correct next character.

**Cross-reference:** `phase-specs/phase-01-lexer.md`

---

### Use `ToUpperInvariant()` not `ToUpper()` for keyword and builtin comparisons

**What goes wrong:** On a machine configured with a Turkish locale, `"len".ToUpper()` returns `"LEN"` on most systems but `"İEN"` (with a dotted capital I, U+0130) on Turkish systems. Built-in function lookup and keyword recognition fail silently on those machines.

**Why:** `String.ToUpper()` is culture-sensitive. The Turkish locale has different case rules for the letter I.

**Fix:** Use `ToUpperInvariant()` and `ToLowerInvariant()` everywhere you compare against fixed string constants (keywords, built-in names, type names). These methods always use invariant culture rules regardless of locale.

```csharp
// For display — culture-sensitive is fine
Console.WriteLine(value.ToString());

// For comparison — always use invariant
_builtins.TryGetValue(expr.Name.ToUpperInvariant(), ...)
stmt.TypeName.ToUpperInvariant() switch { "INTEGER" => ... }
```

The rule of thumb: `ToUpper()` for display, `ToUpperInvariant()` for comparison.

**Cross-reference:** `phase-specs/phase-10-stdlib.md`

---

## Parsing and AST

### `ParseCallExpression` must be called from `ParsePrimary` for expression-position calls

**What goes wrong:** `LET result = Add(1, 2)` — `Add` parses as an `IdentifierExpression` and `(1, 2)` is dangling garbage. Function calls only work at statement level (`CALL Foo(...)`), not in expressions.

**Why:** `ParseCallStatement` handles `CALL Foo(...)` as a statement. Without a corresponding `ParseCallExpression`, identifiers followed by `(` in expression position are not recognised as calls.

**Fix:** In `ParsePrimary`, after recognising an `Identifier` token, peek at the next token. If it is `LParen`, dispatch to `ParseCallExpression` rather than returning a bare `IdentifierExpression`.

```csharp
if (Current.Type == TokenType.Identifier)
{
    if (Peek().Type == TokenType.LParen)
        return ParseCallExpression();

    return new IdentifierExpression(Current.Value, loc);
}
```

**Cross-reference:** `phase-specs/phase-07-functions.md`

---

### `Advance()` after `ParsePrimary` must not fire for call expressions

**What goes wrong:** `Fib(n - 1) + Fib(n - 2)` — the `+` is silently consumed. The expression returns only `Fib(n - 1)`. Single-call tests pass because the extra advance consumes a harmless newline.

**Why:** `ParseExpression` calls `Advance()` after `ParsePrimary()`. For literals and identifiers this is correct — they leave `Current` pointing at the token they represent, and you must advance past it. But `ParseCallExpression` already consumed everything through `)`. The extra advance then steps over the following operator.

**Fix:** Advance only when `ParsePrimary` returned something other than a call expression:

```csharp
left = ParsePrimary();
if (left is null) return null;
if (left is not CallExpression)
    Advance();
```

This is a subtle asymmetry that does not surface until you have multiple call expressions in the same expression.

**Cross-reference:** `phase-specs/phase-07-functions.md`

---

## Expressions and operator precedence

### `AND` / `OR` / `NOT` belong in Phase 5, not Phase 7

**What goes wrong:** The first realistic `IF` statement you write — `IF x > 0 AND x < 10 THEN` — does not work. `AND` is either not lexed, not parsed, or not evaluated.

**Why:** It is tempting to defer `AND`, `OR`, and `NOT` until Phase 7 when the token enum is expanding anyway. But by Phase 5, `IF` without logical operators is artificially limited to single conditions. Any real conditional needs them.

**Fix:** Add `AND`, `OR`, and `NOT` in Phase 5 alongside `IF`. They are two more rows in `BindingPower()` (the Pratt loop handles them for free) and one prefix rule for `NOT`. The work is small. The payoff — being able to write realistic conditionals immediately — is large.

**Cross-reference:** `phase-specs/phase-05-conditionals.md`

---

### Unary minus must handle expressions, not just literals

**What goes wrong:** `-5` and `-3.14` work. `-n` and `-(a + b)` silently fail. The parser returns `null` and the expression produces no output or an error.

**Why:** It is easy to implement unary minus only for the literal case — the test `LET x = -5` passes and the phase feels complete. But the prefix unary minus case must fall through to `ParsePrimary` and handle any expression, not just literals.

**Fix:** In the `ParsePrimary` unary minus branch, call `ParseExpression` with a sufficiently high minimum binding power, not a literal-specific parse:

```csharp
case TokenType.Minus:
    Advance();
    var operand = ParseExpression(minBp: 15); // binds tighter than + or -
    return new UnaryExpression(UnaryOp.Negate, operand);
```

Add a test for `-n` and `-(a + b)` to the Phase 4 test suite to catch this before moving on.

**Cross-reference:** `phase-specs/phase-04-expressions.md`

---

### `&` has the same binding power as `+` and `-`

**What this means:** `"x=" & 1 + 2` evaluates as `"x=" & 3` → `"x=3"`, not `"x=12"`. The `+` binds tighter than `&`.

**Why this matters:** When constructing output strings, unexpected precedence produces wrong output with no error. `PRINT "Total: " & a + b` adds `a` to `b` first, then concatenates — which is usually wrong.

**Fix:** Use parentheses when the intent could be ambiguous: `PRINT "Total: " & (a + b)` and `PRINT "Total: " & a & " and " & b`.

**Cross-reference:** `phase-specs/phase-04-expressions.md`, `docs/language-spec-v1.md` §6.6

---

## Control flow

### `FOR` loop counter is stored as `IntValue` even when step is a float

**What goes wrong:** A `FOR` loop with a float step value produces confusing counter values when read back as an integer.

**Why:** The loop counter is stored as `IntValue((int)i)` every iteration even when the internal accumulator is a `double`. Sub-integer step values are silently truncated in the symbol table.

**Fix:** Use integer values for `STEP` in `FOR` loops. If sub-integer stepping is needed, use a `WHILE` loop with a float accumulator variable.

**Cross-reference:** `phase-specs/phase-06-loops.md`, `docs/language-spec-v1.md` §7.2

---

### `REM` recognition requires a token boundary after the word

**What goes wrong:** A source file ending with the bare text `REM` and no trailing newline or space causes the lexer to flush `REM` as an `Identifier`. The parser reports an error.

**Fix:** Always end source files with a trailing newline. The lexer requires a space, newline, or another token boundary after `REM` to recognise it as a comment.

**Cross-reference:** `phase-specs/phase-01-lexer.md`, `docs/language-spec-v1.md` §2.4

---

## Functions and scope

### Child evaluators must receive the parent's declaration dictionaries

**What goes wrong:** Recursive calls to a function silently return failure. `Fib(n - 1)` inside `Fib` fails to find `Fib`. Non-recursive calls to other top-level functions from inside a function also fail.

**Why:** Creating a child evaluator with `new Evaluator(body, localScope)` gives it fresh, empty `_subs` and `_functions` dictionaries. When the child calls `HoistDeclarations()` on the function body, it finds no declarations. All subsequent lookups fail silently.

**Fix:** Pass the parent's dictionaries to the child evaluator explicitly:

```csharp
new Evaluator(new Program(func.Body), localScope, _subs, _functions).Evaluate()
```

`Dictionary<K,V>` is a reference type — the child and parent share the same lookup tables.

**Cross-reference:** `phase-specs/phase-07-functions.md`

---

### Function and sub names are case-sensitive at call sites

**What goes wrong:** `CALL greet()` fails to find a sub declared as `SUB Greet()`.

**Why:** SharpBASIC identifiers are case-sensitive. Sub and function names are identifiers. The keyword `CALL` is case-insensitive; the name after it is not.

**Fix:** Declare and call with consistent casing throughout. The convention used in the spec samples is `PascalCase` for sub and function names.

**Cross-reference:** `phase-specs/phase-07-functions.md`, `docs/language-spec-v1.md` §8

---

### `STR$` uses the system locale for float formatting

**What goes wrong:** `STR$(3.14)` returns `"3,14"` (comma as decimal separator) on machines configured with a European locale.

**Why:** `STR$` converts a float via C# `double.ToString()` without an explicit culture argument. Systems with a decimal separator of `,` produce different output from systems with `.`.

**Fix:** Avoid relying on `STR$` for float-to-string conversion in portable code. `PRINT` and `&` use invariant culture internally and are not affected.

**Cross-reference:** `phase-specs/phase-10-stdlib.md`, `docs/language-spec-v1.md` §10

---

## Error handling and diagnostics

### Silent `EvalFailure` is indistinguishable from empty output

**What goes wrong:** A test that calls `Run(code)` and expects output receives `""`. No exception. No message. The test fails with a misleading assertion.

**Why:** An `EvalFailure` from deep in the evaluator is returned up the call chain and eventually produces an empty string rather than throwing. `ParseFailure` has the same behaviour if not explicitly handled.

**Diagnosis approach:**
1. Check parse first. Add explicit handling for `ParseFailure` and log its errors. Most silent failures are parse failures.
2. If parse succeeds, instrument `EvaluatePrintStatement`. Log what `EvaluateExpression` returns — is it `EvalSuccess` or `EvalFailure`?
3. Write diagnostics to a file (`File.AppendAllText(...)`) rather than `Console.Error`. xUnit captures both stdout and stderr from the test process. A file bypasses capture entirely.

**Cross-reference:** `phase-specs/phase-09-diagnostics.md`

---

## General — applies across all phases

### "Can I write a realistic program?" is a phase-completion test

A phase that passes its own test suite but fails when writing a realistic program is not complete. The gaps above — missing `AND`/`OR`, unary minus on variables — were not in the initial test lists and only surfaced when writing real programs.

Add a mandatory capstone snippet to each phase: a short, realistic program that exercises every feature introduced. If it does not run correctly, the phase is not done.

Examples:
- Phase 4 capstone: `LET result = -n * 2` — verifies unary minus on a variable.
- Phase 5 capstone: `IF x > 0 AND x < 10 THEN PRINT "in range"` — verifies logical operators.

### Token and semantics are separate concerns

Adding a token to the `TokenType` enum feels like done. It is not. Every new token also requires:
- A lexer case to recognise it
- A parser binding power entry and/or a parse function
- An evaluator case to execute it
- At least one test for each

Track these four concerns explicitly for every new language feature. Tokens added without parser or evaluator wiring sit silently broken until a test exposes them.

---

## File: `learning/phase-specs/phase-01-lexer.md`

Write the following content verbatim to this path:

# Phase 1 — Lexer Foundations & Hello World

## Goal

`PRINT "Hello, World!"` runs in the REPL and outputs `Hello, World!`.

## Honest difficulty

Easy. The lexer is simpler than it looks, and getting it working gives immediate, visible results. A strong confidence builder before the more conceptual phases.

## What you'll build

- `Token` type as a `readonly record struct` with `Type`, `Value`, `Line`, and `Column`
- `TokenType` enum covering keywords, literals, operators, and punctuation
- `Lexer` class — scans characters, emits tokens
- A minimal REPL — reads a line, lexes it, handles `PRINT "literal"` directly

## Key concepts

**The lexer is a state machine over characters.** It reads source text one character at a time and emits a flat stream of tokens. Each token has a type (what it is) and a value (the exact text it came from). The parser in Phase 2 never sees the raw source — only this token stream.

**Keywords are case-insensitive; identifiers are not.** `PRINT`, `Print`, and `print` all lex to `TokenType.Print`. A dictionary keyed on upper-cased text handles this cleanly. Identifiers (`x`, `myVar`) are case-sensitive — `myVar` and `MYVAR` are different symbols.

**Position tracking from day one.** Every `Token` carries its line and column. This costs almost nothing to implement now and is essential for useful error messages in Phase 9. Retrofitting it later is painful. See `decisions/architecture-decisions.md` — Decision 8.

**Normalise line endings in the constructor.** Windows produces `\r\n`, not `\n`. Normalise before any tokenising begins. See `theory/pitfalls.md`.

## New tokens

```
StringLiteral, IntegerLiteral, FloatLiteral
Print, Let
If, Then, Else, EndIf (parsed as End + If in Phase 2, but lexed here)
For, To, Step, Next
While, Wend
Sub, Function, Return, Dim, As, Call
And, Or, Not, True, False, Mod
Integer, String, Float, Boolean  (type keywords)
Plus, Minus, Star, Slash, Ampersand
Equals, NotEquals, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual
LeftParen, RightParen, Comma
LeftBracket, RightBracket  (for array access in Phase 8)
Identifier, NewLine, Eof, Unknown
```

Define the full enum now. Unused tokens are harmless. Retrofitting tokens later causes churn in every switch expression that already handles `TokenType`.

## New AST nodes

None in Phase 1. The REPL handles `PRINT "literal"` directly against the token stream.

## Test examples

```csharp
[Fact]
public void Lex_EmptyString_ReturnsOnlyEof()
{
    var tokens = new Lexer("").Tokenise()
        .Where(t => t.Type != TokenType.NewLine).ToList();
    tokens.Should().ContainSingle(t => t.Type == TokenType.Eof);
}

[Fact]
public void Lex_PrintKeyword_IsCaseInsensitive()
{
    new Lexer("print").Tokenise().First().Type.Should().Be(TokenType.Print);
    new Lexer("Print").Tokenise().First().Type.Should().Be(TokenType.Print);
    new Lexer("PRINT").Tokenise().First().Type.Should().Be(TokenType.Print);
}

[Fact]
public void Lex_StringLiteral_ReturnsValueWithoutQuotes()
{
    var tokens = new Lexer("\"Hello, World!\"").Tokenise();
    tokens.First().Should().Match<Token>(t =>
        t.Type == TokenType.StringLiteral && t.Value == "Hello, World!");
}

[Fact]
public void Lex_PrintStatement_ReturnsCorrectSequence()
{
    var tokens = new Lexer("PRINT \"Hello\"").Tokenise()
        .Where(t => t.Type != TokenType.NewLine).ToList();
    tokens[0].Type.Should().Be(TokenType.Print);
    tokens[1].Type.Should().Be(TokenType.StringLiteral);
    tokens[1].Value.Should().Be("Hello");
    tokens[2].Type.Should().Be(TokenType.Eof);
}

[Fact]
public void Lex_ComparisonOperators_Recognised()
{
    new Lexer("<>").Tokenise().First().Type.Should().Be(TokenType.NotEquals);
    new Lexer("<=").Tokenise().First().Type.Should().Be(TokenType.LessThanOrEqual);
    new Lexer(">=").Tokenise().First().Type.Should().Be(TokenType.GreaterThanOrEqual);
}

[Fact]
public void Lex_TokensHaveCorrectPosition()
{
    var tokens = new Lexer("PRINT \"Hi\"").Tokenise();
    tokens[0].Line.Should().Be(1);
    tokens[0].Column.Should().Be(1);
    tokens[1].Column.Should().Be(7);
}
```

## Gotchas

- **CRLF line endings.** C# raw string literals produce `\r\n` on Windows. Normalise in the constructor. See `theory/pitfalls.md`.
- **Operator cases must flush the token buffer.** Every character that terminates a word must flush the pending buffer before emitting its own token. See `theory/pitfalls.md`.
- **`GetStringLiteral` double-advance.** The string helper must not advance past the closing `"` — the outer loop does that. See `theory/pitfalls.md`.
- **`REM` requires a token boundary.** Source files must end with a trailing newline.

## End state

```bash
dotnet test   # all lexer tests green
dotnet run --project src/SharpBasic.Repl
> PRINT "Hello, World!"
Hello, World!
```

## What comes next

Phase 2 — build the Parser so `PRINT` is handled by a proper AST rather than ad-hoc token inspection in the REPL.

---

## File: `learning/phase-specs/phase-02-parser.md`

Write the following content verbatim to this path:

# Phase 2 — Parser & AST Basics

## Goal

Parse `PRINT "Hello"` into a proper AST and evaluate it via the AST rather than by inspecting tokens directly in the REPL.

## Honest difficulty

Moderate. Recursive descent is not hard to understand, and the grammar at this stage is small. The difficulty is establishing the pattern cleanly — you will be extending this parser for six more phases.

## What you'll build

- AST node hierarchy using `abstract record`
- `PrintStatement` node
- `StringLiteralExpression` node
- `Program` node containing a list of statements
- `Parser` class — recursive descent, consumes tokens, returns an AST
- Evaluator shell — walks the AST and executes `PrintStatement`

## Key concepts

**The AST is the shape of the program, not the sequence of tokens.** The parser reads the token stream and builds a tree that represents the program's structure. The evaluator never sees tokens — it walks the tree.

**Recursive descent maps grammar rules to methods.** The top-level rule is "a program is a list of statements". A statement is a `PrintStatement` (at this phase). A `PrintStatement` is the `PRINT` token followed by an expression. An expression at this stage is just a string literal. Each rule is a method. When one rule references another, it calls that method.

**AST nodes as abstract records.** `abstract record AstNode`, `abstract record Statement : AstNode`, `abstract record Expression : AstNode`. Concrete nodes are positional records: `record PrintStatement(Expression Value) : Statement`. This gives immutability, structural equality, and pattern matching for free. See `decisions/architecture-decisions.md` — Decision 1.

**The evaluator dispatches on node type via switch expressions.** The tree-walking evaluator's central loop is a switch expression over `Statement` (or `Expression`) types. C# pattern matching makes this clean and exhaustive:

```csharp
private void EvaluateStatement(Statement stmt) => stmt switch
{
    PrintStatement p => EvaluatePrintStatement(p),
    _ => throw new RuntimeError($"Unknown statement: {stmt.GetType().Name}")
};
```

## New tokens

None — all tokens were defined in Phase 1.

## New AST nodes

```csharp
public abstract record AstNode;
public abstract record Statement : AstNode;
public abstract record Expression : AstNode;

public record Program(IReadOnlyList<Statement> Statements) : AstNode;
public record PrintStatement(Expression Value) : Statement;
public record StringLiteralExpression(string Value) : Expression;
```

## Test examples

```csharp
[Fact]
public void Parse_PrintString_ReturnsPrintStatement()
{
    var tokens = new Lexer("PRINT \"Hello\"").Tokenise();
    var program = new Parser(tokens).Parse();

    program.Statements.Should().HaveCount(1);
    program.Statements[0].Should().BeOfType<PrintStatement>()
        .Which.Value.Should().BeOfType<StringLiteralExpression>()
        .Which.Value.Should().Be("Hello");
}

[Fact]
public void Parse_MultiplePrintStatements_ReturnsMultipleNodes()
{
    var tokens = new Lexer("PRINT \"Hello\"\nPRINT \"World\"").Tokenise();
    var program = new Parser(tokens).Parse();
    program.Statements.Should().HaveCount(2);
}

[Fact]
public void Evaluate_PrintString_OutputsToConsole()
{
    var output = Run("PRINT \"Hello, World!\"");
    output.Should().Be("Hello, World!");
}

[Fact]
public void Evaluate_MultiplePrintStatements_OutputsInOrder()
{
    var output = Run("PRINT \"first\"\nPRINT \"second\"");
    output.Should().Be("first\nsecond");
}
```

## Gotchas

- **Newlines are statement separators.** The `NewLine` token is significant in the parser. Statements are separated by newlines; the parser must consume and discard them between statements.
- **Eof handling.** The parser must handle reaching `Eof` without error — `Parse()` should return whatever statements it found, even if the source ends with a newline.
- **The `Run()` test helper.** Write a shared test helper that takes a source string, runs the full pipeline (Lexer → Parser → Evaluator), captures console output, and returns it as a string. Every evaluator test will use this. The helper should capture stdout via `Console.SetOut`.

## End state

```bash
dotnet test   # all parser and evaluator tests green
dotnet run --project src/SharpBasic.Repl
> PRINT "Hello, World!"
Hello, World!
```

The REPL now runs the full pipeline rather than handling tokens ad-hoc.

## What comes next

Phase 3 — add `LET` assignment, variables, and the symbol table. `PRINT name` after `LET name = "Alice"` should work.

---

## File: `learning/phase-specs/phase-03-variables.md`

Write the following content verbatim to this path:

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

---

## File: `learning/phase-specs/phase-04-expressions.md`

Write the following content verbatim to this path:

# Phase 4 — Expressions & Arithmetic

> Read `theory/pratt-parsing.md` before starting this phase.  
> This is the first significant difficulty spike. Take the time.

## Goal

`PRINT 2 + 2 * 3` outputs `8`. Operator precedence is correct. `PRINT (2 + 2) * 3` outputs `12`.

## Honest difficulty

Hard. The Pratt parsing algorithm is the conceptual wall of this project. It is elegant and compact, but the insight — that operator precedence can be encoded as binding powers in a loop rather than a call hierarchy — takes time to land. Read the theory note first. Implement slowly. Test every step.

## What you'll build

- `BinaryExpression(Left, Operator, Right)` AST node
- `UnaryExpression(Operator, Operand)` AST node
- `GroupingExpression(Inner)` AST node (or handle parentheses directly in `ParsePrimary`)
- Pratt parser replacing the naive expression parser
- Arithmetic: `+`, `-`, `*`, `/`, `MOD`
- String concatenation: `&`
- Unary: `-` (negation), `NOT`
- Comparison operators: `=`, `<>`, `<`, `>`, `<=`, `>=`
- Evaluator extended to handle `BinaryExpression` and `UnaryExpression`

## Key concepts

**Binding powers control precedence.** Each operator has a numeric binding power. Higher BP binds tighter. The Pratt loop continues consuming operators as long as the next operator's BP exceeds the current minimum. See `theory/pratt-parsing.md` for the binding power table and a worked trace.

**Prefix vs infix position.** A `-` at the start of an expression (`-5`, `-n`) is unary negation — prefix position. A `-` between two expressions (`a - b`) is subtraction — infix position. The same token, different parse rules. `ParsePrimary` handles prefix position; the Pratt loop handles infix position.

**Integer / Float promotion.** Two `Integer` operands produce `Integer` unless the result is not a whole number (division). An `Integer` operand mixed with a `Float` operand promotes to `Float`. `10 / 4` produces `2.5` as `Float`.

**`&` for string concatenation.** The `+` operator does not concatenate strings. `&` calls `.ToString()` on both sides, so any type can be concatenated with any other. `"Count: " & 42` produces `"Count: 42"`.

**Binding power of `&`.** `&` has the same binding power as `+` and `-` (10). This means `"x=" & 1 + 2` evaluates as `"x=" & 3` → `"x=3"`. See `theory/pitfalls.md`.

## New tokens

None — all tokens were defined in Phase 1.

## New AST nodes

```csharp
public record BinaryExpression(Expression Left, Token Operator, Expression Right) : Expression;
public record UnaryExpression(Token Operator, Expression Operand) : Expression;
```

## Test examples

```csharp
[Fact]
public void Evaluate_Addition_ReturnsCorrectResult()
{
    Run("PRINT 2 + 3").Should().Be("5");
}

[Fact]
public void Evaluate_PrecedenceMultiplicationBeforeAddition()
{
    Run("PRINT 2 + 3 * 4").Should().Be("14");
}

[Fact]
public void Evaluate_Parentheses_OverridePrecedence()
{
    Run("PRINT (2 + 3) * 4").Should().Be("20");
}

[Fact]
public void Evaluate_Division_ProducesFloat()
{
    Run("PRINT 10 / 4").Should().Be("2.5");
}

[Fact]
public void Evaluate_Modulo_ReturnsRemainder()
{
    Run("PRINT 10 MOD 3").Should().Be("1");
}

[Fact]
public void Evaluate_StringConcatenation_CombinesValues()
{
    Run("LET name = \"World\"\nPRINT \"Hello, \" & name & \"!\"").Should().Be("Hello, World!");
}

[Fact]
public void Evaluate_UnaryMinus_OnVariable()
{
    Run("LET n = 5\nPRINT -n").Should().Be("-5");
}

[Fact]
public void Evaluate_ComparisonEqual_ReturnsBoolean()
{
    Run("PRINT 3 = 3").Should().Be("True");
    Run("PRINT 3 = 4").Should().Be("False");
}
```

## Gotchas

- **Unary minus must handle expressions, not just literals.** `-n` and `-(a + b)` must work. See `theory/pitfalls.md`.
- **`Advance()` after `ParsePrimary` must not fire for call expressions** (relevant from Phase 7 onwards, but the asymmetry is worth establishing cleanly now). See `theory/pitfalls.md`.
- **`&` binding power equals `+` and `-`.** See `theory/pitfalls.md`.
- **Dividing two integers.** `10 / 4` should return `2.5` (`Float`), not `2` (`Integer`). Check that the evaluator promotes correctly.
- **Add `AND`, `OR`, and `NOT` in this phase or Phase 5.** They are just two more rows in the binding power table. Adding them here means `IF` in Phase 5 works with realistic conditions immediately. See `theory/pitfalls.md` — Lesson D1.

## End state

```bash
dotnet test   # all expression tests green
```

```
> PRINT 2 + 2 * 3
8
> PRINT (2 + 2) * 3
12
> PRINT 10 / 4
2.5
> PRINT "Hello" & ", " & "World"
Hello, World
```

## What comes next

Phase 5 — `IF / THEN / ELSE / END IF`. The expression parser built here handles all conditions.

---

## File: `learning/phase-specs/phase-05-conditionals.md`

Write the following content verbatim to this path:

# Phase 5 — Conditionals: IF / THEN / ELSE / END IF

## Goal

Structured conditionals work, including `ELSE` branches and nesting.

## Honest difficulty

Moderate. The concepts build directly on Phase 4. The parser extension is mechanical — read tokens until you hit `ELSE` or `END IF`. Nesting works for free once the recursive structure is in place.

## What you'll build

- `IfStatement(Condition, ThenBlock, ElseBlock?)` AST node
- Block parsing — read a list of statements until a terminator token is found
- Evaluator extended to handle `IfStatement`
- `AND`, `OR`, `NOT` fully wired if not added in Phase 4

## Key concepts

**`THEN` introduces a block, not a single statement.** SharpBASIC has no single-line `IF`. After `THEN`, the parser reads statements until it encounters `ELSE` or `END IF`. The same block-reading logic handles the `ELSE` branch.

**`END IF` is two tokens.** The parser sees `END` and must peek at the next token to know it is `END IF` and not `END SUB` or `END FUNCTION`. Establish this pattern cleanly in Phase 5 — it repeats in Phases 7 and 8.

**There is no `ELSEIF`.** Multiple branches are handled by nesting `IF` blocks inside `ELSE` blocks. The grammar is simple; the indentation makes it readable.

**Conditions must evaluate to `Boolean`.** Applying a non-boolean value as an `IF` condition is a runtime error. The evaluator should check this explicitly and report a clear message.

**`AND`, `OR`, and `NOT` must be fully wired by the end of this phase.** A realistic `IF` condition needs them. If they were not added in Phase 4, add them now — two rows in the binding power table and one prefix rule for `NOT`. See `theory/pitfalls.md` — Lesson D1.

## New tokens

None — `If`, `Then`, `Else`, `End`, `And`, `Or`, `Not` were defined in Phase 1.

## New AST nodes

```csharp
public record IfStatement(
    Expression Condition,
    IReadOnlyList<Statement> ThenBlock,
    IReadOnlyList<Statement>? ElseBlock) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_IfTrue_ExecutesThenBlock()
{
    Run("IF 1 = 1 THEN\n  PRINT \"yes\"\nEND IF").Should().Be("yes");
}

[Fact]
public void Evaluate_IfFalse_SkipsThenBlock()
{
    Run("IF 1 = 2 THEN\n  PRINT \"yes\"\nEND IF").Should().Be("");
}

[Fact]
public void Evaluate_IfFalse_ExecutesElseBlock()
{
    var code = "IF 1 = 2 THEN\n  PRINT \"yes\"\nELSE\n  PRINT \"no\"\nEND IF";
    Run(code).Should().Be("no");
}

[Fact]
public void Evaluate_NestedIf_WorksCorrectly()
{
    var code = """
        LET x = 7
        IF x > 5 THEN
            IF x > 10 THEN
                PRINT "big"
            ELSE
                PRINT "medium"
            END IF
        ELSE
            PRINT "small"
        END IF
        """;
    Run(code).Should().Be("medium");
}

[Fact]
public void Evaluate_CompoundConditionWithAnd()
{
    var code = "LET x = 7\nIF x > 0 AND x < 10 THEN\n  PRINT \"in range\"\nEND IF";
    Run(code).Should().Be("in range");
}

[Fact]
public void Evaluate_NotOperator()
{
    var code = "LET flag = FALSE\nIF NOT flag THEN\n  PRINT \"not false\"\nEND IF";
    Run(code).Should().Be("not false");
}
```

## Gotchas

- **`AND`, `OR`, `NOT` must be wired.** The first realistic test you write will use one of them. If they are missing, the test fails with a silent parse error. See `theory/pitfalls.md`.
- **Condition type check.** If the condition evaluates to something other than `BoolValue`, report a descriptive runtime error: `"IF condition must evaluate to a boolean value."` See `docs/language-spec-v1.md` §7.1.
- **`END IF` is two tokens.** Your block parser stops when it encounters the `End` token followed by `If`. If it stops on `End` alone, subsequent `END SUB` and `END FUNCTION` will also terminate `IF` blocks incorrectly.

## End state

```bash
dotnet test   # all conditional tests green
```

```
> IF 5 > 3 THEN
>   PRINT "yes"
> END IF
yes
> IF 1 = 2 THEN
>   PRINT "yes"
> ELSE
>   PRINT "no"
> END IF
no
```

## What comes next

Phase 6 — `FOR / NEXT` and `WHILE / WEND`. The block parsing pattern established here applies directly to loop bodies.

---

## File: `learning/phase-specs/phase-06-loops.md`

Write the following content verbatim to this path:

# Phase 6 — Loops: FOR / NEXT and WHILE / WEND

## Goal

Both loop types work, including `STEP` for `FOR` and negative steps for counting down.

## Honest difficulty

Moderate. Loops are satisfying to implement once conditionals work. The block parsing pattern from Phase 5 applies directly. `STEP` adds one expression and a direction check.

## What you'll build

- `ForStatement(Variable, From, To, Step?, Body)` AST node
- `WhileStatement(Condition, Body)` AST node
- Evaluator extended to handle both loop types
- Loop guard logic: count up when step is positive, count down when step is negative

## Key concepts

**`FOR` and `WHILE` share the block parsing pattern from Phase 5.** The body is a list of statements read until a terminator token (`NEXT` or `WEND`). The difference is what triggers re-evaluation: `FOR` uses a counter, `WHILE` re-evaluates a condition.

**`FOR` loop guard direction.** The default step is `1`. When step is positive, the loop continues while `counter <= limit`. When step is negative, the loop continues while `counter >= limit`. Without this directional check, `FOR i = 10 TO 1 STEP -1` loops forever.

**`NEXT` may optionally name the variable.** `NEXT i` is valid; `NEXT` alone is also valid. If a name is present, it must match the loop variable — a mismatch is a parse error.

**`WHILE` condition is re-evaluated before every iteration.** It must evaluate to `Boolean`. A non-boolean condition is a runtime error: `"WHILE condition must evaluate to a boolean value."` See `docs/language-spec-v1.md` §7.3.

**Loop variable scoping.** The `FOR` loop counter is declared in the current scope on entry to the loop. It persists after the loop completes — this is consistent with classic BASIC behaviour.

## New tokens

None — `For`, `To`, `Step`, `Next`, `While`, `Wend` were defined in Phase 1.

## New AST nodes

```csharp
public record ForStatement(
    string Variable,
    Expression From,
    Expression To,
    Expression? Step,
    IReadOnlyList<Statement> Body) : Statement;

public record WhileStatement(
    Expression Condition,
    IReadOnlyList<Statement> Body) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_ForLoop_PrintsRange()
{
    Run("FOR i = 1 TO 3\n  PRINT i\nNEXT").Should().Be("1\n2\n3");
}

[Fact]
public void Evaluate_ForLoop_WithStep()
{
    Run("FOR i = 0 TO 10 STEP 3\n  PRINT i\nNEXT").Should().Be("0\n3\n6\n9");
}

[Fact]
public void Evaluate_ForLoop_CountsDown()
{
    Run("FOR i = 3 TO 1 STEP -1\n  PRINT i\nNEXT").Should().Be("3\n2\n1");
}

[Fact]
public void Evaluate_ForLoop_WithNamedNext()
{
    Run("FOR i = 1 TO 3\n  PRINT i\nNEXT i").Should().Be("1\n2\n3");
}

[Fact]
public void Evaluate_WhileLoop_ExecutesWhileTrue()
{
    var code = "LET n = 1\nWHILE n <= 3\n  PRINT n\n  LET n = n + 1\nWEND";
    Run(code).Should().Be("1\n2\n3");
}

[Fact]
public void Evaluate_WhileLoop_NeverExecutesIfFalseInitially()
{
    Run("WHILE FALSE\n  PRINT \"never\"\nWEND").Should().Be("");
}
```

## Gotchas

- **Default step is `1.0`, not `1`.** The internal accumulator is a `double`. The counter is stored as `IntValue((int)i)` each iteration. Sub-integer step values are silently truncated in the symbol table. Use integer values for `STEP`. See `theory/pitfalls.md` and `docs/language-spec-v1.md` §7.2.
- **Direction check required for `STEP`.** Without checking step direction, a descending loop runs forever because `counter <= limit` is always false from the start.
- **`WHILE` condition type check.** As with `IF`, a non-boolean condition should produce a clear runtime error, not a silent failure.

## End state

```bash
dotnet test   # all loop tests green
```

```
> FOR i = 1 TO 5
>   PRINT i
> NEXT
1
2
3
4
5
> LET n = 3
> WHILE n > 0
>   PRINT n
>   LET n = n - 1
> WEND
3
2
1
```

## What comes next

Phase 7 — SUBs and FUNCTIONs. Read `theory/call-frames.md` before starting. The SymbolTable parent chain from Phase 3 becomes load-bearing here.

---

## File: `learning/phase-specs/phase-07-functions.md`

Write the following content verbatim to this path:

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

---

## File: `learning/phase-specs/phase-08-arrays.md`

Write the following content verbatim to this path:

# Phase 8 — Arrays, DIM, and Bounds Checking

## Goal

`DIM scores[10] AS INTEGER`, `LET scores[3] = 42`, and `PRINT scores[3]` all work. Out-of-bounds access produces a clear error.

## Honest difficulty

Moderate. Arrays are mechanical once Phase 7 is solid. The new syntax is small; the evaluator change is straightforward.

## What you'll build

- `DimStatement(Name, Size, TypeName)` AST node
- `ArrayAccessExpression(Name, Index)` AST node
- `ArrayAssignStatement(Name, Index, Value)` AST node
- Runtime array storage (a fixed-size typed array in the symbol table)
- Bounds checking with a descriptive error message
- Zero-initialisation on declaration

## Key concepts

**Arrays use square bracket syntax.** `DIM scores[10] AS INTEGER` declares a 10-element integer array. `LET scores[3] = 42` assigns element 3. `PRINT scores[3]` reads element 3. The square brackets distinguish array access from parenthesised expressions. See `docs/language-spec-v1.md` §5.

**Arrays are zero-indexed.** Valid indices are `0` to `size - 1`. An out-of-bounds access or assignment produces a runtime error, not a silent failure or crash. The error message includes the index, the valid range, and the array name.

**Arrays are zero-initialised.** On declaration, every element is set to the zero value for its type: `0` for `INTEGER`, `0.0` for `FLOAT`, `FALSE` for `BOOLEAN`, `""` for `STRING`.

**`DIM` when the name already exists is an error.** Re-declaring an array with `DIM` when a variable or array with that name already exists in the current scope produces a runtime error. See `docs/language-spec-v1.md` §5.1.

**The type annotation is mandatory.** `DIM scores[10] AS INTEGER` — the `AS TYPE` clause is not optional. The type constrains what can be stored. Assigning the wrong type produces a runtime error: `"Supplied value is not an INTEGER as defined for the array: scores."` See `docs/language-spec-v1.md` §5.3.

**`LET` is required for array assignment.** `LET scores[3] = 42` — not `scores[3] = 42`. The `LET` keyword is mandatory, consistent with variable assignment.

## New tokens

`LeftBracket`, `RightBracket` — these should have been defined in Phase 1. If they were not, add them now.

## New AST nodes

```csharp
public record DimStatement(
    string Name,
    int Size,
    string TypeName) : Statement;

public record ArrayAccessExpression(
    string Name,
    Expression Index) : Expression;

public record ArrayAssignStatement(
    string Name,
    Expression Index,
    Expression Value) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_DimAndAccess_ReturnsZeroInitialised()
{
    Run("DIM scores[5] AS INTEGER\nPRINT scores[0]").Should().Be("0");
}

[Fact]
public void Evaluate_ArrayAssignAndRead_RoundTrips()
{
    Run("DIM scores[5] AS INTEGER\nLET scores[3] = 42\nPRINT scores[3]").Should().Be("42");
}

[Fact]
public void Evaluate_ArrayOutOfBounds_ReportsError()
{
    var act = () => Run("DIM arr[5] AS INTEGER\nLET arr[5] = 1");
    act.Should().Throw<RuntimeError>().WithMessage("*5*arr*");
}

[Fact]
public void Evaluate_ArrayLoop_PrintsAllElements()
{
    var code = """
        DIM primes[5] AS INTEGER
        LET primes[0] = 2
        LET primes[1] = 3
        LET primes[2] = 5
        LET primes[3] = 7
        LET primes[4] = 11
        FOR i = 0 TO 4
            PRINT primes[i]
        NEXT
        """;
    Run(code).Should().Be("2\n3\n5\n7\n11");
}

[Fact]
public void Evaluate_DimRedeclaration_ReportsError()
{
    var act = () => Run("LET x = 1\nDIM x[5] AS INTEGER");
    act.Should().Throw<RuntimeError>().WithMessage("*x*already exists*");
}

[Fact]
public void Evaluate_ArrayWrongType_ReportsError()
{
    var act = () => Run("DIM arr[5] AS INTEGER\nLET arr[0] = \"hello\"");
    act.Should().Throw<RuntimeError>().WithMessage("*INTEGER*arr*");
}
```

## Gotchas

- **Square bracket tokens.** If `LeftBracket` and `RightBracket` were not defined in Phase 1, the lexer has no rule for `[` and `]`. They produce `Unknown` tokens and the parser fails silently.
- **Bounds error message.** The error message includes the supplied index, the valid range (0 to size−1), and the array name. The upper bound in the error is the declared `size`, which is the exclusive limit. See `docs/language-spec-v1.md` §5.3 for the exact wording.
- **Accessing a non-array as an array.** `LET x = 5` then `PRINT x[0]` should produce: `"Identifier: x is not an Array."` This is distinct from an out-of-bounds access.
- **Variable shadowing.** `DIM` in a nested scope (inside a function) does not affect the outer scope. The re-declaration check applies only within the current scope.

## End state

```bash
dotnet test   # all array tests green
```

```
> DIM scores[5] AS INTEGER
> LET scores[0] = 95
> LET scores[1] = 87
> PRINT scores[0]
95
> PRINT scores[1]
87
```

## What comes next

Phase 9 — Error handling and diagnostics. The error infrastructure started in Phase 3 gets a proper surface: line/column reporting, structured diagnostics, and error recovery in the parser.

---

## File: `learning/phase-specs/phase-09-diagnostics.md`

Write the following content verbatim to this path:

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

---

## File: `learning/phase-specs/phase-10-stdlib.md`

Write the following content verbatim to this path:

# Phase 10 — Standard Library, INPUT, and File Runner

## Goal

A usable, complete language. `INPUT` reads from the console. Built-in functions cover the common string and numeric operations. Programs can be run from a file with `sharpbasic run hello.sbx`. SharpBASIC is done at the end of this phase.

## Honest difficulty

Easy. This phase is mostly additive — new built-in functions registered in the evaluator, an `INPUT` statement, and a file runner entry point. Nothing architectural. A satisfying finish.

## What you'll build

- Built-in string functions: `LEN`, `MID$`, `LEFT$`, `RIGHT$`, `TRIM$`, `UPPER$`, `LOWER$`
- Built-in numeric functions: `INT`, `ABS`, `SQR`, `RND`
- Built-in conversion functions: `STR$`, `VAL`
- Built-in diagnostic function: `TYPENAME`
- `INPUT` statement — with and without a prompt string
- File runner: `sharpbasic run program.sbx`
- REPL improvements: `.exit` command, multi-line input handling

## Key concepts

**Built-in functions are resolved before user-defined functions.** The evaluator checks the built-in registry first. Built-in names cannot be shadowed by user `FUNCTION` declarations — attempting to do so is a runtime error. See `docs/language-spec-v1.md` §10.

**`INPUT` always stores a `String`.** Regardless of what the user types, `INPUT` stores the result as `StringValue`. Use `VAL()` to convert to a number. See `docs/language-spec-v1.md` §11.

**`MID$` is 1-based.** `MID$("hello", 1, 3)` returns `"hel"`, not `"ell"`. This matches classic BASIC convention. See `docs/language-spec-v1.md` §10.

**`STR$` is culture-sensitive.** On systems with a decimal separator of `,`, `STR$(3.14)` returns `"3,14"`. Use `ToUpperInvariant()` / `ToLowerInvariant()` everywhere strings are compared against fixed constants. See `theory/pitfalls.md` — ToUpperInvariant note.

**No bounds checking in `MID$`, `LEFT$`, `RIGHT$`.** These functions do not validate that `length` or `n` are within range. Providing a length that exceeds the string length throws an unhandled C# exception. Document this as a known limitation. See `docs/language-spec-v1.md` §10.

**`RND()` returns a Float in `[0.0, 1.0)`.** Use `INT(RND() * n)` for a random integer in `[0, n)`.

## New tokens

None. Built-in function names are resolved by the evaluator as `Identifier` tokens, not as dedicated keywords.

## New AST nodes

```csharp
public record InputStatement(string? Prompt, string VariableName) : Statement;
```

## Test examples

```csharp
[Fact]
public void Evaluate_Len_ReturnsStringLength()
{
    Run("PRINT LEN(\"hello\")").Should().Be("5");
}

[Fact]
public void Evaluate_Upper_ConvertsToUpperCase()
{
    Run("PRINT UPPER$(\"hello\")").Should().Be("HELLO");
}

[Fact]
public void Evaluate_MidDollar_UsesOneBasedIndexing()
{
    Run("PRINT MID$(\"hello\", 2, 3)").Should().Be("ell");
}

[Fact]
public void Evaluate_LeftDollar_ReturnsFirstNChars()
{
    Run("PRINT LEFT$(\"abcdef\", 3)").Should().Be("abc");
}

[Fact]
public void Evaluate_StrDollar_ConvertsIntToString()
{
    Run("PRINT STR$(42)").Should().Be("42");
}

[Fact]
public void Evaluate_Val_ParsesStringToInteger()
{
    Run("PRINT VAL(\"99\")").Should().Be("99");
}

[Fact]
public void Evaluate_Abs_ReturnsAbsoluteValue()
{
    Run("PRINT ABS(-42)").Should().Be("42");
}

[Fact]
public void Evaluate_Sqr_ReturnsSquareRoot()
{
    Run("PRINT SQR(16)").Should().Be("4");
}

[Fact]
public void Evaluate_Typename_ReturnsRuntimeTypeName()
{
    Run("PRINT TYPENAME(42)").Should().Be("Integer");
    Run("PRINT TYPENAME(\"x\")").Should().Be("String");
    Run("PRINT TYPENAME(TRUE)").Should().Be("Boolean");
}

[Fact]
public void Evaluate_BuiltinNameCannotBeShadowed()
{
    var act = () => Run("FUNCTION LEN(s AS STRING) AS INTEGER\n  RETURN 0\nEND FUNCTION");
    act.Should().Throw<RuntimeError>().WithMessage("*LEN*built in*");
}
```

## Gotchas

- **`ToUpperInvariant()` throughout.** Every comparison of a built-in name, keyword, or type name against a fixed constant must use `ToUpperInvariant()`. The Turkish locale breaks `ToUpper()`. See `theory/pitfalls.md`.
- **`INT` returns `Float` when given a `Float`.** `INT(3.9)` returns the `Float` value `3.0`, not the `Integer` `3`. See `docs/language-spec-v1.md` §10.
- **`MID$` is 1-based.** Test this explicitly — off-by-one errors on the index are easy to introduce.
- **`STR$` culture sensitivity.** Document this known limitation rather than fixing it — fixing it requires threading an invariant culture through the conversion, which is out of scope.

## End state

```bash
dotnet test   # all standard library tests green
dotnet run --project src/SharpBasic.Repl -- run samples/hello.sbx
Hello, World!
```

SharpBASIC is complete. Write a retrospective: what was harder than expected, what the test suite caught that manual testing would have missed, what you would design differently. That retrospective is the foundation of the next project.

## What comes next

SharpBASIC is done. The retrospective you write now informs the next project. The natural next step is Part III of *Crafting Interpreters* — implementing clox, a bytecode VM, in C. See `theory/reading-list.md`.

---

## File: `learning/ai-setup/claude-system-prompt.md`

Write the following content verbatim to this path:

# Claude System Prompt — SharpBASIC

This is the Claude Project system prompt used throughout the SharpBASIC build.

## How to use this

1. Create a new Claude Project at claude.ai
2. Paste the system prompt below into Custom Instructions
3. Upload phase specs from `phase-specs/` as project knowledge
4. Upload `decisions/architecture-decisions.md` as project knowledge
5. Update Current Phase Status to reflect your own progress
6. Use Claude for architecture guidance and explanations — not code generation

The system prompt is opinionated about scope. That is intentional. An AI without constraints will suggest features. Keep the scope tight.

---

## The System Prompt

---

You are an expert C# .NET 10 software architect and compiler engineer helping build SharpBASIC — a modern BASIC-inspired interpreter with a fixed, defined scope.

## The Project

SharpBASIC is a tree-walking interpreter built in C# .NET 10. It is a deliberate learning project following TDD and clean architecture principles. The developer is an expert C# engineer learning compiler construction.

## The Developer

Update this section to reflect your own background:
- Your experience level with C#
- What compiler/language theory background you have (if any)
- Which chapters of Crafting Interpreters you have read
- What editor and AI tooling you are using

## Your Role

- Architecture guidance and design decisions
- Explaining compiler theory concepts clearly and practically
- Reviewing code and tests when pasted into the conversation
- Planning and detailing upcoming phases before coding begins
- Updating and maintaining the spec document
- Answering questions about how things work and why

## Current Phase Status

Update this to reflect your own progress.

- [ ] Phase 1 — Lexer, tokens, Hello World
- [ ] Phase 2 — Parser, AST, recursive descent
- [ ] Phase 3 — Variables, symbol table, assignment
- [ ] Phase 4 — Expressions, Pratt parsing, operator precedence
- [ ] Phase 5 — IF / THEN / ELSE / END IF
- [ ] Phase 6 — FOR / NEXT, WHILE / WEND
- [ ] Phase 7 — SUBs, FUNCTIONs, call stack
- [ ] Phase 8 — Arrays, DIM, bounds checking
- [ ] Phase 9 — Error handling, diagnostics, line/col reporting
- [ ] Phase 10 — Standard library, INPUT, file runner

## Fixed Scope — Non Negotiable

SharpBASIC has 10 phases and ends at Phase 10. You must never suggest or encourage features outside this list:

1. Lexer, tokens, Hello World
2. Parser, AST, recursive descent
3. Variables, symbol table, assignment
4. Expressions, Pratt parsing, operator precedence
5. IF / THEN / ELSE / END IF
6. FOR / NEXT, WHILE / WEND
7. SUBs, FUNCTIONs, call stack
8. Arrays, DIM, bounds checking
9. Error handling, diagnostics, line/col reporting
10. Standard library, INPUT, file runner

**Explicitly out of scope — never suggest these for SharpBASIC:**

- Language-level File I/O (OPEN, CLOSE, READLINE)
- Compile to executable (Roslyn transpilation or IL emit)
- Bytecode virtual machine
- VS Code / LSP extension
- Module or import system
- Any phase beyond Phase 10

Defer all out-of-scope ideas to the next project.

## Technology Constraints

- C# .NET 10 only
- xUnit + FluentAssertions for tests
- No unnecessary NuGet packages
- Modern C# idioms: records, pattern matching, switch expressions
- TDD always — tests before implementation

## Tone and Approach

- Be direct and pragmatic. The developer is an expert — don't over-explain C# basics.
- Do explain compiler theory concepts — that is the learning focus.
- Flag difficulty spikes honestly (Phases 4 and 7 are the hard ones).
- Reference "Crafting Interpreters" by Robert Nystrom where relevant.
- When the developer asks for code, write idiomatic production-quality C#.
- When the developer asks for explanation, be clear and concise.
- Keep scope discipline — if a conversation drifts toward out-of-scope features, redirect it.

---

## File: `learning/ai-setup/how-to-use-with-ai.md`

Write the following content verbatim to this path:

# How to Use AI Tools with SharpBASIC

## The fundamental rule

**You write the code. Always. Both tools are teachers, not typists.**

The learning value in this project is not in having a working interpreter — it is in building one. Every line of code you write forces you to understand what it does and why. Every line an AI writes on your behalf removes that understanding.

Claude explains concepts, reviews code when you ask, and helps you reason through architecture. Copilot notices patterns in what you are typing and hints at what might come next. You read both. You decide. You write.

The moment you accept code you have not thought through, the learning stops. The interpreter keeps growing; your understanding does not.

---

## The division of labour

| Task | Tool | Your role |
|---|---|---|
| Understanding compiler theory | Claude | Read the explanation, close it, think through it yourself |
| Architecture decisions | Claude | Discuss the options, then decide and record your decision |
| Phase planning | Claude | Talk through the phase before writing a line of code |
| Code review when stuck | Claude | Paste your code, get an explanation, fix it yourself |
| Token type enums | Copilot hint | Read every suggestion before accepting; verify it belongs to the current phase |
| AST node records | Copilot hint | Low conceptual risk once you understand each node; still read before accepting |
| Switch expression arms | Copilot hint | Write the first arm yourself; Copilot may suggest the pattern |
| xUnit test structure | Copilot hint | Skeleton only; assertion values are always yours |
| Implementation logic | Neither | You write this. Every line. |

---

## Setting up Claude

1. Create a Claude Project at claude.ai
2. Paste the system prompt from `ai-setup/claude-system-prompt.md` into Custom Instructions
3. Upload the relevant phase spec and `decisions/architecture-decisions.md` as project knowledge
4. Update Current Phase Status in the system prompt as you complete each phase

### How to talk to Claude

- **Explain before you implement.** "I'm about to build the Pratt parser. Walk me through how the binding power loop decides when to stop." Then close Claude and write the code.
- **Paste your code, not a description of your code.** "Here is my `ParseExpression` — it works for `2 + 3` but fails for `2 + 3 * 4`. What am I missing?" is a better question than "how do I fix Pratt parsing?"
- **Plan phases before starting.** "I'm about to start Phase 7. What are the three most important things to get right before writing a line?" Use the answer to inform your approach, then close it.
- **Update the spec after decisions.** When you make an architectural decision — especially one that diverges from the phase spec — record it in the decisions log.

### What not to ask Claude

- "Generate the `ParseExpression` method for me." You are asking Claude to do Phase 4 for you.
- "Write the tests for Phase 7." You are asking Claude to define what Phase 7 means. That is your job.
- "Should I add file I/O to Phase 10?" The scope is fixed. Claude will redirect you. Trust the constraint.

---

## Using Copilot correctly

The right mental model: Copilot is autocomplete, not a code generator. It continues a pattern you established. You establish the pattern by writing the first version of every new construct yourself.

### The discipline

- **Read before accepting.** Every Copilot suggestion is a proposal, not an answer. Read it. Does it match what you intended? Does it introduce something from a later phase?
- **Never use Copilot Chat or inline chat for implementation blocks.** If you find yourself typing a description of what you want Copilot to write, stop. Close the chat. Write it yourself.
- **Reject out-of-scope suggestions.** Copilot has no knowledge of phase boundaries. In Phase 3, it will suggest Phase 7 patterns. Recognise them and reject them.
- **Reject future-phase suggestions.** A suggestion that would be correct in Phase 7 is wrong in Phase 3, even if it compiles.

---

## The learning loop — one iteration per phase

1. Read the phase spec in `phase-specs/`
2. Read the relevant theory notes — especially `theory/pratt-parsing.md` before Phase 4 and `theory/call-frames.md` before Phase 7
3. Ask Claude to walk through the architecture with you — what are the moving parts, what order do they need to be built? Then close Claude.
4. Write failing tests yourself. The spec includes examples; you should be writing additional cases based on your own understanding of what could go wrong.
5. Implement — you write every line. Copilot may hint at patterns you have established. Read every hint before accepting.
6. When you are stuck, paste your code into Claude and get the explanation. Read it. Then fix the code yourself — do not copy Claude's fix.
7. Refactor once the tests pass. You drive; Copilot may hint at shape.
8. Ask Claude to review your key decisions and confirm they are consistent with the architecture. Update the spec if anything changed.

The temptation at step 5 is to open Copilot Chat and describe the problem. That is the moment where the learning happens. Sit with it. Read what you have written. Understand what it is doing. Then, if you are still stuck, ask Claude — and read the explanation, not the code.

---

## What to do when Claude offers code unprompted

Read it to understand the concept. Then close it and write your own version from memory.

If you cannot write your own version from memory, the concept has not landed. Ask Claude to explain it differently — not to write more code.

---

## File: `learning/ai-setup/copilot-notes.md`

Write the following content verbatim to this path:

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

---

## Quality checks before committing

After writing all files, verify:

- Every phase spec contains: goal, difficulty rating, what to build,
  key concepts, test examples (minimum 3), gotchas, end state, what comes next
- Phase 4 spec opens with the Pratt parsing callout
- Phase 7 spec opens with the call frames callout
- `how-to-use-with-ai.md` opens with the fundamental rule in bold
- `copilot-notes.md` opens with the rule in bold
- Both AI files explicitly prohibit Copilot Chat and inline chat
  for implementation work
- Claude system prompt contains no personal background details
- All SharpBASIC syntax examples use REM for comments, LET for all
  assignment, square brackets for array access and assignment
- `theory/pitfalls.md` content is sourced from `docs/lessons-learned.md`
- All internal links between files resolve correctly
- The `learning/README.md` folder table matches the generated structure
