# The Sunken Crown
## Issue 9 — The Bound King

*He has been in that room for three hundred years.*

---

## The Story So Far

Issues 1 through 8 built everything — the dungeon, the map, combat, inventory, atmosphere, the zombie, and the two trap rooms. Every monster is in the file. Every room has its description and its logic.

One room has been waiting. Room 11. The Throne Room. You can navigate there, but nothing happens when you arrive. The Bound King sits on his throne and does not respond.

This issue wires the final boss, the gold mechanic, and the crown. It is the most complex issue in the series — more state flags, more nested loops, more things that can happen in a single room than anywhere else in the game. It is also the payoff for everything that came before.

---

## What's New This Issue

- `SUB BoundKingSequence()` — the full Throne Room encounter from entry to resolution
- Terror mechanic — `terrorActive` flag, SKILL and LUCK modified before combat, restored after
- Crushing Blow — boss deals 4 damage when winning a round by 3 or more attack strength
- Gold mechanic — `goldBags` counter, regeneration rolls with tightening thresholds, second fight
- `SUB HandleTakeBag()` — taking individual bags of gold with regeneration checks
- Crown reveal — `crownAvailable` flag set after combat, crown appears on SEARCH
- `SUB CrownSequence()` — fires immediately on TAKE CROWN, bypasses inventory entirely
- `endState` variable — tracks how the run ended, read by the end screen in Issue 10
- `EnterRoom` updated to detect room 11 and route to `BoundKingSequence` while the King is alive

---

## Concept — Nested Loops and State Flags

The gold mechanic runs inside the Throne Room sequence — a loop within a loop, each bag taken triggering a regeneration check that could restart the fight:

```
LET wantsMoreGold = 1
WHILE goldBags < 4 AND wantsMoreGold = 1 AND stamina > 0
    REM prompt: TAKE BAG or LEAVE
    REM if TAKE BAG:
    LET goldBags = goldBags + 1
    IF goldBags > 1 THEN
        REM roll for regeneration — threshold tightens each bag
        IF regenRoll >= threshold THEN
            LET secondFight = 1
            LET wantsMoreGold = 0
        END IF
    END IF
WEND
```

Break this down:

- `WHILE goldBags < 4 AND wantsMoreGold = 1 AND stamina > 0` — three conditions, all must hold. The loop exits when the player has taken four bags, decides to leave, or is dead.
- `secondFight = 1` — a state flag. It does not restart the fight immediately. It signals to the code after the loop that the fight must happen again before the player can leave.
- After the loop, the code checks `secondFight`. If set, `CombatLoop` runs again with fresh monster STAMINA and the same SKILL. After the second fight, the gold loop does not run again.

State flags are how the game remembers what has happened and uses that memory to decide what comes next. `secondFight`, `crownAvailable`, `goldBags`, `terrorActive` — each one is a simple integer, 0 or 1. Composed together they produce the full complexity of the Throne Room.

The crown sequence is the sharpest example of a state flag doing decisive work. `crownAvailable` is 0 until SEARCH is called in room 11 after combat. The moment it becomes 1, `TAKE CROWN` becomes a valid command — intercepted before the general TAKE handler. No inventory slot, no hard limit interaction. The crown never enters the inventory. The curse transfers on contact.

This is how the whole game works. The Throne Room has more of it in one place than anywhere else. But the pattern is the same one used in every other room.

---

## How It Fits

`EnterRoom` gains a detection branch for room 11. If the Bound King is alive when the player enters, `BoundKingSequence` runs instead of the standard room entry. If the King has been beaten and the player returns, the standard revisit description renders.

`CrownSequence` is intercepted in the main command loop before the general TAKE handler. The order matters — `TAKE CROWN` must be checked first, or it will fall through to the general handler and attempt an inventory operation.

Terror is applied before `CombatLoop` and restored after. The restoration code must run whether the player wins or loses — if the player dies during a terror encounter, `stamina` is already zero and the death check fires on the next loop iteration, but the restoration still needs to happen to leave the stats in a clean state for the end screen.

---

## What You'll See

Navigate to room 11. The Bound King raises his head. Without the Bangle of Courage, Terror fires — SKILL drops by 2, LUCK by 1. Fight him. He falls.

The post-combat beat: *The King falls. You don't wait. You can already see his fingers moving.*

One bag is free. Take a second — regeneration rolls. Take a third — threshold tightens. Take a fourth — roughly 58% chance he gets up. If he does, fight again with what you have left.

Search the room after combat and the crown appears alongside any remaining bags.

Take the crown and the run ends differently.

---

## What to Try

Take all four bags every time you reach the Throne Room across multiple runs. Track how often the regeneration triggers at bag four. The design log says 7+ on 2d6 is roughly 58%. Play it enough times to feel what 58% means in practice — not as a number, but as a decision you have to make while something is pulling itself back together on the floor.

---

## The Listing

```
REM Issue 9 listing — to be added once built and tested
```
