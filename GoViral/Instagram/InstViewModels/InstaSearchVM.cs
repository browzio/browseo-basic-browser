using GoViral.Instagram.InstModels;
using InstaSharp.Endpoints;
using InstaSharp.Models;
using InstaSharp.Models.Responses;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstViewModels
{
    public class InstaSearchVM : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<InstaUserResult> UserSearchResponse { get; set; }
        public ObservableCollection<Tag> TagsSearchResponse { get; set; }
        public ObservableCollection<InstaMedia> MediaSearchResponse { get; set; }

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

            UserSearchResponse = new ObservableCollection<InstaUserResult>();
            TagsSearchResponse = new ObservableCollection<Tag>();
            MediaSearchResponse = new ObservableCollection<InstaMedia>();

            CheckedSearchTags = CheckedSearchUsers = CheckedSearchMedia= true;
        }

        private void OnCommandFromView_Raised(object obj)
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
                    //if (KeyWords.Contains("#"))
                    //{
                    //    "No # Tags Allowed".Show();
                    //    return;
                    //}
                    if (KeyWords.IsNullOrEmpty())
                    {
                        "Enter Search Keyword".Show();
                        return;
                    }
                    string[] kws = KeyWords.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var k in kws)
                    {
                        if (k.IsNullOrEmpty()) continue;
                        string kw = k.Trim();
                        kw = kw.Replace(" ", "_");
                        kw = kw.Replace("#", "");
                        if (CheckedSearchUsers) SearchForUsers(kw);
                        if (CheckedSearchTags) SearchForTags(kw);
                        if (CheckedSearchMedia) SearchForMedia(kw);
                    }
                    break;

                case "CLEARUSERS":
                    UserSearchResponse.Clear();
                    break;

                case "CLEARTAGS":
                    TagsSearchResponse.Clear();
                    break;

                case "CLEARMedia":
                    MediaSearchResponse.Clear();
                    break;

                default:
                    break;
            }
        }

        private async void SearchForMedia(string kw)
        {
            MediasResponse searchMediaResponse = await new Tags(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Recent(kw);
            if (searchMediaResponse.Data == null) return;
            foreach (var m in searchMediaResponse.Data)
            {
                InstaMedia im = new InstaMedia(m);
                im.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                MediaSearchResponse.Add(im);
            }
        }

        private async void SearchForTags(string kw)
        {
            TagsResponse searchTagsResponse = await new Tags(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Search(kw);
            if (searchTagsResponse.Data == null) return;
            foreach (var t in searchTagsResponse.Data)
            {
                TagsSearchResponse.Add(t);
            }
        }

        private async void SearchForUsers(string kw)
        {
            UsersResponse searchRespnse = await new Users(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Search(kw, null);
            if (searchRespnse.Data == null) return;
            foreach (var u in searchRespnse.Data)
            {
                var ur = createInsaUserFromUser(u);
                getRelationshipWith(ur);
                UserSearchResponse.Add(ur);
            }
        }


        #region InstaUser
        private void UserResult_OnRaisedCommandToViewModel(InstaUserResult user, string command)
        {
            if (InstaVM.Instance.InstaResponse == null)
            {
                "You need to log in first.".Show();
                return;
            }

            switch (command)
            {
                case "LOADMEDIA":
                    LoadUsersMedia(user);
                    break;

                //case "LOADLIKEDMEDIA":
                //    LoadUsersLikedMedia(user);
                //    break;

                //case "LOADFolowing":
                //    LoadUsersFolowing(user);
                //    break;

                //case "LOADFollowers":
                //    LoadUsersFolowers(user);
                //    break;

                default:
                    break;
            }
        }

        //private async void LoadUsersFolowers(InstaUserResult user)
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

        //private async void LoadUsersFolowing(InstaUserResult user)
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

        private async void LoadUsersMedia(InstaUserResult user)
        {
            string id =  Convert.ToString(user.Id);
            MediasResponse response = await new Users(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Recent(id);
            if (response.Data == null) return;
            foreach (var m in response.Data)
            {
                InstaMedia im = new InstaMedia(m);
                im.OnRaisedCommandToViewModel += MediaResult_OnRaisedCommandToViewModel;
                user.MediaRecent.Add(im);
            }
            user.RaisePropertyChanged("MediaRecent");
        }

        private async void getRelationshipWith(InstaUserResult ur)
        {
            RelationshipResponse response = await new Relationships(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Relationship(ur.Id);
            if (ur == null || response == null || response.Meta == null || response.Data == null || response.Meta.Code.ToString() != "OK") return;

            ur.Relationship = response.Data;
        }

        private InstaUserResult createInsaUserFromUser(User u)
        {
            InstaUserResult ur = new InstaUserResult()
            {
                Bio = u.Bio,
                Counts = u.Counts,
                FullName = u.FullName,
                Id = u.Id,
                ProfilePicture = u.ProfilePicture,
                Username = u.Username,
                Website = u.Website,
            };
            ur.OnRaisedCommandToViewModel += UserResult_OnRaisedCommandToViewModel;

            return ur;
        }
        #endregion

        #region InstaMedia
        private void MediaResult_OnRaisedCommandToViewModel(InstaMedia media, string command)
        {
            if (InstaVM.Instance.InstaResponse == null)
            {
                "You need to log in first.".Show();
                return;
            }

            switch (command)
            {
                //case "GETShortcode":

                //    break;

                case "ADDComment":
                    AddComment(media);
                    break;

                case "LOADComments":
                    LoadComments(media);
                    break;

                case "LIKE":
                    LikeMedia(media,true);
                    break;

                case "unLIKE":
                    LikeMedia(media, false);
                    break;

                default:
                    break;
            }
        }

        private async void LikeMedia(InstaMedia media, bool isLike)
        {
            LikesResponse response = null;
            if (isLike)
            {
                 response = await new InstaSharp.Endpoints.Likes(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Post(media.Id);
            }
            else
            {
                response = await new InstaSharp.Endpoints.Likes(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Delete(media.Id);
            }
            if (response == null || response.Meta == null || response.Meta.Code.ToString() != "OK") return;

            media.UserHasLiked = isLike;
            media.RaisePropertyChanged("UserHasLiked");
        }

        private async void LoadComments(InstaMedia media)
        {
            CommentsResponse commentResponse = await new InstaSharp.Endpoints.Comments(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Get(media.Id);
            if (commentResponse == null || commentResponse.Data == null) return;

            if (media.Comments == null) media.Comments = new InstaSharp.Models.Comments();
            if (media.Comments.Data == null) media.Comments.Data = new List<Comment>();

            foreach (var c in commentResponse.Data)
            {
                media.Comments.Data.Add(c);
            }
            media.Comments.Data = media.Comments.Data.OrderBy(c => c.CreatedTime).ToList();
            media.RaisePropertyChanged("Comments");
        }

        private async void AddComment(InstaMedia media)
        {
            CommentResponse commentResponse = await new InstaSharp.Endpoints.Comments(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse).Post(media.Id, media.AddCommentText);
            if (commentResponse == null || commentResponse.Data == null) return;

            if (media.Comments == null) media.Comments = new InstaSharp.Models.Comments();
            if (media.Comments.Data == null) media.Comments.Data = new List<Comment>();

            media.Comments.Data.Add(commentResponse.Data);
            media.RaisePropertyChanged("Comments");
        }
        #endregion
    }
}
