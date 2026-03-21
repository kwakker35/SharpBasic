# arrays

Declares a fixed-size array of exam scores, then computes total, average, and highest using loops.

```
sharpbasic arrays.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `DIM` declaration | `DIM scores[5] AS INTEGER` |
| Indexed assignment | `LET scores[0] = 74` |
| Indexed read | `scores[i]` inside a `FOR` loop |
| Running total | `LET total = total + scores[i]` |
| Max-finding loop | Compare each element against a running `highest` |

## Pedagogical note

Arrays in SharpBASIC are **0-indexed** and **fixed-size** — you must declare the size with `DIM` before using the array. The size is the number of elements (not the last index), so `DIM scores[5]` creates indices 0 through 4.

The three loops in this program are a good template for the most common array operations: **iterate and print**, **aggregate (sum)**, and **search (find maximum)**. Every language's standard library is built from these same primitives.
