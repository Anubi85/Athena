using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Athena.Wcf
{
    /// <summary>
    /// Defines methods that must be implemented by the <see cref="AthenaWcfServer"/> WCF service.
    /// </summary>
    [ServiceContract]
    internal interface IAthenaWcfService
    {
        /// <summary>
        /// Sends a <see cref="WcfLogMessage"/> to the server.
        /// </summary>
        /// <param name="msg">The message that as to be logged.</param>
        [OperationContract]
        void WriteMessage(WcfLogMessage msg);
        /// <summary>
        /// Retrieve the messages that have been logged after the <paramref name="startDate"/>.
        /// </summary>
        /// <param name="startDate">The minimum message date.</param>
        /// <returns>The list of all the message logged after the <paramref name="startDate"/>.</returns>
        [OperationContract]
        List<WcfLogMessage> GetMessages(DateTime? startDate);
    }
}
