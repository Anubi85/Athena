using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Commons
{
    /// <summary>
    /// Contains data relative to an <see cref="InternalLogger"/> log message.
    /// </summary>
    internal struct InternalLogMessage
    {
        /// <summary>
        /// Message date and time.
        /// </summary>
        public DateTime Date;
        /// <summary>
        /// Name of the process that generate the message.
        /// </summary>
        public string ProcessName;
        /// <summary>
        /// Log message.
        /// </summary>
        public string Message;
    }
}
