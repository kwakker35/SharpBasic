# The Sunken Crown — Written Content Asset File

> **Purpose:** Single source of truth for all written content in The Sunken Crown.
> Use this file as the reference for all code generation and text pulling.
> Do not write game strings from memory — pull from this file only.
>
> **Status:** Complete. All deliverables locked as of March 2026.
> Do not edit without designer sign-off.
>
> **Contents:**
> - Deliverable 1 — Room descriptions (all 12 rooms, all state variants)
> - Deliverable 2 — Monster encounter text (7 creatures, all variants)
> - Deliverable 3 — Atmospheric event text (12 events)
> - Deliverable 4 — Command response text (10 standard strings)
> - Deliverable 5 — 10-issue explanatory text (story beats, concept callouts, what to try)
> - Deliverable 6 — Throne Room special text (post-combat beat, gold mechanic, crown sequence)
> - Deliverable 7 — Gate resolution text (win text, death text, end screen)
> - Deliverable 8 — Pre-build mechanic text (SEARCH interrupt, zombie disturbance, poison, Hollow Mage luck drain, Bound King refusals)
> - Deliverable 9 — Quit command text (monster-specific, zombie, room-specific, ambient pool, end screen)
>
> **Key conventions used throughout:**
> - State variants are labelled explicitly — (first visit), (revisit), (monster alive), (monster dead) etc.
> - Mechanical annotations are in italics and parentheses — *(STAMINA -1)*, *(conditional)* etc.
> - Placeholder references are in square brackets — *[item list]*
> - The Bound King post-combat beat and crown sequence are in Deliverable 6.
> - Gate win text and death text are in Deliverable 7.
>
> **Technical notes for code generation:**
> - File extension: `.sbx` — all SharpBASIC v1 source files use this extension
> - Language: SharpBASIC v1 — tree-walking interpreter, single file, no modules
> - UI model: frame-based layout, PRINT and INPUT only, no colour, no cursor control, no ANSI
> - All output is plain text rendered at 80 characters wide
> - Commands are normalised with UPPER$ before comparison throughout
>
> **Last updated:** March 2026 — language spec updated
>
> **Paging model:** The game targets a 30-row terminal with 20 content rows available
> after chrome (separators, header, location, exits, prompt). Text blocks that exceed
> 20 lines must insert a `[PAUSE]` marker at a natural break point. The generator
> implements each `[PAUSE]` as `CALL Pause()`. Only the win sequence currently requires
> a pause. All other blocks fit within 20 lines. If text is added or rewritten, recount.
>
> **CHR$(34) note:** All quoted speech in the win text, Bound King sequences, and
> Riddle Room death text contains double-quote characters. Since SharpBASIC v1 has
> no string escape sequences, these must be rendered in code using `CHR$(34)`.
> The text in this file shows the intended output — the code uses `CHR$(34)` to produce it.
> Example: `PRINT CHR$(34) & "You survived." & CHR$(34)` prints `"You survived."`

---

## DELIVERABLE 1 — ROOM DESCRIPTIONS

---

### Room [1] — Entry Hall

**First visit**

The hall is wide and low, the ceiling barely clearing your head. The stone is older here than anywhere you have ever stood — worn smooth by centuries of feet, most of which never came back. Torches burn in iron brackets on either wall. Someone keeps them lit. You have not seen who.

The air smells of cold and damp and something faintly animal. Behind you, the gates are closed. You heard the bar drop. You did not look back.

South is a doorway with no door. Beyond it, the sound of the dungeon settling into itself.

---

**Revisit**

You have been here before. The torches are still burning. Whatever tends them has been and gone since you passed through.

The gates are still closed.

---

### Room [2] — Guardroom

**First visit**

The room smells of old iron and something worse underneath it. Weapon racks line the far wall — empty, or nearly. Whatever was stored here has been used, or lost, or taken by someone who came before you.

A broad figure stands in the passage to the south, back to you. Big. The kind of big that moves slowly because it doesn't need to move fast. It has not noticed you yet.

There are deep gouges in the floor near the centre of the room. Something heavy was dragged here. You don't look too closely at the dark stain beside them.

Exits: north to the Entry Hall, east to the Armoury.

---

**Revisit — monster alive**

It is still here. It has turned to face the door this time.

---

**Revisit — monster dead**

The room is quieter now. The body is where you left it. The gouges in the floor catch the torchlight the same way they did before.

You have been through here already. Whatever there is to find, you know where it is.

---

**Post-fight**

The Brute is down. It fell the way big things fall — all at once, without grace. The impact is still ringing in the floor.

Your breathing is the loudest thing in the room now. The gouges in the stone are at your feet. The dark stain has company.

---

### Room [3] — Armoury

**First visit**

A dead end — or what passes for one down here. The room is narrow, the walls lined with empty brackets and bare hooks. Whatever armed the dungeon's original occupants is long gone.

Against the far wall sits a chest. Iron-banded, old, locked. The lock is substantial. Someone thought what was inside was worth protecting.

The only way out is west, back the way you came.

---

**Revisit**

The chest is as you left it — locked, or open, depending on what you did here.

The brackets on the walls are still empty. The room has nothing more to offer you.

---

### Room [4] — The Crossroads

**First visit**

Four passages meet here. The ceiling rises slightly — not comfort, just space. The floor is worn in crossing patterns where feet have moved between directions over a very long time.

North leads back toward the Guardroom. West opens into darkness that smells of dust and something crushed. East carries a faint sound you cannot quite identify. A passage angles away to the northeast.

This is the centre of the dungeon. You feel it.

---

**Revisit**

You are back at the crossroads. The passages sit where they were. The dungeon has not rearranged itself. Not here.

---

### Room [5] — The Still Chamber

**First visit**

The passage ends in a room that shouldn't be here.

It is perfectly circular. The walls are smooth in a way the rest of the dungeon is not — not worn smooth, shaped smooth, as though the stone was never anything else. There are no torches. The light comes from nowhere you can identify and illuminates nothing clearly.

There is no door. There is no exit. There is only the room, and the silence, and a heaviness behind your eyes that you notice too late to do anything about.

The floor comes up to meet you.

---

**Revisit**

You recognise it immediately. The circular walls. The sourceless light. The silence with weight in it.

You know what this room does.

Knowing does not help.

The floor comes up to meet you.

---

### Room [6] — Collapsed Passage

**First visit**

The ceiling has come down at the far end — not recently. The rubble has been here long enough to gather its own dust. Whatever this passage once connected, it no longer does. Not obviously.

The air is cold and close. Something has been moving through here. The dust on the rubble is disturbed in places, and not by your feet.

You hear it before you see it — a dry, chitinous skittering from somewhere in the dark beyond the fallen stone. Then the sound stops. Then it is very close.

---

**Revisit — monster alive, passage hidden**

The skittering starts again the moment you step through the doorway.

The rubble at the far end sits undisturbed. Whatever is buried under it, you haven't looked yet.

*Note: The passage cannot be found while the Horror is alive — SEARCH while the*
*Horror lives triggers immediate combat. This is the only valid monster-alive revisit state.*

---

**Revisit — monster dead, passage not yet found**

The passage is still. The body of the creature lies where it fell, already desiccating in the cold air.

The rubble at the far end sits as it has for decades. If there is a way through, it is not obvious. Not yet.

---

**Revisit — monster dead, passage found**

The passage is still. The body of the creature lies where it fell.

The gap in the rubble is visible from the doorway now that you know where to look. South, through the collapse, if you're ready to move on.

---

**Post-fight**

The skittering has stopped.

The creature lies where it fell, legs folded at angles they were not built to reach. The cold passage is quiet. The rubble at the far end is exactly as it was — whatever the thing was guarding, it is no longer guarding it.

---

**SEARCH result — finding the passage**

*(Only reachable when the Horror is dead. SEARCH while Horror is alive triggers combat.)*

You work through the rubble methodically. Most of it is solid — ceiling stone, heavy, immovable. But toward the base of the fall, where the collapse met the floor, there is a gap. Low, tight, passable if you commit to it.

Beyond it: a passage, heading south.

It was always there. The dungeon didn't hide it. You just had to look.

---

### Room [7] — The Pit

**First visit**

The floor drops away three feet below the entrance — you step down into the room rather than walking into it. The pit that gives the room its name is in the centre: a square shaft, iron-railed, dropping into darkness. You cannot hear the bottom.

Standing between you and the far passage is a figure in armour. Old armour — dented, repaired, repaired again. It has the posture of something that has been waiting for a long time and has no particular feelings about waiting longer.

It turns to face you with the unhurried certainty of something that has done this before.

---

**Revisit — monster alive**

It is still there. Still waiting. It turns to face you again with the same unhurried certainty.

---

**Revisit — monster dead**

The armoured figure is down. The pit sits in the centre of the room, dropping into the same darkness it always did.

South is clear.

---

**Post-fight**

The armoured figure is down at last. The old armour settles with a sound like a door closing for good.

The pit is behind you, dropping into the same darkness it always did. South is open.

---

### Room [8] — The Riddle Room

**First visit**

The door behind you is gone.

You did not hear it close. You did not feel it move. You turned and it was simply not there — solid stone where the entrance was, seamless, cold, indifferent.

Ahead of you are two doors, identical in every respect except one: a riddle is carved into the stone wall between them, deep and deliberate, cut by someone who knew exactly what they were doing.

Below the riddle, two words. One above each door.

LEFT. RIGHT.

The room waits.

---

**Revisit — already solved**

The door you chose stands open. The passage beyond is exactly as you left it. The riddle is still carved into the wall between them. The other door is sealed. There is no handle, no gap, no way through. Whatever is on the other side stays there.

---

### Room [9] — The Cistern

**First visit**

Water, somewhere below the floor. You can hear it moving through stone channels — old infrastructure, still functioning, tending to a dungeon that has long outlasted whoever built it.

The room is larger than it looks. The ceiling is vaulted, the far corners lost in shadow. Against the north wall, partially hidden by a fall of loose stone, is a recess that doesn't quite look accidental.

Something is in the room with you. You sense it before you see it — a chill that has nothing to do with the water, a faint luminescence at the edge of vision that vanishes when you look directly at it.

---

**Revisit — monster alive**

The cold finds you again the moment you step through the doorway. The luminescence flickers at the edge of vision.

It remembers you too.

---

**Revisit — monster dead**

The room is warmer without it. The water sounds are clearer now — just water, just stone, just the dungeon's old plumbing doing what it was built to do.

---

**Post-fight**

The cold leaves all at once — not gradually, just gone, the moment it stopped moving.

The water sounds come back. Channels, stone, the dungeon's old plumbing tending to itself. The recess in the north wall is still there.

---

### Room [10] — The Underhall

**First visit**

Both passages meet here. This is where the dungeon funnels everything.

The ceiling is high and the room is wide and it smells of age and something organic and wrong. The walls are lined with what you first take for discarded equipment — bundles of cloth and leather, dull metal, shapes that your eyes keep trying to resolve into something else.

They are not equipment.

They are what happens to people who made it this far and ran out of something — STAMINA, luck, time. Dry bones in old armour. The dungeon keeps them here. A record.

In the centre of the room, massive and still and watching you with small dark eyes, is the Troll.

It does not move. It is waiting to see what you do.

---

**Revisit — monster alive**

It remembers you. The small dark eyes find you immediately.

---

**Revisit — monster dead**

The Troll is down. The room is the same — the bones along the walls, the high ceiling, the smell that does not quite go away.

South is open. You are nearly through.

---

**Post-fight**

The Troll is down. Something that size takes a moment to accept. The floor still feels the weight of it.

The room is the same — the bones along the walls, the high ceiling, the smell that does not quite go away. But south is open now.

---

### Room [11] — Throne Room

**First visit**

The room is vast. After everything that came before it, the scale is wrong — ceilings that disappear into darkness above, walls lost in shadow at the edges, a floor of cracked stone buried under three hundred years of accumulated coin.

Gold. More than you have ever seen or imagined. More than you will ever be able to carry. It sits in drifts and piles around the base of the throne like a tide that came in and never went back out.

The throne is at the far end. On it, a figure. Armoured, still, head bowed under the weight of something on its brow.

Not dead. You can tell, even from here. Not dead, not sleeping. Waiting — the way only something that has been waiting for three hundred years can wait, with a patience that has long since stopped being patience and become simply the shape of its existence.

It raises its head.

---

**Revisit**

He is already looking at the door when you enter.

The gold is as it was. The throne is as it was. The figure on it is as it was — minus whatever STAMINA you took from him, already restored, already waiting.

He does not look surprised to see you again.

---

### Room [12] — The Gate

**First visit**

A plain room. Stone floor, stone walls, low ceiling. Two doors in the far wall, side by side. No decoration, no inscription, no indication of what lies beyond either of them.

After everything that brought you here, the ordinariness of it is its own kind of wrong. You stand in the doorway and wait for something to happen. Nothing does. The room offers nothing — no sound from beyond the doors, no draft, no sign. Just two doors in a plain room at the end of a dungeon that has been trying to kill you since you stepped into it.

You keep waiting. The silence holds.

Eventually you realise the dungeon has already made its last move. It just looks like a room.

You step forward and make your choice.

---

*No revisit text — the Gate resolves the run.*

---

## DELIVERABLE 2 — MONSTER ENCOUNTER TEXT

---

### Guardroom Brute — Room [2]

**First encounter**

It hears you before you reach it. The broad shoulders shift, the head turns, and then it is facing you — fully, deliberately, with the unhurried attention of something that has done this many times and found it straightforward.

It is very large. The weapon in its hand looks like it was made for someone even larger.

It moves first.

---

**Revisit — monster alive**

It remembers you. There is no hesitation this time, no moment of assessment. It simply turns and comes.

---

**Death text**

It goes down slowly, the way large things do — first to one knee, then forward, the weapon dropping before the rest of it follows. The floor shakes once when it lands.

The room is considerably quieter.

---

### Skittering Horror — Room [6]

**First encounter**

You hear it first — a dry, rapid skittering from somewhere in the dark beyond the rubble. Then silence. Then it is in the room with you and you understand, viscerally and immediately, why silence followed the sound.

It is fast. Faster than anything that size has any right to be. The chitinous plates catch the torchlight in fragments as it moves and it is already moving toward you before you have fully registered what you are looking at.

The darkness and the sound and the speed of it hit you somewhere older than thought. Your hands are cold. Your grip tightens.

Round one. Don't let it rattle you.

---

**Revisit — monster alive**

It comes from the same direction as before. Faster this time, or perhaps you are simply more aware of how fast it actually is.

The cold feeling in your hands is back. You recognise it now. That helps, slightly.

---

**Death text**

It drops mid-movement, skidding across the stone floor before coming to rest against the far wall. The chitinous legs fold inward, one after another, until it is still.

You stand over it for a moment. The cold in your hands fades slowly.

---

### Pit Guardian — Room [7]

**First encounter**

It turns to face you without urgency. The armour it wears has been repaired so many times the original shape is barely discernible — layer over layer over layer, each repair a testament to something that has been hit repeatedly and kept going.

It raises its weapon with the economy of motion of something that does not waste effort. It has assessed you. It has found you worth engaging. That is all the acknowledgement you are going to get.

---

**Revisit — monster alive**

It is already facing the door when you enter. It learned that from last time.

---

**Death text**

The armour clatters against the stone in sections as it falls — the sound of something that has held together through a great deal finally coming apart all at once.

It does not get up. Whatever kept it going, you have taken enough of it.

---

### Hollow Mage — Room [9]

**First encounter**

You see the light before you see the source — a faint, cold luminescence at the edge of vision that keeps refusing to be where you look for it. Then it stops moving and you see it properly.

What remains of the Mage is not quite solid and not quite absent. It occupies the space between the two in a way that makes your eyes work harder than they should. The cold in the room is its cold. It has been here a long time. It is not pleased to have company, and it intends to make that clear.

You feel something drain from you before the fight has even begun. Your luck. It is taking your luck.

---

**Revisit — monster alive**

The cold finds you the moment you cross the threshold. The luminescence steadies, and fixes on you, and the draining feeling starts again.

It has been waiting.

---

**Death text**

It does not fall so much as diminish — the luminescence pulling inward, flickering, and then gone. The cold lifts from the room slowly, as though the stone itself needs time to remember what warmth is.

The silence that follows is a different quality of silence than before.

---

### Troll — Room [10]

**First encounter**

It does not move when you enter. It watches you from the centre of the room with small, dark, patient eyes, and it waits to see what you will do.

It is enormous. The regenerative capacity of its kind is not something you can see, but you know it is there — the old wounds on its hide that have closed and scarred and closed again, the thickness of it, the sense that what you take from it will not stay taken without sustained effort.

You are going to have to be fast, or you are going to have to be lucky, or you are going to have to be both.

It has decided you are worth its attention. It stands.

---

**Revisit — monster alive**

The small dark eyes find you as soon as you step through. It was already standing.

---

**Death text**

It comes down like something structural failing — a slow, inevitable collapse that shakes the floor when it completes. The dark eyes go flat.

You watch it for a moment longer than necessary. With a Troll, you make sure.

It is sure.

---

### Bound King — Room [11]

**First encounter**

He raises his head.

That is all. He does not stand, does not speak, does not reach for a weapon. He simply raises his head and looks at you across three hundred years and a room full of other people's gold, and the weight of that look is a physical thing.

The crown is fused to his temples in a way that stopped being jewellery a very long time ago. The metal has grown into the skin, or the skin has grown into the metal — after three centuries the distinction has ceased to matter. He wears it because there is no longer a version of him that doesn't.

He looks like a man in his forties. Tired in a way that has nothing to do with sleep.

He stands. He is very still for a moment. Then he comes toward you, and there is nothing in his movement that suggests he expects this to end differently than it always has.

Without the Bangle of Courage: the full weight of what he is lands on you before he takes three steps. Your SKILL falters. Your luck feels suddenly thin.

---

**Revisit — second fight**

He is already on his feet when you turn back.

The gold you took is still in your hands. He looks at it, then at you. Whatever he feels about it does not reach his face.

He comes toward you again. Same certainty. Same weight. He has done this before. So, now, have you.

---

**Death text**

*[As written and locked in the design log — the Bound King's post-combat beat is not reproduced here.]*

---

### Wandering Zombie — Roaming

**First encounter**

You almost walk into it.

It is moving through the dungeon with the directionless persistence of something that has forgotten what it was looking for but has not forgotten how to look. It registers your presence a half-second after you register its, and that half-second is the worst part — the moment before it turns, when you can still pretend it hasn't noticed you.

It has noticed you.

---

**Revisit — monster alive**

It is here again. Or still. With the zombie it is difficult to say which.

It turns toward you with the same directionless persistence as before. It has not learned anything. It does not need to.

---

**Death text**

It goes down and stays down. Whatever was animating it has spent itself.

The dungeon is quieter without it moving through the passages. You are not sure if that is better.

---

*End of locked content — Deliverables 1 and 2.*

---

## DELIVERABLE 3 — ATMOSPHERIC EVENT TEXT

---

### Pure Flavour Events

---

**1. The Torch**

Your torch gutters. For a moment the darkness is absolute — a living, pressing thing. Then the flame catches again. You breathe out. You hadn't noticed you'd stopped.

---

**2. The Sound**

Somewhere below you — deeper than you've been — something moves. A slow, rhythmic scraping. Stone on stone. It stops. You wait. It doesn't start again.

---

**3. The Wall**

You place your hand against the wall to steady yourself. The stone is warm. Not the warmth of your own body heat — it was warm before you touched it. You take your hand away.

---

**4. The Air**

The air changes. A cold current from nowhere, carrying the smell of standing water and something older. Something that hasn't seen light in a very long time.

---

**5. The Bones**

You almost step on them. Small. Difficult to identify. You don't try.

---

**6. The Silence**

The dungeon goes completely quiet. No drip of water, no distant groan of settling stone. Nothing. It lasts perhaps ten seconds. It feels much longer. Then the sounds return, and you realise how much you'd been relying on them.

---

**7. The Marks**

Scratches in the wall at eye height. You lean close. Letters, cut deep and fast by someone who knew they were running out of time. Three words. The middle one is illegible. The first is *do not* and the last is *alone*.

---

### Mechanical Events

---

**8. The Weight** *(STAMINA -1)*

The cold gets into you. Not the sharp cold of wind — the slow cold of deep stone, of places that have never been warm. You feel it in your chest first.

---

**9. The Hunger** *(STAMINA -1)*

You haven't eaten. You'd forgotten until now. Your body reminds you with a clarity that's difficult to ignore.

---

**10. The Stumble** *(conditional)*

Your foot catches on uneven stone.

*If overburdened:* The weight pulls you down hard. You catch yourself on one knee. It costs you. *(STAMINA -1)*

*If not overburdened:* You catch yourself easily. A reminder that the floor is not your friend.

---

### Threat Escalation Events

---

**11. The Zombie Spawn Check**

*[No flavour text — fires the spawn probability check invisibly. If the zombie spawns, the next room description carries the weight of that.]*

---

**12. The Presence**

*If zombie is alive and within 2 rooms of player:*

Something is moving in the dungeon. Not far away. The sound has direction.

*If zombie is dead or not yet spawned:*

You have the distinct feeling you are not alone. You stand very still. Whatever it was, it passes.

---

*End of locked content — Deliverables 1, 2, and 3.*

---

## DELIVERABLE 4 — COMMAND RESPONSE TEXT

---

**1. Unknown command**

The dungeon does not respond to that.

---

**2. No exit that way**

There is no way through in that direction.

---

**3. Overburdened warning on GO**

You move, but the weight tells on you. Everything costs more when you are carrying this much.
*(STAMINA -1)*

---

**4. Overburdened warning on TAKE**

You are already carrying more than you should. You can take one more item, but you will feel it.

---

**5. Already searched**

You have already been through everything this room has to offer.

---

**6. Nothing to fight**

There is nothing here that requires that.

---

**7. Sneak success**

You hold your breath and move carefully. It does not see you. The moment passes. You are through.

---

**8. Sneak failure**

Not careful enough. It turns before you are clear. There is no choice now but to face it.

---

**9. Boss sneak attempt**

You press yourself against the wall and inch toward the door. The creature's eyes find you immediately. Some things cannot be avoided.

---

**10. TAKE ALL when full**

You cannot carry any more. You have reached your limit.

*Left behind:* [item list]

Use TAKE [item] to choose what matters most.

---

*End of locked content — Deliverables 1, 2, 3, and 4.*

---

## DELIVERABLE 5 — 10-ISSUE EXPLANATORY TEXT

---

### Issue 1 — The Gates Open

**Story beat**

Before the dungeon has rooms or monsters or choices, it needs a face. The first thing the player sees is not gameplay — it is atmosphere. A header bar. A separator. Numbers that mean something. The frame that will hold everything that follows.

This is where SharpBASIC announces itself. A handful of PRINT statements, carefully arranged, and suddenly there is a dungeon on the screen. It doesn't do anything yet. It doesn't need to. The feeling of it is enough to start.

**Concept callout — PRINT and string concatenation**

SharpBASIC's `PRINT` statement is the language's only way to speak to the player. Everything the dungeon says — every room description, every combat result, every atmospheric whisper — comes through `PRINT`.

Strings are joined with `&`. This is how you build a line from moving parts:

```
PRINT "SKILL: " & skill & "  STAMINA: " & stamina & "  LUCK: " & luck
```

The `&` operator calls `.ToString()` on whatever it touches, so numbers become text automatically. You don't need to convert them first. The language handles it.

The separator line — eighty characters of `=` — is built the same way, repeated in a loop. One character, eighty times. SharpBASIC doesn't have a string repeat function. You build what you need.

**What to try**

Change the width of the frame. The listing assumes 80 characters — the classic terminal width. What happens if you make it 60? What breaks, and what do you need to fix to make it hold together at a different size?

---

### Issue 2 — The First Room

**Story beat**

The frame exists. Now something has to go inside it. The Entry Hall is the simplest room in the dungeon — no monster, no trap, no decision beyond which way to go. That simplicity is deliberate. The first room is where the player learns to read the game before the game starts asking things of them.

Writing the room description and putting it on screen is the moment SharpBASIC stops being a program and starts being a place.

**Concept callout — SUBs**

The room description is printed by a SUB — a named block of code you can call by name from anywhere in the program.

```
SUB PrintRoom(roomId AS INTEGER)
    IF roomId = 1 THEN
        PRINT "The hall is wide and low..."
    END IF
END SUB
```

SUBs take typed parameters. `roomId AS INTEGER` means the SUB expects a whole number and will use it to decide what to print. Call it with `CALL PrintRoom(1)` and the Entry Hall appears. Call it with `CALL PrintRoom(2)` and the Guardroom appears instead.

This is the architecture the entire game is built on. Every room, every combat frame, every inventory screen goes through a SUB. Learn the shape of it here, in the simplest room, before the complexity arrives.

**What to try**

Add a second room description — write the Guardroom text and add it as `IF roomId = 2 THEN` inside `PrintRoom`. To test it, temporarily change `CALL EnterRoom(1)` to `CALL EnterRoom(2)` in the top-level code. Run the program. The Guardroom renders. Change it back when you are done. This is exactly how the rest of the game gets built.

---

### Issue 3 — The Dice and the Dark

**Story beat**

The dungeon doesn't know who you are yet. Neither do you. Before the gates close, you roll — three attributes that will determine whether you survive the next hour, and you have no say in any of them. That is Fighting Fantasy's founding bargain: the dice decide what you bring in, and skill decides what you do with it.

The opening sequence is also where SharpBASIC first talks to you directly. Not just printing — waiting. Asking. Giving you a moment before the bar drops.

**Concept callout — FUNCTIONs and RND**

A FUNCTION is like a SUB, but it gives something back. `RollDice` is the most important FUNCTION in the game:

```
FUNCTION RollDice(n AS INTEGER) AS INTEGER
    LET total = 0
    FOR i = 1 TO n
        LET total = total + INT(RND() * 6) + 1
    NEXT i
    RETURN total
END FUNCTION
```

`RND()` returns a random number between 0.0 and 1.0. Multiply by 6, take the floor with `INT`, add 1, and you have a die roll. Call `RollDice(2)` and you get the sum of two dice. Call `RollDice(1)` and you get one.

Every random thing in this game — every combat round, every luck test, every monster stat — flows through this single FUNCTION. One source of truth for all randomness. If something behaves unexpectedly, this is the first place to look.

**What to try**

Run `RollDice` a hundred times and print every result. Does the distribution look right? You are looking for roughly equal frequency across 1–6 for a single die, clustering toward 7 for two dice. If it doesn't look right, the rest of the game won't feel right either.

---

### Issue 4 — Moving Through Stone

**Story beat**

The dungeon is not a description. It is a place you move through. This is the issue where SharpBASIC stops printing rooms and starts connecting them — where north means something, and south means something different, and the map becomes real.

Navigation is the skeleton of the game. Everything else hangs off it.

**Concept callout — Arrays**

The dungeon's rooms and exits live in arrays — fixed-size collections of values, each slot numbered from zero.

```
DIM roomExitCount[12] AS INTEGER
DIM exitDest[30] AS INTEGER
DIM exitDir[30] AS INTEGER
```

`roomExitCount[4]` holds the number of exits from room 4. `exitDest[7]` holds the destination of exit slot 7. The navigation handler reads these arrays to decide where GO NORTH takes you.

Arrays are how the dungeon knows its own shape. Without them, every room connection would be a separate IF statement — dozens of them, impossible to maintain. With them, the entire map lives in a handful of initialisation lines and the navigation handler is clean and general.

`DIM` fixes the size at declaration. SharpBASIC checks every access against that size. Go outside it and the interpreter tells you immediately. This is the safety net that keeps a bug from silently corrupting the map.

**What to try**

Add a new connection — make room 1 have a west exit that leads back to room 1. A room that loops back to itself. Does the navigation handler cope? Does anything break? Understanding what the code assumes about the map is as valuable as understanding what it guarantees.

---

### Issue 5 — Something in the Dark

**Story beat**

The Guardroom Brute is the first thing in the dungeon that wants to hurt you. Everything up to this point has been atmosphere and preparation. Now it is personal.

Combat is the game's central tension — two attack strengths compared each round, STAMINA falling on both sides, the question of when to stop being a problem the player hasn't had to face yet. This is where that starts.

**Concept callout — WHILE loops**

Combat runs in a loop. The loop continues while both sides are still standing:

```
WHILE playerStamina > 0 AND monsterStamina > 0
    LET playerAttack = AttackStrength(skill)
    LET monsterAttack = AttackStrength(monsterSkill)
    ' compare, apply damage, print result
WEND
```

`WHILE` evaluates its condition before every iteration. The moment either STAMINA hits zero the loop ends — the fight is over. What happens next depends on which side ran out first.

The condition uses `AND` — both sides must be true for the loop to continue. If either goes false the dungeon moves on. This is how SharpBASIC thinks about compound conditions, and you will use this pattern throughout the game.

**What to try**

Make the Brute hit harder — raise its SKILL by 3 and fight it again. Then lower yours by 3 instead. The same combat loop, the same code, a completely different experience. The numbers are the game. Understanding how sensitive the balance is will inform every monster stat in the dungeon.

---

### Issue 6 — What You Carry

**Story beat**

You can't take everything. The dungeon makes you choose — and every choice to take something is implicitly a choice about what you might have to leave behind later. Inventory is the game's resource management, and the moment you hit four items you feel it immediately in every room you move through and every fight you enter.

The Armoury chest is the first locked thing. You need the Brute's key to open it. Cause and effect, threaded through the dungeon's systems.

**Concept callout — Parallel arrays and item tracking**

Inventory is stored as an array of item codes:

```
DIM inventory[4] AS INTEGER
LET invCount = 0
```

Each slot holds a number. Zero means empty. One means Healing Potion. Seven means Bangle of Courage. The item is just a number — everything about what that number means lives in the code that reads it.

This is parallel array design. `inventory[1]` holds what's in slot one. `invCount` tracks how many slots are filled. `overburdened` is set to 1 when `invCount = 4`. Three arrays, one concept, kept in sync by the TAKE and DROP handlers.

Keeping parallel arrays in sync is one of the most common sources of bugs in this kind of program. If you add to `inventory` without incrementing `invCount`, or decrement `invCount` without clearing the slot, the game's accounting breaks silently. The discipline of updating all of them together, every time, is what keeps it honest.

**What to try**

Drop an item and pick it up again. Check `invCount` before and after. Does it return to exactly where it started? If it doesn't, you have a sync bug. Finding it now, in this simple case, is far easier than finding it later when three other systems depend on the count being right.

---

### Issue 7 — The Dungeon Breathes

**Story beat**

The dungeon is not waiting for you. It has its own rhythms — the torch that gutters, the sound from somewhere below, the feeling that something is moving in the passages while you are standing still. These things happen whether you are ready for them or not.

The zombie is the most direct expression of this. It does not know where you are. It does not care. It simply moves, turn by turn, through the same passages you use — and eventually, if you linger, your paths cross.

**Concept callout — The turn counter and RND-driven events**

Every time-consuming action increments a global counter:

```
LET turns = turns + 1
```

After each increment, the event system runs a probability check:

```
IF RND() * 10 > 8.5 THEN
    CALL AtmosphericEvent()
END IF
```

Roughly 15% of turns trigger an event. Not every turn — that would feel mechanical. Not never — that would feel empty. The randomness makes the dungeon feel alive without making it feel scripted.

The zombie uses the same turn counter but moves on a deterministic rule: every turn after spawning, it picks a random valid exit and moves. It has no memory, no pathfinding, no awareness of you. It wanders. The encounter happens when the numbers align — your position and its position, the same room at the same time.

Two systems, one counter, emergent behaviour neither system was designed to produce alone.

**What to try**

Set the atmospheric event threshold lower — change `8.5` to `5`. Events fire roughly half the time now. Play through the first few rooms. Does it feel different? Does the dungeon feel more alive or just noisier? The threshold is a design decision as much as a code decision.

---

### Issue 8 — Traps and Riddles

**Story beat**

Not everything in the dungeon fights you. Some things simply wait, patient and indifferent, for you to make the wrong choice. The Still Chamber takes your consciousness before you can act. The Riddle Room locks the door behind you and presents a problem that only your own intelligence can solve.

These rooms feel different from everything else in the dungeon because they require different things from you. The combat system is irrelevant here. STAMINA doesn't help. What matters is attention — whether you noticed the right things, whether you thought before you acted.

**Concept callout — IF / ELSE chains and INPUT**

The Riddle Room's inner loop accepts only two answers:

```
INPUT cmd$
LET cmd$ = UPPER$(cmd$)
IF cmd$ = "LEFT" THEN
    ' resolve
ELSE
    IF cmd$ = "RIGHT" THEN
        ' resolve
    ELSE
        PRINT "The door does not move. The room waits."
    END IF
END IF
```

`UPPER$` normalises whatever the player types — `left`, `Left`, and `LEFT` all become `LEFT` before the comparison. The ELSE branch catches everything else and holds the player in the room. The loop continues until a valid answer is given.

This is the same INPUT and IF pattern used throughout the game, applied to a situation where the stakes of getting it wrong are immediate and permanent. The code is simple. The consequence is not.

**What to try**

Add a third answer — `EXAMINE` — that prints the riddle text again without advancing the loop or costing a turn. Does it feel fairer? Does it change the tension? There is no right answer. It is a design question, and how you feel about it tells you something about what kind of game you are making.

---

### Issue 9 — The Bound King

**Story beat**

He has been in that room for three hundred years. He has been beaten before. He will be beaten again. He will be here long after you are gone, long after the dungeon has forgotten your name, patient and furious and unable to leave.

The Throne Room is the culmination of everything the game has built — the highest stakes combat, the gold mechanic that punishes greed, and the crown that sits in plain sight and offers something the game has never offered before: a choice with no mechanical protection against making the wrong one.

**Concept callout — Nested loops and state flags**

The gold mechanic runs inside the combat resolution — a loop within a loop, each bag taken triggering a regeneration check that could restart the fight:

```
WHILE goldBags < 4 AND wantsMoreGold = 1
    ' prompt player
    ' roll for regeneration
    IF regenRoll >= threshold THEN
        LET secondFight = 1
        ' restart combat loop
    END IF
WEND
```

State flags — `secondFight`, `crownAvailable`, `goldBags` — track what has happened so the rest of the code can respond to it. The crown appearing in SEARCH results is controlled by a single flag set after combat ends. The second fight uses the same combat loop as the first, with `secondFight = 1` suppressing the gold collection that follows it.

Complex behaviour from simple flags, composed carefully. This is how the whole game works. The Throne Room just has more of it in one place than anywhere else.

**What to try**

Take all four bags every time you reach the Throne Room. Track how often the regeneration triggers. Does four bags feel like a genuine gamble or a near-certainty? If it feels too safe, lower the threshold. If it feels suicidal, raise it. The numbers in the design are a starting point, not a verdict.

---

### Issue 10 — The Gate

**Story beat**

Two doors. No label. No instruction. The dungeon's last move, disguised as an unremarkable room.

Everything the game has built — the atmosphere, the mechanics, the careful withholding of information — comes down to this. Whether the player noticed the right thing. Whether they thought before they reached.

This is the complete game. Every system connected, every piece of text in place, every choice with a consequence. It is finished. Run it.

**Concept callout — Putting it together**

There is no new SharpBASIC feature in this issue. The Gate uses INPUT, IF, and PRINT — the same tools as Issue 1. What has changed is everything around them.

The complete game is a single `.sbx` file. No modules, no imports, no external dependencies. SharpBASIC's scope — ten phases, one file, a real runnable language — was always pointed at this moment. The constraint was the point.

What you have built is a tree-walking interpreter executing a program that tells a story. Every PRINT statement passes through a lexer, a parser, an AST, and an evaluator before a character appears on screen. You built all of those too. The dungeon runs on an engine you made by hand.

That is not a small thing.

**What to try**

Change the ending. Not the mechanics — the text. The win text, the death text, the end screen. Make it yours. The content is locked for the published version. Your copy is not. A game you have modified, even slightly, is a game you understand differently than one you only played.

---

---

## DELIVERABLE 6 — THRONE ROOM SPECIAL TEXT

---

### Post-Combat Beat

*The King falls. You don't wait. You can already see his fingers moving.*

---

### Regeneration Beat — Each Bag After the First

Three lines delivered sequentially as prompts. The player decides when to stop.

*His fingers move.*

*His hand closes into a fist.*

*He draws a breath.*

---

### TAKE ALL Refused in Throne Room

*You don't have time for that.*

---

### Crown Search Result

*Beneath the piled coin and the fallen king you see it clearly for the first time.
The crown. Solid gold, heavily jewelled — rubies, sapphires, stones you don't have
names for. Nothing else in this room comes close. Nothing you have ever owned comes
close. One item. Carried out of here and you never work another day. You never fear
another debt. You never need anything from anyone again.*

*It is still on his head. His head is right there.*

---

### Crown Sequence — TAKE CROWN

Fires immediately on TAKE CROWN regardless of inventory state. The crown never enters inventory.

*You pick it up. You don't mean to put it on. You do anyway.*

*It fits perfectly. It always did.*

*The Bound King opens his eyes.*

*He looks at you for a long moment. Then he looks at his own hands — empty,
unburdened, free for the first time in three hundred years.*

*He stands. He walks past you without a word. You hear his footsteps on the stairs.*

*You try to follow. Your feet will not move.*

*You are still there when the torch goes out.*

---

### Crown End Screen

```
You are the Bound King now.

Play again? (YES / NO)
```

---

## DELIVERABLE 7 — GATE RESOLUTION TEXT

---

### Gate Death Text — Wrong Door

*You walk through the door into yet another damp stone-lined corridor.
You walk on, exploring the ever more twisting path. Your torch finally
stutters and dies, leaving you to roam the pitch black halls and corridors
until you eventually succumb to starvation, or something that was already
down here finds you first.*

---

### Gate Win Text — Correct Door

*The door opens onto grey sky.*

*You don't move at first. You stand in the doorway and breathe. Cold air. Real air. The smell of mud and grass and distance. Things that have nothing to do with stone and dark and death.*

*Your legs carry you forward because they have forgotten how to stop.*

*There is no crowd. There never is. The competition is not a spectacle — it is a disposal.*

*Lord Malachar is waiting in the courtyard. Six guards flank him. You notice, with the detached clarity of someone who has spent the last hours keeping themselves alive by noticing things, that any one of them could kill you where you stand. You have nothing left. Your hands are shaking. You hadn't noticed until now.*

*He looks at you for a long moment. His expression does not change.*

*Then he steps forward. Alone. The guards don't move.*

*[PAUSE — call Pause() here before continuing]*

*"You survived."*

*It is not a question. It is not a compliment. It is the pronouncement of something that was not supposed to happen, delivered by a man who has made his peace with occasionally being wrong.*

*His eyes drop to the gold in your hands. The gold that was never his to give — it belonged to the dungeon, and the dungeon let you take it.*

*He steps aside.*

*"Go."*

*You go.*

*You are alive. You are free. You are one of almost none.*

---

### Win End Screen

```
You survived The Sunken Crown.
You escaped with X bag(s) of gold.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

---

---

## DELIVERABLE 8 — PRE-BUILD MECHANIC TEXT

---

### Poison Room Entry Message

*The poison spreads further through your blood. You feel it costing you.*

*(STAMINA -1 fires after this line)*

---

### Hollow Mage — Luck Drain at Zero

*You feel the last of your luck leave you. Whatever happens next, it happens without fortune's favour.*

*(Luck floors at zero. This line prints once when the drain fires and luck is already at zero. Silent thereafter.)*

---

### Bound King — TAKE Before Combat Refused

*(Fires when player attempts TAKE BAG or TAKE CROWN while the Bound King is still alive.)*

*The King sits between you and the gold.*

---

### Bound King — SNEAK Refused

*He is already looking at you.*

---

### Bound King — SEARCH Refused

*Not here. Not with him watching.*

---

### SEARCH Interrupt — Fixed Monsters

*These lines fire when the player attempts SEARCH while the named monster is alive in the room. Combat triggers immediately after the line prints. SKILL -1 applies to round 1, additive with any existing Lesser Terror.*

---

**Guardroom Brute — Room [2]**

*The Brute hears the scrape of your boot before you've taken a step toward the wall. It turns with the speed of something that has been waiting for an excuse. You are out of time.*

---

**Skittering Horror — Room [6]**

*The moment your hand touches the rubble the Horror is already moving. Not toward the sound — toward you. It has been still this whole time. It was never unaware.*

---

**Pit Guardian — Room [7]**

*The Guardian does not hurry. It simply turns, raises its weapon, and begins walking toward you with the unhurried certainty of something that has done this before. You had your back to it. That was your mistake.*

---

**Hollow Mage — Room [9]**

*The cold in the room doubles before you have touched anything. The luminescence fixes on you — steady now, no longer drifting. It knew the moment your attention shifted. It was waiting for exactly this.*

---

**Troll — Room [10]**

*The small dark eyes find you the instant you crouch. It doesn't roar. It doesn't rush. It simply stands — and keeps standing, and keeps coming, with the patient certainty of something that has never needed to hurry.*

---

### SEARCH Interrupt — Wandering Zombie

*These lines fire when the player attempts SEARCH while the zombie is in the room. Six options selected at random, each grounded in what is physically present in the room the zombie currently occupies. Combat triggers immediately after the line prints. SKILL -1 applies to round 1.*

*The zombie does not think. It hears disturbance and moves toward it. The lines convey a predator responding to sound, not a creature reacting with intelligence.*

---

**Room [1] — Entry Hall**
*(torch brackets, stone floor, iron gates)*

1. *The scrape of your boot on stone is enough. The zombie turns toward the sound with the absolute certainty of a thing that has nothing else to do.*
2. *You disturb the silence and the zombie disturbs you. It was closer than you thought.*
3. *The creak of an iron bracket above you is all it takes. The zombie is already moving.*

---

**Room [2] — Guardroom**
*(weapon racks, gouged floor, dark stain)*

1. *Your hand catches the edge of an empty weapon rack. The clatter fills the room. The zombie's head swings toward it before the sound has finished.*
2. *A boot finds the uneven stone near the dark stain. The sound is small. The zombie's response is not.*
3. *The weapon rack shifts as you lean past it. One noise. Enough.*

---

**Room [3] — Armoury**
*(iron brackets, bare hooks, the chest)*

1. *A hook swings free from the wall as your shoulder catches it. The sound rings off the stone. The zombie turns with mechanical precision.*
2. *The chest lid drops an inch under your hand. The sound is sharp and final. The zombie is already facing you.*
3. *Iron on stone — your fumbling dislodges something from a bracket. The zombie does not hesitate.*

---

**Room [4] — The Crossroads**
*(bare stone, four passages, worn floor)*

1. *A loose stone shifts under your weight. In the silence of the crossroads it might as well be a shout. The zombie turns.*
2. *Your search disturbs the dust that has settled here for decades. The zombie hears something in the disturbance you cannot.*
3. *You scrape a heel on the worn floor. The sound carries down all four passages. The zombie chooses yours.*

---

**Room [6] — Collapsed Passage**
*(rubble, fallen stone, dry bones)*

1. *The rubble shifts under your hands — stone against stone, loud and irreversible. The zombie was on the other side of the collapse. It is no longer.*
2. *A fragment of bone cracks under your knee. You freeze. The zombie does not.*
3. *The fallen stone grinds as you test it. One sound. The zombie's head turns toward it with the patience of something that has been listening for exactly this.*

---

**Room [7] — The Pit**
*(iron rail, stone floor, the shaft)*

1. *Your hand finds the iron rail and the clang of metal rings out across the pit. The zombie turns toward it with predator's instinct.*
2. *A loose stone skitters toward the shaft. You watch it fall. The zombie watches you.*
3. *The rail vibrates under your grip. Enough vibration. Enough sound. The zombie is already moving.*

---

**Room [9] — The Cistern**
*(stone channels, vaulted ceiling, loose stone recess)*

1. *The loose stone shifts as you reach behind it. The sound of it is nothing — a whisper in the dark. The zombie hears whispers.*
2. *Water drips from your hand onto the channel cover. The tap of it against stone is enough. The zombie's head swings toward you.*
3. *The recess scrapes as you reach into it. Stone on stone, quiet and unmistakable. The zombie does not miss it.*

---

**Room [10] — The Underhall**
*(dry bones along walls, old armour, high ceiling)*

1. *Your foot finds a loose piece of old armour. The rattle of it fills the room and the zombie turns with absolute certainty — not toward the sound, toward you.*
2. *Dry bones shift as you crouch near the wall. The sound is faint. The zombie does not need loud.*
3. *A gauntlet shifts under your hand. Iron on stone, barely audible. The zombie was already turning before it finished.*

---

### Opening Sequence — Narrative Text

*These lines print during the opening sequence before attribute rolling.
They are not room descriptions — they run once before the dungeon begins.*

---

*You have sold everything. Your home, your tools, what little remained after the
debts. Malachar's men took it all — converted to coin, dropped down a chute into
the dark beneath the keep. You watched it go.*

*If you don't walk out of that dungeon, there is nothing to walk back to.*

---

### Still Chamber — Lucky Wake Text

*You open your eyes. Stone ceiling. Cold floor. You are alive and — remarkably — unharmed. Whatever this place did to you, it could have done worse.*

---

### Still Chamber — Unlucky Wake Text

*You open your eyes. Stone ceiling. Cold floor. You curse under your breath. Of all the places to wake up.*

---

### Riddle Room — Holding Response (invalid input)

*The door does not move. The room waits.*

---

### Riddle Room — LUCK Command Refused

*There is no fortune to invoke here. The dungeon offers you the riddle and nothing else.*

*(LUCK is not consumed. The player returns to the riddle prompt.)*

---

### Riddle Room — Wrong Door Death Text

*You pass through the door into a room straight from your worst nightmare. The mouldy bones of previous adventurers litter the floor. Realising your mistake you turn to leave — only to find the door has no handle. On closer examination you notice bloody scratches, and is that a fingernail?*

*A deep rumble shakes your body and turns your bowels to water. You look up to see the ceiling, slow but inexorably, lowering.*

*You frantically try to open the door, adding to the bloody legacy, as the room crushes the life out of you.*

---

### Already Searched — Second Search Response

*You go through the room again. You find nothing you missed the first time.*

*(Note: this replaces the earlier Deliverable 4 string 5 which read "You have already been through everything this room has to offer." The design log version above is the authoritative text.)*

---

### Combat Tension Lines

*These lines fire inside CombatLoop based on STAMINA thresholds. Each fires once
per combat encounter — a flag inside CombatLoop tracks whether each has fired
this fight. They do not fire again if STAMINA fluctuates back above the threshold.*

---

**Player STAMINA ≤ 4 — all combat:**

*You are barely standing. So is your chance of walking out of here.*

---

**Player STAMINA ≤ 2 — all combat:**

*You are one blow from the end. You both know it.*

---

**Monster STAMINA ≤ 4 — all combat:**

*You can feel the momentum shifting. It is yours to lose now.*

---

**Monster STAMINA ≤ 2 — all combat:**

*It is nearly done. Don't give it a way back.*

---

### Crushing Blow — Bound King Only

*(Fires when the Bound King wins a round by 3 or more attack strength. isBoss = 1 only.)*

*The blow lands with the weight of three hundred years of experience. It takes you to one knee and you feel something break inside.*

---

### General Low STAMINA — Outside Combat

*(Fires on room entry when player STAMINA ≤ 4. Once per room visit. Not combat-specific.)*

*You have paid dearly just to be standing here. You drive your battered body on.*

---

### Mouldy Bread — Troll Drop Description

*Beneath the body you find a heel of bread, wrapped in cloth. It has seen better days. It has seen better decades. But it is food.*

*(Appears in SEARCH results in room [10] after Troll is dead. Restores 3 STAMINA on USE.)*

---

### End State Messages

**endState = 1 — Win via Gate**
*(Full win text in Deliverable 7. End screen:)*
```
You survived The Sunken Crown.
You escaped with X bag(s) of gold.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

**endState = 2 — Took the Crown**
*(Crown sequence text in Deliverable 6. End screen:)*
```
You are the Bound King now.

Play again? (YES / NO)
```

**endState = 3 — Wrong answer in Riddle Room**
*(Wrong door death text above fires first. End screen:)*
```
The ceiling lowered. The room did not care.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

**endState = 4 — Wrong door at Gate**
*(Gate death text in Deliverable 7 fires first. End screen:)*
```
The torch went out. The dungeon kept you.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

**endState = 5 — STAMINA reached zero**
```
Your STAMINA reached zero.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

---

---

## DELIVERABLE 9 — QUIT COMMAND TEXT

---

> QUIT is available at any prompt. No confirmation. It sets `gameOver = 1`
> and `endState = 6`. The death text is context-sensitive — monster/zombie
> checks first, then room-specific, then the ambient pool for everything else.
>
> Priority order in `HandleQuit()`:
> 1. Monster alive in current room → monster-specific finish
> 2. Zombie alive and in current room → zombie finish
> 3. Room-specific special case (Pit, Cistern, Riddle Room, Gate)
> 4. Random ambient pool (6 variants)

---

### Monster-Specific Quit Lines

**Guardroom Brute (Room 2):**

You lower your weapon. The Brute watches you do it. Then it does what it was always going to do — efficiently, without interest, the way it has ended everything else that came through that door.

---

**Skittering Horror (Room 6):**

You stop moving. For a moment so does it. Then the chitinous legs find their grip and it crosses the room in the time it takes you to exhale. The last thing you feel is the cold of it, and then nothing.

---

**Pit Guardian (Room 7):**

You drop your guard and the Guardian notes it with the same unhurried certainty it brings to everything. It raises its weapon. It does not hurry. It has never needed to.

---

**Hollow Mage (Room 9):**

The luminescence fixes on you. The cold doubles, then redoubles. You don't fight it. That is the last decision you make.

---

**Troll (Room 10):**

The small dark eyes watch you put your hands down. The Troll stands. It has been waiting for this. It comes forward with a patience that has outlasted everything else down here, and it outlasts you too.

---

**Bound King (Room 11 — King alive):**

You sink to your knees before him. He looks through you as though you are not there — as though you have never been there, as though the effort of acknowledgement is beneath him. His sword finds your neck without him appearing to move. The last thing you see is his face, and it holds nothing. Not contempt. Not satisfaction. Nothing at all.

---

### Zombie Quit Line

*(Fires when the zombie is alive and in the current room, but no fixed monster is present.)*

It doesn't rush. It has never needed to rush. You stand still long enough for it to find you, and then it takes its time. Whatever is left of you when it is done is not much.

---

### Room-Specific Quit Lines

**Pit (Room 7, no monster):**

You stand at the edge and look down. Then something you never saw — never heard — places a hand flat against your back and pushes. Not hard. It barely needs to. The darkness below takes you, and you fall for longer than you thought possible. The bottom, when it comes, does not come gently.

---

**Cistern (Room 9, no monster):**

You are looking at the water when it happens. A single dark shape beneath the surface, and then a cold pressure around your ankle, and then you are moving and the room is moving away from you and the water closes over your head. The channel beneath is deep. Deeper than the dungeon has any right to go.

---

**Riddle Room (Room 8):**

You face the doors. It doesn't matter anymore which one. You pick one — the wrong one, of course; it was always going to be the wrong one — and as it swings open you understand for a single clear instant what the riddle was actually about. Then the understanding is gone, and so are you.

---

**The Gate (Room 12):**

You hear it before you turn: a low, resonant creak from the Gate itself, the sound of something that has not moved in a very long time deciding to move. You turn. The Gate is open. Whatever is on the other side has been there since before the dungeon was built, and it has been patient, and you have come close enough. It steps through. The last thing you see is the dark beyond the threshold closing behind it like a curtain.

---

### Ambient Quit Pool

*(All other rooms — no monster, no zombie, no room-specific case. Random 1–6.)*

**1:**

You hear something behind you. Not footsteps — the absence of footsteps. A silence that follows you and gets closer. You turn too slowly. The cold enters you between the shoulder blades, precise and final, and the torchlight dims in a straight line down to nothing.

---

**2:**

The torch goes out. In the dark, something is already there. You hear it breathing — or something like breathing, something close enough. You do not have time to relight the flame.

---

**3:**

The floor shifts under your foot. Just once, just slightly. By the time you understand what it means, the stone has already decided, and you with it. The chamber below is silent. You are not.

---

**4:**

There is a gap in the wall at arm height that you never looked at closely. There is a hand in it, pale and entirely still, and it has been there the whole time. Now it moves. It is faster than it has any right to be.

---

**5:**

The cold intensifies. That is all. Just cold, deepening, past the point where cold feels like cold and becomes something else instead — a pressure, a weight, a slow erasure. You are less warm than you were. Then less present. Then gone.

---

**6:**

Something in the room has been waiting. Not the monster — not anything you saw. Something older than the room itself, something that was here when the stones were cut and has been here every moment since. You only know it is there the moment before it decides you have stayed long enough.

---

### End Screen — endState = 6

```
The dungeon has been here longer than anyone you have ever met has been alive.
It will be here after everyone you know is dust.
It did not notice you arrive.
It did not notice you leave.

Your SKILL was X. Your STAMINA reached as low as Y. You tested your LUCK Z times.

Play again? (YES / NO)
```

---

*End of asset file — Deliverables 1, 2, 3, 4, 5, 6, 7, 8, and 9. All content locked.*
