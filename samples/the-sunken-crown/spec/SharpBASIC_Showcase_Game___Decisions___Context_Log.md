# SharpBASIC Showcase Game — Decisions & Context Log

> Planning document for the SharpBASIC v1 showcase program.
> A Fighting Fantasy-inspired text adventure demonstrating the full v1 language.
> Update whenever a significant decision is made. Date every entry.

-----

## What This Project Is

A text adventure game written in SharpBASIC v1, designed to showcase the full
breadth of the language’s instruction set. Inspired by the Fighting Fantasy
gamebook series — specifically the “enter a lethal dungeon for prize money”
format of Deathtrap Dungeon.

This is not a clone. It is a new adventure in the same spirit, built specifically
to exercise SharpBASIC v1 features in a meaningful, playable program.

**Primary purpose:** Demonstrate that SharpBASIC v1 is a real, usable language
capable of expressing something genuinely fun.

**Secondary purpose:** A strong closing artefact for the SharpBASIC LinkedIn
series (Post 4 — retrospective and release).

-----

## Fiction & Setting

### Working Title

*The Sunken Crown*

### Premise

You have sold everything. Your home, your tools, what little remained after the
debts. Malachar’s men took it all — converted to coin, dropped down a chute into
the dark beneath the keep. You watched it go.

If you don’t walk out of that dungeon, there is nothing to walk back to.

The prize is not Malachar’s to give. It belongs to the dungeon — three hundred
years of other people’s last resorts, piled around something that cannot die and
cannot leave. You are going to take back what’s yours and as much else as you
can carry.

You step forward.

### The Ruler

Lord Malachar — last of a cursed bloodline. Does not run the trial for sport.
Discovered the dungeon two generations back. Did not create it. Simply controls
access to it and takes the worldly goods of everyone who enters — converted to
coin and dropped into the Throne Room before the gates close. He keeps the gold
of those who don’t return. He does not fully understand what the Bound King is.
He knows it cannot be killed. He knows desperate people will try. He has built
a quiet, grim business around both facts.

The chute that drops contestants’ gold into the Throne Room was built by
Malachar’s grandfather using forced labour — peasants who did not survive the
construction. Malachar’s men skim a portion of every entry fee before it drops.
The Bound King receives what remains. He has no use for any of it.

The cursed bloodline is not metaphor. The Bound King’s curse radiates outward
through the stone — ambient, ancient, uncontrolled. Malachar’s family has lived
above it for two generations. It has twisted them slowly: compelled toward the
dungeon, drawn to bring contestants, unable to stop what they began. The Bound
King does not direct this. He is not aware of it. But the curse serves his
freedom whether he wills it or not. Every contestant is the dungeon working
toward its own end.

### The Backstory — Authorial Canon

This is the truth of the dungeon. How much the player sees is a separate decision.
All of it informs the writing whether it is stated or not.

The dungeon predates Lord Malachar by centuries. It was built beneath the keep by
a king whose name has been deliberately erased — referred to here as the First King.
The First King built the dungeon as a vault. What he stored there is not known.
What matters is that it was his, and it was protected, and he understood what he
was protecting.

The man who would become the Bound King was a general in the First King’s service.
Trusted. Decorated. Given access few others had. He used that access to steal the
First King’s crown — not for its material value, but for what it represented.
Legitimacy. The right to rule. He believed possession of the crown made him king.

The First King caught him in the act. Dying from wounds already taken in a battle
that day — wounds the general knew about, wounds the general had timed his theft
around — the First King used his last coherent moments to speak a curse.

Not a soldier’s curse. Not anger. Something older and more deliberate.

*Bound to this place. Bound to that crown. Never dying. Feeling every wound, every
cut, every second of every year, for as long as the crown sits on your head and the
crown will never leave.*

The general became the Bound King in that moment. The crown fused. He could not
remove it. He could not leave the dungeon — the curse bound him to the place of
his crime. He could not die — the curse was explicit on this point, and thorough.

That was three hundred years ago.

Lord Malachar discovered the dungeon two generations back. He did not create the
trial — he inherited a dungeon that already had a monster in it and saw an
opportunity. The entrance fee is his invention. The Bound King’s presence is not.
Malachar does not fully understand what the Bound King is. He knows it cannot be
killed. He knows desperate people will try. He has built a revenue stream around
both facts.

The Sunken Crown is what people began calling the dungeon. The crown that sank
beneath the earth. The king who fell. Nobody uses the First King’s name anymore.
Nobody uses the general’s name. There is only the crown, and the thing wearing it,
and the dungeon that will not give either of them up.

### The Bound King — What He Is Now

He has had three hundred years to make peace with what he is and has not managed it.
He is not wise. He is not philosophical. He is furious in the way that only something
that cannot die and cannot leave and cannot stop feeling can be furious — a rage so
old and so constant it has become indistinguishable from the stone around him.

He does not speak to contestants. He stopped doing that long ago.
He does not taunt or threaten. He simply fights, because fighting is the only thing
left that marks the passage of time.

When you beat him he falls. He is already regenerating before you reach the door.
He has been beaten before. He will be beaten again. He will be here long after
Lord Malachar is dust and the keep has crumbled and whatever comes next has
forgotten what a crown even was.

The gold around his throne is not hoarded. It simply accumulates. He has no use
for it. It is the residue of every person who came to end him and didn’t make it
out. A monument nobody intended to build.

### Tone

Medieval fantasy where monsters are real and understood. Fear comes from
knowledge, not surprise. The player’s character knows exactly what a Troll is
and what it has done to others. That recognition is the source of dread.

Not horror. Not comedy. The register of the Fighting Fantasy books: matter-of-fact
about danger, respectful of the player’s intelligence.

### No Save Function

Deliberate. No file I/O in SharpBASIC v1, and that constraint becomes a feature.
One life. Roll your attributes and hope. Die and start again.

The opening text will acknowledge this directly in character:
*“Few who enter the Sunken Crown return. None speak of what waits inside. You will have one chance.”*

-----

## Core Mechanics

### Attributes (Fighting Fantasy-style)

|Attribute|Roll    |Notes               |
|---------|--------|--------------------|
|SKILL    |1d6 + 6 |Combat effectiveness|
|STAMINA  |2d6 + 12|Hit points          |
|LUCK     |1d6 + 6 |Degrades on use     |

Starting values stored separately from current values so healing caps at
starting STAMINA, not an arbitrary maximum.

### Dice

`FUNCTION RollDice(n AS INTEGER) AS INTEGER` — sums n d6 rolls via `FOR` loop.
Single source of truth for all randomness.

### Combat (per round)

1. Both sides roll attack strength: `RollDice(2) + SKILL`
2. Higher wins the round
3. Loser takes 2 STAMINA damage
4. Ties — no damage, roll again
5. Combat ends when either side reaches 0 STAMINA

Implemented as a `SUB CombatRound(...)` called from a `WHILE` loop.

### Test Your Luck

Classic FF mechanic. Roll 2d6. If <= current LUCK: lucky outcome. If >: unlucky.
Decrement LUCK by 1 regardless. Every test makes the next one harder.

Implemented as `FUNCTION TestLuck() AS INTEGER` — returns 1 (lucky) or 0 (unlucky).

### Terror Mechanic

Certain monsters cause Terror on sight. Without the Bangle of Courage:

- SKILL temporarily reduced by 2
- LUCK temporarily reduced by 1
- Penalty restored after the encounter ends
- Stored as variables so the restoration is explicit and auditable

The final boss triggers full Terror. Lesser encounters trigger a “first round only”
variant — you steady yourself after the initial shock. This teaches the mechanic
before it becomes lethal.

The Bangle of Courage negates all Terror effects. Finding it early has value
throughout the dungeon, not just at the final boss.

-----

## Randomisation Philosophy

**Hybrid model — fix the structure, randomise the texture.**

|Element            |Approach                |Rationale                                     |
|-------------------|------------------------|----------------------------------------------|
|Map layout         |Fixed                   |Deaths are informative — you learn the dungeon|
|Trap logic         |Fixed                   |Same reason                                   |
|Monster placement  |Fixed                   |Player learns what’s where                    |
|Monster stats      |Randomised within a band|Tension on repeat runs                        |
|Loot placement     |Shuffled from fixed pool|Exploration has value every run               |
|Starting attributes|Rolled as FF intended   |Biggest variance source                       |

Example monster stat band: Ogre SKILL = `RollDice(1) + 7` (range 8-13).
The Ogre is always in the same room. His strength varies.

The loot pool (all items guaranteed to exist in the dungeon) is shuffled across
available rooms at startup. Player cannot beeline for the healing potion — must
explore.

-----

## Feature Coverage Map

|SharpBASIC v1 Feature          |Where Used                                               |
|-------------------------------|---------------------------------------------------------|
|`DIM` arrays                   |Rooms, exits, inventory, loot pool, visited, searched    |
|`FOR` / `NEXT`                 |Dice rolling, inventory display, room exits, loot shuffle|
|`WHILE` / `WEND`               |Main game loop, combat loop                              |
|`FUNCTION` with typed return   |`RollDice`, `TestLuck`, `AttackStrength`, `RoomName`     |
|`SUB` with typed parameters    |`ShowStats`, `ShowInventory`, `CombatRound`, `ShowRoom`  |
|`INPUT`                        |Command prompt, “press ENTER” pacing beats               |
|`IF` / `ELSE` / `END IF`       |Combat outcomes, navigation, item checks, terror         |
|`AND` / `OR` / `NOT`           |Win/lose conditions, item possession checks              |
|`RND`, `INT`                   |All dice rolls, loot shuffle, atmospheric events         |
|`UPPER$`                       |Command normalisation                                    |
|`STR$`                         |Stat display, gold output                                |
|`LEN`                          |Input validation                                         |
|`LEFT$`, `MID$`                |Command parsing — verb extraction and argument splitting |
|String concatenation `&`       |Output formatting throughout                             |
|Typed parameters (`AS INTEGER`)|All function and sub signatures                          |

-----

## Map Design

**Rooms: 12.** Two routes from the Crossroads converging before the boss.

### Structure

Branching from [4] The Crossroads, with both routes mandatory through the boss.
No bypass. The exit is on the other side of the Throne Room — full stop.

Two routes diverge at [4]. The easy route goes west through [6] Collapsed Passage
and [9] The Cistern. The hard route goes east through [7] The Pit and [8] The Riddle
Room. Both converge at [10] The Underhall before the mandatory boss at [11].
Room [5] The Still Chamber branches northeast from [4] — a trap, not a route.
The authoritative map is in the Room Map section below.

### The Living Dungeon

The dungeon is alive with ancient magic. Asymmetric connections are the mechanical
expression of this — going north from a room does not always bring you south back
to where you came from. The dungeon reroutes itself.

One specific asymmetric connection is the centrepiece: go north from [4] to [5],
come back south from [5] and arrive at [2]. The visited-room text makes it land:
*“You have been here before. You should not be here again.”*

This is explained in the backstory — ancient magic, a living dungeon that traps
the unwary and careless. Players don’t have magic. They can only navigate it.

### Visited Room Tracking

`DIM visited(12) AS INTEGER` — each room tracks whether the player has been there.
Room descriptions vary on revisit. The dungeon acknowledges your history:

- Empty chests are noted as already looted
- Evidence of your earlier passage is visible
- The dungeon’s small acts of rearrangement reinforce the living dungeon feel

### Searched Room Tracking

`DIM searched(12) AS INTEGER` — each room tracks whether it has been searched.
The flag marks the room as exhausted — no further items will be found. However
SEARCH can be issued any number of times. Each attempt still costs 2 turns
regardless of outcome. The zombie still moves. Events still tick. The flavour
text acknowledges the wasted effort. The turn cost is the punishment.
The visited flag and searched flag are independent.

### Route characteristics

|Route|Path                                                   |Reward                 |Risk                      |
|-----|-------------------------------------------------------|-----------------------|--------------------------|
|Hard |Crossroads -> Pit -> Riddle Room -> Underhall          |Bangle of Courage      |Tougher encounters, riddle|
|Easy |Crossroads -> Collapsed Passage -> Cistern -> Underhall|Hidden cache in Cistern|Arrives without Bangle    |

Both routes have a chance to find a Healing Potion before the boss. This is
non-negotiable design — the chance must exist on both paths.

### Room Map

```
[1]  Entry Hall
      |
[2]  Guardroom --- [3] Armoury (dead end, locked chest)
      |
[4]  The Crossroads --NE-- [5] The Still Chamber
     /          \                (no exit -- teleports, asymmetric return -> [2])
[6]  Collapsed   [7] The Pit
     Passage *        |
      |           [8] The Riddle Room
      |                |
[9]  The Cistern       |
      \               /
      [10] The Underhall  <- both routes converge here
             |
      [11] Throne Room (Boss)
             |
      [12] The Gate (two doors -- freedom or the dungeon forever)
```

*[6] hidden south exit to [9] — only revealed on SEARCH

**Room [5] The Still Chamber** has no exits. Entry triggers the teleport mechanic
immediately — the player passes out and wakes elsewhere. There is no navigable
return path. The "asymmetric return to [2]" noted in earlier drafts was superseded
by the teleport pool mechanic.

### Exits Table

|Room|Name             |Exits                                           |
|----|-----------------|------------------------------------------------|
|[1] |Entry Hall       |S -> [2]                                        |
|[2] |Guardroom        |N -> [1], E -> [3], S -> [4]                    |
|[3] |Armoury          |W -> [2]                                        |
|[4] |The Crossroads   |N -> [2]†, W -> [6], E -> [7], NE -> [5]‡       |
|[5] |The Still Chamber|No exits — teleports on entry. Luck roll determines destination.|
|[6] |Collapsed Passage|E -> [4], S -> [9] hidden                       |
|[7] |The Pit          |W -> [4], S -> [8]                              |
|[8] |The Riddle Room  |N -> [7], correct door -> [10]                  |
|[9] |The Cistern      |N -> [6], S -> [10]                             |
|[10]|The Underhall    |N -> [8] or N -> [9]†, S -> [11]                |
|[11]|Throne Room      |N -> [10], S -> [12]                            |
|[12]|The Gate         |N -> [11], two doors — one exits, one traps     |

†Route-dependent — which north exit shows depends on how you arrived
‡[5] appears as a distinct NE exit labelled “a dark passage” — indistinguishable from a normal exit until entered

### Room types

- Entry (atmosphere, no enemy)
- Monster encounters
- Trap rooms — The Still Chamber [5], The Riddle Room [8]
- Loot rooms (shuffled pool)
- [9] The Cistern — secret route reward, fixed hidden cache, Hollow Mage guards it
- [10] The Underhall — the Troll. Unavoidable — both routes converge here. Physical remains of previous contestants line the walls — dry bones, old armour, clothing. Those who got this far but were too injured or poisoned to finish it. The bones are evidence of the Troll’s history. A thorough SEARCH after combat may yield something left behind. The only reward for this room is survival.
- [11] Throne Room — boss encounter and prize gold
- [12] The Gate — the final cruel twist

### Win condition

Reach [11], defeat the boss, claim the gold, reach [12], choose the correct door.
STAMINA > 0 throughout. The dungeon gets one last move.

-----

## The Gate — Room [12]

The final room. The dungeon’s last trick.

### What Happens

The player enters after defeating the Bound King, gold in hand. Two doors.
No label. No instruction. The dungeon offers no guidance.

One door leads outside. Freedom, gold, survival.
One door leads deeper — into corridors that twist and turn until the torch
dies and the dungeon claims you quietly.

### The Tell

The correct door looks newer, less worn. Fewer hands have touched it.
Most contestants never reached this room. The ones who did mostly chose wrong.
The worn door is evidence of hope. The newer door is evidence of how rare
survival truly is.

The tell is environmental and subtle. It is not signposted. A careful player
may notice it. Most will not on a first run. The death earns its place because
the information was always there.

### Death Text

> *You walk through the door into yet another damp stone-lined corridor.
> You walk on, exploring the ever more twisting path. Your torch finally
> stutters and dies, leaving you to roam the pitch black halls and corridors
> until you eventually succumb to starvation, or something that was already
> down here finds you first.*

### Win Text

*The door opens onto grey sky.*

*You don’t move at first. You stand in the doorway and breathe. Cold air. Real air. The smell of mud and grass and distance. Things that have nothing to do with stone and dark and death.*

*Your legs carry you forward because they have forgotten how to stop.*

*There is no crowd. There never is. The competition is not a spectacle — it is a disposal.*

*Lord Malachar is waiting in the courtyard. Six guards flank him. You notice, with the detached clarity of someone who has spent the last hours keeping themselves alive by noticing things, that any one of them could kill you where you stand. You have nothing left. Your hands are shaking. You hadn’t noticed until now.*

*He looks at you for a long moment. His expression does not change.*

*Then he steps forward. Alone. The guards don’t move.*

*“You survived.”*

*It is not a question. It is not a compliment. It is the pronouncement of something that was not supposed to happen, delivered by a man who has made his peace with occasionally being wrong.*

*His eyes drop to the gold in your hands. The gold that was never his to give — it belonged to the dungeon, and the dungeon let you take it.*

*He steps aside.*

*“Go.”*

*You go.*

*You are alive. You are free. You are one of almost none.*

-----

Followed by the end screen:

```
You survived The Sunken Crown.
You escaped with X bag(s) of gold.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

### Mechanic

- Correct door randomised at game startup
- Tell is consistent with the correct door each run — newer door is always correct
- No SKILL check, no LUCK test, no combat
- Wrong door = death, no recovery
- Player input: LEFT or RIGHT

-----

## Monster Roster

### Wandering Zombie *(already agreed)*

- Roams via exits array, no special pathfinding
- Standard combat, no special mechanic
- Never enters [5], [8], or [11] — unexplained, left to player inference
- Spawns after turn 3 on a probability check — not guaranteed on fast runs
- Spawns in room [4] The Crossroads
- Death is permanent

### Fixed Encounters

**Guardroom Brute — Room [2]**
Ogre-class. Big, slow, hits hard.

- `SKILL = RollDice(1) + 7`, `STAMINA = RollDice(2) + 10`
- Standard combat — the teaching encounter. Establishes the baseline.
- Carries a key to the Armoury locked chest [3]. Found on SEARCH after combat.

**Skittering Horror — Room [6] Collapsed Passage (easy route)**
Giant cave horror — fast, chitinous, heard before seen.

- `SKILL = RollDice(1) + 5`, `STAMINA = RollDice(1) + 4`
- **Lesser Terror:** Dark, scrabbling, a rush of wind. First round only — SKILL -1,
  restored after round one. Bangle of Courage negates entirely.
- **Poison:** On any round the Horror wins, roll 1d6. On 4+, player is poisoned —
  loses 1 STAMINA on each subsequent room entry until cured with Antidote Vial.
- No loot. Fast encounter, but the cost lingers.

**Pit Guardian — Room [7] The Pit (hard route)**
Undead warrior. Armoured, relentless.

- `SKILL = RollDice(1) + 8`, `STAMINA = RollDice(2) + 8`
- **Armoured:** Damage dealt to the Guardian reduced by 1 per round (minimum 1).
  Rewards having the Sword of Sharpness.
- Fixed drop: Bangle of Courage. Always carried. Found on SEARCH after combat.

**Hollow Mage — Room [9] The Cistern**
Spectral sorcerer. Faded, hungry. Guards the hidden cache.

- `SKILL = RollDice(1) + 6`, `STAMINA = RollDice(1) + 6`
- **Luck Drain:** Player loses 1 LUCK at the start of each combat round regardless
  of outcome. Winning quickly matters. Teaches LUCK management before the boss.
- Guards the Cistern cache — defeat or sneak to access it.

**Troll — Room [10] The Underhall (unavoidable — both routes converge here)**
Classic troll. Regenerating.

- `SKILL = RollDice(1) + 7`, `STAMINA = RollDice(2) + 12`
- **Regeneration:** Restores 1 STAMINA at the start of each combat round.
  Cannot regenerate above starting value. Rewards finishing fast.
- **Fixed drop:** Mouldy Bread. Found on SEARCH after combat. Restores 3 STAMINA.
  Grim but functional — it belonged to someone who didn’t make it out.

**The Bound King — Room [11] Throne Room (final boss)**
Ancient general, cursed by a dying king three hundred years ago. Stole the crown.
Cannot die. Cannot leave. Cannot remove the crown. Has been in this room ever since.
The Sunken Crown — the dungeon’s name — is his crown. It fused to him the moment
the curse landed. He does not speak. He simply fights, because it marks the time.

**Appearance:**
He looks human. That is what makes him wrong.

Three hundred years and he has not decayed. The curse that keeps him alive keeps
him whole. He looks like a man in his forties — the age he was when it happened.
Armoured, bearing the marks of every fight he has ever lost and recovered from.
Scars that healed. Wounds that closed. A body that has been destroyed and rebuilt
so many times it has forgotten how to show it.

The crown is the only thing that looks ancient. Tarnished where it touches his
skin. Fused at the temples in a way that stopped looking natural centuries ago.

He does not look ethereal. He does not look undead. He looks like a man who is
very, very tired and cannot stop.

That is the horror. Not what he is. What he still is.

- `SKILL = RollDice(1) + 9`, `STAMINA = RollDice(2) + 18`
- **Full Terror:** Without Bangle of Courage — SKILL -2, LUCK -1 for the full
  fight. Restored after combat ends. Bangle negates entirely.
- **Crushing Blow:** On any round the King wins by 3 or more attack strength,
  he deals 4 STAMINA damage instead of 2.
- **Cannot be killed:** When his STAMINA reaches 0 he falls but does not die.
  The curse holds. He is already regenerating.
- **Post-combat beat:** *The King falls. You don’t wait. You can already see his
  fingers moving.* The player takes the gold and runs. No ceremony. No victory lap.
  The exit is behind them and something is pulling itself back together on the floor.
- Cannot leave the Throne Room. Does not try. Has been there for three hundred years.
  The gold piled around him is from everyone who came before you.

### The Gold Mechanic

**How the gold got there:**
Before entering the dungeon every contestant hands over everything they own —
converted to coin by Malachar’s men and dropped down a chute into the Throne Room.
You watch it disappear into the dark at the start of the game. Three hundred years
of contestants. Three hundred years of last resorts, piled around a cursed man
who cannot spend any of it.

Your stake is in there. Indistinguishable from everyone else’s.
You are not collecting a prize. You are taking back what’s yours — plus whatever
else you can carry out of three hundred years of other people’s wreckage.

**Gold as discrete items:**
Bags of gold. TAKE BAG takes one. Maximum four — the standard hard inventory limit.
Each bag is a decision made while something is getting back up off the floor.

**TAKE ALL is refused in the Throne Room:**
The player cannot use TAKE ALL here. If they try:
*You don’t have time for that.*
This is the only room in the game where a standard command is explicitly refused
in context. It should feel different. It does.

**Regeneration rolls:**
After each bag taken, roll for regeneration. The threshold tightens with each bag.

|Bags taken|Roll       |Triggers on                     |
|----------|-----------|--------------------------------|
|1         |No roll    |Safe — take the first bag freely|
|2         |RollDice(2)|11+                             |
|3         |RollDice(2)|9+                              |
|4         |RollDice(2)|7+                              |

A 7+ on 2d6 is roughly 58%. Four bags is a genuine gamble. One bag is free.
Two is cautious. Three is brave. Four is either desperate or greedy depending
on how much STAMINA you have left.

**Regeneration beat — each bag after the first:**

*His fingers move.*

*His hand closes into a fist.*

*He draws a breath.*

Each line is a prompt. The player decides when to stop. The inventory system
enforces the consequences.

**If he fully regenerates:**
He stands. You are between him and the door with whatever you are carrying.
He does not speak. He simply gets up — the same fury, the same curse, the same
three-hundred-year weight. You fight again with whatever you have left.

His stats reroll fresh STAMINA, same SKILL. You do not.

If you beat him a second time you cannot take more gold. You run with what
you are carrying. Whatever that is, it is what you get.

**The end screen reflects your greed:**

The unified end screen shows gold bags taken alongside the run stats.
One bag means survival. Four bags means something more. The player sets
their own measure of success.

### The Crown

The crown is on the Bound King’s head when he falls. It stays there. The room
description after combat does not draw attention to it — it is simply part of
the fallen figure on the floor.

If the player SEARCHes the Throne Room, the search costs 2 turns and triggers
two regeneration rolls before the results are revealed. The crown appears in the
search results alongside any remaining bags of gold.

**The search result:**

*Beneath the piled coin and the fallen king you see it clearly for the first time.
The crown. Solid gold, heavily jewelled — rubies, sapphires, stones you don’t have
names for. Nothing else in this room comes close. Nothing you have ever owned comes
close. One item. Carried out of here and you never work another day. You never fear
another debt. You never need anything from anyone again.*

*It is still on his head. His head is right there.*

TAKE CROWN appears in the available actions alongside TAKE BAG. No warning.
No asterisk. The game treats it as a valid action. The player decides if it is.

The crown never enters the inventory. Reaching for it is enough. The curse
transfers on contact — no inventory slot required, no hard limit interaction,
no edge case with full bags. The moment the player issues TAKE CROWN the
sequence fires immediately regardless of carry state.

**If the player takes the crown:**

*You pick it up. You don’t mean to put it on. You do anyway.*

*It fits perfectly. It always did.*

*The Bound King opens his eyes.*

*He looks at you for a long moment. Then he looks at his own hands — empty,
unburdened, free for the first time in three hundred years.*

*He stands. He walks past you without a word. You hear his footsteps on the stairs.*

*You try to follow. Your feet will not move.*

*You are still there when the torch goes out.*

```
You are the Bound King now.

Play again? (YES / NO)
```

### Backstory Visibility — The Rule

The full canon lives in this design document. None of it reaches the player directly.

The player knows two things: the dungeon is called The Sunken Crown, and the Bound
King wears a crown. Everything else they bring themselves. The temptation of the
crown works precisely because the player does not know what it is. The name of the
dungeon is the only clue available. On a second run, knowing what happens, it lands
differently. That is the reward for reading carefully.

The authorial canon informs every line of writing without being stated in any of it.

### Mechanic-to-Feature Map

|Monster          |Mechanic              |SharpBASIC Feature                                 |
|-----------------|----------------------|---------------------------------------------------|
|Zombie           |Wandering             |`FOR` exits array, `RND` movement                  |
|Guardroom Brute  |Standard combat       |`SUB CombatRound` baseline                         |
|Skittering Horror|Poison + terror       |`IF poisoned THEN` on room entry, round-1 SKILL mod|
|Pit Guardian     |Armoured              |Modified damage calc in `CombatRound`              |
|Hollow Mage      |Luck drain            |`LET luck = luck - 1` inside combat loop           |
|Troll            |Regeneration          |STAMINA restore in combat loop                     |
|Bound King       |Terror + Crushing Blow|Conditional SKILL/LUCK modifier, damage threshold  |

-----

## The Still Chamber — Room [5]

A trap room with no visible exit. The dungeon’s most distinctive mechanic.

### What Happens

The player enters via a normal exit from [4] The Crossroads — nothing warns them.
The room description offers no exit door. Before they can act, dizziness takes hold.
They pass out. When they wake, nothing looks different — but they are somewhere else.

### The Mechanic

1. Player enters [5]
2. Flavour text — the room, the stillness, the dizziness, blackout
3. 3 turns pass (zombie moves, atmospheric events tick)
4. **Test Your Luck** on waking — LUCK decrements regardless of outcome
5. Lucky -> land in a room from the lucky pool (random)
6. Unlucky -> land in a room from the unlucky pool (random)
7. Waking text delivered for the destination room

### Exclusions

[5] itself, [11] Throne Room, and [12] The Gate are excluded from both pools.
Waking at the exit having bypassed the boss breaks the game.
Waking back in [5] would loop indefinitely.

The zombie never enters [5]. Its presence would muddy the room’s mechanic —
the Still Chamber belongs to the dungeon, not to wandering creatures.
Zombie exclusion list: [5], [8], and [11].

### Lucky Pool (2 rooms)

|Room          |Why Favourable                                                 |
|--------------|---------------------------------------------------------------|
|[3] Armoury   |Safe. Second chance at Sword of Sharpness if not yet collected.|
|[1] Entry Hall|Neutral and safe. No progress lost, no danger.                 |

### Unlucky Pool (5 rooms)

|Room                 |Why Punishing                                                                                                           |
|---------------------|------------------------------------------------------------------------------------------------------------------------|
|[6] Collapsed Passage|If Horror is alive, you wake up in the room with it.                                                                    |
|[7] The Pit          |If Guardian is alive, same problem.                                                                                     |
|[8] Riddle Room      |Trapped immediately — wrong answer means instant death. If already solved this run, doors are open and you pass through.|
|[9] The Cistern      |If Hollow Mage is alive, you wake up facing it with no warning.                                                         |
|[10] The Underhall   |Drops you at the Troll before you may be ready.                                                                         |

### Monster State Filtering

Both pools check monster state at runtime. Landing in a room where the monster
is already dead resolves as a normal room entry — no fight, just inconvenience.
The pools are not rerolled — the dungeon put you there regardless.

### Visited Flag

[5] sets its visited flag on first entry. On revisit the waking text is different —
the player recognises the room. The dread is knowledge, not surprise.
The mechanic fires regardless. Recognition does not protect you.

### Waking Text

The LUCK test is automatic — the player does not choose to invoke it. The narrative
carries the weight of making that feel fair. The destination does the punishing or
rewarding; the waking text sets the tone.

**Lucky outcome:**
*You open your eyes. Stone ceiling. Cold floor. You are alive and — remarkably —
unharmed. Whatever this place did to you, it could have done worse.*

**Unlucky outcome:**
*You open your eyes. Stone ceiling. Cold floor. You curse under your breath.
Of all the places to wake up.*

### Auto-Luck Narrative Principle

Any time the game tests LUCK without explicit player input, the resolution text
signals the outcome through tone and consequence — not mechanical announcement.
The player sees where they landed and what they face. The STATS display confirms
the LUCK decrement. In the moment, it is storytelling.

-----

## The Riddle Room — Room [8]

A mental trap. No monster, no SKILL test, no sneak, no escape. Intelligence is
the only currency.

### What Happens

The player enters [8]. The door behind them vanishes — or will not open.
Two doors ahead, each bearing an answer. A riddle is carved into the wall between them.
The dungeon waits. There is no way back. There is no way forward except a choice.

- **Correct door** -> progress to the next location
- **Incorrect door** -> instant and unavoidable death

### The Mechanic

- Riddle selected randomly at game startup (not on entry) — fixed for that run
- Correct door (LEFT or RIGHT) assigned randomly at startup alongside riddle
- Player types LEFT or RIGHT
- No other input accepted — unrecognised input: *“The door does not move. The room waits.”*
- No SKILL check, no LUCK test, no SNEAK, no combat
- Solving the riddle correctly tells the player which door is correct — the riddle
  answer maps directly to a door. There is no coin flip after.

### Death Text

> *You pass through the door into a room straight from your worst nightmare. The mouldy
> bones of previous adventurers litter the floor. Realising your mistake you turn to
> leave — only to find the door has no handle. On closer examination you notice bloody
> scratches, and is that a fingernail?*
> 
> *A deep rumble shakes your body and turns your bowels to water. You look up to see
> the ceiling, slow but inexorably, lowering.*
> 
> *You frantically try to open the door, adding to the bloody legacy, as the room
> crushes the life out of you.*

### Riddle Bank (8 riddles — one selected per run)

Each riddle has exactly two plausible answers — one correct, one believable enough
to tempt a panicked player. The wrong answer is never obviously stupid.

|#|Riddle                                                                                                                                   |Correct Answer                                                             |Wrong Answer                      |
|-|-----------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------|----------------------------------|
|1|*I have cities but no houses, mountains but no trees, water but no fish, and roads but no cars. What am I?*                              |A map                                                                      |A dream                           |
|2|*The more you take, the more you leave behind. What am I?*                                                                               |Footsteps                                                                  |Shadows                           |
|3|*I speak without a mouth and hear without ears. I have no body but come alive with wind. What am I?*                                     |An echo                                                                    |The wind                          |
|4|*This man’s father is my father’s son, and I have no brothers. Who is this man?*                                                         |My son                                                                     |Myself                            |
|5|*Two guards stand before two doors. One always tells the truth. One always lies. You may ask one question of one guard. What do you ask?*|Ask either guard which door the other would say is safe — take the opposite|Ask one guard and trust the answer|
|6|*The more you have of it, the less you see. What is it?*                                                                                 |Darkness                                                                   |Silence                           |
|7|*I am not alive but I grow. I don’t have lungs but I need air. I don’t have a mouth but water kills me. What am I?*                      |Fire                                                                       |Time                              |
|8|*What can run but never walks, has a mouth but never talks, has a head but never weeps, has a bed but never sleeps?*                     |A river                                                                    |A road                            |

### Design Notes

- The riddle room is the only place where the player’s own intelligence is tested —
  not SKILL, not LUCK, not STAMINA. A completely different texture from everything else.
- Instant death is correct. It makes the riddle matter in a way no STAMINA cost could.
- Every riddle has one unambiguous answer. The death is always earned, never arbitrary.
- The wrong answer is plausible — something a rushing or panicked player might grab.

-----

## Items & Loot Design

### Loot Philosophy

Pool larger than available slots — 5 shuffled items across 6 loot slots. One slot
is always empty each run. Absence is as interesting as presence.

### Inventory System

- **Soft limit:** 3 items — normal carry capacity
- **Hard limit:** 4 items — overburdened state, cannot exceed
- **Overburdened penalties:**
  - Movement: -1 STAMINA per GO command
  - Combat SKILL: -1
  - Combat damage taken: +1 per round
- All penalties lift immediately when inventory drops to 3 or below
- Overburdened STAMINA penalty does not trigger poison check — separate damage sources

### Item Commands

|Command    |Turns|Notes                                       |
|-----------|-----|--------------------------------------------|
|TAKE [item]|0    |Selective pickup                            |
|TAKE ALL   |0    |Greedy pickup to hard limit of 4, then stops|
|DROP [item]|0    |Item stays in room, re-findable on SEARCH   |
|USE [item] |1    |Consume or activate                         |
|INVENTORY  |0    |No time cost — meta information             |

TAKE ALL with a full pack tells the player what couldn’t be taken and lists
remaining items. Player then uses TAKE [item] to choose selectively.

### SEARCH Behaviour

- SEARCH costs 2 turns regardless of what is found
- SEARCH checks the room and any defeated monster in a single pass — one action
- Room description on SEARCH includes all findable content: room loot, monster drop,
  fixed items
- `DIM searched(12) AS INTEGER` tracks exhausted rooms — second SEARCH still costs 2 turns, zombie still moves, events still tick
- Already-searched rooms: *“You go through the room again. You find nothing you missed the first time.”* The turn cost is the punishment, not a hard block.

### Fixed Items (not shuffled)

|Item              |Location                         |Notes                                                                                   |
|------------------|---------------------------------|----------------------------------------------------------------------------------------|
|Antidote Vial     |Cistern cache [9], always present|Poison cure. Fixed because Horror poisons on easy route — must be reachable before boss.|
|Bangle of Courage |Pit Guardian drop [7]            |Negates Terror. Fixed hard-route payoff.                                                |
||Sword of Sharpness|Armoury locked chest [3]         |Requires Brute's key. +1 to all player damage output while carried. Passive, lifts on drop.|
|Mouldy Bread      |Troll drop [10], always present  |Restores 3 STAMINA. Grim but useful. Belonged to someone who didn’t make it out.        |

### Shuffled Pool (5 items across 6 loot slots)

|Item             |Code|Effect                            |Notes                                                        |
|-----------------|----|---------------------------------|-------------------------------------------------------------|
|Healing Potion   |1   |Restore STAMINA to starting value |Single use                                                  |
|Lucky Charm      |2   |+2 LUCK, single use               |Player chooses when                                         |
|Armour Shard     |3   |Reduce damage taken by 1 per round|Passive — auto-equips on pickup. Lifts on drop.             |
|Dark Bread       |4   |Restore 4 STAMINA                 |Better quality than Mouldy Bread. +1 gap.                   |
|Medal of Valour  |5   |SKILL -1 on pickup, permanent     |Appears as a reward. Is not one. See full spec below.       |

### Medal of Valour — Full Specification

The fifth shuffled item. Named for the general who would become the Bound King —
awarded for genuine service to the First King before his betrayal. The dungeon
has held it for three hundred years. It sits in search results looking exactly
like the Bangle of Courage or the Sword of Sharpness — decorated, old, valuable.
It is none of those things.

**Pickup text (register):** *"It is heavier than it looks. Old — older than anything
else down here. Decorated. Whatever it once meant, it meant something."*

**Effect:** SKILL -1 applied immediately and silently on first pickup. STATS display
will show the change. No announcement in the pickup text. The penalty is permanent —
it cannot be reversed by dropping the item.

**Drop behaviour:** The item can be dropped. It stays in the room. A player who drops
it and checks STATS will find the number unchanged. The permanence is revealed through
silence, not announcement. The item can be re-taken — but `medalTaken = 1` after first
pickup, so the SKILL penalty does not fire a second time.

**USE:** Does nothing. One line — register: *"It sits in your palm. Cold. It does not
respond to you."* Costs 1 turn.

**Lore (authorial — not stated in-game):** The general's name was deliberately erased
from history along with the First King's. The medal predates the betrayal. It was
real once. The dungeon does not explain this. The name is the only clue the player
ever receives.

**Greed mechanic note:** The Medal sits in the same search results as the Healing
Potion. A player who grabs everything pays for it. A player already overburdened
who grabs it anyway faces SKILL -1 on top of the existing combat penalty. The
dungeon has always been teaching the same lesson.

### Item Codes — Locked

All item codes are locked. Fixed items use codes 6–10 and are never placed in
`slotContents`.

|Code|Item              |Pool   |Location           |Effect                                   |Drop behaviour|
|----|------------------|-------|-------------------|-----------------------------------------|--------------|
|0   |*(empty slot)*    |—      |—                  |—                                        |—             |
|1   |Healing Potion    |Shuffled|Loot slots        |Restore STAMINA to starting value        |Stays in room |
|2   |Lucky Charm       |Shuffled|Loot slots        |+2 LUCK, single use                      |Stays in room |
|3   |Armour Shard      |Shuffled|Loot slots        |Damage taken -1 per round, passive       |Stays in room |
|4   |Dark Bread        |Shuffled|Loot slots        |Restore 4 STAMINA                        |Stays in room |
|5   |Medal of Valour   |Shuffled|Loot slots        |SKILL -1 on first pickup, permanent      |Stays in room |
|6   |Antidote Vial     |Fixed  |[9] Cistern cache  |Clears poisoned status                   |Stays in room |
|7   |Bangle of Courage |Fixed  |[7] Pit Guardian   |Negates all Terror while carried         |Stays in room |
|8   |Sword of Sharpness|Fixed  |[3] Armoury chest  |+1 to all player damage output           |Stays in room |
|9   |Mouldy Bread      |Fixed  |[10] Troll drop    |Restore 3 STAMINA                        |Stays in room |
|10  |Guardroom Key     |Fixed  |[2] Brute drop     |Unlocks Armoury chest                    |Stays in room |

### Loot Slot Assignments

|Slot|Room                        |Route|
|----|----------------------------|-----|
|1   |[1] Entry Hall              |Both |
|2   |[2] Guardroom               |Both |
|3   |[4] The Crossroads          |Both |
|4   |[6] Collapsed Passage       |Easy |
|5   |Post-[8] Riddle Room passage|Hard |
|6   |[10] The Underhall          |Both |

Slots 1, 2, 3, and 6 are reachable on both routes — the Healing Potion guarantee
is satisfiable regardless of which route the player takes. Exactly one slot is
always empty each run.

### Pre-Game Entry Hall Check

After the Fisher-Yates shuffle resolves, before the opening sequence begins, a
silent check fires: if the Medal of Valour (code 5) landed in slot 1 (Entry Hall),
a free luck roll runs — `RollDice(2) <= luck` with no LUCK decrement. This is the
only luck check in the game that costs nothing.

- **Lucky:** The Medal swaps with a randomly chosen helpful item (codes 1–4) from
  another slot. The Medal moves deeper into the dungeon. The helpful item moves to
  the Entry Hall. The player sees nothing.
- **Unlucky:** The Medal stays in slot 1. No swap. No announcement. The player finds
  it when they SEARCH — or doesn't search and never knows.

The player's starting LUCK is untouched either way. The check is invisible. The
dungeon either protected the unwary or it didn't.

### Healing Potion Guarantee

Both routes must have at least one opportunity to find a Healing Potion before
the boss. The guarantee is structural — the slot assignments ensure it, not the
shuffle logic. Finding it requires SEARCH; the opportunity exists but the player
must look.

-----

## Input Mechanic

### Command Style

Free-text verb-noun commands with a small fixed vocabulary. The player types
freely but the game understands only defined verbs. Feels like a classic text
adventure without the parsing complexity.

**Recognised commands:**

|Command                   |Turns      |Notes                                                  |
|--------------------------|-----------|-------------------------------------------------------|
|GO [NORTH/SOUTH/EAST/WEST]|1          |Primary navigation                                     |
|LOOK                      |1          |Takes time — you are standing still observing          |
|SEARCH                    |2          |Most time-consuming — physically going through the room|
|FIGHT                     |1 per round|Each combat round advances the turn counter            |
|SNEAK                     |1          |Attempt to bypass a monster                            |
|LUCK                      |1          |Test Your Luck in applicable situations                |
|USE [item]                |1          |Use an item from inventory                             |
|TAKE [item]               |0          |Selective item pickup                                  |
|TAKE ALL                  |0          |Greedy pickup to hard limit                            |
|DROP [item]               |0          |Drop item, stays in room                               |
|INVENTORY                 |0          |No time cost                                           |
|STATS                     |0          |No time cost                                           |
|HELP                      |0          |No time cost                                           |

Input normalised with `UPPER$`. Verb extracted with `LEFT$` and `LEN`.
Parsed with `IF` / `ELSE IF` chain — no complex parser required.

### Contextual Hints

On entering each room the game prints a hint line showing available commands
in context. Player still types freely — hints remind them what makes sense here.
Example: *“You can GO NORTH, SEARCH the room, or FIGHT the creature.”*

-----

## Turn-Based Events

### Turn Counter

Every time-advancing command increments a global `turns` variable.
After each increment, the event system checks for triggered events.

### Estimated Play Length

|Activity |Turns    |
|---------|---------|
|Movement |18-22    |
|Searching|16-20    |
|Combat   |20-36    |
|Other    |8-12     |
|**Total**|**62-90**|

Fast aggressive player (sneaks monsters, minimal searching): 35-45 turns.
Thorough careful player: 80-100 turns.
Target play length for a typical run: 45-75 turns.

### Atmospheric Events

Fire on an organic RND probability check — roughly 15% chance per turn,
averaging every 6-7 turns with natural variance. Never clockwork.

```basic
IF RND * 10 > 8.5 THEN
    CALL AtmosphericEvent()
END IF
```

Event selected randomly from pool of 12 via `RND`. Repeats on long runs are
acceptable — the dungeon being the dungeon.

### Atmospheric Event Pool (12 events)

**Pure Flavour — 7 events**

*1. The Torch*
Your torch gutters. For a moment the darkness is absolute — a living, pressing
thing. Then the flame catches again. You breathe out. You hadn’t noticed you’d stopped.

*2. The Sound*
Somewhere below you — deeper than you’ve been — something moves. A slow, rhythmic
scraping. Stone on stone. It stops. You wait. It doesn’t start again.

*3. The Wall*
You place your hand against the wall to steady yourself. The stone is warm. Not the
warmth of your own body heat — it was warm before you touched it. You take your hand away.

*4. The Air*
The air changes. A cold current from nowhere, carrying the smell of standing water
and something older. Something that hasn’t seen light in a very long time.

*5. The Bones*
You almost step on them. Small. Difficult to identify. You don’t try.

*6. The Silence*
The dungeon goes completely quiet. No drip of water, no distant groan of settling
stone. Nothing. It lasts perhaps ten seconds. It feels much longer. Then the sounds
return, and you realise how much you’d been relying on them.

*7. The Marks*
Scratches in the wall at eye height. You lean close. Letters, cut deep and fast by
someone who knew they were running out of time. Three words. The middle one is
illegible. The first is *do not* and the last is *alone*.

**Mechanical — 3 events**

*8. The Weight* — STAMINA -1
The cold gets into you. Not the sharp cold of wind — the slow cold of deep stone,
of places that have never been warm. You feel it in your chest first.
*(STAMINA ticks down 1. No announcement — STATS will show it.)*

*9. The Hunger* — STAMINA -1
You haven’t eaten. You’d forgotten until now. Your body reminds you with a clarity
that’s difficult to ignore.
*(Same mechanic, different flavour. Two STAMINA events increases frequency without
telegraphing it.)*

*10. The Stumble* — conditional
Your foot catches on uneven stone.

- If overburdened: the weight pulls you down hard. You catch yourself on one knee.
  It costs you. STAMINA -1.
- If not overburdened: you catch yourself easily. A reminder that the floor is not
  your friend. Flavour only.

**Threat Escalation — 2 events**

*11. The Zombie Spawn Check*
Fires the spawn probability check if the zombie hasn’t spawned yet. No flavour
text — the event is invisible. The zombie either appears or doesn’t. If it spawns,
the next room description carries the weight of that.

*12. The Presence*

- If zombie is alive and within 2 rooms of player: *Something is moving in the
  dungeon. Not far away. The sound has direction.*
- If zombie is dead or far away: *You have the distinct feeling you are not alone.
  You stand very still. Whatever it was, it passes.* Flavour only.

### The Wandering Zombie

A roaming entity that exists independently of the room structure. Unlike every
other creature in the dungeon — which is *placed* — the zombie is *loose*.

**Spawn condition:**

```basic
IF turns > 3 AND RND * 10 > 7 AND NOT zombieSpawned THEN
    LET zombieSpawned = 1
    LET zombieRoom = 4
END IF
```

Not guaranteed. Not immediate. The player may never encounter it on a fast run.
Spawns in room [4] The Crossroads.

**Wandering:**
Each turn after spawning, the zombie rolls to move. It picks randomly from
the available exits of its current room — the same exits array the player uses.
It has no knowledge of the player’s position. It simply wanders.

```basic
SUB WanderZombie()
    ' pick random valid exit from zombieRoom
    ' move zombie there
    ' exclude rooms [5], [8], and [11] — zombie never enters these
END SUB
```

**Encounter:**
When the player moves into a room, check if the zombie is present:

```basic
IF zombieSpawned AND zombieRoom = currentRoom THEN
    ' zombie encounter — fight or sneak
END IF
```

SEARCH counts as 2 turns — two zombie movement rolls while your back is turned.
A long combat also eats turns. Lingering has consequences.

**Room exclusions:**
The zombie never enters [5] The Still Chamber, [8] The Riddle Room, or [11] the
Throne Room. These rooms belong to the dungeon’s own mechanisms — the zombie is
a loose, dumb, wandering thing and has no business in spaces with deliberate
architecture. No explanation is given for any of the exclusions. The player may
notice on repeat runs.

**Death:**
If the player kills the zombie it is gone permanently. The dungeon is quieter.
The player spent turns on a fight they might have avoided by moving faster.

-----

## Sneaking

### SNEAK command

Available before entering a room with a known monster (room description hints
at a presence before full entry). Flat SKILL check — same difficulty for all
sneakable monsters.

- **Success** — slip past, monster marked as avoided, no loot from that room
- **Failure** — detected, combat begins immediately, no further sneak attempt

Design tension: fight and risk STAMINA loss but gain drops, or sneak and
preserve yourself for later. Every combat is a genuine decision.

### Boss room sneak

Attempting SNEAK at [11] is explicitly futile. The game acknowledges the attempt
and refuses it in flavour text:
*“You press yourself against the wall and inch toward the door. The creature’s
eyes find you immediately. Some things cannot be avoided.”*
Combat begins regardless. The exception being explicit makes the rule feel earned.

### Sneaking past the zombie

Same mechanic as fixed monsters but feels different — the zombie is unpredictable.
You are not slipping past a stationary guard. You are hoping it does not turn around.

-----

## Opening Sequence Design

Before character creation — a narrative opening using paced `INPUT` beats.

1. Scene setting — the keep, the crowd, the gates
2. Lord Malachar’s proclamation (or a herald reading it)
3. The prize announced
4. “Press ENTER to step forward”
5. Character creation — roll stats, display them
6. “The gates close behind you”
7. Game begins

This sequence showcases narrative output, string formatting, and `INPUT` as a
pacing device before any game mechanic appears.

-----

## UI Design

### Approach: Frame-Based Layout

SharpBASIC has `PRINT` and `INPUT` only — no cursor positioning, no ANSI codes,
no colour. True panel-based TUIs are not possible. The frame-based approach
redraws a consistent structure on every meaningful state change. Authentic to
1990s text adventures (Magnetic Scrolls, Legend Entertainment). The player
always has orientation.

### Standard Frame Structure

```
================================================================================
  THE SUNKEN CROWN                              SKILL: 9  STAMINA: 14  LUCK: 7
================================================================================
  LOCATION: The Guardroom                                            Turn: 7

  The room smells of old iron and worse. A broad-shouldered figure
  blocks the passage south. It hasn't noticed you yet. There are
  marks on the floor — deep gouges, as if something heavy was dragged.

  Exits: NORTH, EAST
  [ A creature is here. You can FIGHT or SNEAK. ]
--------------------------------------------------------------------------------
 >
```

- **Header bar** — `=` border full width (80 chars). Title left, SKILL/STAMINA/LUCK right. Redrawn every frame. `SUB PrintHeader()`.
- **Location line** — room name left, turn counter right. Below the header.
- **Narrative block** — 2-space indented. Room descriptions, atmospheric events, combat text.
- **Contextual hint line** — wrapped in `[ ]`. Always reflects actual available actions. Visually distinct from narrative.
- **Separator** — `---` full width. Creates clear read/type split.
- **Prompt** — `>` on its own line. Consistent. Unmistakeable.

### Inventory Frame

`INVENTORY` command redraws the full screen as a dedicated frame. Press ENTER to return.
Mirrors Fighting Fantasy equipment screen convention. Not a sidebar — sidebars require cursor control.

```
================================================================================
  THE SUNKEN CROWN                              SKILL: 9  STAMINA: 14  LUCK: 7
================================================================================
  INVENTORY                                                          3 / 4 items

  [1] Healing Potion
  [2] Sword of Sharpness    (equipped -- +1 SKILL)
  [3] Armour Shard          (passive -- damage reduced by 1)

  [ Press ENTER to continue ]
--------------------------------------------------------------------------------
```

### Combat Frame

Each round redraws the full frame. Attack numbers visible — player sees the dice.
Authentic FF. Combat feels active, not passive.

```
================================================================================
  THE SUNKEN CROWN                              SKILL: 8  STAMINA: 11  LUCK: 6
================================================================================
  COMBAT: Guardroom Brute                                           Round: 3

  You attack. Your strike lands. The Brute staggers.

  YOUR ATTACK:  14    BRUTE ATTACK:  11    You win this round.
  Brute STAMINA: 8

  [ Press ENTER for next round, or USE [item] ]
--------------------------------------------------------------------------------
 >
```

### Core Display SUBs

- `SUB PrintHeader()`
- `SUB PrintSeparator()`
- `SUB PrintRoom(roomId AS INTEGER)`
- `SUB PrintCombatFrame(...)`
- `SUB PrintInventoryScreen()`

### What Is Not Used

No box-drawing pop-up windows (`+---+` style). Maintenance overhead in BASIC
string manipulation is not worth it. The frame pattern is cleaner and more
authentic to the era.

-----

## Decisions Log

|Date      |Decision                                                                          |Rationale                                                                                                                                                                                                                                         |
|----------|----------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|March 2026|Fighting Fantasy inspiration, not clone                                           |Authentic spirit, original content                                                                                                                                                                                                                |
|March 2026|No save function                                                                  |No file I/O in v1; retro jeopardy is a feature not a limitation                                                                                                                                                                                   |
|March 2026|No Baron — use Lord/King equivalent                                               |Avoid direct Deathtrap Dungeon reference                                                                                                                                                                                                          |
|March 2026|Working title: The Sunken Crown                                                   |Evocative, original. Not final.                                                                                                                                                                                                                   |
|March 2026|Working villain: Lord Malachar                                                    |Not final — needs a decision                                                                                                                                                                                                                      |
|March 2026|Hybrid randomisation                                                              |Fixed structure + randomised texture. Learn the dungeon, not the run.                                                                                                                                                                             |
|March 2026|Terror mechanic for final boss                                                    |Bangle of Courage as key item. Lesser terror encounters teach the mechanic first.                                                                                                                                                                 |
|March 2026|Monster stats randomised within a band                                            |Tension on repeat runs without destroying learnability                                                                                                                                                                                            |
|March 2026|Loot pool shuffled at startup                                                     |Exploration valuable every run                                                                                                                                                                                                                    |
|March 2026|12 rooms                                                                          |Two routes converging before mandatory boss encounter                                                                                                                                                                                             |
|March 2026|No bypass of boss                                                                 |Gold is behind the boss. Exit is behind the boss. No way around.                                                                                                                                                                                  |
|March 2026|Living dungeon — ancient magic                                                    |Explains asymmetric connections narratively. Players have no magic.                                                                                                                                                                               |
|March 2026|One centrepiece asymmetric connection                                             |[4]->[5] south returns to [2] not [4]. Dungeon reroutes itself.                                                                                                                                                                                   |
|March 2026|Visited room tracking                                                             |`DIM visited(12)` — descriptions vary on revisit. Atmosphere and fairness.                                                                                                                                                                        |
|March 2026|Healing potion chance on both routes                                              |Design fairness — both paths must have the opportunity before the boss                                                                                                                                                                            |
|March 2026|Loot pool larger than loot slots                                                  |4 items, 5 slots. Absence creates replayability.                                                                                                                                                                                                  |
|March 2026|Monster drops on select enemies                                                   |2-3 monsters may carry items. Search after combat. Rewards thoroughness.                                                                                                                                                                          |
|March 2026|Cistern cache is fixed, not shuffled                                              |Rewards players who found the secret route. Always something good.                                                                                                                                                                                |
|March 2026|Free-text verb-noun commands                                                      |Classic text adventure feel without complex parser. Small fixed vocabulary.                                                                                                                                                                       |
|March 2026|LOOK and SEARCH cost turns                                                        |Both take time — the dungeon does not pause while you observe or rummage                                                                                                                                                                          |
|March 2026|SEARCH costs 2 turns                                                              |Most time-consuming action — two zombie movement rolls while back is turned                                                                                                                                                                       |
|March 2026|STATS, INVENTORY, HELP cost no turns                                              |Meta information only — dungeon time does not advance                                                                                                                                                                                             |
|March 2026|Contextual hint line per room                                                     |Shows available commands on entry. Player still types freely.                                                                                                                                                                                     |
|March 2026|Atmospheric events — organic RND probability ~15% per turn                        |Averages every 6-7 turns with natural variance. Never clockwork. Dungeon feels alive.                                                                                                                                                             |
|March 2026|Atmospheric event pool — 12 events locked                                         |7 pure flavour, 3 mechanical (STAMINA), 2 threat escalation. All text written and locked.                                                                                                                                                         |
|March 2026|Wandering zombie — not guaranteed to spawn                                        |Spawns after turn 3 on a probability check. Fast runs may never see it.                                                                                                                                                                           |
|March 2026|Zombie spawn room — [4] The Crossroads                                            |Maximum reach from spawn point. Hub of the dungeon. Maximises threat range.                                                                                                                                                                       |
|March 2026|Zombie wanders via exits array                                                    |Uses same data structure as player navigation. No special pathfinding.                                                                                                                                                                            |
|March 2026|Zombie never enters [5], [8], or [11]                                             |These rooms belong to the dungeon’s own mechanisms. Zombie has no business there.                                                                                                                                                                 |
|March 2026|SNEAK available for all monsters except boss                                      |Fight or sneak — genuine decision with STAMINA vs loot tradeoff                                                                                                                                                                                   |
|March 2026|Boss sneak attempt fails with flavour text                                        |Exception made explicit. “Some things cannot be avoided.”                                                                                                                                                                                         |
|March 2026|Combat rounds advance turn counter                                                |Long fights eat turns. Zombie may close in during extended combat.                                                                                                                                                                                |
|March 2026|Monster roster fixed at 6 plus wandering zombie                                   |Covers both routes. Distinct mechanic per creature. All mechanics map to SharpBASIC v1 features.                                                                                                                                                  |
|March 2026|Guardroom Brute — Room [2], standard combat baseline                              |Teaching encounter. Establishes combat before special mechanics appear. Carries key to Armoury chest.                                                                                                                                             |
|March 2026|Skittering Horror — Room [6], poison + lesser terror                              |Heard and felt before seen. Lesser terror (round one only, SKILL -1) teaches mechanic. Poison justifies Antidote Vial.                                                                                                                            |
|March 2026|Pit Guardian — Room [7], armoured mechanic                                        |Damage reduced by 1 per round (min 1). Hard route payoff — fixed Bangle of Courage drop.                                                                                                                                                          |
|March 2026|Hollow Mage — Room [9], luck drain mechanic                                       |Loses 1 LUCK per round regardless of outcome. Guards Cistern cache. Teaches LUCK management before boss.                                                                                                                                          |
|March 2026|Troll — Room [10], regeneration mechanic                                          |Restores 1 STAMINA per round. Unavoidable — both routes converge at Underhall. No loot.                                                                                                                                                           |
|March 2026|Bound King — Room [11], full Terror + Crushing Blow                               |Without Bangle: SKILL -2, LUCK -1 for full fight. Crushing Blow: wins by 3+ deals 4 damage. Cannot leave the Throne Room.                                                                                                                         |
|March 2026|Lesser terror on Skittering Horror only                                           |Darkness and sound sell the fear before sight. Round-one SKILL -1 only. Bangle negates.                                                                                                                                                           |
|March 2026|Sneak checks are flat SKILL rolls                                                 |No per-monster difficulty modifier. Simplicity over granularity at this scope.                                                                                                                                                                    |
|March 2026|Antidote Vial fixed in Cistern cache [9]                                          |Horror poisons on easy route. Vial must be reachable on same route before boss. Cache solves both problems.                                                                                                                                       |
|March 2026|Bangle of Courage fixed drop from Pit Guardian [7]                                |Hard route payoff. Critical for final boss terror mitigation. Not shuffled.                                                                                                                                                                       |
|March 2026|Sword of Sharpness fixed in Armoury locked chest [3]                              |Requires Brute’s key. Rewards thorough play.                                                                                                                                                                                                      |
|March 2026|Shuffle pool: Healing Potion, Lucky Charm, Armour Shard, Dark Bread               |Four items across 5 loot slots. Something always missing each run.                                                                                                                                                                                |
|March 2026|Loot slot assignments locked — 5 slots across [1],[2],[4],[6],post-[8]            |Slots 1-3 shared by both routes. Healing potion guarantee satisfied comfortably.                                                                                                                                                                  |
|March 2026|SEARCH checks room and monster in single pass                                     |One action, 2 turns. Requiring two searches is friction without meaning. The cost is the tax.                                                                                                                                                     |
|March 2026|Searched room tracking — `DIM searched(12)`                                       |Once searched, exhausted. Prevents turn farming.                                                                                                                                                                                                  |
|March 2026|Inventory soft limit 3, hard limit 4                                              |3 = normal, 4 = overburdened. Hard cap — cannot carry 5th item.                                                                                                                                                                                   |
|March 2026|Overburdened penalties — movement, combat SKILL, combat damage                    |Movement: -1 STAMINA per GO. Combat: SKILL -1, damage taken +1. All lift when back to 3.                                                                                                                                                          |
|March 2026|Overburdened STAMINA loss does not trigger poison check                           |Separate damage sources. Keep them clean and distinct.                                                                                                                                                                                            |
|March 2026|TAKE ALL greedy to hard limit, then stops with message                            |Player then uses TAKE [item] to choose selectively. No penalty on TAKE/DROP — free actions.                                                                                                                                                       |
|March 2026|DROP leaves item in room, re-findable on SEARCH                                   |Inventory management is a thinking action. Free. Encourages engagement.                                                                                                                                                                           |
|March 2026|Room [5] — The Still Chamber, teleport trap                                       |No visible exit. Player passes out, wakes elsewhere. Dungeon as active hostile entity, not passive maze.                                                                                                                                          |
|March 2026|Still Chamber uses Test Your Luck for destination                                 |Lucky pool = favourable rooms. Unlucky pool = punishing rooms. LUCK decrements regardless.                                                                                                                                                        |
|March 2026|Still Chamber turn penalty = 3 turns                                              |Zombie moves, atmospheric events tick. Passing out costs time.                                                                                                                                                                                    |
|March 2026|Still Chamber exclusions: [5], [11], [12]                                         |Prevent infinite loop, prevent bypassing boss, prevent skipping to exit.                                                                                                                                                                          |
|March 2026|Still Chamber lucky pool: [1], [3], [9]                                           |Safe rooms or rooms with loot advantage. [9] gives hard-route players Cistern access.                                                                                                                                                             |
|March 2026|Still Chamber unlucky pool: [6], [7], [8], [10]                                   |Rooms with live monsters, instant death trap, or the unavoidable Troll.                                                                                                                                                                           |
|March 2026|Still Chamber visited flag fires on entry                                         |On revisit player recognises the room. Dread from knowledge. Mechanic fires regardless.                                                                                                                                                           |
|March 2026|Still Chamber zombie exclusion — [5] added to zombie no-entry list                |Zombie in the Still Chamber muddies the mechanic. The room belongs to the dungeon.                                                                                                                                                                |
|March 2026|Still Chamber waking text signals luck outcome through narrative tone             |Lucky: unharmed, could have been worse. Unlucky: cursing, dread of destination. No mechanical announcement.                                                                                                                                       |
|March 2026|Auto-luck narrative principle established                                         |Any auto-tested LUCK is signalled through tone and consequence, not announcement. STATS display confirms the decrement.                                                                                                                           |
|March 2026|Room [8] — The Riddle Room, instant death trap                                    |No monster, no SKILL test, no sneak, no escape. Two doors, two answers, one riddle. Wrong door = instant unavoidable death.                                                                                                                       |
|March 2026|Riddle Room: riddle and correct door assigned at startup not on entry             |Fixed for that run. Repeat runs vary. Players who share notes cannot rely on a fixed mapping.                                                                                                                                                     |
|March 2026|Riddle Room: correct door maps directly to riddle answer                          |Solving the riddle tells you the door. No coin flip after. Intelligence is genuinely rewarded.                                                                                                                                                    |
|March 2026|Riddle Room: unrecognised input held by room                                      |“The door does not move. The room waits.” No escape route through bad input.                                                                                                                                                                      |
|March 2026|Riddle bank: 8 riddles, each with correct and plausible wrong answer              |Wrong answer is never obviously stupid — believable enough to catch a panicked player.                                                                                                                                                            |
|March 2026|Room [8] added to Still Chamber unlucky pool                                      |Waking in the Riddle Room is immediately punishing — trapped with a death trap in front of you. If already solved this run, doors are open.                                                                                                       |
|March 2026|Room [8] placed on hard route between [7] and [10]                                |Breathing space before the Troll. Different threat texture between the two fights.                                                                                                                                                                |
|March 2026|Room [12] — The Gate, two-door final twist                                        |One door leads outside. One leads deeper, into darkness and eventual death. The dungeon gets one last move.                                                                                                                                       |
|March 2026|Gate correct door randomised at startup                                           |Consistent with the tell each run. Players who share notes cannot rely on a fixed mapping.                                                                                                                                                        |
|March 2026|Gate tell — correct door is newer, less worn                                      |Environmental and subtle. Not signposted. Evidence that almost nobody survives. Rewards careful players on repeat runs.                                                                                                                           |
|March 2026|Gate wrong door = quiet death, no recovery                                        |No drama, no monster. The dungeon swallows you. Contrast with the Bound King fight makes it land.                                                                                                                                                 |
|March 2026|Gate player input: LEFT or RIGHT                                                  |Same mechanic as Riddle Room. No other input accepted.                                                                                                                                                                                            |
|March 2026|Gate win text locked                                                              |No crowd. Malachar and guards in courtyard. Gold already taken from the Bound King. Malachar steps forward, says “Go.”, steps aside. Player goes. End screen shows run stats then play again prompt.                                              |
|March 2026|Title locked — The Sunken Crown                                                   |Named for the Bound King’s crown — the First King’s crown, stolen, cursed, fused. The dungeon is named for the thing at its heart.                                                                                                                |
|March 2026|Villain name locked — Lord Malachar                                               |Did not create the trial. Inherited a dungeon with a monster in it and built a revenue stream around both facts.                                                                                                                                  |
|March 2026|Bound King backstory locked                                                       |General who stole the First King’s crown. Cursed with his dying breath — bound to the dungeon, bound to the crown, never dying, feeling everything. Three hundred years in that room.                                                             |
|March 2026|Bound King cannot be killed — only beaten                                         |Curse holds. He falls, he regenerates. Post-combat beat: “The King falls. You don’t wait. You can already see his fingers moving.” Player takes gold and runs.                                                                                    |
|March 2026|Regeneration explains the Gate’s worn door                                        |He resets. The trial resets. Some survivors have come before — rare enough that the correct door is barely touched, common enough that the wrong door is worn with desperate hands.                                                               |
|March 2026|Player motivation — cash in everything, last desperate gamble                     |Not an entrance fee. Everything the player owns converted to coin and dropped into the Throne Room. Nothing to go back to if they fail.                                                                                                           |
|March 2026|Gold as discrete items — bags of gold                                             |TAKE BAG takes one. Maximum four — standard hard inventory limit. Each bag is a decision made while the Bound King regenerates.                                                                                                                   |
|March 2026|Regeneration rolls after each bag taken                                           |Bag 1 free. Bag 2: 11+ on 2d6. Bag 3: 9+. Bag 4: 7+ (~58% chance). Four bags is a genuine gamble.                                                                                                                                                 |
|March 2026|Second fight if Bound King fully regenerates                                      |He rerolls fresh STAMINA, same SKILL. Player fights with whatever they have left. Win again and run — no more gold.                                                                                                                               |
|March 2026|TAKE ALL refused in Throne Room                                                   |Only room in the game where a standard command is explicitly refused. “You don’t have time for that.” Preserves jeopardy of gold-gathering.                                                                                                       |
|March 2026|End screen reflects bags taken                                                    |“You escaped with X bag(s) of gold.” Player sets their own measure of success.                                                                                                                                                                    |
|March 2026|Crown mechanic — SEARCH after boss reveals the crown                              |Crown stays on Bound King’s head as he falls. SEARCH costs 2 turns, triggers 2 regeneration rolls. Crown appears in results alongside remaining gold bags.                                                                                        |
|March 2026|Crown description — irresistibly valuable, quietly wrong                          |Solid gold, heavily jewelled, worth more than everything else in the room. The temptation is real. The tell is subtle — “it is still on his head. His head is right there.”                                                                       |
|March 2026|Taking the crown triggers the curse — player becomes the Bound King               |Crown goes on the player’s head. The Bound King is freed. The player cannot leave. End screen: “You are the Bound King now.”                                                                                                                      |
|March 2026|Bound King appearance — looks human, not monstrous                                |Looks like a tired man in his forties. Armoured, scarred but whole. The curse keeps him alive and intact. The crown is the only thing that looks ancient — fused, tarnished, wrong. The horror is what he still is, not what he became.           |
|March 2026|Frame-based UI layout                                                             |SharpBASIC has PRINT and INPUT only — no cursor control, no colour, no ANSI. Frame-based layout redraws context on every significant state change. Authentic to 1990s text adventures.                                                            |
|March 2026|Header redrawn every frame                                                        |`=` border full width, title left, SKILL/STAMINA/LUCK right. Always current. Implemented as `SUB PrintHeader()`. No persistent panel needed.                                                                                                      |
|March 2026|Location line below header                                                        |Room name left, turn counter right. Redrawn with every frame.                                                                                                                                                                                     |
|March 2026|Narrative block 2-space indented                                                  |Breathing room without graphics. Room descriptions, events, combat text all land here.                                                                                                                                                            |
|March 2026|Contextual hint line in `[ ]` brackets                                            |Visually distinct from narrative. Always reflects actual available actions for current state.                                                                                                                                                     |
|March 2026|`>` prompt on its own line below separator                                        |Consistent prompt position. `---` separator creates clear read/type split.                                                                                                                                                                        |
|March 2026|Inventory as a full frame, not a sidebar                                          |`INVENTORY` redraws the full screen with item list and slot count. Press ENTER to return. Mirrors FF equipment screen convention.                                                                                                                 |
|March 2026|Combat frame per round                                                            |Each round redraws: header, combat label, narrative, attack numbers, current enemy STAMINA, hint. Player sees the dice — authentic FF.                                                                                                            |
|March 2026|No box-drawing pop-ups                                                            |Maintenance overhead in BASIC string manipulation is not worth it. Frame pattern is cleaner and more authentic.                                                                                                                                   |
|March 2026|Cover image — Prompt A selected, generated, approved                              |The Bound King slumped on his throne, head bowed, crown fused to his temples, piles of gold coin around the base. No face visible. The mood is exhaustion and endurance, not menace. Lore-faithful — the crown never leaves his head.             |
|March 2026|Cover image — lone crown on floor rejected                                        |Canonically impossible. The crown has never left the Bound King’s head in three hundred years. A lone crown on a dungeon floor contradicts the lore and the crown mechanic.                                                                       |
|March 2026|Cover image — Prompt B (bloody hand reaching for crown) generated but not selected|Strong origin-moment concept — the last second before the curse. Held in reserve. Prompt A chosen as the primary cover for its lore fidelity and melancholic weight.                                                                              |
|March 2026|No player or adventurer depicted in any image                                     |Avoid subconscious gendering. No figure representing the player appears in title art or documentation imagery. Player is never referred to by gender in any documentation.                                                                        |
|March 2026|Docs structure — samples/the-sunken-crown/ folder                                 |Game lives under samples/ in the SharpBASIC repo. A dedicated subfolder with its own wiki-style markdown docs. Modelled on INPUT magazine — a learning artefact as much as a game.                                                                |
|March 2026|Docs folder structure locked                                                      |README.md (title page + cover image + index), the-sunken-crown.sb (complete game), HOWTOPLAY.md (player guide), LORE.md (optional backstory), listing/ subfolder with 10 issue files.                                                             |
|March 2026|Two-track delivery — complete file and 10-issue type-in series                    |Complete .sb file for instant gratification. Separate listing/ folder with 10 issues modelled on INPUT magazine — each additive, each runnable, each explaining one concept before presenting the code.                                           |
|March 2026|10-issue arc structure locked                                                     |Issue 1: frame UI. Issue 2: room system. Issue 3: dice and opening sequence. Issue 4: navigation. Issue 5: combat. Issue 6: inventory and search. Issue 7: atmosphere and zombie. Issue 8: trap rooms. Issue 9: boss. Issue 10: full game.        |
|March 2026|Each issue must be independently runnable                                         |No issue leaves the program in a broken state. Every listing produces visible output when typed in and run. Additive — each issue builds on the last.                                                                                             |
|March 2026|Issue template structure locked                                                   |Header (issue number, title, hook), story beat, concept callout (one SharpBASIC feature explained), the listing, what you’ll see, what to try.                                                                                                    |
|March 2026|Nano Banana cover image prompt locked — Prompt A                                  |Full prompt preserved in decisions log. Key elements: armoured figure on stone throne, head bowed, crown fused to temples, gold coin piles, single torch, painterly fantasy art, muted palette, no face visible, no text.                         |
|March 2026|Write complete game first, then split into issues                                 |The 10-issue split is an editorial act performed on finished, tested code. Issues cannot be guaranteed additive and runnable unless the whole game works first. Sequence: write → playtest → split → write explanatory text.                      |
|March 2026|VS Code syntax highlighting — in scope for public release                         |TextMate grammar (.vsix extension) provides keyword/string/comment colouring for .sb files. Small, self-contained, no LSP required. Makes listings in the 10-issue series look polished. Added to release checklist alongside MIT licence.        |
|March 2026|Full LSP / IntelliSense — out of scope                                            |Error squiggles, go-to-definition, hover docs require a language server. This is a post-SharpBASIC concern. Not a stretch goal.                                                                                                                   |
|March 2026|Game writing paused — complete SharpBASIC first                                   |Phases 9 and 10 still outstanding. Game must be written against the final implemented language, not the designed spec. Generate full language sample files after Phase 10 to confirm feature coverage before writing a line of game code.         |
|March 2026|Repeat SEARCH costs turns but is not blocked                                      |Searching an exhausted room still costs 2 turns. Zombie moves, events tick. The turn cost is the punishment — not a hard stop. Flavour text acknowledges the wasted effort. Respects player intelligence, punishes poor memory.                   |
|March 2026|Opening line revised — “Few who enter return”                                     |“No adventurer has ever returned” contradicted the win text and the Gate’s worn door. Revised to: “Few who enter the Sunken Crown return. None speak of what waits inside.” Consistent with survivors existing but knowledge being suppressed.    |
|March 2026|Chute origin locked — grandfather’s forced labour                                 |The gold chute was built by Malachar’s grandfather using forced labour. Workers did not survive construction. Malachar’s men skim a cut before the gold drops. Fills the logistics gap in the gold mechanic.                                      |
|March 2026|Cursed bloodline explanation locked                                               |The Bound King’s curse radiates outward through the stone. Two generations of Malachar’s family living above it have been slowly twisted — compelled to bring contestants, unable to stop. The Bound King does not direct this consciously.       |
|March 2026|Underhall physical remains locked                                                 |[10] contains dry bones, old armour, old clothing — contestants who reached this far but were too injured or poisoned to continue. A thorough SEARCH may yield something useful left behind. Consistent with gold-as-residue going to Throne Room.|
|March 2026|Room [4] dual NORTH exits resolved                                                |[5] The Still Chamber now appears as a distinct NE exit from [4], labelled “a dark passage.” Indistinguishable from a normal exit until entered. Removes the ambiguous dual-NORTH problem cleanly.                                                |
|March 2026|Cistern “dead-end” label removed                                                  |Mislabel. The Cistern has two exits and is on the easy route through to the Underhall. Label corrected in Room types.                                                                                                                             |
|March 2026|Crown mechanic bypasses inventory entirely                                        |TAKE CROWN triggers the curse on contact. No inventory slot consumed. No interaction with the hard limit or gold bags. The crown never enters the inventory — reaching for it is sufficient.                                                      |
|March 2026|Sneak past Pit Guardian — loss of Bangle is intentional                           |Sneaking the Guardian means no Bangle of Courage. Player faces the Bound King with full Terror unmitigated. This is a consequence of the choice, not an oversight. All choices have consequences.                                                 |
|March 2026|Cistern removed from Still Chamber lucky pool                                     |The Cistern has the Hollow Mage. Landing there is only safe if the Mage is already defeated. Removed from lucky pool — moved to unlucky pool where it belongs.                                                                                    |
|March 2026|Underhall added to Still Chamber unlucky pool                                     |Dropping the player at the Troll before they are ready is punishing. Added to unlucky pool alongside the other monster rooms.                                                                                                                     |
|March 2026|Healing Potion guarantee — no luck gate                                           |Finding the Healing Potion requires SEARCH only. No LUCK check gates it. The opportunity exists on both routes — the player must look for it.                                                                                                     |
|March 2026|MID$ added to feature coverage map                                                |Used in command parsing alongside LEFT$ for argument splitting. Was missing from the map. Now listed as LEFT$, MID$ — command parsing.                                                                                                            |
|March 2026|End screen unified into single screen                                             |Two separate end screens consolidated. One screen: survival statement, bags of gold taken, run stats (starting SKILL, lowest STAMINA, LUCK tests used), play again prompt.                                                                        |
|March 2026|Auto-equip on pickup                                                              |No separate EQUIP command. Passive items activate the moment they are picked up and stay active while carried. Drop the item and the bonus drops with it. Maps to simple IF checks in code.                                                       |
|March 2026|Duplicate map diagram removed                                                     |The first diagram under Structure was out of date and did not show [5], [8], or the NE exit. Replaced with a prose description. The Room Map section is now the single authoritative map.                                                         |
|March 2026|Room Map updated — NE exit from [4] to [5]                                        |Diagram updated to show [5] branching NE from [4], consistent with the exits table. The Still Chamber is clearly a distinct direction, not a duplicate NORTH.                                                                                     |
|March 2026|Underhall room type updated — Troll restored, context added                       |The Troll is unavoidable and present. Physical remains explained as evidence of the Troll’s history. SEARCH after combat may yield something left behind.                                                                                         |
|March 2026|Mouldy Bread — fixed Troll drop, restores 3 STAMINA                               |Always present in [10], found on SEARCH after killing the Troll. Grim and functional. Belonged to someone who didn’t make it out. Distinct from Dark Bread — lower quality, fixed location, guaranteed availability.                              |
|March 2026|Dark Bread restores 4 STAMINA, Mouldy Bread restores 3 STAMINA                    |One point difference establishes quality hierarchy. Dark Bread in shuffled pool — found, not guaranteed. Mouldy Bread fixed Troll drop — always available if you fight and search.                                                                |
|March 2026|Underhall added as loot slot 6 — may be empty                                     |The Underhall can carry one shuffled pool item alongside the fixed Mouldy Bread drop. One item always absent each run — the absence applies to the shuffled slot, not the fixed drop.                                                             |
|March 2026|Still Chamber asymmetric return superseded                                        |The 'return south from [5] reaches [2]' text in earlier drafts was superseded. The teleport pool mechanic is the complete confirmed implementation. Room [5] has no navigable exits. There is no return path stored in the exit arrays.|
|March 2026|SEARCH while monster alive — reveals presence, combat begins unconditionally      |Issuing SEARCH in a room with a live fixed monster reveals the player's presence. The 2-turn cost fires, then combat begins unconditionally — no SNEAK option offered. Applies to all rooms with fixed monsters.|
|March 2026|Poison fires before room description — flavour line confirmed                     |On room entry with active poison, damage fires before the room description. Flavour line register: 'The poison spreads further through your blood.' Brief, matter-of-fact, explicit about the source.|
|March 2026|Hollow Mage luck drain at zero — one-time flavour line then floor                 |When LUCK reaches 0 during a Hollow Mage fight, a one-time line fires in the register of: 'A cold wave washes over you. Whatever luck you had, it has finally run out.' LUCK tests auto-fail thereafter. Drain counter fires each round but LUCK cannot go below 0.|
|March 2026|Sword of Sharpness — +1 to all player damage output                              |Adds +1 to all player damage output against all targets. Against armoured targets: no sword deals 1, with sword deals 2. Against unarmoured targets: no sword deals 2, with sword deals 3. Passive while carried, lifts on drop.|
|March 2026|Terror mechanic — add-and-subtract model, no max SKILL distinction               |Terror applies as skill = skill - 2 before combat, skill = skill + 2 after. No separate maxSkill variable. Medal of Valour SKILL -1 is a permanent reduction to current value — since no mechanic restores SKILL to a stored original, the distinction has no practical meaning.|
|March 2026|Loot pool confirmed as 5 items across 6 slots                                    |Underhall was a late addition as sixth slot, bringing shuffled pool to 5 items. Exactly one slot always empty each run. Underhall can carry shuffled item alongside fixed Mouldy Bread drop simultaneously.|
|March 2026|Medal of Valour added as fifth shuffled item — item code 5                       |Named for the general who became the Bound King — awarded for genuine service before his betrayal. Appears as a shiny, decorated, valuable find in search results. Effect: SKILL -1 on first pickup, applied immediately and silently. Permanent — dropping it restores nothing.|
|March 2026|Medal of Valour — pickup text register                                            |Register: 'It is heavier than it looks. Old — older than anything else down here. Decorated. Whatever it once meant, it meant something.' SKILL -1 applied silently after this line. No announcement.|
|March 2026|Medal of Valour — drop and re-take behaviour                                     |Can be dropped, stays in room, can be re-taken. medalTaken = 1 after first pickup — SKILL penalty does not fire again on re-take. Permanence revealed through silence on drop: STATS unchanged, no announcement. Register: 'You set it down. You wait for something to change.' Then nothing changes.|
|March 2026|Medal of Valour — USE behaviour                                                  |USE does nothing. One line register: 'It sits in your palm. Cold. It does not respond to you.' Costs 1 turn.|
|March 2026|Medal of Valour — greed mechanic reinforcement                                   |Sits in search results alongside helpful items. A player who grabs everything pays. Reinforces the dungeon's existing greed-punishment theme alongside the Bound King gold mechanic and the crown.|
|March 2026|Pre-game Entry Hall luck check — free roll, no LUCK decrement                    |After the Fisher-Yates shuffle, if the Medal of Valour lands in slot 1 (Entry Hall), a silent free check fires: RollDice(2) <= luck with no LUCK cost. Lucky: Medal swaps with a random helpful item (codes 1-4) from another slot. Unlucky: Medal stays. Player sees nothing. Opening sequence begins after both shuffle and check resolve silently.|
|March 2026|All item codes locked                                                            |Codes 1-5 shuffled pool: Healing Potion, Lucky Charm, Armour Shard, Dark Bread, Medal of Valour. Codes 6-10 fixed: Antidote Vial, Bangle of Courage, Sword of Sharpness, Mouldy Bread, Guardroom Key. Code 0: empty slot. Full table in Items and Loot Design section.|
|March 2026|Item pool final balance check complete                                           |Five shuffled items, six slots, one always empty. One item punishes (Medal of Valour), four help. The balance is intentional — the dungeon looks generous and is not.|

-----

## Documentation Structure

### Folder Layout

```
samples/
└── the-sunken-crown/
    ├── README.md                        ← Title page — cover image, premise, index
    ├── the-sunken-crown.sb              ← Complete game — instant gratification
    ├── HOWTOPLAY.md                     ← Player guide
    ├── LORE.md                          ← Optional backstory for those who want it
    └── listing/
        ├── issue-01-the-gates-open.md
        ├── issue-02-the-first-room.md
        ├── issue-03-the-dice-and-the-dark.md
        ├── issue-04-moving-through-stone.md
        ├── issue-05-something-in-the-dark.md
        ├── issue-06-what-you-carry.md
        ├── issue-07-the-dungeon-breathes.md
        ├── issue-08-traps-and-riddles.md
        ├── issue-09-the-bound-king.md
        └── issue-10-the-gate.md
```

### 10-Issue Arc

Modelled on INPUT magazine (Marshall Cavendish, 1984) — a weekly partwork where
each issue explained one concept and presented a typeable listing that ran immediately.
Each issue is additive. No issue leaves the program broken.

|Issue|Title                    |What Gets Built                                              |Runs As                                                 |
|-----|-------------------------|-------------------------------------------------------------|--------------------------------------------------------|
|1    |**The Gates Open**       |`PrintHeader`, `PrintSeparator`, stat display, the frame     |A working frame with placeholder stats                  |
|2    |**The First Room**       |Room description system, `PrintRoom`, contextual hints       |Entry Hall renders. GO SOUTH accepted but not yet wired.|
|3    |**The Dice and the Dark**|`RollDice`, attribute rolling, opening sequence, INPUT pacing|Full opening — roll SKILL, STAMINA, LUCK. Gates close.  |
|4    |**Moving Through Stone** |Room array, exits table, navigation loop, visited tracking   |Walk between Entry Hall, Guardroom, Armoury             |
|5    |**Something in the Dark**|Combat — `CombatRound`, attack strength, STAMINA damage      |Fight the Guardroom Brute. Win or die.                  |
|6    |**What You Carry**       |Inventory, TAKE/DROP, overburdened state, SEARCH             |Pick up the Brute’s key. Open the Armoury chest.        |
|7    |**The Dungeon Breathes** |Atmospheric events, wandering zombie, turn counter           |The dungeon feels alive. The zombie may appear.         |
|8    |**Traps and Riddles**    |Still Chamber, Riddle Room, `TestLuck`                       |Both trap rooms functional.                             |
|9    |**The Bound King**       |Final boss — Terror, Crushing Blow, gold mechanic, crown     |Throne room works end to end.                           |
|10   |**The Gate**             |Two-door ending, win/death text, end screen, play again      |The complete game. Every system connected.              |

### Issue Template

Each issue follows this structure:

1. **Header** — issue number, title, one-line hook
2. **Story beat** — why this section of the game exists and what it feels like
3. **Concept callout** — one SharpBASIC feature explained. The why, not just the what.
4. **The listing** — the code block to type in. Clean, commented, typeable.
5. **What you’ll see** — one sentence describing the exact output when run
6. **What to try** — one suggested modification. Builds curiosity.

### Cover Image

Generated with Nano Banana using Prompt A (locked). The Bound King on his throne —
armoured figure, head bowed, crown fused to temples, gold coin piled around the base,
single torch, painterly fantasy art, muted palette, no face visible.

**Nano Banana Prompt A (locked):**

```
An ancient warrior slumped on a crumbling stone throne in a torchlit dungeon
chamber. The figure is armoured, still, head bowed under the weight of a massive
tarnished gold crown encrusted with dark jewels. The crown is fused to the
temples — the metal has grown into the skin over centuries, no longer separate
from the figure wearing it. Piles of gold coin surround the base of the throne,
accumulated over hundreds of years. A single torch on the wall casts warm amber
light. The figure does not look monstrous — it looks exhausted. Ancient, still,
and enduring. No face visible — head bowed, crown dominant. The mood is heavy,
grim, and sorrowful. Illustration style: detailed painterly fantasy art, not
photorealistic, not cartoonish. Muted palette — dull gold, cold grey stone, deep
shadow, ochre torchlight. Wide shot, slightly low angle. No text, no watermarks,
no borders, no other figures.
```

**Nano Banana Prompt B (reserve — origin moment):**

```
A gauntleted hand reaching down toward an ornate tarnished gold crown resting on
cold stone. The glove is dark leather, the fingers outstretched, almost touching
the crown. The hand and forearm are splattered with fresh blood — not a wound,
the blood of battle, carried in from elsewhere. The crown sits in a shallow pool
of torchlight on a dungeon floor, ancient and massive, jewelled with dark stones.
The moment is suspended — the hand has not yet made contact. The mood is wrong
and inevitable, the last second before something irreversible. Dark, grim,
matter-of-fact. Illustration style: detailed painterly fantasy art, not
photorealistic, not cartoonish. Muted palette — cold stone, deep shadow, dull
gold, the blood the only vivid colour. Close composition, slightly overhead angle
looking down at the crown and the reaching hand. No face visible, no full figure.
No text, no watermarks, no borders.
```

-----

## Open Questions

|Question                                                              |Status                                              |
|----------------------------------------------------------------------|----------------------------------------------------|
|Finalise title: “The Sunken Crown”?                                   |**Resolved — locked, March 2026**                   |
|Finalise villain name: “Lord Malachar”?                               |**Resolved — locked, March 2026**                   |
|Win text for The Gate                                                 |**Resolved — locked, March 2026**                   |
|Item pool — final balance check against difficulty                    |**Resolved — March 2026. 5 shuffled items, 6 slots. Medal of Valour confirmed as fifth item.**|
|Zombie starting room                                                  |**Resolved — Room [4] The Crossroads, March 2026**  |
|Atmospheric event pool                                                |**Resolved — 12 events locked, March 2026**         |
|Atmospheric event frequency                                           |**Resolved — organic RND ~15% per turn, March 2026**|
|Map layout — exact rooms and connections                              |**Resolved — March 2026**                           |
|Room [5] — The Still Chamber mechanic                                 |**Resolved — March 2026**                           |
|Room [8] — identity and purpose                                       |**Resolved — Riddle Room, March 2026**              |
|Room [12] — Gate mechanic                                             |**Resolved — two-door twist, March 2026**           |
|Monster roster — 5-6 creatures with distinct mechanics                |**Resolved — March 2026**                           |
|Final boss identity — creature of legend, name TBD                    |**Resolved — The Bound King, March 2026**           |
|Lesser terror encounters — which monsters trigger partial terror?     |**Resolved — Skittering Horror only, March 2026**   |
|Poison mechanic — does a monster poison, justifying the Antidote Vial?|**Resolved — Skittering Horror poisons, March 2026**|
|UI design — layout and approach                                       |**Resolved — frame-based layout, March 2026**       |

-----

## Current Status

- [x] Concept agreed — FF-inspired, v1 showcase
- [x] Fiction and setting sketched
- [x] Core mechanics defined (attributes, combat, luck, terror)
- [x] Randomisation philosophy agreed
- [x] Feature coverage map drafted
- [x] Opening sequence designed
- [x] Map structure — 12 rooms, two routes, exits table, living dungeon
- [x] Trap rooms — Still Chamber [5] and Riddle Room [8] fully designed
- [x] Gate room [12] — two-door final twist, tell, death text, win text
- [x] Loot design — slots assigned, fixed items, shuffle pool, healing guarantee
- [x] Inventory system — soft/hard limits, overburdened penalties, TAKE/DROP
- [x] SEARCH behaviour — single pass, searched flag, already-searched text
- [x] Input mechanic — full command table with turn costs
- [x] Turn system — which commands cost turns and how many
- [x] Play length estimate — 45-75 turns typical
- [x] Atmospheric event system — 12 events, organic RND frequency, all text written
- [x] Wandering zombie — spawn room [4], wander, exclusions, death
- [x] Sneaking — all monsters except boss, tradeoff design, boss flavour text
- [x] Monster roster with mechanics
- [x] UI design — frame-based layout locked
- [x] Documentation structure — folder layout, two-track delivery, 10-issue arc, issue template
- [x] Cover image — Prompt A generated, approved, locked (Nano Banana)
- [x] Both image prompts preserved in decisions log
- [x] Item pool final balance check — complete. 5 shuffled items, 6 slots, 1 always empty. Medal of Valour confirmed.
- [x] Win text for The Gate — locked
- [x] Title and villain name finalised — The Sunken Crown, Lord Malachar
- [x] Technical Architecture Document produced — all ADRs, state variable map, call tree, flow diagrams
- [ ] SharpBASIC Phases 9 and 10 complete
- [ ] Full language sample files generated from implemented spec
- [ ] Feature coverage confirmed against game design
- [ ] VS Code TextMate grammar (.vsix)
- [ ] Full `.sb` program written
- [ ] Playtested and balanced
- [ ] Split into 10 issues
- [ ] Issue explanatory text written

-----

*Update this log whenever a decision is made or an open question is resolved.*
*Date every entry.*