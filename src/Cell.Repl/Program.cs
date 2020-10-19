using System;
using System.IO;
using System.Collections.Generic;
using CommandLine;
using Cell.Repl;

class Program
{
    /// <summary>
    /// Opens a file.
    /// </summary>
    /// <param name="path">Path to the file to open.</param>
    /// <returns>TextReader.</returns>
    static TextReader OpenFile(string path)
    {
        try
        {
            return File.OpenText(path);
        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e}");
            Environment.Exit(1);
        }
        return null;
    }

    /// <summary>
    /// Executes after parsing the command line arguments.
    /// </summary>
    /// <param name="opts">Options sent from the command line.</param>
    static void RunOptions(CommandLineOptions opts)
    {
        bool isInteractive = false;
        TextReader input = null;
        if (opts.InputFile is object)
            input = OpenFile(opts.InputFile);
        else if (opts.EvaluateString is object)
            input = new StringReader(opts.EvaluateString);
        else
        {
            input = Console.In;
            isInteractive = !Console.IsInputRedirected && !Console.IsOutputRedirected;
        }

        Interpreter.Run(input, isInteractive);
    }

    /// <summary>
    /// Handles command line argument parser errors.
    /// </summary>
    /// <param name="errs">Enumerable of errors.</param>
    static void HandleParseError(IEnumerable<Error> errs) =>
        Console.WriteLine("Something happened while parsing command line arguments.");

    static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }
}
