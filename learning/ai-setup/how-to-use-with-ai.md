# How to Use AI Tools with SharpBASIC

## The fundamental rule

**You write the code. Always. Both tools are teachers, not typists.**

The learning value in this project is not in having a working interpreter — it is in building one. Every line of code you write forces you to understand what it does and why. Every line an AI writes on your behalf removes that understanding.

Claude explains concepts, reviews code when you ask, and helps you reason through architecture. Copilot notices patterns in what you are typing and hints at what might come next. You read both. You decide. You write.

The moment you accept code you have not thought through, the learning stops. The interpreter keeps growing; your understanding does not.

---

## The division of labour

| Task | Tool | Your role |
|---|---|---|
| Understanding compiler theory | Claude | Read the explanation, close it, think through it yourself |
| Architecture decisions | Claude | Discuss the options, then decide and record your decision |
| Phase planning | Claude | Talk through the phase before writing a line of code |
| Code review when stuck | Claude | Paste your code, get an explanation, fix it yourself |
| Token type enums | Copilot hint | Read every suggestion before accepting; verify it belongs to the current phase |
| AST node records | Copilot hint | Low conceptual risk once you understand each node; still read before accepting |
| Switch expression arms | Copilot hint | Write the first arm yourself; Copilot may suggest the pattern |
| xUnit test structure | Copilot hint | Skeleton only; assertion values are always yours |
| Implementation logic | Neither | You write this. Every line. |

---

## Setting up Claude

1. Create a Claude Project at claude.ai
2. Paste the system prompt from `ai-setup/claude-system-prompt.md` into Custom Instructions
3. Upload the relevant phase spec and `decisions/architecture-decisions.md` as project knowledge
4. Update Current Phase Status in the system prompt as you complete each phase

### How to talk to Claude

- **Explain before you implement.** "I'm about to build the Pratt parser. Walk me through how the binding power loop decides when to stop." Then close Claude and write the code.
- **Paste your code, not a description of your code.** "Here is my `ParseExpression` — it works for `2 + 3` but fails for `2 + 3 * 4`. What am I missing?" is a better question than "how do I fix Pratt parsing?"
- **Plan phases before starting.** "I'm about to start Phase 7. What are the three most important things to get right before writing a line?" Use the answer to inform your approach, then close it.
- **Update the spec after decisions.** When you make an architectural decision — especially one that diverges from the phase spec — record it in the decisions log.

### What not to ask Claude

- "Generate the `ParseExpression` method for me." You are asking Claude to do Phase 4 for you.
- "Write the tests for Phase 7." You are asking Claude to define what Phase 7 means. That is your job.
- "Should I add file I/O to Phase 10?" The scope is fixed. Claude will redirect you. Trust the constraint.

---

## Using Copilot correctly

The right mental model: Copilot is autocomplete, not a code generator. It continues a pattern you established. You establish the pattern by writing the first version of every new construct yourself.

### The discipline

- **Read before accepting.** Every Copilot suggestion is a proposal, not an answer. Read it. Does it match what you intended? Does it introduce something from a later phase?
- **Never use Copilot Chat or inline chat for implementation blocks.** If you find yourself typing a description of what you want Copilot to write, stop. Close the chat. Write it yourself.
- **Reject out-of-scope suggestions.** Copilot has no knowledge of phase boundaries. In Phase 3, it will suggest Phase 7 patterns. Recognise them and reject them.
- **Reject future-phase suggestions.** A suggestion that would be correct in Phase 7 is wrong in Phase 3, even if it compiles.

---

## The learning loop — one iteration per phase

1. Read the phase spec in `phase-specs/`
2. Read the relevant theory notes — especially `theory/pratt-parsing.md` before Phase 4 and `theory/call-frames.md` before Phase 7
3. Ask Claude to walk through the architecture with you — what are the moving parts, what order do they need to be built? Then close Claude.
4. Write failing tests yourself. The spec includes examples; you should be writing additional cases based on your own understanding of what could go wrong.
5. Implement — you write every line. Copilot may hint at patterns you have established. Read every hint before accepting.
6. When you are stuck, paste your code into Claude and get the explanation. Read it. Then fix the code yourself — do not copy Claude's fix.
7. Refactor once the tests pass. You drive; Copilot may hint at shape.
8. Ask Claude to review your key decisions and confirm they are consistent with the architecture. Update the spec if anything changed.

The temptation at step 5 is to open Copilot Chat and describe the problem. That is the moment where the learning happens. Sit with it. Read what you have written. Understand what it is doing. Then, if you are still stuck, ask Claude — and read the explanation, not the code.

---

## What to do when Claude offers code unprompted

Read it to understand the concept. Then close it and write your own version from memory.

If you cannot write your own version from memory, the concept has not landed. Ask Claude to explain it differently — not to write more code.
