# tic-tac-toe

A two-player Tic-Tac-Toe game with an ASCII board. Players X and O take turns choosing cells numbered 1-9.

```
sharpbasic tic-tac-toe.bas
```

## What the board looks like

```
 1 | 2 | 3
---+---+---
 4 | 5 | 6
---+---+---
 7 | 8 | 9
```

After a few moves:

```
 X | 2 | O
---+---+---
 4 | X | 6
---+---+---
 7 | 8 | O
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| Flat array as 2D grid | `DIM board[9]`; index = `row*3 + col` |
| `SUB` for rendering | `DrawBoard` — called each turn |
| `FUNCTION` returning `BOOLEAN` | `CheckWin`, `IsBoardFull` |
| Eight win conditions | Three rows + three columns + two diagonals |
| Turn switching | `IF currentMark = "X" THEN ... ELSE ...` |
| Input validation | Range check and "already taken" check |

## Pedagogical note

SharpBASIC arrays are 1D, so the 3×3 grid is stored flat. The mapping `index = cell - 1` converts a 1-based player input to a 0-based array index. This is a common pattern — **virtualise a 2D structure inside a 1D array** — you'll see it in embedded systems, image processing, and game engines.

`CheckWin` is deliberately repetitive (8 explicit conditions). This is the right call at this stage: it's crystal clear and easy to verify. In a language with more advanced features you'd compute the lines programmatically — that would be a good refactoring exercise.
