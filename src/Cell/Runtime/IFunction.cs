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
        /// Gets the count of parameters required to invoke this function.
        /// </summary>
        int ParameterCount { get; }

        /// <summary>
        /// Gets whether this function accepts a variable number of arguments aside from those that are predefined in
        /// the argument count, for example: a variadic function where the `ArgumentCount` property is set to 3 will
        /// require at least 3 arguments in order to be invoked.
        /// </summary>
        bool IsVariadic { get; }

        /// <summary>
        /// Invokes this function.
        /// </summary>
        /// <param name="arguments">Collection of arguments passed to this function.</param>
        /// <returns>Value returned by the interpreted function.</returns>
        object Invoke(System.Collections.Generic.IList<Parser.Expressions.IExpression> arguments);
    }
}
