using System.Xml.Linq;
using ThreadCraft.Core.Curriculum;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Renders wwwroot/sitemap.xml from the loaded curriculum (home, categories, lessons).
/// Runs once at startup so the sitemap always matches the shipped content.
/// </summary>
public static class SitemapGenerator
{
    private static readonly XNamespace Ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");

    public static void WriteToWwwRoot(ICurriculumService curriculum, string contentRootPath)
    {
        var urls = new List<XElement>
        {
            new(Ns + "url", new XElement(Ns + "loc", "/"), new XElement(Ns + "priority", "1.0"))
        };
        foreach (var category in curriculum.GetCategories())
        {
            urls.Add(new(Ns + "url",
                new XElement(Ns + "loc", $"/category/{category.Id}"),
                new XElement(Ns + "priority", "0.8")));
            foreach (var lesson in curriculum.GetLessons(category.Id))
            {
                urls.Add(new(Ns + "url",
                    new XElement(Ns + "loc", $"/lesson/{lesson.Id}"),
                    new XElement(Ns + "priority", "0.6")));
            }
        }

        var doc = new XDocument(new XElement(Ns + "urlset", urls));
        var outPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "sitemap.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
        File.WriteAllText(outPath, doc.Declaration + Environment.NewLine + doc.ToString());
    }
}
