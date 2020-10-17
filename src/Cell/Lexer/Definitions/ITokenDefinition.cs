namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Represents a token definition, input strings are matched against these, if there is a match, then this token
    /// is returned.
    /// </summary>
    interface ITokenDefinition
    {
        /// <summary>
        /// Gets whether this rule shall produce a token or not.
        /// </summary>
        bool Ignore { get; }

        /// <summary>
        /// Gets the default (fallback) token type for this definition.
        /// </summary>
        TokenType DefaultTokenType { get; }

        /// <summary>
        /// Gets the regex pattern of this token definition.
        /// </summary>
        string Pattern { get; }

        /// <summary>
        /// Matches the input string at the give offset (startAt) and returns the length of the match.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="startAt">Position on string to start matching.</param>
        /// <returns>Length of the match.</returns>
        int Match(string input, int startAt);

        /// <summary>
        /// Pre-processes the token after the tokenizer produces it.
        /// </summary>
        /// <param name="token">Input string produced by the tokenizer.</param>
        /// <returns>Preprocessed string.</returns>
        string Process(string token) => token;

        /// <summary>
        /// Gets the correct token type if the resulting token type changes based on the value, e.g.: An operator.
        /// </summary>
        /// <param name="input">Input string to validate.</param>
        /// <returns>Actual token type matching this input string, or the default one of not required.</returns>
        TokenType GetTokenType(string input) => DefaultTokenType;
    }
}
