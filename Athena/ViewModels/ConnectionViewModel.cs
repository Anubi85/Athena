using System.Windows.Input;
using Zeus.UI.Mvvm;
using Zeus.UI.Mvvm.Interfaces;

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
        /// <summary>
        /// The dialog service that managed the application dialogs.
        /// </summary>
        private IDialogService m_DialogService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize view model fields.
        /// </summary>
        public ConnectionViewModel()
        {
            m_ServerAddress = string.Empty;
            m_DialogService = ServiceLocator.Resolve<IDialogService>();
            CloseDialogCommand = new RelayCommand(CloseDialog);
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
        /// <summary>
        /// Gets the command that closes the dialog.
        /// </summary>
        public ICommand CloseDialogCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes the dialog assocated with the view model.
        /// </summary>
        private void CloseDialog()
        {
            m_DialogService.CloseDialog(this, (bool?)true);
        }

        #endregion
    }
}
