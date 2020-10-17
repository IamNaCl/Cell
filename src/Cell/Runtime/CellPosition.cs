namespace Cell.Runtime
{
    /// <summary>
    /// Represents the position of a cell in
    /// </summary>
    public struct CellPosition
    {
        #region Fields
        /// <summary>
        /// Position in the "letter" part of the cell.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Position in the "number" part of the cell.
        /// </summary>
        public readonly int Y;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new copy of the CellPosition struct.
        /// </summary>
        /// <param name="x">Position in the "letter" part.</param>
        /// <param name="y">Position in the "number" part.</param>
        public CellPosition(int x, int y) => (X, Y) = (x, y);
        #endregion

        #region Operators
        public override string ToString() => $"${X}{Y}";

        public bool Equals(CellPosition other) => X == other.X && Y == other.Y;

        public override bool Equals(object other) => other is CellPosition pos && Equals(pos);

        public override int GetHashCode() => System.HashCode.Combine<int, int>(X, Y);

        public static implicit operator CellPosition((int x, int y) pos) => new CellPosition(pos.x, pos.y);

        public static bool operator <(CellPosition left, CellPosition right) => left.X < right.X && left.Y < right.Y;

        public static bool operator >(CellPosition left, CellPosition right) => left.X > right.X && left.Y > right.Y;

        public static bool operator <=(CellPosition left, CellPosition right) => left < right || left.Equals(right);

        public static bool operator >=(CellPosition left, CellPosition right) => left > right || left.Equals(right);
        #endregion
    }
}
