using System.Threading.Tasks;
using System.Windows.Input;
using CustomDialogTest.Helpers;
using CustomDialogTest.Helpers.Interfaces;
using CustomDialogTest.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CustomDialogTest.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogManager _dialogManager ;
        public ICommand OpenDialogCommand { get { return new RelayCommand(async () => await OpenDialog());} }

        public MainViewModel()
        {
            _dialogManager = new DialogManager();
        }

        private async Task OpenDialog()
        {
            var opCode = await _dialogManager.ShowDialogAsync<SelectionDialogViewModel, OpCode>();
        }
    }
}