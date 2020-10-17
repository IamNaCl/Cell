using System.Collections.Generic;

namespace Cell.Runtime
{
    /// <summary>
    /// Represents a Cell context.
    /// A context contains all the information about runtime memory, named ranges, named cells and all that cool stuff.
    /// In order to exeucte functions and store results, a context needs to be created so all calls can work.
    /// </summary>
    public class CellContext : ICellContext, System.IDisposable
    {
        #region Privates
        private bool _disposed = false;
        private IDictionary<CellPosition, object> _cellTable = new Dictionary<CellPosition, object>();
        private IDictionary<string, CellPosition> _namedCells = new Dictionary<string, CellPosition>();
        private IDictionary<string, RangePositions> _namedRanges = new Dictionary<string, RangePositions>();
        #endregion

        #region Properties
        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<CellPosition, object>> this[CellPosition begin, CellPosition end] =>
            throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IDictionary<CellPosition, object> CellTable => _cellTable;

        /// <inheritdoc/>
        public IDictionary<string, RangePositions> NamedRanges => _namedRanges;

        /// <inheritdoc/>
        public IDictionary<string, CellPosition> NamedCells => _namedCells;
        #endregion

        #region Indexers
        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<CellPosition, object>> this[RangePositions range] =>
            throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<CellPosition, object>> this[string rangeName] =>
            throw new System.NotImplementedException();

        /// <inheritdoc/>
        public object this[CellPosition position]
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }
        #endregion

        #region Operations
        /// <inheritdoc/>
        public void SetRange(RangePositions range, object value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetRange(RangePositions sourceRange, RangePositions destinationRange)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public object GetNamedCell(string name)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetNamedCell(string name, object value)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (_disposed)
            {
                // TODO: Dispose of big resources here.
                _disposed = true;
            }
        }
        #endregion

        #region Statics
        // Static lock.
        private static object _staticLock = new object();
        private static Dictionary<string, IFunction> _builtInFunctions = new Dictionary<string, IFunction>();

        /// <summary>
        /// Gets the collection of built-in functions defined for all the contexts to be created.
        /// Please use the RegisterFunction function to add new functions to this dictionary.
        /// </summary>
        public static IReadOnlyDictionary<string, IFunction> BuiltInFunctions => _builtInFunctions;

        /// <summary>
        /// Registers a function in the dictionary for global functions.
        /// If `func` is `null`, nothing is done, same happens if `func.Name` is `null`.
        /// This function throws InvalidOperationException if the function name already exists in the dictionary.
        /// </summary>
        /// <param name="func">Function to register.</param>
        public static void RegisterFunction(IFunction func)
        {
            lock (_staticLock)
            {
                if (func is object && func.Name is object)
                {
                    if (_builtInFunctions.ContainsKey(func.Name))
                        throw new System.InvalidOperationException($"Function name {func.Name} is already defined.");

                    _builtInFunctions.Add(func.Name, func);
                }
            }
        }
        #endregion
    }
}
