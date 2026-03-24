# The Sunken Crown — Issue 7 Generation Context
## The Dungeon Breathes

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–6. Full navigation, combat, and inventory are working.

Key globals include: `turns`, `poisoned`, `monsterAlive[12]`, `inventory[4]`,
`invCount`, `overburdened`, all exit arrays, all room arrays.

---

## Scope — What This Issue Adds

**Global variables:**
```
LET zombieSpawned = 0     ' 0 = not yet spawned, 1 = in the dungeon
LET zombieAlive = 0       ' 0 = dead or unspawned, 1 = alive
LET zombieRoom = 0        ' current room (0 = unspawned)
LET luckDrainZeroFired = 0 ' 0 = message not yet shown, 1 = shown
```

**SUBs:**
```
SUB AdvanceTurns(n AS INTEGER)
SUB AtmosphericEvent()
SUB WanderZombie()
SUB HandleSneak()
```

**Updated:**
- `EnterRoom` — add poison damage check at top (before header)
- `EnterRoom` — add zombie encounter check at bottom (after room description)
- `HandleFight` — wire all remaining fixed monsters + zombie
- `HandleGo` — call `AdvanceTurns(1)` after successful navigation
- Command loop — add LOOK, SNEAK commands
- All SEARCH calls — `AdvanceTurns(2)` (SEARCH costs 2 turns)
- FIGHT call — `AdvanceTurns(1)` per combat round (inside CombatLoop via
  a call to AdvanceTurns, or one increment after combat ends — see gotchas)

**`AdvanceTurns` logic:**
```
SUB AdvanceTurns(n AS INTEGER)
    FOR i = 1 TO n
        SET GLOBAL turns = turns + 1
        IF RND() * 10 > 8.5 THEN
            CALL AtmosphericEvent()
        END IF
        IF zombieAlive = 1 THEN
            CALL WanderZombie()
        END IF
    NEXT i
END SUB
```

**`WanderZombie` logic:**
Read exits for `zombieRoom`. Build list of valid destinations (exclude rooms 5, 8, 11).
Pick one at random. Set `SET GLOBAL zombieRoom = dest`.
If no valid exits (shouldn't happen but possible), zombie does not move.

**`AtmosphericEvent` logic:**
Roll `INT(RND() * 12)` for event index 0–11.
Events 0–6: pure flavour text.
Events 7–9: mechanical (STAMINA -1, conditional).
Event 10: zombie spawn check — if `zombieSpawned = 0 AND turns > 3`, roll
`IF RND() * 10 > 7` — if true, spawn zombie at room 4.
Event 11: The Presence — check zombie proximity.

**`HandleFight` — wire remaining monsters:**
All 6 fixed monsters + zombie. Use SELECT CASE on roomId.
Monster stats and flags:

| Monster | Room | SKILL | STAMINA | Flags |
|---------|------|-------|---------|-------|
| Brute | 2 | RollDice(1)+7 | RollDice(2)+10 | none |
| Horror | 6 | RollDice(1)+5 | RollDice(1)+4 | hasLesserTerror=1, hasPoison=1 |
| Guardian | 7 | RollDice(1)+8 | RollDice(2)+8 | hasArmour=1 |
| Mage | 9 | RollDice(1)+6 | RollDice(1)+6 | hasLuckDrain=1 |
| Troll | 10 | RollDice(1)+7 | RollDice(2)+12 | hasRegen=1 |
| King | 11 | handled in Issue 9 | | |
| Zombie | roaming | RollDice(1)+5 | RollDice(1)+6 | none |

**Fixed item drops — wired in HandleFight/HandleSearch:**
- Brute (room 2): Guardroom Key on SEARCH after death (already in Issue 6)
- Guardian (room 7): Bangle of Courage on SEARCH after death
- Mage (room 9): Antidote Vial on SEARCH after death
- Troll (room 10): Mouldy Bread on SEARCH after death (item code 9, restores 3 STAMINA)

**Hollow Mage luck drain at zero:**
Inside CombatLoop, when `hasLuckDrain = 1` and `luck = 0`:
Print the existential dread message (Deliverable 8). Set `luckDrainZeroFired = 1`.
On subsequent rounds where luck is already 0, do not print again.

---

## Exit State

Atmospheric events fire during play. Zombie may spawn and wander. All fixed
monsters (except Bound King) are fully wired with correct stats and special
mechanics. Poison fires on room entry. Zombie encounter works. SNEAK works.

---

## SET GLOBAL Audit

`AdvanceTurns` writes to `turns` — SET GLOBAL.
`WanderZombie` writes to `zombieRoom` — SET GLOBAL.
`AtmosphericEvent` writes to `zombieSpawned`, `zombieAlive`, `zombieRoom`,
`stamina`, `minStamina` — all SET GLOBAL.
`HandleSneak` writes to nothing (result is local — sneak either succeeds
or routes to HandleFight). Actually: if sneak fails, combat starts. If
sneak succeeds, player stays. No global writes needed in HandleSneak itself.
`EnterRoom` writes to `stamina`, `minStamina` for poison — SET GLOBAL.

**luckDrainZeroFired:** This is a per-combat flag. Declare it as a global
initialised to 0. Reset to 0 at the start of each CombatLoop call. Set to 1
when the message fires. SET GLOBAL for the write inside CombatLoop.

---

## Text Strings Required

**All 12 atmospheric events:** Deliverable 3, all events
**Hollow Mage luck drain at zero:** Deliverable 8
**Poison room entry message:** Deliverable 8
**General low STAMINA (outside combat):** Deliverable 8, General Low STAMINA — Outside Combat
**Zombie first encounter:** Deliverable 2, Wandering Zombie, First encounter
**Zombie revisit alive:** Deliverable 2, Wandering Zombie, Revisit — monster alive
**Zombie death text:** Deliverable 2, Wandering Zombie, Death text
**SEARCH interrupt — zombie (room-specific):** Deliverable 8, SEARCH Interrupt — Wandering Zombie
**All fixed monster encounter/revisit/death text:** Deliverable 2 (Horror, Guardian, Mage, Troll)
**Sneak success:** Deliverable 4, string 7
**Sneak failure:** Deliverable 4, string 8

---

## Known Gotchas

**AdvanceTurns and combat:**
Combat rounds each cost 1 turn. The cleanest approach is to call
`CALL AdvanceTurns(1)` at the end of each combat round inside CombatLoop.
This means atmospheric events and zombie wandering happen mid-combat, which
is correct — the dungeon doesn't pause while you fight.

**Zombie SEARCH interrupt — random room-specific lines:**
The zombie interrupt text in Deliverable 8 has 3 lines per room. Select
randomly: `INT(RND() * 3)` to pick 0, 1, or 2, then use SELECT CASE.

**WanderZombie — valid exit list:**
Build a local array of valid destinations. Each call to `WanderZombie` creates
a fresh local scope — local DIM declarations are re-created on every call, not
persisted between calls. This is correct and expected behaviour for local arrays.
Use: `DIM validDests[4] AS INTEGER` and `LET validCount = 0` as local variables.
The array is populated fresh each call from the exit arrays.

**Sneak mechanics:**
The design log says "flat SKILL check — same difficulty for all sneakable monsters."
No per-monster modifier. Proposed formula (awaiting confirmation):
`IF RollDice(2) <= skill THEN success`
This mirrors the TestLuck pattern — roll 2d6, succeed if <= stat. Gives ~58% at
SKILL 7, ~72% at SKILL 9, ~97% at SKILL 12. Consistent with the game's existing
probability model.
**Confirmed formula: `IF RollDice(2) <= skill THEN success`**
Mirrors TestLuck pattern. No per-monster modifier. ~58% at SKILL 7, ~72% at SKILL 9.
On success: print sneak success text, player passes. Monster is not killed —
no loot available from that room via SEARCH (monster alive blocks SEARCH).
On failure: print sneak failure text, trigger HandleFight with searchInterrupt=0.
Bound King: sneak refused regardless — check `IF currentRoom = 11` first,
print boss sneak refusal (Deliverable 8) and return before any roll.

**Zombie proximity for The Presence event:**
Check if `zombieRoom` is within 2 rooms of `currentRoom`. Simple absolute
difference: `IF ABS(zombieRoom - currentRoom) <= 2`. This is approximate
given the non-linear map layout but is sufficient for this mechanic.

---

## Testing Checklist

- [ ] Atmospheric events fire during navigation (not every turn, not never)
- [ ] Zombie spawns after several turns
- [ ] Zombie wanders — room changes turn by turn
- [ ] Zombie encounter prompt appears when zombie enters player's room
- [ ] FIGHT against zombie works correctly
- [ ] SNEAK past zombie works (roughly, depends on SKILL)
- [ ] Sneak failure triggers combat
- [ ] Poison fires before room description on room entry
- [ ] Poison clears after USE Antidote Vial
- [ ] Skittering Horror — Lesser Terror fires round 1 only
- [ ] Skittering Horror — Poison may trigger on monster-winning rounds
- [ ] Pit Guardian — Armour reduces damage to 1 without sword
- [ ] Pit Guardian — Bangle drops on SEARCH after death
- [ ] Hollow Mage — LUCK drains each round
- [ ] Hollow Mage — existential dread message fires once when luck hits 0
- [ ] Troll — regenerates 1 STAMINA per round
- [ ] LOOK command redraws room
- [ ] AdvanceTurns called correctly for each action type

---

## Issue File Reference

`listing/issue-07-the-dungeon-breathes.md`
