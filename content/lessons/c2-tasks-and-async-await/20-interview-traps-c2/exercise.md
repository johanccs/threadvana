Write `Solution.QuizAsync()` â   no exercise to solve, just return `"ok"`. This lesson
is a review of the entire c2 category. Read the theory, run the demo, and prepare
for the interview questions in the front matter.

## Hints
1. `Solution.QuizAsync()` just needs to return `"ok"` â   use `Task.FromResult("ok")` or `async` + `return "ok";`.
2. If you marked the method `async`, make sure it still returns `Task<string>`, not `string`.
3. There is nothing to await here â   a synchronous body that returns a completed task is perfectly fine.
