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
- `SUB HandleQuit()` — QUIT command: instant, no confirmation, context-sensitive death flavour, `endState = 6`
- `CASE "QUIT"` added to game loop — calls `HandleQuit()`, no `EnterRoom` after

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
REM === ADD TO: file header comment block, after Issue 8 line ===

REM  Issue 9: The Bound King
REM  Boss fight, gold bags, crown, Bangle terror negation, QUIT command.


REM === ADD TO: constants block, after CONST MONSTER_TROLL = 10 ===

CONST MONSTER_KING = 11


REM === ADD TO: item code constants, after CONST ITEM_KEY = 10 ===

CONST ITEM_GOLD     = 11
CONST MAX_GOLD_BAGS = 4


REM === ADD TO: FUNCTION ItemCode, before CASE ELSE ===

        CASE "BAG", "GOLD"
            RETURN ITEM_GOLD


REM === ADD TO: FUNCTION ItemName, before CASE ELSE ===

        CASE ITEM_GOLD
            RETURN "Bag of Gold"


REM === ADD TO: globals block, after LET riddleSolved = 0 ===

REM ----------------------------------------------------------------
REM  Bound King state (Issue 9)
REM  goldBags:       bags of gold taken from the Throne Room (0-4)
REM  secondFight:    1 if King regenerated and must be fought again
REM  crownAvailable: 1 after SEARCH reveals the crown post-combat
REM  kingSkill:      King's SKILL value, fixed per run
REM  kingStamina:    King's current STAMINA this fight
REM ----------------------------------------------------------------
LET goldBags = 0
LET secondFight = 0
LET crownAvailable = 0
LET kingSkill = 0
LET kingStamina = 0
LET terrorActive = 0


REM === ADD TO: reset block inside WHILE keepPlaying, after Issue 8 reset lines ===

    LET goldBags = 0
    LET secondFight = 0
    LET crownAvailable = 0
    LET kingSkill = 0
    LET kingStamina = 0
    LET terrorActive = 0


REM === MODIFY: SUB CombatLoop -- add full terror block after variable init ===

REM  Add after LET poisonRoll = 0:

    REM  Full terror -- Bound King only.
    REM  Bangle of Courage negates terror entirely (passive item).
    IF isBoss = 1 AND HasItem(ITEM_BANGLE) = 0 THEN
        SET GLOBAL skill = skill - 2
        IF luck > 0 THEN
            SET GLOBAL luck = luck - 1
        END IF
        LET terrorActive = 1
    END IF


REM === MODIFY: SUB CombatLoop -- add Crushing Blow inside monster-wins branch ===

REM  Add after the Armour Shard damage reduction, before SET GLOBAL stamina:

                IF isBoss = 1 THEN
                    IF monsterAttack - playerAttack >= 3 THEN
                        LET baseDamage = 4
                        PRINT "  The blow lands with the weight of three hundred years of experience."
                        PRINT "  It takes you to one knee and you feel something break inside."
                    END IF
                END IF


REM === MODIFY: SUB CombatLoop -- add endState = 5 on player death ===

REM  Add inside the stamina <= 0 check in monster-wins branch:

                IF stamina <= 0 THEN
                    SET GLOBAL gameOver = 1
                    SET GLOBAL endState = 5
                END IF


REM === MODIFY: SUB CombatLoop -- add terror restore after combat WEND ===

REM  Add after the WEND and SET GLOBAL inCombat = 0:

    REM  Restore full terror modifier after combat ends (Bound King only).
    IF terrorActive = 1 THEN
        SET GLOBAL skill = skill + 2
        SET GLOBAL luck = MIN(luck + 1, startLuck)
    END IF


REM === NEW SUB: add after SUB RiddleRoomSequence ===

REM =================================================================
REM  SUB BoundKingSequence
REM  The Bound King boss encounter. Manages the full Throne Room
REM  sequence: encounter text, first fight, post-combat gold loop,
REM  regeneration-triggered second fight. Terror is handled inside
REM  CombatLoop (isBoss = 1). King stats persist across visits.
REM  Called from HandleFight when roomId = ROOM_THRONE.
REM =================================================================
SUB BoundKingSequence()
    PRINT ""
    PRINT "  The crown is fused to his temples in a way that stopped being jewellery"
    PRINT "  a very long time ago. The metal has grown into the skin, or the skin"
    PRINT "  has grown into the metal -- after three centuries the distinction has"
    PRINT "  ceased to matter. He wears it because there is no longer a version of"
    PRINT "  him that doesn't."
    PRINT ""
    PRINT "  He looks like a man in his forties. Tired in a way that has nothing to"
    PRINT "  do with sleep."
    PRINT ""
    PRINT "  He stands. He is very still for a moment. Then he comes toward you, and"
    PRINT "  there is nothing in his movement that suggests he expects this to end"
    PRINT "  differently than it always has."
    PRINT ""
    IF HasItem(ITEM_BANGLE) = 0 THEN
        PRINT "  The full weight of what he is lands on you before he takes three"
        PRINT "  steps. Your SKILL falters. Your luck feels suddenly thin."
        PRINT ""
    END IF
    REM  Roll king stats on first encounter only.
    IF kingSkill = 0 THEN
        SET GLOBAL kingSkill = CINT(RollDice(1)) + 9
        SET GLOBAL kingStamina = CINT(RollDice(2)) + 18
    END IF
    CALL CombatLoop(kingSkill, kingStamina, 0, 0, 0, 0, 0, 1, 0)
    IF stamina <= 0 THEN
        PRINT ""
        PRINT "  He watches you fall. He has seen it before. He returns to his throne,"
        PRINT "  sits, and waits. He has been waiting a long time. He can wait longer."
        PRINT ""
        RETURN
    END IF
    REM  King beaten.
    LET monsterAlive[ROOM_THRONE - 1] = 0
    PRINT ""
    PRINT "  The King falls. You don't wait. You can already see his fingers moving."
    PRINT ""
    REM  Gold loop -- player collects bags until they choose to leave or max out.
    LET wantsMoreGold = 1
    WHILE goldBags < MAX_GOLD_BAGS AND wantsMoreGold = 1 AND stamina > 0 AND gameOver = 0
        PRINT "  TAKE BAG, or LEAVE."
        PRINT ""
        INPUT " > "; goldCmd$
        LET goldCmd$ = UPPER$(goldCmd$)
        SELECT CASE goldCmd$
            CASE "TAKE ALL"
                PRINT "  You don't have time for that."
                PRINT ""
            CASE "TAKE BAG"
                CALL HandleTakeBag()
            CASE "LEAVE"
                LET wantsMoreGold = 0
            CASE "QUIT"
                CALL HandleQuit()
            CASE ELSE
                PRINT "  The dungeon does not respond to that."
                PRINT ""
        END SELECT
    WEND
    REM  Second fight -- King regenerated during gold collection.
    IF secondFight = 1 AND stamina > 0 AND gameOver = 0 THEN
        PRINT ""
        PRINT "  He is already on his feet when you turn back."
        PRINT ""
        PRINT "  The gold you took is still in your hands. He looks at it, then at"
        PRINT "  you. Whatever he feels about it does not reach his face."
        PRINT ""
        PRINT "  He comes toward you again. Same certainty. Same weight. He has done"
        PRINT "  this before. So, now, have you."
        PRINT ""
        IF HasItem(ITEM_BANGLE) = 0 THEN
            PRINT "  The full weight of what he is lands on you again."
            PRINT ""
        END IF
        SET GLOBAL kingStamina = CINT(RollDice(2)) + 18
        CALL CombatLoop(kingSkill, kingStamina, 0, 0, 0, 0, 0, 1, 0)
        IF stamina <= 0 THEN
            PRINT ""
            PRINT "  He watches you fall. He returns to his throne. He sits."
            PRINT ""
            RETURN
        END IF
        PRINT ""
        PRINT "  The King falls. This time, he does not move."
        PRINT ""
    END IF
END SUB

REM =================================================================
REM  SUB HandleTakeBag
REM  Adds a bag of gold to inventory, increments goldBags, and rolls
REM  for regeneration on bags after the first. Thresholds escalate:
REM  bag 2 = 11+, bag 3 = 9+, bag 4 = 7+ on 2d6.
REM  Called from the gold loop inside BoundKingSequence.
REM =================================================================
SUB HandleTakeBag()
    IF invCount >= 4 THEN
        PRINT "  Your hands are full. DROP something first."
        PRINT ""
        RETURN
    END IF
    SET GLOBAL goldBags = goldBags + 1
    CALL AddToInventory(ITEM_GOLD)
    PRINT "  Taken."
    PRINT ""
    REM  Regen rolls -- bags after the first risk waking the King.
    IF goldBags = 2 THEN
        IF CINT(RollDice(2)) >= 11 THEN
            PRINT "  His fingers move."
            PRINT ""
            SET GLOBAL secondFight = 1
        END IF
    END IF
    IF goldBags = 3 THEN
        IF CINT(RollDice(2)) >= 9 THEN
            PRINT "  His hand closes into a fist."
            PRINT ""
            SET GLOBAL secondFight = 1
        END IF
    END IF
    IF goldBags = 4 THEN
        IF CINT(RollDice(2)) >= 7 THEN
            PRINT "  He draws a breath."
            PRINT ""
            SET GLOBAL secondFight = 1
        END IF
    END IF
END SUB

REM =================================================================
REM  SUB CrownSequence
REM  Fires on TAKE CROWN. The crown never enters inventory -- it
REM  bypasses the carry limit entirely. Sets endState = 2 and ends
REM  the game. The player becomes the Bound King.
REM =================================================================
SUB CrownSequence()
    PRINT ""
    PRINT "  You pick it up. You don't mean to put it on. You do anyway."
    PRINT ""
    PRINT "  It fits perfectly. It always did."
    PRINT ""
    PRINT "  The Bound King opens his eyes."
    PRINT ""
    PRINT "  He looks at you for a long moment. Then he looks at his own hands --"
    PRINT "  empty, unburdened, free for the first time in three hundred years."
    PRINT ""
    PRINT "  He stands. He walks past you without a word. You hear his footsteps"
    PRINT "  on the stairs."
    PRINT ""
    PRINT "  You try to follow. Your feet will not move."
    PRINT ""
    PRINT "  You are still there when the torch goes out."
    PRINT ""
    SET GLOBAL endState = 2
    SET GLOBAL stamina = 0
    SET GLOBAL gameOver = 1
END SUB


REM === NEW SUB: add after SUB CrownSequence ===

REM =================================================================
REM  SUB HandleQuit
REM  Context-sensitive quit handler. Priority: monster alive in room,
REM  zombie present, room-specific special case, ambient pool.
REM  No confirmation. Sets endState = 6 and gameOver = 1.
REM =================================================================
SUB HandleQuit()
    PRINT ""
    LET handled = 0
    IF monsterAlive[currentRoom - 1] = 1 AND handled = 0 THEN
        SELECT CASE currentRoom
            CASE ROOM_GUARDROOM
                PRINT "  You lower your weapon. The Brute watches you do it. Then it does"
                PRINT "  what it was always going to do -- efficiently, without interest,"
                PRINT "  the way it has ended everything else that came through that door."
            CASE ROOM_COLLAPSED
                PRINT "  You stop moving. For a moment so does it. Then the chitinous legs"
                PRINT "  find their grip and it crosses the room in the time it takes you"
                PRINT "  to exhale. The last thing you feel is the cold of it, and then"
                PRINT "  nothing."
            CASE ROOM_PIT
                PRINT "  You drop your guard and the Guardian notes it with the same"
                PRINT "  unhurried certainty it brings to everything. It raises its weapon."
                PRINT "  It does not hurry. It has never needed to."
            CASE ROOM_CISTERN
                PRINT "  The luminescence fixes on you. The cold doubles, then redoubles."
                PRINT "  You don't fight it. That is the last decision you make."
            CASE ROOM_UNDERHALL
                PRINT "  The small dark eyes watch you put your hands down. The Troll"
                PRINT "  stands. It has been waiting for this. It comes forward with a"
                PRINT "  patience that has outlasted everything else down here, and it"
                PRINT "  outlasts you too."
            CASE ROOM_THRONE
                PRINT "  You sink to your knees before him. He looks through you as though"
                PRINT "  you are not there -- as though you have never been there, as"
                PRINT "  though the effort of acknowledgement is beneath him. His sword"
                PRINT "  finds your neck without him appearing to move. The last thing you"
                PRINT "  see is his face, and it holds nothing. Not contempt. Not"
                PRINT "  satisfaction. Nothing at all."
        END SELECT
        LET handled = 1
    END IF
    IF zombieAlive = 1 AND zombieRoom = currentRoom AND handled = 0 THEN
        PRINT "  It doesn't rush. It has never needed to rush. You stand still long"
        PRINT "  enough for it to find you, and then it takes its time. Whatever is"
        PRINT "  left of you when it is done is not much."
        LET handled = 1
    END IF
    IF handled = 0 THEN
        SELECT CASE currentRoom
            CASE ROOM_PIT
                PRINT "  You stand at the edge and look down. Then something you never"
                PRINT "  saw -- never heard -- places a hand flat against your back and"
                PRINT "  pushes. Not hard. It barely needs to. The darkness below takes"
                PRINT "  you, and you fall for longer than you thought possible. The"
                PRINT "  bottom, when it comes, does not come gently."
            CASE ROOM_CISTERN
                PRINT "  You are looking at the water when it happens. A single dark"
                PRINT "  shape beneath the surface, and then a cold pressure around your"
                PRINT "  ankle, and then you are moving and the room is moving away from"
                PRINT "  you and the water closes over your head. The channel beneath is"
                PRINT "  deep. Deeper than the dungeon has any right to go."
            CASE ROOM_RIDDLE
                PRINT "  You face the doors. It doesn't matter anymore which one. You"
                PRINT "  pick one -- the wrong one, of course; it was always going to be"
                PRINT "  the wrong one -- and as it swings open you understand for a"
                PRINT "  single clear instant what the riddle was actually about. Then"
                PRINT "  the understanding is gone, and so are you."
            CASE ROOM_GATE
                PRINT "  You hear it before you turn: a low, resonant creak from the"
                PRINT "  Gate itself, the sound of something that has not moved in a very"
                PRINT "  long time deciding to move. You turn. The Gate is open. Whatever"
                PRINT "  is on the other side has been there since before the dungeon was"
                PRINT "  built, and it has been patient, and you have come close enough."
                PRINT "  It steps through. The last thing you see is the dark beyond the"
                PRINT "  threshold closing behind it like a curtain."
            CASE ELSE
                LET qLine = CINT(RND() * 6) + 1
                SELECT CASE qLine
                    CASE 1
                        PRINT "  You hear something behind you. Not footsteps -- the"
                        PRINT "  absence of footsteps. A silence that follows you and gets"
                        PRINT "  closer. You turn too slowly. The cold enters you between"
                        PRINT "  the shoulder blades, precise and final, and the torchlight"
                        PRINT "  dims in a straight line down to nothing."
                    CASE 2
                        PRINT "  The torch goes out. In the dark, something is already"
                        PRINT "  there. You hear it breathing -- or something like"
                        PRINT "  breathing, something close enough. You do not have time"
                        PRINT "  to relight the flame."
                    CASE 3
                        PRINT "  The floor shifts under your foot. Just once, just slightly."
                        PRINT "  By the time you understand what it means, the stone has"
                        PRINT "  already decided, and you with it. The chamber below is"
                        PRINT "  silent. You are not."
                    CASE 4
                        PRINT "  There is a gap in the wall at arm height that you never"
                        PRINT "  looked at closely. There is a hand in it, pale and entirely"
                        PRINT "  still, and it has been there the whole time. Now it moves."
                        PRINT "  It is faster than it has any right to be."
                    CASE 5
                        PRINT "  The cold intensifies. That is all. Just cold, deepening,"
                        PRINT "  past the point where cold feels like cold and becomes"
                        PRINT "  something else instead -- a pressure, a weight, a slow"
                        PRINT "  erasure. You are less warm than you were. Then less"
                        PRINT "  present. Then gone."
                    CASE 6
                        PRINT "  Something in the room has been waiting. Not the monster --"
                        PRINT "  not anything you saw. Something older than the room itself,"
                        PRINT "  something that was here when the stones were cut and has"
                        PRINT "  been here every moment since. You only know it is there"
                        PRINT "  the moment before it decides you have stayed long enough."
                END SELECT
        END SELECT
    END IF
    PRINT ""
    SET GLOBAL endState = 6
    SET GLOBAL gameOver = 1
END SUB


REM === ADD TO: SUB HandleSearch -- after floor item loop, before "nothing here" ===

    IF currentRoom = ROOM_THRONE AND monsterAlive[ROOM_THRONE - 1] = 0 THEN
        SET GLOBAL crownAvailable = 1
        PRINT "  Beneath the piled coin and the fallen king you see it clearly for the"
        PRINT "  first time. The crown. Solid gold, heavily jewelled -- rubies, sapphires,"
        PRINT "  stones you don't have names for. Nothing else in this room comes close."
        PRINT "  Nothing you have ever owned comes close. One item. Carried out of here"
        PRINT "  and you never work another day. You never fear another debt. You never"
        PRINT "  need anything from anyone again."
        PRINT ""
        PRINT "  It is still on his head. His head is right there."
        LET foundSomething = 1
    END IF


REM === ADD TO: SUB HandleFight -- Throne Room branch, before zombie branch ===

    IF roomId = ROOM_THRONE THEN
        CALL BoundKingSequence()
    END IF


REM === ADD TO: game loop SELECT CASE, before CASE "SEARCH" ===

            CASE "TAKE CROWN"
                IF currentRoom = ROOM_THRONE AND crownAvailable = 1 THEN
                    CALL CrownSequence()
                ELSE
                    IF currentRoom = ROOM_THRONE AND monsterAlive[ROOM_THRONE - 1] = 1 THEN
                        PRINT "  The King sits between you and the gold."
                        PRINT ""
                    ELSE
                        PRINT "  There is nothing here that fits that description."
                        PRINT ""
                    END IF
                END IF
            CASE "TAKE BAG", "TAKE GOLD"
                IF currentRoom = ROOM_THRONE AND monsterAlive[ROOM_THRONE - 1] = 1 THEN
                    PRINT "  The King sits between you and the gold."
                    PRINT ""
                ELSE
                    CALL HandleTake(RIGHT$(cmd$, LEN(cmd$) - 5))
                END IF


REM === ADD TO: game loop SELECT CASE, before CASE ELSE ===

            CASE "QUIT"
                CALL HandleQuit()


REM === MODIFY: SUB PrintDeathScreen -- replace endState 3 / ELSE block ===

REM  Replace the existing IF endState = 3 / ELSE block with:

    IF endState = 2 THEN
        PRINT "  You are the Bound King now."
    END IF
    IF endState = 3 THEN
        PRINT "  You pass through the door into a room straight from your worst nightmare."
        PRINT "  The mouldy bones of previous adventurers litter the floor. Realising"
        PRINT "  your mistake you turn to leave -- only to find the door has no handle."
        PRINT "  On closer examination you notice bloody scratches, and is that a fingernail?"
        PRINT ""
        PRINT "  A deep rumble shakes your body and turns your bowels to water. You look"
        PRINT "  up to see the ceiling, slow but inexorably, lowering."
        PRINT ""
        PRINT "  You frantically try to open the door, adding to the bloody legacy, as"
        PRINT "  the room crushes the life out of you."
    END IF
    IF endState = 6 THEN
        PRINT "  The dungeon has been here longer than anyone you have ever met has been"
        PRINT "  alive. It will be here after everyone you know is dust."
        PRINT "  It did not notice you arrive."
        PRINT "  It did not notice you leave."
    END IF
    IF endState = 5 THEN
        PRINT "  Your STAMINA reached zero."
    END IF
    IF endState <> 2 THEN
        PRINT ""
        PRINT "  Your SKILL was " & startSkill & ". Your STAMINA reached as low as " & minStamina & "."
        PRINT "  You tested your LUCK " & luckTestCount & " times."
    END IF
```
