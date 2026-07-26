using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(50);

        try { Solution.Run(); }
        catch { /* harness absorbs any unexpected fall-out */ }

        await Task.Delay(300);

        var result = new HarnessResult();

        result.Add(
            name: "error-caught",
            passed: Solution.ErrorFromWorker.Contains("danger"),
            expected: "ErrorFromWorker contains the word 'danger'",
            actual: $"ErrorFromWorker = '{Solution.ErrorFromWorker}'",
            message: "The exception was not caught by your worker. Wrap " +
                     "DangerousCode.Run() in try/catch, and set ErrorFromWorker = ex.Message in the catch.");

        result.Add(
            name: "main-unaffected",
            passed: Solution.SeenByMain.Contains("all clear"),
            expected: "the main thread still reports 'all clear'",
            actual: $"SeenByMain = '{Solution.SeenByMain}'",
            message: "The main thread should not know about the worker's error " +
                     "(that's the point of this lesson). SeenByMain must stay as the main thread's message.");

        result.Add(
            name: "worker-finished",
            passed: Solution.ErrorFromWorker != "" || Solution.SeenByMain.Contains("all clear"),
            expected: "something ran",
            actual: $"ErrorWorker={Solution.ErrorFromWorker}, SeenByMain={Solution.SeenByMain}",
            message: "Was Run() called? Check that your thread actually starts and catches.");

        return result;
    }
}
