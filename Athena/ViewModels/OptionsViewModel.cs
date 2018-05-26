using Athena.Models;
using System.Windows.Input;
using Zeus.Config;
using Zeus.UI.Controls;
using Zeus.UI.Mvvm;
using Zeus.UI.Mvvm.Interfaces;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data binding in options view.
    /// </summary>
    class OptionsViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The application options model.
        /// </summary>
        private OptionsModel m_Model;
        /// <summary>
        /// The dialog service that allows to interact with dialogs.
        /// </summary>
        private IDialogService m_DialogService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize view model fields.
        /// </summary>
        public OptionsViewModel(OptionsModel options)
        {
            m_Model = options;
            m_DialogService = ServiceLocator.Resolve<IDialogService>();
            SaveAndCloseCommand = new RelayCommand(SaveAndClose);
            RegisterPropagation(m_Model, () => m_Model.RefreshTime, () => RefreshTime);
            RegisterPropagation(m_Model, () => m_Model.AreRowsColored, () => AreRowsColored);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the application refresh time.
        /// </summary>
        public int RefreshTime
        {
            get { return m_Model.RefreshTime; }
            set { m_Model.RefreshTime = value; }
        }
        /// <summary>
        /// Gets or sets the flag that indicates if the rows are colored.
        /// </summary>
        public bool AreRowsColored
        {
            get { return m_Model.AreRowsColored; }
            set { m_Model.AreRowsColored = value; }
        }
        /// <summary>
        /// Gets the command that closes the dialog and serialize the new settings.
        /// </summary>
        public ICommand SaveAndCloseCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes the dialog and save the parameters to file.
        /// </summary>
        private void SaveAndClose()
        {
            //save settings
            ConfigManager.SaveSection<OptionsModel>(m_Model);
            //close the dialog
            m_DialogService.CloseDialog(this);
        }

        #endregion
    }
}
