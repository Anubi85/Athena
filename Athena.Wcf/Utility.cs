using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Athena.Wcf
{
    /// <summary>
    /// Provides some common functionalities.
    /// </summary>
    internal static class Utility
    {
        #region Methods

        /// <summary>
        /// Gets the WCF service andpoint informations.
        /// </summary>
        /// <param name="address">The endpoint address of the WCF service server.</param>
        /// <returns>The service endpoint configuration.</returns>
        public static ServiceEndpoint GetEndpoint(IPAddress address = null)
        {
            return new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IAthenaWcfService)),
                new BasicHttpBinding(),
                new EndpointAddress(string.Format("http://{0}:8521/Athena", address ?? IPAddress.Loopback)));
        }

        #endregion
    }
}
