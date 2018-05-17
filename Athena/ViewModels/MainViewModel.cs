using Athena.Models;
using System.Windows;
using System.Windows.Input;
using Zeus.UI.Mvvm;
using Zeus.UI.Mvvm.Interfaces;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data binding in the application main view.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="OptionsModel"/> object.
        /// </summary>
        public OptionsModel Options { get; private set; }
        /// <summary>
        /// Gets the visibility of the Pause menu.
        /// </summary>
        public Visibility PauseMenuVisibility
        {
            get { return Options.IsPaused ? Visibility.Collapsed : Visibility.Visible; }
        }
        /// <summary>
        /// Gets the visibility of the Continue menu.
        /// </summary>
        public Visibility ContinueMenuVisibility
        {
            get { return Options.IsPaused ? Visibility.Visible : Visibility.Collapsed; }
        }
        /// <summary>
        /// Gets the string that will be shown in the status bar and that represent the current application ststus.
        /// </summary>
        public string StatusLabel
        {
            get { return Options.IsPaused ? "Paused" : "Online"; }
        }
        /// <summary>
        /// Gets the command that handle the Pause/Resume events.
        /// </summary>
        public ICommand PauseResumeCommand { get; private set; }
        /// <summary>
        /// Gets the command that handles the connection to a server.
        /// </summary>
        public ICommand ConnectToServerCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Pause or Resume the log message update.
        /// </summary>
        private void PauseResume()
        {
            Options.IsPaused = !Options.IsPaused;
        }
        /// <summary>
        /// Connects to a server to retrieve messages.
        /// </summary>
        private void ConnectToServer()
        {
            ServiceLocator.Resolve<IDialogService>().ShowDialog(new ConnectionViewModel());
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the view model fields.
        /// </summary>
        public MainViewModel()
        {
            Options = new OptionsModel();
            RegisterPropagation(Options, () => Options.IsPaused, () => PauseMenuVisibility);
            RegisterPropagation(Options, () => Options.IsPaused, () => ContinueMenuVisibility);
            RegisterPropagation(Options, () => Options.IsPaused, () => StatusLabel);
            PauseResumeCommand = new RelayCommand(PauseResume);
            ConnectToServerCommand = new RelayCommand(ConnectToServer);
        }

        #endregion
    }
}
