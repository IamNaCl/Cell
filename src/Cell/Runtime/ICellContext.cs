using System.Collections.Generic;

namespace Cell.Runtime
{
    /// <summary>
    /// Represents a memory context in Cell, an instance of this is used to store results and other information about
    /// the expression that was last executed.
    /// </summary>
    public interface ICellContext
    {
        #region Indexers
        /// <summary>
        /// Gets or sets the value of a cell.
        /// </summary>
        object this[int index] { get; set; }

        /// <summary>
        /// Gets a range.
        /// </summary>
        IReadOnlyDictionary<int, object> this[int startIndex, int endIndex] { get; }

        /// <summary>
        /// Gets a function defined in this context by its name, or null if it doesn't exist.
        /// If the new value of the function is null, the function will be removed from the list.
        /// </summary>
        IFunction this[string functionName] { get; }
        #endregion

        #region GetCell/SetCell/GetRange/SetRange
        /// <summary>
        /// Gets the value of a cell.
        /// </summary>
        /// <param name="index">Cell index.</param>
        /// <returns>Whatever is stored in this cell, or null if the cell is yet to be used.</returns>
        object GetCell(int index);

        /// <summary>
        /// Sets the value of a cell.
        /// </summary>
        /// <param name="index">Cell index to update/set.</param>
        /// <param name="value">New value of the cell.</param>
        void SetCell(int index, object value);

        /// <summary>
        /// Gets a collection of cells.
        /// </summary>
        /// <param name="startIndex">Start position in the range.</param>
        /// <param name="endIndex">End position in the range.</param>
        /// <returns>
        /// Enumerable of int/object pairs, the key represents the index of the cell and the value is the actual value
        /// under that cell, if null then the cell is empty.
        /// </returns>
        IReadOnlyDictionary<int, object> GetRange(int startIndex, int endIndex);

        /// <summary>
        /// Sets an entire range to a specific value.
        /// </summary>
        /// <param name="startIndex">Start position in the range.</param>
        /// <param name="endIndex">End position in the range.</param>
        /// <param name="value">New value to set, if null the objects will be removed from the dictionary.</param>
        void SetRange(int startIndex, int endIndex, object value);

        /// <summary>
        /// Sets a range with the values of other range.
        /// If the source range is smaller than the destination range, then the value is replicated until the end.
        /// If the destination range is emaller than the source range, only destination.length is copied.
        /// If any of the values within source range is null then the value will be removed from the destination range.
        /// </summary>
        /// <param name="sourceStart">Start index of the source range.</param>
        /// <param name="sourceEnd">End index of the source range.</param>
        /// <param name="destStart">Start index of the destination range.</param>
        /// <param name="destEnd">End index of the destination range.</param>
        void SetRange(int sourceStart, int sourceEnd, int destStart, int destEnd);
        #endregion
    }
}
