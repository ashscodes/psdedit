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
        Console.WriteLine(output);
    }
}