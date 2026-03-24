# The Sunken Crown — Issue 6 Generation Context
## What You Carry

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–5. Key additions to note:

**Globals include:** `monsterAlive[12]`, `minStamina`, `turns`, `poisoned`

**SUBs include:** `CombatLoop(...)`, `HandleFight(roomId)`, `InitMonsters()`

**FUNCTIONs include:** `RollDice(n)`, `AttackStrength(statSkill)`

---

## Scope — What This Issue Adds

**Constants — item codes (all locked):**
```
CONST ITEM_EMPTY = 0
CONST ITEM_HEALING_POTION = 1
CONST ITEM_LUCKY_CHARM = 2
CONST ITEM_ARMOUR_SHARD = 3
CONST ITEM_DARK_BREAD = 4
CONST ITEM_MEDAL_OF_VALOUR = 5
CONST ITEM_ANTIDOTE_VIAL = 6
CONST ITEM_BANGLE_OF_COURAGE = 7
CONST ITEM_SWORD_OF_SHARPNESS = 8
CONST ITEM_MOULDY_BREAD = 9
CONST ITEM_GUARDROOM_KEY = 10
CONST ITEM_GOLD_BAG = 11
CONST MAX_INVENTORY = 4
CONST LOOT_SLOTS = 6
```

**Global variables:**
```
DIM inventory[4] AS INTEGER       ' item code per slot, 0 = empty
LET invCount = 0                  ' items currently carried
LET overburdened = 0              ' 0 = normal, 1 = carrying 4 items
LET medalTaken = 0                ' 0 = not yet picked up, 1 = penalty applied
LET armouryLocked = 1             ' 0 = unlocked, 1 = locked
DIM slotContents[6] AS INTEGER    ' item code per loot slot
DIM slotTaken[6] AS INTEGER       ' 0 = available, 1 = collected
DIM slotRoom[6] AS INTEGER        ' which room each loot slot belongs to
DIM roomItems[12][12] AS INTEGER  ' item codes in each room, 0 = empty slot
DIM roomItemCount[12] AS INTEGER  ' number of items currently in each room
```

**FUNCTIONs:**
```
FUNCTION ItemName(code AS INTEGER) AS STRING
FUNCTION HasItem(code AS INTEGER) AS INTEGER
```

**SUBs:**
```
SUB ShuffleLoot()
SUB AddToInventory(itemCode AS INTEGER)
SUB RemoveFromInventory(code AS INTEGER)
SUB HandleTake(itemName$ AS STRING)  REM itemName$ is UPPER$-normalised player input
SUB HandleDrop(itemName$ AS STRING)
SUB HandleUse(itemName$ AS STRING)
SUB HandleSearch()
SUB PrintInventoryScreen()
SUB PrintRoomItems(roomId AS INTEGER)
```

**Updated:**
- `CombatLoop` — add overburdened damage check (3 if overburdened, 2 if not)
- `CombatLoop` — add Sword of Sharpness check for armoured monsters
- `EnterRoom` — call `PrintRoomItems(roomId)` to show visible room contents
- `ShuffleLoot()` called at startup alongside `InitExits()` and `InitMonsters()`
- Command loop — add TAKE, DROP, USE, SEARCH, INVENTORY commands

**`ShuffleLoot` — pool:**
Items 1–5 (Healing Potion, Lucky Charm, Armour Shard, Dark Bread, Medal of Valour)
plus one empty slot (0). Six items, six slots. Fisher-Yates shuffle (0-indexed).
Slot room assignments: slotRoom[0]=1, slotRoom[1]=2, slotRoom[2]=4,
slotRoom[3]=6, slotRoom[4]=8, slotRoom[5]=10.

**Fixed items (not shuffled):**
- Antidote Vial — placed in room 9 on SEARCH after Hollow Mage dead (Issue 7)
- Bangle of Courage — dropped by Pit Guardian on SEARCH after combat (Issue 7)
- Sword of Sharpness — in Armoury chest (room 3), revealed when armouryLocked=0
- Guardroom Key — dropped by Brute on SEARCH after combat
- Mouldy Bread — fixed Troll drop, room 10. Found on SEARCH after Troll dead.
  Restores 3 STAMINA on USE. Always present — not shuffled. Item code 9.

**`HandleSearch` — room-specific logic:**
- Room 2 (Brute dead): reveal Guardroom Key + shuffled slot if present
- Room 3 (armouryLocked=0): reveal Sword of Sharpness
- Room 6: only two valid description states exist:
  1. Horror alive — skittering revisit text, passage hidden (the only monster-alive state)
  2. Horror dead — check exitHidden[11]: if still 1 show passage-not-found text,
     if 0 show passage-found text
  The "monster alive, passage found" combination is impossible — SEARCH while the
  Horror lives triggers combat, so the passage can never be found while it lives.
  Do not implement a code path for this combination.
- Room 6 (Horror dead, SEARCH called): reveal hidden south exit (set exitHidden[11]=0)
  Note: Horror must be dead before SEARCH reveals the passage — but it is SEARCH
  that reveals it, not the kill. A player who kills the Horror and leaves without
  searching will not see the south exit on return until they SEARCH.
- Other rooms: reveal shuffled slot if present
- Searched flag set after first search
- Second search prints already-searched response
- Monster alive: trigger interrupt (SEARCH interrupt — calls HandleFight)

**`PrintRoomItems` — display items visible in room:**
Shows items from `roomItems[roomId]` array. Called from `EnterRoom`.
Items dropped by player, items revealed by SEARCH, fixed drops — all visible.

---

## Exit State

Full inventory system working. TAKE, DROP, USE, SEARCH, INVENTORY all function.
Brute drops key. Key unlocks Armoury. Sword revealed in Armoury. Overburdened
state applies movement penalty and combat penalty.

---

## SET GLOBAL Audit

`AddToInventory` writes to: `inventory[slot]` (LET), `invCount` (SET GLOBAL),
`overburdened` (SET GLOBAL), `skill` if medal taken (SET GLOBAL), `medalTaken` (SET GLOBAL).

`RemoveFromInventory` writes to: `inventory[slot]` (LET), `invCount` (SET GLOBAL),
`overburdened` (SET GLOBAL).

`HandleUse` writes to: `stamina` (SET GLOBAL for Healing Potion and food items),
`poisoned` (SET GLOBAL for Antidote Vial), `luck` (SET GLOBAL for Lucky Charm).
Note: Sword of Sharpness and Armour Shard are passive — they do not require USE.
They are active while in inventory and checked inside CombatLoop.

`HandleSearch` writes to: `searched[roomId]` (LET),
`exitHidden[11]` (LET — slot 11 is room 6's south exit to room 9, set in InitExits.
  This is a magic number. Define a CONST or add a comment: ' slot 11 = room 6 south exit),
`armouryLocked` (SET GLOBAL).

`ShuffleLoot` writes to: `slotContents[i]` (LET), `slotTaken[i]` (LET), `slotRoom[i]` (LET).

`HandleDrop` writes to: `roomItems[roomId][roomItemCount[roomId]]` (LET),
`roomItemCount[roomId]` (LET — array element, not scalar global).

---

## Text Strings Required

**Already searched:** Deliverable 4, string 5
**Overburdened warning on TAKE:** Deliverable 4, string 4
**TAKE ALL when full:** Deliverable 4, string 10
**Nothing found on first SEARCH (room has no loot):** No message printed — the
  searched flag is set and the room is marked exhausted. On the second SEARCH the
  already-searched string fires (Deliverable 8). This is the confirmed resolution.
  A silent first search is correct — the player sees no items, no message, just the
  separator and prompt. The absence is the information.

**SEARCH interrupt text per monster:** Deliverable 8, SEARCH Interrupt — Fixed Monsters

---

## Known Gotchas

**Mouldy Bread placement — see above — needs design decision before coding.**

**Room item storage — 2D array, no capacity limit:**
`DIM roomItems[12][12] AS INTEGER` — 12 rooms, up to 12 items per room.
`DIM roomItemCount[12] AS INTEGER` — how many items are currently in each room.

To add an item to a room:
```
LET roomItems[roomId][roomItemCount[roomId]] = itemCode
LET roomItemCount[roomId] = roomItemCount[roomId] + 1
```

To display items in a room:
```
FOR i = 0 TO roomItemCount[roomId] - 1
    IF roomItems[roomId][i] > 0 THEN
        PRINT "  " & ItemName(roomItems[roomId][i])
    END IF
NEXT i
```

To remove an item from a room (on TAKE):
Find slot i where `roomItems[roomId][i] = itemCode`. Set that slot to 0.
Decrement `roomItemCount[roomId]`.
To keep the array compact, shift remaining items down one slot.

12 slots per room is ample — only 14 items exist in the whole game and the
player can carry 4 at once, so no room can ever hold more than 10 realistically.

**Overburdened penalty in CombatLoop:**
`CombatLoop` was written in Issue 5 without the overburdened check. Issue 6
adds the check: `IF overburdened = 1 THEN damage = 3 ELSE damage = 2`.
This is a modification to existing code — update the CombatLoop SUB.

**Sword of Sharpness in CombatLoop:**
The sword adds +1 to all player damage output, against all targets.
Apply after all other damage modifiers:
```
IF HasItem(ITEM_SWORD_OF_SHARPNESS) = 1 THEN
    LET damage = damage + 1
END IF
```
This means:
- Standard combat: 2 becomes 3
- Armoured monster (Pit Guardian) without sword: 1
- Armoured monster with sword: 2
- Armoured monster, overburdened, with sword: still 2 (armour reduction applied first, then sword)
The sword is passive while in inventory. Lifts immediately on DROP.

**Armour Shard in CombatLoop:**
Armour Shard is passive — auto-equips on pickup, reduces damage taken by 1 per round.
Add check in the monster-wins-round damage calculation:
`IF HasItem(ITEM_ARMOUR_SHARD) = 1 THEN damage = damage - 1`
Apply after overburdened check, before Crushing Blow check.
Minimum damage is 1 — Armour Shard cannot reduce to 0.

**HandleTake — name matching:**
The player types `TAKE KEY`, `TAKE SWORD`, `TAKE POTION` etc. The input after
UPPER$ normalisation must be matched to an item code. The matching approach:
Check items visible in the current room (`roomItems[currentRoom]`) against a
canonical name for each item code. Use a helper FUNCTION:

```
FUNCTION ItemCodeFromName(name$ AS STRING) AS INTEGER
    SELECT CASE name$
        CASE "KEY"
            RETURN ITEM_GUARDROOM_KEY
        CASE "SWORD"
            RETURN ITEM_SWORD_OF_SHARPNESS
        CASE "POTION"
            RETURN ITEM_HEALING_POTION
        CASE "BANGLE"
            RETURN ITEM_BANGLE_OF_COURAGE
        CASE "VIAL"
            RETURN ITEM_ANTIDOTE_VIAL
        CASE "BREAD"
            REM ambiguous — could be dark or mouldy. Check room context.
            RETURN ITEM_DARK_BREAD  REM default; room logic resolves mouldy
        CASE "CHARM"
            RETURN ITEM_LUCKY_CHARM
        CASE "SHARD"
            RETURN ITEM_ARMOUR_SHARD
        CASE "MEDAL"
            RETURN ITEM_MEDAL_OF_VALOUR
        CASE "BAG"
            RETURN ITEM_GOLD_BAG
        CASE ELSE
            RETURN ITEM_EMPTY
    END SELECT
END FUNCTION
```

If `ItemCodeFromName` returns ITEM_EMPTY (0), the item is not recognised.
Print the unknown command response.

**HasItem FUNCTION:**
```
FUNCTION HasItem(code AS INTEGER) AS INTEGER
    FOR i = 0 TO MAX_INVENTORY - 1
        IF inventory[i] = code THEN
            RETURN 1
        END IF
    NEXT i
    RETURN 0
END FUNCTION
```

---

## Testing Checklist

- [ ] TAKE item adds to inventory correctly
- [ ] invCount increments correctly
- [ ] Fourth item triggers overburdened message
- [ ] Fifth item refused with correct response
- [ ] DROP item removes from inventory, decrements invCount
- [ ] Overburdened clears when invCount drops below 4
- [ ] INVENTORY shows correct items and count
- [ ] SEARCH room 2 after Brute dead reveals key
- [ ] USE key in room 3 unlocks chest (armouryLocked = 0)
- [ ] SEARCH room 3 after unlock reveals Sword of Sharpness
- [ ] SEARCH room 6 after Horror dead reveals south exit
- [ ] GO SOUTH from room 6 after reveal navigates to room 9
- [ ] Overburdened movement costs STAMINA
- [ ] Sword of Sharpness gives +1 damage against all targets
- [ ] Sword of Sharpness: unarmoured combat deals 3 not 2
- [ ] Sword of Sharpness: Pit Guardian takes 2 not 1 per round
- [ ] Medal of Valour reduces SKILL by 1 on first pickup
- [ ] Medal of Valour penalty only applies once per run
- [ ] SEARCH while monster alive triggers combat interrupt
- [ ] Already-searched response on second SEARCH

---

## Issue File Reference

`listing/issue-06-what-you-carry.md`
