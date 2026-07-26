using System.Linq;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var tasks = new Task[10];
        for (var i = 0; i < 10; i++)
        {
            var page = i % 2 == 0 ? "home" : "about";
            tasks[i] = Task.Run(() => Solution.RecordHit(page));
        }
        await Task.WhenAll(tasks);
        var home = Solution.GetHits("home");
        var about = Solution.GetHits("about");
        result.Add(
            name: "hits-are-correct",
            passed: home == 5 && about == 5,
            expected: "home=5, about=5",
            actual: $"home={home}, about={about}",
            message: home != 5 || about != 5 ? "Check AddOrUpdate and TryGetValue." : "");
        return result;
    }
}
