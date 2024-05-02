using System;
using System.IO;
using PsdEdit;

internal class Program
{
    private static void Main(string[] args)
    {
        var file = new FileInfo(args[0]);
        var result = file.ReadPsdFile();
        var output = result.Success ? $"Read {result.Count} items." : $"Failed {result} task.";
        // Console.WriteLine(output);

        if (result.Items[7] is PsdMap psdMap)
        {
            if (psdMap[1] is not null && psdMap[1] is PsdComment psdComment)
            {
                if (psdComment.TryGetMapEntry(out PsdMapEntry mapEntry, out IPsdObject other))
                {
                    var lol = mapEntry;
                    Console.WriteLine(lol.GetValue());
                }
            }
        }
    }
}