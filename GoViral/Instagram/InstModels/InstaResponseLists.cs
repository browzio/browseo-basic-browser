using GoViral.Instagram.InstModels;
using GoViral.Instagram.InstViewModels;
using InstaSharp.Endpoints;
using InstaSharp.Models;
using InstaSharp.Models.Responses;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstModels
{
    public class InstaResponseLists : ViewModelBase
    {
        public event Action<string> OnSendContentToSorter = delegate { };
         
        public event Action<InstaResponseLists, string> OnRaisedCommandToViewModel = delegate { };

        [Newtonsoft.Json.JsonIgnore]
        public ICommand OnCommandFromView { get; set; }

        private string tag;
        public string Tag
        {
            get { return tag; }
            set { tag = value; RaisePropertyChanged("Tag"); }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                foreach (var i in UserSearchResponse)
                {
                    i.IsChecked = isChecked;
                }
                foreach (var i in TagsSearchResponse)
                {
                    i.IsChecked = isChecked;
                }
                foreach (var i in MediaSearchResponse)
                {
                    i.IsChecked = isChecked;
                }
                RaisePropertyChanged("IsChecked");
            }
        }

        public int UsersCount { get { return UserSearchResponse.Count; } }
        public int TagsCount { get { return TagsSearchResponse.Count; } }
        public int MediaCount { get { return MediaSearchResponse.Count; } }

        public ObservableCollection<InstaUser> UserSearchResponse { get; set; }
        public ObservableCollection<InstaTag> TagsSearchResponse { get; set; }
        public ObservableCollection<InstaMedia> MediaSearchResponse { get; set; }

        private RssFeedsLinksMultiWindow multiWindowForLinksAdd;

        public InstaResponseLists(string tag)
        {
            Tag = tag;

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            UserSearchResponse = new ObservableCollection<InstaUser>();
            TagsSearchResponse = new ObservableCollection<InstaTag>();
            MediaSearchResponse = new ObservableCollection<InstaMedia>();

            UserSearchResponse.CollectionChanged += UserSearchResponse_CollectionChanged;
            MediaSearchResponse.CollectionChanged += MediaSearchResponse_CollectionChanged;
            TagsSearchResponse.CollectionChanged += TagsSearchResponse_CollectionChanged;
        }


        public async void OnCommandFromView_Raised(object obj)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            using (new WaitCursor())
            {
                try
                {
                    string param = obj as string;
                    switch (param)
                    {
                        case "CLEARUSERS":
                            UserSearchResponse.Clear();
                            break;

                        case "CLEARTAGS":
                            TagsSearchResponse.Clear();
                            break;

                        case "CLEARMedia":
                            MediaSearchResponse.Clear();
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
                            var orderd = UserSearchResponse.OrderByDescending(u => u.Counts == null ? 0 :
                                              param == "ORDERFollowersUsers" ? u.Counts.FollowedBy :
                                              param == "ORDERFollowingUsers" ? u.Counts.Follows :
                                              u.Counts.Media).ToList();
                            if (orderd.Count == UserSearchResponse.Count)
                            {
                                UserSearchResponse.Clear();
                                foreach (var u in orderd)
                                {
                                    UserSearchResponse.Add(u);
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
                            foreach (var u in UserSearchResponse)
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
                            var orderdTags = TagsSearchResponse.OrderByDescending(t => t.MediaCount).ToList();

                            if (orderdTags.Count == TagsSearchResponse.Count)
                            {
                                TagsSearchResponse.Clear();
                                foreach (var t in orderdTags)
                                {
                                    TagsSearchResponse.Add(t);
                                }
                            }
                            break;

                        case "ORDERCommentsMediaTags":
                        case "ORDERLikesMediaTags":
                            foreach (var t in TagsSearchResponse)
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
                            var orderdMedia = MediaSearchResponse.OrderByDescending(m => param == "ORDERCommentsMedia" ? m.Comments == null ? 0 : m.Comments.Count :
                                                          m.Likes == null ? 0 : m.Likes.Count).ToList();

                            if (orderdMedia.Count == MediaSearchResponse.Count)
                            {
                                MediaSearchResponse.Clear();
                                foreach (var m in orderdMedia)
                                {
                                    MediaSearchResponse.Add(m);
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

                        case "DOMINATEUsers":
                        case "DOMINATETags":
                        case "DOMINATEMedia":
                            OnRaisedCommandToViewModel(this, param);
                            break;

                        case "SellectAllUsers":
                        case "SellectNonUsers":
                            foreach (var u in UserSearchResponse)
                            {
                                u.IsChecked = param == "SellectAllUsers" ? true : false;
                                foreach (var m in u.MediaRecent)
                                {
                                    m.IsChecked = param == "SellectAllUsers" ? true : false;
                                }
                            }
                            break;
                        case "SellectAllTags":
                        case "SellectNonTags":
                            foreach (var t in TagsSearchResponse)
                            {
                               t.IsChecked = param == "SellectAllTags" ? true : false;
                                foreach (var m in t.MediaRecent)
                                {
                                    m.IsChecked = param == "SellectAllTags" ? true : false;
                                }
                            }
                            break;
                        case "SellectAllMedia":
                        case "SellectNonMedia":
                            foreach (var m in MediaSearchResponse)
                            {
                                m.IsChecked = param == "SellectAllMedia" ? true : false;
                            }
                            break;

                        case "SENDTOSORTERAllUsers":
                            foreach (var u in UserSearchResponse)
                            {
                                AddUrlForMulti(u.Username + "," + InstaVM.LINK_INSTAGRAM + u.Username);
                                foreach (var m in u.MediaRecent)
                                {
                                    AddUrlForMulti(u.Username + " Media " + "," + m.Link);
                                }
                            }
                            break;

                        case "SENDTOSORTERSelectedUsers":
                            foreach (var u in UserSearchResponse)
                            {
                                if (!u.IsChecked) continue;

                                AddUrlForMulti(u.Username + "," + InstaVM.LINK_INSTAGRAM + u.Username);
                                foreach (var m in u.MediaRecent)
                                {
                                    if (!m.IsChecked) continue;
                                    AddUrlForMulti(u.Username + " Media " + "," + m.Link);
                                }
                            }
                            break;

                        case "SENDTOSORTERAllTags":
                            foreach (var t in TagsSearchResponse)
                            {
                                AddUrlForMulti(t.Name + "," + InstaVM.LINK_INSTAGRAM + t.Name);
                                foreach (var m in t.MediaRecent)
                                {
                                    AddUrlForMulti(t.Name + " Media " + "," + m.Link);
                                }
                            }
                            break;

                        case "SENDTOSORTERSelectedTags":
                            foreach (var t in TagsSearchResponse)
                            {
                                if (!t.IsChecked) continue;

                                AddUrlForMulti(t.Name + "," + InstaVM.LINK_INSTAGRAM + t.Name);
                                foreach (var m in t.MediaRecent)
                                {
                                    if (!m.IsChecked) continue;
                                    AddUrlForMulti(t.Name + " Media " + "," + m.Link);
                                }
                            }
                            break;

                        case "SENDTOSORTERAllMedia":
                            foreach (var m in MediaSearchResponse)
                            {
                                AddUrlForMulti(m.User == null ? "Media Link" : m.User.Username + " Media " + "," + m.Link);
                            }
                            break;

                        case "SENDTOSORTERSelectedMedia":
                            foreach (var m in MediaSearchResponse)
                            {
                                if (!m.IsChecked) continue;

                                AddUrlForMulti(m.User == null ? "Media Link" : m.User.Username + " Media " + "," + m.Link);
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.Show();
                }
            }
        }

        private void AddUrlForMulti(string text)
        {
            if (multiWindowForLinksAdd == null)
            {
                multiWindowForLinksAdd = new RssFeedsLinksMultiWindow();
                multiWindowForLinksAdd.Closed += MultiWindowForLinksAdd_Closed;
                multiWindowForLinksAdd.Title = "Page Name , Url";
                multiWindowForLinksAdd.Show();
            }
            text = text.RemoveAmps();
            multiWindowForLinksAdd.tbInputedText.Text += text + Environment.NewLine;
        }
        private void MultiWindowForLinksAdd_Closed(object sender, EventArgs e)
        {
            if (multiWindowForLinksAdd.ButtonLeftClicked && !multiWindowForLinksAdd.tbInputedText.Text.IsNullOrEmpty())
            {
                OnSendContentToSorter(multiWindowForLinksAdd.tbInputedText.Text);
            }

            multiWindowForLinksAdd = null;
        }

        internal void SetAllChildEvents()
        {
            if (UserSearchResponse != null)
                foreach (var u in UserSearchResponse)
                {
                    u.OnRaisedCommandToViewModel += UserResult_OnRaisedCommandToViewModel;

                    if(u.MediaRecent!=null)
                        foreach (var m in u.MediaRecent)
                            m.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                }

            if (TagsSearchResponse != null)
                foreach (var t in TagsSearchResponse)
                {
                    t.OnRaisedCommandToViewModel += TagResult_OnRaisedCommandToViewModel;

                    if (t.MediaRecent != null)
                        foreach (var m in t.MediaRecent)
                            m.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                }

            if(MediaSearchResponse!=null)
                foreach (var m in MediaSearchResponse)
                    m.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
        }

        #region bulk media
        public async Task LikeAllMediasMedia(bool isLike)
        {
            List<Task> loadList = new List<Task>();
            foreach (var m in MediaSearchResponse)
            {
                loadList.Add(LikeMedia(m, isLike));
            }

            await Task.WhenAll(loadList);
        }

        public async Task LoadAllMediaComments()
        {
            List<Task> loadList = new List<Task>();
            foreach (var m in MediaSearchResponse)
            {
                loadList.Add(LoadComments(m));
            }

            await Task.WhenAll(loadList);
        }
        #endregion

        #region bulk Tags
        public async Task LoadAllTagsMedia()
        {
            List<Task> loadList = new List<Task>();
            foreach (var t in TagsSearchResponse)
            {
                loadList.Add(LoadTagsRecentMedia(t));
            }

            await Task.WhenAll(loadList);
        }

        public async Task LoadAllMediaCommentsTags()
        {
            List<Task> loadList = new List<Task>();
            foreach (var t in TagsSearchResponse)
            {
                foreach (var m in t.MediaRecent)
                {
                    loadList.Add(LoadComments(m));
                }
            }

            await Task.WhenAll(loadList);
        }

        public async Task LikeAllMediaTags(bool isLike)
        {
            List<Task> loadList = new List<Task>();
            foreach (var t in TagsSearchResponse)
            {
                foreach (var m in t.MediaRecent)
                {
                    loadList.Add(LikeMedia(m, isLike));
                }
            }

            await Task.WhenAll(loadList);
        }
        #endregion

        #region bulk users
        public async Task UnblockAllUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                loadList.Add(UnBlockUser(u));
            }

            await Task.WhenAll(loadList);
        }

        public async Task BloackAllUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                loadList.Add(BlockUser(u));
            }

            await Task.WhenAll(loadList);
        }

        public async Task AcceptAllFolowRequests()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                if (u.Relationship == null || u.Relationship.IncomingStatus != IncomingStatus.RequestedBy) continue;

                loadList.Add(FolowUser(u));
            }

            await Task.WhenAll(loadList);
        }

        public async Task FollowAllUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                loadList.Add(FolowUser(u));
            }

            await Task.WhenAll(loadList);
        }

        public async Task LikeAllUsersMedia(bool isLike)
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                foreach (var m in u.MediaRecent)
                {
                    loadList.Add(LikeMedia(m, isLike));
                }
            }

            await Task.WhenAll(loadList);
        }

        public async Task LoadAllUsersCounts()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                loadList.Add(LoadUsersCounts(u));
            }

            await Task.WhenAll(loadList);
        }

        public async Task LoadAllUsersMedia()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                loadList.Add(LoadUsersMedia(u));
            }

            await Task.WhenAll(loadList);
        }

        public async Task LoadAllMediaCommentsUsers()
        {
            List<Task> loadList = new List<Task>();
            foreach (var u in UserSearchResponse)
            {
                foreach (var m in u.MediaRecent)
                {
                    loadList.Add(LoadComments(m));
                }
            }

            await Task.WhenAll(loadList);
        }

        public async Task LoadAllRelationships()
        {
            List<Task<Relationship>> loadList = new List<Task<Relationship>>();
            foreach (var u in UserSearchResponse)
            {
                loadList.Add(LoadRelationshipWith(u));
            }

            //while(loadList.Count > 0)
            //{
            //    Task<Relationship> loaded =   await Task.WhenAny(loadList);
            //    UserSearchResponse[loadList.IndexOf(loaded)].Relationship = loadList[loadList.IndexOf(loaded)].Result;
            //    loadList.Remove(loaded);
            //}

            await Task.WhenAll(loadList);
        }
        #endregion

        private void UserSearchResponse_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("UsersCount");
        }

        private void TagsSearchResponse_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("TagsCount");
        }

        private void MediaSearchResponse_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("MediaCount");
        }

        public void RasieAllCountsChanged()
        {
            RaisePropertyChanged("MediaCount");
            RaisePropertyChanged("TagsCount");
            RaisePropertyChanged("UsersCount");
        }

        public void AddMedia(InstaMedia im)
        {
            //if (MediaSearchResponse.Any(m => m.Id == im.Id)) return;
            InstaMedia exists = MediaSearchResponse.FirstOrDefault(m => m.Id == im.Id);
            if (exists == null)
            {
                im.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                MediaSearchResponse.Add(im);
            }
            else
            {
                exists.CloneAll(im);
                RaisePropertyChanged("MediaSearchResponse");
            }
        }

        public void AddTag(InstaTag ta)
        {
            //if (TagsSearchResponse.Any(t => t.Name == ta.Name)) return;
            InstaTag exists = TagsSearchResponse.FirstOrDefault(t => t.Name == ta.Name);
            if (exists == null)
            {
                ta.OnRaisedCommandToViewModel += TagResult_OnRaisedCommandToViewModel;
                TagsSearchResponse.Add(ta);
            }
            else
            {
                exists.CloneAll(ta);
                RaisePropertyChanged("TagsSearchResponse");
            }
        }

        public void AddUser(InstaUser ur)
        {
            // if (UserSearchResponse.Any(u => u.Id == ur.Id)) return;
            InstaUser exists = UserSearchResponse.FirstOrDefault(u => u.Id == ur.Id);
            if (exists == null)
            {
                ur.OnRaisedCommandToViewModel += UserResult_OnRaisedCommandToViewModel;
                UserSearchResponse.Add(ur);
            }
            else
            {
                exists.CloneAll(ur);
                RaisePropertyChanged("UserSearchResponse");
            }
        }

        #region InstaUser
        public async void UserResult_OnRaisedCommandToViewModel(InstaUser user, string command)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            if (InstaVM.Instance.InstaResponse == null)
            {
                "You need to log in first.".Show();
                return;
            }

            using (new WaitCursor())
            {
                try
                {
                    switch (command)
                    {
                        case "LOADMEDIA":
                            await LoadUsersMedia(user);
                            break;

                        case "FOLLOW":
                            await FolowUser(user);
                            break;

                        case "FOLLOWUn":
                            await UnFolowUser(user);
                            break;

                        case "FOLLOWAccept":
                            await AcceptFollowUser(user);
                            break;

                        case "FOLLOWBlock":
                            await BlockUser(user);
                            break;

                        case "FOLLOWUnBlock":
                            await UnBlockUser(user);
                            break;

                        case "LOADCounts":
                            await LoadUsersCounts(user);
                            break;

                        case "LOADRelation":
                            await LoadRelationshipWith(user);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.Show();
                }
            }
        }

        public async Task LoadUsersCounts(InstaUser user)
        {
            UserResponse response = await InstaVM.Instance.GetEndpointUsers().Get(Convert.ToString(user.Id));
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Counts = response.Data.Counts;
            user.RaisePropertyChanged("Counts");
        }

        public async Task UnBlockUser(InstaUser user)
        {
            RelationshipResponse response = await InstaVM.Instance.GetEndpointRelationships().Relationship(user.Id, Relationships.Action.Unblock);
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Relationship = response.Data;
        }

        public async Task BlockUser(InstaUser user)
        {
            RelationshipResponse response = await InstaVM.Instance.GetEndpointRelationships().Relationship(user.Id, Relationships.Action.Block);
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Relationship = response.Data;
        }

        public async Task AcceptFollowUser(InstaUser user)
        {
            RelationshipResponse response = await InstaVM.Instance.GetEndpointRelationships().Relationship(user.Id, Relationships.Action.Approve);
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Relationship = response.Data;
        }

        public async Task UnFolowUser(InstaUser user)
        {
            RelationshipResponse response = await InstaVM.Instance.GetEndpointRelationships().Relationship(user.Id, Relationships.Action.Unfollow);
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Relationship = response.Data;
        }

        public async Task FolowUser(InstaUser user)
        {
            RelationshipResponse response = await InstaVM.Instance.GetEndpointRelationships().Relationship(user.Id, Relationships.Action.Follow);
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Relationship = response.Data;
        }

        //private async Task LoadUsersFolowers(InstaUserResult user)
        //{
        //    UsersResponse response = await new Relationships(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).FollowedBy(user.Id);
        //    if (response.Data == null) return;
        //    foreach (var u in response.Data)
        //    {
        //        var ur = createInsaUserFromUser(u);
        //        user.Folowers.Add(ur);
        //    }
        //    user.RaisePropertyChanged("Folowers");
        //}

        //private async Task LoadUsersFolowing(InstaUserResult user)
        //{
        //    List<User> response = await new Relationships(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).FollowsAll(user.Id);
        //    if (response == null) return;
        //    foreach (var u in response)
        //    {
        //        var ur = createInsaUserFromUser(u);
        //        user.Folowing.Add(ur);
        //    }
        //    user.RaisePropertyChanged("Folowing");
        //}

        public async Task LoadUsersMedia(InstaUser user)
        {
            MediasResponse response = await InstaVM.Instance.GetEndpointUsers().Recent(Convert.ToString(user.Id));
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            foreach (var m in response.Data)
            {
                // if (user.MediaRecent.Any(mm =>mm.Id  == m.Id)) continue;
                InstaMedia exists = user.MediaRecent.FirstOrDefault(mm => mm.Id == m.Id);
                if(exists == null)
                {
                    InstaMedia im = new InstaMedia(m);
                    im.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                    user.MediaRecent.Add(im);
                }
                else
                {
                    exists.CloneAll(m);
                }
            }
            user.RaisePropertyChanged("MediaRecent");
        }

        public async Task<Relationship> LoadRelationshipWith(InstaUser user)
        {
            RelationshipResponse response = await InstaVM.Instance.GetEndpointRelationships().Relationship(user.Id);
            InstaVM.Instance.CheckShouldnotContinueRequest(user, response.Data, response);

            user.Relationship = response.Data;
            user.RaisePropertyChanged("Relationship");

            return user.Relationship;
        }
        #endregion

        #region InstaTag
        public async void TagResult_OnRaisedCommandToViewModel(InstaTag tag, string command)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            if (InstaVM.Instance.InstaResponse == null)
            {
                "You need to log in first.".Show();
                return;
            }

            using (new WaitCursor())
            {
                try
                {
                    switch (command)
                    {
                        case "LOADMEDIA":
                            await LoadTagsRecentMedia(tag);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.Show();
                }
            }
        }

        public async Task LoadTagsRecentMedia(InstaTag tag)
        {
            MediasResponse response = await InstaVM.Instance.GetEndpointTags().Recent(tag.Name);
            InstaVM.Instance.CheckShouldnotContinueRequest(tag, response.Data, response);

            foreach (var m in response.Data)
            {
                //if (tag.MediaRecent.Any(mm => mm.Id == m.Id)) continue;

                InstaMedia exists = tag.MediaRecent.FirstOrDefault(mm => mm.Id == m.Id);
                if (exists == null)
                {
                    InstaMedia im = new InstaMedia(m);
                    im.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                    tag.MediaRecent.Add(im);
                }
                else
                {
                    exists.CloneAll(m);
                }
            }

            tag.RaisePropertyChanged("MediaRecent");
        }
        #endregion

        #region InstaMedia
        public async void MediaResult_OnRaisedCommandToViewModel(InstaMedia media, string command)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            if (InstaVM.Instance.InstaResponse == null)
            {
                "You need to log in first.".Show();
                return;
            }

            using (new WaitCursor())
            {
                try
                {
                    switch (command)
                    {
                        case "ADDComment":
                            await AddComment(media);
                            break;

                        case "LOADComments":
                            await LoadComments(media);
                            break;

                        case "LIKE":
                            await LikeMedia(media, true);
                            break;

                        case "unLIKE":
                            await LikeMedia(media, false);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.Show();
                }
            }
        }

        public async Task LikeMedia(InstaMedia media, bool isLike)
        {
            if ((media.UserHasLiked == true && isLike) || (media.UserHasLiked == false && !isLike)) return;

            LikesResponse response = null;
            if (isLike && (media.UserHasLiked == false || media.UserHasLiked == null)) response = await InstaVM.Instance.GetEndpointLikes().Post(media.Id);
            else if (!isLike && (media.UserHasLiked == true || media.UserHasLiked == null)) response = await InstaVM.Instance.GetEndpointLikes().Delete(media.Id);
            InstaVM.Instance.CheckShouldnotContinueRequest(media, response, response);

            media.UserHasLiked = isLike;
            media.Likes.Count = isLike ? media.Likes.Count + 1 : media.Likes.Count - 1;

            if (media.Likes.Count < 0) media.Likes.Count = 0;

            media.RaisePropertyChanged("UserHasLiked");
            media.RaisePropertyChanged("Likes");
        }

        public async Task LoadComments(InstaMedia media)
        {
            CommentsResponse response = await InstaVM.Instance.GetEndpointComments().Get(media.Id);
            InstaVM.Instance.CheckShouldnotContinueRequest(media, response.Data, response);

            if (media.Comments == null) media.Comments = new InstaSharp.Models.Comments();

            foreach (var c in response.Data)
            {
                media.CommentsData.Add(c);
            }
            var data = media.CommentsData.OrderBy(c => c.CreatedTime).ToList();
            media.CommentsData.Clear();
            foreach (var c in data)
            {
                media.CommentsData.Add(c);
            }
            media.CommentCount = media.Comments.Count;
            media.RaisePropertyChanged("Comments");
            media.RaisePropertyChanged("CommentCount");
            media.RaisePropertyChanged("CommentsData");
            //media.RaisePropertyChanged("CommentsData");
            //media.SetCommentsLock();
            return;
        }

        public async Task AddComment(InstaMedia media)
        {
            if (media.Comments == null || media.Comments.Count == 0)
                await LoadComments(media);

            CommentResponse response = await InstaVM.Instance.GetEndpointComments().Post(media.Id, media.AddCommentText);
            InstaVM.Instance.CheckShouldnotContinueRequest(media, response.Data, response);

            if (media.Comments == null) media.Comments = new InstaSharp.Models.Comments();

            media.CommentsData.Add(response.Data);
            media.CommentCount += 1;
            media.RaisePropertyChanged("Comments");
            media.RaisePropertyChanged("CommentsData");
            media.RaisePropertyChanged("CommentCount");
            //media.SetCommentsLock();
        }
        #endregion
    }
}
