namespace Athena.Commons
{
    /// <summary>
    /// Represent a generic log channel.
    /// </summary>
    public interface ILogChannel
    {
        /// <summary>
        /// Initializes the log channel.
        /// </summary>
        /// <returns>Return true if the initialization succeeds, false otherwise.</returns>
        bool Initialize();

        /// <summary>
        /// Writes a new message on the log channel.
        /// </summary>
        /// <param name="msg">The message that has to be processed.</param>
        void WriteMessage(LogMessage msg);
    }
}
