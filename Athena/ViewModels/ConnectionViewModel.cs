using Zeus.UI.Mvvm;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data binding in connection view.
    /// </summary>
    class ConnectionViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The address of the Athena log server.
        /// </summary>
        private string m_ServerAddress;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize view model fields.
        /// </summary>
        public ConnectionViewModel()
        {
            m_ServerAddress = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Athena server address.
        /// </summary>
        public string AthenaServerAddress
        {
            get { return m_ServerAddress; }
            set { Set(ref m_ServerAddress, value); }
        }

        #endregion
    }
}
