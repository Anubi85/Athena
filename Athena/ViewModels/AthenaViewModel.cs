using Athena.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zeus.Config;
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
        private OptionsModel m_Options;
        /// <summary>
        /// The Athena log server currently selected.
        /// </summary>
        private LogServerViewModel m_SelectedServer;
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
        /// Gets the visibility of the paused menu item.
        /// </summary>
        public Visibility PauseMenuVisibility
        {
            get { return m_Model.IsPaused ? Visibility.Collapsed : Visibility.Visible; }
        }
        /// <summary>
        /// Gets the visibility of the resume menu item.
        /// </summary>
        public Visibility ResumeMenuVisibility
        {
            get { return m_Model.IsPaused ? Visibility.Visible : Visibility.Collapsed; }
        }
        /// <summary>
        /// Gets the visibility of the paused status bar item.
        /// </summary>
        public Visibility PauseLabelVisibility
        {
            get { return m_Model.IsPaused ? Visibility.Visible : Visibility.Collapsed; }
        }
        /// <summary>
        /// Gets the visibility of the No Server Connected status bar item.
        /// </summary>
        public Visibility NoServerConnectedLabelVisibility
        {
            get { return Servers.Count == 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        /// <summary>
        /// Gets the visibility of the Server Statuc status bar item.
        /// </summary>
        public Visibility ServerStatusLabelVisibility
        {
            get { return Servers.Count == 0 ? Visibility.Collapsed : Visibility.Visible; }
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
        /// Gets or sets the currently selected Athena log server.
        /// </summary>
        public LogServerViewModel SelectedServer
        {
            get { return m_SelectedServer; }
            set { Set(ref m_SelectedServer, value); }
        }
        /// <summary>
        /// Gets the text of the Server Status status bar item.
        /// </summary>
        public string ServerStatusLabelText
        {
            get
            {
                if (m_SelectedServer == null)
                {
                    return "No Server Selected";
                }
                else
                {
                    return m_SelectedServer.IsConnected ? "Online" : "Offline";
                }
            }
        }
        /// <summary>
        /// Gets the background color of the Server Status statud bar item.
        /// </summary>
        public Color? ServerStatusLabelBackgroundColor
        {
            get
            {
                if (m_Model.IsPaused)
                {
                    return (Color)Application.Current.FindResource("YellowColor");
                }
                else
                {
                    if (m_SelectedServer == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (m_SelectedServer.IsConnected)
                        {
                            return (Color)Application.Current.FindResource("GreenColor");
                        }
                        else
                        {
                            return (Color)Application.Current.FindResource("RedColor");
                        }
                    }
                }
            }
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
                Servers.Add(new LogServerViewModel(m_Model.AddServer(IPAddress.Parse(connectionVM.AthenaServerAddress)), m_Options));
            }
        }
        /// <summary>
        /// Display the options dialog.
        /// </summary>
        /// <param name="owner">The owner window.</param>
        private void ShowOptionsDialog(object owner)
        {
            m_Dialogservice.ShowDialog(new OptionsViewModel(m_Options), owner as Window, (vm) => m_Model.ChangeRefreshTimerInterval((vm as OptionsViewModel).RefreshTime));
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
            m_Options = ConfigManager.LoadSection<OptionsModel>();
            //if section not found create it
            if (m_Options == null)
            {
                m_Options = new OptionsModel();
                ConfigManager.SaveSection<OptionsModel>(m_Options);
            }
            m_Model.ChangeRefreshTimerInterval(m_Options.RefreshTime);
            Servers = new ObservableCollection<LogServerViewModel>();
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => IsPaused);
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => PauseMenuVisibility);
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => ResumeMenuVisibility);
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => PauseLabelVisibility);
            RegisterPropagation(Servers, () => Servers.Count, () => NoServerConnectedLabelVisibility);
            RegisterPropagation(Servers, () => Servers.Count, () => ServerStatusLabelVisibility);
            RegisterPropagation(m_Model, () => m_Model.AutoScroll, () => AutoScroll);
            RegisterPropagation(this, () => SelectedServer, () => ServerStatusLabelText);
            RegisterPropagation(this, () => SelectedServer, () => ServerStatusLabelBackgroundColor);
            RegisterPropagation(m_Model, () => m_Model.IsPaused, () => ServerStatusLabelBackgroundColor);
            RegisterPropagation(m_Options, () => m_Options.AreRowsColored, () => AreRowsColored);
            RegisterPropagation(m_Options, () => m_Options.RefreshTime, () => RefreshTime);
            PauseResumeCommand = new RelayCommand(PauseResume);
            ConnectToServerCommand = new RelayCommand(ConnectToServer);
            ShowOptionsCommand = new RelayCommand(ShowOptionsDialog);                        
        }

        #endregion
    }
}
