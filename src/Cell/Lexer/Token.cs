namespace Cell.Lexer
{
    /// <summary>
    /// Lexer's output, represents the smallest unit of data that can be useful for the language.
    /// </summary>
    public class Token
    {
        #region Properties
        /// <summary>
        /// Gets the token type for this token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets the value of this token.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the offset on input string where this token was located.
        /// </summary>
        public int Offset { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Token class.
        /// </summary>
        /// <param name="type">Type of this token.</param>
        /// <param name="value">Value as string of this token.</param>
        /// <param name="offset">Offset where this token was located.</param>
        public Token(TokenType type, string value, int offset) =>
            (Type, Value, Offset) = (type, value ?? type.AsString(), offset);
        #endregion

        #region Operators
        public override string ToString() => $"{Type.AsString()}: {Value ?? Type.AsString()}";

        public static implicit operator TokenType(Token token) => token?.Type ?? TokenType.Unknown;
        #endregion
    }
}
