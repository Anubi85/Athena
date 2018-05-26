using Athena.Models;
using Athena.Wcf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Zeus.UI.Mvvm;

namespace Athena.ViewModels
{
    /// <summary>
    /// The view model used for data bingind in server logged messages view.
    /// </summary>
    class LogServerViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The model associated with this view model instance.
        /// </summary>
        private LogServerModel m_Model;
        /// <summary>
        /// Syncronization object used to syncronize collection access.
        /// </summary>
        private object m_SyncObject;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize view model fields.
        /// </summary>
        /// <param name="model">The <see cref="LogServerModel"/> object that allows to interact with the Athena server.</param>
        public LogServerViewModel(LogServerModel model)
        {
            m_Model = model;
            m_Model.NewMessagesAvaialble += ProcessNewMessages;
            RegisterPropagation(m_Model, () => m_Model.IsConnected, () => IsConnected);
            m_SyncObject = new object();
            LogMessages = new ObservableCollection<WcfLogMessage>();
            BindingOperations.EnableCollectionSynchronization(LogMessages, m_SyncObject);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process the new messages received from the server.
        /// </summary>
        /// <param name="messages">The list of the new messages received.</param>
        private void ProcessNewMessages(List<WcfLogMessage> messages)
        {
            foreach(WcfLogMessage msg in messages.OrderBy(m => m.Time))
            {
                lock (m_SyncObject)
                {
                    LogMessages.Add(msg);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Athena log server name.
        /// </summary>
        public string ServerName
        {
            get { return m_Model.Address.ToString(); }
        }
        /// <summary>
        /// Gets a flag that indicates if the server is connected.
        /// </summary>
        public bool IsConnected
        {
            get { return m_Model.IsConnected; }
        }
        /// <summary>
        /// Gets the collection of the avaialble log messages.
        /// </summary>
        public ObservableCollection<WcfLogMessage> LogMessages { get; private set; }

        #endregion
    }
}
