# SharpBASIC — Project Plan & Learning Guide

> A guided journey to building a modern BASIC compiler in C# .NET 10  
> Following TDD and clean architecture principles throughout.

-----

## Context & Goals

You are building **SharpBASIC** — a modern BASIC-inspired programming language with no line numbers, implemented as a tree-walking interpreter in C# .NET 10. The project serves three goals simultaneously:

- **Deep learning** — understand how languages are really built (lexing, parsing, AST, evaluation)
- **Build something usable** — a real, runnable language with a REPL and file runner
- **Portfolio / career** — production-quality code, well tested, well documented

**Key constraints:**

- C# .NET 10, expert level — use modern idioms freely
- TDD throughout — tests written before implementation, always
- Clean pipeline architecture — each stage independently testable

-----

## Language Design: Modern BASIC

No line numbers. Structured, readable, friendly. Inspired by QBasic/FreeBASIC but modernised.

### Hello World

```basic
PRINT "Hello, World!"
```

### Variables

```basic
LET name = "Alice"
LET age = 30
LET pi = 3.14159
LET active = TRUE
```

### Arithmetic & Expressions

```basic
PRINT 2 + 2 * 3       ' respects precedence → 8
PRINT (2 + 2) * 3     ' parentheses → 12
LET result = 10 / 4   ' → 2.5
LET greeting = "Hello" & " " & "World"   ' string concat with &
```

### Conditionals

```basic
IF age >= 18 THEN
    PRINT "Adult"
ELSE
    PRINT "Minor"
END IF
```

### Loops

```basic
FOR i = 1 TO 10
    PRINT i
NEXT i

FOR i = 0 TO 100 STEP 5
    PRINT i
NEXT i

WHILE active
    PRINT "Running..."
    LET active = FALSE
WEND
```

### Subroutines & Functions

```basic
SUB Greet(name AS STRING)
    PRINT "Hello, " & name
END SUB

FUNCTION Add(a AS INTEGER, b AS INTEGER) AS INTEGER
    RETURN a + b
END FUNCTION

CALL Greet("Alice")
LET sum = Add(3, 4)
```

### Arrays

```basic
DIM scores(10) AS INTEGER
LET scores(0) = 95
PRINT scores(0)
```

### Built-in Functions (Phase 10)

```basic
PRINT LEN("Hello")       ' 5
PRINT MID$("Hello", 2, 3) ' ell
PRINT STR$(42)            ' "42"
PRINT INT(3.9)            ' 3
```

### Comments

```basic
' This is a comment
PRINT "Code"   ' inline comment
```

-----

## Architecture Overview

```
Source Text
    ↓
[ Lexer ]           → Token stream
    ↓
[ Parser ]          → Abstract Syntax Tree (AST)
    ↓
[ Semantic Analyser ] → Annotated / validated AST
    ↓
[ Evaluator ]       → Executes the tree (tree-walking interpreter)
    ↓
[ REPL / File Runner ] → Console output
```

Each stage is a separate C# project with its own test project. Stages communicate only through well-defined types — no coupling between non-adjacent stages.

-----

## Solution Structure

```
SharpBasic/
├── src/
│   ├── SharpBasic.Ast/            ← AST node definitions (shared)
│   ├── SharpBasic.Lexer/          ← Tokeniser
│   ├── SharpBasic.Parser/         ← Recursive descent parser
│   ├── SharpBasic.Semantics/      ← Type checking, symbol resolution
│   ├── SharpBasic.Evaluator/      ← Tree-walking interpreter
│   └── SharpBasic.Repl/           ← Console app (REPL + file runner)
├── tests/
│   ├── SharpBasic.Lexer.Tests/
│   ├── SharpBasic.Parser.Tests/
│   ├── SharpBasic.Ast.Tests/
│   ├── SharpBasic.Semantics.Tests/
│   └── SharpBasic.Evaluator.Tests/
├── samples/                        ← .bas sample programs
│   ├── hello.bas
│   ├── fibonacci.bas
│   └── calculator.bas
├── docs/
│   └── language-spec.md            ← Living language specification
└── SharpBasic.sln
```

-----

## Key Technical Decisions

|Concern             |Decision                               |Rationale                                              |
|--------------------|---------------------------------------|-------------------------------------------------------|
|AST nodes           |`abstract record` hierarchy            |Immutable, pattern-matchable, value equality for free  |
|Visitor dispatch    |`switch` expressions + pattern matching|Idiomatic C# 10+, avoids interface bloat               |
|Token type          |`readonly record struct`               |Zero allocation on the hot lexer path                  |
|Error model         |`Result<T>` / discriminated union      |No exception-driven control flow in compiler internals |
|Test framework      |xUnit + FluentAssertions               |Industry standard, expressive assertions               |
|NuGet extras        |None required initially                |Keep dependencies minimal                              |
|Interpreter strategy|Tree-walking first                     |Ship fast, optimise later (could add bytecode VM later)|

-----

## The 10-Phase Roadmap

Every phase follows this TDD loop:

```
1. Design the feature (update language-spec.md)
2. Write failing tests (Red)
3. Write minimum implementation (Green)
4. Refactor to clean code
5. Demo it working end-to-end
```

-----

### Phase 1 — Lexer & Hello World *(Start here)*

**Goal:** `PRINT "Hello, World!"` runs in the REPL.

**What you’ll build:**

- `Token` type (`readonly record struct`)
- `TokenType` enum
- `Lexer` class — scans characters, emits tokens
- `Repl` — reads a line, lexes it, (for now) just prints tokens, then evaluates PRINT

**Tokens to support:**

- `PRINT` keyword
- String literals `"..."`
- Newline / EOF
- Whitespace (skipped)

**Test examples:**

```csharp
// Lexer.Tests
[Fact]
public void Lex_PrintKeyword_ReturnsCorrectToken()
{
    var lexer = new Lexer("PRINT");
    var tokens = lexer.Tokenise();
    tokens.Should().ContainSingle(t => t.Type == TokenType.Print);
}

[Fact]
public void Lex_StringLiteral_ReturnsStringToken()
{
    var lexer = new Lexer("\"Hello, World!\"");
    var tokens = lexer.Tokenise();
    tokens.Should().ContainSingle(t => t.Type == TokenType.StringLiteral && t.Value == "Hello, World!");
}

[Fact]
public void Lex_PrintStatement_ReturnsCorrectSequence()
{
    var lexer = new Lexer("PRINT \"Hello, World!\"");
    var tokens = lexer.Tokenise().Where(t => t.Type != TokenType.Whitespace).ToList();
    tokens[0].Type.Should().Be(TokenType.Print);
    tokens[1].Type.Should().Be(TokenType.StringLiteral);
}
```

**Key types:**

```csharp
public enum TokenType { Print, StringLiteral, NewLine, Eof, Unknown }

public readonly record struct Token(TokenType Type, string Value, int Line, int Column);
```

-----

### Phase 2 — Parser & AST

**Goal:** Parse `PRINT "Hello"` into a proper AST.

**What you’ll build:**

- AST node hierarchy using `abstract record`
- `PrintStatement` node
- `StringLiteralExpression` node
- `Parser` class — recursive descent, consumes tokens, returns AST

**Key types:**

```csharp
public abstract record AstNode;
public abstract record Statement : AstNode;
public abstract record Expression : AstNode;

public record PrintStatement(Expression Value) : Statement;
public record StringLiteralExpression(string Value) : Expression;
public record Program(IReadOnlyList<Statement> Statements) : AstNode;
```

**Test examples:**

```csharp
[Fact]
public void Parse_PrintString_ReturnsPrintStatement()
{
    var tokens = new Lexer("PRINT \"Hello\"").Tokenise();
    var parser = new Parser(tokens);
    var program = parser.Parse();

    program.Statements.Should().HaveCount(1);
    program.Statements[0].Should().BeOfType<PrintStatement>()
        .Which.Value.Should().BeOfType<StringLiteralExpression>()
        .Which.Value.Should().Be("Hello");
}
```

-----

### Phase 3 — Variables & Assignment

**Goal:** `LET name = "Alice"` then `PRINT name` works.

**What you’ll build:**

- `LetStatement` AST node
- `IdentifierExpression` AST node
- `SymbolTable` (dictionary of variable names → values)
- Type enum: `String`, `Integer`, `Float`, `Boolean`
- Evaluator begins — walks the AST and executes

**Test examples:**

```csharp
[Fact]
public void Evaluate_LetThenPrint_OutputsValue()
{
    var output = Run("LET x = \"World\"\nPRINT x");
    output.Should().Be("World");
}
```

-----

### Phase 4 — Expressions & Arithmetic

**Goal:** `PRINT 2 + 2 * 3` outputs `8`. Operator precedence correct.

**What you’ll build:**

- `IntegerLiteralExpression`, `FloatLiteralExpression`
- `BinaryExpression(Left, Operator, Right)` AST node
- Pratt parser (top-down operator precedence) for expressions
- Arithmetic: `+`, `-`, `*`, `/`, `MOD`
- String concat: `&`
- Unary: `-` (negation)

**Key concept — Pratt parsing:**  
Assign each operator a binding power (precedence). `*` and `/` bind tighter than `+` and `-`. The parser recurses based on these powers — elegant and extensible.

-----

### Phase 5 — Control Flow: IF / THEN / ELSE / END IF

**Goal:** Structured conditionals work, including nesting.

**What you’ll build:**

- `IfStatement(Condition, ThenBlock, ElseBlock?)` AST node
- Boolean operators: `=`, `<>`, `<`, `>`, `<=`, `>=`
- Logical: `AND`, `OR`, `NOT`
- Block parsing — read statements until `END IF` / `ELSE`

**Test examples:**

```csharp
[Fact]
public void Evaluate_IfTrue_ExecutesThenBlock()
{
    var output = Run("IF 1 = 1 THEN\n  PRINT \"yes\"\nEND IF");
    output.Should().Be("yes");
}

[Fact]
public void Evaluate_IfFalse_ExecutesElseBlock()
{
    var output = Run("IF 1 = 2 THEN\n  PRINT \"yes\"\nELSE\n  PRINT \"no\"\nEND IF");
    output.Should().Be("no");
}
```

-----

### Phase 6 — Loops: FOR / NEXT and WHILE / WEND

**Goal:** Both loop types work, including STEP.

**What you’ll build:**

- `ForStatement(Variable, From, To, Step?, Body)` AST node
- `WhileStatement(Condition, Body)` AST node
- Loop variable scoped correctly
- Infinite loop guard (max iterations for safety in REPL)

-----

### Phase 7 — Subroutines & Functions

**Goal:** Define and call SUBs and FUNCTIONs. Recursion works.

**What you’ll build:**

- `SubDeclaration` and `FunctionDeclaration` AST nodes
- `CallStatement` and `CallExpression` nodes
- `ReturnStatement` node
- Parameter list with typed parameters (`name AS TYPE`)
- Call stack — new scope per call
- Return value plumbing

**Test examples:**

```csharp
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
```

-----

### Phase 8 — Arrays

**Goal:** `DIM`, indexing, and bounds checking work.

**What you’ll build:**

- `DimStatement(Name, Size, Type)` AST node
- `ArrayAccessExpression` and `ArrayAssignStatement`
- Runtime array storage
- Bounds checking with descriptive errors

-----

### Phase 9 — Error Handling & Diagnostics

**Goal:** Errors are clear, located (line/col), and structured.

**What you’ll build:**

- `Diagnostic` type with line, column, message, severity
- `CompilerError` / `RuntimeError` hierarchy
- `Result<T, IReadOnlyList<Diagnostic>>` return type throughout
- Error recovery in parser (continue after error, collect multiple)
- `ON ERROR GOTO` / `ON ERROR RESUME NEXT` in language

**Example error messages:**

```
[Line 3, Col 7] Error: Undefined variable 'nme'. Did you mean 'name'?
[Line 5, Col 1] Error: Type mismatch — expected INTEGER, got STRING.
[Line 8, Col 12] Error: Array index 15 out of bounds (size 10).
```

-----

### Phase 10 — Standard Library & Polish

**Goal:** A usable, complete language. Ship it.

**What you’ll build:**

- Built-in functions: `LEN`, `MID$`, `LEFT$`, `RIGHT$`, `UPPER$`, `LOWER$`, `INT`, `STR$`, `VAL`, `ABS`, `SQR`, `RND`
- `INPUT` statement — read from console
- File runner: `sharpbasic run hello.bas`
- REPL improvements: history, multi-line input, `.exit` command
- `samples/` directory with working example programs

-----

## Stretch Goals (Post Phase 10)

Once the core is solid, natural next steps:

- **Bytecode VM** — compile AST to bytecode, execute on a stack VM (major performance gain)
- **Compile to IL** — use `System.Reflection.Emit` to generate real .NET bytecode
- **VS Code Extension** — syntax highlighting, error squiggles via Language Server Protocol
- **Module system** — `IMPORT "utils.bas"` to split programs across files
- **Transpile to C#** — emit C# source from the AST

-----

## Prerequisites & Setup

### Required Tools

```
.NET 10 SDK          https://dotnet.microsoft.com/download
Git                  https://git-scm.com
VS Code or Rider     (Rider recommended for C# expert)
```

### VS Code Extensions (if using VS Code)

```
C# Dev Kit
C# (OmniSharp)
xUnit Test Explorer
```

### Creating the Solution

Run these commands to scaffold the full solution from scratch:

```bash
# Create root
mkdir SharpBasic && cd SharpBasic
git init

# Solution file
dotnet new sln -n SharpBasic

# Source projects
dotnet new classlib -n SharpBasic.Ast       -o src/SharpBasic.Ast       --framework net10.0
dotnet new classlib -n SharpBasic.Lexer     -o src/SharpBasic.Lexer     --framework net10.0
dotnet new classlib -n SharpBasic.Parser    -o src/SharpBasic.Parser    --framework net10.0
dotnet new classlib -n SharpBasic.Semantics -o src/SharpBasic.Semantics --framework net10.0
dotnet new classlib -n SharpBasic.Evaluator -o src/SharpBasic.Evaluator --framework net10.0
dotnet new console  -n SharpBasic.Repl      -o src/SharpBasic.Repl      --framework net10.0

# Test projects
dotnet new xunit -n SharpBasic.Lexer.Tests     -o tests/SharpBasic.Lexer.Tests     --framework net10.0
dotnet new xunit -n SharpBasic.Parser.Tests    -o tests/SharpBasic.Parser.Tests    --framework net10.0
dotnet new xunit -n SharpBasic.Ast.Tests       -o tests/SharpBasic.Ast.Tests       --framework net10.0
dotnet new xunit -n SharpBasic.Semantics.Tests -o tests/SharpBasic.Semantics.Tests --framework net10.0
dotnet new xunit -n SharpBasic.Evaluator.Tests -o tests/SharpBasic.Evaluator.Tests --framework net10.0

# Add all projects to solution
dotnet sln add src/SharpBasic.Ast/SharpBasic.Ast.csproj
dotnet sln add src/SharpBasic.Lexer/SharpBasic.Lexer.csproj
dotnet sln add src/SharpBasic.Parser/SharpBasic.Parser.csproj
dotnet sln add src/SharpBasic.Semantics/SharpBasic.Semantics.csproj
dotnet sln add src/SharpBasic.Evaluator/SharpBasic.Evaluator.csproj
dotnet sln add src/SharpBasic.Repl/SharpBasic.Repl.csproj
dotnet sln add tests/SharpBasic.Lexer.Tests/SharpBasic.Lexer.Tests.csproj
dotnet sln add tests/SharpBasic.Parser.Tests/SharpBasic.Parser.Tests.csproj
dotnet sln add tests/SharpBasic.Ast.Tests/SharpBasic.Ast.Tests.csproj
dotnet sln add tests/SharpBasic.Semantics.Tests/SharpBasic.Semantics.Tests.csproj
dotnet sln add tests/SharpBasic.Evaluator.Tests/SharpBasic.Evaluator.Tests.csproj

# Add project references (dependency chain)
dotnet add src/SharpBasic.Lexer/SharpBasic.Lexer.csproj reference src/SharpBasic.Ast/SharpBasic.Ast.csproj
dotnet add src/SharpBasic.Parser/SharpBasic.Parser.csproj reference src/SharpBasic.Ast/SharpBasic.Ast.csproj
dotnet add src/SharpBasic.Parser/SharpBasic.Parser.csproj reference src/SharpBasic.Lexer/SharpBasic.Lexer.csproj
dotnet add src/SharpBasic.Semantics/SharpBasic.Semantics.csproj reference src/SharpBasic.Ast/SharpBasic.Ast.csproj
dotnet add src/SharpBasic.Evaluator/SharpBasic.Evaluator.csproj reference src/SharpBasic.Ast/SharpBasic.Ast.csproj
dotnet add src/SharpBasic.Evaluator/SharpBasic.Evaluator.csproj reference src/SharpBasic.Semantics/SharpBasic.Semantics.csproj
dotnet add src/SharpBasic.Repl/SharpBasic.Repl.csproj reference src/SharpBasic.Lexer/SharpBasic.Lexer.csproj
dotnet add src/SharpBasic.Repl/SharpBasic.Repl.csproj reference src/SharpBasic.Parser/SharpBasic.Parser.csproj
dotnet add src/SharpBasic.Repl/SharpBasic.Repl.csproj reference src/SharpBasic.Evaluator/SharpBasic.Evaluator.csproj

# Add FluentAssertions to all test projects
dotnet add tests/SharpBasic.Lexer.Tests/SharpBasic.Lexer.Tests.csproj package FluentAssertions
dotnet add tests/SharpBasic.Parser.Tests/SharpBasic.Parser.Tests.csproj package FluentAssertions
dotnet add tests/SharpBasic.Evaluator.Tests/SharpBasic.Evaluator.Tests.csproj package FluentAssertions
dotnet add tests/SharpBasic.Semantics.Tests/SharpBasic.Semantics.Tests.csproj package FluentAssertions

# Add test project references to src projects
dotnet add tests/SharpBasic.Lexer.Tests/SharpBasic.Lexer.Tests.csproj reference src/SharpBasic.Lexer/SharpBasic.Lexer.csproj
dotnet add tests/SharpBasic.Parser.Tests/SharpBasic.Parser.Tests.csproj reference src/SharpBasic.Parser/SharpBasic.Parser.csproj
dotnet add tests/SharpBasic.Evaluator.Tests/SharpBasic.Evaluator.Tests.csproj reference src/SharpBasic.Evaluator/SharpBasic.Evaluator.csproj
dotnet add tests/SharpBasic.Semantics.Tests/SharpBasic.Semantics.Tests.csproj reference src/SharpBasic.Semantics/SharpBasic.Semantics.csproj

# Create folder structure
mkdir -p samples docs

# Verify everything builds
dotnet build
dotnet test
```

-----

## Phase 1 — Starter Code

Once the solution is scaffolded, here is the starter code to get Phase 1 green.

### `SharpBasic.Ast` — Token Types

```csharp
// src/SharpBasic.Ast/TokenType.cs
namespace SharpBasic.Ast;

public enum TokenType
{
    // Literals
    StringLiteral,
    IntegerLiteral,
    FloatLiteral,

    // Keywords
    Print,
    Let,
    If, Then, Else, EndIf,
    For, To, Step, Next,
    While, Wend,
    Sub, EndSub,
    Function, EndFunction,
    Return,
    Dim,
    As,
    Call,
    And, Or, Not,
    True, False,

    // Types
    String, Integer, Float, Boolean,

    // Operators
    Plus, Minus, Star, Slash, Mod,
    Ampersand,          // & (string concat)
    Equals,             // =
    NotEquals,          // <>
    LessThan,           // <
    GreaterThan,        // >
    LessThanOrEqual,    // <=
    GreaterThanOrEqual, // >=

    // Punctuation
    LeftParen, RightParen,
    Comma,

    // Structure
    Identifier,
    NewLine,
    Eof,
    Unknown
}
```

```csharp
// src/SharpBasic.Ast/Token.cs
namespace SharpBasic.Ast;

public readonly record struct Token(
    TokenType Type,
    string Value,
    int Line,
    int Column
)
{
    public static Token Eof(int line, int col) => new(TokenType.Eof, "", line, col);
    public override string ToString() => $"[{Type} '{Value}' {Line}:{Column}]";
}
```

### `SharpBasic.Lexer` — The Lexer

```csharp
// src/SharpBasic.Lexer/Lexer.cs
using SharpBasic.Ast;

namespace SharpBasic.Lexer;

public sealed class Lexer
{
    private readonly string _source;
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["PRINT"]    = TokenType.Print,
        ["LET"]      = TokenType.Let,
        ["IF"]       = TokenType.If,
        ["THEN"]     = TokenType.Then,
        ["ELSE"]     = TokenType.Else,
        ["END"]      = TokenType.Unknown,   // context-sensitive, handled in parser
        ["FOR"]      = TokenType.For,
        ["TO"]       = TokenType.To,
        ["STEP"]     = TokenType.Step,
        ["NEXT"]     = TokenType.Next,
        ["WHILE"]    = TokenType.While,
        ["WEND"]     = TokenType.Wend,
        ["SUB"]      = TokenType.Sub,
        ["FUNCTION"] = TokenType.Function,
        ["RETURN"]   = TokenType.Return,
        ["DIM"]      = TokenType.Dim,
        ["AS"]       = TokenType.As,
        ["CALL"]     = TokenType.Call,
        ["AND"]      = TokenType.And,
        ["OR"]       = TokenType.Or,
        ["NOT"]      = TokenType.Not,
        ["TRUE"]     = TokenType.True,
        ["FALSE"]    = TokenType.False,
        ["MOD"]      = TokenType.Mod,
        ["INTEGER"]  = TokenType.Integer,
        ["STRING"]   = TokenType.String,
        ["FLOAT"]    = TokenType.Float,
        ["BOOLEAN"]  = TokenType.Boolean,
    };

    public Lexer(string source) => _source = source;

    public IReadOnlyList<Token> Tokenise()
    {
        var tokens = new List<Token>();
        while (true)
        {
            var token = NextToken();
            tokens.Add(token);
            if (token.Type == TokenType.Eof) break;
        }
        return tokens;
    }

    private Token NextToken()
    {
        SkipWhitespace();
        if (_pos >= _source.Length) return Token.Eof(_line, _col);

        char c = Current();

        if (c == '\'') { SkipComment(); return NextToken(); }
        if (c == '\n') return ConsumeNewLine();
        if (c == '"')  return ConsumeString();
        if (char.IsLetter(c) || c == '_') return ConsumeIdentifierOrKeyword();
        if (char.IsDigit(c)) return ConsumeNumber();

        return ConsumePunctuation();
    }

    private Token ConsumeNewLine()
    {
        var tok = new Token(TokenType.NewLine, "\\n", _line, _col);
        _pos++; _line++; _col = 1;
        return tok;
    }

    private Token ConsumeString()
    {
        int startCol = _col;
        _pos++; _col++; // skip opening "
        var sb = new System.Text.StringBuilder();
        while (_pos < _source.Length && Current() != '"')
        {
            sb.Append(Current());
            _pos++; _col++;
        }
        _pos++; _col++; // skip closing "
        return new Token(TokenType.StringLiteral, sb.ToString(), _line, startCol);
    }

    private Token ConsumeIdentifierOrKeyword()
    {
        int start = _pos, startCol = _col;
        while (_pos < _source.Length && (char.IsLetterOrDigit(Current()) || Current() == '_' || Current() == '$'))
        {
            _pos++; _col++;
        }
        var text = _source[start.._pos];
        var type = Keywords.TryGetValue(text.TrimEnd('$'), out var kw) ? kw : TokenType.Identifier;
        return new Token(type, text, _line, startCol);
    }

    private Token ConsumeNumber()
    {
        int start = _pos, startCol = _col;
        bool isFloat = false;
        while (_pos < _source.Length && (char.IsDigit(Current()) || Current() == '.'))
        {
            if (Current() == '.') isFloat = true;
            _pos++; _col++;
        }
        var text = _source[start.._pos];
        var type = isFloat ? TokenType.FloatLiteral : TokenType.IntegerLiteral;
        return new Token(type, text, _line, startCol);
    }

    private Token ConsumePunctuation()
    {
        int startCol = _col;
        char c = Current();
        _pos++; _col++;

        return c switch
        {
            '+' => new Token(TokenType.Plus,        "+", _line, startCol),
            '-' => new Token(TokenType.Minus,       "-", _line, startCol),
            '*' => new Token(TokenType.Star,        "*", _line, startCol),
            '/' => new Token(TokenType.Slash,       "/", _line, startCol),
            '&' => new Token(TokenType.Ampersand,   "&", _line, startCol),
            '(' => new Token(TokenType.LeftParen,   "(", _line, startCol),
            ')' => new Token(TokenType.RightParen,  ")", _line, startCol),
            ',' => new Token(TokenType.Comma,       ",", _line, startCol),
            '=' => new Token(TokenType.Equals,      "=", _line, startCol),
            '<' when Peek() == '>' => Advance(new Token(TokenType.NotEquals,          "<>", _line, startCol)),
            '<' when Peek() == '=' => Advance(new Token(TokenType.LessThanOrEqual,    "<=", _line, startCol)),
            '>' when Peek() == '=' => Advance(new Token(TokenType.GreaterThanOrEqual, ">=", _line, startCol)),
            '<' => new Token(TokenType.LessThan,    "<", _line, startCol),
            '>' => new Token(TokenType.GreaterThan, ">", _line, startCol),
            _   => new Token(TokenType.Unknown, c.ToString(), _line, startCol)
        };
    }

    private Token Advance(Token t) { _pos++; _col++; return t; }
    private void SkipWhitespace() { while (_pos < _source.Length && Current() is ' ' or '\t' or '\r') { _pos++; _col++; } }
    private void SkipComment()    { while (_pos < _source.Length && Current() != '\n') { _pos++; _col++; } }
    private char Current() => _source[_pos];
    private char Peek()    => _pos + 1 < _source.Length ? _source[_pos + 1] : '\0';
}
```

### `SharpBasic.Lexer.Tests` — First Test Suite

```csharp
// tests/SharpBasic.Lexer.Tests/LexerTests.cs
using FluentAssertions;
using SharpBasic.Ast;
using SharpBasic.Lexer;

namespace SharpBasic.Lexer.Tests;

public class LexerTests
{
    private static IReadOnlyList<Token> Lex(string source) =>
        new Lexer(source).Tokenise().Where(t => t.Type != TokenType.NewLine).ToList();

    [Fact]
    public void Lex_EmptyString_ReturnsOnlyEof()
    {
        var tokens = Lex("");
        tokens.Should().ContainSingle(t => t.Type == TokenType.Eof);
    }

    [Fact]
    public void Lex_PrintKeyword_ReturnsCorrectToken()
    {
        var tokens = Lex("PRINT");
        tokens.First().Type.Should().Be(TokenType.Print);
    }

    [Fact]
    public void Lex_Keywords_AreCaseInsensitive()
    {
        Lex("print").First().Type.Should().Be(TokenType.Print);
        Lex("Print").First().Type.Should().Be(TokenType.Print);
        Lex("PRINT").First().Type.Should().Be(TokenType.Print);
    }

    [Fact]
    public void Lex_StringLiteral_ReturnsValueWithoutQuotes()
    {
        var tokens = Lex("\"Hello, World!\"");
        tokens.First().Should().Match<Token>(t =>
            t.Type == TokenType.StringLiteral && t.Value == "Hello, World!");
    }

    [Fact]
    public void Lex_PrintStatement_ReturnsCorrectSequence()
    {
        var tokens = Lex("PRINT \"Hello, World!\"");
        tokens[0].Type.Should().Be(TokenType.Print);
        tokens[1].Type.Should().Be(TokenType.StringLiteral);
        tokens[1].Value.Should().Be("Hello, World!");
        tokens[2].Type.Should().Be(TokenType.Eof);
    }

    [Fact]
    public void Lex_IntegerLiteral_ReturnsCorrectValue()
    {
        var tokens = Lex("42");
        tokens.First().Should().Match<Token>(t =>
            t.Type == TokenType.IntegerLiteral && t.Value == "42");
    }

    [Fact]
    public void Lex_FloatLiteral_ReturnsCorrectValue()
    {
        var tokens = Lex("3.14");
        tokens.First().Should().Match<Token>(t =>
            t.Type == TokenType.FloatLiteral && t.Value == "3.14");
    }

    [Fact]
    public void Lex_Comment_IsIgnored()
    {
        var tokens = Lex("' this is a comment");
        tokens.Should().ContainSingle(t => t.Type == TokenType.Eof);
    }

    [Fact]
    public void Lex_InlineComment_IsIgnored()
    {
        var tokens = Lex("PRINT \"Hi\" ' comment");
        tokens.Should().NotContain(t => t.Value.Contains("comment"));
    }

    [Fact]
    public void Lex_Arithmetic_ReturnsOperatorTokens()
    {
        var tokens = Lex("2 + 3 * 4");
        tokens.Select(t => t.Type).Should().Contain(new[]
        {
            TokenType.IntegerLiteral,
            TokenType.Plus,
            TokenType.IntegerLiteral,
            TokenType.Star,
            TokenType.IntegerLiteral
        });
    }

    [Fact]
    public void Lex_ComparisonOperators_Recognized()
    {
        Lex("<>").First().Type.Should().Be(TokenType.NotEquals);
        Lex("<=").First().Type.Should().Be(TokenType.LessThanOrEqual);
        Lex(">=").First().Type.Should().Be(TokenType.GreaterThanOrEqual);
    }

    [Fact]
    public void Lex_TokensHaveCorrectLineAndColumn()
    {
        var tokens = new Lexer("PRINT \"Hi\"").Tokenise();
        tokens[0].Line.Should().Be(1);
        tokens[0].Column.Should().Be(1);
        tokens[1].Column.Should().Be(7);
    }
}
```

### `SharpBasic.Repl` — Minimal REPL to run Hello World

```csharp
// src/SharpBasic.Repl/Program.cs
using SharpBasic.Ast;
using SharpBasic.Lexer;

Console.WriteLine("SharpBASIC v0.1 — Phase 1");
Console.WriteLine("Type PRINT \"message\" to test. Type EXIT to quit.");
Console.WriteLine();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line is null || line.Equals("EXIT", StringComparison.OrdinalIgnoreCase)) break;

    try
    {
        var tokens = new Lexer(line).Tokenise()
            .Where(t => t.Type is not TokenType.NewLine and not TokenType.Eof)
            .ToList();

        // Phase 1: only handle PRINT "literal"
        if (tokens.Count >= 2 && tokens[0].Type == TokenType.Print && tokens[1].Type == TokenType.StringLiteral)
        {
            Console.WriteLine(tokens[1].Value);
        }
        else
        {
            Console.WriteLine($"[Phase 1] Tokens: {string.Join(", ", tokens.Select(t => t.ToString()))}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
```

-----

## Continuing the Journey

When you resume this project on another machine — after running the scaffold commands and adding the starter code — your first task is:

```bash
dotnet test   # all Phase 1 tests should pass
dotnet run --project src/SharpBasic.Repl
# > PRINT "Hello, World!"
# Hello, World!
```

Then move to Phase 2: build the Parser and AST so PRINT is properly parsed into a tree rather than handled ad-hoc in the REPL.

### Recommended Resources

- **“Crafting Interpreters”** by Robert Nystrom — free online at craftinginterpreters.com — the single best resource for this project
- **“Writing An Interpreter In Go”** by Thorsten Ball — excellent on Pratt parsing (Phase 4)
- **Roslyn source code** — github.com/dotnet/roslyn — real-world inspiration for error handling and AST design
- **BASIC language reference** — pick any original BASIC spec for vocabulary inspiration

-----

*Generated as part of a guided learning journey. Resume anytime — the plan is self-contained.*