using CommandLine;

namespace Cell.Repl
{
    /// <summary>
    /// Contains all the possible options that can be handled by the Cell repl.
    /// </summary>
    class CommandLineOptions
    {
        /// <summary>
        /// Is it an interactive session?
        /// </summary>
        [Option('i', "interactive", Required = false, HelpText = "Runs the REPL in interactive mode.")]
        public bool Interactive { get; set; }

        /// <summary>
        /// Evaluates a string (overrides interactive).
        /// </summary>
        [Option('e', "eval", Required = false, HelpText = "Evaluates an input string.")]
        public string EvaluateString { get; set; }

        /// <summary>
        /// Gets the name of the input file (overrides both interactive and string).
        /// </summary>
        [Option('f', "input-file", HelpText = "Runs the contents of a Cell script.")]
        public string InputFile { get; set; }
    }
}
