using System;
using System.Collections.Generic;
using System.IO;
using Cell.Lexer;
using Cell.Parser;
using Cell.Parser.Expressions;
using Cell.Runtime;

namespace Cell.Repl
{
    static class Interpreter
    {
        private static void PrintError(string error) => Console.Error.WriteLine($"error: {error}");

        public static object Run(TextReader inputFile, bool isInteractive = false)
        {
            if (inputFile is null)
                return null;

            object result = null;
            bool needMoreTokens = false;
            IList<Token> tokenList = new List<Token>();
            string error = null;
            using (var context = new CellContext(Console.In, Console.Out, Console.Error))
            {
                while (true)
                {
                    if (!needMoreTokens)
                        tokenList.Clear();

                    if (isInteractive)
                        Console.Write(needMoreTokens? ".. ": "$$ ");

                    string input = inputFile.ReadLine();
                    if (input is null)
                        break;

                    needMoreTokens = false;

                    switch (Tokenizer.Tokenize(input, ref tokenList, out error))
                    {
                        case TokenizerResult.Ok:
                        {
                            var expr = Parser.Parser.Parse(tokenList, out error);
                            if (error is object)
                                goto case TokenizerResult.Error;

                            result = expr.Evaluate(context, out error);
                            if (error is object)
                                goto case TokenizerResult.Error;
                        } break;
                        case TokenizerResult.Error:
                        {
                            PrintError(error);
                            if (!ReferenceEquals(inputFile, Console.In))
                                Environment.Exit(1);
                        } break;
                        case TokenizerResult.NeedsMore:
                            needMoreTokens = true; break;
                    }
                }
            }
            return result;
        }
    }
}
