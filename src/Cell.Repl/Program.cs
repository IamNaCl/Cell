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

    // To be updated.
    static void Main(string[] args)
    {
        bool needMore = false;
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

                    if (expression is null || errorString is object)
                        PrintError(errorString);
                    else
                        Console.WriteLine(expression.Inspect());

                    break;
                }
                case TokenizerResult.Error:
                    PrintError(errorString); break;

                case TokenizerResult.NeedsMore:
                    needMore = true; break;
            }
        }
    }
}
