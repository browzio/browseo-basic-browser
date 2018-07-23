using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Facebook;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers
{
    public static class FBRequests
    {
        public const string DefaultOgRequestUrl = "https://graph.facebook.com/v3.0/";

        public const string Endpoint_feed = "feed";
        public const string Endpoint_accounts = "accounts";
        public const string Endpoint_groups = "groups";
        public const string Endpoint_events = "events";
        public const string Endpoint_albums = "albums";
        public const string Endpoint_videos = "videos";
        public const string Endpoint_photos = "photos";
        public const string Endpoint_live_videos = "live_videos";
        public const string Endpoint_video_broadcasts = "video_broadcasts";
        public const string Endpoint_likes = "likes";
        public const string Endpoint_insights = "insights";

        public const string Get_User_Fields = "name,id,friends";
        
        //public const string Get_Accounts_Fields = "id,name,talking_about_count,fan_count,instagram_business_account,link,overall_star_rating,rating_count,unread_message_count,unread_notif_count,unseen_message_count,website,access_token,likes,category";
        public const string Get_Accounts_Fields = "id, about, access_token, affiliation, artists_we_like, attire, awards, band_interests, band_members, bio, birthday, booking_agent, built, category, company_overview, culinary_team, current_location, description, description_html, directed_by, display_subtext, displayed_message_response_time, features, founded, general_info, general_manager, genre, global_brand_root_id, hometown, impressum, influences, link, members, mission, mpg, name, name_with_location_descriptor, network, page_token, personal_info, personal_interests, pharma_safety_info, phone, plot_outline, press_contact, price_range, produced_by, products, promotion_eligible, promotion_ineligible_reason, public_transit, record_label, release_date, schedule, screenplay_by, season, single_line_address, starring, store_location_descriptor, studio, username, verification_status, website, whatsapp_number, written_by,can_checkin, can_post, has_added_app, has_whatsapp_number, is_always_open, is_chain, is_community_page, is_eligible_for_branded_content, is_messenger_bot_get_started_enabled, is_messenger_platform_bot, is_owned, is_permanently_closed, is_published, is_unclaimed, is_webhooks_subscribed, leadgen_has_crm_integration, leadgen_has_fat_ping_crm_integration, offer_eligible, supports_instant_articles,checkins, country_page_likes, fan_count, new_like_count, rating_count, store_number, talking_about_count, unread_message_count, unread_notif_count, unseen_message_count, were_here_count,overall_star_rating,leadgen_tos_acceptance_time,featured_video";
        public const string Get_Likes_Fields = "name,id,created_time,talking_about_count,fan_count,link,overall_star_rating,rating_count,website,category";

        public const string Get_Groups_Fields = "name,id,administrator,created_time,email,link,purpose,unread";
        public const string Get_GroupsOwned_Fields = "name,id,administrator,created_time,email,link,purpose,unread,member_count,member_request_count";

        public const string Get_Events_Fields = "id,name,attending_count,declined_count,end_time,interested_count,maybe_count,noreply_count,start_time,timezone,type,updated_time,rsvp_status,ticket_uri,place,cover";

        public const string Get_Feed_Fields = "id,name,created_time,updated_time,is_expired,is_hidden,is_live_audio,is_popular,is_published,link,permalink_url,scheduled_publish_time,shares,source,status_type,subscribed,target,timeline_visibility,type";
        public const string Get_Albums_Fields = "id,name,link,created_time,updated_time,count,video_count,photo_count";
        public const string Get_Videos_Fields = "id,title,created_time,updated_time,content_category,permalink_url,published,scheduled_publish_time";
        public const string Get_Photos_Fields = "id,name,created_time,updated_time,link";
        public const string Get_LiveVideos_Fields = "id,status,title,permalink_url,live_views";
        public const string Get_VideoBroadcasts_Fields = "id,status,title,permalink_url";

        public const string Get_Sharedposts_Fields = "sharedposts{id,link,created_time,updated_time,story,permalink_url,shares}";
        public const string Get_CountsSummary_Fields = "likes.limit(0).summary(true),comments.limit(0).summary(true),reactions.limit(0).summary(true)";

        public static string SummaryFields { get { return "," + Get_CountsSummary_Fields + "," + Get_Sharedposts_Fields; } }
    }

    public class FacebookApisController : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        public string AccessToken_FB { get; set; }
        public string AccessToken_Insta { get; set; }


        private FacebookUserSummary userSummary;
        public FacebookUserSummary UserSummary
        {
            get { return userSummary; }
            set { userSummary = value; NotifyOfPropertyChange(); }
        }

        public FacebookApisController()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            UserSummary = new FacebookUserSummary();
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            var param = obj as string;
            if (param == null) return;
            try
            {
                switch (param)
                {
                    case "SetApiKeys":
                        break;

                    case "OauthApi":
                        new AuthTokenActivator(this).SetOauthCode();
                        break;

                    case "AnalyzeAllData":
                        await AnalyzeAllData();
                        break;

                    case "AnalyzeAllInsitesData":
                        await AnalyzeAllInsitesData();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = "Facebook Apis Error " + param + " threw an Exception: " + ex.Message;

            }
        }

        private async Task AnalyzeAllInsitesData()
        {
            string url = "";

            url = buildRequestUrl("me/" + FBRequests.Endpoint_accounts, FBRequests.Get_Accounts_Fields);
            var accountsOwenedResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<accountsData>>(url);
            if(accountsOwenedResponse != null)
            {
                foreach (var account in accountsOwenedResponse.data)
                {
                    //TODO:remove 
                    if (account.id != "696763390481869") continue;
                    
                    //People talking about this
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        "page_content_activity", 
                        account.access_token);
                    var page_activity = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_content_activity_by_action_type",
                            "page_content_activity_by_action_type_unique"
                        }),
                        account.access_token);
                    var page_activity_by = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsPageStoryType>>>(url);

                    //Page Impressions
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[] 
                        {
                            "page_impressions",
                            "page_impressions_unique",
                            "page_impressions_paid",
                            "page_impressions_paid_unique",
                            "page_impressions_organic",
                            "page_impressions_organic_unique",
                            "page_impressions_viral",
                            "page_impressions_viral_unique",
                            "page_impressions_nonviral",
                            "page_impressions_nonviral_unique"
                        }),
                        account.access_token);
                    var page_impressions = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        "page_impressions_by_story_type",
                        account.access_token);
                    var page_impressions_by = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsPageStoryType>>>(url);

                    //Page Engagement
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[] 
                        {
                            "page_engaged_users",
                            "page_post_engagements",
                            "page_consumptions",
                            "page_consumptions_unique",
                            "page_places_checkin_total",
                            "page_places_checkin_total_unique",
                            "page_negative_feedback",
                            "page_negative_feedback_unique",
                            "page_fans_online_per_day"
                        }),
                        account.access_token);
                    var page_engagement = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_negative_feedback_by_type",
                            "page_negative_feedback_by_type_unique"
                        }),
                        account.access_token);
                    var page_engagement_neg = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<NegativeFeedbackType>>>(url);

                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_positive_feedback_by_type",
                            "page_positive_feedback_by_type_unique"
                        }),
                        account.access_token);
                    var page_engagement_pos = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<PositiveFeedbackType>>>(url);

                    //Page Reactions
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_actions_post_reactions_like_total",
                            "page_actions_post_reactions_love_total",
                            "page_actions_post_reactions_wow_total",
                            "page_actions_post_reactions_haha_total",
                            "page_actions_post_reactions_sorry_total",
                            "page_actions_post_reactions_anger_total"
                        }),
                        account.access_token);
                    var page_reactions = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    //Page CTA Clicks
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_total_actions",
                            "page_website_clicks_logged_in_unique",
                        }),
                        account.access_token);
                    var page_cta = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    //Page User Demographics
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_fans",
                            "page_fan_adds",
                            "page_fan_adds_unique",
                            "page_fan_removes",
                            "page_fan_removes_unique",
                            "page_actions_post_reactions_sorry_total",
                            "page_website_clicks_logged_in_unique"
                        }),
                        account.access_token);
                    var page_demographics = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);


                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_fans_by_like_source",
                            "page_fans_by_like_source_unique"
                        }),
                        account.access_token);
                    var page_demographics_like_source = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<LikeSourceType>>>(url);

                    //Page Content
                    //TODO:with reflection and the rest of unknown

                    //Page Views
                    //696763390481869/insights?pretty=0&since=1481815715&until=1489591715&metric=page_views_total
                    //696763390481869/insights?pretty=0&since=1529509688&until=1531612800&metric=page_views_total
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_views_total",
                            "page_views_logout",
                            "page_views_logged_in_total",
                            "page_views_logged_in_unique"
                        }),
                        account.access_token);
                    var page_views = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    //Page Video Views
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_video_views",
                            "page_video_views_paid",
                            "page_video_views_organic",
                            "page_video_views_autoplayed",
                            "page_video_views_click_to_play",
                            "page_video_views_unique",
                            "page_video_repeat_views",
                            "page_video_view_time"
                        }),
                        account.access_token);
                    var page_video_views = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    //Page Posts
                    url = buildInsitesRequestUrl(account.id + "/" + FBRequests.Endpoint_insights,
                        string.Join(",", new string[]
                        {
                            "page_posts_impressions",
                            "page_posts_impressions_unique",
                            "page_posts_impressions_paid",
                            "page_posts_impressions_paid_unique",
                            "page_posts_impressions_organic",
                            "page_posts_impressions_organic_unique",
                            "page_posts_served_impressions_organic_unique",
                            "page_posts_impressions_viral",
                            "page_posts_impressions_viral_unique",
                            "page_posts_impressions_nonviral",
                            "page_posts_impressions_nonviral_unique"
                        }),
                        account.access_token);
                    var page_posts_impressions = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<InsightsResultBase<InsightsValue>>>(url);

                    //Page Post Impressions
                    //post_impressions*
                    //post_impressions_unique*
                    //post_impressions_paid*
                    //post_impressions_paid_unique*
                    //post_impressions_fan*
                    //post_impressions_fan_unique*
                    //post_impressions_fan_paid*
                    //post_impressions_fan_paid_unique*
                    //post_impressions_organic*
                    //post_impressions_organic_unique*
                    //post_impressions_viral*
                    //post_impressions_viral_unique*
                    //post_impressions_nonviral*
                    //post_impressions_nonviral_unique*

                    //Page Post Engagement
                    //post_engaged_users*
                    //
                    //post_negative_feedback*
                    //post_negative_feedback_unique*
                    //
                    //post_engaged_fan
                    //
                    //post_clicks*
                    //post_clicks_unique*

                    //Page Post Reactions
                    //post_reactions_like_total
                    //post_reactions_love_total
                    //post_reactions_wow_total
                    //post_reactions_haha_total
                    //post_reactions_sorry_total
                    //post_reactions_anger_total
                    //post_reactions_by_type_total

                    //Page Video Posts
                }
            }
        }

        private string buildInsitesRequestUrl(string endpoint, string metrics, string page_acces_token, int since = 0, int until = 0)
        {
            string url = 
                FBRequests.DefaultOgRequestUrl +
                endpoint + 
                "?metric=" + metrics + 
                "&access_token=" + page_acces_token;

            if (since != 0 &&
                until != 0)
            {
                url =
                    FBRequests.DefaultOgRequestUrl +
                    endpoint +
                    "?metric=" + metrics +
                    "&since=" + since +
                    "&until=" + until +
                    "&access_token=" + page_acces_token;
            }
            else
            {
                int unixTimestamp_since = (int)(DateTime.UtcNow.AddDays(-7).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int unixTimestamp_until = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                url =
                    FBRequests.DefaultOgRequestUrl +
                    endpoint +
                    "?metric=" + metrics +
                    "&since=" + unixTimestamp_since +
                    "&until=" + unixTimestamp_until +
                    "&access_token=" + page_acces_token;
            }

            return url;
        }

        private async Task AnalyzeAllData()
        {
            string id = "me";
            string url = "";

            //userResponse
            url = buildRequestUrl(id, FBRequests.Get_User_Fields);
            var userResponse = await HttpRequestsBase.GetUrlRequest<User>(url);
            UserSummary.userResponse = userResponse;

            NotifyOfPropertyChange("UserSummary");

            //feedResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_feed,
                "caption",
                "description",
                "message",
                "name",
                "id",
                "object_id",
                "parent_id",
                "is_instagram_eligible",
                "is_published",
                "permalink_url",
                "link",
                "source",
                "full_picture",
                "icon",
                "picture",
                "created_time",
                "updated_time",
                "status_type",
                "type",
                "with_tags",
                "shares",
                "from",
                "properties" + 
                FBRequests.SummaryFields);
            var feedResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Post>>(url);
            UserSummary.feedResponse = feedResponse;

            NotifyOfPropertyChange("UserSummary");

            //albumsResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_albums,
                "id",
                "can_upload",
                "count", 
                "cover_photo", 
                "created_time",
                "description",
                "event", 
                "from", 
                "link", 
                "location",
                "name",
                "place", 
                "privacy",
                "type",
                "updated_time" + 
                FBRequests.SummaryFields);
            var albumsResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Models.Facebook.Album>>(url);
            UserSummary.albumsResponse = albumsResponse;

            NotifyOfPropertyChange("UserSummary");

            //photosResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_photos,
                "id",
                "can_upload",
                "count",
                "cover_photo",
                "created_time",
                "description",
                "event",
                "from",
                "link",
                "location",
                "name",
                "place",
                "privacy",
                "type",
                "updated_time" +
                FBRequests.SummaryFields);
            var photosResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Photo>>(url);
            UserSummary.photosResponse = photosResponse;

            NotifyOfPropertyChange("UserSummary");

            //videosResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_videos,
                "backdated_time",
                "created_time",
                "scheduled_publish_time",
                "updated_time",
                "content_category",
                "description",
                "id",
                "embed_html",
                "icon",
                "permalink_url",
                "picture",
                "source",
                "title",
                "universal_video_id",
                "length",
                "is_crosspost_video",
                "is_crossposting_eligible",
                "is_episode",
                "is_instagram_eligible",
                //"is_reference_only",Copyright API only opens to page users
                "published",
                "embeddable" +
                FBRequests.SummaryFields);
            var videosResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Video>>(url);
            UserSummary.videosResponse = videosResponse;

            NotifyOfPropertyChange("UserSummary");

            //live_videosResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_live_videos,
                FBRequests.Get_LiveVideos_Fields +
                FBRequests.SummaryFields);
            var live_videosResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<live_videosData>>(url);
            UserSummary.live_videosResponse = live_videosResponse;

            NotifyOfPropertyChange("UserSummary");

            //live_videoBroadcastsResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_video_broadcasts,
                FBRequests.Get_VideoBroadcasts_Fields + 
                FBRequests.SummaryFields);
            var videoBroadcastsResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<live_videosData>>(url);
            UserSummary.videoBroadcastsResponse = videoBroadcastsResponse;

            NotifyOfPropertyChange("UserSummary");

            //groupsResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_groups,
                "id",
                "description",
                "email",
                "icon",
                "name",
                "privacy",
                "administrator",
                "bookmark_order",
                "created_time",
                "unread");
            var groupsResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Group>>(url);
            UserSummary.groupsResponse = groupsResponse;

            NotifyOfPropertyChange("UserSummary");

            //eventsResponse
            //id,name,attending_count,declined_count,end_time,interested_count,maybe_count,noreply_count,start_time,timezone,type,updated_time,rsvp_status,ticket_uri,place,cover
            //,end_time,name,scheduled_publish_time,start_time,ticket_uri,ticket_uri_start_sales_time,ticketing_terms_uri,timezone,type,can_guests_invite,discount_code_enabled,guest_list_enabled,is_canceled,is_draft,is_page_owned,attending_count,declined_count,interested_count,maybe_count,noreply_count,updated_time,parent_group
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_events,
                "id",
                "description",
                "end_time",
                "name",
                "start_time",
                "ticket_uri",
                "ticket_uri_start_sales_time",
                "ticketing_terms_uri",
                "timezone",
                "type",
                "discount_code_enabled",
                "attending_count",
                "declined_count",
                "interested_count",
                "maybe_count",
                "noreply_count",
                "updated_time");
            var eventsResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Event>>(url);
            UserSummary.eventsResponse = eventsResponse;

            NotifyOfPropertyChange("UserSummary");

            //likesResponse TODO: expand me/likes?limit=100&summary=true
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_likes, 
                FBRequests.Get_Likes_Fields + "&summary=true");
            var likesResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<likesData>>(url);
            UserSummary.likesResponse = likesResponse;

            NotifyOfPropertyChange("UserSummary");

            //accountsOwenedResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_accounts,
                FBRequests.Get_Accounts_Fields + ",perms&summary=true");
            var accountsOwenedResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Page>>(url);
            await GetFeedResponses(accountsOwenedResponse);
            UserSummary.accountsOwenedResponse = accountsOwenedResponse;

            NotifyOfPropertyChange("UserSummary");

            //groupsOwnedResponse
            url = buildRequestUrl(id + "/" + FBRequests.Endpoint_groups, 
                "id",
                "description",
                "email",
                "icon",
                "name",
                "privacy",
                "administrator",
                "bookmark_order",
                "unread",
                "member_count",
                "member_request_count",
                "updated_time");
            var groupsOwnedResponse = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Group>>(url);
            await GetFeedResponses(groupsOwnedResponse);
            UserSummary.groupsOwnedResponse = groupsOwnedResponse;

            NotifyOfPropertyChange("UserSummary");
        }

        private async Task GetFeedResponses<T>(DefaultDataResponse<T> responses)
        {
            if (responses != null &&
                responses.data != null)
            {
                foreach (var response in responses.data)
                {
                    var haveFeed = response as IHavePosts;
                    
                    if(typeof(T) == typeof(Page))
                    {
                        var perms = (Convert.ChangeType(response, typeof(Page)) as Page).perms;
                        if (perms == null || !perms.Contains("ADMINISTER")) continue;
                    }

                    var url = buildRequestUrl(haveFeed.id + "/" + FBRequests.Endpoint_feed,
                        FBRequests.Get_Feed_Fields + FBRequests.SummaryFields);
                    haveFeed.feed = await HttpRequestsBase.GetUrlRequest<DefaultDataResponse<Post>>(url);
                }
            }
        }

        private string buildRequestUrl(string endpoint, params string[] fields)
        {
            string fieldsString = string.Join(",", fields);

            string url =
                FBRequests.DefaultOgRequestUrl +
                endpoint +
                "?fields=" + fieldsString +
                "&limit=100" +//+ limit +
                "&access_token=" + AccessToken_FB;

            return url;
        }
    }
}
