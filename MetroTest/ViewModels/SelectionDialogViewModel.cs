using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CustomDialogTest.Helpers;
using CustomDialogTest.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace CustomDialogTest.ViewModels
{
    public class SelectionDialogViewModel : DialogViewModel<OpCode>
    {
        private OpCode _selectedOpCode;
        private ObservableCollection<OpCode> _downCodes = new ObservableCollection<OpCode>();

        public OpCode SelectedOpCode
        {
            get { return _selectedOpCode; }
            set { Set(() => SelectedOpCode, ref _selectedOpCode, value); }
        }

        public ObservableCollection<OpCode> DownCodes
        {
            get { return _downCodes; }
            set { Set(() => DownCodes, ref _downCodes, value); }
        }

        public SelectionDialogViewModel()
        {
            //Hard-coded for simplicity. Would use PressOperationCode WCF in production
            DownCodes = new ObservableCollection<OpCode>()
            {
                new OpCode() { Description = "O.I.M. Program", GroupCode = "100" , OperationCode = "629" },
                new OpCode() { Description = "Press Down, No Crew", GroupCode = "101" , OperationCode = "124" },
                new OpCode() { Description = "Crewed, No Work", GroupCode = "102" , OperationCode = "089" }
            };

            SelectedOpCode = DownCodes.First();
        }

        public ICommand OkCommand { get { return new RelayCommand(OnOk, CanOk); } }
        public ICommand CancelCommand { get { return new RelayCommand(OnCancel);} }

        private bool CanOk()
        {
            return SelectedOpCode != null;
        }

        private void OnOk()
        {
            Close(SelectedOpCode);
        }

        private void OnCancel()
        {
            Close(null);
        }
    }
}
