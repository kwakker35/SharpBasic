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
