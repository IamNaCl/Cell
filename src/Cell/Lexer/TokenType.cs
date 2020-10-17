using System.Linq;

namespace Cell.Lexer
{
    /// Remember to update the dictionary below if new token types are added.
    /// <summary>
    /// Enum containing all the supported token types in Cell.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Token is end of file token.
        /// </summary>
        EOF,

        /// <summary>
        /// Token is a reference to a cell.
        /// </summary>
        Cell,

        /// <summary>
        /// Token is a '+' sign.
        /// </summary>
        Plus,

        /// <summary>
        /// Token is a '-' sign.
        /// </summary>
        Minus,

        /// <summary>
        /// Token is a '*' sign.
        /// </summary>
        Star,

        /// <summary>
        /// Token is a '/' sign.
        /// </summary>
        Slash,

        /// <summary>
        /// Token is a '<=' operator.
        /// </summary>
        LessEq,

        /// <summary>
        /// Token is a '<' operator.
        /// </summary>
        LessT,

        /// <summary>
        /// Token is a '==' or '=' operator.
        /// </summary>
        Equal,

        /// <summary>
        /// Token is a '!=' or '<>' operator.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Token is a '>' operator.
        /// </summary>
        GreaterT,

        /// <summary>
        /// Token is a '>=' operator.
        /// </summary>
        GreaterEq,

        /// <summary>
        /// Token is the ampersand operator.
        /// </summary>
        Ampersand,

        /// <summary>
        /// Left bracket '('.
        /// </summary>
        BracketL,

        /// <summary>
        /// Right bracket ')'.
        /// </summary>
        BracketR,

        /// <summary>
        /// A literal comma ','.
        /// </summary>
        Comma,

        /// <summary>
        /// Identifier name (function name or named cell/range).
        /// </summary>
        Name,

        /// <summary>
        /// Token is a numeric literal.
        /// </summary>
        Number,

        /// <summary>
        /// Token is a string literal.
        /// </summary>
        String,

        /// <summary>
        /// Token type is unsupported/unknown.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Contains extension methods for the TokenType enum.
    /// </summary>
    public static class TokenTypeExtensions
    {
        // Contains the string representations of the token types.
        // Remember to update this if new tokens are added.
        private static System.Collections.Generic.IDictionary<TokenType, string> _reprs =
                    new System.Collections.Generic.Dictionary<TokenType, string>
        {
            [TokenType.EOF] = "<eof>",
            [TokenType.Cell] = "cell",
            [TokenType.Plus] = "+",
            [TokenType.Minus] = "-",
            [TokenType.Star] = "*",
            [TokenType.Slash] = "/",
            [TokenType.LessEq] = "<=",
            [TokenType.LessT] = "<",
            [TokenType.Equal] = "== or =",
            [TokenType.NotEqual] = "!= or <>",
            [TokenType.GreaterT] = ">",
            [TokenType.GreaterEq] = ">=",
            [TokenType.Ampersand] = "&",
            [TokenType.BracketL] = "(",
            [TokenType.BracketR] = ")",
            [TokenType.Comma] = ",",
            [TokenType.Name] = "name",
            [TokenType.Number] = "number",
            [TokenType.String] = "string",
            [TokenType.Unknown] = "unknown",
        };

        /// <summary>
        /// Gets thre string representation of the token type.
        /// </summary>
        /// <param name="type">Type to get its representation.</param>
        /// <returns>String containing the string representation of the token type or "invalid".</returns>
        public static string AsString(this TokenType type) =>
            _reprs.TryGetValue(type, out var val)? val: "invalid";

        /// <summary>
        /// Checks whether the current token type exists within the array of token types passed as argument.
        /// </summary>
        /// <param name="type">Enumerator containing the token to check.</param>
        /// <param name="types">Token types to match against the current token on enumerator.</param>
        /// <returns>True if the token type for the current token exists within the array of types.</returns>
        public static bool TokenIs(this TokenType type, params TokenType[] types) =>
            types.Contains(type);
    }
}
