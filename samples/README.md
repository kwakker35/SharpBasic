# SharpBASIC Samples

A collection of programs that showcase the SharpBASIC language, arranged in order of increasing complexity. Each sample has its own folder with a `.bas` source file and a `README.md` explaining what it teaches.

Run any sample with:

```
sharpbasic <sample-folder>/<sample-name>.bas
```

---

## Index

| # | Sample | What it covers | Complexity |
|---|--------|---------------|-----------|
| 1 | [hello-world](hello-world/) | `PRINT` — the starting point | ⭐ |
| 2 | [variables](variables/) | Typed variables, `LET`, `STR$`, `TYPENAME` | ⭐ |
| 3 | [user-input](user-input/) | `INPUT`, string concatenation with `&` | ⭐ |
| 4 | [calculator](calculator/) | `INPUT`, `VAL`, `STR$`, `IF`/`ELSE` decision tree | ⭐⭐ |
| 5 | [fizzbuzz](fizzbuzz/) | `FOR`/`NEXT`, `MOD`, nested conditionals | ⭐⭐ |
| 6 | [times-tables](times-tables/) | Nested `FOR`/`NEXT` loops | ⭐⭐ |
| 7 | [guess-the-number](guess-the-number/) | `WHILE`/`WEND`, `RND`, `INT`, attempt counter | ⭐⭐⭐ |
| 8 | [fibonacci](fibonacci/) | `FUNCTION`, `RETURN`, recursion, call stack | ⭐⭐⭐ |
| 9 | [string-utilities](string-utilities/) | `LEN`, `MID$`, `LEFT$`, `RIGHT$`, `UPPER$`, `LOWER$`, `TRIM$` | ⭐⭐⭐ |
| 10 | [arrays](arrays/) | `DIM`, indexed access, aggregate and search loops | ⭐⭐⭐ |
| 11 | [hangman](hangman/) | `SUB`, `FUNCTION`, boolean arrays, string ops, game loop | ⭐⭐⭐⭐ |
| 12 | [tic-tac-toe](tic-tac-toe/) | Flat array as 2D grid, ASCII board, win detection | ⭐⭐⭐⭐ |
| 13 | [wordle](wordle/) | Two-pass scoring algorithm, function-scoped arrays, complex game logic | ⭐⭐⭐⭐⭐ |
| 14 | [capstone](capstone/) | *Coming soon* — the full-language showcase | ⭐⭐⭐⭐⭐ |

---

## Language features by sample

| Feature | First introduced |
|---------|-----------------|
| `PRINT` | hello-world |
| `LET` / typed variables | variables |
| `STR$`, `TYPENAME` | variables |
| `INPUT`, `&` concat | user-input |
| `VAL`, `IF`/`ELSE`/`END IF` | calculator |
| `FOR`/`NEXT`, `MOD` | fizzbuzz |
| `WHILE`/`WEND`, `RND`, `INT` | guess-the-number |
| `FUNCTION`, `RETURN`, recursion | fibonacci |
| `LEN`, `MID$`, `LEFT$`, `RIGHT$`, `UPPER$`, `LOWER$`, `TRIM$` | string-utilities |
| `DIM`, array indexing | arrays |
| `SUB`, `CALL` | hangman |
| Multi-array programs, algorithms | wordle |
