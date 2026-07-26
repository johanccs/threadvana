using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Completed = 0;
        await Solution.ScrapeUrlsAsync(new[] { "a", "b", "c", "d" });
        result.Add("all-scraped", Solution.Completed == 4, "4", $"{Solution.Completed}",
            Solution.Completed != 4 ? "Scrape all 4 URLs with SemaphoreSlim throttling." : "");
        return result;
    }
}
