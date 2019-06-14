using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.ViewModels
{
    public class BirdsEyeStreamTabViewModel
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<StreamTabViewModel> StreamTabs { get; set; }

        public FacebookApisController FacebookApisController { get; set; }

        public BirdsEyeStreamTabViewModel(FacebookApisController facebookApisController)
        {
            FacebookApisController = facebookApisController;

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            StreamTabs = new ObservableCollection<StreamTabViewModel>();

            LoadSavedTabs();
        }

        private void LoadSavedTabs()
        {
        }

        private void OnCommandFromView_Raised(object obj)
        {
            switch (obj as string)
            {
                case "CreateNewTab":
                    CreateNewTab();
                    break;

                default:
                    break;
            }
        }

        private void CreateNewTab()
        {
            SetNameAndDataWindow stnw = new SetNameAndDataWindow();
            stnw.Title = "Create New Tab";
            stnw.tblockInfo.Text = "Name for tab.";
            stnw.ShowDialog();
            if (stnw.OkClicked)
            {
                foreach (var tab in StreamTabs)
                {
                    if (tab.TabTitle == stnw.tbInputText.Text)
                    {
                        MessageBox.Show("There already exists a tab with this name.");
                        return;
                    }
                }


                StreamTabs.Add(new StreamTabViewModel(FacebookApisController)
                {
                    TabTitle = stnw.tbInputText.Text
                });
            }
        }
    }
}
