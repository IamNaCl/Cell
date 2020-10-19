using System;
using System.Collections.Generic;
using Cell.Parser.Expressions;

namespace Cell.Runtime
{
    /// <summary>
    /// Static class that contains all the pre-defined built-in functions for Cell.
    /// </summary>
    public static class Functions
    {
        #region Fields
        private static object _staticLock = new object();
        private static IDictionary<string, IFunction> _functions =
                    new Dictionary<string, IFunction>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region Helpers
        /// <summary>
        /// Evaluates the arguments of a function, call this before sending the arguments to the underlying function.
        /// </summary>
        /// <param name="args">Arguments to evaluate.</param>
        /// <param name="context">Cell context to operate on.</param>
        /// <param name="error">Output string with any error.</param>
        /// <returns>Instance of List`1 with the evaluated arguments.</returns>
        public static IList<object> EvalArgs(this IList<IExpression> args, ICellContext context, out string error)
        {
            error = null;
            var input = args ?? new List<IExpression>();
            var list = new List<object>(input.Count);

            for (int i = 0; i < list.Count; i++)
                list.Add(input[i].Evaluate(context, out error));

            return list;
        }

        /// <summary>
        /// Attempts to get a function.
        /// </summary>
        /// <param name="functionName">Name of the function to look up for.</param>
        /// <returns>Instance of IFunction or null if not found.</returns>
        public static IFunction GetFunction(string functionName)
        {
            if (functionName is null)
                throw new ArgumentNullException(nameof(functionName));

            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentException("Function name is empty.");

            lock (_staticLock)
                return _functions.TryGetValue(functionName, out var func)? func: null;
        }

        /// <summary>
        /// Attempts to add a function to the global context (Available from any new context).
        /// Note: This function will throw if the function already exists, as functions in the global context are not
        /// meant to be overwritten.
        /// </summary>
        /// <param name="function">Function to be added.</param>
        public static void AddFunction(IFunction function)
        {
            if (function is null)
                throw new ArgumentNullException(nameof(function));

            lock (_staticLock)
            {
                if (_functions.TryGetValue(function.Name, out var func))
                    throw new InvalidOperationException($"Cannot overwrite global function '{function.Name}'.");

                _functions[function.Name] = function;
            }
        }
        #endregion

        #region Functions
        #endregion
    }
}
