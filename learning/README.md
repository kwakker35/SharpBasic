# SharpBASIC — Learning Materials

SharpBASIC is a complete BASIC-inspired tree-walking interpreter built in C# .NET 10 across 10 deliberate phases. It was built as a learning project, and these materials are a first-class deliverable alongside the code.

---

## What you can do with this

- **Follow the journey from scratch.** The phase specs guide you through building the same interpreter, phase by phase, using the same AI-assisted workflow. Every decision is documented. Every pitfall is recorded.
- **Fork the repo and use the AI prompts.** The `ai-setup/` folder contains the exact Claude system prompt and Copilot guidance used during the build, stripped of personal context and ready to adapt.
- **Study it as a reference.** The phase specs, architecture decisions, and theory notes form a coherent account of how a tree-walking interpreter is actually built — not how one could theoretically be built.
- **Contribute improvements.** If you find an error, a missing edge case, or a lesson worth recording, a pull request is welcome.

---

## How this folder is organised

| Folder | Contents |
|---|---|
| `ai-setup/` | Claude system prompt, how to use AI tools correctly, Copilot guidance |
| `phase-specs/` | One spec per phase — goal, concepts, test examples, gotchas, end state |
| `decisions/` | The 10 key architectural decisions with reasoning |
| `theory/` | Deeper dives into Pratt parsing, call frames, pitfalls, and reading list |

---

## Where to start

1. Read `decisions/architecture-decisions.md` to understand the shape of the project before any code exists.
2. Read `theory/pratt-parsing.md` before Phase 4. It will save you significant time.
3. Read `theory/call-frames.md` before Phase 7. The concept needs to land before the implementation begins.
4. Work through `phase-specs/phase-01-lexer.md` and follow the spec for each phase in order.
5. Set up the AI tooling using `ai-setup/claude-system-prompt.md` and `ai-setup/how-to-use-with-ai.md`.

The specs reference `theory/pitfalls.md` throughout. That document records everything that actually went wrong during the build. Read the relevant sections before each phase, not after.

---

## A note on these materials

These materials are free to adapt and use. A link back to the original repository is appreciated but not required.

The code is the authority. Where any document conflicts with the running interpreter or `docs/language-spec-v1.md`, the spec wins.
