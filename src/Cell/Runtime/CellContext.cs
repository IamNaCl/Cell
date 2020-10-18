using System;
using System.Collections.Generic;

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
        public IEnumerable<KeyValuePair<int, object>> this[int startIndex, int endIndex] =>
            GetRange(startIndex, endIndex);
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
        public IEnumerable<KeyValuePair<int, object>> GetRange(int start, int end)
        {
            // Should we go upwards? If yes, then keep iterating while i < end
            // Otherwise, iterate until i >= end
            int increase = end >= start? 1: -1;
            Predicate<int> condition = increase == 1? (_ => _ <= end): (_ => _ >= end);
            for (int i = start; condition(i); i += increase)
            {
                KeyValuePair<int, object> result;

                // Request the lock and release it right before returning.
                lock (_syncLock)
                {
                    _cells.TryGetValue(i, out var value);
                    result = new KeyValuePair<int, object>(i, value);
                }

                yield return result;
            }
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
                // 1. Check the length of both ranges.
                // 2. If the source range is smaller than the destination, then copy the source range N times.
                // 3. If the destination range is smaller than the source, then copy destination.Length items.
                int srcLen = Math.Max(sourceStart, sourceEnd) - Math.Min(sourceStart, sourceEnd);
                int dstLen = Math.Max(destStart, destEnd) - Math.Min(destStart, destEnd);

                // If either length is zero, don't do anything then.
                if (srcLen <= 0 || dstLen <= 0)
                    return;

                // Do we need to increase/decrease the value of either range?
                int inc1 = sourceEnd >= sourceStart? 1: -1;
                int inc2 = destEnd >= destStart? 1: -1;

                // Now iterate the entire thing.
                int len = 0;
                for (int s1 = sourceStart, s2 = destStart; len <= dstLen; s1 += inc1, s2 += inc2)
                {
                    object value = null;
                    _cells.TryGetValue((s1 % srcLen) + sourceStart, out value);

                    if (value is null)
                        _cells.Remove(s2);
                    else
                        _cells[s2] = value;

                    ++len;
                }
            }
        }

        /// <summary>
        /// Gets a function by its name (case insensitive).
        /// </summary>
        /// <param name="functionName">Name of the function to look up.</param>
        /// <returns>Instance of IFunction or null if no function exists with the given name.</returns>
        public IFunction GetFunction(string functionName)
        {
            if (functionName is null)
                throw new ArgumentNullException(nameof(functionName));

            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentException("Function name is empty.");

            return _functions.TryGetValue(functionName, out var function)? function: null;
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
