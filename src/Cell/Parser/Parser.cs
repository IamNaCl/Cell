using System;
using System.Collections.Generic;
using Cell.Lexer;
using Cell.Parser.Expressions;

namespace Cell.Parser
{
    /// <summary>
    /// Handles the parsing of tokens.
    /// </summary>
    public static class Parser
    {
        #region Parser Privates
        private static IDictionary<TokenType, string> _functions = new Dictionary<TokenType, string>
        {
            [TokenType.Plus] = "ADD",
            [TokenType.Minus] = "SUBTRACT",
            [TokenType.Star] = "MULTIPLY",
            [TokenType.Slash] = "DIVIDE",
            [TokenType.GreaterEq] = "GREATER_EQUAL",
            [TokenType.GreaterT] = "GREATER_THAN",
            [TokenType.LessT] = "LESS_THAN",
            [TokenType.LessEq] = "LESS_EQUAL",
            [TokenType.Equal] = "EQUAL",
            [TokenType.NotEqual] = "NOT_EQUAL",
            [TokenType.Ampersand] = "CONCAT",
            [TokenType.Cell] = "GET_CELL"
        };
        #endregion

        #region Match and Eat
        /// <summary>
        /// Matches the current token against a certain type.
        /// </summary>
        /// <param name="source">Source enumerator.</param>
        /// <param name="type">Token type to match.</param>
        /// <param name="error">Output error string.</param>
        /// <returns>True if advanced successfully, otherwise false.</returns>
        private static bool MatchAndEat(this IEnumerator<Token> source, TokenType type, out string error)
        {
            error = null;

            if (source is null)
            {
                error = "Source enumerator is null.";
                return false;
            }

            if (source.Current == type)
                return source.MoveNext();

            error = $"Unexpected '{source.Current.Type.AsString()}', expected '{type.AsString()}'";
            return false;
        }
        #endregion

        #region Generic Parsing Functions
        /// <summary>
        /// Delegate used
        /// </summary>
        /// <param name="source">Source enumerator.</param>
        /// <param name="error">Output error string.</param>
        /// <returns>Instance of a class that implements IExpression.</returns>
        private delegate IExpression UpperDelegate(IEnumerator<Token> source, out string error);

        /// <summary>
        /// Parses while a certain condition matches.
        /// </summary>
        /// <param name="source">Source enumerator.</param>
        /// <param name="functionName">Function used to get the name of the function that should be called.</param>
        /// <param name="upper">Previous function to get the arguments from.</param>
        /// <param name="condition">Checks whether the token type of the current token matches the current.</param>
        /// <param name="error">Output error string.</param>
        /// <returns>Instance of a class that implements IExpression.</returns>
        private static IExpression ParseWhile(this IEnumerator<Token> source,
                                              UpperDelegate upper,
                                              Func<TokenType, string> functionName,
                                              Predicate<TokenType> condition,
                                              out string error)
        {
            var left = upper(source, out error);

            if (condition((TokenType)source.Current) && left is null)
                return null;

            while (condition((TokenType)source.Current))
            {
                var type = (TokenType)source.Current;
                source.MatchAndEat(type, out error);

                var right = upper(source, out error);
                if (right is null)
                {
                    error = "Expected right side of the expression.";
                    return null;
                }

                left = new FunctionCallExpression(functionName(type), left, right);
            }

            return left;
        }

        /// <summary>
        /// Parses if a certain condition matches.
        /// </summary>
        /// <param name="source">Source enumerator.</param>
        /// <param name="functionName">Function used to get the name of the function that should be called.</param>
        /// <param name="upper">Previous function to get the arguments from.</param>
        /// <param name="condition">Checks whether the token type of the current token matches the current.</param>
        /// <param name="error">Output error string.</param>
        /// <returns>Instance of a class that implements IExpression.</returns>
        private static IExpression ParseIf(this IEnumerator<Token> source,
                                              UpperDelegate upper,
                                              Func<TokenType, string> functionName,
                                              Predicate<TokenType> condition,
                                              out string error)
        {
            var left = upper(source, out error);
            if (condition((TokenType)source.Current) && left is null)
                return null;

            if (condition((TokenType)source.Current))
            {
                var type = (TokenType)source.Current;
                source.MatchAndEat(type, out error);

                var right = upper(source, out error);
                if (right is null)
                {
                    error = "Expected right side of the expression.";
                    return null;
                }

                left = new FunctionCallExpression(functionName(type), left, right);
            }

            return left;
        }
        #endregion

        #region Parser Functions
        // Parses an arithmethic factor.
        private static IExpression ParseFactor(this IEnumerator<Token> source, out string error)
        {
            error = null;
            var current = source.Current;

            switch ((TokenType)current)
            {
                case TokenType.Number:
                {
                    source.MatchAndEat(TokenType.Number, out error);
                    return new LiteralExpression(double.Parse(current.Value));
                }
                case TokenType.String:
                {
                    source.MatchAndEat(TokenType.String, out error);
                    return new LiteralExpression(current.Value);
                }
                case TokenType.Name:
                {
                    source.MatchAndEat(TokenType.Name, out error);
                    IList<IExpression> args = null;
                    // After matching, is it a function call?
                    if (source.Current == TokenType.BracketL)
                    {
                        // Tell the parser that we want another expression, which should be parenthesized.
                        args = source.ParseExpression(out error) switch
                        {
                            BlockExpression block => block.Body,
                            IExpression expr => new List<IExpression> { expr },
                            _ => null
                        };
                    }

                    // Now cast the stuff into a function call expression.
                    // Everything here is a function, even when no parentheses are involved.
                    return new FunctionCallExpression(current.Value, args);
                }
                case TokenType.BracketL:
                {
                    source.MatchAndEat(TokenType.BracketL, out error);
                    var expr = ParseExpression(source, out error);
                    source.MatchAndEat(TokenType.BracketR, out error);
                    return expr;
                }
                case TokenType.Cell:
                {
                    // Match the cell.
                    source.MatchAndEat(TokenType.Cell, out error);
                    try
                    {
                        var begin = current.Value.ToCellPosition();

                        // The tokens say that this is actually a range, not a single cell.
                        if (source.Current == TokenType.Cell)
                        {
                            var end = source.Current.Value.ToCellPosition();
                            return new FunctionCallExpression("GET_RANGE", new List<IExpression>
                            {
                                new LiteralExpression(begin.X), new LiteralExpression(begin.Y),
                                new LiteralExpression(end.X), new LiteralExpression(end.Y),
                            });
                        }

                        // Otherwise, just return the range with literals.
                        return new FunctionCallExpression("GET_CELL", new List<IExpression>
                        {
                            new LiteralExpression(begin.X), new LiteralExpression(begin.Y),
                        });
                    }
                    catch
                    {
                        error = "Numeric value in cell or range is too big.";
                    }
                } break;
            }

            return null;
        }

        // Negates an arithmethic factor.
        private static IExpression ParseNegatedFactor(this IEnumerator<Token> source, out string error)
        {
            if (((TokenType)source.Current).TokenIs(TokenType.Plus, TokenType.Minus))
            {
                // Do we need to negate the current outcome?
                var negate = source.Current == TokenType.Minus;

                source.MatchAndEat((TokenType)source.Current, out error);
                var factor = source.ParseFactor(out error);

                // In case of something like this happens: --5.
                if (factor is null)
                {
                    error = $"Unexpected token: '{((TokenType)source.Current).AsString()}'";
                    return null;
                }

                // If you give me a +(-5) then you want to invert the -5 into 5, that's why we return the abs here.
                return new FunctionCallExpression(negate? "NEGATE": "ABS", factor);
            }

            return source.ParseFactor(out error);
        }

        // Parses an arithmethic term ('*' or '/')
        private static IExpression ParseArithmethicTerm(this IEnumerator<Token> source, out string error) =>
            source.ParseWhile(ParseNegatedFactor,
                              _ => _functions[_],
                              _ => _.TokenIs(TokenType.Star, TokenType.Slash),
                              out error);

        // Parses an arithmethic expression.
        private static IExpression ParseArithmethicExpression(this IEnumerator<Token> source, out string error) =>
            source.ParseWhile(ParseArithmethicTerm,
                              _ => _functions[_],
                              _ => _.TokenIs(TokenType.Plus, TokenType.Minus),
                              out error);

        // Parse a boolean relation.
        private static IExpression ParseRelation(this IEnumerator<Token> source, out string error) =>
            source.ParseIf(ParseArithmethicExpression,
                           _ => _functions[_],
                           _ => _.TokenIs(TokenType.GreaterEq, TokenType.GreaterT, TokenType.LessT, TokenType.LessEq),
                           out error);

        // Parse equality.
        private static IExpression ParseEquality(this IEnumerator<Token> source, out string error) =>
            source.ParseIf(ParseRelation,
                           _ => _functions[_],
                           _ => _.TokenIs(TokenType.Equal, TokenType.NotEqual),
                           out error);

        // <equality> & <equality>
        private static IExpression ParseConcatenation(this IEnumerator<Token> source, out string error) =>
            source.ParseIf(ParseEquality,
                           _ => _functions[_],
                           _ => _ == TokenType.Ampersand,
                           out error);

        private static IExpression ParseExpression(this IEnumerator<Token> source, out string error)
        {
            // Get the current statement/expression.
            var expr = source.ParseConcatenation(out error);

            // Are we gathering multiple statements?
            if (source.Current == TokenType.Comma)
            {
                // Is the previous one null? We don't do that here: "(, <expr>)"
                if (expr is null)
                {
                    error = "Empty expression before comma in block.";
                    return null;
                }

                // Create our brand new list of expressions for the block expression.
                // Then iterate until the end of the enumerator.
                var body = new List<IExpression> { expr };
                while (error is null && source.Current == TokenType.Comma)
                {
                    // Ignore the comma and get the next expression.
                    source.MatchAndEat(TokenType.Comma, out error);
                    expr = source.ParseConcatenation(out error);

                    // If the next expression is null, again, we don't do that here: "(<expr>, )"
                    if (expr is null)
                    {
                        error = $"Empty expression after comma in block expression.";
                        return null;
                    }

                    // Add the next expression to the list.
                    body.Add(expr);
                }

                // We got our block, we're good to go.
                expr = new BlockExpression(body);
            }

            return expr;
        }
        #endregion

        #region Parse
        /// <summary>
        /// Parses the collection of tokens provided as argument.
        /// </summary>
        /// <param name="tokens">Collection of tokens to parse.</param>
        /// <param name="error">Error message produced by the parser.</param>
        /// <returns>Expression produced by parsing the input list of tokens.</returns>
        public static IExpression Parse(IList<Token> tokens, out string error)
        {
            error = null;
            if (tokens is null || tokens.Count == 0)
                return null;

            // Get the enumerator and set the first token.
            var enumerator = tokens.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.ParseExpression(out error);
        }
        #endregion
    }
}
