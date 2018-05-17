using System.ComponentModel;

namespace Athena.LogServer
{
    [RunInstaller(true)]
    public partial class LogServerInstaller : System.Configuration.Install.Installer
    {
        public LogServerInstaller()
        {
            InitializeComponent();
            serviceInstaller.ServiceName = Program.ServiceName;
        }
    }
}
