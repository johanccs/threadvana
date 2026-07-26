using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace ThreadCraft.Execution;

/// <summary>
/// Translates common Roslyn diagnostics into junior-friendly English
/// (docs/architecture.md §Friendly diagnostics). The raw compiler message is always
/// kept alongside in CompileIssue.RawMessage.
/// </summary>
public static class FriendlyDiagnostics
{
    public static string Map(Diagnostic diagnostic)
    {
        var raw = diagnostic.GetMessage();
        var quoted = FirstQuotedText(raw);

        return diagnostic.Id switch
        {
            "CS1002" => "A statement is missing its semicolon at the end.",
            "CS1513" => "A closing brace } is missing somewhere.",
            "CS1022" => "There is code outside of any class or method — check your braces { }.",
            "CS0246" => quoted is not null
                ? $"'{quoted}' is unknown — check the spelling, or add the right using at the top of the file."
                : "A name used here is unknown — check the spelling or add a using at the top of the file.",
            "CS0103" => quoted is not null
                ? $"The name '{quoted}' does not exist here — check the spelling, or declare it first."
                : "A name used here does not exist — check the spelling or declare it first.",
            "CS1061" => quoted is not null
                ? $"This object has no method or property called '{quoted}' — check the spelling."
                : "That method or property does not exist on this type — check the spelling.",
            "CS0029" => "You are putting one type of value into a variable of a different type — check the types match.",
            "CS1503" => "You are passing one type of value into a method that expects a different type.",
            "CS4032" or "CS4033" =>
                "You used 'await' inside a method that is not marked async — add 'async' and make the method return Task.",
            "CS1525" => "Something here is not valid C# — check for a missing value, bracket or parenthesis.",
            "CS0201" => "This line does not do anything — did you mean to call a method or assign a value?",
            "CS7036" => "A required argument is missing from this method call.",
            "CS0161" => "This method promises to return a value, but some paths through it return nothing.",
            "CS0127" => "This method returns void, so it cannot return a value.",
            "CS0116" => "There is a method or statement directly inside the class/namespace where it does not belong — check your braces.",
            "CS5001" => "The program has no entry point — make sure the required class and method names are unchanged.",
            "CS0656" => "Something the compiler needs is missing — check the class and method signatures match the exercise.",
            _ => $"The compiler says: {raw}"
        };
    }

    private static string? FirstQuotedText(string message)
    {
        var match = Regex.Match(message, "'([^']+)'");
        return match.Success ? match.Groups[1].Value : null;
    }
}
