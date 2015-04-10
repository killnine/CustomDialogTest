using System;

namespace CustomDialogTest.Helpers.Interfaces
{
    public interface IDialogViewModel
    {
        /// <summary>
        /// This event is raised when the dialog was closed.
        /// </summary>
        event EventHandler Closed;
    }
}