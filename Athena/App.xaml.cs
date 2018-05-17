using Athena.ViewModels;
using Athena.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            ServiceLocator.Register<IDialogService>(dialogService);
        }
    }
}
