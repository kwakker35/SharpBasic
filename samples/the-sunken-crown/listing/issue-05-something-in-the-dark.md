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
- `CASE "CHEAT"` — dev command: sets SKILL to 12, STAMINA to 100. Strip before release (see Issue 10).

---

## Concept — WHILE Loops

Combat runs in a loop. The loop continues while both sides are still standing:

```
WHILE stamina > 0 AND monsterStamina > 0
    LET playerAttack = AttackStrength(skill)
    LET monsterAttack = AttackStrength(monsterSkill)
    REM compare, apply damage, print result
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
REM === ADD TO: constants block, after CONST MAX_ROOMS = 12 ===

REM ----------------------------------------------------------------
REM  Monster identity constants
REM  Room numbers for each fixed monster. Declared as CONST so the
REM  code always refers to monsters by name, not by raw room number.
REM ----------------------------------------------------------------
CONST MONSTER_BRUTE = 2
CONST MONSTER_HORROR = 6
CONST MONSTER_GUARDIAN = 7
CONST MONSTER_MAGE = 9
CONST MONSTER_TROLL = 10
CONST MONSTER_KING = 11


REM === ADD TO: globals block, replace the REM/LET for gameOver ===

REM ----------------------------------------------------------------
REM  Game loop control
REM  gameOver: 0 = game running, 1 = game ended
REM  endState: 0 = ongoing, 1=win, 2=became king, 3=riddle,
REM            4=gate, 5=stamina
REM ----------------------------------------------------------------
LET gameOver = 0
LET endState = 0
LET keepPlaying = 1

REM ----------------------------------------------------------------
REM  Combat and status state
REM  monsterAlive[r-1]: 0 = dead/absent, 1 = present and alive
REM  minStamina: lowest STAMINA reached this run (for end screen)
REM  turns: total turns elapsed
REM  poisoned: 0 = clean, 1 = poisoned
REM  luckTestCount: number of LUCK tests taken (for end screen)
REM ----------------------------------------------------------------
DIM monsterAlive[MAX_ROOMS] AS INTEGER
LET minStamina = 0
LET turns = 0
LET poisoned = 0
LET luckTestCount = 0


REM === REPLACE: SUB RollStartingStats() -- add last line before END SUB ===

    SET GLOBAL minStamina = stamina


REM === NEW SUB: add after SUB InitExits() ===

SUB InitMonsters()
    LET monsterAlive[ROOM_GUARDROOM - 1] = 1   REM  Guardroom Brute
    LET monsterAlive[ROOM_COLLAPSED - 1] = 1   REM  Skittering Horror
    LET monsterAlive[ROOM_PIT - 1] = 1         REM  Pit Guardian
    LET monsterAlive[ROOM_CISTERN - 1] = 1     REM  Hollow Mage
    LET monsterAlive[ROOM_UNDERHALL - 1] = 1   REM  Troll
    LET monsterAlive[ROOM_THRONE - 1] = 1      REM  Bound King
END SUB


REM === REPLACE: SUB PrintRoom(), ROOM_GUARDROOM revisit branch ===
REM  Find: IF visited[roomId - 1] = 1 THEN (inside roomId = ROOM_GUARDROOM)
REM  Replace the entire IF/ELSE visited block with the version below.

        IF visited[roomId - 1] = 1 THEN
            IF monsterAlive[roomId - 1] = 1 THEN
                PRINT "  It is still here. It has turned to face the door this time."
            ELSE
                PRINT "  The room is quieter now. The body is where you left it. The gouges"
                PRINT "  in the floor catch the torchlight the same way they did before."
                PRINT ""
                PRINT "  You have been through here already. Whatever there is to find,"
                PRINT "  you know where it is."
            END IF
        ELSE
            REM  ... existing first-visit PRINT lines unchanged ...
        END IF


REM === REPLACE: SUB PrintRoom(), ROOM_COLLAPSED revisit branch ===
REM  Find: IF visited[roomId - 1] = 1 THEN (inside roomId = ROOM_COLLAPSED)
REM  Replace -- includes both dead variants (searched / unsearched).

        IF visited[roomId - 1] = 1 THEN
            IF monsterAlive[roomId - 1] = 1 THEN
                PRINT "  The skittering starts again the moment you step through the doorway."
                PRINT ""
                PRINT "  The rubble at the far end sits undisturbed. Whatever is buried under it,"
                PRINT "  you haven't looked yet."
            ELSE
                IF searched[roomId - 1] = 1 THEN
                    PRINT "  The passage is still. The body of the creature lies where it fell."
                    PRINT ""
                    PRINT "  The gap in the rubble is visible from the doorway now that you know"
                    PRINT "  where to look. South, through the collapse, if you're ready to move on."
                ELSE
                    PRINT "  The passage is still. The body of the creature lies where it fell,"
                    PRINT "  already desiccating in the cold air."
                    PRINT ""
                    PRINT "  The rubble at the far end sits as it has for decades. If there is a"
                    PRINT "  way through, it is not obvious. Not yet."
                END IF
            END IF
        ELSE
            REM  ... existing first-visit PRINT lines unchanged ...
        END IF


REM === REPLACE: SUB PrintRoom(), ROOM_PIT revisit branch ===

        IF visited[roomId - 1] = 1 THEN
            IF monsterAlive[roomId - 1] = 1 THEN
                PRINT "  It is still there. Still waiting. It turns to face you again with the"
                PRINT "  same unhurried certainty."
            ELSE
                PRINT "  The armoured figure is down. The pit sits in the centre of the room,"
                PRINT "  dropping into the same darkness it always did."
                PRINT ""
                PRINT "  South is clear."
            END IF
        ELSE
            REM  ... existing first-visit PRINT lines unchanged ...
        END IF


REM === REPLACE: SUB PrintRoom(), ROOM_CISTERN revisit branch ===

        IF visited[roomId - 1] = 1 THEN
            IF monsterAlive[roomId - 1] = 1 THEN
                PRINT "  The cold finds you again the moment you step through the doorway. The"
                PRINT "  luminescence flickers at the edge of vision."
                PRINT ""
                PRINT "  It remembers you too."
            ELSE
                PRINT "  The room is warmer without it. The water sounds are clearer now --"
                PRINT "  just water, just stone, just the dungeon's old plumbing doing what"
                PRINT "  it was built to do."
            END IF
        ELSE
            REM  ... existing first-visit PRINT lines unchanged ...
        END IF


REM === REPLACE: SUB PrintRoom(), ROOM_UNDERHALL revisit branch ===

        IF visited[roomId - 1] = 1 THEN
            IF monsterAlive[roomId - 1] = 1 THEN
                PRINT "  It remembers you. The small dark eyes find you immediately."
            ELSE
                PRINT "  The Troll is down. The room is the same -- the bones along the"
                PRINT "  walls, the high ceiling, the smell that does not quite go away."
                PRINT ""
                PRINT "  South is open. You are nearly through."
            END IF
        ELSE
            REM  ... existing first-visit PRINT lines unchanged ...
        END IF


REM === REPLACE: SUB EnterRoom() -- add postFight parameter and aftermath text ===

SUB EnterRoom(roomId AS INTEGER, postFight AS INTEGER)
    CALL PrintHeader()
    PRINT "  LOCATION: " & RoomName(roomId)
    PRINT ""
    IF postFight = 1 THEN
        IF roomId = ROOM_GUARDROOM THEN
            PRINT "  The Brute is down. It fell the way big things fall -- all at once,"
            PRINT "  without grace. The impact is still ringing in the floor."
            PRINT ""
            PRINT "  Your breathing is the loudest thing in the room now. The gouges in"
            PRINT "  the stone are at your feet. The dark stain has company."
        END IF
        IF roomId = ROOM_COLLAPSED THEN
            PRINT "  The skittering has stopped."
            PRINT ""
            PRINT "  The creature lies where it fell, legs folded at angles they were not"
            PRINT "  built to reach. The cold passage is quiet. The rubble at the far end"
            PRINT "  is exactly as it was -- whatever the thing was guarding, it is no"
            PRINT "  longer guarding it."
        END IF
        IF roomId = ROOM_PIT THEN
            PRINT "  The armoured figure is down at last. The old armour settles with a"
            PRINT "  sound like a door closing for good."
            PRINT ""
            PRINT "  The pit is behind you, dropping into the same darkness it always did."
            PRINT "  South is open."
        END IF
        IF roomId = ROOM_CISTERN THEN
            PRINT "  The cold leaves all at once -- not gradually, just gone, the moment"
            PRINT "  it stopped moving."
            PRINT ""
            PRINT "  The water sounds come back. Channels, stone, the dungeon's old"
            PRINT "  plumbing tending to itself. The recess in the north wall is still there."
        END IF
        IF roomId = ROOM_UNDERHALL THEN
            PRINT "  The Troll is down. Something that size takes a moment to accept. The"
            PRINT "  floor still feels the weight of it."
            PRINT ""
            PRINT "  The room is the same -- the bones along the walls, the high ceiling,"
            PRINT "  the smell that does not quite go away. But south is open now."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    ELSE
        CALL PrintRoom(roomId)
    END IF
    PRINT ""
    CALL PrintSeparator()
    LET visited[roomId - 1] = 1
    IF monsterAlive[roomId - 1] = 1 THEN
        PRINT ""
        PRINT "  FIGHT to engage, or SNEAK to try to pass."
        PRINT ""
    END IF
END SUB


REM === NEW FUNCTION: add after SUB EnterRoom() ===

FUNCTION AttackStrength(statSkill AS INTEGER) AS INTEGER
    RETURN RollDice(2) + statSkill
END FUNCTION


REM === NEW SUB: add after FUNCTION AttackStrength() ===

SUB CombatLoop(monsterSkill AS INTEGER, monsterStamina AS INTEGER,
               hasArmour AS INTEGER, hasRegen AS INTEGER,
               hasLuckDrain AS INTEGER, hasLesserTerror AS INTEGER,
               hasPoison AS INTEGER, isBoss AS INTEGER,
               searchInterrupt AS INTEGER)
    LET round = 1
    LET firstRound = 1
    LET terrorActive = 0
    LET startMonsterStamina = monsterStamina
    LET lesserSearchPenalty = 0
    LET penaltyApplied = 0
    LET playerLowFired = 0
    LET playerCriticalFired = 0
    LET monsterLowFired = 0
    LET monsterCriticalFired = 0
    LET playerAttack = 0
    LET monsterAttack = 0
    LET baseDamage = 0
    LET poisonRoll = 0

    REM  Full terror -- Bound King only.
    REM  Bangle of Courage check added in Issue 9 when inventory is available.
    IF isBoss = 1 THEN
        SET GLOBAL skill = skill - 2
        SET GLOBAL luck = luck - 1
        LET terrorActive = 1
    END IF

    WHILE gameOver = 0 AND monsterStamina > 0

        IF hasLuckDrain = 1 THEN
            IF luck > 0 THEN
                SET GLOBAL luck = luck - 1
            END IF
        END IF

        REM  Monster regen fires before attack rolls (Troll).
        REM  Capped at starting STAMINA -- cannot heal above where it started.
        IF hasRegen = 1 THEN
            LET monsterStamina = MIN(monsterStamina + 1, startMonsterStamina)
        END IF

        IF firstRound = 1 THEN
            LET lesserSearchPenalty = 0
            IF hasLesserTerror = 1 THEN
                LET lesserSearchPenalty = lesserSearchPenalty + 1
            END IF
            IF searchInterrupt = 1 THEN
                LET lesserSearchPenalty = lesserSearchPenalty + 1
            END IF
            IF lesserSearchPenalty > 0 THEN
                SET GLOBAL skill = skill - lesserSearchPenalty
                LET penaltyApplied = 1
            END IF
        END IF

        LET playerAttack = AttackStrength(skill)
        LET monsterAttack = AttackStrength(monsterSkill)
        PRINT ""
        PRINT "  Round " & round & ":   Your attack " & playerAttack & "   --   Monster attack " & monsterAttack
        PRINT ""

        SLEEP(COMBAT_DELAY)

        IF playerAttack > monsterAttack THEN
            IF hasArmour = 1 THEN
                LET monsterStamina = monsterStamina - 1
                PRINT "  You strike but the armour absorbs most of it.  Monster STAMINA: " & monsterStamina
            ELSE
                LET monsterStamina = monsterStamina - 2
                PRINT "  You hit.  Monster STAMINA: " & monsterStamina
            END IF
            IF monsterStamina <= 4 AND monsterLowFired = 0 THEN
                PRINT ""
                PRINT "  You can feel the momentum shifting. It is yours to lose now."
                LET monsterLowFired = 1
            END IF
            IF monsterStamina <= 2 AND monsterCriticalFired = 0 THEN
                PRINT ""
                PRINT "  It is nearly done. Don't give it a way back."
                LET monsterCriticalFired = 1
            END IF
        ELSE
            IF monsterAttack > playerAttack THEN
                LET baseDamage = 2
                REM  Overburdened damage bonus (baseDamage = 3) added in Issue 6.
                IF isBoss = 1 THEN
                    IF monsterAttack - playerAttack >= 3 THEN
                        LET baseDamage = 4
                        PRINT "  The blow lands with the weight of three hundred years of experience."
                        PRINT "  It takes you to one knee and you feel something break inside."
                    END IF
                END IF
                SET GLOBAL stamina = stamina - baseDamage
                SET GLOBAL minStamina = MIN(minStamina, stamina)
                PRINT "  It hits.  Your STAMINA: " & stamina
                IF hasPoison = 1 THEN
                    LET poisonRoll = RollDice(1)
                    IF poisonRoll >= 4 THEN
                        SET GLOBAL poisoned = 1
                    END IF
                END IF
                IF stamina <= 4 AND playerLowFired = 0 THEN
                    PRINT ""
                    PRINT "  You are barely standing. So is your chance of walking out of here."
                    LET playerLowFired = 1
                END IF
                IF stamina <= 2 AND playerCriticalFired = 0 THEN
                    PRINT ""
                    PRINT "  You are one blow from the end. You both know it."
                    LET playerCriticalFired = 1
                END IF
                IF stamina <= 0 THEN
                    SET GLOBAL gameOver = 1
                    SET GLOBAL endState = 5
                END IF
            ELSE
                PRINT "  Neither blow lands cleanly. You circle."
            END IF
        END IF

        SLEEP(COMBAT_DELAY)

        REM  Restore round-1 SKILL penalty at the end of round 1.
        IF penaltyApplied = 1 THEN
            SET GLOBAL skill = skill + lesserSearchPenalty
            LET penaltyApplied = 0
        END IF

        SET GLOBAL turns = turns + 1
        LET round = round + 1
        LET firstRound = 0

    WEND

    IF terrorActive = 1 THEN
        SET GLOBAL skill = skill + 2
        SET GLOBAL luck = luck + 1
    END IF
END SUB


REM === NEW SUB: add after SUB CombatLoop() ===

SUB HandleFight(roomId AS INTEGER)
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
        CALL CombatLoop(mSkill, mStamina, 0, 0, 0, 0, 0, 0, 0)
        IF gameOver = 0 THEN
            LET monsterAlive[roomId - 1] = 0
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
        CALL CombatLoop(mSkill, mStamina, 0, 0, 0, 1, 1, 0, 0)
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
        CALL CombatLoop(mSkill, mStamina, 1, 0, 0, 0, 0, 0, 0)
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
        CALL CombatLoop(mSkill, mStamina, 0, 0, 1, 0, 0, 0, 0)
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
        CALL CombatLoop(mSkill, mStamina, 0, 1, 0, 0, 0, 0, 0)
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


REM === NEW SUB: add after SUB HandleFight() ===

SUB PrintDeathScreen()
    PRINT ""
    CALL PrintSeparator()
    PRINT ""
    PRINT "  Your STAMINA reached zero."
    PRINT ""
    PRINT "  Your SKILL was " & startSkill & ". Your STAMINA reached as low as " & minStamina & "."
    PRINT "  You tested your LUCK " & luckTestCount & " times."
    PRINT ""
END SUB


REM === REPLACE: main program block (bottom of file) ===
REM  Replace everything from CALL InitExits() to the end of the file.

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
    NEXT i
    CALL InitMonsters()
    CALL OpeningSequence()
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
            CASE "CHEAT"
                REM  DEV TOOL -- strip before release (Issue 10)
                LET skill = 12
                LET stamina = 24
                PRINT "  [CHEAT] SKILL: " & skill & "  STAMINA: " & stamina
                PRINT ""
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
