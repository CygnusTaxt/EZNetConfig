/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Effisoft.IPSwitcher.UI.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Services;
using Effisoft.EZNetConfig.Common.ViewModel;
using Effisoft.IPSwitcher.UI.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Effisoft.EZNetConfig.UI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<IPConfigurationViewModel>();
            SimpleIoc.Default.Register<ProxyServerConfigurationViewModel>();
            SimpleIoc.Default.Register<IIPConfigurationService, IPConfigurationService>();
            SimpleIoc.Default.Register<IDialogMessageService, DialogMessageService>();
            SimpleIoc.Default.Register<IProxyConfigurationService, ProxyConfigurationService>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public IPConfigurationViewModel IPConfiguration
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPConfigurationViewModel>();
            }
        }

        public ProxyServerConfigurationViewModel ProxyServerConfiguration
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProxyServerConfigurationViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}