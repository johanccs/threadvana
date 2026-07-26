using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var reply = await Solution.PickTool();
        result.Add("review-loaded", reply == "ok", "ok", reply, reply != "ok" ? "Return ok." : "");
        return result;
    }
}
