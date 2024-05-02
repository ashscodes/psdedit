using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace PsdEdit;

public sealed class PsdTokenParser : IPsdTokenParser
{
    public PsdTokenCollection Parse(string filePath, out string content)
    {
        content = string.Empty;
        var ast = Parser.ParseFile(filePath, out Token[] tokens, out ParseError[] errors);
        Ensure.ParseTaskSucceeded(GetMessages(errors), filePath);
        content = ast.ToString();
        return NewTokenCollection(tokens);
    }

    public PsdTokenCollection Parse(ScriptBlock scriptBlock, out string content)
    {
        content = scriptBlock.ToString();
        return Parse(content);
    }

    private static IEnumerable<string> GetMessages(ParseError[] errors)
    {
        foreach (var error in errors)
        {
            yield return error.Message;
        }
    }

    private static PsdTokenCollection NewTokenCollection(Token[] tokens)
        => new PsdTokenCollection() { Array.ConvertAll(tokens, t => (PsdToken)t) };

    internal static PsdTokenCollection Parse(string content)
    {
        var ast = Parser.ParseInput(content, out Token[] tokens, out ParseError[] errors);
        Ensure.ParseTaskSucceeded(GetMessages(errors), content);
        return NewTokenCollection(tokens);
    }
}