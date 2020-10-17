namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of the name/identifier token type.
    /// </summary>
    class NameDefinition : TokenDefinition
    {
        /// <summary>
        /// Creates a new instance of the NameDefinition class.
        /// </summary>
        public NameDefinition() : base(TokenType.Name, @"[a-zA-Z_@][a-zA-Z0-9_@]*") { }
    }
}
