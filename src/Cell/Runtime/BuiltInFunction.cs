using System;
using System.Collections.Generic;
using Cell.Parser.Expressions;

namespace Cell.Runtime
{
    /// <summary>
    /// Represents a built-in function from the C# context to the interpreter.
    /// </summary>
    public class BuiltInFunction : IFunction
    {
        // Actual function to invoke.
        private FunctionDelegate _func;

        #region Public Properties
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public int ParameterCount { get; }

        /// <inheritdoc/>
        public bool IsVariadic { get; }
        #endregion

        /// <inheritdoc/>
        public object Invoke(ICellContext ctx, IList<IExpression> arguments, out string error)
        {
            error = null;
            if (arguments?.Count != ParameterCount && !IsVariadic)
                error = $"{Name}: argument count differs from parameter count.";

            if (IsVariadic && arguments?.Count < ParameterCount)
                error = $"{Name}: function requires {(IsVariadic? "at least": "")} {ParameterCount} arguments.";

            if (error is object)
                return null;

            return _func(ctx, arguments ?? new List<IExpression>(), out error);
        }

        /// <summary>
        /// Creates a new instance of the BuiltInFunction class.
        /// </summary>
        /// <param name="name">Name of this function.</param>
        /// <param name="paramCount">Parameter count.</param>
        /// <param name="isVariadic">Does this function accept a variable number of arguments?</param>
        /// <param name="func">C# Function to invoke.</param>
        public BuiltInFunction(string name, int paramCount, bool isVariadic, FunctionDelegate func)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Function name is empty.");

            if (func is null)
                throw new ArgumentNullException(nameof(func));

            if (paramCount < 0)
                paramCount = 0;

            _func = func;
            Name = name;
            ParameterCount = paramCount;
            IsVariadic = isVariadic;
        }

        public override string ToString() => $"<function:{Name}>";
    }
}
