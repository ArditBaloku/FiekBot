namespace FiekBot.Expressions
{
    public interface IExpressionEvaluator
    {
        /// <summary>
        /// Evaluates an expression and returns a string representation of the result.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        /// <returns></returns>
        string Evaluate(string expression);
    }
}
