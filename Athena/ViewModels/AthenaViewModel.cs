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
        /// <summary>
        /// The application options view model.
        /// </summary>
        private OptionsViewModel m_Options;
        /// <summary>
        /// Dialog service used to display dialogs.
        /// </summary>
        private IDialogService m_Dialogservice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a flag that indicates if the data refresh is paused.
        /// </summary>
        public bool IsPaused
        {
            get { return m_Model.IsPaused; }
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
        /// Gets a flag that indicates if the rows must be coloured or not.
        /// </summary>
        public bool AreRowsColored
        {
            get { return m_Options.AreRowsColored; }
        }
        /// <summary>
        /// Gets the refresh time, in milliseconds, for the servers data update.
        /// </summary>
        public int RefreshTime
        {
            get { return m_Options.RefreshTime; }
        }
        /// <summary>
        /// Gets the command that allows to display the options dialog.
        /// </summary>
        public ICommand ShowOptionsCommand { get; private set; }

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
            bool? result = m_Dialogservice.ShowModalDialog(connectionVM, owner as Window);
            if (result.HasValue && result.Value && !string.IsNullOrEmpty(connectionVM.AthenaServerAddress))
            {
                //add the new server
                Servers.Add(new LogServerViewModel(m_Model.AddServer(IPAddress.Parse(connectionVM.AthenaServerAddress))));
            }
        }
        /// <summary>
        /// Display the options dialog.
        /// </summary>
        /// <param name="owner">The owner window.</param>
        private void ShowOptionsDialog(object owner)
        {
            m_Dialogservice.ShowDialog(m_Options, owner as Window, (vm) => m_Model.ChangeRefreshTimerInterval((vm as OptionsViewModel).RefreshTime));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the view model fields.
        /// </summary>
        public AthenaViewModel()
        {
            m_Dialogservice = ServiceLocator.Resolve<IDialogService>();
            m_Model = new AthenaModel();
            m_Options = new OptionsViewModel();
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => IsPaused);
            RegisterPropagation(m_Model, () => m_Model.AutoScroll, () => AutoScroll);
            RegisterPropagation(m_Options, () => m_Options.AreRowsColored, () => AreRowsColored);
            RegisterPropagation(m_Options, () => m_Options.RefreshTime, () => RefreshTime);
            PauseResumeCommand = new RelayCommand(PauseResume);
            ConnectToServerCommand = new RelayCommand(ConnectToServer);
            ShowOptionsCommand = new RelayCommand(ShowOptionsDialog);
            Servers = new ObservableCollection<LogServerViewModel>();
            m_Model.ChangeRefreshTimerInterval(m_Options.RefreshTime);
        }

        #endregion
    }
}
