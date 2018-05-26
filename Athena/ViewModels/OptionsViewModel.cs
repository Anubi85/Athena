using Athena.Models;
using Zeus.UI.Mvvm;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize view model fields.
        /// </summary>
        public OptionsViewModel(OptionsModel options)
        {
            m_Model = options;
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

        #endregion
    }
}
