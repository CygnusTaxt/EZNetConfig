using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class IPConfigurationViewModel : ViewModelBase
    {
        private readonly IIPConfigurationService _ipInformationService;
        private readonly IDialogMessageService _dialogMessageService;

        /// <summary>
        /// The <see cref="IPConfigurationList" /> property's name
        /// </summary>
        public const string IPConfigurationListPropertyName = "IPConfigurationList";

        private ObservableCollection<IPConfiguration> _ipConfigurationList;

        /// <summary>
        /// Gets the IPInformationList property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ObservableCollection<IPConfiguration> IPConfigurationList
        {
            get { return _ipConfigurationList; }
            set
            {
                _ipConfigurationList = value;
                RaisePropertyChanged(IPConfigurationListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IPConfigurationItem" /> property's name
        /// </summary>
        public const string IPConfigurationItemPropertyName = "IPConfigurationItem";

        private IPConfiguration _ipConfigurationItem;

        /// <summary>
        /// Holds the current displayed IP Configuration
        /// </summary>
        public IPConfiguration IPConfigurationItem
        {
            get { return _ipConfigurationItem; }
            set
            {
                _ipConfigurationItem = value;
                RaisePropertyChanged(IPConfigurationItemPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="StatusBarText" /> property's name
        /// </summary>
        public const string StatusBarTextPropertyName = "StatusBarText";

        private string _statusBarText;

        /// <summary>
        /// Text to display on the Status Bar
        /// </summary>
        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                RaisePropertyChanged(StatusBarTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EnableIPTextBoxes" /> property's name
        /// </summary>
        public const string EnableIPTextBoxesPropertyName = "EnableIPTextBoxes";

        private bool _enableIPTextBoxes;

        /// <summary>
        /// Enables or disables TextBoxes when DHCP is toggled
        /// </summary>
        public bool EnableIPTextBoxes
        {
            get { return _enableIPTextBoxes; }
            set
            {
                _enableIPTextBoxes = value;
                RaisePropertyChanged(EnableIPTextBoxesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="AvailableNetworkInterfaces" /> property's name
        /// </summary>
        public const string AvailableNetworkInterfacesPropertyName = "AvailableNetworkInterfaces";

        private ObservableCollection<NetworkInterface> _availableNetworkInterfaces;

        /// <summary>
        /// List with connected network interfaces
        /// </summary>
        public ObservableCollection<NetworkInterface> AvailableNetworkInterfaces
        {
            get { return _availableNetworkInterfaces; }
            set
            {
                _availableNetworkInterfaces = value;
                RaisePropertyChanged(AvailableNetworkInterfacesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedNetworkInterface" /> property's name
        /// </summary>
        public const string SelectedNetworkInterfacePropertyName = "SelectedNetworkInterface";

        private NetworkInterface _selectedNetworkInterface;

        /// <summary>
        /// The selected network interface
        /// </summary>
        public NetworkInterface SelectedNetworkInterface
        {
            get { return _selectedNetworkInterface; }
            set
            {
                _selectedNetworkInterface = value;
                RaisePropertyChanged(SelectedNetworkInterfacePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="EnableControl" /> property's name
        /// </summary>
        public const string EnableControlPropertyName = "EnableControl";

        private bool _enableControl;

        /// <summary>
        /// Enables or disables the binded UI Elements
        /// </summary>
        public bool EnableControl
        {
            get { return _enableControl; }
            set
            {
                _enableControl = value;
                RaisePropertyChanged(EnableControlPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ProgressValue" /> property's name.
        /// </summary>
        public const string ProgressValuePropertyName = "ProgressValue";

        private int _progressValue;

        /// <summary>
        /// Sets and gets the ProgressValue property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }

            set
            {
                if (_progressValue == value)
                {
                    return;
                }

                RaisePropertyChanging(ProgressValuePropertyName);
                _progressValue = value;
                RaisePropertyChanged(ProgressValuePropertyName);
            }
        }

        public RelayCommand GetAllIPInfoCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand<IPConfiguration> ShowIPInformationCommand { get; set; }

        public RelayCommand<NetworkInterface> SelectNetworkInterfaceCommand { get; set; }

        public RelayCommand<bool> DisableIPConfigurationDHCPCommand { get; set; }

        public RelayCommand ApplyIPConfigurationCommand { get; set; }

        public RelayCommand<ProgressReport> SendProgressInformationCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the IPInformationViewModel class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IPConfigurationViewModel(IIPConfigurationService ipInformationService,
            IDialogMessageService dialogMessageService)
        {
            _ipInformationService = ipInformationService;
            _dialogMessageService = dialogMessageService;
            IPConfigurationList = new ObservableCollection<IPConfiguration>();
            IPConfigurationItem = new IPConfiguration();
            AvailableNetworkInterfaces = new ObservableCollection<NetworkInterface>();
            ProgressReport prgReport = new ProgressReport
            {
                ProgressMessage = "Ready",
                ProgressValue = 0
            };
            GetAllIPInfoCommand = new RelayCommand(GetIPInformationList);
            SaveCommand = new RelayCommand(SaveIPInformation);
            ShowIPInformationCommand = new RelayCommand<IPConfiguration>(ShowIPInformation);
            SelectNetworkInterfaceCommand = new RelayCommand<NetworkInterface>(SelectNetworkAdapter);
            ApplyIPConfigurationCommand = new RelayCommand(ApplyIPConfiguration);
            DisableIPConfigurationDHCPCommand = new RelayCommand<bool>(DisableIPConfigurationDHCP);
            EnableIPTextBoxes = true;
            EnableControl = true;
            ReportProgress(prgReport);
        }

        /// <summary>
        /// Disables checkboxes based on the DHCP selection
        /// </summary>
        /// <param name="isDHCP">Check if configuration is DHCP</param>
        public void DisableIPConfigurationDHCP(bool isDHCP)
        {
            EnableIPTextBoxes = !(isDHCP);
        }

        /// <summary>
        /// Applies the selected network configuration
        /// </summary>
        public async void ApplyIPConfiguration()
        {
            EnableControl = false;
            EnableIPTextBoxes = !(true);

            if (SelectedNetworkInterface == null)
            {
                await _dialogMessageService.ShowAsync("Error", "Please select a network interface");
            }
            else if (IPConfigurationItem.IPConfigurationName == null || IPConfigurationItem.IPConfigurationName == string.Empty)
            {
                await _dialogMessageService.ShowAsync("Error", "Please selct an IP configuration to apply");
            }
            else
            {
                try
                {
                    var progressIndicator = new Progress<ProgressReport>(ReportProgress);
                    await _ipInformationService.ApplyIPConfigurationAsync(IPConfigurationItem, SelectedNetworkInterface,
                        progressIndicator);
                    await _dialogMessageService.ShowAsync("IP Settings Applied", "IP settings have been applied succesfully");
                    ReportProgress(new ProgressReport { ProgressMessage = "Ready", ProgressValue = 0 });
                }
                catch (Exception ex)
                {
                    _dialogMessageService.ShowAsync("Error", ex.Message + ex.InnerException);
                    ReportProgress(new ProgressReport { ProgressMessage = "Ready", ProgressValue = 0 });
                }
            }

            EnableControl = true;
            EnableIPTextBoxes = !(false);
        }

        /// <summary>
        /// Gets the entries on the XML file containing the IP Configuration information
        /// </summary>
        public async void GetIPInformationList()
        {
            StatusBarText = "Getting saved IP Configurations";
            IPConfigurationList.Clear();
            var ipListFromFile = await Tools.GetIPInformationListAsync();
            ipListFromFile.IPConfList.ForEach(ip => IPConfigurationList.Add(ip));
            RaisePropertyChanged(IPConfigurationListPropertyName);
            GetConnectedNetworkInterfaces();
            StatusBarText = "Ready";
        }

        /// <summary>
        /// Saves the current IP Configuration
        /// </summary>
        public async void SaveIPInformation()
        {
            EnableControl = false;
            StatusBarText = "Saving IP Configuration";
            IPConfigurationList ipInfoList = new IPConfigurationList();

            if (!string.IsNullOrWhiteSpace(IPConfigurationItem.IPConfigurationName))
            {
                var item = IPConfigurationList.FirstOrDefault(x => x.IPConfigurationName == IPConfigurationItem.IPConfigurationName);

                if (item == null)
                {
                    IPConfigurationList.Add(IPConfigurationItem);
                }

                foreach (var ipInfo in IPConfigurationList)
                {
                    ipInfoList.IPConfList.Add(ipInfo);
                }

                await Tools.SaveIPInformationListAsync(ipInfoList);
                GetIPInformationList();
                IPConfigurationItem = new IPConfiguration();
                StatusBarText = "IP Configuration Saved";
            }
            else
            {
                await _dialogMessageService.ShowAsync("Error", "You must enter a valid IP Configuration name");
            }

            EnableControl = true;
        }

        /// <summary>
        /// Get a list of connected network interfaces
        /// </summary>
        public async void GetConnectedNetworkInterfaces()
        {
            Task<List<NetworkInterface>> t1 = Task<List<NetworkInterface>>.Factory.StartNew(() =>
                {
                    return NetworkInterface.GetAllNetworkInterfaces().ToList(); ;
                });

            StatusBarText = "Getting available Network Interfaces";

            var availableInterfaces = await t1;

            availableInterfaces = availableInterfaces.Where(aInt => (aInt.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                aInt.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit || aInt.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                aInt.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT || aInt.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                aInt.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && aInt.OperationalStatus == OperationalStatus.Up).ToList();

            AvailableNetworkInterfaces.Clear();

            availableInterfaces.ForEach(aInt => AvailableNetworkInterfaces.Add(aInt));

            RaisePropertyChanged(AvailableNetworkInterfacesPropertyName);
        }

        /// <summary>
        /// Shows the IP Information selected
        /// </summary>
        /// <param name="ipConfiguration">IPInformation object to show</param>
        public void ShowIPInformation(IPConfiguration ipConfiguration)
        {
            if (ipConfiguration != null)
            {
                IPConfigurationItem = ipConfiguration;
                DisableIPConfigurationDHCP(ipConfiguration.IsDHCP);
            }
        }

        /// <summary>
        /// Change the selected network adapter
        /// </summary>
        /// <param name="networkAdapter">The selected network adapter</param>
        public void SelectNetworkAdapter(NetworkInterface networkAdapter)
        {
            SelectedNetworkInterface = networkAdapter;
        }

        private void ReportProgress(ProgressReport value)
        {
            Messenger.Default.Send<MessageCommunicator>(new MessageCommunicator
            {
                ProgressReportFromView = value
            });
        }
    }
}