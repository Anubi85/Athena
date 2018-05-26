using Athena.ViewModels;
using Athena.Views;
using System.Windows;
using Zeus.UI.Mvvm;
using Zeus.UI.Mvvm.Interfaces;

namespace Athena
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            IDialogService dialogService = new DialogService();
            dialogService.Register<ConnectionView, ConnectionViewModel>();
            dialogService.Register<OptionsView, OptionsViewModel>();
            ServiceLocator.Register<IDialogService>(dialogService);
        }
    }
}
