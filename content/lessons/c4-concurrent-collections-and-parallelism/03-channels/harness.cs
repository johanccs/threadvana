using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        await Solution.RunAsync();
        await Task.Delay(50);

        var result = new HarnessResult();

        result.Add(
            name: "last-message-correct",
            passed: Solution.LastMessage == "done",
            expected: "the last message consumed is 'done'",
            actual: $"LastMessage = '{Solution.LastMessage}'",
            message: "The consumer should read all 3 messages and store the last one ('done'). " +
                     "Make sure the producer writes all 3 and calls Complete().");

        result.Add(
            name: "something-consumed",
            passed: Solution.LastMessage != "",
            expected: "at least one message was consumed",
            actual: $"LastMessage = '{Solution.LastMessage}'",
            message: "No messages were received. Check that the consumer reads from the channel.");

        return result;
    }
}
