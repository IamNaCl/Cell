namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of a whitespace skipper, this definition shall not produce a token and should be used
    /// to skip whitespaces.
    /// </summary>
    class WhiteSpaceDefinition : TokenDefinition
    {
        /// <summary>
        /// Creates a new instance of the WhiteSpaceDefinition class.
        /// </summary>
        public WhiteSpaceDefinition() : base(TokenType.Unknown, @"\s+", true) { }
    }
}
