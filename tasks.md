# SharpBASIC — Task Tracker

Current phase and active task list. Update as you progress.

---

## Current Phase: 1 — Lexer & Hello World

**Goal:** `PRINT "Hello, World!"` runs in the REPL.

| # | Branch | Task | Status |
|---|--------|------|--------|
| 1 | `chore/init-git` | First commit — scaffold, README, .gitignore | ⬜ Not started |
| 2 | `feat/token-type-enum` | `TokenType` enum in `SharpBasic.Ast` | ⬜ Not started |
| 3 | `feat/token-struct` | `Token` `readonly record struct` in `SharpBasic.Ast` | ⬜ Not started |
| 4 | `feat/lexer-print-keyword` | Lexer recognises `PRINT` keyword | ⬜ Not started |
| 5 | `feat/lexer-string-literal` | Lexer scans `"..."` into a `StringLiteral` token | ⬜ Not started |
| 6 | `feat/lexer-structure-tokens` | `NewLine`, `Eof`, `Unknown` + whitespace skipping | ⬜ Not started |
| 7 | `feat/repl-hello-world` | REPL wires it together — `PRINT "Hello, World!"` works | ⬜ Not started |

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
| 2 | Parser + AST |
| 3 | Variables & Assignment |
| 4 | Expressions & Arithmetic (Pratt parsing) |
| 5 | IF / THEN / ELSE / END IF |
| 6 | FOR / NEXT and WHILE / WEND |
| 7 | Subroutines & Functions |
| 8 | Arrays |
| 9 | Error Handling & Diagnostics |
| 10 | Standard Library & Polish |
