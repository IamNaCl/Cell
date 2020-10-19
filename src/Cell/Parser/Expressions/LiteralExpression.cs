namespace Cell.Parser.Expressions
{
    /// <summary>
    /// Represents a literal value, returned as expression since it is needed by the parser and most function calls.
    /// </summary>
    class LiteralExpression : IExpression
    {
        /// <summary>
        /// Gets the actual value.
        /// </summary>
        public object Literal { get; }

        /// <inheritdoc/>
        public object Evaluate(Runtime.ICellContext context, out string error)
        {
            error = null;
            return Literal;
        }

        /// <inheritdoc/>
        public string Inspect() => $"{Literal.ToString()}";

        /// <summary>
        /// Creates a new instance of the LiteralExpression class.
        /// </summary>
        /// <param name="value">Value for this literal.</param>
        public LiteralExpression(object value) => Literal = value;
    }
}
