using System;
using System.Linq;
using Cell.Runtime;

namespace Cell.Parser.Expressions
{
    /// <summary>
    /// Represents an interpreted function call, used to perform any kind of
    /// </summary>
    class FunctionCallExpression : IExpression
    {
        #region Properties
        /// <summary>
        /// Gets the expression that should return the function name for this call.
        /// </summary>
        public string FunctionName { get; private set; }

        /// <summary>
        /// Gets the collection of arguments to process.
        /// </summary>
        public System.Collections.Generic.IList<IExpression> Arguments { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the FunctionCallExpression class.
        /// </summary>
        /// <param name="func">Literal name of the function to execute.</param>
        /// <param name="args">Arguments to this function.</param>
        public FunctionCallExpression(string func, System.Collections.Generic.IList<IExpression> args) =>
            (FunctionName, Arguments) = (func ?? "<invalid>", args ?? new System.Collections.Generic.List<IExpression>());

        /// <summary>
        /// Creates a new instance of the FunctionCallExpression class with one argument.
        /// </summary>
        /// <param name="functionName">Name of the function being called.</param>
        /// <param name="arg1">First argument.</param>
        public FunctionCallExpression(string functionName, IExpression arg1)
            : this(functionName, new System.Collections.Generic.List<IExpression> { arg1 }) { }

        /// <summary>
        /// Creates a new instance of the FunctionCallExpression class with two arguments.
        /// </summary>
        /// <param name="functionName">Name of the function being called.</param>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        public FunctionCallExpression(string functionName, IExpression arg1, IExpression arg2)
            : this(functionName, new System.Collections.Generic.List<IExpression> { arg1, arg2 }) { }
        #endregion

        #region IExpression
        /// <inheritdoc/>
        public object Evaluate(ICellContext context, out string error)
        {
            error = null;
            var func = context[FunctionName];
            if (func is null)
                error = $"{FunctionName} is not a function.";

            return func?.Invoke(context, Arguments, out error);
        }

        /// <inheritdoc/>
        public string Inspect() =>
            $"{FunctionName}({string.Join(", ", Arguments.Select(_ => _?.Inspect() ?? null))})";
        #endregion
    }
}
