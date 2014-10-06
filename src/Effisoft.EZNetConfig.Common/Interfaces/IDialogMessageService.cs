using System.Collections;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Interfaces
{
    /// <summary>
    /// Interface for the Dialog Message Service for ViewModels
    /// </summary>
    public interface IDialogMessageService
    {
        /// <summary>
        /// Show Dialog Message asynchronously
        /// </summary>
        /// <param name="title">Message Dialog Title</param>
        /// <param name="message">Message Dialog Message</param>
        /// <returns></returns>
        Task ShowAsync(string title, string message);

        /// <summary>
        /// Show Dialog Message asynchronously
        /// </summary>
        /// <param name="title">Message Dialog Title</param>
        /// <param name="message">Message Dialog Message</param>
        /// <param name="commands">List of commands for the dialog message</param>
        /// <returns></returns>
        Task ShowAsync(string title, string message, IEnumerable commands);
    }
}