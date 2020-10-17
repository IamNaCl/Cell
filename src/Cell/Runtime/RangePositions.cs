namespace Cell.Runtime
{
    /// <summary>
    /// Represents a range.
    /// </summary>
    public struct RangePositions
    {
        #region Fields
        /// <summary>
        /// Gets the starting point of the range.
        /// </summary>
        public readonly CellPosition Begin;

        /// <summary>
        /// Gets the end point of the range.
        /// </summary>
        public readonly CellPosition End;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new copy of the Range struct.
        /// </summary>
        /// <param name="begin">Begin position in the range.</param>
        /// <param name="end">End position in the range.</param>
        public RangePositions(CellPosition begin, CellPosition end) => (Begin, End) = (begin, end);
        #endregion

        #region Operators
        public static implicit operator RangePositions((CellPosition b, CellPosition e) pos) =>
            new RangePositions(pos.b, pos.e);

        public override string ToString() => $"{Begin}{End}";
        #endregion
    }
}
