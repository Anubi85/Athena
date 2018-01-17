using System;
using System.Text.RegularExpressions;

namespace Athena.Commons
{
    /// <summary>
    /// Contains the unformatted data of a log message.
    /// </summary>
    public sealed class LogMessage
    {
        #region Constructor

        /// <summary>
        /// Instantiate a new <see cref="LogMessage"/> object and initialize its properties.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="methodName">The name of the method that originate the message.</param>
        /// <param name="processName">The name of the process that originate the message.</param>
        /// <param name="text">The text of the message.</param>
        public LogMessage(LogMessageType type, string methodName, string processName, string text)
        {
            Time = DateTime.UtcNow;
            Type = type;
            MethodName = methodName;
            ProcessName = processName;
            Text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the time at wich the <see cref="LogMessage"/> object has been created.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public LogMessageType Type { get; private set; }

        /// <summary>
        /// Gets the name of the method that generate the message.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the name of the process that generate the message.
        /// </summary>
        public string ProcessName { get; private set; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Text { get; private set; }

        #endregion

        #region Constants

        /// <summary>
        /// Match pattern used by <see cref="Regex.Replace(string, string, string)"/> to format the message.
        /// </summary>
        private const string c_MatchPattern = "(?<={{)(?i){0}(?-i)";

        #endregion

        #region Methods

        /// <summary>
        /// Format the message with the given string format.
        /// </summary>
        /// <param name="format">String format for the message.</param>
        /// <returns>Message data formatted according wih given format string.</returns>
        public string ApplyFormat(string format)
        {
            //convert tags to indexes
            string parsedFormat = format;
            parsedFormat = Regex.Replace(parsedFormat, string.Format(c_MatchPattern, "Date"), "0");
            parsedFormat = Regex.Replace(parsedFormat, string.Format(c_MatchPattern, "Time"), "0");
            parsedFormat = Regex.Replace(parsedFormat, string.Format(c_MatchPattern, "Type"), "1");
            parsedFormat = Regex.Replace(parsedFormat, string.Format(c_MatchPattern, "ProcessName"), "2");
            parsedFormat = Regex.Replace(parsedFormat, string.Format(c_MatchPattern, "MethodName"), "3");
            parsedFormat = Regex.Replace(parsedFormat, string.Format(c_MatchPattern, "Text"), "4");
            try
            {
                return string.Format(parsedFormat, Time, Type, ProcessName, MethodName, Text);
            }
            catch (FormatException)
            {
                return "Bad format string sintax!";
            }
        }

        #endregion
    }
}
