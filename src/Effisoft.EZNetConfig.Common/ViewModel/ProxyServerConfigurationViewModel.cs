using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Effisoft.EZNetConfig.Common.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ProxyServerConfigurationViewModel : ViewModelBase
    {
        private readonly IProxyConfigurationService _proxyConfService;
        private readonly IDialogMessageService _dialogMessageService;

        #region Properties

        /// <summary>
        /// The <see cref="ProxyConfigurationList" /> property's name.
        /// </summary>
        public const string ProxyConfigurationListPropertyName = "ProxyConfigurationList";

        private ObservableCollection<ProxyConfiguration> _proxyConfigurationList;

        /// <summary>
        /// Sets and gets the ProxyConfigurationList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ProxyConfiguration> ProxyConfigurationList
        {
            get
            {
                return _proxyConfigurationList;
            }

            set
            {
                if (_proxyConfigurationList == value)
                {
                    return;
                }

                RaisePropertyChanging(ProxyConfigurationListPropertyName);
                _proxyConfigurationList = value;
                RaisePropertyChanged(ProxyConfigurationListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ProxyConfigurationItem" /> property's name.
        /// </summary>
        public const string ProxyConfigurationItemPropertyName = "ProxyConfigurationItem";

        private ProxyConfiguration _proxyConfiguration;

        /// <summary>
        /// Sets and gets the ProxyConfigurationItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ProxyConfiguration ProxyConfigurationItem
        {
            get
            {
                return _proxyConfiguration;
            }

            set
            {
                if (_proxyConfiguration == value)
                {
                    return;
                }

                RaisePropertyChanging(ProxyConfigurationItemPropertyName);
                _proxyConfiguration = value;
                RaisePropertyChanged(ProxyConfigurationItemPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EnableControl" /> property's name.
        /// </summary>
        public const string EnableControlPropertyName = "EnableControl";

        private bool _enableControl;

        /// <summary>
        /// Sets and gets the EnableControl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool EnableControl
        {
            get
            {
                return _enableControl;
            }

            set
            {
                _enableControl = value;
                RaisePropertyChanged(EnableControlPropertyName);
            }
        }

        #endregion

        #region RelayCommands

        public RelayCommand GetAllProxyConfCommand { get; set; }
        public RelayCommand<ProxyConfiguration> ShowProxyConfigurationCommandd { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ApplyProxyConfCommand { get; set; }
        public RelayCommand ClearProxyConfCommand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ProxyServerConfigurationViewModel class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProxyServerConfigurationViewModel(IProxyConfigurationService proxyConfService,
            IDialogMessageService dialogMessageService)
        {
            _proxyConfService = proxyConfService;
            _dialogMessageService = dialogMessageService;

            ProxyConfigurationList = new ObservableCollection<ProxyConfiguration>();
            ProxyConfigurationItem = new ProxyConfiguration();
            EnableControl = true;

            GetAllProxyConfCommand = new RelayCommand(GetProxyConfigurationList);
            ShowProxyConfigurationCommandd = new RelayCommand<ProxyConfiguration>(ShowProxyConfiguration);
            SaveCommand = new RelayCommand(SaveProxyConfiguration);
            ApplyProxyConfCommand = new RelayCommand(ApplyProxyConf);
            ClearProxyConfCommand = new RelayCommand(ClearProxy);
        } 

        #endregion

        #region [Public Methods]
        
        private async void ClearProxy()
        {
            EnableControl = false;
            var progressIndicator = new Progress<ProgressReport>(ReportProgress);
            await _proxyConfService.ClearProxyConfAsync(progressIndicator);
            await _dialogMessageService.ShowAsync("Proxy Configuration", "Proxy Configuration Removed");
            ReportProgress(new ProgressReport
            {
                ProgressMessage = "Ready",
                ProgressValue = 0
            });
            UpdatedConfiguration(true);
            EnableControl = true;
        }

        private void ShowProxyConfiguration(ProxyConfiguration proxyConf)
        {
            if (proxyConf != null)
            {
                ProxyConfigurationItem = proxyConf;
            }
        }

        private async void GetProxyConfigurationList()
        {
            ProxyConfigurationList.Clear();
            var proxyConfList = await Tools.GetProxyConfigurationListAsync();
            proxyConfList.ProxyConfList.ForEach(x => ProxyConfigurationList.Add(x));
            RaisePropertyChanged(ProxyConfigurationListPropertyName);
        } 

        #endregion

        #region [Private Methods]

        private void ReportProgress(ProgressReport value)
        {
            Messenger.Default.Send<MessageCommunicator>(new MessageCommunicator
                {
                    ProgressReportFromView = value
                });
        }

        private void UpdatedConfiguration(bool isConfigChanged)
        {
            Messenger.Default.Send<ConfigurationCommunicator>(new ConfigurationCommunicator
            {
                CurrentConfigurationChanged = isConfigChanged
            });
        }

        private async void ApplyProxyConf()
        {
            EnableControl = false;
            var progressIndicator = new Progress<ProgressReport>(ReportProgress);
            await _proxyConfService.ApplyProxyConfAsync(ProxyConfigurationItem, progressIndicator);
            await _dialogMessageService.ShowAsync("Proxy Configuration", "Proxy Configuration Applied");
            ReportProgress(new ProgressReport
                {
                    ProgressMessage = "Ready",
                    ProgressValue = 0
                });
            UpdatedConfiguration(true);
            EnableControl = true;
        }

        private async void SaveProxyConfiguration()
        {
            ProxyConfigurationList proxyConfList = new ProxyConfigurationList();

            if (!string.IsNullOrWhiteSpace(ProxyConfigurationItem.ProxyConfigurationName))
            {
                var item = ProxyConfigurationList.FirstOrDefault(x => x.ProxyConfigurationName == ProxyConfigurationItem.ProxyConfigurationName);

                if (item == null)
                {
                    ProxyConfigurationList.Add(ProxyConfigurationItem);
                }

                foreach (var ipInfo in ProxyConfigurationList)
                {
                    proxyConfList.ProxyConfList.Add(ipInfo);
                }

                await Tools.SaveProxyConfigurationListAsync(proxyConfList);
                await _dialogMessageService.ShowAsync("Proxy Configuration", "Proxy Configuration Saved");
                GetProxyConfigurationList();
                ProxyConfigurationItem = new ProxyConfiguration();
            }
            else
            {
                await _dialogMessageService.ShowAsync("Error", "You must enter a valid IP Configuration name");
            }
        }

        #endregion
    }
}