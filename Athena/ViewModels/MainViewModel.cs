using Athena.Models;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Zeus.UI.Mvvm;
using Zeus.UI.Mvvm.Interfaces;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data binding in the application main view.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The model used by the main view.
        /// </summary>
        private OptionsModel m_Options;
        /// <summary>
        /// The timer used to polling the Athena log servers.
        /// </summary>
        private DispatcherTimer m_UpdateTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the visibility of the Pause menu.
        /// </summary>
        public Visibility PauseMenuVisibility
        {
            get { return m_Options.IsPaused ? Visibility.Collapsed : Visibility.Visible; }
        }
        /// <summary>
        /// Gets the visibility of the Continue menu.
        /// </summary>
        public Visibility ContinueMenuVisibility
        {
            get { return m_Options.IsPaused ? Visibility.Visible : Visibility.Collapsed; }
        }
        /// <summary>
        /// Gets the string that will be shown in the status bar and that represent the current application ststus.
        /// </summary>
        public string StatusLabel
        {
            get { return m_Options.IsPaused ? "Paused" : "Online"; }
        }
        /// <summary>
        /// Gets or sets a flag that indicates if the autoscroll is enabled.
        /// </summary>
        public bool AutoScroll
        {
            get { return m_Options.AutoScroll; }
            set { m_Options.AutoScroll = value; }
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

        #endregion

        #region Methods

        /// <summary>
        /// Pause or Resume the log message update.
        /// </summary>
        private void PauseResume()
        {
            m_Options.IsPaused = !m_Options.IsPaused;
        }
        /// <summary>
        /// Connects to a server to retrieve messages.
        /// </summary>
        /// <param name="owner">The owner window.</param>
        private void ConnectToServer(object owner)
        {
            ConnectionViewModel vm = new ConnectionViewModel();
            bool? result = ServiceLocator.Resolve<IDialogService>().ShowModalDialog(vm, owner as Window);
            if (result.HasValue && result.Value && !string.IsNullOrEmpty(vm.AthenaServerAddress))
            {
                LogServerViewModel serverVM = new LogServerViewModel();
                if (serverVM.Connect(IPAddress.Parse(vm.AthenaServerAddress)))
                {
                    Servers.Add(serverVM);
                }
            }
        }
        /// <summary>
        /// Updates the Athena log servers data.
        /// </summary>
        /// <param name="sender">The object that generates the event.</param>
        /// <param name="e">An object that contains information about the occurred event.</param>
        private void UpdateServersCallback(object sender, EventArgs e)
        {
            foreach(LogServerViewModel serverVM in Servers)
            {
                serverVM.Update();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the view model fields.
        /// </summary>
        public MainViewModel()
        {
            m_Options = new OptionsModel();
            RegisterPropagation(m_Options, () => m_Options.IsPaused, () => PauseMenuVisibility);
            RegisterPropagation(m_Options, () => m_Options.IsPaused, () => ContinueMenuVisibility);
            RegisterPropagation(m_Options, () => m_Options.IsPaused, () => StatusLabel);
            PauseResumeCommand = new RelayCommand(PauseResume);
            ConnectToServerCommand = new RelayCommand(ConnectToServer);
            Servers = new ObservableCollection<LogServerViewModel>();
            m_UpdateTimer = new DispatcherTimer(new TimeSpan(5000000), DispatcherPriority.Normal, UpdateServersCallback, Dispatcher.CurrentDispatcher);//perform an update every 500 ms
        }

        #endregion
    }
}
