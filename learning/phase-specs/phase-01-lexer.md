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
