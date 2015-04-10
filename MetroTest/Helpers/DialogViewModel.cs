using System;
using System.Threading.Tasks;
using CustomDialogTest.Helpers.Interfaces;
using GalaSoft.MvvmLight;

namespace CustomDialogTest.Helpers
{
    public abstract class DialogViewModel : ViewModelBase, IDialogViewModel
    {
        private readonly TaskCompletionSource<int> _tcs;

        protected DialogViewModel()
        {
            _tcs = new TaskCompletionSource<int>();
        }

        /// <summary>
        /// Completes the <see cref="DialogViewModel.Task"/> and raises the <see cref="Closed"/> event.
        /// </summary>
        protected void Close()
        {
            _tcs.SetResult(0);

            var handler = Closed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// A task promising the closing the dialog view model. It is completed when <see cref="Close()"/> was called.
        /// </summary>
        public Task Task
        {
            get { return _tcs.Task; }
        }

        /// <summary>
        /// This event is raised when the dialog was closed.
        /// </summary>
        public event EventHandler Closed;
    }
    public abstract class DialogViewModel<TResult> : ViewModelBase, IDialogViewModel
    {
        private readonly TaskCompletionSource<TResult> _tcs;

        protected DialogViewModel()
        {
            _tcs = new TaskCompletionSource<TResult>();
        }

        /// <summary>
        /// Completes the <see cref="DialogViewModel.Task"/> with the given result and raises the <see cref="Closed"/> event.
        /// </summary>
        protected void Close(TResult result)
        {
            _tcs.SetResult(result);

            var handler = Closed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// A task promising the result of the dialog view model. It is completed when <see cref="Close"/> was called.
        /// </summary>
        public Task<TResult> Task
        {
            get { return _tcs.Task; }
        }

        /// <summary>
        /// This event is raised when the dialog was closed.
        /// </summary>
        public event EventHandler Closed;
    }
}
