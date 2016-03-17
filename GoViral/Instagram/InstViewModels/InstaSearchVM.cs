using GoViral.Instagram.InstModels;
using GoViral.Instagram.IntsWindows;
using GoViral.Models;
using GoViral.Models.FilterResults;
using GoViral.Windows;
using InstaSharp.Endpoints;
using InstaSharp.Models;
using InstaSharp.Models.Responses;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstViewModels
{
    public class InstaSearchVM : ViewModelBase
    {
        public event Action<InstaResponseLists> OnSendToDominate = delegate { };
        public event Action<string> OnSendContentToSorter = delegate { };

        public ICommand OnCommandFromView { get; set; }

        private InstaResponseLists instaResponseLists;
        public InstaResponseLists InstaResponseLists
        {
            get { return instaResponseLists; }
            set { instaResponseLists = value; RaisePropertyChanged("InstaResponseLists"); }
        }
        public ObservableCollection<InstaResponseLists> InstaResponseListsList { get; set; }
        
        public ObservableCollection<SearchResult> SearchResultsForFilter { get; set; }
        private bool threeStateFilterchecked;
        public bool ThreeStateFilterchecked
        {
            get { return threeStateFilterchecked; }
            set
            {
                threeStateFilterchecked = value;
                foreach (var sr in SearchResultsForFilter)
                {
                    sr.IsChecked = (bool)threeStateFilterchecked;
                }

                RaisePropertyChanged("ThreeStateFilterchecked");
            }
        }
        public ObservableCollection<FilterOption> FilterOptionsList { get; set; }


        private string keyWords;
        public string KeyWords
        {
            get { return keyWords; }
            set { keyWords = value; RaisePropertyChanged("KeyWords"); }
        }

        private bool checkedSearchUsers;
        public bool CheckedSearchUsers
        {
            get { return checkedSearchUsers; }
            set { checkedSearchUsers = value; RaisePropertyChanged("CheckedSearchUsers"); }
        }
        
        private bool checkedSearchTags;
        public bool CheckedSearchTags
        {
            get { return checkedSearchTags; }
            set { checkedSearchTags = value; RaisePropertyChanged("CheckedSearchTags"); }
        }

        private bool checkedSearchMedia;
        public bool CheckedSearchMedia
        {
            get { return checkedSearchMedia; }
            set { checkedSearchMedia = value; RaisePropertyChanged("CheckedSearchMedia"); }
        }


        public InstaSearchVM()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            InstaResponseListsList = new ObservableCollection<InstaResponseLists>();

            SearchResultsForFilter = new ObservableCollection<SearchResult>();
            FilterOptionsList = new ObservableCollection<FilterOption>();

            CheckedSearchTags = CheckedSearchUsers = CheckedSearchMedia = false;

            LoadSaved();
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            using (new WaitCursor())
            {
                try
                {
                    string param = obj as string;
                    switch (param)
                    {
                        case "SEARCH":
                            if (InstaVM.Instance.InstaResponse == null)
                            {
                                "You need to log in first.".Show();
                                return;
                            }

                            if(!CheckedSearchTags && !CheckedSearchUsers && !CheckedSearchMedia)
                            {
                                "Select A Search Option".Show();
                                return;
                            }

                            if (KeyWords.IsNullOrEmpty())
                            {
                                "Enter Search Keyword".Show();
                                return;
                            }
                            string[] kws = KeyWords.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            List<Task> searches = new List<Task>();
                            foreach (var k in kws)
                            {
                                if (k.IsNullOrEmpty()) continue;
                                string kw = k.Trim();
                                kw = kw.Replace(" ", "_");
                                kw = kw.Replace("#", "");
                                //Task searchU = null, searchT = null, searchM = null;
                                //if (CheckedSearchUsers) searchU = SearchForUsers(kw);
                                //if (CheckedSearchTags) searchT = SearchForTags(kw);
                                //if (CheckedSearchMedia) searchM = SearchForMedia(kw);

                                //if (searchU != null) await searchU;
                                //if (searchT != null) await searchT;
                                //if (searchM != null) await searchM;
                                
                                if (CheckedSearchUsers) searches.Add(SearchForUsers(kw));
                                if (CheckedSearchTags) searches.Add(SearchForTags(kw));
                                if (CheckedSearchMedia) searches.Add(SearchForMedia(kw));
                            }
                            await Task.WhenAll(searches);

                            //await Task.Run(() =>
                            //    {
                            //        try
                            //        {
                            //            Task.WaitAny(searches.ToArray());
                            //        }
                            //        catch (AggregateException ae)
                            //        {
                            //            string errors = "";
                            //            foreach (var e in ae.Flatten().InnerExceptions)
                            //            {
                            //                errors += e.Message + Environment.NewLine;
                            //            }
                            //            if (!errors.IsNullOrEmpty()) throw new Exception(errors);
                            //        }
                            //    }).ContinueWith(t => {
                            //        throw new Exception(t.Exception.InnerException.Message); }, TaskContinuationOptions.OnlyOnFaulted); 
                            break;

                        case "SAVE":
                            await SaveSearch();
                            break;

                        case "CLEARUSERS":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                respontsLists.UserSearchResponse.Clear();
                                respontsLists.RasieAllCountsChanged();
                            }
                            break;

                        case "CLEARTAGS":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                respontsLists.TagsSearchResponse.Clear();
                                respontsLists.RasieAllCountsChanged();
                            }
                            break;

                        case "CLEARMedia":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                respontsLists.MediaSearchResponse.Clear();
                                respontsLists.RasieAllCountsChanged();
                            }
                            break;

                        case "DOMINATEUsers":
                        case "DOMINATETags":
                        case "DOMINATEMedia":
                        case "DOMINATETagsSelected":
                        case "DOMINATETagsAll":
                        case "DOMINATEUsersSelected":
                        case "DOMINATEUsersAll":
                        case "DOMINATEMedialected":
                        case "DOMINATEMediaAll":
                            NewList_OnRaisedCommandToViewModel(null, param);
                            break;

                        #region ct menu users
                        case "LOADAllRelationUsers":
                            await LoadAllRelationships();
                            break;

                        case "LOADAllMediaUsers":
                            await LoadAllUsersMedia();
                            break;

                        case "LOADAllCountsUsers":
                            await LoadAllUsersCounts();
                            break;

                        case "LOADAllCommentsUsers":
                            await LoadAllMediaCommentsUsers();
                            break;

                        case "ORDERFollowersUsers":
                        case "ORDERFollowingUsers":
                        case "ORDERMediaUsers":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                var orderd = respontsLists.UserSearchResponse.OrderByDescending(u => u.Counts == null ? 0 :
                                                  param == "ORDERFollowersUsers" ? u.Counts.FollowedBy :
                                                  param == "ORDERFollowingUsers" ? u.Counts.Follows :
                                                  u.Counts.Media).ToList();
                                if (orderd.Count == respontsLists.UserSearchResponse.Count)
                                {
                                    respontsLists.UserSearchResponse.Clear();
                                    foreach (var u in orderd)
                                    {
                                        respontsLists.UserSearchResponse.Add(u);
                                    }
                                }
                            }

                            break;

                        case "LIKEAllMediaUsers":
                            await LikeAllUsersMedia(true);
                            break;

                        case "LIKEAllMediaUnUsers":
                            await LikeAllUsersMedia(false);
                            break;

                        case "ORDERCommentsMediaUsers":
                        case "ORDERLikesMediaUsers":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                foreach (var u in respontsLists.UserSearchResponse)
                                {
                                    var orderdMediaUsers = u.MediaRecent.OrderByDescending(m => u.MediaRecent.Count == 0 ? 0 :
                                                                                           param == "ORDERLikesMediaUsers" ? m.Likes == null ? 0 : m.Likes.Count :
                                                                                           m.Comments == null ? 0 : m.Comments.Count).ToList();
                                    if (u.MediaRecent.Count == orderdMediaUsers.Count)
                                    {
                                        u.MediaRecent.Clear();
                                        foreach (var m in orderdMediaUsers)
                                        {
                                            u.MediaRecent.Add(m);
                                        }
                                    }
                                }
                            }
                            break;

                        case "FOLLOWAllUsers":
                            await FollowAllUsers();
                            break;

                        case "FOLLOWAcceptAllUsers":
                            await AcceptAllFolowRequests();
                            break;

                        case "FOLLOWBlockAllUsers":
                            await BloackAllUsers();
                            break;

                        case "FOLLOWUnBlockAllUsers":
                            await UnblockAllUsers();
                            break;
                        #endregion

                        #region ct menu tags
                        case "LOADAllMediaTags":
                            await LoadAllTagsMedia();
                            break;

                        case "LOADAllCommentsMediaTags":
                            await LoadAllMediaCommentsTags();
                            break;

                        case "ORDERMediaTags":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                var orderdTags = respontsLists.TagsSearchResponse.OrderByDescending(t => t.MediaCount).ToList();

                                if (orderdTags.Count == respontsLists.TagsSearchResponse.Count)
                                {
                                    respontsLists.TagsSearchResponse.Clear();
                                    foreach (var t in orderdTags)
                                    {
                                        respontsLists.TagsSearchResponse.Add(t);
                                    }
                                }
                            }
                            break;

                        case "ORDERCommentsMediaTags":
                        case "ORDERLikesMediaTags":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                foreach (var t in respontsLists.TagsSearchResponse)
                                {
                                    var orderdMediat = t.MediaRecent.OrderByDescending(m => t.MediaRecent.Count == 0 ? 0 :
                                                                                           param == "ORDERLikesMediaTags" ? m.Likes == null ? 0 : m.Likes.Count :
                                                                                           m.Comments == null ? 0 : m.Comments.Count).ToList();
                                    if (orderdMediat.Count == t.MediaRecent.Count)
                                    {
                                        t.MediaRecent.Clear();
                                        foreach (var m in orderdMediat)
                                        {
                                            t.MediaRecent.Add(m);
                                        }
                                    }
                                }
                            }
                            break;

                        case "LIKEAllMediaTags":
                            await LikeAllMediaTags(true);
                            break;

                        case "LIKEAllMediaTagsUn":
                            await LikeAllMediaTags(false);
                            break;
                        #endregion

                        #region ct menu media
                        case "LOADAllCommentsMedia":
                            await LoadAllMediaComments();
                            break;

                        case "ORDERCommentsMedia":
                        case "ORDERLikesMedia":
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                var orderdMedia = respontsLists.MediaSearchResponse.OrderByDescending(m => param == "ORDERCommentsMedia" ? m.Comments == null ? 0 : m.Comments.Count :
                                                              m.Likes == null ? 0 : m.Likes.Count).ToList();

                                if (orderdMedia.Count == respontsLists.MediaSearchResponse.Count)
                                {
                                    respontsLists.MediaSearchResponse.Clear();
                                    foreach (var m in orderdMedia)
                                    {
                                        respontsLists.MediaSearchResponse.Add(m);
                                    }
                                }
                            }
                            break;

                        case "LIKEAllMedia":
                            await LikeAllMediasMedia(true);
                            break;

                        case "LIKEAllMediaUn":
                            await LikeAllMediasMedia(false);
                            break;
                        #endregion

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
                            foreach (var respontsLists in InstaResponseListsList)
                            {
                                respontsLists.OnCommandFromView_Raised(obj);
                            }
                            break;

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

        private async Task SaveSearch()
        {
            await Task.Run(()=> 
            {
                string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "insteo", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                foreach (var kwSearch in InstaResponseListsList)
                {
                    JsonSerializerSettings jsSettings = new JsonSerializerSettings();
                    jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    jsSettings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;

                    string kwPath = Path.Combine(dirPath, kwSearch.Tag);
                    if (!Directory.Exists(kwPath)) Directory.CreateDirectory(kwPath);

                    string filePath = Path.Combine(kwPath, InstaVM.PATHSEARCHFILE);

                    string json = JsonConvert.SerializeObject(kwSearch, jsSettings);

                    File.WriteAllText(filePath, json);
                }
            });
        }

        private async void LoadSaved()
        {
            try
            {
                List<KeyValuePair<string, string>> kwThenJson = await LoadJsonList();
                if (kwThenJson == null) return;

                foreach (var kwJson in kwThenJson)
                {
                    InstaResponseLists loadedResponse = JsonConvert.DeserializeObject<InstaResponseLists>(kwJson.Value);
                    if(loadedResponse != null)
                    {
                        loadedResponse.OnRaisedCommandToViewModel += NewList_OnRaisedCommandToViewModel;
                        loadedResponse.OnSendContentToSorter += LoadedResponse_OnSendContentToSorter;
                        loadedResponse.SetAllChildEvents();
                        InstaResponseListsList.Add(loadedResponse);
                    }
                }
            }
            catch
            { }
        }

        private void LoadedResponse_OnSendContentToSorter(string contents)
        {
            OnSendContentToSorter(contents);
        }

        private Task<List<KeyValuePair<string, string>>> LoadJsonList()
        {
            return Task.Run(() =>
            {
                string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "insteo", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(dirPath)) return null;

                List<KeyValuePair<string, string>> kwThenJson = new List<KeyValuePair<string, string>>();

                foreach (var kwSearch in new DirectoryInfo(dirPath).GetDirectories())
                {
                    string filePath = Path.Combine(kwSearch.FullName, InstaVM.PATHSEARCHFILE);
                    if (!File.Exists(filePath)) continue;

                    kwThenJson.Add(new KeyValuePair<string, string>(kwSearch.Name, File.ReadAllText(filePath)));
                }

                return kwThenJson;
            });
        }

        #region bulk media
        private async Task LikeAllMediasMedia(bool isLike)
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.LikeAllMediasMedia(isLike));

            await Task.WhenAll(loadList);
        }

        private async Task LoadAllMediaComments()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.LoadAllMediaComments());

            await Task.WhenAll(loadList);
        }
        #endregion

        #region bulk Tags
        private async Task LoadAllTagsMedia()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.LoadAllTagsMedia());

            await Task.WhenAll(loadList);
        }

        private async Task LoadAllMediaCommentsTags()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                    loadList.Add(respontsLists.LoadAllMediaCommentsTags());

            await Task.WhenAll(loadList);
        }

        private async Task LikeAllMediaTags(bool isLike)
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                    loadList.Add(respontsLists.LikeAllMediaTags(isLike));

            await Task.WhenAll(loadList);
        }
        #endregion

        #region bulk users
        private async Task UnblockAllUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.UnblockAllUsers());

            await Task.WhenAll(loadList);
        }

        private async Task BloackAllUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.BloackAllUsers());

            await Task.WhenAll(loadList);
        }

        private async Task AcceptAllFolowRequests()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.AcceptAllFolowRequests());
            
            await Task.WhenAll(loadList);
        }

        private async Task FollowAllUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.FollowAllUsers());

            await Task.WhenAll(loadList);
        }

        private async Task LikeAllUsersMedia(bool isLike)
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                    loadList.Add(respontsLists.LikeAllUsersMedia(isLike));

            await Task.WhenAll(loadList);
        }

        private async Task LoadAllUsersCounts()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.LoadAllUsersCounts());

            await Task.WhenAll(loadList);
        }

        private async Task LoadAllUsersMedia()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.LoadAllUsersMedia());

            await Task.WhenAll(loadList);
        }

        private async Task LoadAllMediaCommentsUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                    loadList.Add(respontsLists.LoadAllMediaCommentsUsers());

            await Task.WhenAll(loadList);
        }

        private async Task LoadAllRelationships()
        {
            List<Task> loadList = new List<Task>();
            foreach (var respontsLists in InstaResponseListsList)
                loadList.Add(respontsLists.LoadAllRelationships());

            await Task.WhenAll(loadList);
        }
        #endregion

        private async Task SearchForMedia(string kw)
        {
            MediasResponse response = await InstaVM.Instance.GetEndpointTags().Recent(kw);
            InstaResponseLists newList = InstaResponseListsList.FirstOrDefault(t => t.Tag == kw);
            if (newList == null)
            {
                newList = new InstaResponseLists(kw);
                newList.OnRaisedCommandToViewModel += NewList_OnRaisedCommandToViewModel;
                newList.OnSendContentToSorter += LoadedResponse_OnSendContentToSorter;
                InstaResponseListsList.Add(newList);
            }
            InstaVM.Instance.CheckShouldnotContinueRequest(newList.MediaSearchResponse, response.Data, response);

            foreach (var m in response.Data)
            {
                InstaMedia im = new InstaMedia(m);
                newList.AddMedia(im);
            }

            newList.RasieAllCountsChanged();
        }

        private async Task SearchForTags(string kw)
        {
            TagsResponse response = await InstaVM.Instance.GetEndpointTags().Search(kw);
            InstaResponseLists newList = InstaResponseListsList.FirstOrDefault(t => t.Tag == kw);
            if (newList == null)
            {
                newList = new InstaResponseLists(kw);
                newList.OnRaisedCommandToViewModel += NewList_OnRaisedCommandToViewModel;
                newList.OnSendContentToSorter += LoadedResponse_OnSendContentToSorter;
                InstaResponseListsList.Add(newList);
            }
            InstaVM.Instance.CheckShouldnotContinueRequest(newList.TagsSearchResponse, response.Data, response);

            foreach (var t in response.Data)
            {
                InstaTag ta = new InstaTag(t);
                newList.AddTag(ta);
            }

            newList.RasieAllCountsChanged();
        }

        private async Task SearchForUsers(string kw)
        {
            UsersResponse response = await InstaVM.Instance.GetEndpointUsers().Search(kw, null);
            InstaResponseLists newList = InstaResponseListsList.FirstOrDefault(t => t.Tag == kw);
            if (newList == null)
            {
                newList = new InstaResponseLists(kw);
                newList.OnRaisedCommandToViewModel += NewList_OnRaisedCommandToViewModel;
                newList.OnSendContentToSorter += LoadedResponse_OnSendContentToSorter;
                InstaResponseListsList.Add(newList);
            }
            InstaVM.Instance.CheckShouldnotContinueRequest(newList.UserSearchResponse, response.Data, response);

            foreach (var u in response.Data)
            {
                var ur = new InstaUser(u);
                newList.AddUser(ur);
                //await getRelationshipWith(ur).ConfigureAwait(continueOnCapturedContext: false);
            }
            newList.RasieAllCountsChanged();

            //List<InstaUserResult> thisUsersResult = new List<InstaUserResult>();

            //await GetEndpointUsers().Search(kw, null).ContinueWith((response) =>
            //{
            //    checkShouldnotContinueRequest(UserSearchResponse, response.Result == null ? null : response.Result.Data, response.Result);
            //    foreach (var u in response.Result.Data)
            //    {
            //        var ur = createInsaUserFromUser(u);
            //        UserSearchResponse.Add(ur);
            //        thisUsersResult.Add(ur);
            //        // getRelationshipWith(ur);
            //    }
            //}).ConfigureAwait(continueOnCapturedContext: false);


            //List<Task> tListRelationships = new List<Task>();
            //foreach (var u in thisUsersResult)
            //{
            //    tListRelationships.Add(getRelationshipWith(u));
            //}
            //await Task.WhenAll(tListRelationships);


            //await Task.WhenAll(tListRelationships).ConfigureAwait(continueOnCapturedContext: false);
            // return Task<List<Task>>.WhenAll<Task>(tListRelationships);
            //Task.Factory.StartNew(() =>
            //{
            //    try
            //    {
            //        Task.WaitAll(tListRelationships.ToArray());
            //    }
            //    catch (AggregateException ae)
            //    {
            //        string errors = "";
            //        foreach (var e in ae.Flatten().InnerExceptions)
            //        {
            //            errors += e.Message + Environment.NewLine;
            //        }
            //        if (!errors.IsNullOrEmpty()) throw new Exception(errors);
            //    }
            //}).ContinueWith(t => {
            //    throw new Exception(t.Exception.InnerException.Message);
            //}, TaskContinuationOptions.OnlyOnFaulted);
        }


        private void NewList_OnRaisedCommandToViewModel(InstaResponseLists responseList, string command)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) Mouse.OverrideCursor = null;

            SearchResultsForFilter.Clear();
            FilterOptionsList.Clear();

            InstaResponseLists resultsList = new InstaResponseLists("");
            SelectOptionToSendWindow sotsw = new SelectOptionToSendWindow();
            try
            {
                switch (command)
                {
                    case "DOMINATEUsers":
                        addDominateOptionsUsers(responseList);
                        break;

                    case "DOMINATETags":
                        addDominateOptionsTags(responseList);
                        break;

                    case "DOMINATEMedia":
                        addDominateOptionsMedia(responseList);
                        break;
                        
                    case "DOMINATETagsSelected":
                        if (responseList == null)
                        {
                            foreach (var item in InstaResponseListsList)
                            {
                                foreach (var tag in item.TagsSearchResponse)
                                {
                                    if (!tag.IsChecked) continue;

                                    resultsList.TagsSearchResponse.Add(tag);
                                    foreach (var m in tag.MediaRecent)
                                    {
                                        if (!m.IsChecked) continue;
                                        resultsList.MediaSearchResponse.Add(m);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in responseList.TagsSearchResponse)
                            {
                                if (!item.IsChecked) continue;

                                resultsList.TagsSearchResponse.Add(item);
                                foreach (var m in item.MediaRecent)
                                {
                                    if (!m.IsChecked) continue;
                                    resultsList.MediaSearchResponse.Add(m);
                                }
                            }
                        }

                        sotsw.DataContext = resultsList;
                        break;

                    case "DOMINATEUsersSelected":
                        if (responseList == null)
                        {
                            foreach (var item in InstaResponseListsList)
                            {
                                foreach (var tag in item.UserSearchResponse)
                                {
                                    if (!tag.IsChecked) continue;
                                    resultsList.UserSearchResponse.Add(tag);

                                    foreach (var m in tag.MediaRecent)
                                    {
                                        if (!m.IsChecked) continue;
                                        resultsList.MediaSearchResponse.Add(m);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in responseList.UserSearchResponse)
                            {
                                if (!item.IsChecked) continue;
                                resultsList.UserSearchResponse.Add(item);

                                foreach (var m in item.MediaRecent)
                                {
                                    if (!m.IsChecked) continue;
                                    resultsList.MediaSearchResponse.Add(m);
                                }
                            }
                        }

                        sotsw.DataContext = resultsList;
                        break;

                    case "DOMINATEMedialected":
                        if (responseList == null)
                        {
                            foreach (var item in InstaResponseListsList)
                            {
                                foreach (var tag in item.MediaSearchResponse)
                                {
                                    if (!tag.IsChecked) continue;
                                    resultsList.MediaSearchResponse.Add(tag);
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in responseList.MediaSearchResponse)
                            {
                                if (!item.IsChecked) continue;
                                resultsList.MediaSearchResponse.Add(item);
                            }
                        }

                        sotsw.DataContext = resultsList;
                        break;

                    case "DOMINATETagsAll":
                        if(responseList == null)
                        {
                            foreach (var item in InstaResponseListsList)
                            {
                                foreach (var tag in item.TagsSearchResponse)
                                {
                                    resultsList.TagsSearchResponse.Add(tag);
                                    foreach (var m in tag.MediaRecent)
                                    {
                                        resultsList.MediaSearchResponse.Add(m);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in responseList.TagsSearchResponse)
                            {
                                resultsList.TagsSearchResponse.Add(item);
                                foreach (var m in item.MediaRecent)
                                {
                                    resultsList.MediaSearchResponse.Add(m);
                                }
                            }
                        }

                        sotsw.DataContext = resultsList;
                        break;

                    case "DOMINATEUsersAll":
                        if (responseList == null)
                        {
                            foreach (var item in InstaResponseListsList)
                            {
                                foreach (var tag in item.UserSearchResponse)
                                {
                                    resultsList.UserSearchResponse.Add(tag);

                                    foreach (var m in tag.MediaRecent)
                                    {
                                        resultsList.MediaSearchResponse.Add(m);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in responseList.UserSearchResponse)
                            {
                                resultsList.UserSearchResponse.Add(item);

                                foreach (var m in item.MediaRecent)
                                {
                                    resultsList.MediaSearchResponse.Add(m);
                                }
                            }
                        }

                        sotsw.DataContext = resultsList;
                        break;

                    case "DOMINATEMediaAll":
                        if (responseList == null)
                        {
                            foreach (var item in InstaResponseListsList)
                            {
                                foreach (var tag in item.MediaSearchResponse)
                                {
                                    resultsList.MediaSearchResponse.Add(tag);
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in responseList.MediaSearchResponse)
                            {
                                resultsList.MediaSearchResponse.Add(item);
                            }
                        }

                        sotsw.DataContext = resultsList;
                        break;

                    default:
                        break;
                }

                if (sotsw.DataContext != null)
                {
                    if (sotsw.ShowDialog() == true)
                    {
                        OnSendToDominate(resultsList);
                    }
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

        private void addDominateOptionsUsers(InstaResponseLists responseList)
        {
            foreach (var l in InstaResponseListsList)
            {
                SearchResult sr =  new SearchResult() { Keyword = l.Tag };
                if (responseList != null && responseList.Tag == sr.Keyword) sr.IsChecked = true;
                SearchResultsForFilter.Add(sr);
            }

            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Following, Title = OptionType.Following.GetDescription(), ListState = ListType.Users });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Followers, Title = OptionType.Followers.GetDescription(), ListState = ListType.Users });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Media, Title = OptionType.Media.GetDescription(), ListState = ListType.Users });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Comments, Title = OptionType.Comments.GetDescription(), ListState = ListType.Users });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Likes, Title = OptionType.Likes.GetDescription(), ListState = ListType.Users });
            ShowFilterWindow();
        }

        private void addDominateOptionsTags(InstaResponseLists responseList)
        {
            foreach (var l in InstaResponseListsList)
            {
                SearchResult sr = new SearchResult() { Keyword = l.Tag };
                if (responseList != null && responseList.Tag == sr.Keyword) sr.IsChecked = true;
                SearchResultsForFilter.Add(sr);
            }

            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Media, Title = OptionType.Media.GetDescription(), ListState = ListType.Tags });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Comments, Title = OptionType.Comments.GetDescription(), ListState = ListType.Tags });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Likes, Title = OptionType.Likes.GetDescription(), ListState = ListType.Tags });
            ShowFilterWindow();
        }

        private void addDominateOptionsMedia(InstaResponseLists responseList)
        {
            foreach (var l in InstaResponseListsList)
            {
                SearchResult sr = new SearchResult() { Keyword = l.Tag };
                if (responseList != null && responseList.Tag == sr.Keyword) sr.IsChecked = true;
                SearchResultsForFilter.Add(sr);
            }

            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Comments, Title = OptionType.Comments.GetDescription(), ListState = ListType.Media });
            FilterOptionsList.Add(new FilterOption() { OptionState = OptionType.Likes, Title = OptionType.Likes.GetDescription(), ListState = ListType.Media });
            ShowFilterWindow();
        }

        private void ShowFilterWindow()
        {
            TopNsearchResultsToDominateWindow filterWindow = new TopNsearchResultsToDominateWindow();
            filterWindow.DataContext = this;
            if(filterWindow.ShowDialog() == true)
            {
                InstaResponseLists resultsList = new InstaResponseLists("");

                int countsMax = filterWindow.MaxNums;
                int totalChecked = FilterOptionsList.Count(o => o.IsChecked);
                if (totalChecked == 0 || countsMax == 0) return;

                #region order then add
                foreach (var sr in SearchResultsForFilter)
                {
                    InstaResponseLists responseCollection = InstaResponseListsList.FirstOrDefault(i => i.Tag == sr.Keyword);
                    if (responseCollection == null) continue;

                    foreach (var filterOption in FilterOptionsList)
                    {
                        if (!filterOption.IsChecked) continue;


                        switch (filterOption.OptionState)
                        {
                            case OptionType.Likes:
                                switch (filterOption.ListState)
                                {
                                    case ListType.Users:
                                        foreach (var u in responseCollection.UserSearchResponse)
                                        {
                                            if (u.MediaRecent.Count == 0) continue;

                                            var orderd = u.MediaRecent.OrderByDescending(m => m.Likes == null ? 0 : m.Likes.Count).ToList();
                                            if (filterWindow.rbBottom.IsChecked == true) orderd.Reverse();
                                            foreach (var m in orderd)
                                            {
                                                if (m.Likes == null || m.Likes.Count < filterOption.StartingFrom) continue;

                                                InstaMedia newM = new InstaMedia(m);
                                                if(!resultsList.MediaSearchResponse.Any(mm=>m.Link == mm.Link))resultsList.MediaSearchResponse.Add(newM);
                                            }
                                        }
                                        break;

                                    case ListType.Media:
                                        var orderdM = responseCollection.MediaSearchResponse.OrderByDescending(m => m.Likes == null ? 0 : m.Likes.Count).ToList();
                                        if (filterWindow.rbBottom.IsChecked == true)
                                        {
                                            orderdM.Reverse();
                                        }
                                        foreach (var m in orderdM)
                                        {
                                            if (m.Likes == null || m.Likes.Count < filterOption.StartingFrom) continue;

                                            InstaMedia newM = new InstaMedia(m);
                                            if (!resultsList.MediaSearchResponse.Any(mm => m.Link == mm.Link)) resultsList.MediaSearchResponse.Add(newM);
                                        }
                                        break;

                                    case ListType.Tags:
                                        foreach (var t in responseCollection.TagsSearchResponse)
                                        {
                                            if (t.MediaRecent.Count == 0) continue;

                                            var orderd = t.MediaRecent.OrderByDescending(m => m.Likes == null ? 0 : m.Likes.Count).ToList();
                                            if (filterWindow.rbBottom.IsChecked == true) orderd.Reverse();

                                            foreach (var m in orderd)
                                            {
                                                if (m.Likes == null || m.Likes.Count < filterOption.StartingFrom) continue;
                                                InstaMedia newM = new InstaMedia(m);
                                                if (!resultsList.MediaSearchResponse.Any(mm => m.Link == mm.Link)) resultsList.MediaSearchResponse.Add(newM);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case OptionType.Comments:
                                switch (filterOption.ListState)
                                {
                                    case ListType.Users:
                                        foreach (var u in responseCollection.UserSearchResponse)
                                        {
                                            if (u.MediaRecent.Count == 0) continue;

                                            var orderd = u.MediaRecent.OrderByDescending(m => m.Comments == null ? 0 : m.Comments.Count).ToList();
                                            if (filterWindow.rbBottom.IsChecked == true) orderd.Reverse();

                                            foreach (var m in orderd)
                                            {
                                                InstaMedia newM = new InstaMedia(m);
                                                if (!resultsList.MediaSearchResponse.Any(mm => m.Link == mm.Link)) resultsList.MediaSearchResponse.Add(newM);
                                            }
                                        }
                                        break;

                                    case ListType.Media:
                                        var orderdM = responseCollection.MediaSearchResponse.OrderByDescending(m => m.Comments == null ? 0 : m.Comments.Count).ToList();
                                        if (filterWindow.rbBottom.IsChecked == true)
                                        {
                                            orderdM.Reverse();
                                        }
                                        foreach (var m in orderdM)
                                        {
                                            if (m.Comments == null || m.Comments.Count < filterOption.StartingFrom) continue;

                                            InstaMedia newM = new InstaMedia(m);
                                            if (!resultsList.MediaSearchResponse.Any(mm => m.Link == mm.Link)) resultsList.MediaSearchResponse.Add(newM);
                                        }
                                        break;

                                    case ListType.Tags:
                                        foreach (var t in responseCollection.TagsSearchResponse)
                                        {
                                            if (t.MediaRecent.Count == 0) continue;

                                            var orderd = t.MediaRecent.OrderByDescending(m => m.Comments == null ? 0 : m.Comments.Count).ToList();
                                            if (filterWindow.rbBottom.IsChecked == true) orderd.Reverse();

                                            foreach (var m in orderd)
                                            {
                                                if (m.Comments == null || m.Comments.Count < filterOption.StartingFrom) continue;

                                                InstaMedia newM = new InstaMedia(m);
                                                if (!resultsList.MediaSearchResponse.Any(mm => m.Link == mm.Link)) resultsList.MediaSearchResponse.Add(newM);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Media:
                                switch (filterOption.ListState)
                                {
                                    case ListType.Users:
                                        var orderdU = responseCollection.UserSearchResponse.OrderByDescending(u => u.Counts == null ? 0 : u.Counts.Media).ToList();
                                        if (filterWindow.rbBottom.IsChecked == true) orderdU.Reverse();

                                        foreach (var u in orderdU)
                                        {
                                            if (u.Counts == null || u.Counts.Media < filterOption.StartingFrom) continue;

                                            InstaUser newU = new InstaUser(u);
                                            if (!resultsList.UserSearchResponse.Any(mm => u.Id == mm.Id)) resultsList.UserSearchResponse.Add(newU);
                                        }
                                        break;
                                    case ListType.Tags:
                                        var orderdT = responseCollection.TagsSearchResponse.OrderByDescending(u => u.MediaCount).ToList();
                                        if (filterWindow.rbBottom.IsChecked == true) orderdT.Reverse();

                                        foreach (var t in orderdT)
                                        {
                                            if (t.MediaCount < filterOption.StartingFrom) continue;

                                            InstaTag newT = new InstaTag(t);
                                            if (!resultsList.TagsSearchResponse.Any(mm => t.Name == mm.Name)) resultsList.TagsSearchResponse.Add(newT);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Following:
                                var orderdUF = responseCollection.UserSearchResponse.OrderByDescending(u => u.Counts == null ? 0 : u.Counts.Media).ToList();
                                if (filterWindow.rbBottom.IsChecked == true) orderdUF.Reverse();

                                foreach (var u in orderdUF)
                                {
                                    if (u.Counts == null || u.Counts.Follows < filterOption.StartingFrom) continue;
                                    InstaUser newU = new InstaUser(u);
                                    if (!resultsList.UserSearchResponse.Any(mm => u.Id == mm.Id)) resultsList.UserSearchResponse.Add(newU);
                                }
                                break;
                            case OptionType.Followers:
                                var orderdUFo = responseCollection.UserSearchResponse.OrderByDescending(u => u.Counts == null ? 0 : u.Counts.Media).ToList();
                                if (filterWindow.rbBottom.IsChecked == true) orderdUFo.Reverse();

                                foreach (var u in orderdUFo)
                                {
                                    if (u.Counts == null || u.Counts.FollowedBy < filterOption.StartingFrom) continue;
                                    InstaUser newU = new InstaUser(u);
                                    if (!resultsList.UserSearchResponse.Any(mm => u.Id == mm.Id)) resultsList.UserSearchResponse.Add(newU);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                #endregion
                

                if(resultsList.UsersCount + resultsList.TagsCount + resultsList.MediaCount > countsMax)
                {
                    countsMax = countsMax / totalChecked;
                    if (countsMax <= 0) countsMax = totalChecked;

                    List<InstaUser> users = new List<InstaUser>(); 
                    List<InstaTag> tags = new List<InstaTag>();
                    List<InstaMedia> media = new List<InstaMedia>();

                    foreach (var u in resultsList.UserSearchResponse)
                    {
                        if (users.Count < countsMax)
                        {
                            users.Add(u);
                        }
                    }

                    foreach (var u in resultsList.TagsSearchResponse)
                    {
                        if (tags.Count < countsMax)
                        {
                            tags.Add(u);
                        }
                    }

                    foreach (var u in resultsList.MediaSearchResponse)
                    {
                        if (media.Count < countsMax)
                        {
                            media.Add(u);
                        }
                    }


                    resultsList.UserSearchResponse.Clear();
                    resultsList.TagsSearchResponse.Clear();
                    resultsList.MediaSearchResponse.Clear();

                    foreach (var u in users) resultsList.UserSearchResponse.Add(u);
                    foreach (var t in tags) resultsList.TagsSearchResponse.Add(t);
                    foreach (var m in media) resultsList.MediaSearchResponse.Add(m);

                }

                SelectOptionToSendWindow sotsw = new SelectOptionToSendWindow();
                sotsw.DataContext = resultsList;
                if (sotsw.ShowDialog() == true)
                {
                    OnSendToDominate(resultsList);
                }
            }
        }
    }
}
