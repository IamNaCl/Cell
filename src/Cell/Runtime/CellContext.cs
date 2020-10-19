using System;
using System.Collections.Generic;
using System.Linq;

namespace Cell.Runtime
{
    /// <summary>
    /// Basic implementation of a Cell context.
    /// </summary>
    public class CellContext : ICellContext
    {
        #region Privates
        // Lock for different threads.
        private object _syncLock = new object();

        // Collection of functions (case insensitive).
        private IDictionary<string, IFunction> _functions;

        // Collection of cells.
        private IDictionary<int, object> _cells;
        #endregion

        #region Indexers
        /// <inheritdoc/>
        public object this[int index] { get => GetCell(index); set => SetCell(index, value); }

        /// <inheritdoc/>
        public IFunction this[string functionName] => GetFunction(functionName);

        /// <inheritdoc/>
        public IReadOnlyDictionary<int, object> this[int startIndex, int endIndex] =>
            GetRange(startIndex, endIndex);
        #endregion

        #region Private Functions
        /// <summary>
        /// Gets a condition for a for loop.
        /// </summary>
        /// <param name="increases">Whether to reach length or to come from length.</param>
        /// <returns>Instance of Func`3.</returns>
        private Func<int, int, bool> GetCondition(bool increases)
        {
            // Cannot match lambdas for some reason...
            if (increases)
                return (index, length) => index <= length;
            else
                return (index, length) => index >= length;
        }

        /// <summary>
        /// Gets all the indices of a given range.
        /// </summary>
        /// <param name="start">Start point in the range.</param>
        /// <param name="end">End point in the range.</param>
        /// <returns>List of integers.</returns>
        private IList<int> GetRangeIndexes(int start, int end)
        {
            var incr = end > start? 1: -1;
            var condition = GetCondition(incr == 1);
            var list = new List<int>(incr == 1? end - start: start - end);

            for (int i = start; condition(i, end); i += incr)
                list.Add(i);

            return list;
        }
        #endregion

        #region Get/Set Cell/Range/Function
        /// <inheritdoc/>
        public object GetCell(int index)
        {
            lock (_syncLock)
                return _cells.TryGetValue(index, out var val)? val: null;
        }

        /// <inheritdoc/>
        public void SetCell(int index, object value)
        {
            lock (_syncLock)
            {
                _cells[index] = value;
                if (value is null)
                    _cells.Remove(index);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<int, object> GetRange(int start, int end)
        {
            // Condition that we'll be using
            int increase = end >= start? 1: -1;
            var condition = GetCondition(increase == 1);
            var result = new Dictionary<int, object>();

            lock (_syncLock)
                for (int i = start; condition(i, end); i += increase)
                    result.Add(i, _cells.TryGetValue(i, out var value)? value: null);

            return result;
        }

        /// <inheritdoc/>
        public void SetRange(int startIndex, int endIndex, object value)
        {
            lock (_syncLock)
            {
                // Swap to make it always from smallest to biggest.
                int start = Math.Min(startIndex, endIndex);
                int end   = Math.Max(startIndex, endIndex);
                while (start <= end)
                {
                    if (value is null)
                        _cells.Remove(start);
                    else
                        _cells[start] = value;

                    start++;
                }
            }
        }

        /// <inheritdoc/>
        public void SetRange(int sourceStart, int sourceEnd, int destStart, int destEnd)
        {
            lock (_syncLock)
            {
                var sourceRange = GetRange(sourceStart, sourceEnd).Values.ToArray();
                var targetIndexes = GetRangeIndexes(destStart, destEnd);
                for(int i = 0; i < targetIndexes.Count; i++)
                    SetCell(targetIndexes[i], sourceRange[i % sourceRange.Length]);
            }
        }

        /// <summary>
        /// Gets a function by its name (case insensitive).
        /// </summary>
        /// <param name="funcName">Name of the function to look up.</param>
        /// <returns>Instance of IFunction or null if no function exists with the given name.</returns>
        public IFunction GetFunction(string funcName)
        {
            if (funcName is null)
                throw new ArgumentNullException(nameof(funcName));

            if (string.IsNullOrEmpty(funcName))
                throw new ArgumentException("Function name is empty.");

            return _functions.TryGetValue(funcName, out var function)? function: Functions.GetFunction(funcName);
        }

        /// <summary>
        /// Sets/adds a function to the function dictionary.
        /// </summary>
        /// <param name="function">Function to add/insert into the functions dictionary.</param>
        public void AddFunction(IFunction function)
        {
            if (function is object)
                lock (_syncLock)
                    _functions[function.Name] = function;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the CellContext class.
        /// </summary>
        public CellContext()
        {
            _cells = new Dictionary<int, object>();
            _functions = new Dictionary<string, IFunction>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion
    }
}
