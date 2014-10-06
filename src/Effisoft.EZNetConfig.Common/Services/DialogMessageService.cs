using Effisoft.EZNetConfig.Common.Interfaces;
using System.Threading.Tasks;
using System.Windows;

namespace Effisoft.EZNetConfig.Common.Services
{
    /// <summary>
    /// Implementation of the IDialogMessageService
    /// </summary>
    public class DialogMessageService : IDialogMessageService
    {
        /// <summary>
        /// Show Dialog Message asynchronously
        /// </summary>
        /// <param name="title">Message Dialog Title</param>
        /// <param name="message">Message Dialog Message</param>
        /// <returns></returns>
        public async Task ShowAsync(string title, string message)
        {
            await ShowAsync(title, message, null);
        }

        /// <summary>
        /// Show Dialog Message asynchronously
        /// </summary>
        /// <param name="title">Message Dialog Title</param>
        /// <param name="message">Message Dialog Message</param>
        /// <param name="commands">List of commands for the dialog message</param>
        /// <returns></returns>
        public async Task ShowAsync(string title, string message, System.Collections.IEnumerable commands)
        {
            await Task.Factory.StartNew(() =>
                {
                    MessageBox.Show(message, title);
                });
        }
    }
}