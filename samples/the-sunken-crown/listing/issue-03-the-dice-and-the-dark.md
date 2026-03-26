# The Sunken Crown
## Issue 3 — The Dice and the Dark

*The dungeon doesn't know who you are yet. Neither do you.*

---

## The Story So Far

Issues 1 and 2 built the frame and put the first room inside it. The dungeon looks like something. It responds to input. But it doesn't know who you are.

Before the gates close, you roll. Three numbers — SKILL, STAMINA, LUCK — generated at random, fixed for this run, and entirely beyond your control. That is Fighting Fantasy's founding bargain: the dice decide what you bring in, and what you do with it is up to you.

This issue rolls those numbers. It also writes the opening sequence — the narrative beats before play begins, the moment Malachar's men take everything you own, the bar dropping on the gates. The dungeon has been waiting. Now it knows you are here.

---

## What's New This Issue

- `FUNCTION RollDice(n AS INTEGER) AS INTEGER` — the single source of all randomness
- `SUB RollStartingStats()` — rolls SKILL, STAMINA, and LUCK, stores starting values
- `SUB OpeningSequence()` — narrative opening, INPUT pacing beats, gates close
- Placeholder stats from Issue 1 replaced with real rolled values
- `startStamina` and `startSkill` stored separately for healing cap and end screen

---

## Concept — FUNCTIONs and RND

A FUNCTION is like a SUB, but it gives something back. Where a SUB performs an action, a FUNCTION calculates a value and returns it to whoever called it.

Here is `RollDice` — the most important FUNCTION in the game:

```
FUNCTION RollDice(n AS INTEGER) AS INTEGER
    LET total = 0
    FOR i = 1 TO n
        LET total = total + CINT(RND() * 6) + 1
    NEXT i
    RETURN total
END FUNCTION
```

Break this down line by line:

- `FUNCTION RollDice(n AS INTEGER) AS INTEGER` — declares a function named `RollDice` that takes one integer parameter `n` and returns an integer
- `LET total = 0` — initialises the accumulator to zero
- `FOR i = 1 TO n` — loops `n` times — once per die being rolled
- `LET total = total + CINT(RND() * 6) + 1` — the die roll itself:
  - `RND()` returns a random number from 0.0 up to (but not including) 1.0
  - Multiply by 6 to get a range from 0.0 to 5.999...
  - `INT()` floors it to one of 0, 1, 2, 3, 4, or 5
  - Add 1 to shift the range to 1–6
  - Add the result to `total`
- `NEXT i` — go back and roll again
- `RETURN total` — send the accumulated total back to the caller


Call `RollDice(1)` for a single die roll. Call `RollDice(2)` for the sum of two dice. Every random event in this game — combat rounds, luck tests, monster stats, the loot shuffle — flows through this single FUNCTION. One source of truth for all randomness. If the game ever feels wrong, this is the first place to look.

---

## How It Fits

Issue 1 declared `skill`, `stamina`, and `luck` as placeholder zeros. Issue 3 replaces that placeholder with a call to `RollStartingStats()` in the opening sequence. From this issue forward, the stats in the header mean something.

The opening sequence runs once, before the main game loop. After it completes, the game enters room 1 exactly as before — except now the player has real attributes rolled from real dice.

---

## What You'll See

When you run the program, the opening sequence runs first — narrative text, INPUT pacing beats, attribute rolling displayed, and the gates closing. Then the Entry Hall renders with the header showing your actual rolled stats.

Every run produces different numbers. Roll a 7 SKILL and a 14 STAMINA and the dungeon feels entirely different to rolling a 12 SKILL and a 24 STAMINA. That variance is intentional. The dice decide what you bring in.

If the stats in the header still show zero after the opening sequence, check that `RollStartingStats()` is being called before `EnterRoom(1)`.

---

## What to Try

Add a temporary loop at the bottom of the file before you start playing:

```
FOR i = 1 TO 100
    PRINT RollDice(1)
NEXT i
```

Does the distribution look right? You are looking for roughly equal frequency across 1–6. Run it again. Different numbers, same spread. If any value never appears or dominates, something is wrong with the calculation — and everything that depends on it will be wrong too. Remove the test loop before Issue 4.

---

## The Listing

```
REM === ADD TO: player stats block, after LET luck = 0 ===
LET startSkill = 0   REM starting SKILL -- reference value for end screen
LET startStamina = 0 REM starting STAMINA -- healing cap; never changes after RollStartingStats
LET startLuck = 0 REM starting LUCK -- reference value for end screen

REM === NEW FUNCTION: add after SUB Pause(), before SUB PrintRoom() ===
REM =================================================================
REM  FUNCTION RollDice -- n AS INTEGER -> INTEGER
REM  Rolls n six-sided dice and returns the sum. Called with 1 or 2.
REM  Note: INT(Float) returns Float, so total promotes to Float after
REM  the first iteration. Return type is advisory -- the caller
REM  receives the Float value and arithmetic works correctly.
REM =================================================================
FUNCTION RollDice(n AS INTEGER) AS INTEGER
    LET total = 0
    FOR i = 1 TO n
        LET total = total + CINT(RND() * 6) + 1
    NEXT i
    RETURN total
END FUNCTION

REM === NEW SUB: add after FUNCTION RollDice() ===
REM =================================================================
REM  SUB RollStartingStats
REM  Rolls SKILL, STAMINA, and LUCK using Fighting Fantasy ranges.
REM  Stores starting values in startSkill and startStamina for the
REM  healing cap and end screen. Uses SET GLOBAL for all five writes.
REM =================================================================
SUB RollStartingStats()
    SET GLOBAL skill = RollDice(1) + 6
    SET GLOBAL startSkill = skill
    SET GLOBAL stamina = RollDice(2) + 12
    SET GLOBAL startStamina = stamina
    SET GLOBAL luck = RollDice(1) + 6
    SET GLOBAL startLuck = luck
END SUB

REM === NEW SUB: add after SUB RollStartingStats() ===
REM =================================================================
REM  SUB OpeningSequence
REM  Runs once at startup before the main game loop. Prints the
REM  opening narrative, pauses, rolls and displays starting
REM  attributes, then pauses again before the dungeon begins.
REM =================================================================
SUB OpeningSequence()
    PRINT ""
    PRINT "  You have sold everything. Your home, your tools, what little remained"
    PRINT "  after the debts. Malachar's men took it all — converted to coin, dropped"
    PRINT "  down a chute into the dark beneath the keep. You watched it go."
    PRINT ""
    PRINT "  If you don't walk out of that dungeon, there is nothing to walk back to."
    PRINT ""
    CALL Pause()
    PRINT ""
    PRINT "  Rolling your attributes..."
    PRINT ""
    CALL RollStartingStats()
    PRINT "  SKILL:    " & skill
    PRINT "  STAMINA:  " & stamina
    PRINT "  LUCK:     " & luck
    PRINT ""
    CALL Pause()
END SUB

REM === REPLACE: in the main program block at the bottom of the file ===
REM  Remove:  CALL EnterRoom(1)
REM  Add:     CALL OpeningSequence()
REM           CALL EnterRoom(currentRoom)
REM  (All lines above and below are unchanged.)
CALL OpeningSequence()
CALL EnterRoom(currentRoom)
```
