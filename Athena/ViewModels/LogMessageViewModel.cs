using Athena.Models;
using Athena.Wcf;
using System;
using System.Windows;
using System.Windows.Media;
using Zeus.Log;
using Zeus.UI.Mvvm;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for message data bindind in server logged messages view.
    /// </summary>
    class LogMessageViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The model that contains all the message data.
        /// </summary>
        private WcfLogMessage m_Model;
        /// <summary>
        /// The application options.
        /// </summary>
        private OptionsModel m_Options;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message time.
        /// </summary>
        public DateTime Time
        {
            get { return m_Model.Time; }
        }
        /// <summary>
        /// Gets the log message level.
        /// </summary>
        public LogLevels Level
        {
            get { return m_Model.Level; }
        }
        /// <summary>
        /// Gets the name of the process that generate the message.
        /// </summary>
        public string Process
        {
            get { return m_Model.ProcessName; }
        }
        /// <summary>
        /// Gets the name of the method that generate the message.
        /// </summary>
        public string Method
        {
            get { return m_Model.MethodName; }
        }
        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message
        {
            get { return m_Model.Text; }
        }
        /// <summary>
        /// Gets the foreground color of the datagrid row.
        /// </summary>
        public Color? RowForegroundColor
        {
            get
            {
                if (m_Options.AreRowsColored)
                {
                    switch (m_Model.Level)
                    {
                        case LogLevels.Fatal:
                        case LogLevels.Error:
                            return (Color)Application.Current.FindResource("RedColor");
                        case LogLevels.Warning:
                            return (Color)Application.Current.FindResource("YellowColor");
                        case LogLevels.Success:
                            return (Color)Application.Current.FindResource("GreenColor");
                        case LogLevels.Debug:
                        case LogLevels.Trace:
                            return (Color)Application.Current.FindResource("BlueColor");
                        default:
                            return (Color)Application.Current.FindResource("ForegroundColor");
                    }
                }
                else
                {
                    return (Color)Application.Current.FindResource("ForegroundColor");
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the view model data.
        /// </summary>
        /// <param name="msg">The message object associated with the view model.</param>
        /// <param name="options">The application options.</param>
        public LogMessageViewModel(WcfLogMessage msg, OptionsModel options)
        {
            m_Model = msg;
            m_Options = options;
            RegisterPropagation(m_Options, () => m_Options.AreRowsColored, () => RowForegroundColor);
        }

        #endregion
    }
}
