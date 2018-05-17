using Zeus.UI.Mvvm;

namespace Athena.Models
{
    /// <summary>
    /// The application options model.
    /// </summary>
    public class OptionsModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// A flags that indicates if the log message update is paused or not.
        /// </summary>
        private bool m_IsPaused;
        /// <summary>
        /// A flag that indicates if the message list shall sroll automatically.
        /// </summary>
        private bool m_AutoScroll;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the flag that pause/resume the log message update.
        /// </summary>
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { Set(ref m_IsPaused, value); }
        }
        /// <summary>
        /// Gets or sets the flag that enable or disable the log message list auto scroll.
        /// </summary>
        public bool AutoScroll
        {
            get { return m_AutoScroll; }
            set { Set(ref m_AutoScroll, value); }
        }

        #endregion
    }
}
