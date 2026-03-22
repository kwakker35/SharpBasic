# The Sunken Crown
## Issue 10 — The Gate

*Two doors. No label. No instruction. The dungeon's last move.*

---

## The Story So Far

Issues 1 through 9 built everything. The frame, the map, the attributes, the combat, the inventory, the atmosphere, the zombie, the trap rooms, and the final boss. Every system is in place. Every room has its logic.

One room is not yet wired. Room 12. The Gate. You can navigate there, but nothing happens. Two doors sit in the far wall and the program does not know what to do with them.

This issue finishes the game.

---

## What's New This Issue

- `SUB InitGate()` — assigns the correct door at startup, 1=LEFT or 2=RIGHT
- `SUB GateSequence()` — the final two-door resolution, accepts LEFT or RIGHT only
- Win path — correct door, Malachar in the courtyard, freedom
- Death path — wrong door, the corridor that goes nowhere, darkness
- `SUB PrintEndScreen()` — displays run statistics and the play again prompt
- Play again loop — all state resets and the full game restarts on YES
- `EnterRoom` updated to detect room 12 and route to `GateSequence`
- STATS and HELP commands added to the command loop
- Complete MAIN block — the play again loop wraps the entire game

---

## Concept — Putting It Together

There is no new SharpBASIC feature in this issue. The Gate uses INPUT, IF, and PRINT — the same tools as Issue 1. What has changed is everything around them.

The complete game is a single `.sbx` file. No modules, no imports, no external dependencies. SharpBASIC's scope — ten phases, one file, a real runnable language — was always pointed at this moment. The constraint was the point.

Here is what happens when the player types LEFT or RIGHT at The Gate:

```
INPUT cmd$
LET cmd$ = UPPER$(cmd$)
IF cmd$ = "LEFT" THEN
    IF gateCorrectDoor = 1 THEN
        ' win sequence
    ELSE
        ' death sequence
    END IF
ELSE
    IF cmd$ = "RIGHT" THEN
        IF gateCorrectDoor = 2 THEN
            ' win sequence
        ELSE
            ' death sequence
        END IF
    ELSE
        PRINT "  The dungeon does not respond to that."
    END IF
END IF
```

The same INPUT-UPPER$-IF pattern used since Issue 2. The same nested IF structure used since Issue 8. The code is simple. What surrounds it — ten issues of building, every system connected, the entire dungeon held in one file — is not.

What you have built is a tree-walking interpreter executing a program that tells a story. Every PRINT statement passes through a lexer, a parser, an AST, and an evaluator before a character appears on screen. You built all of those too. The dungeon runs on an engine you made by hand.

That is not a small thing.

---

## How It Fits

`GateSequence` is the last special room handler added to `EnterRoom`. After this issue, `EnterRoom` routes correctly for all twelve rooms — standard entry for most rooms, dedicated sequences for rooms 5, 8, 11, and 12.

The play again loop is the outermost structure of the entire MAIN block. On YES, every state variable resets to its starting value and every array is cleared. `InitExits`, `InitMonsters`, `ShuffleLoot`, `InitRiddle`, and `InitGate` all run again. The opening sequence runs. A new run begins. Every run is a fresh game with fresh dice.

The end screen reads `endState` to select its message. The state was set wherever the run ended — STAMINA zero, Riddle Room wrong door, Gate wrong door, Gate correct door, or TAKE CROWN. The end screen is the same SUB regardless of outcome.

---

## What You'll See

Navigate to room 12. The Gate description renders. The prompt accepts only LEFT or RIGHT. Choose the less worn door. If correct, the win sequence runs — Malachar in the courtyard, his single word, freedom. If wrong, the dungeon keeps you quietly.

The end screen shows your stats. Play again and everything resets. Different attributes, different loot, different riddle, different gate. The dungeon is learned across runs, not within them.

Play it through at least twice. On the second run you know things you didn't on the first. That is the game working exactly as designed.

---

## What to Try

Change the ending. Not the mechanics — the text. The win text, the death text, the end screen message. Make it yours. The published version uses text from the content asset file. Your copy does not have to.

A game you have modified, even slightly, is a game you understand differently than one you only played. The text is in the content asset file. The code just calls PRINT. Change what gets printed and the ending changes. That is all a text adventure is, at its foundation.

---

## Congratulations

You have built The Sunken Crown — a complete, playable text adventure — across ten issues, each one adding one layer to a single `.sbx` file.

More than that: you built it in SharpBASIC v1 — a language you also built. The lexer, the parser, the AST, the tree-walking evaluator. The dungeon runs on an engine you made by hand.

That was always the point.

---

## The Listing

```
' Issue 10 listing — to be added once built and tested
```
