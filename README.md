[![Build](https://github.com/kwakker35/SharpBasic/actions/workflows/build.yml/badge.svg)](https://github.com/kwakker35/SharpBasic/actions/workflows/build.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# SharpBASIC

A BASIC-inspired interpreter written in C# .NET 10 — built from scratch, phase by phase, as a deliberate study of language theory and compiler construction.

```
REM Fibonacci — recursive FUNCTION, demonstrating call stacks

FUNCTION Fibonacci(n AS INTEGER) AS INTEGER
    IF n <= 1 THEN
        RETURN n
    END IF
    RETURN Fibonacci(n - 1) + Fibonacci(n - 2)
END FUNCTION

PRINT "=== Fibonacci Sequence ==="
PRINT ""

FOR i = 0 TO 15
    PRINT "F(" & STR$(i) & ") = " & STR$(Fibonacci(i))
NEXT i

PRINT ""
PRINT "Classic check: F(10) = " & STR$(Fibonacci(10))
```

Recursive functions, FOR loops, and string formatting in one self-contained program. Run it with:

    sharpbasic samples/fibonacci/fibonacci.sbx

---

## What SharpBASIC is

SharpBASIC is a structured, line-oriented interpreted language with no line numbers and no GOTO. It has four primitive types, typed arrays, lexically scoped subroutines and functions, and a full set of built-in string and numeric functions — running through a clean pipeline of Lexer, Parser, AST, and tree-walking Evaluator, all written in C# .NET 10.

What makes it interesting beyond the language itself is how it was built: every design decision, every bug discovered, and every architectural trade-off is documented in the repo. It was constructed phase by phase using test-driven development and an AI-assisted workflow, and the full learning materials — phase specifications, theory notes, and setup instructions — are included so that anyone can follow the same path from scratch.

---

## Quick start

**Prerequisites:**

- .NET 10 SDK
- PowerShell (Windows, or PowerShell Core on Mac/Linux)
- VS Code (optional, for syntax highlighting)

**Install:**

```powershell
git clone https://github.com/kwakker35/SharpBasic
cd SharpBasic
./install.ps1
```

`install.ps1` builds and packs the interpreter as a versioned NuGet package, then installs it as a global .NET tool so `sharpbasic` is available system-wide.

**Run a sample:**

    sharpbasic samples/hello-world/hello-world.sbx

**REPL:**

    sharpbasic

**VS Code syntax highlighting (optional):**

Install the extension from the `vscode-extension/sharpbasic-syntax/` folder. In VS Code: Extensions panel → `...` menu → Install from VSIX → select `sharpbasic-syntax-0.4.0.vsix`.

---

## Language features

| Feature | Detail |
|---|---|
| Data types | `Integer` (32-bit), `Float` (64-bit), `String`, `Boolean`, typed `Array<T>` |
| Variables | `LET` for declaration and assignment; lexical scope — reads walk the parent chain, writes are local only |
| Arrays | `DIM name[size] AS type`; 0-indexed; zero-initialised on declaration; bounds checked at runtime; 2D: `DIM name[rows][cols] AS type`, indexed with `name[r][c]` |
| Operators | Arithmetic (`+`, `-`, `*`, `/`, `MOD`); string concatenation (`&`); comparison (`=`, `<>`, `<`, `>`, `<=`, `>=`); logical (`AND`, `OR`); unary (`-`, `NOT`) |
| Control flow | `IF`/`THEN`/`ELSE`/`END IF`; `FOR`/`NEXT`/`STEP` (positive and negative step); `WHILE`/`WEND`; `SELECT CASE`/`CASE`/`END SELECT` |
| Constants | `CONST name = value`; named compile-time constants, scoped like variables |
| Global scope | `SET GLOBAL name = value`; writes a value to a variable in an enclosing scope by name |
| Subroutines | `SUB`/`END SUB`; typed parameters; invoked with `CALL`; hoisted — call before declaration is valid |
| Functions | `FUNCTION`/`END FUNCTION`; typed return value; called in expression position; recursive; hoisted |
| Built-in functions | `LEN`, `MID$`, `LEFT$`, `RIGHT$`, `UPPER$`, `LOWER$`, `TRIM$`, `STR$`, `VAL`, `INT`, `CINT`, `ABS`, `SQR`, `RND`, `CHR$`, `STRING$`, `ASC`, `MAX`, `MIN`, `CLAMP`, `TYPENAME` |
| I/O | `PRINT expression` (always appends newline); `INPUT varname` and `INPUT "prompt"; varname` |
| Comments | `REM` — full-line or inline |

---

## Sample programs

| File | Description |
|---|---|
| [hello-world.sbx](samples/hello-world/hello-world.sbx) | The minimal SharpBASIC program — a single PRINT statement |
| [user-input.sbx](samples/user-input/user-input.sbx) | Reads the user's name with INPUT and prints a greeting |
| [variables.sbx](samples/variables/variables.sbx) | Demonstrates all four primitive types and the TYPENAME built-in |
| [string-utilities.sbx](samples/string-utilities/string-utilities.sbx) | Exercises every built-in string function: `TRIM$`, `UPPER$`, `LOWER$`, `LEFT$`, `RIGHT$`, `MID$`, `LEN`, `CHR$`, `ASC`, `STRING$` |
| [fizzbuzz.sbx](samples/fizzbuzz/fizzbuzz.sbx) | Classic FOR loop with nested IF/ELSE — Fizz, Buzz, FizzBuzz for 1–30 |
| [times-tables.sbx](samples/times-tables/times-tables.sbx) | Nested FOR loops generating the complete 1–12 multiplication table |
| [arrays.sbx](samples/arrays/arrays.sbx) | DIM, indexed read/write, and aggregate calculations (total, average, maximum) over a score array |
| [fibonacci.sbx](samples/fibonacci/fibonacci.sbx) | Recursive FUNCTION computing the Fibonacci sequence from F(0) to F(15) |
| [calculator.sbx](samples/calculator/calculator.sbx) | Four-function calculator using INPUT, VAL, and SELECT CASE |
| [guess-the-number.sbx](samples/guess-the-number/guess-the-number.sbx) | Number guessing game using WHILE, RND, and higher/lower feedback |
| [tic-tac-toe.sbx](samples/tic-tac-toe/tic-tac-toe.sbx) | Two-player ASCII Tic-Tac-Toe with a flat array board, SUBs for drawing, and a FUNCTION for win detection |
| [hangman.sbx](samples/hangman/hangman.sbx) | Word guessing game with a letter-tracking array, ASCII scaffold drawing SUB, and recursive-position display FUNCTION |
| [wordle.sbx](samples/wordle/wordle.sbx) | Six-attempt Wordle clone with green/yellow/grey scoring logic implemented in FUNCTIONs and a display SUB |
| [the-sunken-crown.sbx](samples/the-sunken-crown/the-sunken-crown.sbx) | Fighting Fantasy-inspired text adventure — dungeon crawler with combat, inventory, and parallel array state management |

---

## Learning materials

SharpBASIC was built as a structured learning project: ten phases, TDD throughout, with every design decision documented. The materials below accompany the source code and are written for anyone who wants to build the same interpreter from scratch, or understand why it was built the way it was.

- [Learning system overview](learning/README.md)
- [Phase specifications](learning/phase-specs/) — 10 phases, each a self-contained build spec
- [Architecture decisions](learning/decisions/architecture-decisions.md)
- [Theory notes](learning/theory/) — Pratt parsing, call frames, pitfalls
- [AI setup](learning/ai-setup/) — how to recreate the AI-assisted learning experience
- [Language specification](docs/language-spec-v1.md) — the definitive v1 language reference

---

## Project structure

```
SharpBasic/
├── src/                        ← Interpreter source — Lexer, Parser, AST, Evaluator, REPL
├── tests/                      ← xUnit test suite, one project per interpreter stage
├── samples/                    ← Working .sbx example programs
├── docs/                       ← Language reference (v1), lessons learned, build notes
├── learning/                   ← Phase specs, theory notes, AI setup
├── vscode-extension/           ← SharpBASIC syntax highlighting for VS Code
├── .github/                    ← GitHub Actions workflows
├── install.ps1                 ← Build and install as global .NET tool
├── SharpBasic.sln              ← Solution file
├── LICENSE                     ← MIT
└── README.md                   ← This file
```

---

## Contributing

Pull requests are welcome. Before contributing to the interpreter itself, read [learning/decisions/architecture-decisions.md](learning/decisions/architecture-decisions.md) — the key decisions (abstract record AST nodes, discriminated-union result types, Pratt parsing, pass-by-value call frames) are deliberate and not open for revision. The v1 language spec is fixed; the right scope for contributions is learning materials, sample programs, and bug fixes in the existing implementation. If you are unsure whether a change fits, open an issue first.

---

## License

MIT — see [LICENSE](LICENSE)
