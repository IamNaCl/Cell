namespace Cell.Lexer
{
    /// <summary>
    ///
    /// </summary>
    public enum TokenizerResult
    {
        /// <summary>
        /// Tokenizer found enough tokens to produce a statement.
        /// </summary>
        Ok,

        /// <summary>
        /// Tokenizer is missing a few scopes to complete a statements.
        /// </summary>
        NeedsMore,

        /// <summary>
        /// There was an error while tokenizing.
        /// </summary>
        Error
    }
}
