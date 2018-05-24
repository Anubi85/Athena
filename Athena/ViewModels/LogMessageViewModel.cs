using System;
using Zeus.UI.Mvvm;

namespace Athena.ViewModels
{
    /// <summary>
    /// Represents a log message downloaded from the Athena log server.
    /// </summary>
    public class LogMessageViewModel : ViewModelBase
    {
        #region Properties

        public DateTime Date { get { return DateTime.Now; } }
        public string Message { get { return "Message"; } }

        #endregion
    }
}
