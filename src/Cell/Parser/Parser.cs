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
        private static bool IsInvalidCellIndex(this string str, out string error)
        {
            error = null;
            if (str is null || str.Length > 9)
                error = "Cell index has more than 9 digits.";

            return error is object;
        }

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
                    IList<IExpression> args = new List<IExpression>();
                    // After matching, is it a function call?
                    if (source.Current == TokenType.BracketL)
                    {
                        source.MatchAndEat(TokenType.BracketL, out error);
                        while (source.Current is object && source.Current != TokenType.BracketR)
                        {
                            // Get the argument and if it failed, just abort.
                            var arg = source.ParseExpression(out error);
                            if (arg is null)
                            {
                                error = "empty expression in function arguments.";
                                return null;
                            }

                            if (source.Current == TokenType.Comma)
                                source.MatchAndEat(TokenType.Comma, out error);

                            args.Add(arg);
                        }
                        source.MatchAndEat(TokenType.BracketR, out error);
                    }

                    // Now cast the stuff into a function call expression.
                    // Everything here is a function, even when no parentheses are involved.
                    return new FunctionCallExpression(current.Value, args);
                }
                case TokenType.BracketL:
                {
                    source.MatchAndEat(TokenType.BracketL, out error);
                    var expr = ParseExpression(source, out error);
                    // Is the previous one null? We don't do that here: "(, <expr>)"
                    if (expr is null)
                    {
                        error = "empty expression within brackets.";
                        return null;
                    }

                    // Are we gathering multiple statements?
                    if (source.Current == TokenType.Comma)
                    {

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
                                error = $"empty expression after comma in block expression.";
                                return null;
                            }

                            // Add the next expression to the list.
                            body.Add(expr);
                        }

                        // We got our block, we're good to go.
                        expr = new BlockExpression(body);
                    }
                    source.MatchAndEat(TokenType.BracketR, out error);
                    return expr;
                }
                case TokenType.Cell:
                {
                    // All of this implementation for cell is dirty. TODO: Clean things up a bit.
                    int begin = 0, end = 0;
                    source.MatchAndEat(TokenType.Cell, out error);

                    // Greater than "$123456789"
                    if (current.Value.IsInvalidCellIndex(out error))
                        return null;

                    // Convert it to integer.
                    begin = int.Parse(current.Value);

                    // Did we get another cell right after the first one?
                    if ((TokenType)source.Current == TokenType.Cell)
                    {
                        if (source.Current.Value.IsInvalidCellIndex(out error))
                            return null;

                        end = int.Parse(source.Current.Value);

                        source.MatchAndEat(TokenType.Cell, out error);
                        return new FunctionCallExpression("GET_RANGE", new LiteralExpression(begin),
                                                          new LiteralExpression(end));
                    }

                    // It is a cell by default so, return it.
                    return new FunctionCallExpression(_functions[TokenType.Cell], new LiteralExpression(begin));
                }
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
                    error = $"unexpected token: '{((TokenType)source.Current).AsString()}'";
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
            source.ParseWhile(ParseEquality,
                           _ => _functions[_],
                           _ => _ == TokenType.Ampersand,
                           out error);

        private static IExpression ParseExpression(this IEnumerator<Token> source, out string error) =>
            // Get the current statement/expression.
            source.ParseConcatenation(out error);
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
            var result = enumerator.ParseExpression(out error);
            if (enumerator.Current is object)
            {
                error = "there are tokens left on input, are you missing a pair of brackets?";
                result = null;
            }
            return result;
        }
        #endregion
    }
}
