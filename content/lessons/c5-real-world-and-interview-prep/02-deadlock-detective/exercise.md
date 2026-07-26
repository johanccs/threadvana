Fix the circular wait.

The starter has two threads with a classic deadlock: Thread 1 locks A then B.
Thread 2 locks B then A. If the timing is right, both grab their first lock and
wait forever for the second.

Fix it by making Thread 2 lock A first, then B — the same order as Thread 1.
One change is all it takes.

## Hints
1. Thread 2's locks should be `lock (LockA)` then inside it `lock (LockB)` — the opposite of what it does now.
2. You only change Thread 2's lock order. Thread 1 is already correct.
3. Consistent lock ordering everywhere is the #1 deadlock prevention rule.
