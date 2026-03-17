# SharpBASIC — Task Tracker

Current phase and active task list. Update as you progress.

---

## Current Phase: 10 — Standard Library & File Runner

**Goal:** Built-in functions (`LEN`, `MID$`, `STR$`, etc.) and ability to run `.bas` files from the command line.

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `feat/phase10-stdlib` | Standard library built-in functions | ⬜ |
| 2 | `feat/phase10-file-runner` | File runner — `sharpbasic run <file.bas>` | ⬜ |

---

## Completed — Phase 9 — Error Handling & Diagnostics

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `feat/phase9-diagnostic` | `Diagnostic` record + `DiagnosticSeverity` enum in `SharpBasic.Ast` | ✅ Complete — merged to `main` |
| 2 | `feat/phase9-lexer-location` | Lexer line/column tracking — `_line`/`_col` fields, `Advance()` updated | ✅ Complete — merged to `main` |
| 3 | `feat/phase9-migrate-errors` | Migrate `ParseFailure`/`EvalFailure` to `IReadOnlyList<Diagnostic>`; delete `ParseError`, `ParseStatementError`, `EvalError` | ✅ Complete — merged to `main` |
| 4 | `feat/phase9-repl-diagnostics` | REPL displays `diagnostic.ToString()` output | ✅ Complete — merged to `main` |
| 5 | `chore/remove-semantics` | Remove `SharpBasic.Semantics` from solution and disk | ✅ Complete — merged to `main` |

---

## Completed — Phase 8 — Arrays

---

## Completed — Phase 7 — Subroutines & Functions

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `feat/phase7-tokens` | 14 new tokens: `Sub`, `Function`, `Return`, `Call`, `As`, `And`, `Or`, `Not`, `True`, `False`, `Comma`, `Integer`, `String`, `Boolean` | ✅ Complete — merged to `main` |
| 2 | `feat/phase7-ast` | AST nodes: `Parameter`, `SubDeclaration`, `FunctionDeclaration`, `ReturnStatement`, `CallStatement`, `CallExpression` | ✅ Complete — merged to `main` |
| 3 | `feat/phase7-parser` | Parser: `ParseSubDeclaration`, `ParseFunctionDeclaration`, `ParseCallStatement`, `ParseCallExpression` + lexer flush & CRLF bugs fixed | ✅ Complete — merged to `main` |
| 4 | `feat/phase7-evaluator` | Evaluator: two-pass hoisting, call frames (SymbolTable chain), `ReturnException`, argument binding, recursive Fibonacci | ✅ Complete — merged to `main` |

---

## Completed — Phase 4

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `feat/lexer-numbers-operators` | Lexer recognises integer literals and `+` `-` `*` `/` `(` `)` | ✅ Complete — merged to `main` |
| 2 | `feat/ast-expressions` | `IntLiteralExpression`, `FloatLiteralExpression`, `BinaryExpression` AST nodes | ✅ Complete — merged to `main` |
| 3 | `feat/parser-pratt` | Pratt expression parser — precedence, associativity | ✅ Complete — merged to `main` |
| 4 | `feat/evaluator-arithmetic` | Evaluator handles `BinaryExpression`, int and float arithmetic, unified promotion path | ✅ Complete — merged to `main` |

---

## Completed — Phase 3

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `test/run-helper` | `Run()` test helper in `EvaluatorTests` | ✅ Complete — merged to `main` |
| 2 | `feat/symbol-table` | `SymbolTable` class + inject into `Evaluator` | ✅ Complete — merged to `main` |
| 3 | `feat/lexer-let-identifier` | Lexer recognises `LET`, `=`, and identifier tokens | ✅ Complete — merged to `main` |
| 4 | `feat/ast-let-identifier` | `LetStatement` and `IdentifierExpression` AST nodes | ✅ Complete — merged to `main` |
| 5 | `feat/parser-let` | Parser parses `LET x = "value"` into a `LetStatement` | ✅ Complete — merged to `main` |
| 6 | `feat/evaluator-let` | Evaluator executes `LetStatement` and resolves `IdentifierExpression` | ✅ Complete — merged to `main` |

---

## Completed — Phase 2

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `feat/ast-nodes` | AST node hierarchy — `AstNode`, `Statement`, `Expression`, `Program` | ✅ Complete — merged to `main` |
| 2 | `feat/ast-print-statement` | `PrintStatement` and `StringLiteralExpression` nodes | ✅ Complete — merged to `main` |
| 3 | `feat/parser-print` | `Parser` class — parses `PRINT "..."` into a `PrintStatement` | ✅ Complete — merged to `main` |
| 4 | `feat/evaluator-core` | `ParseResult` + `EvalResult` discriminated unions, `Evaluator` class, `SourceLocation` on AST nodes, REPL uses full pipeline | ✅ Complete — merged to `main` |

---

## Completed — Phase 1

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `chore/init-git` | First commit — scaffold, README, .gitignore | ✅ Complete — merged to `main` |
| 2 | `feat/token-type-enum` | `TokenType` enum in `SharpBasic.Ast` | ✅ Complete — merged to `main` |
| 3 | `feat/token-struct` | `Token` `readonly record struct` in `SharpBasic.Ast` | ✅ Complete — merged to `main` |
| 4 | `feat/lexer-print-keyword` | Lexer recognises `PRINT` keyword | ✅ Complete — merged to `main` |
| 5 | `feat/lexer-string-literal` | Lexer scans `"..."` into a `StringLiteral` token | ✅ Complete — merged to `main` |
| 6 | `feat/lexer-structure-tokens` | `NewLine`, `Eof`, `Unknown` + whitespace skipping | ✅ Complete — merged to `main` |
| 7 | `feat/repl-hello-world` | REPL wires it together — `PRINT "Hello, World!"` works | ✅ Complete — merged to `main` |

### Status Key
| Symbol | Meaning |
|--------|---------|
| ⬜ | Not started |
| 🔵 | In progress |
| ✅ | Complete — merged to `main` |

---

## Upcoming Phases

| Phase | Focus |
|-------|-------|
| 9.5 | Logical operators: AND, OR, NOT + unary minus |
| 10 | Standard Library & File Runner |
