# wordle

A clone of the popular word game. You have 6 tries to guess a hidden 5-letter word. After each guess you get colour-coded feedback.

```
sharpbasic wordle.bas
```

## Feedback key

```
[A]  correct letter, correct position   (green)
(A)  correct letter, wrong position     (yellow)
.A.  letter not in the word             (grey)
```

### Example round

```
Guess 1: CRANE
.C.(R)[A].N.(E)

Guess 2: GREAT
.G.[R][E][A].T.

Guess 3: BREAD
[B][R][E][A][D]
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| Multiple `DIM` arrays | `wordList`, `result[5]`, `answerUsed[5]` inside a function |
| Two-pass scoring algorithm | Pass 1: greens; Pass 2: yellows (handles duplicates correctly) |
| `FUNCTION` returning `STRING` | `ScoreGuess` — 5-char score string `"GBGYB"` |
| `FUNCTION` returning `BOOLEAN` | `IsValidGuess` — length and alphabet checks |
| `SUB` for formatted output | `PrintResult` renders `[X] (X) .X.` notation |
| `WHILE`/`WEND` with two exit conditions | Guesses exhausted OR word found |
| Input normalisation | `UPPER$(TRIM$(rawInput))` |

## Pedagogical note

The two-pass scoring algorithm is the most interesting part of this program. A naïve single-pass approach gets duplicate letters wrong — for example, guessing `SPEED` against the answer `CRANE` would incorrectly mark both E's as yellow. The fix is to **lock in greens first**, mark those positions as used, and only then scan for yellows against the remaining unused letters.

This is a real algorithmic idea: **greedy matching with availability tracking**. The `answerUsed[5]` boolean array is the availability flag — once a position is "used" (by a green or a yellow), it can't match again.

Wordle is the hardest sample in this collection. If it runs correctly, your interpreter handles: nested function-scoped arrays, multi-parameter functions, complex string operations, and boolean logic across multiple passes. That's most of the language in one program.
