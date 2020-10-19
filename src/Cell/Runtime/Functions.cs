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
        private static IDictionary<string, IFunction> _functions;
        #endregion

        #region Helpers
        /// <summary>
        /// Evaluates the arguments of a function, call this before sending the arguments to the underlying function.
        /// </summary>
        /// <param name="args">Arguments to evaluate.</param>
        /// <param name="context">Cell context to operate on.</param>
        /// <param name="error">Output string with any error.</param>
        /// <returns>Instance of List`1 with the evaluated arguments.</returns>
        public static IList<object> Eval(this IList<IExpression> args, ICellContext context, out string error)
        {
            error = null;
            var input = args ?? new List<IExpression>();
            var list = new List<object>(input.Count);

            for (int i = 0; i < args.Count; i++)
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

        /// <summary>
        /// My failed attempt to create a function that makes errors easier.
        /// </summary>
        /// <param name="functionName">Name of the function being executed.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>String with the actual error.</returns>
        private static string BuildError(string functionName, string errorMessage) =>
            $"{functionName}: {errorMessage ?? "unknown error."}";
        #endregion

        #region Cells, Ranges and Inspect
        private static object GetCell(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var index = realArgs[0];

            if (index is double)
                index = (int)((double)index);

            if (!(index is int))
                error = BuildError("GET_CELL", "argument is not a number.");

            return error is object? null: ctx[(int)index];
        }

        private static object GetRange(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var start = realArgs[0];
            var end = realArgs[1];

            if (start is double)
                start = (int)((double)start);

            if (end is double)
                end = (int)((double)end);

            if (!(start is int) || !(end is int))
                error = BuildError("GET_RANGE", "argument is not a number.");

            return error is object? null: ctx[(int)start, (int)end];
        }

        private static object Inspect(ICellContext ctx, IList<IExpression> args, out string error)
        {
            error = null;
            return args[0].Inspect();
        }
        #endregion

        #region Arithmethic Functions
        private static object Add(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("ADD", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("ADD", "second argument is not a number.");

            return error is object? null: (object)((double)l + (double)r);
        }

        private static object Subtract(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("SUBTRACT", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("SUBTRACT", "second argument is not a number.");

            return error is object? null: (object)((double)l - (double)r);
        }

        private static object Multiply(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("MULTIPLY", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("MULTIPLY", "second argument is not a number.");

            return error is object? null: (object)((double)l * (double)r);
        }

        private static object Divide(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("DIVIDE", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("DIVIDE", "second argument is not a number.");

            if ((double)r == 0.0)
                error = BuildError("DIVIDE", "did you even try to divide by zero?");

            return error is object? null: (object)((double)l / (double)r);
        }

        private static object Negate(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];

            if (!(l is double))
                error = BuildError("NEGATE", "first argument is not a number.");

            return (error is object)? null: (object)(-(double)l);
        }

        private static object Abs(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];

            if (!(l is double))
                error = BuildError("PLUS", "first argument is not a number.");

            return (error is object)? null: (object)Math.Abs((double)l);
        }
        #endregion

        #region Logic Functions
        private static object LessEqual(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("LESS_EQUAL", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("LESS_EQUAL", "second argument is not a number.");

            return error is object? null: (object)((double)l <= (double)r);
        }

        private static object LessThan(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("LESS_THAN", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("LESS_THAN", "second argument is not a number.");

            return error is object? null: (object)((double)l < (double)r);
        }

        private static object Equal(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (l is null || l.Equals(string.Empty) || l.Equals(0.0))
                l = false;

            if (r is null || r.Equals(string.Empty) || r.Equals(0.0))
                r = false;

            return l.Equals(r);
        }

        private static object NotEqual(ICellContext ctx, IList<IExpression> args, out string error)
        {
            return !(bool)Equal(ctx, args, out error);
        }

        private static object GreaterThan(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("GREATER_THAN", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("GREATER_THAN", "second argument is not a number.");

            return error is object? null: (object)((double)l > (double)r);
        }

        private static object GreaterEqual(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (!(l is double))
                error = BuildError("GREATER_EQUAL", "first argument is not a number.");

            if (!(r is double))
                error = BuildError("GREATER_EQUAL", "second argument is not a number.");

            return error is object? null: (object)((double)l >= (double)r);
        }

        private static object And(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (l is null || l.Equals(string.Empty) || l.Equals(0.0)) l = false;
            if (r is null || r.Equals(string.Empty) || r.Equals(0.0)) r = false;

            // This is in case of array or range.
            if (l is object && !(l is bool)) l = true;
            if (r is object && !(r is bool)) r = true;

            return (bool)l && (bool)r;
        }

        private static object Or(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];
            var r = realArgs[1];

            if (l is null || l.Equals(string.Empty) || l.Equals(0.0)) l = false;
            if (r is null || r.Equals(string.Empty) || r.Equals(0.0)) r = false;

            // This is in case of array or range.
            if (l is object && !(l is bool)) l = true;
            if (r is object && !(r is bool)) r = true;

            return (bool)l || (bool)r;
        }

        private static object Not(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            var l = realArgs[0];

            if (l is null || l.Equals(string.Empty) || l.Equals(0.0)) l = false;

            // This is in case of array or range.
            if (l is object && !(l is bool)) l = true;

            return !(bool)l;
        }
        #endregion

        #region String Functions
        private static object Concat(ICellContext ctx, IList<IExpression> args, out string error)
        {
            var realArgs = args.Eval(ctx, out error);
            if (error is null)
            {
                var result = new System.Text.StringBuilder();
                foreach (var arg in realArgs)
                {
                    // Arg is actually a range.
                    if (arg is Dictionary<int, object> dict)
                        result.AppendJoin("", dict.Values);
                    // Arg is an array defined in the language, arrays should always be IList<object>
                    else if (arg is IList<object> arr)
                        result.AppendJoin("", arr);
                    // Arg is: null, string, double or bool.
                    else
                        result.Append(arg?.ToString() ?? "");

                }
                return result.ToString();
            }
            return null;
        }
        #endregion

        #region Static Constructor
        static Functions()
        {
            _functions = new Dictionary<string, IFunction>(StringComparer.OrdinalIgnoreCase)
            {
                ["GET_CELL"] = new BuiltInFunction("GET_CELL", 1, false, GetCell),
                ["GET_RANGE"] = new BuiltInFunction("GET_RANGE", 2, false, GetRange),
                ["INSPECT"] = new BuiltInFunction("INSPECT", 1, false, Inspect),
                ["ADD"] = new BuiltInFunction("ADD", 2, false, Add),
                ["SUBTRACT"] = new BuiltInFunction("SUBTRACT", 2, false, Subtract),
                ["MULTIPLY"] = new BuiltInFunction("MULTIPLY", 2, false, Multiply),
                ["DIVIDE"] = new BuiltInFunction("DIVIDE", 2, false, Divide),
                ["NOT"] = new BuiltInFunction("NOT", 1, false, Negate),
                ["ABS"] = new BuiltInFunction("ABS", 1, false, Abs),
                ["GREATER_EQUAL"] = new BuiltInFunction("GREATER_EQUAL", 2, false, GreaterEqual),
                ["GREATER_THAN"] = new BuiltInFunction("GREATER_THAN", 2, false, GreaterThan),
                ["EQUAL"] = new BuiltInFunction("EQUAL", 2, false, Equal),
                ["NOT_EQUAL"] = new BuiltInFunction("NOT_EQUAL", 2, false, NotEqual),
                ["LESS_THAN"] = new BuiltInFunction("LESS_THAN", 2, false, LessThan),
                ["LESS_EQUAL"] = new BuiltInFunction("LESS_EQUAL", 2, false, LessEqual),
                ["AND"] = new BuiltInFunction("AND", 2, false, And),
                ["OR"] = new BuiltInFunction("OR", 2, false, Or),
                ["NOT"] = new BuiltInFunction("NOT", 1, false, Not),
                ["CONCAT"] = new BuiltInFunction("CONCAT", 1, true, Concat)
            };
        }
        #endregion
    }
}
