using System.Collections.Generic;
using System.Net;
using System.Threading;
using Zeus.UI.Mvvm;

namespace Athena.Models
{
    /// <summary>
    /// The application options model.
    /// </summary>
    class AthenaModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// A flags that indicates if the log message update is paused or not.
        /// </summary>
        private bool m_IsPaused;
        /// <summary>
        /// A flag that indicates if the message list shall sroll automatically.
        /// </summary>
        private bool m_AutoScroll;
        /// <summary>
        /// The time interval between two refresh in milliseconds;
        /// </summary>
        private int m_RefreshTime;
        /// <summary>
        /// Collection of the monitored Athena servers.
        /// </summary>
        private Dictionary<IPAddress, LogServerModel> m_Servers;
        /// <summary>
        /// The timer that handles the server data update.
        /// </summary>
        private Timer m_RefreshTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the flag that pause/resume the log message update.
        /// </summary>
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { Set(ref m_IsPaused, value); }
        }
        /// <summary>
        /// Gets or sets the flag that enable or disable the log message list auto scroll.
        /// </summary>
        public bool AutoScroll
        {
            get { return m_AutoScroll; }
            set { Set(ref m_AutoScroll, value); }
        }
        /// <summary>
        /// Gets or sets the refresh time, in milliseconds, for the servers data update.
        /// </summary>
        public int RefreshTime
        {
            get { return m_RefreshTime; }
            set
            {
                Set(ref m_RefreshTime, value);
                m_RefreshTimer.Change(0, m_RefreshTime);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update timer callback functions. Perform an update of all the avaialble Athena log servers.
        /// </summary>
        /// <param name="state">An object used for status syncronization purposes.</param>
        private void UpdateServers(object state)
        {
            foreach (LogServerModel server in m_Servers.Values)
            {
                server.Update();
            }
        }
        /// <summary>
        /// Add a new Athena log server to the monitored servers collection.
        /// </summary>
        /// <param name="address">The address of the server that has to be added.</param>
        /// <returns>The <see cref="LogServerModel"/> associated with the requested server if succeed, null otherwise.</returns>
        public LogServerModel AddServer(IPAddress address)
        {
            LogServerModel newServer = new LogServerModel(address);
            m_Servers.Add(address, newServer);
            return newServer;
        }
        /// <summary>
        /// Remove an Athena log server from the monitored servers collection.
        /// </summary>
        /// <param name="address">The address of the server that has to be removed.</param>
        /// <returns>true if the server has been sucessfully removed, falce othervise</returns>
        public bool RemoveServer(IPAddress address)
        {
            if (m_Servers.ContainsKey(address))
            {
                m_Servers.Remove(address);
                return true;
            }
            return false;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Intializes class internal fields and starts the update timer.
        /// </summary>
        public AthenaModel()
        {
            m_Servers = new Dictionary<IPAddress, LogServerModel>();
            m_RefreshTime = 15000;
            m_RefreshTimer = new Timer(UpdateServers, new object(), 0, m_RefreshTime);
        }

        #endregion
    }
}
