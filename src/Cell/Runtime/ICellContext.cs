using System.Collections.Generic;

namespace Cell.Runtime
{
    /// <summary>
    /// Represents an object context in Cell, contains the cell table, the
    /// </summary>
    public interface ICellContext
    {
        /// <summary>
        /// Gets the collection of used cells in this context.
        /// </summary>
        IDictionary<CellPosition, object> CellTable { get; }

        /// <summary>
        /// Gets the collection of named ranges defined in this context.
        /// </summary>
        IDictionary<string, RangePositions> NamedRanges { get; }

        /// <summary>
        /// Gets the collection of named cells defined in this context.
        /// </summary>
        IDictionary<string, CellPosition> NamedCells { get; }

        /// <summary>
        /// Gets a range from this context.
        /// </summary>
        IEnumerable<KeyValuePair<CellPosition, object>> this[RangePositions range] { get; }

        /// <summary>
        /// Gets a range from this context.
        /// </summary>
        IEnumerable<KeyValuePair<CellPosition, object>> this[CellPosition begin, CellPosition end] { get; }

        /// <summary>
        /// Gets the range defined in a name within this context.
        /// </summary>
        IEnumerable<KeyValuePair<CellPosition, object>> this[string rangeName] { get; }

        /// <summary>
        /// Gets or sets the value of a cell within this context.
        /// </summary>
        object this[CellPosition position] { get; set; }

        #region Setters
        /// <summary>
        /// Sets all the positions in this range to a given value.
        /// If `value` is `null`, then the range is deleted from the cells dictionary.
        /// </summary>
        /// <param name="range">Range to update.</param>
        /// <param name="value">Value to set ve objects on this range.</param>
        void SetRange(RangePositions range, object value);

        /// <summary>
        /// Copies a range into another.
        /// If the source range is smaller than the destination, the source will be replicated until the end.
        /// </summary>
        /// <param name="sourceRange">Source range.</param>
        /// <param name="destinationRange">Destination range.</param>
        void SetRange(RangePositions sourceRange, RangePositions destinationRange);

        /// <summary>
        /// Gets the value of a named cell.
        /// </summary>
        /// <param name="name">Name of the cell.</param>
        /// <returns>Value of the cell.</returns>
        object GetNamedCell(string name);

        /// <summary>
        /// Sets the value of a named cell.
        /// If the new value is null, the cell will be removed from the range dictionary.
        /// </summary>
        /// <param name="name">Name of the cell.</param>
        /// <param name="value">New value of the cell.</param>
        void SetNamedCell(string name, object value);
        #endregion
    }
}
