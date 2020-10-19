using System;
using System.Collections.Generic;
using Cell.Lexer;
using Cell.Parser;

class Program
{
    /// <summary>
    /// Prints an error thru the error stream in the Console class.
    /// </summary>
    /// <param name="errorMessage">Message to print.</param>
    private static void PrintError(string errorMessage) =>
        Console.Error.WriteLine($"error: {errorMessage ?? "Unknown error."}");

    /// <summary>
    /// Runs the REPL program.
    /// </summary>
    private static void RunRepl()
    {
        bool needMore = false;
        var context = new Cell.Runtime.CellContext();
        IList<Token> tokenList = new List<Token>();
        while (true)
        {
            // If we don't need more tokens at the beginning of the loop, it means that the previous attempt to execute
            // a statement was in vain, so let's clear everything up.
            if (!needMore)
                tokenList.Clear();

            Console.Write(needMore? ".. ": "$$ ");
            needMore = false;
            string input = Console.ReadLine();

            switch (Tokenizer.Tokenize(input, ref tokenList, out var errorString))
            {
                case TokenizerResult.Ok:
                {
                    var expression = Parser.Parse(tokenList, out errorString);
                    if (expression is object)
                    {
                        var result = expression.Evaluate(context, out errorString);
                        if (errorString is object)
                            PrintError(errorString);
                    }
                    else if (expression is null && errorString is object)
                        PrintError(errorString);

                    break;
                }
                case TokenizerResult.Error:
                    PrintError(errorString); break;

                case TokenizerResult.NeedsMore:
                    needMore = true; break;
            }
        }
    }

    // To be updated with a proper CLI.
    static void Main(string[] args) => RunRepl();
}
