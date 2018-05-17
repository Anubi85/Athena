using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using Zeus.Log;

namespace Athena.Wcf
{
    /// <summary>
    /// An implementation for a client to the Athena WCF service.
    /// </summary>
    internal class AthenaWcfClient
    {
        #region Helper interface

        /// <summary>
        /// Helper interface that allows to use the <see cref="ICommunicationObject"/> and <see cref="IAthenaWcfService"/> interfaces methods.
        /// </summary>
        interface IChannel : IAthenaWcfService, ICommunicationObject { }

        #endregion

        #region Constants

        /// <summary>
        /// Default timeput value in milliseconds.
        /// </summary>
        private const int c_DefaultCommunicationTimeout = 20;

        #endregion

        #region Fields

        /// <summary>
        /// The WCF channels factory.
        /// </summary>
        private ChannelFactory<IChannel> m_ChannelFactory;
        /// <summary>
        /// The timeout object used in channel opening.
        /// </summary>
        private TimeSpan m_CommunicationTimeout;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new client with the specified endpoint.
        /// </summary>
        /// <param name="address">The endpoint address of the Athena WCF server.</param>
        public AthenaWcfClient(IPAddress address)
        {
            m_CommunicationTimeout = new TimeSpan(c_DefaultCommunicationTimeout * 10000);//10000 converts from ms to ticks
            m_ChannelFactory = new ChannelFactory<IChannel>(Utility.GetEndpoint(address));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new channel and open it.
        /// </summary>
        /// <returns>The newly created channel.</returns>
        private IAthenaWcfService CreateAndOpenChannel()
        {
            IChannel ch = m_ChannelFactory.CreateChannel();
            ch.Open(m_CommunicationTimeout);
            return ch;
        }

        /// <summary>
        /// Closes the given channel.
        /// </summary>
        /// <param name="athenaChannel">The channel to be close.</param>
        private void CloseChannel(IAthenaWcfService athenaChannel)
        {
            IChannel ch = athenaChannel as IChannel;
            try
            {
                ch.Close(m_CommunicationTimeout);
            }
            catch (TimeoutException)
            {
                ch.Abort();
            }
        }

        /// <summary>
        /// Sends a <see cref="LogMessage"/> to the server.
        /// </summary>
        /// <param name="msg">The message that as to be logged.</param>
        public void WriteMessage(LogMessage msg)
        {
            IAthenaWcfService ch = CreateAndOpenChannel();
            ch.WriteMessage(WcfLogMessage.FromLogMessage(msg));
            CloseChannel(ch);
        }

        /// <summary>
        /// Retrieve the messages that have been logged after the <paramref name="startDate"/>.
        /// </summary>
        /// <param name="startDate">The minimum message date.</param>
        /// <returns>The list of all the message logged after the <paramref name="startDate"/>.</returns>
        public List<WcfLogMessage> GetMessages(DateTime? startDate)
        {
            IAthenaWcfService ch = CreateAndOpenChannel();
            List<WcfLogMessage> res = ch.GetMessages(startDate);
            CloseChannel(ch);
            return res;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the communication timeout for channel opening and closing opearations.
        /// </summary>
        public int CommunicationTimeout
        {
            get { return (int)m_CommunicationTimeout.TotalMilliseconds; }
            set { m_CommunicationTimeout = new TimeSpan(value * 10000); }//10000 convert from ms to ticks
        }

        #endregion
    }
}
