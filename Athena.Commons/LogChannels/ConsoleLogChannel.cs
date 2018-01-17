using System;

namespace Athena.Commons.LogChannels
{
    /// <summary>
    /// Logs a message to the console.
    /// </summary>
    public class ConsoleLogChannel : ILogChannel
    {
        #region ILogChannel interface

        /// <summary>
        /// Initializes the log channel.
        /// </summary>
        /// <returns>Return true if the initialization succeeds, false otherwise.</returns>
        public bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Writes a new message on the console.
        /// </summary>
        /// <param name="msg">The message that has to be processed.</param>
        public void WriteMessage(LogMessage msg)
        {
            ConsoleColor original = Console.ForegroundColor;
            switch(msg.Type)
            {
                case LogMessageType.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogMessageType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogMessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.Write(string.Format("[{0}]:", msg.Type).PadRight(12));
            Console.ForegroundColor = original;
            Console.WriteLine(msg.ApplyFormat("{time:yyyy-MM-dd HH:mm:ss} {methodname}::{text}"));
        }

        #endregion
    }
}
