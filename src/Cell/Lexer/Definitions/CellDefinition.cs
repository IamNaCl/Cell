namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of the cell token type.
    /// </summary>
    class CellDefinition : TokenDefinition
    {
        public override string Process(string token) => token.Substring(1, token.Length - 1);

        /// <summary>
        /// Creates a new instance of the CellDefinition class.
        /// </summary>
        public CellDefinition() : base(TokenType.Cell, @"[$:][0-9]+") { }
    }
}
