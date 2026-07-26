# Writing Style Guide (MANDATORY for all lesson content & UI copy)

Audience: a **junior developer** who knows basic C# syntax (classes, methods, loops)
but has **never written multithreaded code**. If a sentence would confuse a junior,
rewrite it. (Project instruction #12: clear, simple, junior-dev explanations.)

## The 6 rules

1. **Plain English first, jargon second.** Every technical term is defined in one
   simple sentence the first time it appears. Example: *"A thread is a worker that
   does one thing at a time. Your program always starts with one worker — the main thread."*
2. **Everyday analogy before code.** Give a real-world picture first.
   - Semaphore = a parking lot with N spaces; when it's full, cars wait.
   - lock = a single bathroom key; only one person inside at a time.
   - Thread pool = a team of on-call workers; you hand in tasks, they pick them up.
   - async/await = ordering coffee and getting a buzzer; you sit down (do other
     things) and the buzzer calls you back when it's ready.
3. **Short sentences.** Max ~20 words. One idea per sentence.
4. **No assumed knowledge.** Never write "as you already know", "obviously", "simply".
   If a lesson needs an earlier concept, link the earlier lesson by title.
5. **Encouraging, direct tone.** "You" not "we". Celebrate small wins.
   Never shame mistakes: errors are "totally normal — everyone hits this".
6. **Interview corner = simple model answers.** What the interviewer asks, then a
   2–4 sentence answer a junior could actually say out loud, then one "bonus point"
   sentence for depth.

## Fixed lesson skeleton (theory Markdown body, in this order)

```markdown
## What is it?
(2–3 plain sentences. Define the term. No code yet.)

## The real-world picture
(The analogy. 2–4 sentences.)

## How it works in C#
(Minimal code, heavily commented. Show the smallest possible example.
Every non-obvious line gets a comment.)

## See it move
(Tell the learner to press "Run demo" and WHAT to watch for in the visualization.
e.g. "Watch the two swimlanes — notice they overlap in time.")

## Watch out
(The 1–3 most common mistakes juniors make here. Phrase as "You might think X… but Y".)

## Key takeaways
(3–5 bullets, each one line. These are the interview answers in disguise.)
```

## UI copy

- Buttons say what happens: "Run demo", "Check my code", "Show a hint".
- Validation failures always follow: **what we expected → what happened → the one
  most likely cause → what to try next.** Never just an assertion dump.
