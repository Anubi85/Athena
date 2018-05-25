using Athena.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zeus.UI.Mvvm;

namespace Athena.Models
{
    /// <summary>
    /// Represents an Athena log server and allows to interect with it.
    /// </summary>
    class LogServerModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// The address of the Athena log server associated with this object.
        /// </summary>
        private IPAddress m_ServerAddress;
        /// <summary>
        /// Client that handle the communication with the Athena log server.
        /// </summary>
        private AthenaWcfClient m_Client;
        /// <summary>
        /// The date of the last message read from the server.
        /// </summary>
        private DateTime? m_LastReadMessageDate;
        /// <summary>
        /// A flag that indicates if the server is connected or not.
        /// </summary>
        private bool m_IsConnected;

        #endregion

        #region Events

        /// <summary>
        /// Event that notify the avaialbility of new log messages.
        /// </summary>
        public event Action<List<WcfLogMessage>> NewMessagesAvaialble;

        #endregion

        #region Methods

        /// <summary>
        /// Checks if new messages are avaialble on the server and in case rise the <see cref="NewMessagesAvaialble"/> event.
        /// </summary>
        public void Update()
        {
            try
            {
                List<WcfLogMessage> messages = m_Client.GetMessages(m_LastReadMessageDate);
                if (messages.Count != 0)
                {
                    m_LastReadMessageDate = messages.OrderBy(m => m.Time).Last().Time;
                    NewMessagesAvaialble?.Invoke(messages);
                }
                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
            }
        }        

        #endregion

        #region Properties

        /// <summary>
        /// Gets the address of the server associated with this object.
        /// </summary>
        public IPAddress Address
        {
            get { return m_ServerAddress; }
        }
        /// <summary>
        /// Gets a flag that indicates if the server is connected.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
            private set { Set(ref m_IsConnected, value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new object that represent an Athena log server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        public LogServerModel(IPAddress address)
        {
            m_ServerAddress = address;
            m_Client = new AthenaWcfClient(address);
            m_IsConnected = true;
            m_LastReadMessageDate = null;
        }

        #endregion
    }
}
