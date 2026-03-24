# The Sunken Crown — Issue 9 Generation Context
## The Bound King

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–8. All systems working. All rooms navigable.
Room 11 (Throne Room) currently has no special handling — the Bound King
is in `monsterAlive[11]` but no encounter sequence exists. FIGHT in room 11
falls through to the default no-monster response (King never wired in HandleFight).

---

## Scope — What This Issue Adds

**Global variables:**
```
LET goldBags = 0          ' bags taken from Throne Room this run
LET secondFight = 0       ' 0 = not triggered, 1 = regeneration triggered second fight
LET crownAvailable = 0    ' 0 = not yet found, 1 = revealed by SEARCH
LET terrorActive = 0      ' 0 = no terror, 1 = terror penalties active
LET endState = 0          ' 1=win, 2=became king, 3=riddle, 4=gate, 5=stamina
```

**SUBs:**
```
SUB BoundKingSequence()
SUB CrownSequence()
SUB HandleTakeBag()
```

**Updated:**
- `EnterRoom` — detect room 11 with monsterAlive[11]=1: route to `BoundKingSequence()`
- `HandleSearch` — room 11 after King beaten: if crownAvailable=1, reveal crown
  in search results alongside any remaining gold bags
- Command loop — intercept `TAKE CROWN` before general TAKE handler when
  currentRoom=11 and crownAvailable=1
- Command loop — TAKE BAG routes to HandleTakeBag when currentRoom=11
- `HandleFight` — room 11 handled by BoundKingSequence, not here
- `HandleSearch` — room 11 with King alive: refused (print refusal text, no combat)
- SNEAK — room 11 with King alive: refused (already handled if sneak checks room 11)

**`BoundKingSequence` logic:**

Step 1 — Bangle check:
`IF HasItem(ITEM_BANGLE_OF_COURAGE) = 0 THEN` apply terror:
```
SET GLOBAL terrorActive = 1
SET GLOBAL skill = skill - 2
IF luck > 0 THEN SET GLOBAL luck = luck - 1 END IF
```
Print terror text.

Step 2 — Roll boss stats:
```
LET bossSkill = RollDice(1) + 9
LET bossStamina = RollDice(2) + 18
```

Step 3 — First fight:
`CALL CombatLoop(bossSkill, bossStamina, 0, 0, 0, 0, 0, 1, 0)`

Step 4 — Restore terror (always, even if player died):
```
IF terrorActive = 1 THEN
    SET GLOBAL skill = skill + 2
    SET GLOBAL luck = luck + 1
    SET GLOBAL terrorActive = 0
END IF
```

Step 5 — If player alive after first fight:
Print post-combat beat. Set `LET monsterAlive[11] = 0`.
REM crownAvailable remains 0 — crown is not visible until SEARCH is called.
REM It will be set to 1 inside HandleSearch when the player searches room 11
REM after combat. Do NOT set it here.

Step 6 — Gold loop:
WHILE goldBags < MAX_GOLD_BAGS AND wantsMoreGold = 1 AND stamina > 0
  Print gold prompt. INPUT goldCmd$. UPPER$.
  TAKE ALL → print refused response.
  TAKE BAG → CALL HandleTakeBag()
  LEAVE → SET LOCAL wantsMoreGold = 0 (local LET is fine here)
  Other → unknown command response
WEND

Step 7 — Second fight if triggered:
IF secondFight = 1 AND stamina > 0 THEN
  Print second fight text.
  LET bossStamina = RollDice(2) + 18  REM local re-roll — bossStamina is not a global
  Apply terror if no Bangle.
  CALL CombatLoop(bossSkill, bossStamina, 0, 0, 0, 0, 0, 1, 0)
  Restore terror.
  If survived: print second fight survival text.
END IF

**`HandleTakeBag` logic:**
Increment goldBags. Call AddToInventory(ITEM_GOLD_BAG).
Regeneration rolls (bags 2, 3, 4):
- Bag 2: IF RollDice(2) >= 11 → trigger regen
- Bag 3: IF RollDice(2) >= 9 → trigger regen
- Bag 4: IF RollDice(2) >= 7 → trigger regen
If regen triggered: print regeneration beat text (his fingers move / hand closes /
draws a breath), SET GLOBAL secondFight = 1.

**`CrownSequence` logic:**
Print full crown sequence text (Deliverable 6).
SET GLOBAL endState = 2. SET GLOBAL stamina = 0. SET GLOBAL gameOver = 1.

**Constants:**
```
CONST MAX_GOLD_BAGS = 4
```

---

## Exit State

Full Throne Room encounter works. Terror, combat, gold mechanic, crown all
function. End states 2 (crown) and 5 (stamina in boss fight) work correctly.

---

## SET GLOBAL Audit

`BoundKingSequence` writes to: `terrorActive`, `skill`, `luck`, `secondFight`,
`crownAvailable` — all SET GLOBAL.
Also writes to: `monsterAlive[11]` — array element, use LET (not SET GLOBAL).
`HandleTakeBag` writes to: `goldBags`, `secondFight` — SET GLOBAL.
`AddToInventory` (called from HandleTakeBag) writes to: `invCount`, `overburdened` — SET GLOBAL.
`CrownSequence` writes to: `endState`, `stamina`, `gameOver` — SET GLOBAL.

**Note on wantsMoreGold:**
This is a local control variable inside `BoundKingSequence` — declared with
`LET wantsMoreGold = 1` inside the SUB. Since LET writes to local scope,
this is correct as-is. No SET GLOBAL needed for local control variables.

**Note on goldBags in the gold loop condition:**
The WHILE condition `WHILE goldBags < MAX_GOLD_BAGS` reads `goldBags` from
the global scope via the parent chain (GET walks up). This is correct.
`HandleTakeBag` writes to `goldBags` via SET GLOBAL. When the loop re-evaluates
its condition, it reads the updated global value. The two mechanisms work together
correctly — GET reads the global, SET GLOBAL updates it.

**Gold bags and full inventory — resolved:**
Gold bags are items (item code 11). The carry limit applies. If the player
arrives at the Throne Room carrying 4 items, AddToInventory will refuse the
gold bag and print the cannot-carry message. The player must DROP something
first. This is intentional — a strategic decision the player makes in the
Throne Room. No special handling needed.

---

## Text Strings Required

**Bound King first encounter:** Deliverable 2, Bound King, First encounter
**Bound King second fight:** Deliverable 2, Bound King, Revisit — second fight
**Post-combat beat:** Deliverable 6, Post-Combat Beat
**Regeneration beats (3 lines):** Deliverable 6, Regeneration Beat — Each Bag After the First
**TAKE ALL refused in Throne Room:** Deliverable 6, TAKE ALL Refused
**Crown search result:** Deliverable 6, Crown Search Result
**Crown sequence:** Deliverable 6, Crown Sequence — TAKE CROWN
**Crown end screen:** Deliverable 6, Crown End Screen
**Crushing Blow line:** Deliverable 8, Crushing Blow — Bound King Only
**SNEAK refused (Bound King):** Deliverable 8, Bound King — SNEAK Refused
**SEARCH refused (Bound King):** Deliverable 8, Bound King — SEARCH Refused
**TAKE before combat refused:** Deliverable 8, Bound King — TAKE Before Combat Refused

---

## Known Gotchas

**Terror restore must always run:**
Even if the player dies during the boss fight, the terror restore must execute.
This is because the end screen reads `skill` and `luck` — if they are not
restored, the stats on the end screen will be wrong.
Structure: call CombatLoop, then restore regardless of stamina.

**bossSkill declared locally:**
`LET bossSkill = RollDice(1) + 9` inside `BoundKingSequence`. This is a local
LET — correct. bossSkill is not a global. It is passed to CombatLoop as a
parameter. The second fight reuses the same bossSkill (same roll, per design log).

**Gold bags added to inventory:**
`AddToInventory(ITEM_GOLD_BAG)` adds item code 11 to inventory. If inventory
is full (4 items), AddToInventory will refuse and print the cannot-carry message.
This creates an edge case: player has full inventory and tries to take gold.
Resolution: gold bags bypass the normal carry limit? Or the player must DROP
something first? **Flag for designer confirmation.**
Resolved — see gold bags note above.

**Second fight — no gold collection after, no third fight:**
After the second fight, the gold loop does not run again under any circumstances.
`secondFight = 1` suppresses it permanently for this run. The player runs with
whatever they already have. There is no third regeneration check. This is final.

**Regeneration is bag-driven, not turn-driven:**
Regeneration rolls only fire when the player takes a bag. Leaving room 11 and
returning does not advance or reset the King's regeneration state. Time passing
in other rooms has no effect. The threat is greed, not the clock.

**goldBags persists across visits:**
`goldBags` is a global that is never reset during a run (only on play again).
If the player takes 1 bag and leaves, `goldBags = 1` on return. Taking bag 2
on the return visit fires the bag-2 regeneration roll (threshold 11+), not a
fresh bag-1 free take. The thresholds always match the total bags taken this run.

**Crown bypass inventory:**
`TAKE CROWN` is intercepted before `HandleTake`. It calls `CrownSequence`
directly. No inventory slot is consumed. No carry limit check. Correct per
Decision 11.

---

## Testing Checklist

- [ ] Enter room 11 — Bound King encounter description renders
- [ ] Without Bangle — terror text prints, SKILL -2 and LUCK -1 before combat
- [ ] With Bangle — terror text absent, stats unchanged
- [ ] Combat with boss — Crushing Blow fires when margin >= 3
- [ ] Terror stats restored after combat (with or without Bangle)
- [ ] Post-combat beat prints after King falls
- [ ] One gold bag free — no regeneration roll
- [ ] Bag 2 — regeneration roll fires (11+ on 2d6)
- [ ] Bag 3 — regeneration roll fires (9+ on 2d6)
- [ ] Bag 4 — regeneration roll fires (7+ on 2d6, ~58% trigger rate)
- [ ] Regeneration triggers second fight
- [ ] Second fight — King's stamina rerolled, player's not restored
- [ ] After second fight — no more gold collection
- [ ] TAKE ALL in Throne Room — refused response
- [ ] SEARCH room 11 after combat — crown appears
- [ ] TAKE CROWN — curse sequence fires, endState=2
- [ ] TAKE CROWN with full inventory — still fires
- [ ] SNEAK in room 11 — refused, no combat
- [ ] SEARCH in room 11 with King alive — refused, no combat
- [ ] Gold bags visible on end screen (inventory count, not goldBags counter)

---

## Issue File Reference

`listing/issue-09-the-bound-king.md`
