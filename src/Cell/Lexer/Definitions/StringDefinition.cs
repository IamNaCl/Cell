namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition of the string token type.
    /// </summary>
    class StringDefinition : TokenDefinition
    {
        /// <inheritdoc />
        public override string Process(string token)
        {
            if (string.IsNullOrEmpty(token))
                return string.Empty;

            // Twice of the same quote.
            string toReplace = $"{token[0]}{token[0]}";

            // Remove the quotes.
            token = token.Substring(1, token.Length - 2);

            // If it isn't an empty string after removing the beginning and ending quotes, then try to replace the
            // other ones.
            if (token.Length > 1)
                token = token.Replace(toReplace, toReplace[0].ToString());

            return token;
        }

        /// <summary>
        /// Creates a new instance of the StringDefinition class.
        /// </summary>
        public StringDefinition() : base(TokenType.String, @"(""(""""|[^""""])*""|'(''|[^''])*'|`(``|[^``])*`)") { }
    }
}
