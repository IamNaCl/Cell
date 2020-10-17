namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of the cell token type.
    /// </summary>
    class CellDefinition : TokenDefinition
    {
        /// <summary>
        /// Creates a new instance of the CellDefinition class.
        /// </summary>
        public CellDefinition() : base(TokenType.Cell, @"[$:]([a-zA-Z]{1,4})([0-9]+)") { }
    }
}
