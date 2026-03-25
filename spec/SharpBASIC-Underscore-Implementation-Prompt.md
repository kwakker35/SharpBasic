# SharpBASIC — Add Underscore Support to Identifiers

## Branch
Create a new branch before starting. Do not commit at the end — leave changes staged for review.

## Overview
Add support for underscore (`_`) as a valid character in identifiers, with one constraint:
underscore is valid between word characters only. It cannot appear as the first character,
the last character, or consecutively (no `__`).

Valid:
```
CONST MAX_WIDTH = 80
CONST ITEM_GOLD_BAG = 11
CONST DIR_N = 1
LET start_time = 0
```

Invalid:
```
LET _name = 1        ' leading underscore — invalid
LET done_ = 1        ' trailing underscore — invalid
LET x__y = 1         ' consecutive underscores — invalid
LET _ = 1            ' bare underscore — invalid
```

---

## Rule Definition

An identifier is valid if:
- It starts with a letter (`[A-Za-z]`)
- It contains only letters, digits, and underscores
- It does not end with an underscore
- It does not contain consecutive underscores (`__`)
- The `$` suffix for built-in string functions remains unchanged — these are not identifiers

Regex equivalent: `[A-Za-z][A-Za-z0-9]*(_[A-Za-z0-9]+)*`

---

## Implementation

### 1 — Lexer

The lexer currently accumulates identifier characters until it hits a non-identifier
character. Underscore must be added to the set of valid mid-identifier characters.

Find the identifier accumulation logic. Add `_` as a valid character to continue
accumulation (alongside letters and digits).

After accumulation, validate the resulting token:
- If it ends with `_` → emit as an error token or throw a lexer diagnostic
- If it contains `__` → emit as an error token or throw a lexer diagnostic
- Otherwise → emit as a normal identifier token

The validation can happen either during accumulation or as a post-accumulation check.
Post-accumulation is simpler and keeps the hot path clean.

### 2 — Tests

Write tests using xUnit and FluentAssertions. Cover every case below.
All tests must pass before the implementation is considered complete.

**Valid identifier tests — must lex and evaluate without error:**

```
CONST MAX_WIDTH = 80
PRINT MAX_WIDTH
```
Expected: prints `80`

```
CONST ITEM_GOLD_BAG = 11
PRINT ITEM_GOLD_BAG
```
Expected: prints `11`

```
LET start_time = 42
PRINT start_time
```
Expected: prints `42`

```
LET x_y_z = 99
PRINT x_y_z
```
Expected: prints `99`

```
LET a1_b2_c3 = 7
PRINT a1_b2_c3
```
Expected: prints `7`

```
CONST DIR_N = 1
CONST DIR_S = 2
PRINT DIR_N & " " & DIR_S
```
Expected: prints `1 2`

**Multi-word constant pattern — primary use case:**

```
CONST FRAME_WIDTH = 80
CONST SCREEN_HEIGHT = 30
CONST CONTENT_ROWS = 20
CONST MAX_INVENTORY = 4
CONST ITEM_HEALING_POTION = 1
CONST ITEM_LUCKY_CHARM = 2
CONST ITEM_GOLD_BAG = 11
PRINT FRAME_WIDTH & " " & SCREEN_HEIGHT & " " & CONTENT_ROWS
```
Expected: prints `80 30 20`

**Underscore in SUB and FUNCTION names:**

```
SUB print_result(val AS INTEGER)
    PRINT val
END SUB

CALL print_result(42)
```
Expected: prints `42`

```
FUNCTION get_max(a AS INTEGER, b AS INTEGER) AS INTEGER
    IF a > b THEN
        RETURN a
    END IF
    RETURN b
END FUNCTION

PRINT get_max(3, 7)
```
Expected: prints `7`

**Invalid identifier tests — must produce a diagnostic error, not crash:**

Leading underscore:
```
LET _name = 1
```
Expected: runtime or parse error, not silent failure

Trailing underscore:
```
LET done_ = 1
```
Expected: runtime or parse error, not silent failure

Consecutive underscores:
```
LET x__y = 1
```
Expected: runtime or parse error, not silent failure

Bare underscore:
```
LET _ = 1
```
Expected: runtime or parse error, not silent failure

**Boundary tests — must NOT be affected:**

Built-in string functions with `$` suffix must still work:
```
PRINT UPPER$("hello")
PRINT LEFT$("abcdef", 3)
PRINT MID$("hello", 2, 3)
```
Expected: `HELLO`, `abc`, `ell`

Existing identifiers without underscores must be unaffected:
```
LET myVar = 10
LET camelCase = 20
PRINT myVar & " " & camelCase
```
Expected: `10 20`

Keywords must still be recognised regardless of underscore changes:
```
LET count = 0
WHILE count < 3
    LET count = count + 1
WEND
PRINT count
```
Expected: `3`

**Underscore in array names:**

```
DIM item_codes[5] AS INTEGER
LET item_codes[0] = 11
LET item_codes[1] = 7
PRINT item_codes[0] & " " & item_codes[1]
```
Expected: `11 7`

**Underscore in CONST used as array size — if supported:**

```
CONST MAX_SIZE = 5
DIM scores[MAX_SIZE] AS INTEGER
LET scores[0] = 42
PRINT scores[0]
```
Expected: `42`

---

## Error Messages

When an invalid underscore pattern is detected, the diagnostic should be clear:

- Leading underscore: `Identifier cannot begin with an underscore: {name}`
- Trailing underscore: `Identifier cannot end with an underscore: {name}`
- Consecutive underscores: `Identifier cannot contain consecutive underscores: {name}`

These follow the existing diagnostic pattern in the interpreter.

---

## Spec Update

Update `language-spec-v1.md` section 2.2 (Case Sensitivity / Identifiers) to document
the underscore rule. Add after the existing identifier description:

> Underscores are permitted within identifiers between word characters.
> An underscore may not appear as the first or last character of an identifier,
> and consecutive underscores (`__`) are not permitted.
> Valid: `MAX_WIDTH`, `item_count`, `DIR_N`
> Invalid: `_name`, `done_`, `x__y`, `_`

---

## Do Not Change

- The `$` suffix on built-in string functions (`UPPER$`, `LEFT$`, `MID$` etc.)
- Any existing behaviour for identifiers that do not contain underscores
- Keywords — underscore support does not affect keyword recognition
- The symbol table case sensitivity rule
- No commit at end of implementation — leave staged for review
