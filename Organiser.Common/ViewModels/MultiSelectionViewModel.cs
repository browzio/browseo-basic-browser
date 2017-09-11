using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.ViewModels
{
    public class MultiSelectionData : PropertyChangedBase
    {
        public string Name { get; set; }
        
        private bool isCheckes;
        public bool IsChecked
        {
            get { return isCheckes; }
            set { isCheckes = value; NotifyOfPropertyChange(); }
        }
    }

    public class MultiSelectionViewModel : PropertyChangedViewModelBase
    {
        public ObservableCollection<MultiSelectionData> SelectionList { get; set; }

        public MultiSelectionViewModel()
        {
            SelectionList = new ObservableCollection<MultiSelectionData>();
        }

        public override void OnReceivedCommandFromView(string param)
        {
            switch (param)
            {
                case "All":
                    foreach (var o in SelectionList)
                    {
                        o.IsChecked = true;
                    }
                break;

                case "None":
                    foreach (var o in SelectionList)
                    {
                        o.IsChecked = false;
                    }
                    break;

                default:
                    break;
            }
        }

        public bool ShowWindow(string title)
        {
            MultiSelectionView msv = new MultiSelectionView();
            msv.Title = title;
            msv.DataContext = this;
            msv.ShowDialog();

            return msv.DialogResult == true;
        }

        public void Add(string name)
        {
            SelectionList.Add(new MultiSelectionData() { Name = name });
        }

        public List<string> GetCheckedNameList()
        {
            var names = new List<string>();

            foreach (var o in SelectionList)
            {
                if (!o.IsChecked) continue;

                names.Add(o.Name);
            }

            return names;
        }

        public string GetCheckedNameString()
        {
            var names = "";
            foreach (var o in SelectionList)
            {
                if (!o.IsChecked) continue;

                names += o.Name + " ";
            }

            return names;
        }
    }
}
