using Athena.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Zeus.UI.Mvvm;
using Zeus.UI.Mvvm.Interfaces;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data binding in the application main view.
    /// </summary>
    class AthenaViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The model associated with this view model instance.
        /// </summary>
        private AthenaModel m_Model;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the visibility of the Pause menu.
        /// </summary>
        public Visibility PauseMenuVisibility
        {
            get { return m_Model.IsPaused ? Visibility.Collapsed : Visibility.Visible; }
        }
        /// <summary>
        /// Gets the visibility of the Resume menu.
        /// </summary>
        public Visibility ResumeMenuVisibility
        {
            get { return m_Model.IsPaused ? Visibility.Visible : Visibility.Collapsed; }
        }
        /// <summary>
        /// Gets or sets a flag that indicates if the autoscroll is enabled.
        /// </summary>
        public bool AutoScroll
        {
            get { return m_Model.AutoScroll; }
            set { m_Model.AutoScroll = value; }
        }
        /// <summary>
        /// Gets the command that handle the Pause/Resume events.
        /// </summary>
        public ICommand PauseResumeCommand { get; private set; }
        /// <summary>
        /// Gets the command that handles the connection to a server.
        /// </summary>
        public ICommand ConnectToServerCommand { get; private set; }
        /// <summary>
        /// Gets the avaialble Athena log servers.
        /// </summary>
        public ObservableCollection<LogServerViewModel> Servers { get; private set; }
        /// <summary>
        /// Gets or sets a flag that indicates if the rows must be coloured or not.
        /// </summary>
        public bool AreRowsColored
        {
            get { return m_Model.AreRowsColored; }
            set { m_Model.AreRowsColored = value; }
        }
        /// <summary>
        /// Gets or sets the refresh time, in milliseconds, for the servers data update.
        /// </summary>
        public int RefreshTime
        {
            get { return m_Model.RefreshTime; }
            set { m_Model.RefreshTime = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Pause or Resume the log message update.
        /// </summary>
        private void PauseResume()
        {
            m_Model.IsPaused = !m_Model.IsPaused;
        }
        /// <summary>
        /// Connects to a server to retrieve messages.
        /// </summary>
        /// <param name="owner">The owner window.</param>
        private void ConnectToServer(object owner)
        {
            ConnectionViewModel connectionVM = new ConnectionViewModel();
            IDialogService dialogService = ServiceLocator.Resolve<IDialogService>();
            bool? result = dialogService.ShowModalDialog(connectionVM, owner as Window);
            if (result.HasValue && result.Value && !string.IsNullOrEmpty(connectionVM.AthenaServerAddress))
            {
                //try to connect to the server
                LogServerModel server = m_Model.AddServer(IPAddress.Parse(connectionVM.AthenaServerAddress));
                if (server != null)
                {
                    Servers.Add(new LogServerViewModel(server));
                }
                else
                {
                    dialogService.ShowError(string.Format("Fail to connect to server {0}", connectionVM.AthenaServerAddress));
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the view model fields.
        /// </summary>
        public AthenaViewModel()
        {
            m_Model = new AthenaModel();
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => PauseMenuVisibility);
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => ResumeMenuVisibility);
            RegisterPropagation(m_Model, () => m_Model.AutoScroll, () => AutoScroll);
            RegisterPropagation(m_Model, () => m_Model.AreRowsColored, () => AreRowsColored);
            RegisterPropagation(m_Model, () => m_Model.RefreshTime, () => RefreshTime);
            PauseResumeCommand = new RelayCommand(PauseResume);
            ConnectToServerCommand = new RelayCommand(ConnectToServer);
            Servers = new ObservableCollection<LogServerViewModel>();
        }

        #endregion
    }
}
