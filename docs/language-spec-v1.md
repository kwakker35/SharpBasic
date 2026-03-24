# SharpBASIC Language Reference v1

> Derived from the SharpBASIC interpreter source code (Lexer, Parser, Evaluator, AST).  
> Only features that are implemented are documented here.

---

## Table of Contents

1. [Overview](#1-overview)
2. [Lexical Rules](#2-lexical-rules)
   - 2.1 [Character Set](#21-character-set)
   - 2.2 [Case Sensitivity](#22-case-sensitivity)
   - 2.3 [Whitespace and Newlines](#23-whitespace-and-newlines)
   - 2.4 [Comments](#24-comments)
   - 2.5 [Line Continuation](#25-line-continuation)
   - 2.6 [Operator Spacing](#26-operator-spacing)
3. [Data Types](#3-data-types)
4. [Variables](#4-variables)
   - 4.1 [Declaration and Assignment](#41-declaration-and-assignment)
   - 4.2 [Scope Rules](#42-scope-rules)
   - 4.3 [Constants](#43-constants)
   - 4.4 [SET GLOBAL](#44-set-global)
5. [Arrays](#5-arrays)
   - 5.1 [Declaration](#51-declaration)
   - 5.2 [Access and Assignment](#52-access-and-assignment)
   - 5.3 [Bounds Behaviour](#53-bounds-behaviour)
   - 5.4 [Two-Dimensional Arrays](#54-two-dimensional-arrays)
6. [Operators](#6-operators)
   - 6.1 [Arithmetic Operators](#61-arithmetic-operators)
   - 6.2 [String Operator](#62-string-operator)
   - 6.3 [Comparison Operators](#63-comparison-operators)
   - 6.4 [Logical Operators](#64-logical-operators)
   - 6.5 [Unary Operators](#65-unary-operators)
   - 6.6 [Precedence Table](#66-precedence-table)
7. [Control Flow](#7-control-flow)
   - 7.1 [IF / THEN / ELSE / END IF](#71-if--then--else--end-if)
   - 7.2 [FOR / NEXT / STEP](#72-for--next--step)
   - 7.3 [WHILE / WEND](#73-while--wend)
   - 7.4 [SELECT CASE](#74-select-case)
8. [Subroutines](#8-subroutines)
9. [Functions](#9-functions)
10. [Built-in Functions](#10-built-in-functions)
11. [INPUT Statement](#11-input-statement)
12. [PRINT Statement](#12-print-statement)
13. [Error Behaviour](#13-error-behaviour)
14. [Complete Example](#14-complete-example)

---

## 1. Overview

SharpBASIC is a structured, line-oriented interpreted language with a clean pipeline (Lexer → Parser → AST → Tree-walking Evaluator). It has no line numbers, no GOTO, and no implicit type coercion — every operation is explicitly typed. Programs are stored in `.sbx` files and run from the command line or entered interactively in the REPL.

---

## 2. Lexical Rules

### 2.1 Character Set

Source files are read as text. Line endings are normalised at load time: `\r\n` and bare `\r` are both converted to `\n` before tokenisation. No encoding is enforced by the runtime; the host file system encoding is used as-is.

### 2.2 Case Sensitivity

**Keywords are case-insensitive.** `PRINT`, `Print`, and `print` are all valid. The lexer normalises keywords to upper-case internally via `ToUpperInvariant()`.

**Identifiers are case-sensitive.** `myVar` and `MYVAR` are two distinct symbols at runtime. The symbol table uses a case-sensitive key. Always use consistent casing for identifiers throughout your program.

### 2.3 Whitespace and Newlines

Spaces are the primary token delimiter. The lexer accumulates characters into a buffer and flushes the buffer when it encounters a space, a recognised symbol character, a digit at the start of a token, or a newline.

**Newlines are significant.** A `NewLine` token is emitted for each `\n` and the parser uses it as a statement separator in certain contexts (after `THEN`, after `FOR`, `WHILE`, inside blocks).

### 2.4 Comments

Lines beginning with `REM` (followed by at least one space) are comment lines. The lexer detects the word `REM` when a space character immediately follows it, then discards everything up to the next newline.

```
REM This is a comment
LET x = 42  REM Inline comment is also supported
REM
```

The last form — `REM` alone on a line with nothing after it — is also valid and treated as an empty comment.

> **Gotcha:** `REM` recognition requires a space, newline, or another token boundary after the word. If a source file ends with the bare text `REM` and no trailing newline or space, the lexer flushes the token as an `Identifier` and the parser reports an error. Always end source files with a trailing newline.

### 2.5 Line Continuation

There is **no line-continuation character**. Each logical statement must fit on one physical line (or be a multi-line block construct like `IF`/`FOR`/`WHILE`/`SUB`/`FUNCTION`).

### 2.6 Operator Spacing

All operators have dedicated lexer branches. Surrounding spaces are optional for every operator:

```
LET x = 5
LET x=5
LET y = 3+4
LET z = a<b
```

---

## 3. Data Types

| Type name | Storage | Range / notes | Literal syntax |
|-----------|---------|---------------|----------------|
| `Integer` | C# `int` (32-bit signed) | −2,147,483,648 … 2,147,483,647 | `42`, `-7` |
| `Float` | C# `double` (64-bit IEEE 754) | Full double precision | `3.14`, `-0.5` |
| `String` | C# `string` | Unicode, arbitrary length | `"hello"` |
| `Boolean` | C# `bool` | `TRUE` or `FALSE` | `TRUE`, `FALSE` |
| `Array<T>` | Fixed-size typed array | Declared size, 0-indexed | Declared with `DIM` |

There is no implicit conversion between types. Integer and Float are automatically promoted to Float when mixed in arithmetic (see [§6.1](#61-arithmetic-operators)).

String literals are delimited by double-quotes. There is no escape sequence syntax — a literal double-quote cannot appear inside a string literal.

Float literals must contain a decimal point to be recognised as Float; `3` is an Integer literal, `3.0` is a Float literal.

---

## 4. Variables

### 4.1 Declaration and Assignment

Variables do not need to be declared before use. The `LET` keyword both declares (on first use) and assigns:

```
LET name = expression
```

Examples:

```
LET x = 10
LET greeting = "Hello, world"
LET ratio = 3.14
LET flag = TRUE
```

Re-assigning an existing variable with `LET` replaces its value (there is no type enforcement on re-assignment).

There is no bare assignment (`x = 5`); the `LET` keyword is mandatory.

### 4.2 Scope Rules

SharpBASIC uses a lexically-chained symbol table. Each SUB and FUNCTION call creates a new local scope whose parent is the caller's scope.

- **Reading**: `Get` walks up the parent chain, so a sub/function can read variables from an enclosing scope.
- **Writing**: `Set` always writes to the **current** (local) scope. Assignments inside a sub/function do **not** mutate variables in the caller's scope.

```
LET x = 10

SUB ShowX()
    REM can read x from the outer scope
    PRINT x
    REM LET writes to LOCAL scope only - outer x is unchanged
    LET x = 99
END SUB

CALL ShowX()
REM still prints 10 - outer x was not mutated
PRINT x
```

---

### 4.3 Constants

A constant is a named value that cannot be changed after declaration. Use `CONST` to declare it:

```
CONST name = literal
```

- The value must be a **literal** — integer, float, string, or boolean. Variable expressions and function calls are not permitted.
- Constants are **global only**. Declaring `CONST` inside a `SUB` or `FUNCTION` is a runtime error.
- Re-assigning a constant with `LET` after declaration is a runtime error: *"Cannot assign to constant {name}."*
- No `AS type` annotation — the type is inferred from the literal.
- Constants are resolved through the parent chain like variables but are stored with a write-protect flag.

```
CONST MAX = 100
CONST MIN = 1
CONST GREETING = "Hello"
CONST PI = 3.14159

PRINT MAX        REM → 100
PRINT GREETING   REM → Hello
```

---

### 4.4 SET GLOBAL

`SET GLOBAL` writes a value to a variable that already exists in the **global** (topmost) scope. It is the only way to mutate global state from inside a SUB or FUNCTION.

```
SET GLOBAL name = expression
```

- `SET GLOBAL` is only valid **inside a SUB or FUNCTION**. Using it at the top level is a runtime error: *"SET GLOBAL can only be used inside a SUB or FUNCTION."*
- The named variable must already exist in the global scope. If it does not exist, the statement fails: *"SET GLOBAL: variable {name} not found in global scope."*
- The expression is evaluated in the local scope of the current sub/function, then the result is stored directly in the global scope.
- `SET GLOBAL` cannot target a `CONST`-declared name — attempting to do so is a runtime error.

```
LET counter = 0

SUB Increment()
    SET GLOBAL counter = counter + 1
END SUB

CALL Increment()
CALL Increment()
PRINT counter   REM → 2
```

> **Gotcha:** `counter + 1` inside the SUB reads `counter` from the global scope (via the parent chain), then writes the result back to the global scope via `SET GLOBAL`. If you used `LET counter = counter + 1` instead, `LET` would create a local variable — the global `counter` would remain unchanged.

---

## 5. Arrays

### 5.1 Declaration

Arrays are declared with `DIM`. Size must be a literal integer at parse time. The type annotation is mandatory.

```
DIM name[size] AS type
```

Valid types: `INTEGER`, `FLOAT`, `STRING`, `BOOLEAN`.

```
DIM scores[10] AS INTEGER
DIM names[5] AS STRING
```

Arrays are zero-initialised on declaration:
- `INTEGER` → `0`
- `FLOAT` → `0.0`
- `BOOLEAN` → `FALSE`
- `STRING` → `""` (empty string)

> **Gotcha:** Declaring a variable with `DIM` when a variable with that name already exists in the current scope produces a runtime error: *"The variable {name} already exists and cannot be redefined."*

### 5.2 Access and Assignment

Array elements are accessed with square bracket notation. Indices are 0-based.

```
REM read element 0
PRINT scores[0]
REM write element 3
LET scores[3] = 42
```

The `LET` keyword is required for array element assignment. Note the use of `LET name[index] = value` syntax, not `name[index] = value`.

### 5.3 Bounds Behaviour

The evaluator checks that the index is in the range `0` to `size - 1`. An out-of-range access or assignment produces a runtime error:

```
Supplied index {idx} is outside of the range 0-{size} defined for the array: {name}.
```

The valid index range is `0` to `size - 1`. Any access or assignment using an index outside this range produces a clean diagnostic — it does not crash.

> **Note:** The upper bound in the error message is the declared `size` (e.g. `0-5` for a 5-element array). This is the **exclusive** limit; the last valid index is `size - 1`.

---

### 5.4 Two-Dimensional Arrays

Declare a two-dimensional array by supplying two size brackets:

```
DIM name[rows][cols] AS type
```

Access and assignment use the same double-bracket syntax. Both indices are 0-based:

```
LET name[row][col] = expression
PRINT name[row][col]
```

Bounds checking applies to each dimension independently.

```
REM 3×3 identity matrix
DIM matrix[3][3] AS INTEGER

FOR r = 0 TO 2
    FOR c = 0 TO 2
        IF r = c THEN
            LET matrix[r][c] = 1
        ELSE
            LET matrix[r][c] = 0
        END IF
    NEXT c
NEXT r

FOR r = 0 TO 2
    FOR c = 0 TO 2
        PRINT matrix[r][c]
    NEXT c
NEXT r
```

> **Note:** The declaration uses two separate bracket pairs — `DIM name[rows][cols]`, not `DIM name(rows, cols)`. The parser distinguishes this from a function call by the `[` token: `name(r, c)` would parse as a call expression.

---

## 6. Operators

### 6.1 Arithmetic Operators

Operands must be numeric (`Integer` or `Float`). An `Integer` operand mixed with a `Float` operand promotes the result to `Float`. Two `Integer` operands return `Integer` unless the result is not a whole number (in which case the result is `Float`).

| Operator | Operation | Example |
|----------|-----------|---------|
| `+` | Addition | `LET n = 3 + 4` |
| `-` | Subtraction | `LET n = 10 - 3` |
| `*` | Multiplication | `LET n = 6 * 7` |
| `/` | Division | `LET n = 10 / 4` → `2.5` (Float) |
| `MOD` | Integer modulo | `LET n = 10 MOD 3` → `1` |

Division by zero produces a runtime error: *"Attempted to divide by zero."*

`MOD` truncates both operands to integer before computing the remainder.

### 6.2 String Operator

| Operator | Operation | Example |
|----------|-----------|---------|
| `&` | Concatenation | `LET s = "Hello" & ", " & "world"` |

`&` calls `.ToString()` on both sides, so any value type can be concatenated:

```
REM results in "Count: 42"
LET msg = "Count: " & 42
REM results in "True is true"
LET msg = TRUE & " is true"
```

`+` does **not** concatenate strings. Applying `+` to two `StringValue` operands is a runtime error.

### 6.3 Comparison Operators

Produce a `Boolean` result.

| Operator | Meaning |
|----------|---------|
| `=` | Equal |
| `<>` | Not equal |
| `<` | Less than |
| `>` | Greater than |
| `<=` | Less than or equal |
| `>=` | Greater than or equal |

Numeric comparisons work on both `Integer` and `Float`. String comparisons support only `=` and `<>` (value equality — two strings with identical content compare equal). Applying `<`, `>`, `<=`, or `>=` to strings is a runtime error.

### 6.4 Logical Operators

Operands must both be `Boolean`. Applying to non-boolean types is a runtime error.

| Operator | Operation | Example |
|----------|-----------|---------|
| `AND` | Logical AND | `IF x > 0 AND x < 10 THEN` |
| `OR` | Logical OR | `IF flag OR done THEN` |

### 6.5 Unary Operators

| Operator | Operation | Valid operand | Example |
|----------|-----------|---------------|---------|
| `-` | Arithmetic negation | `Integer`, `Float` | `LET n = -x` |
| `NOT` | Boolean negation | `Boolean` | `LET b = NOT flag` |

### 6.6 Precedence Table

Higher binding power binds tighter. Operators at the same level are left-associative (Pratt parser).

| Binding power | Operators |
|---------------|-----------|
| 20 | `*`, `/`, `MOD` |
| 10 | `+`, `-`, `&` |
| 5 | `=`, `<>`, `<`, `>`, `<=`, `>=` |
| 3 | `AND` |
| 2 | `OR` |
| 0 | (everything else — stops the Pratt loop) |

Parentheses override precedence in the usual way: `(expr)`.

> **Gotcha:** `&` has the same binding power as `+` and `-` (10), so `"x=" & 1 + 2` is `"x=" & 3` → `"x=3"`, not `"x=12"`.

---

## 7. Control Flow

### 7.1 IF / THEN / ELSE / END IF

The condition must evaluate to a `Boolean`. If it does not, the runtime reports an error: *"IF condition must evaluate to a boolean value."*

**Syntax:**

```
IF condition THEN
    statements
END IF

IF condition THEN
    statements
ELSE
    statements
END IF
```

Single-line `IF` is permitted: `THEN` may be followed by a statement on the same line. `END IF` is always required to close the block.  
There is no `ELSEIF` / `ELSIF`. Nest `IF` blocks for multiple branches.

**Example:**

```
LET score = 85

IF score >= 90 THEN
    PRINT "A"
ELSE
    IF score >= 80 THEN
        PRINT "B"
    ELSE
        PRINT "C"
    END IF
END IF
```

### 7.2 FOR / NEXT / STEP

**Syntax:**

```
FOR var = start TO limit
    statements
NEXT

FOR var = start TO limit STEP step
    statements
NEXT var
```

- `var` — loop counter identifier (created / overwritten in the current scope)
- `start`, `limit`, `step` — any numeric expression
- `STEP` is optional; default step is `1.0`
- The identifier after `NEXT` is optional; if provided it must match `var`, otherwise a parse error is raised
- The loop guard is `step > 0 ? i <= limit : i >= limit`; a negative `STEP` counts down

> **Gotcha:** The loop counter is stored as `IntValue((int)i)` every iteration, even when `start`, `limit`, or `step` are floats. Sub-integer step values are silently truncated to int in the symbol table, but the internal accumulator `i` remains a `double`. Use `STEP` with integer values to avoid surprising counter values.

> **Gotcha:** `start`, `limit`, and `step` must evaluate to `Integer` or `Float`. Supplying a `String` or `Boolean` expression for any of these causes an unhandled exception that halts the interpreter.

**Examples:**

```
FOR i = 1 TO 5
    PRINT i
NEXT

FOR i = 10 TO 1 STEP -2
    PRINT i
NEXT i
```

### 7.3 WHILE / WEND

**Syntax:**

```
WHILE condition
    statements
WEND
```

The condition is re-evaluated before every iteration. It must evaluate to a `Boolean`; otherwise a runtime error is raised: *"WHILE condition must evaluate to a boolean value."*

**Example:**

```
LET n = 1
WHILE n <= 5
    PRINT n
    LET n = n + 1
WEND
```

---

### 7.4 SELECT CASE

`SELECT CASE` dispatches on the value of an expression. It is an alternative to nested `IF / ELSE` blocks when branching on a single value.

**Syntax:**

```
SELECT CASE expression
    CASE value1
        statements
    CASE value2, value3
        statements
    CASE ELSE
        statements
END SELECT
```

- The expression is evaluated once.
- Each `CASE` clause lists one or more comma-separated match values. If the expression equals **any** of them, that clause executes.
- **First match wins.** Subsequent clauses are skipped — there is no fall-through.
- `CASE ELSE` is optional. If provided, it must be the last clause and executes when no earlier clause matched.
- `END SELECT` closes the block (two tokens: `END` then `SELECT`).

**Example:**

```
SELECT CASE op
    CASE "+"
        PRINT "Addition"
    CASE "-"
        PRINT "Subtraction"
    CASE "*", "/"
        PRINT "Multiplication or division"
    CASE ELSE
        PRINT "Unknown operator"
END SELECT
```

---

## 8. Subroutines

### Declaration

```
SUB name(param1 AS type, param2 AS type, ...)
    statements
END SUB
```

- Parameter types must be `INTEGER`, `FLOAT`, `STRING`, or `BOOLEAN`.
- A zero-parameter SUB requires empty parentheses: `SUB Greet()`.
- The body terminates at `END SUB` (must be two tokens: `END` then `SUB`).

### Calling a SUB

```
CALL name(arg1, arg2, ...)
```

Arguments are expressions evaluated in the caller's scope before the new local scope is created (pass-by-value). Arguments are positional; the argument count must match the parameter count exactly. Too few arguments produces a runtime error diagnostic; extra arguments beyond the parameter count are silently ignored.

### Return

`RETURN` (with no value) inside a SUB exits immediately. Execution in the caller continues after the `CALL` statement. Reaching `END SUB` without a `RETURN` is also valid — the SUB simply returns.

### Scope

A fresh `SymbolTable` is created with the caller's table as parent. The SUB can **read** variables from the caller's scope, but all `LET` assignments within the SUB write to the local scope only (see [§4.2](#42-scope-rules)).

### Name restriction

A SUB name must not match any built-in function name (case-insensitive). Attempting to declare such a SUB produces a runtime error at the hoisting phase.

### Declaration order (hoisting)

All `SUB` and `FUNCTION` declarations are **hoisted**: they are registered before any top-level statement executes. A `SUB` may therefore be declared anywhere in the file — even after the `CALL` statement that invokes it.

### Case sensitivity

SUB names are **case-sensitive** at call sites. `CALL Greet()` and `CALL greet()` refer to different subs. Declare and call with consistent casing.

**Example:**

```
SUB PrintLine(msg AS STRING)
    PRINT msg
END SUB

CALL PrintLine("Hello from a SUB")
```

---

## 9. Functions

### Declaration

```
FUNCTION name(param1 AS type, ...) AS returntype
    statements
    RETURN expression
END FUNCTION
```

- The return type must be one of `INTEGER`, `FLOAT`, `STRING`, `BOOLEAN`.
- The body terminates at `END FUNCTION`.
- At least one code path must execute a `RETURN expression` — if the function body completes without throwing `ReturnException`, the evaluator returns a failure: *"Missing RETURN statement in FUNCTION."*

### Calling a FUNCTION

A FUNCTION is called in expression position (not with `CALL`):

```
LET result = name(arg1, arg2, ...)
PRINT name(42)
```

### RETURN

```
RETURN expression
```

Exits the function and delivers the value back to the call site. The declared return type is advisory — there is no runtime enforcement that the returned value matches the declared type.

### Scope and name restriction

Same rules as SUBs: new local scope with parent chain, caller variables readable but not writable, name must not collide with built-ins. FUNCTION declarations are hoisted along with SUBs — a function may be declared after the expression that calls it. FUNCTION names are **case-sensitive** at call sites.

**Recursive example:**

```
FUNCTION Factorial(n AS INTEGER) AS INTEGER
    IF n <= 1 THEN
        RETURN 1
    END IF
    RETURN n * Factorial(n - 1)
END FUNCTION

PRINT Factorial(10)
```

---

## 10. Built-in Functions

Built-in functions are resolved by name (case-insensitive) before user-defined functions. They cannot be shadowed by user `FUNCTION` or `SUB` declarations.

### String functions

| Signature | Return type | Description | Example |
|-----------|-------------|-------------|---------|
| `LEN(s)` | `Integer` | Number of characters in `s` | `LEN("hello")` → `5` |
| `MID$(s, start, length)` | `String` | Substring; `start` is **1-based** | `MID$("hello", 2, 3)` → `"ell"` |
| `LEFT$(s, n)` | `String` | First `n` characters | `LEFT$("hello", 3)` → `"hel"` |
| `RIGHT$(s, n)` | `String` | Last `n` characters | `RIGHT$("hello", 3)` → `"llo"` |
| `TRIM$(s)` | `String` | Remove leading/trailing whitespace | `TRIM$("  hi  ")` → `"hi"` |
| `UPPER$(s)` | `String` | Convert to upper-case | `UPPER$("hello")` → `"HELLO"` |
| `LOWER$(s)` | `String` | Convert to lower-case | `LOWER$("HELLO")` → `"hello"` |
| `CHR$(n)` | `String` | Character from Unicode code point `n` | `CHR$(65)` → `"A"`, `CHR$(34)` → `"\""`  |

> **`MID$` uses 1-based indexing.** `MID$("abc", 1, 1)` → `"a"`, not `MID$("abc", 0, 1)`.

> **`CHR$(34)` is the only way to embed a double-quote in a string**, since string literals have no escape sequence syntax. `CHR$(10)` produces a newline character.

> **Gotcha:** `MID$`, `LEFT$`, and `RIGHT$` perform no internal bounds checking. Providing a `length` or `n` value that exceeds the string length (e.g. `LEFT$("hi", 10)`) throws an unhandled C# `ArgumentOutOfRangeException` that halts the interpreter. Validate string lengths before calling these functions.

### Numeric functions

| Signature | Return type | Description | Example |
|-----------|-------------|-------------|---------|
| `ABS(n)` | Same as input | Absolute value | `ABS(-5)` → `5` |
| `SQR(n)` | `Float` | Square root | `SQR(9)` → `3.0` |
| `INT(n)` | `Integer` (if int input) / `Float` (if float input) | Floor: if `n` is Integer returns it unchanged; if Float returns `Math.Floor(n)` as Float | `INT(3.9)` → `3.0` |
| `RND()` | `Float` | Pseudo-random number in range `[0.0, 1.0)` | `RND()` → e.g. `0.7341…` |

> **`INT` returns a `Float` when given a `Float` argument.** `INT(3.9)` returns the `Float` value `3.0`, not the `Integer` `3`. To get an `Integer` from a `Float`, use `VAL(STR$(INT(n)))`.

### Conversion functions

| Signature | Return type | Description | Example |
|-----------|-------------|-------------|---------|
| `STR$(n)` | `String` | Convert `Integer` or `Float` to string | `STR$(42)` → `"42"` |
| `VAL(s)` | `Integer` or `Float` | Parse string to number; tries `int.Parse` first, then `double.Parse` | `VAL("42")` → `42`, `VAL("3.14")` → `3.14` |

`VAL` returns `null` (which surfaces as a failure) if the string cannot be parsed as either integer or float.

> **Gotcha (`STR$`):** `STR$` converts a `Float` via C# `double.ToString()` **without** an explicit culture argument. On systems where the decimal separator is `,` (e.g. many European locales), `STR$(3.14)` returns `"3,14"`. `PRINT` and `&` are not affected — `FloatValue.ToString()` uses invariant culture internally. If portability matters, avoid relying on `STR$` for float-to-string conversion.

### Diagnostic function

| Signature | Return type | Description | Example |
|-----------|-------------|-------------|---------|
| `TYPENAME(v)` | `String` | Returns the runtime type name of any value | `TYPENAME(42)` → `"Integer"`, `TYPENAME("x")` → `"String"` |

Type names returned: `"Integer"`, `"Float"`, `"String"`, `"Boolean"`, `"Array<typename>"`, `"Void"`.

---

## 11. INPUT Statement

`INPUT` reads a line of text from standard input and stores it as a `String` in the named variable. The variable does not need to be declared beforehand.

**Syntax:**

```
INPUT varname
INPUT "prompt"; varname
```

- With a prompt: the prompt string is printed (with `? ` appended) and the cursor waits on the same line.
- The result is **always stored as `String`**, regardless of what the user types.

```
INPUT "Enter your name"; name
PRINT "Hello, " & name

INPUT age
LET ageNum = VAL(age)
```

> **Gotcha:** The variable always receives a `String`. Use `VAL()` to convert to a number before arithmetic.

---

## 12. PRINT Statement

Evaluates a single expression and prints it to standard output, followed by a newline (`Console.WriteLine`).

**Syntax:**

```
PRINT expression
```

There is no multi-argument form and no semicolon-suppression of the newline. Every `PRINT` ends with `\n`.

```
PRINT "Hello, world"
PRINT 42 + 8
PRINT "Result: " & (10 * 5)
```

To build a complex output on one line, construct a string with `&`:

```
PRINT "x=" & x & " y=" & y
```

---

## 13. Error Behaviour

All runtime errors produce a `Diagnostic` with a line/column location and message. When the evaluator encounters an error it returns `EvalFailure` containing one or more diagnostics. Execution of the current program halts and all diagnostics are printed to stderr.

| Scenario | Error message |
|----------|---------------|
| Undefined variable | `Unknown Identifier: {name}` |
| Divide by zero | `Attempted to divide by zero.` |
| IF/WHILE condition is not Boolean | `IF condition must evaluate to a boolean value.` / `WHILE condition must evaluate to a boolean value.` |
| AND/OR applied to non-Boolean | `Invalid combination: … Cannot perform AND or OR on non boolean values.` |
| Type mismatch on `+` with strings | `Unknown Expression: SharpBasic.Ast.BinaryExpression` |
| String compared with `<`, `>`, `<=`, `>=` | `Unknown Expression: SharpBasic.Ast.BinaryExpression` |
| Array index out of range | `Supplied index {n} is outside of the range 0-{size} defined for the array: {name}.` |
| Assigning wrong type to typed array | `Supplied value is not an {TYPE} as defined for the array: {name}.` |
| Accessing non-array as array | `Identifier: {name} is not an Array.` |
| Re-declaring an existing variable with DIM | `The variable {name} already exists and cannot be redefined.` |
| SUB not found | `Sub: {name} not found.` |
| FUNCTION not found | `Function: {name} not found.` |
| FUNCTION with no executed RETURN | `Missing RETURN statement in FUNCTION.` |
| SUB name matches a built-in | `SUB {name} is the name of a built in function and cannot be reused.` |
| FUNCTION name matches a built-in | `FUNCTION {name} is the name of a built in function and cannot be reused.` |
| Unary `-` or `NOT` applied to wrong type | `Invalid Operator: {op} for type {type}.` |

The REPL continues after an error (the symbol table is preserved). File runner exits with code `1` on any error.

---

## 14. Complete Example

The following program exercises nearly every language feature: variables, arithmetic, strings, boolean logic, IF/ELSE, FOR/NEXT with STEP, WHILE/WEND, arrays, SUBs, FUNCTIONs, built-in functions, INPUT, and PRINT.

```
REM SharpBASIC comprehensive example

REM ── Built-in function demos ──────────────────────────────

LET greeting = "  Hello, SharpBASIC!  "
PRINT TRIM$(greeting)
PRINT UPPER$(TRIM$(greeting))
PRINT LEN(TRIM$(greeting))
PRINT LEFT$("abcdef", 3)
PRINT RIGHT$("abcdef", 3)
PRINT MID$("abcdef", 2, 3)
PRINT STR$(3.14)
PRINT VAL("99")
PRINT ABS(-42)
PRINT SQR(16)
PRINT TYPENAME(TRUE)

REM ── Variables and arithmetic ─────────────────────────────

LET a = 10
LET b = 3
PRINT a + b
PRINT a - b
PRINT a * b
PRINT a / b
PRINT a MOD b

REM ── String concatenation ─────────────────────────────────

LET name = "World"
PRINT "Hello, " & name & "!"

REM ── Boolean and comparison ───────────────────────────────

LET x = 7
LET inRange = x > 0 AND x < 10
IF inRange THEN
    PRINT "x is in range"
END IF

REM ── IF / ELSE ────────────────────────────────────────────

IF x > 5 THEN
    PRINT "big"
ELSE
    PRINT "small"
END IF

REM ── FOR / NEXT ───────────────────────────────────────────

FOR i = 1 TO 5
    PRINT i
NEXT

FOR i = 10 TO 2 STEP -3
    PRINT i
NEXT i

REM ── WHILE / WEND ─────────────────────────────────────────

LET count = 1
WHILE count <= 3
    PRINT "count=" & count
    LET count = count + 1
WEND

REM ── Arrays ───────────────────────────────────────────────

DIM primes[5] AS INTEGER
LET primes[0] = 2
LET primes[1] = 3
LET primes[2] = 5
LET primes[3] = 7
LET primes[4] = 11

FOR i = 0 TO 4
    PRINT primes[i]
NEXT

REM ── Subroutine ───────────────────────────────────────────

SUB SayHello(who AS STRING)
    PRINT "Hello, " & who & "!"
END SUB

CALL SayHello("Alice")
CALL SayHello("Bob")

REM ── Function (recursive) ─────────────────────────────────

FUNCTION Fib(n AS INTEGER) AS INTEGER
    IF n <= 1 THEN
        RETURN n
    END IF
    RETURN Fib(n - 1) + Fib(n - 2)
END FUNCTION

PRINT "Fib(10) = " & Fib(10)

REM ── INPUT ────────────────────────────────────────────────

INPUT "Enter a number"; raw
LET num = VAL(raw)
PRINT "Double: " & (num * 2)
```
