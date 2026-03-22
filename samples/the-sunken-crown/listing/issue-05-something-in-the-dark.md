# The Sunken Crown
## Issue 5 — Something in the Dark

*The Guardroom Brute is the first thing in the dungeon that wants to hurt you.*

---

## The Story So Far

Issues 1 through 4 built the frame, the opening sequence, and the full navigable map. The dungeon has a shape. You can walk through it. The rooms describe themselves.

But nothing pushes back yet.

This issue changes that. The Guardroom Brute is the teaching encounter — the first fight, the simplest mechanics, the moment the dungeon becomes dangerous. Everything about combat is established here. Every other monster in the game uses the same system.

---

## What's New This Issue

- `FUNCTION AttackStrength(statSkill AS INTEGER) AS INTEGER` — calculates attack strength for one side
- `SUB CombatLoop(...)` — the parameterised combat engine, handles all monster encounters
- `SUB InitMonsters()` — sets all fixed monsters to alive at startup
- Guardroom Brute wired to room 2 — FIGHT works, the Brute has stats, it can kill you
- `monsterAlive` array — tracks which monsters are still alive across the dungeon
- `minStamina` tracking — records the lowest STAMINA reached this run for the end screen
- Death handling — STAMINA reaching zero ends the run

---

## Concept — WHILE Loops

Combat runs in a loop. The loop continues while both sides are still standing:

```
WHILE stamina > 0 AND monsterStamina > 0
    LET playerAttack = AttackStrength(skill)
    LET monsterAttack = AttackStrength(monsterSkill)
    ' compare, apply damage, print result
WEND
```

Break this down:

- `WHILE stamina > 0 AND monsterStamina > 0` — the loop condition. Both sides must be alive for the loop to continue. The moment either reaches zero, the condition becomes false and the loop exits immediately.
- `AND` — both parts must be true. If `stamina` drops to zero, `stamina > 0` is false. The `AND` makes the whole condition false. The loop stops.
- `AttackStrength(skill)` — calls a FUNCTION that rolls two dice and adds the SKILL value. Higher result wins the round.
- The loop body compares the two values, applies damage to the loser, and prints what happened.

After the loop exits, the code checks which side ran out of STAMINA first. Monster STAMINA at zero — player won. Player STAMINA at zero — run is over.

`WHILE` checks the condition before each iteration, not after. If both sides somehow entered the loop already at zero — which should never happen, but is worth understanding — the loop body never runs at all. The check comes first, always.

The `AND` compound condition is used throughout the game wherever two things must both be true to continue. It appears in the gold mechanic loop, the zombie encounter check, and the inventory system. Learn its shape here.

`CombatLoop` is parameterised — it takes monster stats and special flags as parameters. Every monster from the Brute to the Bound King runs through the same SUB. The mechanics differ by what is passed in, not by separate combat code for each creature.

---

## How It Fits

Issue 4 added navigation but no encounters. This issue adds the first encounter to room 2. `EnterRoom` is updated to check whether a monster is present and print the encounter hint if so. The command loop gains a FIGHT branch.

`InitMonsters()` is called at startup alongside `InitExits()` — all fixed monsters start alive. The `monsterAlive` array tracks their state throughout the run. When a monster is killed, `monsterAlive[room]` is set to 0 and the room description changes on revisit.

---

## What You'll See

Navigate to room 2. The room description includes the Brute. The prompt shows FIGHT as an available action. Type `FIGHT` and the combat loop runs — attack strengths printed each round, STAMINA falling on both sides. Win and the Brute is dead. Its body stays in the room on revisit. Lose and the run ends with a stats summary.

If combat never ends, check the loop condition — both STAMINA values must be able to reach zero. If the Brute never dies regardless of how many rounds pass, check that damage is being applied to `monsterStamina` and not to a local copy.

---

## What to Try

Raise the Brute's SKILL by 3 and fight it again. Then lower yours by 3 instead. The same loop, the same code, a completely different experience. The numbers are the game. Understanding how sensitive the balance is now will inform every monster stat decision for the remaining seven creatures.

---

## The Listing

```
' Issue 5 listing — to be added once built and tested
```
