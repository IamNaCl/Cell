namespace Cell.Lexer.Definitions
{
    /// <summary>
    /// Definition for all the operator token types.
    /// </summary>
    class OperatorDefinition : TokenDefinition
    {
        /// <inheritdoc/>
        public override TokenType GetTokenType(string value)
        {
            switch (value)
            {
                case "+":  return TokenType.Plus;
                case "-":  return TokenType.Minus;
                case "*":  return TokenType.Star;
                case "/":  return TokenType.Slash;
                case "<=": return TokenType.LessEq;
                case "<":  return TokenType.LessT;
                case ">":  return TokenType.GreaterT;
                case ">=": return TokenType.GreaterEq;
                case "=":
                case "==": return TokenType.Equal;
                case "<>":
                case "!=": return TokenType.NotEqual;
                case ",":  return TokenType.Comma;
                case "&":  return TokenType.Ampersand;
                case "(":  return TokenType.BracketL;
                case ")":  return TokenType.BracketR;
                default:   return DefaultTokenType;
            }
        }
        /// <summary>
        /// Creates a new instance of the OperatorDefinition clas.
        /// </summary>
        public OperatorDefinition() : base(TokenType.Unknown, @"(<=|>=|!=|<>|==?|\+|\-|\*|\/|\,|&|\(|\))") { }
    }
}
