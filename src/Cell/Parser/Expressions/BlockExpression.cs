using System.Collections.Generic;
using System.Linq;
using Cell.Runtime;

namespace Cell.Parser.Expressions
{
    /// <summary>
    /// Represents a block of expressions.
    /// </summary>
    class BlockExpression : IExpression
    {
        /// <summary>
        /// Gets the body of this block.
        /// </summary>
        public IList<IExpression> Body { get; private set; }

        /// <inheritdoc/>
        public object Evaluate(ICellContext context)
        {
            object result = null;
            foreach (var statement in Body)
                result = statement.Evaluate(context);
            return result;
        }

        /// <inheritdoc/>
        public string Inspect() =>
            $"({string.Join(", ", Body.Select(_ => _.Inspect()))})";

        public BlockExpression(IList<IExpression> body) => Body = body ?? new List<IExpression>();
    }
}
