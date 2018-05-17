using Athena.Wcf;
using System.Net;
using Zeus.Data;
using Zeus.Log;
using Zeus.Log.Channels;
using Zeus.Plugin;

namespace Athena.LogChannel
{
    /// <summary>
    /// Implements a log channel that can be used by the Zeus.Log dll to log messages.
    /// </summary>
    [ExportPlugin(typeof(ILogChannel))]
    [ExportPluginMetadata("Name", "AthenaChannel")]
    public sealed class AthenaChannel : LogChannelBase
    {
        #region Fields

        /// <summary>
        /// The WCF client that communicates with the Athena WCF server.
        /// </summary>
        private AthenaWcfClient m_WcfClient;

        #endregion

        #region Constants

        /// <summary>
        /// The key of the settings value that contains information about the Athena WCF server IP address.
        /// </summary>
        private const string c_ServerIpAddressKey = "ServerAddress";
        /// <summary>
        /// The key of the setting value that contains information avout the communication timeout with the server.
        /// </summary>
        private const string c_ServerCommunicationTimeout = "ServerTimeout";

        #endregion

        #region ILogChannel interface

        /// <summary>
        /// Initializes the log channel.
        /// </summary>
        /// <param name="settings">The object that contains channel settings.</param>
        public override void Initialize(DataStore settings)
        {
            m_WcfClient = new AthenaWcfClient(IPAddress.Parse(settings.TryGet<string>(c_ServerIpAddressKey, "127.0.0.1")));
            m_WcfClient.CommunicationTimeout = settings.TryGet<int>(c_ServerCommunicationTimeout, m_WcfClient.CommunicationTimeout);
        }
        /// <summary>
        /// Writes a new message on the Athena log server.
        /// </summary>
        /// <param name="msg">The message that has to be processed.</param>
        public override void WriteMessage(LogMessage msg)
        {
            m_WcfClient.WriteMessage(msg);
        }

        #endregion

        #region IDisposable interface

        /// <summary>
        /// Releases channel resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
