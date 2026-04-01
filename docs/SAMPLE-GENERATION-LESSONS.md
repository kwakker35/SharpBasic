# SharpBASIC — Sample Generation Lessons Learned
Generated: 2026-03-22

## Purpose

This document captures patterns of error found across the SharpBASIC sample
programs. Use it as a checklist when writing new samples, and as a reference
when the language spec is extended to new phases.

---

## Error Pattern Catalogue

---

### Pattern 1 — Typed LET declaration

**Frequency:** 9 samples affected (every sample with local variables)

**Description:**
`LET` was written with an `AS TYPE` type annotation between the identifier and
the `=`, mimicking QBasic `DIM` syntax or typed variable declaration syntax
from other languages.

**Example (broken):**
```basic
LET name AS STRING = "SharpBASIC"
LET count AS INTEGER = 0
LET ratio AS FLOAT = 3.14
LET flag AS BOOLEAN = TRUE
```

**Example (correct):**
```basic
LET name = "SharpBASIC"
LET count = 0
LET ratio = 3.14
LET flag = TRUE
```

**Spec reference:** `docs/language-spec-v1.md` — §4.1 Declaration and Assignment

**Root cause:**
SharpBASIC's `LET` is a plain assignment that also declares. It does not accept
a type annotation. The typed form `LET x AS TYPE = value` does not exist in the
grammar; the parser sees `AS` as an identifier after the variable name and
fails expecting `=`. This form was likely carried over from QBasic
familiarity where `DIM x AS INTEGER` is the typing syntax, or from strongly
typed language habits.

**Prevention rule:**
Use `LET name = value` only. To declare a typed container use `DIM name[size] AS TYPE` for arrays. Plain variables are untyped at declaration.

---

### Pattern 2 — Boolean equality comparison with `= TRUE` / `= FALSE`

**Frequency:** 3 samples affected (tic-tac-toe, hangman, wordle)

**Description:**
Boolean values were compared to the literals `TRUE` or `FALSE` using the `=`
operator in `IF` and `WHILE` conditions. The evaluator does not support
comparing two `Boolean` values with `=` — it falls through to numeric
conversion which throws an unhandled exception.

**Example (broken):**
```basic
WHILE gameOver = FALSE
    ...
WEND

IF solved = TRUE THEN
    PRINT "You won!"
END IF

IF isAlpha = FALSE THEN
    RETURN FALSE
END IF
```

**Example (correct):**
```basic
WHILE NOT gameOver
    ...
WEND

IF solved THEN
    PRINT "You won!"
END IF

IF NOT isAlpha THEN
    RETURN FALSE
END IF
```

**Spec reference:** `docs/language-spec-v1.md` — §6.3 Comparison Operators, §7.1 IF / THEN / ELSE / END IF, §7.3 WHILE / WEND

**Root cause:**
The spec states comparison operators work on numeric and string types. The
evaluator has a dedicated code path for string equality and numeric comparisons
but no equivalent path for Boolean values — they are not numbers and are not
strings, so the `ToFloat()` conversion throws. Boolean values should be used
directly as conditions or negated with `NOT`.

**Prevention rule:**
Never write `= TRUE` or `= FALSE` in a condition. Use the Boolean value directly
(`IF flag THEN`) or negate it (`IF NOT flag THEN`).

---

### Pattern 3 — `INT()` Float result used as array index

**Frequency:** 2 samples affected (hangman, wordle)

**Description:**
`INT(RND() * n)` was used directly as an array index. Per the spec, `INT`
returns a `Float` when given a `Float` argument. The array evaluator requires
an `IntValue` for the index; receiving a `FloatValue` produces the error
`"Invalid index supplied."`.

**Example (broken):**
```basic
LET wordIndex = INT(RND() * 8)
LET secret = words[wordIndex]
```

**Example (correct):**
```basic
LET wordIndex = VAL(STR$(INT(RND() * 8)))
LET secret = words[wordIndex]
```

**Spec reference:** `docs/language-spec-v1.md` — §5.2 Access and Assignment, §10 Built-in Functions (`INT`, `VAL`, `STR$`)

**Root cause:**
`INT(floatValue)` returns a `FloatValue` (e.g. `3.0`), not an `IntValue` (`3`).
The spec documents this gotcha explicitly: *"`INT` returns a `Float` when given
a `Float` argument."* The workaround `VAL(STR$(INT(n)))` round-trips through
string representation to force integer parsing.

**Prevention rule:**
Never use `INT(RND() * n)` directly as an array index. Always convert the result
to `Integer` first using `VAL(STR$(INT(n)))`.

---

### Pattern 4 — `STR$()` called on non-numeric value

**Frequency:** 1 sample affected (variables)

**Description:**
`STR$()` was called with a `Boolean` argument to convert it to a string for
`PRINT`. `STR$()` only accepts `Integer` or `Float` arguments; calling it with
a `Boolean` returns null and causes a runtime failure.

**Example (broken):**
```basic
LET isOpen = TRUE
PRINT "Open source: " & STR$(isOpen)
```

**Example (correct):**
```basic
LET isOpen = TRUE
PRINT "Open source: " & isOpen
```

**Spec reference:** `docs/language-spec-v1.md` — §6.2 String Operator, §10 Built-in Functions (`STR$`)

**Root cause:**
`&` calls `.ToString()` on any value type (Integer, Float, String, Boolean),
so string concatenation works directly without conversion. `STR$` is only for
numeric-to-string conversion. Using `STR$` on a Boolean was unnecessary.

**Prevention rule:**
Use `&` to concatenate any value into a string. Reach for `STR$` only when you
need numeric formatting (e.g. `STR$(3.14)`). Never call `STR$` on a Boolean.

---

### Pattern 5 — `MID$` length exceeds available characters

**Frequency:** 1 sample affected (string-utilities)

**Description:**
`MID$(s, start, length)` was called with a `length` that extended past the end
of the string. As of the current interpreter version, `MID$`, `LEFT$`, and
`RIGHT$` perform bounds checking and produce a clean runtime diagnostic instead
of crashing.

**Example (broken):**
```basic
REM "SharpBASIC" has 10 chars; position 7 leaves only 4 chars ("ASIC")
PRINT MID$("SharpBASIC", 7, 5)
```

**Example (correct):**
```basic
REM position 6 leaves 5 chars ("BASIC")
PRINT MID$("SharpBASIC", 6, 5)
```

**Spec reference:** `docs/language-spec-v1.md` — §10 Built-in Functions

**Root cause:**
`MID$` uses 1-based indexing. Off-by-one errors in the start position produce
an out-of-range error. Always verify that `start + length - 1 <= LEN(s)` before
calling `MID$`.

**Prevention rule:**
Always verify that `start + length - 1 <= LEN(s)` before calling `MID$`.
The same applies to `LEFT$` and `RIGHT$` — ensure `n <= LEN(s)`.

---

### Pattern 6 — `'` used instead of `REM` for comments

**Frequency:** Introduced during tooling — affected all 9 fixed samples

**Description:**
Single-quote `'` was used as an inline comment character. SharpBASIC has no
single-quote comment syntax. Only `REM` (followed by a space or end of line)
is valid.

**Example (broken):**
```basic
LET x = 10  ' this is a comment
```

**Example (correct):**
```basic
LET x = 10  REM this is a comment
```

**Spec reference:** `docs/language-spec-v1.md` — §2.4 Comments

**Root cause:**
Single-quote comments are common in Visual Basic and many other BASIC dialects.
SharpBASIC uses `REM` exclusively, as documented in §2.4.

**Prevention rule:**
Use `REM` for all comments. Inline comments follow the statement on the same
line: `LET x = 10  REM explanation`.

---

## Writing Rules for Future Samples

Derived from the patterns above.

1. **No typed LET.** Write `LET x = value`. Never write `LET x AS TYPE = value`.
2. **Never compare Booleans with `=`.** Use `IF flag THEN` and `IF NOT flag THEN`. Never `= TRUE` or `= FALSE`.
3. **Convert `INT(RND())` before using as an array index.** Use `VAL(STR$(INT(RND() * n)))` to produce an `Integer`.
4. **Use `&` to concatenate any value type.** Only use `STR$` for numeric formatting. Never pass a Boolean to `STR$`.
5. **Validate `MID$` / `LEFT$` / `RIGHT$` lengths.** Ensure `start + length - 1 <= LEN(s)`. The runtime does not bounds-check.
6. **Comments use `REM` only.** Single-quote `'` is not a comment character in SharpBASIC.
7. **`INPUT` always returns a String.** Use `VAL()` before doing arithmetic on user input.
8. **Arrays need `DIM` with square brackets and `AS TYPE`.** `DIM scores[10] AS INTEGER`. Access with `scores[0]`.
9. **`WHILE` and `IF` conditions must be Boolean.** Numeric or String conditions produce a runtime error.
10. **Identifiers are case-sensitive.** `myVar` and `MYVAR` are different. Keep casing consistent throughout a program.

---

## Phase Coverage Gaps

Features defined in `docs/language-spec-v1.md` with no dedicated sample program.

| Feature | Spec Section | Coverage Status |
|---------|--------------|-----------------|
| `TRIM$` / `UPPER$` / `LOWER$` standalone | §10 | Covered only as part of string-utilities compound demo |
| `SQR()` — square root | §10 | Missing |
| `ABS()` — absolute value | §10 | Missing |
| `TYPENAME()` — runtime type introspection | §10 | Covered in variables sample only |
| `MOD` operator | §6.1 | Covered in fizzbuzz implicitly; no dedicated demo |
| Negative `STEP` in `FOR` loops | §7.2 | Missing |
| `RETURN` inside `SUB` (early exit) | §8 | Missing |
| Reading outer-scope variables from inside a SUB | §4.2 | Missing |
| `PRINT` with string built via `&` over multiple lines | §12 | Not explicitly demonstrated |

---

## Complexity Risk Flags

Feature combinations that produced multiple errors across samples.

| Feature / Combination | Risk Level | Notes |
|-----------------------|------------|-------|
| `LET` with type annotation | High | Present in every non-trivial sample; affects all samples with local variables |
| `Boolean = TRUE` / `= FALSE` in conditions | High | Required Boolean values are tested in IF/WHILE across multiple complex samples |
| `INT(RND())` as array index | Medium | `INT` Float-return gotcha is documented in spec but easy to miss when the intent is an integer index |
| `MID$` / `LEFT$` / `RIGHT$` near string boundaries | Medium | No bounds checking; easy to trigger with off-by-one start positions |
| `STR$` on non-numeric types | Low | Only affects Boolean; easy to avoid with `&` concatenation |
