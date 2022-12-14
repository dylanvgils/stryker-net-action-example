using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Minor.Nijn
{
    /// <summary>
    /// Matcher for checking and matching RabbitMQ topic patterns
    /// </summary>
    public static class TopicMatcher
    {
        private const string ValidTopicExpression = @"^(?:(?:\w+|\*|\#)\.)*(?:\w+|\*|\#)$";
        private const string AsteriskCaptureGroup = @"(?:\w+)";
        private const string HashTagCaptureGroup  = @"(?:\w+\.?)+";

        /// <summary>
        /// Checks if the provided topic expressions are valid and if the provided topic matches one of the expressions
        /// </summary>
        /// <param name="topicExpressions">Topic expressions to check for</param>
        /// <param name="topic">The topic to match</param>
        /// <returns></returns>
        public static bool IsMatch(IEnumerable<string> topicExpressions, string topic)
        {
            return topicExpressions.Any(expr => IsMatch(expr, topic));
        }

        /// <summary>
        /// Checks if the provided topic expression is valid and if the provided topic matches the expression
        /// </summary>
        /// <param name="topicExpression">Topic expression to check for</param>
        /// <param name="topic">The topic to match</param>
        /// <returns></returns>
        public static bool IsMatch(string topicExpression, string topic)
        {
            return topicExpression == topic || MatchTopicExpression(topicExpression, topic);
        }

        /// <summary>
        /// Checks if the provided topics are valid
        /// </summary>
        /// <returns>Returns true when all topics are valid</returns>
        /// <exception cref="BusConfigurationException">This exception is thrown when one of the topic is invalid</exception>
        public static bool AreValidTopicExpressions(IEnumerable<string> topics)
        {
            foreach (var topic in topics)
            {
                IsValidTopic(topic);
            }

            return true;
        }

        private static bool MatchTopicExpression(string expression, string topic)
        {
            IsValidTopic(expression);

            string[] expressionParts = expression.Split('.');
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < expressionParts.Length; i++)
            {
                string expressionPart = expressionParts[i];
                bool isLast = expressionParts.Length == (i + 1);
                builder.Append(ParseExpressionPart(expressionPart, isLast));
            }

            string pattern = "^" + builder + "$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(topic.Trim());
        }

        private static void IsValidTopic(string topic)
        {
            Regex regex = new Regex(ValidTopicExpression, RegexOptions.Compiled);

            if (regex.IsMatch(topic))
            {
                return;
            }

            throw new BusConfigurationException($"Topic expression '{topic}' is invalid");
        }

        private static string ParseExpressionPart(string expressionPart, bool isLast)
        {
            string result;

            switch (expressionPart.Trim())
            {
                case "*":
                    result = AsteriskCaptureGroup; 
                    break;
                case "#":
                    result = HashTagCaptureGroup;
                    break;
                default:
                    result = $"(?:{expressionPart.Trim()})";
                    break;
            }

            return isLast ? result : result + @"\.";
        }
    }
}
