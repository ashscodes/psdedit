using System.Management.Automation;

namespace PsdEdit;

public static class PowerShellExtensions
{
    public static PsdReadResult ReadPsdScriptBlock(this ScriptBlock scriptBlock, IPsdTokenParser parser = null)
    {
        parser ??= new PsdTokenParser();
        var tokenCollection = parser.Parse(scriptBlock, out string content);

        Ensure.StringNotNullOrEmpty(content, nameof(content));

        using var task = new PsdReadTask(PsdTokenManager.Create(tokenCollection), string.Empty, content);
        task.Process();
        return task.Result;
    }
}