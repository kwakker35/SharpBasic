# SharpBASIC — Phase 7 Redirect & Guidance Notes

> These notes correct two issues from Phase 6 and guide Phase 7 implementation.
>
> **Important — teaching approach:**
> Do not generate complete implementations unprompted. Lead with the concept,
> show the shape of the solution, then ask the developer if they want to proceed.
> Prefer showing method signatures and letting the developer reason about the body.
> Highlight decision points and ask what the developer thinks before generating code.

---

## Fix 1 — Remove `NextVar` Token Type

`NextVar` was introduced to validate that `NEXT i` matches `FOR i`.
This is the right validation but the wrong layer.

**Ask the developer:** "Which layer should own this validation and why —
the lexer, the parser, or the evaluator?"

Once discussed, guide toward removing `NextVar` from `TokenType` and
implementing the check in the parser when `NEXT` is encountered.
Do not show the implementation until the developer has reasoned through it.

---

## Fix 2 — Extend `TokenType` and the Keyword Dictionary

The current enum is missing tokens needed for Phase 7 and some that should
have landed in Phase 5.

**Ask the developer** to identify what is missing before showing them.
Prompt with: "Phase 7 introduces SUBs and FUNCTIONs — what new keywords
will the lexer need to recognise?"

Missing tokens to arrive at through discussion:
- `Sub`, `Function`, `Return`, `Call`, `As`
- `True`, `False`
- `Integer`, `Float`, `String`, `Boolean`
- `And`, `Or`, `Not`
- `Comma`

Remind the developer that adding to the enum is not enough — the keyword
dictionary in `Lexer.cs` also needs updating. Ask them to spot that
themselves rather than stating it upfront.

---

## Phase 7 — Guided Implementation Order

Work through these steps one at a time. Do not move to the next step until
the developer has understood and confirmed the current one.

### Step 1 — Concepts first

Before any code, ask the developer:
- "What information does a call frame need to hold?"
- "When does a frame get pushed — at declaration or at call time?"
- "Where should argument expressions be evaluated — before or after the
  frame is pushed, and why does the order matter?"

Let the developer reason through these. Correct gently if needed.
Do not proceed to AST nodes until the concepts are clear.

### Step 2 — AST nodes

Show the `Parameter` record as a starting point and ask the developer
to sketch `FunctionDeclaration` and `ReturnStatement` themselves
before confirming or correcting.

### Step 3 — Parser

Guide the developer through `ParseParameterList` first as it is the
most mechanical piece. Ask them to attempt `ParseFunctionDeclaration`
before generating a solution.

### Step 4 — The return mechanism

**This is a key teaching moment.** Ask the developer how they would
handle `RETURN` unwinding across multiple stack levels before suggesting
anything. When they have a proposal, discuss the tradeoffs, then introduce
`ReturnException` as the idiomatic interpreter approach. Explain why
using an exception for control flow is intentional and correct here even
though it goes against normal application code practice.

### Step 5 — Evaluator call sequence

Present the call sequence as a numbered list of steps and ask the developer
to implement one step at a time. Do not generate the whole evaluator at once.
The ordering of argument evaluation relative to frame pushing is a deliberate
learning point — ask the developer to reason about why the order matters
before they write the code.

### Step 6 — SUB vs FUNCTION boundary

Ask the developer: "What should happen if a FUNCTION body completes without
hitting RETURN? What about a SUB that returns a value?"
Let them define the rules before implementing the guards.

---

## Tests — Write These First

Present each scenario as a description and ask the developer to write the
test before the implementation. Do not generate test code unprompted.

Scenarios to cover in order:
1. A SUB that prints a value when called
2. A FUNCTION that returns a value used in a PRINT
3. Argument count mismatch throws a runtime error
4. A FUNCTION that completes without RETURN throws a runtime error
5. A recursive FUNCTION — Fibonacci is the canonical example

The recursive test is the proof that frames and scope are working correctly.
Treat it as the phase completion milestone. If Fibonacci with n=10 returns
55, Phase 7 is done.

---

## What NOT to Generate

- Do not add `NextVar` back under any name
- Do not implement closures, first-class functions, or anonymous functions
- Do not add `ELSEIF` — nested IF is the correct SharpBASIC approach
- Do not add features beyond SUBs, FUNCTIONs, CALL, and RETURN
- Do not generate a complete implementation in one shot — work incrementally

---

*Teaching approach: concept first, signature second, implementation third.*
*The developer should be doing the reasoning. Copilot guides, not drives.*
