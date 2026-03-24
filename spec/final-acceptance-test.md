# The Sunken Crown — Final Acceptance Test Plan

> Produced before build begins. Executed after Issue 10 is complete and signed off.
> Purpose: verify that the complete game works correctly as an integrated system,
> covering interactions between systems that individual issue tests cannot reach.
> All tests are manual — run the game and observe the output.
>
> **Pass criteria:** every test below produces the expected result.
> **Fail criteria:** any unexpected output, crash, or silent wrong behaviour.
> A fail at any point stops the test session. Fix the issue, restart from the
> beginning of the affected section.

---

## How to Run

```
sharpbasic run the-sunken-crown.sbx
```

Run from the `samples/the-sunken-crown/` directory. Each test session starts
from a fresh run unless stated otherwise.

---

## Section 1 — Startup and Attribute Rolling

**Test 1.1 — Opening sequence runs**
Start the game. Confirm the narrative opening text appears. Press ENTER at each
prompt. Confirm the game does not hang or crash.

**Test 1.2 — Attributes roll in valid ranges**
Complete the opening sequence. Note SKILL, STAMINA, and LUCK values.
- SKILL must be between 7 and 12 inclusive
- STAMINA must be between 14 and 24 inclusive
- LUCK must be between 7 and 12 inclusive

Run three times. Confirm values differ across runs. Confirm they stay within range.

**Test 1.3 — Header displays correct stats**
After rolling, confirm the header bar shows the correct SKILL, STAMINA, LUCK values.
Change nothing. Confirm the header updates correctly after any stat change.

**Test 1.4 — Play again resets all state**
Play through to any death. At the play again prompt, type YES. Confirm a fresh
opening sequence runs. Confirm new attributes are rolled. Confirm visited flags
are cleared — the Entry Hall should show first-visit text, not revisit text.

---

## Section 2 — Navigation

**Test 2.1 — All exits work**
Navigate the full dungeon using only GO commands. Visit every room. Confirm:
- Room descriptions appear on first entry
- Revisit descriptions appear on return
- LOCATION line shows correct room name
- Exits list is accurate for each room

**Test 2.2 — No exit that way**
In a room with limited exits, attempt a direction that does not exist.
Confirm the correct response prints and the player remains in the same room.

**Test 2.3 — Overburdened movement cost**
Carry 4 items. Navigate one room. Confirm STAMINA decreases by 1.
Drop one item. Navigate one room. Confirm STAMINA does not decrease.

**Test 2.4 — Room 5 — Still Chamber traps on entry**
Navigate to room 5 via GO NE from the Crossroads. Confirm the Still Chamber
sequence fires immediately. Confirm 3 turns advance. Confirm a luck test fires.
Confirm the player wakes in a different room. Confirm LUCK has decremented by 1.

**Test 2.5 — Room 5 revisit still traps**
Navigate to room 5 a second time. Confirm the revisit text appears but the
trap still fires. Confirm the player is teleported again.

**Test 2.6 — Poison on movement**
Fight the Skittering Horror in room 6 and become poisoned (lose a round, trigger
the 4+ d6 check). Navigate to the next room. Confirm the poison message prints
before the room description. Confirm STAMINA decreases by 1. Navigate another
room. Confirm it fires again. Use the Antidote Vial. Navigate again. Confirm
poison message no longer fires.

---

## Section 3 — Combat

**Test 3.1 — Basic combat loop**
Fight the Guardroom Brute. Confirm:
- Attack strengths print each round
- STAMINA decrements correctly on losing rounds
- The Brute's STAMINA decrements on winning rounds
- Combat ends when either side reaches zero
- Correct outcome text prints (death or victory)

**Test 3.2 — Overburdened combat penalty**
Carry 4 items. Fight any monster. Confirm damage taken on losing rounds is 3
not 2.

**Test 3.3 — Armoured monster — Pit Guardian**
Fight the Pit Guardian without the Sword of Sharpness. Confirm damage dealt
to the Guardian is 1 per winning round. Fight again with the Sword in inventory.
Confirm damage dealt is 2 per winning round.

**Test 3.4 — Lesser Terror — Skittering Horror**
Fight the Skittering Horror. Confirm SKILL is reduced by 1 on round 1 only.
Confirm SKILL returns to normal from round 2 onward.

**Test 3.5 — Poison trigger — Skittering Horror**
Fight the Skittering Horror. Lose several rounds. Confirm that on some losing
rounds the poison message fires (roughly 50% chance on 4+). Note: this is
probabilistic — run multiple times if needed to observe both outcomes.

**Test 3.6 — Luck drain — Hollow Mage**
Fight the Hollow Mage. Confirm LUCK decrements by 1 at the start of each round
regardless of who wins. Fight for at least 4 rounds. Confirm cumulative drain.

**Test 3.7 — Hollow Mage luck drain at zero**
Enter combat with the Hollow Mage with LUCK at 0 (use the LUCK command
repeatedly before the fight to drain it). Confirm the existential dread message
prints once and luck does not go below 0.

**Test 3.8 — Troll regeneration**
Fight the Troll. Confirm the Troll restores 1 STAMINA at the start of each round.
Confirm the Troll's STAMINA cannot exceed its starting value.

**Test 3.9 — Wandering zombie combat**
Wait for the zombie to spawn and enter the player's room. Type FIGHT. Confirm
standard combat with no special mechanics. Confirm zombie death sets zombieAlive
to 0 and it does not reappear.

**Test 3.10 — Combat tension lines — player**
Allow STAMINA to drop to 4 or below in combat. Confirm the ≤4 line fires once.
Allow STAMINA to drop to 2 or below. Confirm the ≤2 line fires once.
Confirm neither line fires a second time in the same fight even if STAMINA
fluctuates around the threshold.

**Test 3.11 — Combat tension lines — monster**
Reduce a monster to 4 or below STAMINA. Confirm the ≤4 rallying line fires once.
Reduce the monster to 2 or below. Confirm the ≤2 line fires once.

**Test 3.12 — Crushing Blow — Bound King**
Fight the Bound King. When he wins a round by 3 or more attack strength,
confirm the Crushing Blow flavour line prints and damage is 4 not 2.
Confirm the Crushing Blow line does not fire against other monsters.

**Test 3.13 — General low STAMINA outside combat**
Reduce STAMINA to 4 or below via poison, atmosphere, or overburdened movement.
Navigate to a new room. Confirm the general low STAMINA line fires before the
room description. Navigate again. Confirm it fires again on the next entry.

**Test 3.14 — Player death**
Allow STAMINA to reach zero in combat. Confirm the death sequence fires.
Confirm the end screen shows correct stats. Confirm play again prompt appears.

**Test 3.15 — SEARCH interrupt**
Enter room 2 with the Brute alive. Type SEARCH. Confirm the Brute interrupt
text prints. Confirm combat triggers immediately. Confirm SKILL -1 on round 1.
Confirm SKILL returns to normal from round 2.

**Test 3.16 — SEARCH interrupt additive with Lesser Terror**
Enter room 6 with the Skittering Horror alive. Type SEARCH. Confirm the Horror
interrupt text fires. Confirm SKILL -2 on round 1 (SEARCH -1 plus Lesser
Terror -1). Confirm SKILL returns to normal from round 2.

---

## Section 4 — Inventory

**Test 4.1 — TAKE and INVENTORY**
Pick up an item. Type INVENTORY. Confirm the item appears in the correct slot.
Confirm invCount is 1.

**Test 4.2 — Carry limit**
Pick up 4 items. Confirm overburdened message fires on the fourth item.
Attempt to pick up a fifth item. Confirm the cannot carry response prints.
Confirm invCount remains 4.

**Test 4.3 — DROP and re-TAKE**
Drop an item. Confirm invCount decrements to 3. Confirm overburdened clears.
Navigate to another room and back. Confirm the item is still in the room.
Pick it up again. Confirm invCount returns to 4.

**Test 4.4 — Item persistence across rooms**
Drop an item in room 2. Navigate to room 4 and back. Confirm the item is
still in room 2 on return. Confirm it appears in the room frame on entry.

**Test 4.5 — USE Healing Potion**
Take damage in combat. Use the Healing Potion. Confirm STAMINA increases.
Confirm STAMINA does not exceed startStamina. Confirm the potion is removed
from inventory.

**Test 4.6 — USE Antidote Vial**
Become poisoned. Use the Antidote Vial. Navigate a room. Confirm poison
message does not fire.

**Test 4.7 — USE Bangle of Courage**
Carry the Bangle. Enter room 11. Confirm Terror does not fire — SKILL and
LUCK should be unchanged before combat begins.

**Test 4.8 — Sword of Sharpness — universal +1 damage**
Carry the Sword. Fight any standard monster. Confirm 3 damage per winning round
(not 2). Drop the Sword mid-combat if possible, or restart. Confirm 2 damage
per winning round without it.
Fight the Pit Guardian with the Sword. Confirm 2 damage per winning round
(armour reduction applied first, then +1 sword bonus).
Fight the Pit Guardian without the Sword. Confirm 1 damage per winning round.

**Test 4.9 — Medal of Valour penalty**
Pick up the Medal of Valour. Confirm SKILL decrements by 1 immediately.
Confirm this only happens once — SKILL does not decrement again on a
subsequent pickup in the same run.

**Test 4.10 — Guardroom Key unlocks Armoury**
Kill the Brute. SEARCH room 2. TAKE the key. Navigate to room 3. USE the key.
SEARCH room 3. Confirm the Sword of Sharpness appears.

---

## Section 5 — SEARCH

**Test 5.1 — SEARCH reveals loot**
SEARCH a room with a loot slot. Confirm the item appears. TAKE it. SEARCH again.
Confirm the already-searched response prints.

**Test 5.2 — SEARCH reveals room items on entry**
Drop an item in a room. Leave. Return. Confirm the item is visible in the room
frame on entry without needing to SEARCH.

**Test 5.3 — SEARCH while monster alive triggers combat**
Enter room 2 with the Brute alive. Type SEARCH. Confirm interrupt text and
immediate combat. Confirm no search results are shown.

**Test 5.4 — SEARCH in room 11 refused**
Enter room 11 with the Bound King alive. Type SEARCH. Confirm the refused
response prints. Confirm combat does not trigger. Confirm the player can
still GO NORTH.

**Test 5.5 — Hidden passage revealed**
Navigate to room 6. Kill the Skittering Horror. SEARCH. Confirm the south
passage to room 9 is revealed. Confirm it now appears in the exits list.
Confirm GO SOUTH navigates to room 9.

---

## Section 6 — Atmosphere and Zombie

**Test 6.1 — Atmospheric events fire**
Navigate between rooms for 20+ turns without fighting. Confirm at least one
atmospheric event fires. Confirm events feel irregular — not every turn, not
never.

**Test 6.2 — Zombie spawns**
Play slowly — take many turns in the early rooms. After turn 3, the zombie
may spawn. Confirm it eventually appears. Confirm it wanders — its room
changes turn by turn.

**Test 6.3 — Zombie encounter**
Allow the zombie to enter the player's room. Confirm the encounter prompt
appears. Confirm FIGHT and SNEAK both work.

**Test 6.4 — Zombie does not enter excluded rooms**
If possible, observe zombie movement over many turns. Confirm it never
appears in rooms 5, 8, or 11.

**Test 6.5 — The Presence event**
With the zombie alive and nearby (within 2 rooms), wait for atmospheric
event 12 to fire. Confirm the directional version of the text appears
rather than the vague version.

---

## Section 7 — Trap Rooms

**Test 7.1 — Riddle Room — correct answer**
Navigate to room 8. Read the riddle. Answer correctly (LEFT or RIGHT as
appropriate). Confirm the door opens. Confirm south exit to room 10 is available.

**Test 7.2 — Riddle Room — wrong answer**
Navigate to room 8. Answer incorrectly. Confirm the death sequence fires.
Confirm endState = 3 on the end screen.

**Test 7.3 — Riddle Room — invalid input held**
Navigate to room 8. Type something other than LEFT or RIGHT. Confirm the
holding response prints. Confirm the loop continues. Confirm the player
cannot navigate away.

**Test 7.4 — Riddle Room — already solved pass-through**
Solve the riddle. Navigate away. Return to room 8. Confirm the room is
entered normally with the revisit description. Confirm no inner loop fires.

**Test 7.5 — Still Chamber — lucky outcome**
Navigate to room 5. If outcome is lucky, confirm player wakes in room 1
or room 3. Confirm LUCK has decremented.

**Test 7.6 — Still Chamber — unlucky outcome**
Navigate to room 5. If outcome is unlucky, confirm player wakes in one
of rooms 6, 7, 8, 9, or 10. If room 8, confirm riddleSolved state is
respected — if already solved, pass-through; if not, riddle fires.

---

## Section 8 — Bound King and Throne Room

**Test 8.1 — Terror fires without Bangle**
Enter room 11 without the Bangle. Confirm Terror text prints. Confirm
SKILL reduced by 2 and LUCK reduced by 1 before combat. Confirm both
restored after combat ends.

**Test 8.2 — Terror suppressed with Bangle**
Carry the Bangle. Enter room 11. Confirm Terror text does not print.
Confirm SKILL and LUCK unchanged before combat.

**Test 8.3 — Crushing Blow**
Fight the Bound King. When the King wins a round by 3 or more attack
strength, confirm damage is 4 not 2. Run multiple combat sessions to
observe both normal damage and Crushing Blow.

**Test 8.4 — SNEAK refused**
Enter room 11 with King alive. Type SNEAK. Confirm refusal text. Confirm
no combat triggered. Confirm GO NORTH still works.

**Test 8.5 — SEARCH refused**
Enter room 11 with King alive. Type SEARCH. Confirm refusal text. Confirm
no combat triggered.

**Test 8.6 — Post-combat beat**
Defeat the Bound King. Confirm post-combat beat text prints.

**Test 8.7 — First gold bag free**
After defeating the King, take one gold bag. Confirm no regeneration roll
fires. Confirm goldBags = 1.

**Test 8.8 — Regeneration rolls tighten**
Take bags 2, 3, and 4. Confirm regeneration rolls fire for each. Run
multiple times to observe both regeneration triggers and non-triggers.
Confirm threshold tightens — bag 4 triggers roughly 58% of the time.

**Test 8.9 — Second fight on regeneration**
Trigger regeneration. Confirm the second fight text prints. Confirm the
King's STAMINA is freshly rolled. Confirm the player's STAMINA is not
restored. Fight to completion. Confirm no further gold collection after
the second fight.

**Test 8.10 — Crown revealed by SEARCH**
After defeating the King, SEARCH room 11. Confirm the crown appears in
search results alongside any remaining gold bags.

**Test 8.11 — TAKE CROWN triggers curse sequence**
After SEARCH reveals the crown, type TAKE CROWN. Confirm the full crown
sequence prints. Confirm the run ends with endState = 2. Confirm no
inventory slot is consumed.

**Test 8.12 — TAKE CROWN with full inventory**
Fill inventory to 4 items. SEARCH room 11. Type TAKE CROWN. Confirm the
curse sequence fires regardless of carry state.

---

## Section 9 — The Gate

**Test 9.1 — Gate renders correctly**
Navigate to room 12. Confirm the Gate description renders. Confirm only
LEFT and RIGHT are accepted as commands.

**Test 9.2 — Correct door — win sequence**
Determine the correct door (play multiple runs or check gateCorrectDoor).
Choose the correct door. Confirm the full win sequence prints. Confirm
endState = 1. Confirm end screen shows correct gold bag count from
inventory, not from goldBags counter.

**Test 9.3 — Wrong door — death sequence**
Choose the wrong door. Confirm the death sequence prints. Confirm
endState = 4.

**Test 9.4 — Correct door randomises per run**
Play three runs through to the Gate. Note which door is correct each time.
Confirm it is not the same door every run.

**Test 9.5 — Gold count reflects inventory not goldBags**
Take 3 gold bags in the Throne Room. Drop 1 bag before reaching the Gate.
Win via the correct door. Confirm end screen shows 2 bags, not 3.

**Test 9.6 — Invalid input held at Gate**
At the Gate, type something other than LEFT or RIGHT. Confirm the dungeon
does not respond message prints. Confirm the loop continues.

---

## Section 10 — End Screen and Play Again

**Test 10.1 — End screen shows correct stats**
Complete a run. Confirm end screen shows:
- Correct gold bag count (from inventory at Gate)
- Correct starting SKILL (startSkill, not current skill)
- Correct minimum STAMINA reached
- Correct LUCK test count
- Correct turn count

**Test 10.2 — All end states produce correct end screen**
Verify the end screen message for each endState:
- 1 = win via Gate
- 2 = took the crown
- 3 = wrong answer in Riddle Room
- 4 = wrong door at Gate
- 5 = STAMINA reached zero

**Test 10.3 — Play again full reset**
After any run ending, type YES at the play again prompt. Confirm:
- New attributes are rolled
- All visited flags cleared (Entry Hall shows first-visit text)
- All monsters alive
- Inventory empty
- Loot reshuffled (different items in different rooms)
- Riddle rerandomised
- Gate correct door rerandomised
- Zombie not spawned
- Poison cleared
- Turn counter reset

**Test 10.4 — Play again NO exits cleanly**
At the play again prompt, type NO. Confirm the program exits cleanly.

---

## Section 11 — Cross-System Integration

These tests specifically target interactions between systems that individual
issue tests cannot reach. They should only be run once the full game is complete.

**Test 11.1 — Still Chamber lands on dead monster room**
Arrange for a monster to be dead (e.g. kill the Skittering Horror in room 6).
Navigate to room 5 repeatedly until the Still Chamber places you in room 6.
Confirm the room is entered normally with the monster-dead description —
no encounter prompt, no combat.

**Test 11.2 — Still Chamber lands on Riddle Room already solved**
Solve the Riddle Room. Then navigate to room 5. If the Still Chamber places
you in room 8, confirm you pass through normally rather than facing the
riddle again.

**Test 11.3 — Still Chamber lands on Riddle Room unsolved**
Navigate to room 5 before solving the Riddle Room. If the Still Chamber
places you in room 8, confirm the riddle fires.

**Test 11.4 — Overburdened through the Bound King fight**
Enter room 11 carrying 4 items. Fight the King. Confirm overburdened
damage penalty (3 per losing round) applies during the boss fight.
Confirm it lifts if you drop an item during gold collection.

**Test 11.5 — Poison active through boss fight**
Become poisoned before reaching room 11. Enter the Throne Room. Confirm
poison fires before the room description. Fight the King while poisoned.
Confirm poison continues to fire on room entries after the fight.

**Test 11.6 — Zombie encounters player in Throne Room approach**
With the zombie alive, navigate to room 10. If the zombie is present,
type FIGHT. Confirm standard combat. Confirm the Troll encounter in the
same room still works correctly after killing the zombie.

**Test 11.7 — Loot in multiple rooms after DROP chain**
Pick up an item. Drop it. Pick up a different item. Drop it in a different
room. Navigate back to both rooms. Confirm both dropped items appear in
the correct rooms and are retrievable.

**Test 11.8 — Full run with minimum engagement**
Complete a full run as fast as possible — navigate directly to room 11,
fight the King, take one bag, go to the Gate, choose correctly. Confirm
everything works with minimal interaction. This tests the critical path.

**Test 11.9 — Full run with maximum engagement**
Complete a full run attempting to do everything — all rooms visited, all
monsters killed, all loot collected, riddle solved, all gold bags taken,
second fight survived. Confirm the game holds together under maximum
state complexity.

**Test 11.10 — LUCK exhaustion**
Drain LUCK to zero using the LUCK command. Enter combat with the Hollow
Mage. Confirm the luck drain at zero message fires. Confirm luck stays
at zero. Confirm the game does not crash or behave unexpectedly when
luck tests are called with luck at zero.

---

## Sign-Off

The game passes final acceptance when all tests in Sections 1 through 11
produce their expected results across a minimum of three independent full
playthroughs.

Document any failures with:
- The test number and description
- What was expected
- What actually happened
- The issue number most likely responsible

*Final acceptance test plan produced March 2026.*
*Execute after Issue 10 sign-off.*
