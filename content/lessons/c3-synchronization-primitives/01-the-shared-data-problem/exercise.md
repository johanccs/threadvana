Stop the lost increments.

`Solution.Run()` is provided: it starts two threads, and each one increments
`Solution.Counter` 1000 times. Inside `Increment1000Times()` the increment is
written out as its three real steps â   READ, ADD, WRITE â   and right now NOTHING
protects them, so the total keeps coming out below 2000.

Your job: wrap the three increment lines in a `lock`, using the provided key:

```csharp
lock (Solution.Gate)
{
    // the three lines: read, add, write
}
```

Both threads run the same method, so both will automatically use the SAME key â  
that is what makes the lock work.

We run `Run()` 5 times and require EXACTLY 2000 every single time. With a
correct lock that happens always; without it, almost never.

## Hints
1. The three lines `int temp = Counter; temp = temp + 1; Counter = temp;` must ALL sit inside the same lock â   together they become one unbreakable step.
2. The syntax is: `lock (Solution.Gate) { ... }` â   put the three lines between the braces.
3. Leave `Thread.Yield()` alone â   it is only there to make the race visible, and the lock still wins.