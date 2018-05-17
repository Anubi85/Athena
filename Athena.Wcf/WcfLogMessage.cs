using System;
using System.Runtime.Serialization;
using Zeus.Log;

namespace Athena.Wcf
{
    /// <summary>
    /// Defines a log message that can be exchange throught the Athena WCF service.
    /// </summary>
    [DataContract]
    internal class WcfLogMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the log message time.
        /// </summary>
        [DataMember]
        public DateTime Time { get; set; }
        /// <summary>
        /// Gets or sets the log message level.
        /// </summary>
        [DataMember]
        public LogLevels Level { get; set; }
        /// <summary>
        /// Gets or sets the name of the process that generate the message.
        /// </summary>
        [DataMember]
        public string ProcessName { get; set; }
        /// <summary>
        /// Gets or sets the name of the method that generate the message.
        /// </summary>
        [DataMember]
        public string MethodName { get; set; }
        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create a <see cref="WcfLogMessage"/> object from a <see cref="LogMessage"/> object.
        /// </summary>
        /// <param name="msg">The <see cref="LogMessage"/> object that needs to be converted.</param>
        /// <returns>The newly created <see cref="WcfLogMessage"/> object.</returns>
        public static WcfLogMessage FromLogMessage(LogMessage msg)
        {
            WcfLogMessage res = new WcfLogMessage();
            res.Level = msg.Level;
            res.Time = msg.Time;
            res.MethodName = msg.MethodName;
            res.ProcessName = msg.ProcessName;
            res.Text = msg.Text;
            return res;
        }

        #endregion
    }
}
