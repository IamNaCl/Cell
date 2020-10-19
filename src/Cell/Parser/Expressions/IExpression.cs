namespace Cell.Parser.Expressions
{
    /// <summary>
    /// Represents an arithmethic/logical/function call expression.
    /// The parser returns an instance of a class that implements this interface in order to allow the user to evaluate
    /// or inspect their expressions.
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// Evaluates an expression that produces a value.
        /// </summary>
        /// <param name="context">Context to operate on.</param>
        /// <param name="error">Output error message, in case of error.</param>
        /// <returns>Result of this expression.</returns>
        object Evaluate(Runtime.ICellContext context, out string error);

        /// <summary>
        /// Generic version of Evaluate, basically returns object as T.
        /// </summary>
        /// <param name="context">Context to operate on.</param>
        /// <param name="error">Output error message, in case of error.</param>
        /// <typeparam name="T">Result type for this function.</typeparam>
        /// <returns>Result of this expression.</returns>
        T Evaluate<T>(Runtime.ICellContext context, out string error) => (T)Evaluate(context, out error);

        /// <summary>
        /// Inspects this expression.
        /// Let's say the source code was: 5 + 3, this function would return "ADD(5, 3)" so you can use it in later
        /// evaluation or save it in a cell.
        /// </summary>
        /// <returns>The "real" code used as the input for this expression.</returns>
        string Inspect();
    }
}
