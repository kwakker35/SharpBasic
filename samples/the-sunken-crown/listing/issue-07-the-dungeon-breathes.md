# The Sunken Crown
## Issue 7 — The Dungeon Breathes

*The dungeon is not waiting for you.*

---

## The Story So Far

Issues 1 through 6 built a navigable dungeon with combat and inventory. The player has attributes, can fight, can pick up items, and can search rooms.

But the dungeon is still static between actions. Nothing happens unless the player makes it happen. The atmosphere exists only in the room descriptions. Between commands, the world holds its breath.

This issue changes that. The dungeon gets its own rhythms. Things happen whether the player is ready or not. And something starts moving through the passages.

---

## What's New This Issue

- `SUB AdvanceTurns(n AS INTEGER)` — central time management, fires per-turn events
- `SUB AtmosphericEvent()` — selects and prints one of 12 atmospheric events at random; cases 1–7 and 12 suppressed during combat; cases 8–10 (STAMINA drain) always fire; case 10 text varies by `inCombat`
- `inCombat` global flag — set to 1 at start of `CombatLoop`, cleared to 0 on exit
- Wandering zombie — `zombieSpawned`, `zombieAlive`, `zombieRoom` state variables; reset on new game
- `SUB WanderZombie()` — moves the zombie one step via the exit arrays each turn, excludes ROOM_STILL, ROOM_RIDDLE, ROOM_THRONE and any room with a live fixed monster
- Zombie combat branch added to `HandleFight` — fires when zombie is alive and in the current room
- Zombie SEARCH interrupt text added to `HandleSearch` — random line from Deliverable 8
- Poison check added to top of `EnterRoom` — fires before room description, STAMINA -1
- Zombie encounter prompt added to bottom of `EnterRoom` — fires after room description
- `SUB HandleLook()` — repeats room description, costs 1 turn
- `SUB HandleSneak(dir AS INTEGER)` — `SNEAK NORTH` etc; no monster: move costs 2 turns; monster present: 1-turn SKILL roll; success moves player, failure triggers combat; monster stays alive
- GO blocked by live monster — refusal message, no combat, no turn cost
- `CASE "LOOK"` and `CASE "SNEAK"` added to game loop
- All bare `SET GLOBAL turns = turns + N` replaced with `CALL AdvanceTurns(N)` throughout
- `CALL AdvanceTurns(1)` added to `HandleGo` on successful move

---

## Concept — The Turn Counter and RND-Driven Events

Every time-consuming action increments a global counter through `AdvanceTurns`:

```
LET turns = turns + 1
```

After each increment, the event system runs a probability check:

```
IF RND() * 10 > 8.5 THEN
    CALL AtmosphericEvent()
END IF
```

Break this down:

- `RND()` returns a value from 0.0 to just under 1.0
- Multiply by 10 to get a range from 0.0 to just under 10.0
- Compare to 8.5 — the result exceeds 8.5 roughly 15% of the time
- When it does, `AtmosphericEvent()` fires

15% is not every turn. It is not never. It is often enough to feel alive, irregular enough to feel unscripted. The dungeon breathes on its own schedule.

The zombie is a second system running on the same counter. Every turn after spawning, `WanderZombie()` runs. The zombie picks a random valid exit from its current room and moves there. It uses the same exit arrays the player uses — no separate pathfinding, no special logic. It wanders.

The encounter happens when the numbers align. Your position and the zombie's position happen to be the same room on the same turn. Neither system knows about the other. The emergent behaviour — the feeling that the dungeon is alive and something is moving through it — comes from two simple systems sharing one counter.

---

## How It Fits

`AdvanceTurns` replaces every bare `turns = turns + 1` from Issue 5 onward. Every command that costs turns — GO, FIGHT, LOOK, SEARCH, USE — now calls `AdvanceTurns` with the appropriate count. This is the single place where the dungeon's time system lives.

`EnterRoom` gains two new checks: poison damage fires at the top before the room description, and the zombie encounter check fires at the bottom after it. Both are additions to the existing SUB — its core structure does not change.

The atmospheric event text for all 12 events comes from the content asset file, Deliverable 3.

---

## What You'll See

Move through the dungeon. Atmospheric events start appearing between actions — the torch gutters, something scrapes in the dark below, the wall is warm when you touch it. They fire irregularly. No two runs feel quite the same.

After a few turns the zombie may spawn at the Crossroads. Move fast and you may never see it. Linger — searching rooms, fighting long battles — and eventually your paths cross. When they do, FIGHT and SNEAK both work against it.

Navigate to room 6 and fight the Skittering Horror. Win. Move to the next room. The poison message fires before the room description. Navigate to room 9 to find the Antidote Vial.

---

## What to Try

Change the atmospheric event threshold from `8.5` to `5`. Events fire roughly half the time now. Play through the first few rooms. Does the dungeon feel more alive or just noisier?

The threshold is a design decision as much as a code decision. 8.5 is right for this game — but understanding why requires knowing what the wrong numbers feel like. Change it back to 8.5 before Issue 8.

---

## The Listing

```
REM === ADD TO: file header comment block, after Issue 6 line ===
REM  Issue 7: The Dungeon Breathes
REM  Atmospheric events, wandering zombie, SNEAK, LOOK, AdvanceTurns, poison drain.


REM === ADD TO: globals block, after LET luckTestCount = 0 ===

REM ----------------------------------------------------------------
REM  Zombie state
REM  zombieSpawned: 0 = not yet in dungeon, 1 = spawned at Crossroads
REM  zombieAlive:   0 = dead or unspawned, 1 = roaming
REM  zombieRoom:    current room (1-12); 0 when unspawned
REM ----------------------------------------------------------------
LET zombieSpawned = 0
LET zombieAlive = 0
LET zombieRoom = 0


REM === MODIFY: SUB QueueFlavour -- add inCombat bypass ===
REM  Replace the body of QueueFlavour. When inCombat = 1, print
REM  immediately so combat-time atmospheric events stay inline.

SUB QueueFlavour(line$ AS STRING)
    IF inCombat = 1 THEN
        PRINT line$
    ELSE
        IF pendingFlavourCount < 30 THEN
            LET pendingFlavour[pendingFlavourCount] = line$
            SET GLOBAL pendingFlavourCount = pendingFlavourCount + 1
        END IF
    END IF
END SUB


REM === ADD TO: reset block, add LET inCombat = 0 after LET poisoned = 0 ===

    LET inCombat = 0

REM === ADD TO: reset block, after LET searchInterruptActive = 0 ===

    LET zombieSpawned = 0
    LET zombieAlive = 0
    LET zombieRoom = 0


REM === ADD BEFORE: SUB HandleSearch ===

REM =================================================================
REM  SUB WanderZombie
REM  Moves the zombie one step through the dungeon each turn.
REM  Uses the same exit arrays as the player. Excludes
REM  ROOM_STILL (5), ROOM_RIDDLE (8), ROOM_THRONE (11).
REM =================================================================
SUB WanderZombie()
    LET zStart = roomExitStart[zombieRoom - 1]
    LET zCount = roomExitCount[zombieRoom - 1]
    LET validCount = 0
    FOR i = zStart TO zStart + zCount - 1
        LET dest = exitDest[i]
        IF dest <> ROOM_STILL AND dest <> ROOM_RIDDLE AND dest <> ROOM_THRONE AND monsterAlive[dest - 1] = 0 THEN
            LET validCount = validCount + 1
        END IF
    NEXT i
    IF validCount = 0 THEN
        RETURN
    END IF
    LET pick = CINT(RND() * validCount) + 1
    LET seen = 0
    FOR i = zStart TO zStart + zCount - 1
        LET dest = exitDest[i]
        IF dest <> ROOM_STILL AND dest <> ROOM_RIDDLE AND dest <> ROOM_THRONE AND monsterAlive[dest - 1] = 0 THEN
            LET seen = seen + 1
            IF seen = pick THEN
                SET GLOBAL zombieRoom = dest
                RETURN
            END IF
        END IF
    NEXT i
END SUB

REM =================================================================
REM  SUB AtmosphericEvent
REM  Fires one of 12 atmospheric events chosen at random.
REM  Events 1-7 are pure flavour. Events 8-9 deal STAMINA -1.
REM  Event 10 is conditional on overburdened. Events 11-12 are
REM  threat escalation: zombie spawn check and proximity presence.
REM  Text sourced from Deliverable 3 of the content asset file.
REM =================================================================
SUB AtmosphericEvent()
    LET evt = CINT(RND() * 12) + 1
    SELECT CASE evt
        CASE 1
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  Your torch gutters. For a moment the darkness is absolute -- a living,")
                CALL QueueFlavour("  pressing thing. Then the flame catches again. You breathe out. You")
                CALL QueueFlavour("  hadn't noticed you'd stopped.")
                CALL QueueFlavour("")
            END IF
        CASE 2
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  Somewhere below you -- deeper than you've been -- something moves. A")
                CALL QueueFlavour("  slow, rhythmic scraping. Stone on stone. It stops. You wait. It doesn't")
                CALL QueueFlavour("  start again.")
                CALL QueueFlavour("")
            END IF
        CASE 3
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  You place your hand against the wall to steady yourself. The stone is")
                CALL QueueFlavour("  warm. Not the warmth of your own body heat -- it was warm before you")
                CALL QueueFlavour("  touched it. You take your hand away.")
                CALL QueueFlavour("")
            END IF
        CASE 4
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  The air changes. A cold current from nowhere, carrying the smell of")
                CALL QueueFlavour("  standing water and something older. Something that hasn't seen light")
                CALL QueueFlavour("  in a very long time.")
                CALL QueueFlavour("")
            END IF
        CASE 5
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  You almost step on them. Small. Difficult to identify. You don't try.")
                CALL QueueFlavour("")
            END IF
        CASE 6
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  The dungeon goes completely quiet. No drip of water, no distant groan")
                CALL QueueFlavour("  of settling stone. Nothing. It lasts perhaps ten seconds. It feels much")
                CALL QueueFlavour("  longer. Then the sounds return, and you realise how much you'd been")
                CALL QueueFlavour("  relying on them.")
                CALL QueueFlavour("")
            END IF
        CASE 7
            IF inCombat = 0 THEN
                CALL QueueFlavour("")
                CALL QueueFlavour("  Scratches in the wall at eye height. You lean close. Letters, cut deep")
                CALL QueueFlavour("  and fast by someone who knew they were running out of time. Three words.")
                CALL QueueFlavour("  The middle one is illegible. The first is " & CHR$(34) & "do not" & CHR$(34) & " and the last is " & CHR$(34) & "alone." & CHR$(34))
                CALL QueueFlavour("")
            END IF
        CASE 8
            CALL QueueFlavour("")
            CALL QueueFlavour("  The cold gets into you. Not the sharp cold of wind -- the slow cold of")
            CALL QueueFlavour("  deep stone, of places that have never been warm. You feel it in your")
            CALL QueueFlavour("  chest first.")
            CALL QueueFlavour("")
            SET GLOBAL stamina = stamina - 1
            SET GLOBAL minStamina = MIN(minStamina, stamina)
            CALL QueueFlavour("  STAMINA: " & stamina)
            CALL QueueFlavour("")
            IF stamina <= 0 THEN
                SET GLOBAL gameOver = 1
                SET GLOBAL endState = 5
            END IF
        CASE 9
            CALL QueueFlavour("")
            CALL QueueFlavour("  You haven't eaten. You'd forgotten until now. Your body reminds you")
            CALL QueueFlavour("  with a clarity that's difficult to ignore.")
            CALL QueueFlavour("")
            SET GLOBAL stamina = stamina - 1
            SET GLOBAL minStamina = MIN(minStamina, stamina)
            CALL QueueFlavour("  STAMINA: " & stamina)
            CALL QueueFlavour("")
            IF stamina <= 0 THEN
                SET GLOBAL gameOver = 1
                SET GLOBAL endState = 5
            END IF
        CASE 10
            IF overburdened = 1 THEN
                IF inCombat = 1 THEN
                    CALL QueueFlavour("")
                    CALL QueueFlavour("  The weight you are carrying pulls at your arms. You are slower")
                    CALL QueueFlavour("  than you should be. It costs you.")
                    CALL QueueFlavour("")
                ELSE
                    CALL QueueFlavour("")
                    CALL QueueFlavour("  Your foot catches on uneven stone. The weight pulls you down hard.")
                    CALL QueueFlavour("  You catch yourself on one knee. It costs you.")
                    CALL QueueFlavour("")
                END IF
                SET GLOBAL stamina = stamina - 1
                SET GLOBAL minStamina = MIN(minStamina, stamina)
                CALL QueueFlavour("  STAMINA: " & stamina)
                CALL QueueFlavour("")
                IF stamina <= 0 THEN
                    SET GLOBAL gameOver = 1
                    SET GLOBAL endState = 5
                END IF
            ELSE
                IF inCombat = 0 THEN
                    CALL QueueFlavour("")
                    CALL QueueFlavour("  Your foot catches on uneven stone. You catch yourself easily.")
                    CALL QueueFlavour("  A reminder that the floor is not your friend.")
                    CALL QueueFlavour("")
                END IF
            END IF
        CASE 11
            IF zombieSpawned = 0 AND turns > 3 THEN
                IF RND() * 10 > 7 THEN
                    SET GLOBAL zombieSpawned = 1
                    SET GLOBAL zombieAlive = 1
                    SET GLOBAL zombieRoom = ROOM_CROSSROADS
                END IF
            END IF
        CASE 12
            IF zombieAlive = 1 AND inCombat = 0 THEN
                LET zNear = 0
                LET zStart = roomExitStart[currentRoom - 1]
                LET zCount = roomExitCount[currentRoom - 1]
                FOR i = zStart TO zStart + zCount - 1
                    IF exitDest[i] = zombieRoom THEN LET zNear = 1 END IF
                NEXT i
                IF zombieRoom = currentRoom THEN LET zNear = 1 END IF
                IF zNear = 1 THEN
                    CALL QueueFlavour("")
                    CALL QueueFlavour("  Something is moving in the dungeon. Not far away. The sound has")
                    CALL QueueFlavour("  direction.")
                    CALL QueueFlavour("")
                ELSE
                    CALL QueueFlavour("")
                    CALL QueueFlavour("  You have the distinct feeling you are not alone. You stand very")
                    CALL QueueFlavour("  still. Whatever it was, it passes.")
                    CALL QueueFlavour("")
                END IF
            END IF
    END SELECT
END SUB

REM =================================================================
REM  SUB AdvanceTurns -- n AS INTEGER
REM  Central time manager. Increments the turn counter n times.
REM  Each increment runs a 15% RND check for an atmospheric event
REM  and moves the zombie one step if it is alive and spawned.
REM  Stops early if gameOver is set during event processing.
REM =================================================================
SUB AdvanceTurns(n AS INTEGER)
    FOR t = 1 TO n
        IF gameOver = 0 THEN
            SET GLOBAL turns = turns + 1
            IF RND() * 10 > 8.5 THEN
                CALL AtmosphericEvent()
            END IF
            IF zombieSpawned = 1 AND zombieAlive = 1 THEN
                CALL WanderZombie()
            END IF
        END IF
    NEXT t
END SUB


REM === ADD BEFORE: SUB HandleGo ===

REM =================================================================
REM  SUB HandleLook
REM  Advances one turn (triggers atmospheric and zombie events)
REM  then returns. The game loop re-renders the room via EnterRoom.
REM =================================================================
SUB HandleLook()
    CALL AdvanceTurns(1)
END SUB

REM =================================================================
REM  SUB HandleSneak
REM  Attempts to pass a monster or the wandering zombie unseen in
REM  the given direction. Finds the exit first -- invalid direction
REM  costs nothing. No hostile present: move, costs 2 turns.
REM  ROOM_THRONE always refuses. Roll: RollDice(2) <= skill = success
REM  (move to dest, 1 turn). Failure: fight in current room, no move.
REM =================================================================
SUB HandleSneak(dir AS INTEGER)
    REM --- Collapsed Passage: entering from Cistern clears the rubble ---
    IF currentRoom = ROOM_CISTERN AND dir = DIR_N AND exitHidden[11] = 1 THEN
        LET exitHidden[11] = 0
        CALL QueueFlavour("  You push through from the south. The rubble shifts. The way through is clear.")
        CALL QueueFlavour("")
    END IF
    LET dest = 0
    LET sStart = roomExitStart[currentRoom - 1]
    LET sCount = roomExitCount[currentRoom - 1]
    FOR i = sStart TO sStart + sCount - 1
        IF exitDir[i] = dir AND exitHidden[i] = 0 THEN
            LET dest = exitDest[i]
        END IF
    NEXT i
    IF dest = 0 THEN
        PRINT "  There is no way through in that direction."
        PRINT ""
        RETURN
    END IF
    LET target = 0
    IF monsterAlive[currentRoom - 1] = 1 THEN
        LET target = 1
    END IF
    IF zombieAlive = 1 AND zombieRoom = currentRoom THEN
        LET target = 1
    END IF
    IF target = 0 THEN
        SET GLOBAL currentRoom = dest
        CALL AdvanceTurns(2)
        RETURN
    END IF
    IF currentRoom = ROOM_THRONE THEN
        PRINT "  He is already looking at you. There is no passing unseen."
        PRINT ""
        RETURN
    END IF
    CALL AdvanceTurns(1)
    LET roll = RollDice(2)
    IF roll <= skill THEN
        CALL QueueFlavour("  You hold your breath and move with extreme care. It does not notice.")
        CALL QueueFlavour("  You are through.")
        CALL QueueFlavour("")
        SET GLOBAL currentRoom = dest
    ELSE
        PRINT "  It clocks you before you are halfway across."
        PRINT ""
        CALL HandleFight(currentRoom)
    END IF
END SUB


REM === MODIFY: SUB CombatLoop -- set/clear inCombat flag ===

REM  Add immediately after SUB CombatLoop(...) declaration:
    SET GLOBAL inCombat = 1

REM  Add immediately before END SUB (after terror restore block):
    SET GLOBAL inCombat = 0

REM === MODIFY: SUB HandleGo -- add monster/zombie block, rubble-clearing, AdvanceTurns ===

REM  Add at top of SUB HandleGo, before LET start = ...:

    IF monsterAlive[currentRoom - 1] = 1 THEN
        CALL QueueFlavour("  It moves to block your path. FIGHT or SNEAK.")
        CALL QueueFlavour("")
        RETURN
    END IF
    IF zombieAlive = 1 AND zombieRoom = currentRoom THEN
        CALL QueueFlavour("  It moves to block your path. FIGHT or SNEAK.")
        CALL QueueFlavour("")
        RETURN
    END IF
    REM --- Collapsed Passage: entering from Cistern clears the rubble ---
    IF currentRoom = ROOM_CISTERN AND dir = DIR_N AND exitHidden[11] = 1 THEN
        LET exitHidden[11] = 0
        CALL QueueFlavour("  You push through from the south. The rubble shifts. The way through is clear.")
        CALL QueueFlavour("")
    END IF

REM  Replace bare RETURN inside the exit-found branch with:

            CALL AdvanceTurns(1)
            RETURN


REM === MODIFY: SUB HandleSearch -- add zombie interrupt before monsterAlive check ===

    IF zombieAlive = 1 AND zombieRoom = currentRoom THEN
        LET zLine = CINT(RND() * 3) + 1
        SELECT CASE zLine
            CASE 1
                PRINT "  You hear it before you see it -- a wet, irregular shuffling."
                PRINT "  It has found you."
            CASE 2
                PRINT "  Something cold brushes your arm. You spin."
                PRINT "  It is closer than it should be."
            CASE 3
                PRINT "  The smell reaches you first. Then the shape in the dark shifts."
                PRINT "  It is already here."
        END SELECT
        PRINT ""
        CALL AdvanceTurns(2)
        SET GLOBAL searchInterruptActive = 1
        CALL HandleFight(currentRoom)
        RETURN
    END IF


REM === MODIFY: SUB EnterRoom -- add poison check after CALL FlushFlavour() ===

    IF poisoned = 1 THEN
        SET GLOBAL stamina = stamina - 1
        SET GLOBAL minStamina = MIN(minStamina, stamina)
        IF stamina <= 0 THEN
            SET GLOBAL gameOver = 1
            SET GLOBAL endState = 5
            RETURN
        END IF
        PRINT "  The poison spreads further through your blood. You feel it costing you."
        PRINT ""
    END IF

REM  Add at bottom of SUB EnterRoom, after existing monsterAlive prompt block:

    IF zombieAlive = 1 AND zombieRoom = roomId AND monsterAlive[roomId - 1] = 0 THEN
        PRINT ""
        PRINT "  FIGHT to engage, or SNEAK to try to pass."
        PRINT ""
    END IF


REM === MODIFY: SUB HandleFight -- fix header comment ===

REM  Issue 5: Guardroom Brute. Issue 6: Horror, Guardian, Mage, Troll.
REM  Issue 7: Zombie. Bound King wired in Issue 9.

REM === MODIFY: SUB HandleFight -- add zombieHere guard, replace monsterAlive check ===

REM  At top of SUB HandleFight, after SET GLOBAL searchInterruptActive = 0:

    LET zombieHere = 0
    IF zombieAlive = 1 AND zombieRoom = currentRoom THEN
        LET zombieHere = 1
    END IF
    IF monsterAlive[roomId - 1] = 0 AND zombieHere = 0 THEN
        PRINT "  There is nothing here that requires that."
        PRINT ""
        RETURN
    END IF

REM === ADD TO: SUB HandleFight -- zombie combat branch, before END SUB ===

    IF zombieHere = 1 THEN
        PRINT ""
        PRINT "  You almost walk into it. The smell hits you first -- something cold"
        PRINT "  and wrong, like earth and old blood. It turns toward you with the"
        PRINT "  slow, indifferent purpose of something that does not need to hurry."
        PRINT ""
        LET mSkill = RollDice(1) + 2
        LET mStamina = RollDice(2) + 8
        CALL CombatLoop("Wandering Zombie", mSkill, mStamina, 0, 0, 0, 0, 0, 0, activeInterrupt)
        IF gameOver = 0 THEN
            SET GLOBAL zombieAlive = 0
            PRINT ""
            PRINT "  It goes down without drama -- no last surge, no last sound. It just"
            PRINT "  stops. Whatever drove it is gone. You stand over it until you are"
            PRINT "  certain. The dungeon is quiet."
            PRINT ""
        ELSE
            PRINT ""
            PRINT "  It does not stop. You fall back, then fall. The dungeon does not care."
            PRINT ""
        END IF
    END IF


REM === ADD TO: game loop SELECT CASE, before CASE ELSE ===

REM === MODIFY: game loop -- add FlushFlavour before INPUT prompt ===
REM  Replace:
 REM     WHILE gameOver = 0
REM         INPUT " > "; cmd$
REM  With:
REM      WHILE gameOver = 0
REM          CALL FlushFlavour()
REM          INPUT " > "; cmd$

            CASE "LOOK"
                CALL HandleLook()
                CALL EnterRoom(currentRoom, 0)
            CASE "SNEAK NORTH"
                CALL HandleSneak(DIR_N)
                IF gameOver = 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "SNEAK SOUTH"
                CALL HandleSneak(DIR_S)
                IF gameOver = 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "SNEAK EAST"
                CALL HandleSneak(DIR_E)
                IF gameOver = 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "SNEAK WEST"
                CALL HandleSneak(DIR_W)
                IF gameOver = 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
            CASE "SNEAK NE"
                CALL HandleSneak(DIR_NE)
                IF gameOver = 0 THEN
                    CALL EnterRoom(currentRoom, 0)
                END IF
```
