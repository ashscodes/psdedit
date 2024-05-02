namespace PsdEdit;

public static class PsdTokenManagerExtensions
{
    private static bool IsArrayEnd(this PsdTokenManager tokens)
    {
        if (tokens.Current.IsComma)
        {
            var tokensToConsume = tokens.Next.IsLineTerminator ? 2 : 1;
            tokens.Consume(tokensToConsume);
            return false;
        }

        if (tokens.Current.IsLineTerminator && tokens.Previous.IsComment)
        {
            tokens.Consume();
            return false;
        }

        return tokens.Current.IsArrayClose || tokens.Current.IsLineTerminator;
    }

    private static PsdArray ReadArray(this PsdTokenManager tokens, PsdArray array)
    {
        if (!array.IsArrayLiteral)
        {
            tokens.Consume();
        }

        if (tokens.Current.IsArrayClose)
        {
            tokens.Consume();
            tokens.TestForInlineComment(array);
            return array;
        }

        do
        {
            IPsdObject value = tokens.ReadArrayValue();
            array.Add(value);
        }
        while (!tokens.IsArrayEnd());

        tokens.TestForInlineComment(array);
        return array;
    }

    private static IPsdObject ReadArrayValue(this PsdTokenManager tokens)
    {
        IPsdObject value = null;
        if (tokens.Current.IsComment)
        {
            value = tokens.ReadComment();
        }
        else if (tokens.Lookup.IsBaseValue())
        {
            value = tokens.ReadBaseValue();
        }
        else if (tokens.Current.IsMapOpen)
        {
            value = tokens.ReadMap(new PsdMap());
        }
        else if (tokens.Current.IsArrayOpen)
        {
            value = tokens.ReadArray(new PsdArray());
        }
        else
        {
            tokens.Consume();
        }

        return value;
    }

    private static PsdComment ReadComment(this PsdTokenManager tokens)
    {
        if (tokens.Current.IsComma)
        {
            tokens.Consume();
        }

        var comment = new PsdComment(tokens.Current.Text, tokens.Lookup.HasPrecedingEmptyLines());
        comment.IsWithinArray = tokens.Previous.IsComma;
        tokens.Consume();
        return comment;
    }

    private static string ReadIdentifier(this PsdTokenManager tokens)
    {
        string identifier = tokens.Current.ToPsdValue().ToString();
        tokens.Consume();
        return identifier;
    }

    private static PsdKeyword ReadKeyword(this PsdTokenManager tokens)
    {
        var keyword = new PsdKeyword(tokens.Current.Text);
        tokens.Consume();

        if (tokens.Current.IsNewLine)
        {
            tokens.Consume();
        }

        if (tokens.Current.IsArrayOpen)
        {
            keyword.Conditions = tokens.ReadKeywordConditions();
        }

        if (tokens.Current.IsNestedMap)
        {
            keyword.ScriptBlock = tokens.ReadKeywordScriptBlock();
        }

        return keyword;
    }

    private static PsdKeywordCollection ReadKeywordCollection(this PsdTokenManager tokens)
    {
        var keywords = new PsdKeywordCollection();
        while (tokens.Current.IsKeyword)
        {
            keywords.Add(tokens.ReadKeyword());
        }

        return keywords;
    }

    private static PsdKeywordConditionCollection ReadKeywordConditions(this PsdTokenManager tokens)
    {
        using (var task = new PsdReadKeywordConditionsTask(tokens))
        {
            task.Process();
            return task.Result.Success ? task.Result.Conditions : null; // Errors already in Result.
        }
    }

    private static PsdScriptBlock ReadKeywordScriptBlock(this PsdTokenManager tokens)
    {
        int nestedMaps = 1;
        bool isNested = true;

        var scriptBlock = new PsdScriptBlock();
        tokens.Consume();

        while (isNested)
        {
            if (tokens.Current.IsNestedMap)
            {
                nestedMaps++;
            }

            if (tokens.Current.IsMapClose)
            {
                nestedMaps--;
                if (nestedMaps == 0)
                {
                    isNested = false;
                    tokens.Consume(true);
                    continue;
                }
            }

            IPsdValue value = tokens.ReadBaseValue();
            scriptBlock.Add(value);
        }

        tokens.TestForInlineComment(scriptBlock);
        return scriptBlock;
    }

    private static PsdMapEntry ReadMapEntry(this PsdTokenManager tokens)
    {
        bool hasPrecedingEmptyLine = tokens.Lookup.HasPrecedingEmptyLines();
        string identifier = tokens.ReadIdentifier();

        // If the value is a map, return the empty map now to nest following items in collection.
        if (tokens.Next.IsMapOpen)
        {
            return new PsdMapEntry(identifier,
                                   tokens.ReadMap(new PsdMap(identifier)),
                                   hasPrecedingEmptyLine);
        }

        tokens.Consume(); // Consume assignment operator.
        IPsdObject value = tokens.ReadMapEntryValue();

        return value is not null
            ? new PsdMapEntry(identifier, value, hasPrecedingEmptyLine)
            : throw new PsdReaderException(identifier, tokens.Index);
    }

    private static IPsdObject ReadMapEntryValue(this PsdTokenManager tokens)
    {
        if (tokens.Current.IsArrayOpen)
        {
            return tokens.ReadArray(new PsdArray());
        }
        else if (tokens.Lookup.IsArrayLiteral())
        {
            return tokens.ReadArray(new PsdArray(true));
        }
        else if (tokens.Current.IsKeyword)
        {
            return tokens.ReadKeywordCollection();
        }
        else if (tokens.Lookup.BaseValues.IsMatch(tokens.Current))
        {
            var baseValueToken = tokens.Current;
            tokens.Consume();
            return baseValueToken.ToPsdValue();
        }
        else
        {
            return null;
        }
    }

    internal static IPsdValue ReadBaseValue(this PsdTokenManager tokens)
    {
        IPsdValue value = tokens.Current.ToPsdValue();
        tokens.Consume();
        tokens.TestForInlineComment(value);
        return value;
    }

    internal static PsdMap ReadMap(this PsdTokenManager tokens, PsdMap map, bool returnWithoutValues = false)
    {
        map.HasPrecedingEmptyLine = tokens.Lookup.HasPrecedingEmptyLines();

        if (map.IsNamed || returnWithoutValues)
        {
            tokens.Consume(map.IsNamed ? 2 : 1);
        }
        else
        {
            tokens.Consume();
            do
            {
                var psdObject = tokens.ReadNextObject();
                map.Add(psdObject);
            }
            while (!tokens.Current.IsMapClose);

            tokens.TestForInlineComment(map);
        }

        return map;
    }

    internal static IPsdObject ReadNextObject(this PsdTokenManager tokens)
    {
        IPsdObject psdObject = null;

        if (tokens.Current.IsComment)
        {
            psdObject = tokens.ReadComment();
        }
        else if (tokens.Lookup.IsMapEntry())
        {
            psdObject = tokens.ReadMapEntry();
        }
        else
        {
            tokens.Consume();
        }

        return psdObject;
    }

    internal static void TestForInlineComment(this PsdTokenManager tokens, IPsdObject psdObject)
    {
        if (psdObject is not null && tokens.Lookup.IsInlineComment())
        {
            psdObject.Comment = tokens.ReadComment();
        }
    }
}