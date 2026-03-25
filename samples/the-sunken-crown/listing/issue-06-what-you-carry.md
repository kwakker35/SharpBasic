# The Sunken Crown
## Issue 6 — What You Carry

*You can't take everything. The dungeon makes you choose.*

---

## The Story So Far

Issues 1 through 5 built everything up to and including the first fight. The dungeon has a shape, you have attributes, and the Guardroom Brute can kill you.

But there is nothing to pick up yet. The Brute drops a key when it falls — you just can't take it. The Armoury chest is locked — you have no way to open it. The loot the dungeon is hiding across its twelve rooms sits undiscovered.

This issue changes that. Inventory goes in. TAKE, DROP, USE, and SEARCH all start working. The Brute's key unlocks the Armoury chest. The overburdened state begins punishing greed. The dungeon's resource management layer is live.

---

## What's New This Issue

- `inventory` array and `invCount` — the carry system
- `overburdened` flag — set when `invCount` reaches 4, cleared when it drops below
- Loot slot arrays — `slotContents`, `slotTaken`, `slotRoom`
- `SUB ShuffleLoot()` — Fisher-Yates shuffle of 5 items across 6 loot slots at startup
- `FUNCTION ItemName(code AS INTEGER) AS STRING` — converts item codes to display names
- `SUB AddToInventory(itemCode AS INTEGER)` — adds an item, updates carry state
- `SUB HandleSearch()` — searches the current room, reveals loot and monster drops
- `SUB PrintInventoryScreen()` — the INVENTORY command display
- `armouryLocked` flag — tracks the Armoury chest state
- Medal of Valour penalty — SKILL -1 on first pickup, silent and permanent

---

## Concept — Parallel Arrays and Item Tracking

Inventory is stored as an array of item codes:

```
DIM inventory[4] AS INTEGER    REM  item code per slot, 0 = empty
LET invCount = 0               REM  number of items currently carried
LET overburdened = 0           REM  0 = normal, 1 = carrying 4 items
```

Each slot holds a number. Zero means empty. The item is just a number — everything the game knows about what that number means lives in the code that reads it.

Here is the complete item code table. These codes are locked and must not change:

```
REM 0  = empty slot
REM 1  = Healing Potion
REM 2  = Lucky Charm
REM 3  = Armour Shard
REM 4  = Dark Bread
REM 5  = Medal of Valour
REM 6  = Antidote Vial
REM 7  = Bangle of Courage
REM 8  = Sword of Sharpness
REM 9  = Mouldy Bread
REM 10 = Guardroom Key
```

This is the parallel array pattern. `inventory[1]` holds what is in slot one. `invCount` tracks how many slots are filled. `overburdened` is derived from `invCount` — it is set to 1 whenever `invCount` reaches 4, and cleared to 0 whenever `invCount` drops below 4.

These three must always be in sync. The pattern for every TAKE operation — note the use of `SET GLOBAL` for all global writes:

```
REM Find the first empty slot
LET slot = 0
FOR i = 1 TO 4
    IF inventory[i] = 0 AND slot = 0 THEN
        LET slot = i
    END IF
NEXT i
REM Place the item and update carry state
LET inventory[slot] = itemCode
SET GLOBAL invCount = invCount + 1
IF invCount = 4 THEN
    SET GLOBAL overburdened = 1
END IF
```

And every DROP operation:

```
LET inventory[slot] = 0
SET GLOBAL invCount = invCount - 1
IF invCount < 4 THEN
    SET GLOBAL overburdened = 0
END IF
```

`LET` is used for the array element assignment because array elements are not globals — they are slots within a global array. `SET GLOBAL` is used for the scalar globals `invCount` and `overburdened` because those are the values that must be visible to the rest of the program.

The item name lookup uses `SELECT CASE` — one of the cleanest uses of the pattern in the game:

```
SELECT CASE code
    CASE 1
        RETURN "Healing Potion"
    CASE 7
        RETURN "Bangle of Courage"
    CASE 11
        RETURN "Gold Bag"
    CASE ELSE
        RETURN "Unknown Item"
END SELECT
```

If you add to `inventory` without incrementing `invCount`, the game thinks you have fewer items than you do. If you decrement `invCount` without clearing the slot, the slot is invisible but still occupied. Either breaks every system that depends on the count. The discipline of updating all three together, every time, is what keeps the accounting honest.

---

## How It Fits

Issue 5 killed the Guardroom Brute and left its body in room 2. This issue adds SEARCH to room 2 — the Brute carries a key, and SEARCH after combat finds it. The key unlocks the Armoury chest in room 3, which contains the Sword of Sharpness.

`ShuffleLoot()` runs at startup alongside `InitExits()` and `InitMonsters()`. The five shuffled items are distributed across six loot slots before the opening sequence. One slot is always empty — absence is as interesting as presence.

The Medal of Valour sits in the shuffled pool looking like a reward. It is not. SKILL -1 applies silently on first pickup. The penalty is permanent. The dungeon does not explain.

---

## What You'll See

Navigate to room 2, fight the Brute, win. Type `SEARCH` — the key appears. Type `TAKE KEY`. Navigate to room 3, type `USE KEY` — the chest unlocks. Type `SEARCH` — the Sword of Sharpness appears.

Type `INVENTORY` at any point to see what you are carrying and your current carry status. Pick up four items and move rooms — the overburdened message fires and STAMINA drops by 1. Drop back to three items and the penalty stops.

---

## What to Try

Drop an item and pick it up again. Check `invCount` before and after. Does it return to exactly where it started? If it doesn't, you have a sync bug. Finding it now, in this simple case, is far easier than finding it later when three other systems depend on the count being right.

---

## The Listing

```
REM Issue 6 listing — to be added once built and tested
```
