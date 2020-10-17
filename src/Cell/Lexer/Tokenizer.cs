using System;
using System.Collections.Generic;
using Cell.Lexer.Definitions;

namespace Cell.Lexer
{
    /// <summary>
    /// Cell's tokenizer, produces tokens.
    /// </summary>
    public static class Tokenizer
    {
        #region Privates
        // Collection of token definitions.
        private static IList<ITokenDefinition> _defs = new List<ITokenDefinition>
        {
            new WhiteSpaceDefinition(),
            new CommentDefinition(),
            new NameDefinition(),
            new NumberDefinition(),
            new StringDefinition(),
            new CellDefinition(),
            new OperatorDefinition()
        };
        #endregion

        #region Tokenize
        /// <summary>
        /// Gets the current scope level in the token list.
        /// </summary>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Scope level.</returns>
        private static int GetScopeLevel(IList<Token> tokens)
        {
            int scope = 0;
            if (tokens is object)
                foreach (var token in tokens)
                    if (token == TokenType.BracketL)
                        ++scope;
                    else if (token == TokenType.BracketR)
                        --scope;

            return scope;
        }

        /// <summary>
        /// Tokenizes the input string.
        /// </summary>
        /// <param name="input">String to tokenize.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Result of tokenization.</returns>
        public static TokenizerResult Tokenize(string input, ref IList<Token> tokens, out string error)
        {
            if (input is null)
            {
                error = "Input is null.";
                return TokenizerResult.Error;
            }

            if (tokens is null)
                tokens = new List<Token>();

            int offset = 0;
            int scopeLevel = GetScopeLevel(tokens);
            string errMsg = null;
            TokenizerResult result = TokenizerResult.Ok;

            while (offset < input.Length)
            {
                int oldOffset = offset;
                foreach (var def in _defs)
                {
                    // Match the current token definition.
                    int matchLen = def.Match(input, offset);

                    // If there's a match that cannot be ignored, then set it up properly.
                    if (matchLen > 0 && !def.Ignore)
                    {
                        var value = def.Process(input.Substring(offset, matchLen));
                        var token = new Token(def.GetTokenType(value), value, offset);

                        // Is it a bracket?
                        if (token == TokenType.BracketL)
                            ++scopeLevel;
                        else if (token == TokenType.BracketR)
                        {
                            --scopeLevel;
                            if (scopeLevel < 0)
                            {
                                error = "Unexpected ')' without previous active scopes.";
                                tokens.Clear();
                                return TokenizerResult.Error;
                            }
                        }
                        tokens.Add(token);
                    }

                    // If the match length was greater than zero, then add it and restart the process.
                    if (matchLen > 0)
                    {
                        offset += matchLen;
                        break;
                    }
                }

                // If up to this point the current offset is the same as the old offset, it means that we found an
                // unknown character, so we just state the error and stop the loop.
                if (oldOffset == offset)
                {
                    errMsg = $"Unexpected {(offset >= input.Length? "end of input": "character")} at offset {offset}.";
                    result = TokenizerResult.Error;
                    tokens.Clear(); // Clear on error.
                    scopeLevel = 0;
                    break;
                }
            }

            // Do we need more tokens to produce a statement?
            if (scopeLevel > 0)
                result = TokenizerResult.NeedsMore;

            error = errMsg;
            return result;
        }
        #endregion
    }
}
