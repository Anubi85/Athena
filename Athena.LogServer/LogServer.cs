using Athena.Wcf;
using System.ServiceModel;
using System.ServiceProcess;

namespace Athena.LogServer
{
    /// <summary>
    /// Implements the windows service that hosts the Athena Log Server WCF service.
    /// </summary>
    public partial class LogServer : ServiceBase
    {
        #region Fields

        /// <summary>
        /// The Athena WCF service log server host.
        /// </summary>
        private ServiceHost m_WcfHost;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public LogServer()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the service resources and starts the Athena WCF log server host.
        /// </summary>
        /// <param name="args">The arguments passed to the service.</param>
        protected override void OnStart(string[] args)
        {
            m_WcfHost = new ServiceHost(typeof(AthenaWcfServer));
            m_WcfHost.AddServiceEndpoint(Utility.GetEndpoint());
            m_WcfHost.Open();
        }
        /// <summary>
        /// Clean up the resources used by the Athena WCF log server service.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                m_WcfHost?.Close();
            }
            catch
            {
                m_WcfHost?.Abort();
            }
        }

        #endregion
    }
}
