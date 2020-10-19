using System.Collections.Generic;
using Cell.Parser.Expressions;

namespace Cell.Runtime
{
    /// <summary>
    /// Delegate type for built-in functions.
    /// </summary>
    /// <param name="context">Context to operate on.</param>
    /// <param name="args">Arguments passed to the function.</param>
    /// <param name="error">Error string.</param>
    /// <returns>Result of this function call.</returns>
    public delegate object FunctionDelegate(ICellContext context, IList<IExpression> args, out string error);
}
