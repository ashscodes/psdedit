using System.Management.Automation;

namespace PsdEdit;

public interface IPsdTokenParser
{
    PsdTokenCollection Parse(string filePath, out string content);

    PsdTokenCollection Parse(ScriptBlock scriptBlock, out string content);
}