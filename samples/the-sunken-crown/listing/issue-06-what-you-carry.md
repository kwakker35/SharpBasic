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
REM === ADD TO: file header comment block, add Issue 6 line after Issue 5 ===
REM  Issue 6: What You Carry
REM  Inventory, loot shuffle, SEARCH, TAKE, DROP, USE, overburdened,
REM  Sword/Shard/Medal combat effects, Guardroom Key drop.

REM === ADD TO: constants block, after CONST MONSTER_KING = 11 ===

REM ----------------------------------------------------------------
REM  Item codes -- locked (Decision 5)
REM  0=empty, 1=Healing Potion, 2=Lucky Charm, 3=Armour Shard,
REM  4=Dark Bread, 5=Medal of Valour, 6=Antidote Vial,
REM  7=Bangle of Courage, 8=Sword of Sharpness, 9=Mouldy Bread,
REM  10=Guardroom Key
REM ----------------------------------------------------------------
CONST ITEM_EMPTY    = 0
CONST ITEM_POTION   = 1
CONST ITEM_CHARM    = 2
CONST ITEM_SHARD    = 3
CONST ITEM_BREAD    = 4
CONST ITEM_MEDAL    = 5
CONST ITEM_ANTIDOTE = 6
CONST ITEM_BANGLE   = 7
CONST ITEM_SWORD    = 8
CONST ITEM_MOULDY   = 9
CONST ITEM_KEY      = 10


REM === ADD TO: globals block, after LET luckTestCount = 0 ===

REM ----------------------------------------------------------------
REM  Inventory state
REM  inventory[1..4]: item code per slot (0 = empty). Slot 0 unused.
REM  invCount: items currently carried (0-4).
REM  overburdened: 1 when invCount = 4 -- movement costs STAMINA.
REM  medalTaken: 1 after Medal of Valour first pickup (SKILL -1).
REM ----------------------------------------------------------------
DIM inventory[5] AS INTEGER
LET invCount = 0
LET overburdened = 0
LET medalTaken = 0

REM ----------------------------------------------------------------
REM  Loot slots (shuffled at each run start)
REM  slotContents[1..6]: item code assigned to each slot (0 = empty).
REM  slotTaken[1..6]: 0 = available, 1 = collected. Slot 0 unused.
REM  Slot-to-room mapping handled by FUNCTION RoomLootSlot.
REM ----------------------------------------------------------------
DIM slotContents[7] AS INTEGER
DIM slotTaken[7] AS INTEGER

REM ----------------------------------------------------------------
REM  Armoury state
REM  armouryLocked: 1 = chest locked, 0 = opened with Guardroom Key.
REM ----------------------------------------------------------------
LET armouryLocked = 1

REM ----------------------------------------------------------------
REM  Room floor items
REM  droppedItem[r][s]: item code at floor slot s in room r (0-based r).
REM  droppedCount[r]: items currently on room r's floor. Max 12/room.
REM  r = roomId - 1. Supports stockpiling: drops, monster loot, etc.
REM ----------------------------------------------------------------
DIM droppedItem[MAX_ROOMS][MAX_ROOMS] AS INTEGER
DIM droppedCount[MAX_ROOMS] AS INTEGER

REM ----------------------------------------------------------------
REM  SEARCH interrupt flag
REM  Set to 1 by HandleSearch when SEARCH interrupts a live-monster
REM  room. Read and consumed inside HandleFight; reset to 0 after use.
REM ----------------------------------------------------------------
LET searchInterruptActive = 0


REM === NEW FUNCTION: add after FUNCTION DirName() ===

REM =================================================================
REM  FUNCTION ItemCode -- item$ AS STRING -> INTEGER
REM  Maps a player-typed item keyword to its integer item code.
REM  Returns -1 if the keyword is not recognised. Used by HandleTake,
REM  HandleDrop, and HandleUse for input validation.
REM =================================================================
FUNCTION ItemCode(item$ AS STRING) AS INTEGER
    SELECT CASE item$
        CASE "POTION"
            RETURN ITEM_POTION
        CASE "CHARM"
            RETURN ITEM_CHARM
        CASE "SHARD"
            RETURN ITEM_SHARD
        CASE "BREAD"
            RETURN ITEM_BREAD
        CASE "MEDAL"
            RETURN ITEM_MEDAL
        CASE "ANTIDOTE"
            RETURN ITEM_ANTIDOTE
        CASE "BANGLE"
            RETURN ITEM_BANGLE
        CASE "SWORD"
            RETURN ITEM_SWORD
        CASE "MOULDY"
            RETURN ITEM_MOULDY
        CASE "KEY"
            RETURN ITEM_KEY
        CASE ELSE
            RETURN -1
    END SELECT
END FUNCTION

REM === NEW FUNCTION: add after FUNCTION ItemCode() ===

REM =================================================================
REM  FUNCTION RoomLootSlot -- roomId AS INTEGER -> INTEGER
REM  Returns the loot slot number (1-6) for a room, or 0 if the room
REM  has no shuffled loot slot. Used by HandleSearch and HandleTake.
REM =================================================================
FUNCTION RoomLootSlot(roomId AS INTEGER) AS INTEGER
    SELECT CASE roomId
        CASE ROOM_ENTRY
            RETURN 1
        CASE ROOM_GUARDROOM
            RETURN 2
        CASE ROOM_CROSSROADS
            RETURN 3
        CASE ROOM_COLLAPSED
            RETURN 4
        CASE ROOM_RIDDLE
            RETURN 5
        CASE ROOM_UNDERHALL
            RETURN 6
        CASE ELSE
            RETURN 0
    END SELECT
END FUNCTION

REM === NEW FUNCTION: add after FUNCTION RoomLootSlot() ===

REM =================================================================
REM  FUNCTION ItemName -- code AS INTEGER -> STRING
REM  Returns the display name for an item code. Used throughout
REM  inventory display, SEARCH results, and TAKE/DROP/USE feedback.
REM =================================================================
FUNCTION ItemName(code AS INTEGER) AS STRING
    SELECT CASE code
        CASE ITEM_POTION
            RETURN "Healing Potion"
        CASE ITEM_CHARM
            RETURN "Lucky Charm"
        CASE ITEM_SHARD
            RETURN "Armour Shard"
        CASE ITEM_BREAD
            RETURN "Dark Bread"
        CASE ITEM_MEDAL
            RETURN "Medal of Valour"
        CASE ITEM_ANTIDOTE
            RETURN "Antidote Vial"
        CASE ITEM_BANGLE
            RETURN "Bangle of Courage"
        CASE ITEM_SWORD
            RETURN "Sword of Sharpness"
        CASE ITEM_MOULDY
            RETURN "Mouldy Bread"
        CASE ITEM_KEY
            RETURN "Guardroom Key"
        CASE ELSE
            RETURN "Unknown Item"
    END SELECT
END FUNCTION


REM === NEW SUB: add after SUB InitMonsters() ===

REM =================================================================
REM  SUB ShuffleLoot
REM  Fisher-Yates shuffle of the six-item loot pool (five named items
REM  plus one empty slot). Assigns shuffled results to slotContents[1..6].
REM  Entry Hall medal check: if Medal lands in slot 1, runs a free luck
REM  roll (no LUCK cost); swaps with first helpful item in slots 2-6 on
REM  success. Places fixed items: Antidote Vial in ROOM_CISTERN (index 8).
REM  Called once per run after OpeningSequence sets luck.
REM =================================================================
SUB ShuffleLoot()
    DIM lootPool[6] AS INTEGER
    LET lootPool[0] = ITEM_POTION
    LET lootPool[1] = ITEM_CHARM
    LET lootPool[2] = ITEM_SHARD
    LET lootPool[3] = ITEM_BREAD
    LET lootPool[4] = ITEM_MEDAL
    LET lootPool[5] = ITEM_EMPTY

    REM  Fisher-Yates shuffle -- 0-based, indices 0-5
    FOR i = 5 TO 1 STEP -1
        LET j = CINT(RND() * (i + 1))
        LET temp = lootPool[i]
        LET lootPool[i] = lootPool[j]
        LET lootPool[j] = temp
    NEXT i

    FOR i = 0 TO 5
        SET GLOBAL slotContents[i + 1] = lootPool[i]
        SET GLOBAL slotTaken[i + 1] = 0
    NEXT i

    REM  Entry Hall medal check: Medal in slot 1 hurts a new player.
    REM  Try a free luck roll (no LUCK decrement); swap on success.
    IF slotContents[1] = ITEM_MEDAL THEN
        LET swapRoll = RollDice(2)
        IF swapRoll <= luck THEN
            LET swapTarget = 0
            FOR k = 2 TO 6
                IF slotContents[k] >= ITEM_POTION AND slotContents[k] <= ITEM_BREAD AND swapTarget = 0 THEN
                    LET swapTarget = k
                END IF
            NEXT k
            IF swapTarget > 0 THEN
                LET tempCode = slotContents[1]
                SET GLOBAL slotContents[1] = slotContents[swapTarget]
                SET GLOBAL slotContents[swapTarget] = tempCode
            END IF
        END IF
    END IF

    REM  Fixed item: Antidote Vial in Room 9 cistern cache (index 8)
    SET GLOBAL droppedItem[8][0] = ITEM_ANTIDOTE
    SET GLOBAL droppedCount[8] = 1
END SUB

REM === NEW SUB: add after SUB ShuffleLoot() ===

REM =================================================================
REM  SUB AddToInventory -- itemCode AS INTEGER
REM  Adds an item to the first empty inventory slot, updates invCount
REM  and overburdened. Fires the Medal of Valour SKILL penalty (once
REM  only) on first pickup. Called by HandleTake for single and ALL.
REM =================================================================
SUB AddToInventory(itemCode AS INTEGER)
    LET slot = 0
    FOR i = 1 TO 4
        IF inventory[i] = ITEM_EMPTY AND slot = 0 THEN LET slot = i END IF
    NEXT i
    SET GLOBAL inventory[slot] = itemCode
    SET GLOBAL invCount = invCount + 1
    IF invCount = 4 THEN SET GLOBAL overburdened = 1 END IF
    IF itemCode = ITEM_MEDAL AND medalTaken = 0 THEN
        PRINT "  It is heavier than it looks. Old -- older than anything else down here."
        PRINT "  Decorated. Whatever it once meant, it meant something."
        PRINT ""
        SET GLOBAL skill = skill - 1
        SET GLOBAL medalTaken = 1
    END IF
END SUB

REM === NEW SUB: add after SUB AddToInventory() ===

REM =================================================================
REM  SUB HandleSearch
REM  Processes the SEARCH command. Costs 2 turns regardless of outcome.
REM  Live monster present: fires SEARCH interrupt combat (SKILL -1
REM  round 1 via CombatLoop's searchInterrupt parameter).
REM  ROOM_THRONE with King alive: refuses search entirely, no combat.
REM  On success: shows loot slot contents, floor items, and any
REM  room-specific reveals (ROOM_COLLAPSED hidden exit, ROOM_ARMOURY chest state).
REM  Sets searched[roomId-1] = 1 on completion.
REM =================================================================
SUB HandleSearch()
    IF monsterAlive[currentRoom - 1] = 1 THEN
        IF currentRoom = ROOM_THRONE THEN
            PRINT "  Not here. Not with him watching."
            PRINT ""
            SET GLOBAL turns = turns + 2
            RETURN
        END IF
        SELECT CASE currentRoom
            CASE ROOM_GUARDROOM
                PRINT "  The Brute hears the scrape of your boot before you've taken a step"
                PRINT "  toward the wall. It turns with the speed of something that has been"
                PRINT "  waiting for an excuse. You are out of time."
            CASE ROOM_COLLAPSED
                PRINT "  The moment your hand touches the rubble the Horror is already moving."
                PRINT "  Not toward the sound -- toward you. It has been still this whole time."
                PRINT "  It was never unaware."
            CASE ROOM_PIT
                PRINT "  The Guardian does not hurry. It simply turns, raises its weapon, and"
                PRINT "  begins walking toward you with the unhurried certainty of something"
                PRINT "  that has done this before. You had your back to it."
            CASE ROOM_CISTERN
                PRINT "  The cold in the room doubles before you have touched anything. The"
                PRINT "  luminescence fixes on you -- steady, no longer drifting. It knew the"
                PRINT "  moment your attention shifted."
            CASE ROOM_UNDERHALL
                PRINT "  The small dark eyes find you the instant you crouch. It doesn't roar."
                PRINT "  It doesn't rush. It simply stands and keeps coming with the patient"
                PRINT "  certainty of something that has never needed to hurry."
        END SELECT
        PRINT ""
        SET GLOBAL turns = turns + 2
        SET GLOBAL searchInterruptActive = 1
        CALL HandleFight(currentRoom)
        RETURN
    END IF

    SET GLOBAL turns = turns + 2

    IF searched[currentRoom - 1] = 1 THEN
        PRINT "  You go through the room again. You find nothing you missed the first time."
        PRINT ""
        RETURN
    END IF

    LET mySlot = RoomLootSlot(currentRoom)
    LET foundSomething = 0

    IF mySlot > 0 THEN
        IF slotTaken[mySlot] = 0 AND slotContents[mySlot] > ITEM_EMPTY THEN
            PRINT "  You find: " & ItemName(slotContents[mySlot])
            LET foundSomething = 1
        END IF
    END IF

    IF currentRoom = ROOM_ARMOURY THEN
        IF armouryLocked = 1 THEN
            PRINT "  The chest is here. Iron-banded, locked."
            LET foundSomething = 1
        END IF
    END IF

    IF currentRoom = ROOM_COLLAPSED THEN
        SET GLOBAL exitHidden[11] = 0
        PRINT "  You work through the rubble methodically. Most of it is solid --"
        PRINT "  ceiling stone, heavy, immovable. But toward the base of the fall,"
        PRINT "  where the collapse met the floor, there is a gap. Low, tight, passable"
        PRINT "  if you commit to it."
        PRINT ""
        PRINT "  Beyond it: a passage, heading south."
        PRINT ""
        PRINT "  It was always there. The dungeon didn't hide it. You just had to look."
        LET foundSomething = 1
    END IF

    FOR s = 0 TO droppedCount[currentRoom - 1] - 1
        LET floorCode = droppedItem[currentRoom - 1][s]
        IF floorCode > ITEM_EMPTY THEN
            PRINT "  " & ItemName(floorCode) & " is here."
            LET foundSomething = 1
        END IF
    NEXT s

    IF foundSomething = 0 THEN
        PRINT "  There is nothing here of use."
    END IF

    PRINT ""
    SET GLOBAL searched[currentRoom - 1] = 1
END SUB

REM === NEW SUB: add after SUB HandleSearch() ===

REM =================================================================
REM  SUB PrintInventoryScreen
REM  Redraws the full screen as a dedicated inventory frame.
REM  Lists all four carry slots (occupied or empty) with passive item
REM  tags. Waits for ENTER to continue. Costs 0 turns.
REM =================================================================
SUB PrintInventoryScreen()
    CALL PrintHeader()
    LET invCount$ = invCount & " / 4 items"
    LET padLen = FRAME_WIDTH - 11 - LEN(invCount$)
    PRINT "  INVENTORY" & STRING$(" ", padLen) & invCount$
    PRINT ""
    FOR i = 1 TO 4
        IF inventory[i] > ITEM_EMPTY THEN
            LET tag$ = ""
            IF inventory[i] = ITEM_SHARD THEN LET tag$ = "  (passive -- damage -1)" END IF
            IF inventory[i] = ITEM_BANGLE THEN LET tag$ = "  (passive -- negates Terror)" END IF
            IF inventory[i] = ITEM_SWORD THEN LET tag$ = "  (passive -- +1 damage vs armour)" END IF
            PRINT "  [" & i & "] " & ItemName(inventory[i]) & tag$
        ELSE
            PRINT "  [" & i & "] -- empty"
        END IF
    NEXT i
    PRINT ""
    INPUT "  [ Press ENTER to continue ]"; p$
    PRINT ""
    CALL PrintSeparator()
END SUB

REM === NEW SUB: add after SUB PrintInventoryScreen() ===

REM =================================================================
REM  SUB HandleTake -- item$ AS STRING
REM  Processes TAKE [item] and TAKE ALL commands.
REM  TAKE ALL: greedily picks up items in array order until invCount
REM  reaches 4. If items remain, prints partial-fill flavour and leftovers.
REM  TAKE [item]: checks item present, checks hard limit (4), adds it.
REM  When full: shows current inventory inline + "DROP an item".
REM  When at 3: prints overburdened warning before taking.
REM  Costs 0 turns.
REM =================================================================
SUB HandleTake(item$ AS STRING)
    IF item$ = "ALL" THEN
        LET taken = 0
        LET leftover = 0
        LET leftoverNames$ = ""
        LET mySlot = RoomLootSlot(currentRoom)
        IF mySlot > 0 THEN
            IF slotTaken[mySlot] = 0 AND slotContents[mySlot] > ITEM_EMPTY THEN
                IF invCount < 4 THEN
                    CALL AddToInventory(slotContents[mySlot])
                    SET GLOBAL slotTaken[mySlot] = 1
                    LET taken = taken + 1
                ELSE
                    IF leftoverNames$ = "" THEN
                        LET leftoverNames$ = ItemName(slotContents[mySlot])
                    ELSE
                        LET leftoverNames$ = leftoverNames$ & ", " & ItemName(slotContents[mySlot])
                    END IF
                    LET leftover = leftover + 1
                END IF
            END IF
        END IF

        LET s = 0
        WHILE s < droppedCount[currentRoom - 1]
            LET floorCode = droppedItem[currentRoom - 1][s]
            IF floorCode > ITEM_EMPTY THEN
                IF invCount < 4 THEN
                    CALL AddToInventory(floorCode)
                    FOR k = s TO droppedCount[currentRoom - 1] - 2
                        SET GLOBAL droppedItem[currentRoom - 1][k] = droppedItem[currentRoom - 1][k + 1]
                    NEXT k
                    SET GLOBAL droppedItem[currentRoom - 1][droppedCount[currentRoom - 1] - 1] = ITEM_EMPTY
                    SET GLOBAL droppedCount[currentRoom - 1] = droppedCount[currentRoom - 1] - 1
                    LET taken = taken + 1
                ELSE
                    IF leftoverNames$ = "" THEN
                        LET leftoverNames$ = ItemName(floorCode)
                    ELSE
                        LET leftoverNames$ = leftoverNames$ & ", " & ItemName(floorCode)
                    END IF
                    LET leftover = leftover + 1
                    LET s = s + 1
                END IF
            ELSE
                LET s = s + 1
            END IF
        WEND

        IF taken = 0 AND leftover = 0 THEN
            PRINT "  There is nothing here to take."
        ELSE
            IF leftover > 0 THEN
                PRINT "  You take what you can carry. The dungeon is patient."
                PRINT ""
                PRINT "  Left behind: " & leftoverNames$
                PRINT ""
                PRINT "  Use TAKE [item] to choose what matters most."
            END IF
        END IF
        PRINT ""
        RETURN
    END IF

    LET itemCode = ItemCode(item$)
    IF itemCode = -1 THEN
        PRINT "  You don't know what that is."
        PRINT ""
        RETURN
    END IF

    LET available = 0
    LET fromSlot = 0
    LET fromFloor = 0
    LET floorSlot = 0
    LET mySlot = RoomLootSlot(currentRoom)
    IF mySlot > 0 THEN
        IF slotTaken[mySlot] = 0 AND slotContents[mySlot] = itemCode THEN
            LET available = 1
            LET fromSlot = mySlot
        END IF
    END IF

    IF available = 0 THEN
        FOR s = 0 TO droppedCount[currentRoom - 1] - 1
            IF droppedItem[currentRoom - 1][s] = itemCode AND available = 0 THEN
                LET available = 1
                LET fromFloor = 1
                LET floorSlot = s
            END IF
        NEXT s
    END IF

    IF available = 0 THEN
        PRINT "  There is nothing like that here."
        PRINT ""
        RETURN
    END IF

    IF invCount = 4 THEN
        PRINT "  You cannot carry any more."
        PRINT ""
        PRINT "  Carrying:"
        FOR i = 1 TO 4
            PRINT "  [" & i & "] " & ItemName(inventory[i])
        NEXT i
        PRINT ""
        PRINT "  DROP an item to make room."
        PRINT ""
        RETURN
    END IF

    IF invCount = 3 THEN
        PRINT "  You are already carrying more than you should. You can take one more"
        PRINT "  item, but you will feel it."
        PRINT ""
    END IF

    CALL AddToInventory(itemCode)
    IF fromSlot > 0 THEN
        SET GLOBAL slotTaken[fromSlot] = 1
    END IF
    IF fromFloor = 1 THEN
        FOR k = floorSlot TO droppedCount[currentRoom - 1] - 2
            SET GLOBAL droppedItem[currentRoom - 1][k] = droppedItem[currentRoom - 1][k + 1]
        NEXT k
        SET GLOBAL droppedItem[currentRoom - 1][droppedCount[currentRoom - 1] - 1] = ITEM_EMPTY
        SET GLOBAL droppedCount[currentRoom - 1] = droppedCount[currentRoom - 1] - 1
    END IF

    IF itemCode <> ITEM_MEDAL THEN
        PRINT "  Taken."
    END IF
    PRINT ""
END SUB

REM === NEW SUB: add after SUB HandleTake() ===

REM =================================================================
REM  SUB HandleDrop -- item$ AS STRING
REM  Removes an item from inventory, places it on the current room's
REM  floor array, and updates invCount and overburdened.
REM  Costs 0 turns.
REM =================================================================
SUB HandleDrop(item$ AS STRING)
    LET itemCode = ItemCode(item$)
    IF itemCode = -1 THEN
        PRINT "  You don't know what that is."
        PRINT ""
        RETURN
    END IF

    LET found = 0
    LET invSlot = 0
    FOR i = 1 TO 4
        IF inventory[i] = itemCode AND found = 0 THEN
            LET found = 1
            LET invSlot = i
        END IF
    NEXT i

    IF found = 0 THEN
        PRINT "  You are not carrying that."
        PRINT ""
        RETURN
    END IF

    SET GLOBAL inventory[invSlot] = ITEM_EMPTY
    SET GLOBAL invCount = invCount - 1
    IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
    SET GLOBAL droppedItem[currentRoom - 1][droppedCount[currentRoom - 1]] = itemCode
    SET GLOBAL droppedCount[currentRoom - 1] = droppedCount[currentRoom - 1] + 1
    PRINT "  Dropped."
    PRINT ""
END SUB

REM === NEW SUB: add after SUB HandleDrop() ===

REM =================================================================
REM  SUB HandleUse -- item$ AS STRING
REM  Activates an item from inventory. Consumables are removed from
REM  the slot on use; passives print a reminder and stay.
REM  Guardroom Key: unlocks Armoury chest if in ROOM_ARMOURY, places Sword
REM  of Sharpness on ROOM_ARMOURY's floor, consumes the key.
REM  Costs 1 turn.
REM =================================================================
SUB HandleUse(item$ AS STRING)
    LET itemCode = ItemCode(item$)
    IF itemCode = -1 THEN
        PRINT "  You don't know what that is."
        PRINT ""
        RETURN
    END IF

    LET found = 0
    LET invSlot = 0
    FOR i = 1 TO 4
        IF inventory[i] = itemCode AND found = 0 THEN
            LET found = 1
            LET invSlot = i
        END IF
    NEXT i

    IF found = 0 THEN
        PRINT "  You are not carrying that."
        PRINT ""
        RETURN
    END IF

    SET GLOBAL turns = turns + 1
    SELECT CASE itemCode
        CASE ITEM_POTION
            SET GLOBAL stamina = startStamina
            PRINT "  You drink it in one pull. The wounds close faster than they have any right to."
            PRINT ""
            PRINT "  STAMINA restored to " & stamina
            SET GLOBAL inventory[invSlot] = ITEM_EMPTY
            SET GLOBAL invCount = invCount - 1
            IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
        CASE ITEM_CHARM
            SET GLOBAL luck = luck + 2
            PRINT "  You close your fist around it and make your wish. Whatever it is, it takes it."
            PRINT ""
            PRINT "  LUCK: " & luck
            SET GLOBAL inventory[invSlot] = ITEM_EMPTY
            SET GLOBAL invCount = invCount - 1
            IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
        CASE ITEM_SHARD
            PRINT "  You are already wearing it. It reduces the damage you take while you carry it."
            PRINT "  Nothing more to do."
        CASE ITEM_BREAD
            SET GLOBAL stamina = MIN(stamina + 4, startStamina)
            PRINT "  Dry, dense, hard. You eat it without tasting it."
            PRINT ""
            PRINT "  STAMINA: " & stamina
            SET GLOBAL inventory[invSlot] = ITEM_EMPTY
            SET GLOBAL invCount = invCount - 1
            IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
        CASE ITEM_MEDAL
            PRINT "  It sits in your palm. Cold. It does not respond to you."
        CASE ITEM_ANTIDOTE
            SET GLOBAL poisoned = 0
            PRINT "  Bitter. You drain it and throw the vial aside. The cold in your blood recedes."
            PRINT ""
            PRINT "  Poison cleared."
            SET GLOBAL inventory[invSlot] = ITEM_EMPTY
            SET GLOBAL invCount = invCount - 1
            IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
        CASE ITEM_BANGLE
            PRINT "  You are already wearing it. It does what it does while you carry it."
        CASE ITEM_SWORD
            PRINT "  It is in your hand. Keep it there. It does its work in combat."
        CASE ITEM_MOULDY
            SET GLOBAL stamina = MIN(stamina + 3, startStamina)
            PRINT "  You eat it. You try not to think about how long it has been down here."
            PRINT ""
            PRINT "  STAMINA: " & stamina
            SET GLOBAL inventory[invSlot] = ITEM_EMPTY
            SET GLOBAL invCount = invCount - 1
            IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
        CASE ITEM_KEY
            IF currentRoom = ROOM_ARMOURY THEN
                IF armouryLocked = 1 THEN
                    PRINT "  You try the key in the lock. It turns."
                    PRINT ""
                    PRINT "  The chest opens. The air that comes out smells of oil and old metal."
                    PRINT "  Inside: a sword. Old, but maintained. The edge catches the light."
                    SET GLOBAL armouryLocked = 0
                    SET GLOBAL droppedItem[2][droppedCount[2]] = ITEM_SWORD
                    SET GLOBAL droppedCount[2] = droppedCount[2] + 1
                    SET GLOBAL inventory[invSlot] = ITEM_EMPTY
                    SET GLOBAL invCount = invCount - 1
                    IF invCount < 4 THEN SET GLOBAL overburdened = 0 END IF
                ELSE
                    PRINT "  The chest is already open."
                END IF
            ELSE
                PRINT "  There is nothing here to use it on."
            END IF
    END SELECT
    PRINT ""
END SUB


REM === REPLACE: SUB HandleGo -- add overburdened movement cost ===
REM  The Issue 4 version simply sets currentRoom and returns.
REM  Replace the entire SUB with this version that adds the
REM  overburdened STAMINA penalty on each successful move.

SUB HandleGo(dir AS INTEGER)
    LET start = roomExitStart[currentRoom - 1]
    LET count = roomExitCount[currentRoom - 1]
    FOR i = start TO start + count - 1
        IF exitDir[i] = dir AND exitHidden[i] = 0 THEN
            SET GLOBAL currentRoom = exitDest[i]
            IF overburdened = 1 THEN
                CALL QueueFlavour("  You move, but the weight tells on you. Everything costs more")
                CALL QueueFlavour("  when you are carrying this much.")
                CALL QueueFlavour("")
                SET GLOBAL stamina = stamina - 1
                SET GLOBAL minStamina = MIN(minStamina, stamina)
                CALL QueueFlavour("  STAMINA: " & stamina)
                CALL QueueFlavour("")
                IF stamina <= 0 THEN
                    SET GLOBAL gameOver = 1
                    SET GLOBAL endState = 5
                END IF
            END IF
            RETURN
        END IF
    NEXT i
    PRINT "  There is no way through in that direction."
    PRINT ""
END SUB


REM === REPLACE: in SUB CombatLoop, player-wins armour branch ===
REM  The Issue 5 version always deals 1 damage to armoured monsters.
REM  Replace the armour block with the Sword of Sharpness check.
REM  Find:
REM      IF hasArmour = 1 THEN
REM          LET monsterStamina = monsterStamina - 1
REM          PRINT "  You strike..."
REM      ELSE
REM  Replace with:

        IF playerAttack > monsterAttack THEN
            REM  Player wins the round.
            IF hasArmour = 1 THEN
                REM  Sword of Sharpness cuts clean through armour.
                LET hasSword = 0
                FOR sw = 1 TO 4
                    IF inventory[sw] = ITEM_SWORD THEN LET hasSword = 1 END IF
                NEXT sw
                IF hasSword = 1 THEN
                    LET monsterStamina = monsterStamina - 2
                    PRINT "  Your blade cuts clean through the armour.  Monster STAMINA: " & monsterStamina
                ELSE
                    LET monsterStamina = monsterStamina - 1
                    PRINT "  You strike but the armour absorbs most of it.  Monster STAMINA: " & monsterStamina
                END IF
            ELSE
                REM  ... rest of player-wins branch unchanged ...


REM === REPLACE: in SUB CombatLoop, monster-wins damage block ===
REM  The Issue 5 version always uses baseDamage = 2.
REM  Replace the damage calculation to add overburdened and Armour Shard.
REM  Find:
REM      LET baseDamage = 2
REM      REM  Overburdened damage bonus (baseDamage = 3) added in Issue 6.
REM  Replace with:

                REM  Monster wins the round.
                LET baseDamage = 2
                IF overburdened = 1 THEN LET baseDamage = 3 END IF
                LET hasShard = 0
                FOR sh = 1 TO 4
                    IF inventory[sh] = ITEM_SHARD THEN LET hasShard = 1 END IF
                NEXT sh
                IF hasShard = 1 THEN LET baseDamage = MAX(baseDamage - 1, 1) END IF
                REM  ... Crushing Blow and stamina lines unchanged ...


REM === REPLACE: SUB HandleFight -- add searchInterrupt and key drop ===
REM  Replace the entire SUB. The key changes: searchInterruptActive is
REM  read and cleared at the top. The Brute's CombatLoop call passes
REM  activeInterrupt instead of hardcoded 0. The Brute drops ITEM_KEY
REM  on death. All other monster calls pass activeInterrupt too.

SUB HandleFight(roomId AS INTEGER)
    LET activeInterrupt = searchInterruptActive
    SET GLOBAL searchInterruptActive = 0
    IF monsterAlive[roomId - 1] = 0 THEN
        PRINT "  There is nothing here that requires that."
        PRINT ""
        RETURN
    END IF
    IF roomId = ROOM_GUARDROOM THEN
        PRINT ""
        PRINT "  It hears you before you reach it. The broad shoulders shift, the head"
        PRINT "  turns, and then it is facing you -- fully, deliberately, with the"
        PRINT "  unhurried attention of something that has done this many times and"
        PRINT "  found it straightforward."
        PRINT ""
        PRINT "  It is very large. The weapon in its hand looks like it was made for"
        PRINT "  someone even larger."
        PRINT ""
        PRINT "  It moves first."
        PRINT ""
        LET mSkill = RollDice(1) + 7
        LET mStamina = RollDice(2) + 10
        CALL CombatLoop(mSkill, mStamina, 0, 0, 0, 0, 0, 0, activeInterrupt)
        IF gameOver = 0 THEN
            LET monsterAlive[roomId - 1] = 0
            SET GLOBAL droppedItem[roomId - 1][droppedCount[roomId - 1]] = ITEM_KEY
            SET GLOBAL droppedCount[roomId - 1] = droppedCount[roomId - 1] + 1
            PRINT ""
            PRINT "  It goes down slowly, the way large things do -- first to one knee,"
            PRINT "  then forward, the weapon dropping before the rest of it follows."
            PRINT "  The floor shakes once when it lands."
            PRINT ""
            PRINT "  The room is considerably quieter."
            PRINT ""
        ELSE
            PRINT ""
            PRINT "  The weapon connects before you can recover."
            PRINT "  The last thing you see is the floor approaching."
            PRINT ""
        END IF
    END IF
    IF roomId = ROOM_COLLAPSED THEN
        PRINT ""
        PRINT "  You hear it first -- a dry, rapid skittering from somewhere in the dark"
        PRINT "  beyond the rubble. Then silence. Then it is in the room with you and you"
        PRINT "  understand, viscerally and immediately, why silence followed the sound."
        PRINT ""
        PRINT "  It is fast. Faster than anything that size has any right to be. The"
        PRINT "  chitinous plates catch the torchlight in fragments as it moves and it"
        PRINT "  is already moving toward you before you have fully registered what you"
        PRINT "  are looking at."
        PRINT ""
        PRINT "  Round one. Don't let it rattle you."
        PRINT ""
        LET mSkill = RollDice(1) + 5
        LET mStamina = RollDice(1) + 4
        CALL CombatLoop(mSkill, mStamina, 0, 0, 0, 1, 1, 0, activeInterrupt)
        IF gameOver = 0 THEN
            LET monsterAlive[roomId - 1] = 0
            PRINT ""
            PRINT "  It drops mid-movement, skidding across the stone floor before coming"
            PRINT "  to rest against the far wall. The chitinous legs fold inward, one"
            PRINT "  after another, until it is still."
            PRINT ""
            PRINT "  You stand over it for a moment. The cold in your hands fades slowly."
            PRINT ""
        ELSE
            PRINT ""
            PRINT "  It is on you before the next round begins. You don't feel the last"
            PRINT "  blow. You feel the cold, spreading, and then nothing."
            PRINT ""
        END IF
    END IF
    IF roomId = ROOM_PIT THEN
        PRINT ""
        PRINT "  It turns to face you without urgency. The armour it wears has been"
        PRINT "  repaired so many times the original shape is barely discernible --"
        PRINT "  layer over layer over layer, each repair a testament to something that"
        PRINT "  has been hit repeatedly and kept going."
        PRINT ""
        PRINT "  It raises its weapon with the economy of motion of something that does"
        PRINT "  not waste effort. It has assessed you. It has found you worth engaging."
        PRINT ""
        LET mSkill = RollDice(1) + 8
        LET mStamina = RollDice(2) + 8
        CALL CombatLoop(mSkill, mStamina, 1, 0, 0, 0, 0, 0, activeInterrupt)
        IF gameOver = 0 THEN
            LET monsterAlive[roomId - 1] = 0
            PRINT ""
            PRINT "  The armour clatters against the stone in sections as it falls --"
            PRINT "  the sound of something that has held together through a great deal"
            PRINT "  finally coming apart all at once."
            PRINT ""
            PRINT "  It does not get up. Whatever kept it going, you have taken enough."
            PRINT ""
        ELSE
            PRINT ""
            PRINT "  The armoured figure finishes what it started with the economy of"
            PRINT "  something that has done exactly this many times before."
            PRINT "  The light goes first."
            PRINT ""
        END IF
    END IF
    IF roomId = ROOM_CISTERN THEN
        PRINT ""
        PRINT "  You see the light before you see the source -- a faint, cold luminescence"
        PRINT "  at the edge of vision that keeps refusing to be where you look for it."
        PRINT "  Then it stops moving and you see it properly."
        PRINT ""
        PRINT "  The cold in the room is its cold. It has been here a long time. It is"
        PRINT "  not pleased to have company, and it intends to make that clear."
        PRINT ""
        PRINT "  You feel something drain from you before the fight has even begun."
        PRINT ""
        LET mSkill = RollDice(1) + 6
        LET mStamina = RollDice(1) + 6
        CALL CombatLoop(mSkill, mStamina, 0, 0, 1, 0, 0, 0, activeInterrupt)
        IF gameOver = 0 THEN
            LET monsterAlive[roomId - 1] = 0
            PRINT ""
            PRINT "  It does not fall so much as diminish -- the luminescence pulling"
            PRINT "  inward, flickering, and then gone. The cold lifts from the room"
            PRINT "  slowly, as though the stone itself needs time to remember warmth."
            PRINT ""
            PRINT "  The silence that follows is a different quality of silence than before."
            PRINT ""
        ELSE
            PRINT ""
            PRINT "  The last of your luck drains away and with it everything else."
            PRINT "  The cold in the room is the last thing you feel."
            PRINT ""
        END IF
    END IF
    IF roomId = ROOM_UNDERHALL THEN
        PRINT ""
        PRINT "  It does not move when you enter. It watches you from the centre of the"
        PRINT "  room with small, dark, patient eyes, and it waits to see what you do."
        PRINT ""
        PRINT "  It is enormous. The old wounds on its hide have closed and scarred and"
        PRINT "  closed again. You are going to have to be fast, or lucky, or both."
        PRINT ""
        PRINT "  It has decided you are worth its attention. It stands."
        PRINT ""
        LET mSkill = RollDice(1) + 7
        LET mStamina = RollDice(2) + 12
        CALL CombatLoop(mSkill, mStamina, 0, 1, 0, 0, 0, 0, activeInterrupt)
        IF gameOver = 0 THEN
            LET monsterAlive[roomId - 1] = 0
            PRINT ""
            PRINT "  It goes down at last. The wounds don't close this time."
            PRINT "  Whatever you took from it, you took enough."
            PRINT ""
        ELSE
            PRINT ""
            PRINT "  Every wound you deal closes before the next round. It outlasts you."
            PRINT "  That was always how this ended."
            PRINT ""
        END IF
    END IF
    IF roomId = ROOM_THRONE THEN
        PRINT ""
        PRINT "  He raises his head."
        PRINT ""
        PRINT "  That is all. He does not stand, does not speak, does not reach for a"
        PRINT "  weapon. He simply raises his head and looks at you across three hundred"
        PRINT "  years and a room full of other people's gold, and the weight of that"
        PRINT "  look is a physical thing."
        PRINT ""
        PRINT "  You are not ready for this. You turn back."
        PRINT ""
    END IF
END SUB


REM === REPLACE: main program block -- replaces the Issue 5 version ===
REM  Adds: ShuffleLoot() call, inventory/floor resets, SEARCH/INVENTORY/
REM  TAKE/DROP/USE commands in the game loop.

CALL InitExits()

WHILE keepPlaying = 1
    LET gameOver = 0
    LET endState = 0
    LET currentRoom = ROOM_ENTRY
    LET minStamina = 0
    LET turns = 0
    LET poisoned = 0
    LET luckTestCount = 0
    FOR i = 0 TO MAX_ROOMS - 1
        LET visited[i] = 0
        LET searched[i] = 0
        LET droppedCount[i] = 0
        FOR j = 0 TO MAX_ROOMS - 1
            LET droppedItem[i][j] = ITEM_EMPTY
        NEXT j
    NEXT i
    LET invCount = 0
    LET overburdened = 0
    LET armouryLocked = 1
    LET medalTaken = 0
    LET searchInterruptActive = 0
    FOR i = 1 TO 4
        LET inventory[i] = ITEM_EMPTY
    NEXT i
    CALL InitMonsters()
    CALL OpeningSequence()
    CALL ShuffleLoot()
    CALL EnterRoom(currentRoom, 0)

    WHILE gameOver = 0
        INPUT " > "; cmd$
        LET cmd$ = UPPER$(cmd$)
        SELECT CASE cmd$
            CASE "GO NORTH"
                CALL HandleGo(DIR_N)
                IF currentRoom > 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "GO SOUTH"
                CALL HandleGo(DIR_S)
                IF currentRoom > 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "GO EAST"
                CALL HandleGo(DIR_E)
                IF currentRoom > 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "GO WEST"
                CALL HandleGo(DIR_W)
                IF currentRoom > 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "GO NE"
                CALL HandleGo(DIR_NE)
                IF currentRoom > 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "FIGHT"
                CALL HandleFight(currentRoom)
                IF gameOver = 0 THEN
                    CALL EnterRoom(currentRoom, 1)
                END IF
            CASE "SEARCH"
                CALL HandleSearch()
            CASE "INVENTORY"
                CALL PrintInventoryScreen()
            CASE "TAKE ALL", "TAKE KEY", "TAKE POTION", "TAKE CHARM", "TAKE SHARD", "TAKE BREAD", "TAKE MEDAL", "TAKE ANTIDOTE", "TAKE BANGLE", "TAKE SWORD", "TAKE MOULDY"
                CALL HandleTake(RIGHT$(cmd$, LEN(cmd$) - 5))
            CASE "DROP KEY", "DROP POTION", "DROP CHARM", "DROP SHARD", "DROP BREAD", "DROP MEDAL", "DROP ANTIDOTE", "DROP BANGLE", "DROP SWORD", "DROP MOULDY"
                CALL HandleDrop(RIGHT$(cmd$, LEN(cmd$) - 5))
            CASE "USE KEY", "USE POTION", "USE CHARM", "USE SHARD", "USE BREAD", "USE MEDAL", "USE ANTIDOTE", "USE BANGLE", "USE SWORD", "USE MOULDY"
                CALL HandleUse(RIGHT$(cmd$, LEN(cmd$) - 4))
            CASE ELSE
                PRINT "  The dungeon does not respond to that."
                PRINT ""
        END SELECT
    WEND

    CALL PrintDeathScreen()
    INPUT "  Play again? (YES / NO): "; playAgain$
    LET playAgain$ = UPPER$(playAgain$)
    PRINT ""
    IF playAgain$ = "YES" THEN
        LET keepPlaying = 1
    ELSE
        LET keepPlaying = 0
    END IF
WEND
```
