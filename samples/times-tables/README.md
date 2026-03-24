# times-tables

Prints the complete multiplication table from 1×1 to 12×12 using nested loops.

```
sharpbasic times-tables.sbx
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| Nested `FOR`/`NEXT` | Outer loop for rows, inner loop for columns |
| Integer arithmetic | `LET product AS INTEGER = i * j` |
| String building | `STR$(i) & " x " & STR$(j) & " = " & STR$(product)` |

## Pedagogical note

Nested loops are fundamental. The outer loop runs 12 times; for each iteration the inner loop runs 12 times — 144 iterations total. Notice how the inner `NEXT j` closes the inner loop while the outer `NEXT i` closes the outer one. Swapping them is a common mistake that the parser will reject with a mismatch error. The blank `PRINT ""` between groups is a simple separator — no extra formatting primitives needed.
