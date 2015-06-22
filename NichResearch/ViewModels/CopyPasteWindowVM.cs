using NichResearch.Helpers;
using NichResearch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NichResearch.ViewModels
{
    public class CopyPasteWindowVM : INotifyPropertyChanged
    {
        private ICommand sendToWindow;
        public ICommand SendToWindow
        {
            get { return sendToWindow; }
            set { sendToWindow = value; }
        }

        #region lists
        private ObservableCollection<CopyPasteItem> copyPasteItemList;
        public ObservableCollection<CopyPasteItem> CopyPasteItemList
        {
            get { return copyPasteItemList; }
            set { copyPasteItemList = value; }
        }

        private int sICopyPasteItemList;
        public int SICopyPasteItemList
        {
            get { return sICopyPasteItemList; }
            set { sICopyPasteItemList = value; }
        }

        private string commandType;
        public string CommandType
        {
            get { return commandType; }
            set
            {
                commandType = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CommandType"));
            }
        }

        #endregion

        public CopyPasteWindowVM()
        {
            CopyPasteItemList = new ObservableCollection<CopyPasteItem>();

            SendToWindow = new RelayCommand(Copy);
        }

        private void Copy(object obj)
        {
            Clipboard.SetText("Hello, clipboard");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
