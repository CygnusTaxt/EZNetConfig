using Effisoft.EZNetConfig.Common.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Effisoft.IPSwitcher.UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="ProgressReportInfo" /> property's name.
        /// </summary>
        public const string ProgressReportInfoPropertyName = "ProgressReportInfo";

        private ProgressReport _progressReportInfo;

        /// <summary>
        /// Sets and gets the ProgressReportInfo property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ProgressReport ProgressReportInfo
        {
            get
            {
                return _progressReportInfo;
            }

            set
            {
                RaisePropertyChanging(ProgressReportInfoPropertyName);
                _progressReportInfo = value;
                RaisePropertyChanged(ProgressReportInfoPropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ProgressReportInfo = new ProgressReport();
            ReceiveProgressInfo();
        }

        void ReceiveProgressInfo()
        {
            if (ProgressReportInfo != null)
            {
                Messenger.Default.Register<MessageCommunicator>(this, (prg) =>
                {
                    this.ProgressReportInfo = prg.ProgressReportFromView;
                });
            }
        }
    }
}