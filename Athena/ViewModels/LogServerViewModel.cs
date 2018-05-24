using System.Net;
using Zeus.UI.Mvvm;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data bingind in server logged messages view.
    /// </summary>
    public class LogServerViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The IP address of the Athena log server associated with this view model.
        /// </summary>
        private IPAddress m_ServerAddress;

        #endregion

        #region Methods

        /// <summary>
        /// Connect to the server specified in <paramref name="address"/> parameter.
        /// </summary>
        /// <param name="address">The IP address of the Athena log server.</param>
        /// <returns>Return true if the connection succeed, false otherwise.</returns>
        public bool Connect(IPAddress address)
        {
            m_ServerAddress = address;
            return true;
        }

        /// <summary>
        /// Interrogate the server in order to download new messages if any.
        /// </summary>
        public void Update()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Athena log server name.
        /// </summary>
        public string ServerName
        {
            get { return m_ServerAddress?.ToString(); }
        }

        #endregion
    }
}
