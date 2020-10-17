namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of a comment, this shall be ignored by the tokenizer.
    /// </summary>
    class CommentDefinition : TokenDefinition
    {
        /// <summary>
        /// Creates a new instance of the CommentDefinition class.
        /// </summary>
        public CommentDefinition() : base(TokenType.Unknown, @"#.*$", true) { }
    }
}
