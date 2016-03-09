using InstaSharp.Endpoints;
using InstaSharp.Models;
using InstaSharp.Models.Responses;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstViewModels
{
    public class InstaSearchVM : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<User> UserSearchResponse { get; set; }

        private string keyWords;
        public string KeyWords
        {
            get { return keyWords; }
            set { keyWords = value; RaisePropertyChanged("KeyWords"); }
        }



        public InstaSearchVM()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            UserSearchResponse = new ObservableCollection<User>();
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            switch (param)
            {
                case "SEARCH":
                    if (KeyWords.IsNullOrEmpty()) "Enter Search Keyword".Show();
                    startSearch();
                    break;

                default:
                    break;
            }
        }

        private async void startSearch()
        {
            string[] kws = KeyWords.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries );
            foreach (var k in kws)
            {
                if (k.IsNullOrEmpty()) continue;
                string kw = k.Trim();
                UsersResponse searchRespnse = await new Users(InstaVM.Instance.InstaConfig).Search(kw, 100);
                foreach (var u in searchRespnse.Data)
                {
                    UserSearchResponse.Add(u);
                }
            }
        }
    }
}
