using System.Text.RegularExpressions;

namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Base class used by all the token definitions for the lexer.
    /// </summary>
    abstract class TokenDefinition : ITokenDefinition
    {
        // Instance of the compiled regex.
        private Regex _regex;

        #region Properties
        /// <inheritdoc/>
        public bool Ignore { get; private set; }

        /// <inheritdoc/>
        public TokenType DefaultTokenType { get; private set; }

        /// <inheritdoc/>
        public string Pattern { get; private set; }
        #endregion

        #region ITokenDefinition
        /// <inheritdoc/>
        public virtual TokenType GetTokenType(string value) => DefaultTokenType;

        /// <inheritdoc/>
        public virtual string Process(string token) => token;
        #endregion

        #region Match
        /// <summary>
        /// Matches the input agains the regex for this definition.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="startAt">Position to start matching.</param>
        /// <returns>Match information.</returns>
        internal Match MatchRegex(string input, int startAt = 0) =>
            _regex.Match(input, startAt);

        /// <inheritdoc/>
        public int Match(string input, int startAt)
        {
            if (string.IsNullOrEmpty(input) || startAt >= input.Length)
                return 0;

            var match = MatchRegex(input, startAt);
            return match.Success && match.Index == startAt? match.Length: 0;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the fields of the TokenDefinition class.
        /// </summary>
        /// <param name="defaultTokenType">Default token type for these.</param>
        /// <param name="pattern">Regex pattern to match in order to produce this token.</param>
        /// <param name="ignore">Whether this definition shall produce a token or not.</param>
        public TokenDefinition(TokenType defaultTokenType, string pattern, bool ignore = false)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new System.ArgumentNullException(nameof(pattern));

            (Ignore, DefaultTokenType, Pattern) = (ignore, defaultTokenType, pattern);
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }
        #endregion
    }
}
