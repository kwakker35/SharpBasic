# fizzbuzz

The classic programming exercise: count from 1 to 30, substituting Fizz for multiples of 3, Buzz for multiples of 5, and FizzBuzz for multiples of both.

```
sharpbasic fizzbuzz.sbx
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `FOR`/`NEXT` loop | `FOR i = 1 TO 30 ... NEXT i` |
| `MOD` operator | `i MOD 3 = 0` |
| Nested `IF`/`ELSE`/`END IF` | Priority chain: 15 → 3 → 5 → default |
| `STR$` | Printing a number as text |

## Pedagogical note

The order of the `IF` checks matters: test `MOD 15` **first**, before `MOD 3` and `MOD 5`. Why? Because 15 is divisible by both 3 and 5 — if you test `MOD 3` first, multiples of 15 print "Fizz" instead of "FizzBuzz". This is a small but important lesson about evaluation order in conditional chains.
