namespace Cell.Runtime
{
    /// <summary>
    /// Represents a function with name and parameters.
    /// The Cell context has a static dictionary of functions that need to match this
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Gets the name of this function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the count of arguments required to invoke this function.
        /// </summary>
        int ArgumentCount { get; }

        /// <summary>
        /// Invokes this function.
        /// </summary>
        /// <param name="arguments">Collection of arguments passed to this function.</param>
        /// <returns>Value returned by the interpreted function.</returns>
        object Invoke(System.Collections.Generic.IList<Parser.Expressions.IExpression> arguments);
    }
}
