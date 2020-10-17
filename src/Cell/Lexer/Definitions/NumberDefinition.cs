namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of the number token type.
    /// </summary>
    class NumberDefinition : TokenDefinition
    {
        /// <summary>
        /// Creates a new instance of the NumberDefinition class.
        /// </summary>
        public NumberDefinition() : base(TokenType.Number, @"([0-9]+\.[0-9]+|[0-9]+|\.[0-9]+)([Ee][+-]?[0-9]+)?") { }
    }
}
