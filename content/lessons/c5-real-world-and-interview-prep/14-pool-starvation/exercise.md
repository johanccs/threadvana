Fire 50 `Task.Run` calls that each do `Thread.Sleep(500)` (sync sleep on pool threads). Return `"starved"` after `Task.WhenAll`.

## Hints
1. `var tasks = new Task[50]; for (var i=0;i<50;i++) tasks[i]=Task.Run(()=>Thread.Sleep(500));`
2. `await Task.WhenAll(tasks); return "starved";`
