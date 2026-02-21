# SharpBASIC

A modern BASIC-inspired programming language implemented as a tree-walking interpreter in C# .NET 10.

No line numbers. Structured, readable, and friendly — inspired by QBasic/FreeBASIC but modernised.

## Features

- Modern BASIC syntax without line numbers
- Tree-walking interpreter pipeline
- REPL and file runner
- Built with TDD and clean architecture principles

## Language Example

```basic
LET name = "Alice"
LET age = 30

IF age >= 18 THEN
    PRINT "Hello, " & name & "! You are an adult."
ELSE
    PRINT "Hello, " & name & "!"
END IF

FOR i = 1 TO 5
    PRINT i
NEXT i

FUNCTION Add(a AS INTEGER, b AS INTEGER) AS INTEGER
    RETURN a + b
END FUNCTION

PRINT Add(3, 4)
```

## Architecture

```
Source Text
    ↓
[ Lexer ]              → Token stream
    ↓
[ Parser ]             → Abstract Syntax Tree (AST)
    ↓
[ Semantic Analyser ]  → Annotated / validated AST
    ↓
[ Evaluator ]          → Executes the tree (tree-walking interpreter)
    ↓
[ REPL / File Runner ] → Console output
```

## Solution Structure

```
SharpBASIC/
├── src/
│   ├── SharpBasic.Ast/         ← AST node definitions (shared)
│   ├── SharpBasic.Lexer/       ← Tokeniser
│   ├── SharpBasic.Parser/      ← Recursive descent parser
│   ├── SharpBasic.Semantics/   ← Type checking, symbol resolution
│   ├── SharpBasic.Evaluator/   ← Tree-walking interpreter
│   └── SharpBasic.Repl/        ← Console app (REPL + file runner)
├── tests/
│   ├── SharpBasic.Lexer.Tests/
│   ├── SharpBasic.Parser.Tests/
│   ├── SharpBasic.Semantics.Tests/
│   └── SharpBasic.Evaluator.Tests/
└── spec/
    └── SharpBASIC — Project Plan & Learning Guide.md
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Getting Started

```bash
# Clone the repository
git clone https://github.com/yourusername/SharpBASIC.git
cd SharpBASIC

# Build the solution
dotnet build

# Run all tests
dotnet test

# Launch the REPL
dotnet run --project src/SharpBasic.Repl

# Run a .bas file
dotnet run --project src/SharpBasic.Repl -- path/to/program.bas
```

## Development

This project follows **Test-Driven Development (TDD)** — tests are written before implementation at every stage. Each pipeline stage is a separate project with its own test project, communicating only through well-defined types.

## License

MIT
