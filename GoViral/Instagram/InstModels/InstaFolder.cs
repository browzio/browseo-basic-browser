using GoViral.Instagram.InstViewModels;
using InstaSharp.Models.Responses;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstModels
{
    public class InstalLink
    {
        [Newtonsoft.Json.JsonIgnore]
        public ICommand OnCommandFromView { get; set; }
        public event Action<InstalLink, string> OnRaisedCommandToViewModel = delegate { };

        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public InstalLink()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            OnRaisedCommandToViewModel(this, param);
        }
    }
    public class InstaFolder : ViewModelBase
    {
        public event Action<string> OnSendContentToSorter = delegate { };

        [Newtonsoft.Json.JsonIgnore]
        public ICommand OnCommandFromView { get; set; }

        public string FolderTitle { get; set; }

        public ObservableCollection<InstalLink> Links { get; set; }


        private InstaResponseLists instaResponseLists;
        public InstaResponseLists InstaResponseLists
        {
            get { return instaResponseLists; }
            set { instaResponseLists = value; RaisePropertyChanged("InstaResponseLists"); }
        }

        public InstaFolder()
        {
            Links = new ObservableCollection<InstalLink>();

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            using (new WaitCursor())
            {
                try
                {

                    string param = obj as string;
                    switch (param)
                    {
                        case "CLEARUSERS":
                            if (InstaResponseLists != null)
                            {
                                InstaResponseLists.UserSearchResponse.Clear();
                                InstaResponseLists.RasieAllCountsChanged();
                            }
                            break;

                        case "CLEARTAGS":
                            if (InstaResponseLists != null)
                            {
                                InstaResponseLists.TagsSearchResponse.Clear();
                                InstaResponseLists.RasieAllCountsChanged();
                            }
                            break;

                        case "CLEARMedia":
                            if (InstaResponseLists != null)
                            {
                                InstaResponseLists.MediaSearchResponse.Clear();
                                InstaResponseLists.RasieAllCountsChanged();
                            }
                            break;

                        case "RefreshUSERS":
                            if (InstaVM.Instance.InstaResponse == null)
                            {
                                "You need to log in first.".Show();
                                return;
                            }
                            if (InstaResponseLists != null)
                            {
                                List<Task> searches = new List<Task>();
                                foreach (var u in InstaResponseLists.UserSearchResponse)
                                {
                                    searches.Add(SearchForUsers(u.Username));
                                }
                                await Task.WhenAll(searches);
                                RaisePropertyChanged("InstaResponseLists");
                            }
                            break;

                        case "RefreshTags":
                            if (InstaVM.Instance.InstaResponse == null)
                            {
                                "You need to log in first.".Show();
                                return;
                            }
                            if (InstaResponseLists != null)
                            {
                                List<Task> searches = new List<Task>();
                                foreach (var t in InstaResponseLists.TagsSearchResponse)
                                {
                                    searches.Add(SearchForTags(t.Name));
                                }
                                await Task.WhenAll(searches);
                                RaisePropertyChanged("InstaResponseLists");
                            }
                            break;

                        case "RefreshMedia":
                            if (InstaVM.Instance.InstaResponse == null)
                            {
                                "You need to log in first.".Show();
                                return;
                            }
                            if (InstaResponseLists != null)
                            {
                                List<Task> searches = new List<Task>();
                                foreach (var m in InstaResponseLists.MediaSearchResponse)
                                {
                                    searches.Add(SearchForMedia(m.Id));
                                    //foreach (var t in m.Tags)
                                    //{
                                    //    searches.Add(SearchForMedia(t));
                                    //}
                                }
                                await Task.WhenAll(searches);
                                RaisePropertyChanged("InstaResponseLists");
                            }
                            break;

                        #region ct menu for InstaResponseLists
                        case "LOADAllRelationUsers":
                        case "LOADAllMediaUsers":
                        case "LOADAllCountsUsers":
                        case "LOADAllCommentsUsers":
                        case "ORDERFollowersUsers":
                        case "ORDERFollowingUsers":
                        case "ORDERMediaUsers":
                        case "LIKEAllMediaUsers":
                        case "LIKEAllMediaUnUsers":
                        case "ORDERCommentsMediaUsers":
                        case "ORDERLikesMediaUsers":
                        case "FOLLOWAllUsers":
                        case "FOLLOWAcceptAllUsers":
                        case "FOLLOWBlockAllUsers":
                        case "FOLLOWUnBlockAllUsers":
                        case "LOADAllMediaTags":
                        case "LOADAllCommentsMediaTags":
                        case "ORDERMediaTags":
                        case "ORDERCommentsMediaTags":
                        case "ORDERLikesMediaTags":
                        case "LIKEAllMediaTags":
                        case "LIKEAllMediaTagsUn":
                        case "LOADAllCommentsMedia":
                        case "ORDERCommentsMedia":
                        case "ORDERLikesMedia":
                        case "LIKEAllMedia":
                        case "LIKEAllMediaUn":
                        case "SENDTOSORTERAllUsers":
                        case "SENDTOSORTERSelectedUsers":
                        case "SENDTOSORTERAllTags":
                        case "SENDTOSORTERSelectedTags":
                        case "SENDTOSORTERAllMedia":
                        case "SENDTOSORTERSelectedMedia":
                        case "SellectAllUsers":
                        case "SellectNonUsers":
                        case "SellectAllTags":
                        case "SellectNonTags":
                        case "SellectAllMedia":
                        case "SellectNonMedia":
                            Mouse.OverrideCursor = null;
                            if (InstaResponseLists != null) InstaResponseLists.OnCommandFromView_Raised(obj);
                            break;
                        #endregion

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        ex.InnerException.Message.Show();
                    else
                        ex.Message.Show();
                }
            }
        }

        private async Task SearchForMedia(string kw)
        {
            MediasResponse response = await InstaVM.Instance.GetEndpointTags().Recent(kw);
            InstaVM.Instance.CheckShouldnotContinueRequest(InstaResponseLists.MediaSearchResponse, response.Data, response);

            foreach (var m in response.Data)
            {
                InstaMedia im = new InstaMedia(m);
                InstaResponseLists.AddMedia(im);
            }

            InstaResponseLists.RasieAllCountsChanged();
        }

        private async Task SearchForTags(string kw)
        {
            TagsResponse response = await InstaVM.Instance.GetEndpointTags().Search(kw);
            InstaVM.Instance.CheckShouldnotContinueRequest(InstaResponseLists.TagsSearchResponse, response.Data, response);

            foreach (var t in response.Data)
            {
                InstaTag ta = new InstaTag(t);
                InstaResponseLists.AddTag(ta);
            }

            InstaResponseLists.RasieAllCountsChanged();
        }

        private async Task SearchForUsers(string kw)
        {
            UsersResponse response = await InstaVM.Instance.GetEndpointUsers().Search(kw, null);
            InstaVM.Instance.CheckShouldnotContinueRequest(InstaResponseLists.UserSearchResponse, response.Data, response);

            foreach (var u in response.Data)
            {
                var ur = new InstaUser(u);
                InstaResponseLists.AddUser(ur);
            }
            InstaResponseLists.RasieAllCountsChanged();
        }

        internal void SetResponsesEvents()
        {
           if(InstaResponseLists!= null)
            {
                InstaResponseLists.OnSendContentToSorter -= InstaResponseLists_OnSendContentToSorter;
                InstaResponseLists.OnSendContentToSorter += InstaResponseLists_OnSendContentToSorter;
            }
        }

        private void InstaResponseLists_OnSendContentToSorter(string content)
        {
            OnSendContentToSorter(content);
        }
    }
}
