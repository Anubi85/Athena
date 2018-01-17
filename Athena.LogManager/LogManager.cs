using Athena.Commons;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Athena.Logger
{
    /// <summary>
    /// Manages the message logging of the application.
    /// </summary>
    public sealed class LogManager
    {
        #region Singleton

        /// <summary>
        /// The one and only <see cref="LogManager"/> instance.
        /// </summary>
        public static LogManager Logger { get; } = new LogManager();

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate a new instance and allocate and initialize class resources.
        /// </summary>
        private LogManager()
        {
            m_LogChannels = new ConcurrentQueue<ILogChannel>();
            m_MessageQuque = new BlockingCollection<LogMessage>();
            m_ProcessName = Process.GetCurrentProcess().ProcessName;            
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Release object resources.
        /// </summary>
        ~LogManager()
        {
            m_MessageQuque?.Dispose();
        }

        #endregion

        #region Fields

        /// <summary>
        /// The list of channels used by the log manager to log messages.
        /// </summary>
        private ConcurrentQueue<ILogChannel> m_LogChannels;
        /// <summary>
        /// The queue that contains the message to be processed by each configured channel.
        /// </summary>
        private BlockingCollection<LogMessage> m_MessageQuque;
        /// <summary>
        /// The cancellation token used to stop worker thread.
        /// </summary>
        private CancellationTokenSource m_Cancel;
        /// <summary>
        /// The <see cref="Thread"/> that actually process the log messages.
        /// </summary>
        private Thread m_WorkerThread;
        /// <summary>
        /// The name of the process that instantiate the class.
        /// </summary>
        private string m_ProcessName;

        #endregion

        #region Constants

        /// <summary>
        /// The default value of the methodName parameters used in the Write*TYPE*Message methods.
        /// </summary>
        private const string c_DefaultMethodName = "No Data";

        #endregion

        #region Methods

        /// <summary>
        /// Apend a new message to the message queue.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="methodName">The name of the method that generate the message.</param>
        /// <param name="text">The message text.</param>        
        private void WriteMessage(LogMessageType type, string methodName, string text)
        {
            m_MessageQuque.Add(new LogMessage(type, methodName, m_ProcessName, text));
        }

        /// <summary>
        /// Append a message to the message queue.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="methodName">The name of the method that generate the message (automatically retrieved).</param>
        public void WriteTraceMessage(string text, [CallerMemberName] string methodName = c_DefaultMethodName)
        {
            WriteMessage(LogMessageType.Trace, methodName, text);
        }

        /// <summary>
        /// Append an info message to the message queue.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="methodName">The name of the method that generate the message (automatically retrieved).</param>
        public void WriteInfoMessage(string text, [CallerMemberName] string methodName = c_DefaultMethodName)
        {
            WriteMessage(LogMessageType.Info, methodName, text);
        }

        /// <summary>
        /// Append a success message to the message queue.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="methodName">The name of the method that generate the message (automatically retrieved).</param>
        public void WriteSuccessMessage(string text, [CallerMemberName] string methodName = c_DefaultMethodName)
        {
            WriteMessage(LogMessageType.Success, methodName, text);
        }

        /// <summary>
        /// Append a warning message to the message queue.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="methodName">The name of the method that generate the message (automatically retrieved).</param>
        public void WriteWarningMessage(string text, [CallerMemberName] string methodName = c_DefaultMethodName)
        {
            WriteMessage(LogMessageType.Warning, methodName, text);
        }

        /// <summary>
        /// Append an error message to the message queue.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="methodName">The name of the method that generate the message (automatically retrieved).</param>
        public void WriteErrorMessage(string text, [CallerMemberName] string methodName = c_DefaultMethodName)
        {
            WriteMessage(LogMessageType.Error, methodName, text);
        }

        /// <summary>
        /// This method read a message from the message queue and pass it to all the configured channels in order to be processed.
        /// </summary>
        private void DoWork()
        {
            LogMessage msg;
            while (true)
            {
                try
                {
                    msg = m_MessageQuque.Take(m_Cancel.Token);
                    if (msg == null)
                    {
                        return;
                    }
                    foreach(ILogChannel ch in m_LogChannels)
                    {
                        ch.WriteMessage(msg);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Enable the message processing of the logger.
        /// </summary>
        public void Start()
        {
            if (m_WorkerThread == null)
            {
                //create a new cancellation token
                m_Cancel = new CancellationTokenSource();
                //create a new task that will process the new messages
                m_WorkerThread = new Thread(DoWork);
                m_WorkerThread.Start();
            }
        }

        /// <summary>
        /// Disable the message processing.
        /// </summary>
        public void Stop()
        {
            if (!CompleteMessageProcessing)
            {
                m_Cancel.Cancel();
            }
            else
            {
                m_MessageQuque.Add(null);                
            }
            m_WorkerThread.Join();
            m_WorkerThread = null;
            m_Cancel.Dispose();
        }

        public bool AddChannel<T>() where T: ILogChannel, new()
        {
            ILogChannel channel = new T();
            bool res = channel.Initialize();
            if (res)
            {
                m_LogChannels.Enqueue(channel);
            }
            return res;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set a flag that tell to the object if it has to complete the message processing or discard unprocessed messaged when it will be destroyed.
        /// </summary>
        public bool CompleteMessageProcessing { get; set; } = true;

        #endregion
    }
}
