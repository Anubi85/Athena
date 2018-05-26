using Zeus.UI.Mvvm;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data binding in options view.
    /// </summary>
    public class OptionsViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The application refresh time.
        /// </summary>
        private int m_RefreshTime;
        /// <summary>
        /// A flag that indicates if the rows are colored.
        /// </summary>
        private bool m_AreRowsColored;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize view model fields.
        /// </summary>
        public OptionsViewModel()
        {
            m_RefreshTime = 500;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the application refresh time.
        /// </summary>
        public int RefreshTime
        {
            get { return m_RefreshTime; }
            set { Set(ref m_RefreshTime, value); }
        }
        /// <summary>
        /// Gets or sets the flag that indicates if the rows are colored.
        /// </summary>
        public bool AreRowsColored
        {
            get { return m_AreRowsColored; }
            set { Set(ref m_AreRowsColored, value); }
        }

        #endregion
    }
}
