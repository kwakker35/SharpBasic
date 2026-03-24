# The Sunken Crown — Issue 5 Generation Context
## Something in the Dark

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–4:

**Constants:** `FRAME_WIDTH`, `DIR_N/S/E/W/NE`, `MAX_EXITS = 30`, `MAX_ROOMS = 12`

**Globals:** `skill`, `startSkill`, `stamina`, `startStamina`, `luck`,
`currentRoom`, `visited[12]`, `searched[12]`, `gameOver`,
`roomExitStart[12]`, `roomExitCount[12]`, `exitDir[30]`, `exitDest[30]`, `exitHidden[30]`

**FUNCTIONs:** `RollDice(n)`, `DirName(dir)`, `RoomName(roomId)`

**SUBs:** `PrintHeader()`, `PrintSeparator()`, `PrintRoom(roomId)`,
`EnterRoom(roomId)`, `PrintExits(roomId)`, `HandleGo(dir)`,
`InitExits()`, `RollStartingStats()`, `OpeningSequence()`

---

## Scope — What This Issue Adds

**Constants:**
```
CONST MONSTER_BRUTE = 2
CONST MONSTER_HORROR = 6
CONST MONSTER_GUARDIAN = 7
CONST MONSTER_MAGE = 9
CONST MONSTER_TROLL = 10
CONST MONSTER_KING = 11
```

**Global variables:**
```
DIM monsterAlive[12] AS INTEGER   ' 0 = dead/absent, 1 = present and alive
LET minStamina = 0                ' lowest STAMINA reached this run
LET turns = 0                     ' turn counter
LET poisoned = 0                  ' 0 = clean, 1 = poisoned
```

**FUNCTIONs:**
```
FUNCTION AttackStrength(statSkill AS INTEGER) AS INTEGER
```

**SUBs:**
```
SUB InitMonsters()
SUB CombatLoop(monsterSkill AS INTEGER, monsterStamina AS INTEGER,
               hasArmour AS INTEGER, hasRegen AS INTEGER,
               hasLuckDrain AS INTEGER, hasLesserTerror AS INTEGER,
               hasPoison AS INTEGER, isBoss AS INTEGER,
               searchInterrupt AS INTEGER)
SUB HandleFight(roomId AS INTEGER)
SUB PrintDeathScreen()  REM temporary — replaced by PrintEndScreen in Issue 10
```

**Updated:**
- `InitMonsters()` called at startup alongside `InitExits()`
- `EnterRoom` — add encounter prompt when `monsterAlive[roomId] = 1`
- Command loop — add FIGHT command routing to `HandleFight(currentRoom)`
- Command loop — add death check after each action

**`CombatLoop` — mechanics to implement:**
- Both sides roll `AttackStrength` each round
- Higher wins the round
- Player winning: deal 2 damage to monster (1 if hasArmour=1 and no sword)
- Monster winning: deal 2 damage to player (3 if overburdened — flag not yet
  declared, will be added in Issue 6; assume 2 for now)
- Crushing Blow (isBoss=1): if monster wins by 3+, deal 4 instead of 2
- Lesser Terror (hasLesserTerror=1): SKILL -1 round 1 only, restored round 2
- Luck drain (hasLuckDrain=1): LUCK -1 each round regardless of outcome
- Regen (hasRegen=1): monster restores 1 STAMINA each round (cap at start value)
- Poison (hasPoison=1): on rounds monster wins, roll 1d6 — on 4+ set poisoned=1
- searchInterrupt=1: SKILL -1 round 1, additive with lesser terror penalty
- All global writes in CombatLoop use SET GLOBAL

**`HandleFight` — dispatcher for Issue 5:**
Only wires the Guardroom Brute (room 2). Other monsters get wired in Issues 7 and 9.
If no monster alive in room, print nothing-to-fight response.

**Monster stats — Guardroom Brute:**
```
mSkill = RollDice(1) + 7
mStamina = RollDice(2) + 10
```
Flags: all zero. Standard combat, no special mechanics.

---

## Exit State

Navigate to room 2. Encounter prompt appears. FIGHT triggers combat.
Rounds print with attack strengths and STAMINA values. Brute can be killed.
Player death prints death screen and ends run. Everything from Issues 1–4
continues to work.

---

## SET GLOBAL Audit

`CombatLoop` writes to: `stamina`, `luck`, `minStamina`, `poisoned`, `skill`
(lesser terror restore). All must use `SET GLOBAL`.

`HandleFight` writes to: `monsterAlive[roomId]` — array element, use `LET`.

`PrintDeathScreen` writes to: nothing (display only).

**PrintDeathScreen is temporary.** It is a minimal death handler for Issues 5–9.
Issue 10 introduces `PrintEndScreen` which handles all end states (not just death).
When Issue 10 is coded, `PrintDeathScreen` calls are replaced by `PrintEndScreen`
calls. Do not build `PrintDeathScreen` to do more than it needs to for now.

`InitMonsters` writes to: `monsterAlive[i]` — array elements, use `LET`.

**IMPORTANT — lesser terror and search interrupt restoration:**
`skill` is modified at the start of round 1 and must be restored at the end
of round 1. The restore must also use `SET GLOBAL skill = skill + penalty`.
The penalty value must be tracked in a local variable to know what to restore.

---

## Combat Tension Lines

Four threshold lines fire inside CombatLoop — all from Deliverable 8,
Combat Tension Lines section. Two flags track whether each has fired
this fight (local variables inside CombatLoop, reset each call):

```
LET playerLowFired = 0      ' 0 = not yet fired, 1 = fired this fight
LET playerCriticalFired = 0
LET monsterLowFired = 0
LET monsterCriticalFired = 0
```

- Player STAMINA ≤ 4 AND playerLowFired = 0 → print line, set playerLowFired = 1
- Player STAMINA ≤ 2 AND playerCriticalFired = 0 → print line, set playerCriticalFired = 1
- Monster STAMINA ≤ 4 AND monsterLowFired = 0 → print line, set monsterLowFired = 1
- Monster STAMINA ≤ 2 AND monsterCriticalFired = 0 → print line, set monsterCriticalFired = 1

These are local LET variables inside CombatLoop — correct, no SET GLOBAL needed.
Check thresholds at the end of each round after damage is applied.

---

## Text Strings Required

**Combat tension lines:** Deliverable 8, Combat Tension Lines section — all four lines
**Guardroom Brute first encounter:** Deliverable 2, Guardroom Brute, First encounter
**Guardroom Brute revisit (alive):** Deliverable 2, Guardroom Brute, Revisit — monster alive
**Guardroom Brute death text:** Deliverable 2, Guardroom Brute, Death text
**Nothing to fight:** Deliverable 4, string 6

UI chrome (inline):
- `"  YOUR ATTACK: "` & playerAttack & `"    MONSTER ATTACK: "` & monsterAttack
- `"  You win this round. Monster STAMINA: "` & monsterStamina
- `"  Monster wins this round. Your STAMINA: "` & stamina
- `"  A tie — neither side lands a blow."`

---

## Known Gotchas

**Sword of Sharpness not yet in inventory:**
The Sword item does not exist until Issue 6. For Issue 5, the armour reduction
check cannot test for the Sword. Implement armour reduction as: if hasArmour=1,
deal 1 damage. The Sword check will be added in Issue 6 as a modifier.
Document this as a TODO in a comment.

**overburdened not yet declared:**
The overburdened penalty (3 damage when overburdened) cannot be applied yet.
Use 2 damage as the standard for Issue 5. Add a TODO comment. Issue 6 adds
the overburdened flag and the penalty check.

**minStamina initialisation:**
`LET minStamina = 0` at global scope. But it should track the lowest stamina
reached. Initialise it to `stamina` after `RollStartingStats` in
`OpeningSequence` — or set it in `RollStartingStats` itself after rolling.
The value must be the rolled stamina, not 0.

**turns counter:**
`turns` is declared in Issue 5 but `AdvanceTurns` is not introduced until
Issue 7. For Issue 5, increment `turns` directly after combat:
`SET GLOBAL turns = turns + 1` per combat round. Or increment once after
combat ends. The exact per-turn increment pattern is established in Issue 7.
For now, increment once per FIGHT command call.

---

## Testing Checklist

- [ ] Navigate to room 2 — Brute encounter prompt appears
- [ ] FIGHT triggers combat loop
- [ ] Attack strengths print each round
- [ ] Player STAMINA decrements correctly on losing rounds
- [ ] Brute STAMINA decrements correctly on winning rounds
- [ ] Combat ends when Brute STAMINA reaches zero — death text prints
- [ ] Combat ends when player STAMINA reaches zero — death screen appears
- [ ] Brute is dead after combat — monsterAlive[2] = 0
- [ ] Revisiting room 2 after Brute death shows monster-dead description
- [ ] FIGHT in a room with no monster prints nothing-to-fight response
- [ ] Nothing from Issues 1–4 is broken

---

## Issue File Reference

`listing/issue-05-something-in-the-dark.md`
