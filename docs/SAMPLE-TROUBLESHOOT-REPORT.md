# SharpBASIC — Sample Troubleshooting Report
Generated: 2026-03-22

## Summary

| Metric | Count |
|--------|-------|
| Total samples | 13 |
| Passed (no changes needed) | 4 |
| Fixed successfully | 9 |
| Needs manual review | 0 |

---

## Results by Sample

| Sample | Status | Root Cause | Fix Applied |
|--------|--------|------------|-------------|
| samples/hello-world/hello-world.sbx | PASS | — | — |
| samples/fizzbuzz/fizzbuzz.sbx | PASS | — | — |
| samples/fibonacci/fibonacci.sbx | PASS | — | — |
| samples/user-input/user-input.sbx | PASS | — | — |
| samples/variables/variables.sbx | FIXED | `LET name AS TYPE = value` syntax; `STR$()` called on Boolean | Removed `AS TYPE` annotations; replaced `STR$(isOpen)` with `isOpen` (spec §4.1, §10) |
| samples/string-utilities/string-utilities.sbx | FIXED | `LET name AS TYPE = value` syntax; `MID$(word, 7, 5)` exceeds string length | Removed `AS TYPE` annotations; corrected `MID$` start position from 7 to 6 (spec §4.1, §10) |
| samples/arrays/arrays.sbx | FIXED | `LET name AS TYPE = value` syntax | Removed `AS TYPE` annotations (spec §4.1) |
| samples/calculator/calculator.sbx | FIXED | `LET name AS TYPE = value` syntax | Removed `AS TYPE` annotations (spec §4.1) |
| samples/times-tables/times-tables.sbx | FIXED | `LET name AS TYPE = value` syntax | Removed `AS TYPE` annotation (spec §4.1) |
| samples/guess-the-number/guess-the-number.sbx | FIXED | `LET name AS TYPE = value` syntax | Removed `AS TYPE` annotations (spec §4.1) |
| samples/tic-tac-toe/tic-tac-toe.sbx | FIXED | `LET name AS TYPE = value` syntax; `Boolean = TRUE/FALSE` comparisons | Removed `AS TYPE` annotations; replaced `= TRUE`/`= FALSE` with direct Boolean or `NOT` (spec §4.1, §6.3) |
| samples/hangman/hangman.sbx | FIXED | `LET name AS TYPE = value` syntax; `INT()` Float result used as array index; `Boolean = TRUE/FALSE` comparisons | Removed `AS TYPE` annotations; wrapped `INT(RND())` with `VAL(STR$())` to convert Float→Integer; replaced Boolean comparisons with direct use or `NOT` (spec §4.1, §5.2, §6.3, §10) |
| samples/wordle/wordle.sbx | FIXED | `LET name AS TYPE = value` syntax; `INT()` Float result used as array index; `Boolean = TRUE/FALSE` comparisons | Removed `AS TYPE` annotations; wrapped `INT(RND())` with `VAL(STR$())` to convert Float→Integer; replaced Boolean comparisons with direct use or `NOT` (spec §4.1, §5.2, §6.3, §10) |

---

## Needs Manual Review

None. All samples were fixed successfully.
