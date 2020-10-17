namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of the cell token type.
    /// </summary>
    class CellDefinition : TokenDefinition
    {
        #region Statics
        private static CellDefinition _instance = null;

        /// <summary>
        /// Gets the reference to the static instance of the CellDefinition class.
        /// </summary>
        public static CellDefinition Instance => _instance is object? _instance: (_instance = new CellDefinition());
        #endregion

        /// <summary>
        /// Creates a new instance of the CellDefinition class.
        /// </summary>
        public CellDefinition() : base(TokenType.Cell, @"[$:]([a-zA-Z]{1,4})([0-9]+)") { }
    }
}
