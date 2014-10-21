using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CurrentConfigurationViewModel : ViewModelBase
    {
        private readonly ICurrentIPConfigurationService _currentIPConfigurationService;

        /// <summary>
        /// The <see cref="CurrentIPConfiguration" /> property's name.
        /// </summary>
        public const string CurrentIPConfigurationPropertyName = "CurrentIPConfiguration";

        private IPConfiguration _currentIPConfiguration;

        /// <summary>
        /// Sets and gets the CurrentIPConfiguration property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IPConfiguration CurrentIPConfiguration
        {
            get
            {
                return _currentIPConfiguration;
            }

            set
            {
                if (_currentIPConfiguration == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentIPConfigurationPropertyName);
                _currentIPConfiguration = value;
                RaisePropertyChanged(CurrentIPConfigurationPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentProxyServer" /> property's name.
        /// </summary>
        public const string CurrentProxyServerPropertyName = "CurrentProxyServer";

        private ProxyConfiguration _currentProxyServer;

        /// <summary>
        /// Sets and gets the CurrentProxyServer property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ProxyConfiguration CurrentProxyServer
        {
            get
            {
                return _currentProxyServer;;
            }

            set
            {
                if (_currentProxyServer == value)
                {
                    return;
                }

                _currentProxyServer = value;
                RaisePropertyChanged(CurrentProxyServerPropertyName);
            }
        }

        public RelayCommand GetCurrentIPInfoCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the CurrentIPConfigurationViewModel class.
        /// </summary>
        public CurrentConfigurationViewModel(ICurrentIPConfigurationService currentIPConfigurationService)
        {
            this._currentIPConfigurationService = currentIPConfigurationService;
            this.CurrentIPConfiguration = new IPConfiguration();
            GetCurrentIPInfoCommand = new RelayCommand(GetCurrentIPInfo);
            UpdateCurrentConfigurationInfo();
        }

        public async void GetCurrentIPInfo()
        {
            this.CurrentIPConfiguration = await _currentIPConfigurationService.GetCurrentIPConfigurationAsync();
            this.CurrentProxyServer = await _currentIPConfigurationService.GetCurrentProxyConfiguration();
        }

        void UpdateCurrentConfigurationInfo()
        {
            Messenger.Default.Register<ConfigurationCommunicator>(this, (info) =>
                {
                    if(info.CurrentConfigurationChanged)
                    {
                        GetCurrentIPInfo();
                    }
                });
        }
    }
}