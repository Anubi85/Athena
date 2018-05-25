using System;
using System.Collections.Generic;
using System.Linq;
using Zeus;

namespace Athena.Wcf
{
    /// <summary>
    /// Implements the <see cref="IAthenaWcfService"/> WCF server.
    /// </summary>
    internal class AthenaWcfServer : IAthenaWcfService
    {
        #region Constants

        /// <summary>
        /// Default size for the message buffer.
        /// </summary>
        private const int c_DefaultMessageBufferSize = 1024;

        #endregion

        #region Fields

        /// <summary>
        /// The messages buffer.
        /// </summary>
        private static CircularBuffer<WcfLogMessage> s_MessageBuffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the static fields.
        /// </summary>
        static AthenaWcfServer()
        {
            s_MessageBuffer = new CircularBuffer<WcfLogMessage>(c_DefaultMessageBufferSize);
        }

        #endregion

        #region IAthenaWcfService interface

        /// <summary>
        /// Sends a <see cref="WcfLogMessage"/> to the server.
        /// </summary>
        /// <param name="msg">The message that as to be logged.</param>
        public void WriteMessage(WcfLogMessage msg)
        {
            lock(s_MessageBuffer.SyncRoot)
            {
                s_MessageBuffer.Add(msg);
            }
        }
        /// <summary>
        /// Retrieve the messages that have been logged after the <paramref name="startDate"/>.
        /// </summary>
        /// <param name="startDate">The minimum message date.</param>
        /// <returns>The list of all the message logged after the <paramref name="startDate"/>.</returns>
        public List<WcfLogMessage> GetMessages(DateTime? startDate)
        {
            lock (s_MessageBuffer.SyncRoot)
            {
                if (startDate.HasValue)
                {
                    return s_MessageBuffer.Where(msg => msg.Time > startDate.Value).ToList();
                }
                else
                {
                    return s_MessageBuffer.ToList();
                }
            }
        }

        #endregion
    }
}
