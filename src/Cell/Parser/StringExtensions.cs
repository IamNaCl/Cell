namespace Cell.Parser
{
    /// <summary>
    /// Contains extension methods for strings.
    /// </summary>
    static class StringExtensions
    {
        /// <summary>
        /// Contverts a string to an integer that fits inside a cell.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>Integer for the letter part of the cell.</returns>
        public static int ToCellInteger(this string str)
        {
            if (str is null || str.Length == 0)
                return 0;

            // I know this isn't the best way to do it, but
            var res = 0;
            var len = str.Length;

            if (len >= 1)
                res |= (byte)(str[0] & 0xFF);

            if (len >= 2)
                res |= (byte)(str[1] & 0xFF) << 8;

            if (len >= 3)
                res |= (byte)(str[2] & 0xFF) << 16;

            if (len >= 4)
                res |= (byte)(str[3] & 0xFF) << 24;

            return res;
        }

        /// <summary>
        /// Converts the input string to a cell position.
        /// </summary>
        /// <param name="str">Input string formatted as a cell position.</param>
        /// <returns>Cell position.</returns>
        public static Runtime.CellPosition ToCellPosition(this string str)
        {
            var match = Lexer.Definitions.CellDefinition.Instance.MatchRegex(str);
            return (match.Groups[1].Value.ToCellInteger(), int.Parse(match.Groups[2].Value));
        }
    }
}
