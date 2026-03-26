# SharpBASIC — Add New Built-in Functions

## Branch
Create the new branch from `feat/the-sunken-crown`, not from `main`:
```
git checkout feat/the-sunken-crown
git checkout -b feat/new-builtins
```
Do all work on `feat/new-builtins`. Do not commit at the end — leave staged for review.
When ready to merge, merge back into `feat/the-sunken-crown`, not `main`.

## Overview
Add the following built-ins to SharpBASIC. Every one must be implemented using
the same registration mechanism as existing built-ins — registered in the built-in
array/dictionary, resolved by name before user-defined functions, protected from
SUB/FUNCTION shadowing. No null sentinels. No special-case branches outside the
normal built-in resolution path.

**Before writing any code:** read how `UPPER$` is registered end-to-end — from
the built-in array registration through to the call site in `EvaluateCallExpression`
(or equivalent). Follow that exact pattern for every new function below, including
how the `$` suffix is handled in the key. `STRING$` and `ASC` both have `$` suffix
equivalents already working — use the same key format.

---

## Functions to Add

### 1 — SLEEP(milliseconds AS INTEGER)
**Type:** Statement (void — no return value, cannot appear in an expression)
**Implementation:** `System.Threading.Thread.Sleep(milliseconds)`
**Errors:**
- Negative value → `SLEEP requires a non-negative integer argument`
- Non-integer → `SLEEP requires an integer argument`
- Used in expression context → parse or runtime error

```
SLEEP(1500)
SLEEP(0)       REM valid no-op
```

---

### 2 — STRING$(char AS STRING, count AS INTEGER) → STRING
**Type:** Function — returns STRING
**Implementation:** `new string(char[0], count)`
**Registration:** Follow the exact same pattern as `UPPER$` — same key format in
the built-in array, same resolution path in `EvaluateCallExpression`. The `$`
suffix must be handled consistently with existing string built-ins.
**Errors:**
- `char` length ≠ 1 → `STRING$ requires a single character as its first argument`
- Negative `count` → `STRING$ requires a non-negative integer as its second argument`
- `count = 0` → returns empty string (valid)

```
PRINT STRING$("=", 80)     REM prints 80 = characters
LET sep = STRING$("-", 40)
```

---

### 3 — ASC(char AS STRING) → INTEGER
**Type:** Function — returns INTEGER
**Implementation:** `(int)s[0]`
**Companion to:** `CHR$`
**Errors:**
- Empty string → `ASC requires a non-empty string argument`
- Multi-character string → uses first character only (consistent with C# behaviour)

```
PRINT ASC("A")     REM prints 65
PRINT ASC("a")     REM prints 97
```

---

### 4 — CINT(n) → INTEGER
**Type:** Function — returns INTEGER
**Implementation:** `(int)n` — truncates toward zero
**Purpose:** Solves the `INT()` float return problem. `INT(3.9)` returns `3.0` (Float).
`CINT(3.9)` returns `3` (Integer). No more `VAL(STR$(INT(n)))` workaround.
**Errors:** None beyond type mismatch if a non-numeric argument is passed.

```
LET i = CINT(3.9)      REM i = 3 (Integer)
LET j = CINT(RND() * 6 + 1)   REM proper integer die roll
```

---

### 5 — CLAMP(n, min, max) → same type as n
**Type:** Function — returns same numeric type as `n`
**Implementation:** `Math.Clamp(n, min, max)`
**Purpose:** Eliminates repetitive IF chains for bounding values. Used constantly
for STAMINA healing caps, minimum damage of 1, LUCK floor of 0 etc.
**Errors:**
- `min > max` → `CLAMP requires min to be less than or equal to max`
- Non-numeric arguments → type error

```
SET GLOBAL stamina = CLAMP(stamina + 4, 0, startStamina)
SET GLOBAL luck = CLAMP(luck - 1, 0, startLuck)
LET damage = CLAMP(damage, 1, 99)   REM minimum damage is 1
```

---

### 6 — MAX(a, b) → same type as a and b
**Type:** Function — returns numeric
**Implementation:** `Math.Max(a, b)`

```
PRINT MAX(3, 7)    REM prints 7
LET damage = MAX(rawDamage - armourReduction, 1)
```

---

### 7 — MIN(a, b) → same type as a and b
**Type:** Function — returns numeric
**Implementation:** `Math.Min(a, b)`

```
PRINT MIN(3, 7)    REM prints 3
SET GLOBAL stamina = MIN(stamina + heal, startStamina)
```

---

## Tests

Write all tests using xUnit and FluentAssertions.

### SLEEP

```
SLEEP(0)
PRINT "ok"
```
Expected: `ok`

```
SLEEP(100)
PRINT "done"
```
Expected: `done`. Verify at least 80ms elapsed with `Stopwatch`.

```
LET ms = 50
SLEEP(ms)
PRINT "ok"
```
Expected: `ok`

```
SLEEP(50 * 2)
PRINT "ok"
```
Expected: `ok`

```
CONST COMBAT_DELAY = 1500
SLEEP(COMBAT_DELAY)
PRINT "ok"
```
Expected: `ok`

```
SLEEP(-1)
```
Expected: runtime diagnostic containing `non-negative`

Cannot shadow:
```
SUB SLEEP(ms AS INTEGER)
    PRINT ms
END SUB
```
Expected: error at hoisting phase

Cannot use in expression:
```
LET x = SLEEP(100)
```
Expected: parse or runtime error

---

### STRING$

```
PRINT STRING$("=", 5)
```
Expected: `=====`

```
PRINT STRING$("*", 1)
```
Expected: `*`

```
PRINT STRING$("x", 0)
```
Expected: empty string (valid)

```
CONST FRAME_WIDTH = 80
PRINT LEN(STRING$("=", FRAME_WIDTH))
```
Expected: `80`

```
PRINT STRING$("-", 10) & "X" & STRING$("-", 10)
```
Expected: `----------X----------`

```
LET ch = "="
LET n = 20
PRINT STRING$(ch, n)
```
Expected: `====================`

```
PRINT STRING$("ab", 5)
```
Expected: runtime diagnostic containing `single character`

```
PRINT STRING$("", 5)
```
Expected: runtime diagnostic containing `single character`

```
PRINT STRING$("=", -1)
```
Expected: runtime diagnostic containing `non-negative`

Cannot shadow:
```
SUB STRING$(c AS STRING, n AS INTEGER)
    PRINT c
END SUB
```
Expected: error at hoisting phase

```
PRINT STRING$(CHR$(34), 3)
```
Expected: `"""`

---

### ASC

```
PRINT ASC("A")
```
Expected: `65`

```
PRINT ASC("a")
```
Expected: `97`

```
PRINT ASC(" ")
```
Expected: `32`

```
PRINT ASC(CHR$(65))
```
Expected: `65` — round-trip with CHR$

```
PRINT ASC("Hello")
```
Expected: `72` — uses first character only

```
PRINT ASC("")
```
Expected: runtime diagnostic containing `non-empty`

```
LET code = ASC("Z")
PRINT code
```
Expected: `90`

Cannot shadow:
```
SUB ASC(s AS STRING)
    PRINT s
END SUB
```
Expected: error at hoisting phase

---

### CINT

```
PRINT CINT(3.9)
```
Expected: `3`

```
PRINT CINT(3.1)
```
Expected: `3`

```
PRINT CINT(-3.9)
```
Expected: `-3` — truncates toward zero

```
PRINT CINT(0.0)
```
Expected: `0`

```
LET i = CINT(RND() * 6) + 1
PRINT i >= 1
PRINT i <= 6
```
Expected: `True`, `True` — proper integer die roll

```
LET f = 7.8
LET i = CINT(f)
PRINT i
```
Expected: `7`

REM Verify return type is actually INTEGER not FLOAT
```
LET i = CINT(3.9)
PRINT i + 1
```
Expected: `4` — integer arithmetic, not `4.0`

Cannot shadow:
```
SUB CINT(n AS FLOAT)
    PRINT n
END SUB
```
Expected: error at hoisting phase

---

### CLAMP

```
PRINT CLAMP(5, 1, 10)
```
Expected: `5`

```
PRINT CLAMP(0, 1, 10)
```
Expected: `1`

```
PRINT CLAMP(15, 1, 10)
```
Expected: `10`

```
PRINT CLAMP(1, 1, 10)
```
Expected: `1` — at lower boundary

```
PRINT CLAMP(10, 1, 10)
```
Expected: `10` — at upper boundary

```
LET stamina = 20
LET startStamina = 18
LET result = CLAMP(stamina, 0, startStamina)
PRINT result
```
Expected: `18` — healing cap use case

```
LET luck = -1
LET result = CLAMP(luck, 0, 12)
PRINT result
```
Expected: `0` — luck floor use case

```
PRINT CLAMP(5, 10, 1)
```
Expected: runtime diagnostic containing `min` and `max`

Cannot shadow:
```
SUB CLAMP(n AS INTEGER, lo AS INTEGER, hi AS INTEGER)
    PRINT n
END SUB
```
Expected: error at hoisting phase

---

### MAX

```
PRINT MAX(3, 7)
```
Expected: `7`

```
PRINT MAX(7, 3)
```
Expected: `7`

```
PRINT MAX(5, 5)
```
Expected: `5`

```
PRINT MAX(-3, -7)
```
Expected: `-3`

```
LET damage = MAX(rawDamage - 2, 1)
PRINT damage
```
Where `rawDamage = 2`: Expected: `1` — minimum damage use case

Cannot shadow:
```
SUB MAX(a AS INTEGER, b AS INTEGER)
    PRINT a
END SUB
```
Expected: error at hoisting phase

---

### MIN

```
PRINT MIN(3, 7)
```
Expected: `3`

```
PRINT MIN(7, 3)
```
Expected: `3`

```
PRINT MIN(5, 5)
```
Expected: `5`

```
PRINT MIN(-3, -7)
```
Expected: `-7`

```
LET healed = MIN(stamina + 4, startStamina)
PRINT healed
```
Where `stamina = 16`, `startStamina = 18`: Expected: `18` — healing cap use case
Where `stamina = 10`, `startStamina = 18`: Expected: `14`

Cannot shadow:
```
SUB MIN(a AS INTEGER, b AS INTEGER)
    PRINT a
END SUB
```
Expected: error at hoisting phase

---

## Language Spec Update

Update `language-spec-v1.md` as follows:

**String functions table — add:**
`STRING$(char, count)` | `String` | `char` repeated `count` times. `char` must be exactly one character. | `STRING$("=", 5)` → `"====="`
`ASC(s)` | `Integer` | Unicode code point of first character of `s` | `ASC("A")` → `65`

**Numeric functions table — add:**
`CINT(n)` | `Integer` | Truncate to integer toward zero | `CINT(3.9)` → `3`
`MAX(a, b)` | Same as inputs | Larger of two values | `MAX(3, 7)` → `7`
`MIN(a, b)` | Same as inputs | Smaller of two values | `MIN(3, 7)` → `3`
`CLAMP(n, min, max)` | Same as `n` | Constrain `n` between `min` and `max` | `CLAMP(15, 1, 10)` → `10`

**Statement built-ins — add:**
`SLEEP(ms)` | None | Pause for `ms` milliseconds. Non-negative integer only. | `SLEEP(1500)`

---

## Do Not Change

- Any existing built-in behaviour
- Keyword case-insensitivity — all new names work in any case
- The SUB/FUNCTION name collision detection — all new names must be protected
- The `$` suffix handling — `STRING$` and `ASC` must follow exactly the same
  registration pattern as `UPPER$` and `LEFT$`
- No commit at end — leave staged for review
