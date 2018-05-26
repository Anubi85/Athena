using Zeus.UI.Mvvm;

namespace Athena.Models
{
    /// <summary>
    /// The application option model.
    /// </summary>
    public class OptionsModel : ObservableObject
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
        /// Initialize model fields.
        /// </summary>
        public OptionsModel()
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
