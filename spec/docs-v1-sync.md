# Feature Spec: Docs & Samples v1 Sync

**Branch:** `docs/v1-feature-sync`  
**Priority:** Do LAST — after all 5 features merged to `main`  
**Status:** ⬜ Not started

---

## Goal

Synchronise all documentation, sample programs, and phase specs to reflect
the five v1 features (`SET GLOBAL`, `CHR$`, `CONST`, `SELECT CASE`, `2D arrays`).

---

## Files to Update

### `docs/language-spec-v1.md`

Add or extend the following sections:

| Section | Change needed |
|---------|---------------|
| Variables & Scope | Add `SET GLOBAL` explanation + example. Correct old description if it says `Set` writes upward (it doesn't). |
| Constants | New sub-section: `CONST name = literal` syntax, rules, errors |
| Built-in Functions | Add `CHR$(n)` entry |
| Control Flow | Add `SELECT CASE … END SELECT` syntax + example |
| Arrays | Add 2D array `DIM name(r, c)` syntax, access, assignment |

---

### `learning/phase-specs/` — phase notes

| File | Change |
|------|--------|
| `phase-03-variables.md` | Add note on `CONST` and `SET GLOBAL` (forward references OK) |
| `phase-10-stdlib.md` | Add `CHR$` to built-in function list |

---

### `samples/` programs — review each

| Sample | Update needed? |
|--------|----------------|
| `variables/variables.sbx` | Add `CONST` example and `SET GLOBAL` example |
| `calculator/calculator.sbx` | Consider `SELECT CASE` for operator dispatch |
| `fizzbuzz/fizzbuzz.sbx` | No change expected |
| `fibonacci/fibonacci.sbx` | No change expected |
| `guess-the-number/guess-the-number.sbx` | Consider `CONST MAX_TRIES` |
| `hello-world/hello-world.sbx` | No change expected |
| `arrays/arrays.sbx` | Add 2D array demonstration |
| `string-utilities/string-utilities.sbx` | Add `CHR$` example |
| `user-input/user-input.sbx` | No change expected |
| `tic-tac-toe/tic-tac-toe.sbx` | Consider 2D board array, `SELECT CASE` for menu |
| `times-tables/times-tables.sbx` | No change expected |
| `wordle/wordle.sbx` | Review whether any of the 5 features would simplify it |
| `hangman/hangman.sbx` | Review |

> **The Sunken Crown is explicitly excluded from this task.**
> It will be updated separately as a game development milestone.

---

### `README.md` (root)

- Bump version if appropriate
- Add brief mention of `SELECT CASE`, `CONST`, 2D arrays if the README covers the language

---

## Acceptance Criteria

- [ ] All new syntax appears in `language-spec-v1.md` with examples
- [ ] `samples/arrays/arrays.sbx` demonstrates 2D arrays
- [ ] `samples/variables/variables.sbx` demonstrates `CONST` and `SET GLOBAL`
- [ ] `samples/string-utilities/string-utilities.sbx` demonstrates `CHR$`
- [ ] All samples pass the interpreter (manual smoke test with REPL or file runner)
- [ ] No phase spec contradicts the implemented behaviour

---

## TDD Note

This is a documentation-only task. No new tests.  
Run the sample `.sbx` files through the interpreter to verify they execute without error.
