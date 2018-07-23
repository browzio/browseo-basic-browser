using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Facebook
{
    public class FacebookUserSummary
    {
        public User userResponse { get; set; }

        public DefaultDataResponse<Post> feedResponse { get; set; }

        public DefaultDataResponse<Album> albumsResponse { get; set; }
        public DefaultDataResponse<Photo> photosResponse { get; set; }
        public DefaultDataResponse<Video> videosResponse { get; set; }

        public DefaultDataResponse<live_videosData> live_videosResponse { get; set; }
        public DefaultDataResponse<live_videosData> videoBroadcastsResponse { get; set; }

        public DefaultDataResponse<Group> groupsResponse { get; set; }

        public DefaultDataResponse<Event> eventsResponse { get; set; }

        public DefaultDataResponse<likesData> likesResponse { get; set; }

        public DefaultDataResponse<Page> accountsOwenedResponse { get; set; }
        public DefaultDataResponse<Group> groupsOwnedResponse { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/user/
    //user_friends
    public class User
    {
        public string id { get; set; }
        public string name { get; set; }
        public request_with_summary friends { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/page/
    //manage_pages or pages_show_list
    //A Page access token can also be used to view those restricted fields. A Page access token is required to view any User information for any objects owned by a Page.
    //You need to be the Admin of the root page for child pages in order to read the global_brand_children edge for a page.
    public class Page : IHavePosts
    {
        //id
        //numeric string
        //Page ID.No access token is required to access this field
        //
        //Core
        //about
        //string
        //Information about the Page
        //
        //access_token
        //string
        //The Page's access token. Only returned if the User making the request has a role (other than Live Contributor) on the Page. If your business requires two-factor authentication, the User must also be authenticated
        //
        //
        //ad_campaign
        //AdCampaign
        //The Page's currently running promotion campaign
        //
        //
        //affiliation
        //string
        //Affiliation of this person.Applicable to Pages representing people
        //
        //app_id
        //id
        //App ID for app-owned Pages and app Pages
        //
        //app_links
        //AppLinks
        //AppLinks data associated with the Page's URL
        //
        //
        //artists_we_like
        //string
        //Artists the band likes.Applicable to Bands
        //
        //attire
        //string
        //Dress code of the business. Applicable to Restaurants or Nightlife.Can be one of Casual, Dressy or Unspecified
        //
        //awards
        //string
        //The awards information of the film.Applicable to Films
        //
        //band_interests
        //string
        //Band interests.Applicable to Bands
        //
        //band_members
        //string
        //Members of the band.Applicable to Bands
        //
        //best_page
        //Page
        //The best available Page on Facebook for the concept represented by this Page.The best available Page takes into account authenticity and the number of likes
        //
        //bio
        //string
        //Biography of the band.Applicable to Bands
        //
        //birthday
        //string
        //Birthday of this person.Applicable to Pages representing people
        //
        //booking_agent
        //string
        //Booking agent of the band. Applicable to Bands
        //
        //built
        //string
        //Year vehicle was built.Applicable to Vehicles
        //
        //business
        //The Business associated with this Page.Visible only with a page access token or a user access token that has admin rights on the page
        //
        //can_checkin
        //bool
        //Whether this page has checkin functionality enabled
        //
        //can_post
        //bool
        //Whether the current session user can post on this Page
        //
        //category
        //string
        //The Page's category. e.g. Product/Service, Computers/Technology
        //
        //
        //Core
        //category_list
        //list<PageCategory>
        //The Page's sub-categories
        //
        //
        //checkins
        //unsigned int32
        //Number of checkins at a place represented by a Page
        //
        //Core
        //company_overview
        //string
        //The company overview. Applicable to Companies
        //
        //connected_instagram_account
        //ShadowIGUser
        //Instagram account connected to page via page settings
        //
        //contact_address
        //MailingAddress
        //The mailing or contact address for this page.This field will be blank if the contact address is the same as the physical address
        //
        //context
        //OpenGraphContext
        //Social context for this Page
        //
        //copyright_attribution_insights
        //CopyrightAttributionInsights
        //Insight metrics that measures performance of copyright attribution.An example metric would be number of incremental followers from attribution
        //
        //copyright_whitelisted_ig_partners
        //list<string>
        //Whitelisted Instagram usernames that will not be reported in copyright match systems
        //
        //country_page_likes
        //unsigned int32
        //If this is a Page in a Global Pages hierarchy, the number of people who are being directed to this Page
        //
        //cover
        //CoverPhoto
        //Information about the page's cover photo
        //
        //
        //culinary_team
        //string
        //Culinary team of the business. Applicable to Restaurants or Nightlife
        //
        //current_location
        //string
        //Current location of the Page
        //
        //
        //description
        //string
        //The description of the Page
        //
        //
        //Core
        //description_html
        //string
        //The description of the Page in raw HTML
        //
        //
        //directed_by
        //string
        //The director of the film. Applicable to Films
        //
        //display_subtext
        //string
        //Subtext about the Page being viewed
        //
        //displayed_message_response_time
        //string
        //Page estimated message response time displayed to user
        //
        //emails
        //list<string>
        //The emails listed in the About section of a Page
        //
        //
        //engagement
        //Engagement
        //The social sentence and like count information for this Page.This is the same info used for the like button
        //
        //fan_count
        //unsigned int32
        //The number of users who like the Page. For Global Pages this is the count for all Pages across the brand
        //
        //featured_video
        //Video
        //Video featured by the Page
        //
        //
        //features
        //string
        //Features of the vehicle.Applicable to Vehicles
        //
        //food_styles
        //list<string>
        //The restaurant's food styles. Applicable to Restaurants
        //
        //
        //founded
        //string
        //When the company was founded. Applicable to Pages in the Company category
        //
        //general_info
        //string
        //General information provided by the Page
        //
        //general_manager
        //string
        //General manager of the business. Applicable to Restaurants or Nightlife
        //
        //genre
        //string
        //The genre of the film. Applicable to Films
        //
        //global_brand_page_name
        //string
        //The name of the Page with country codes appended for Global Pages. Only visible to the Page admin
        //
        //
        //global_brand_root_id
        //numeric string
        //This brand's global Root ID
        //
        //
        //has_added_app
        //bool
        //Indicates whether this Page has added the app making the query in a Page tab
        //
        //has_whatsapp_number
        //bool
        //has whatsapp number
        //
        //
        //hometown
        //string
        //Hometown of the band.Applicable to Bands
        //
        //hours
        //map<string, string>
        //Indicates a single range of opening hours for a day. Each day can have 2 different hours ranges.The keys in the map are in the form of {day}
        //        _{number
        //    }
        //    _{status
        //}. {day} should be the first 3 characters of the day of the week, {number} should be either 1 or 2 to allow for the two different hours ranges per day. {status} should be either open or close to delineate the start or end of a time range.An example would be mon_1_open with value 17:00 and mon_1_close with value 21:15 which would represent a single opening range of 5pm to 9:15pm on Mondays
        //
        //impressum
        //string
        //Legal information about the Page publishers
        //
        //influences
        //string
        //Influences on the band.Applicable to Bands
        //
        //instagram_business_account
        //ShadowIGUser
        //Instagram account linked to page during Instagram business conversion flow
        //
        //instant_articles_review_status
        //enum
        //Indicates the current Instant Articles review status for this page
        //
        //is_always_open
        //bool
        //Indicates whether this location is always open
        //
        //is_chain
        //bool
        //Indicates whether location is part of a chain
        //
        //is_community_page
        //bool
        //Indicates whether the Page is a community Page
        //
        //is_eligible_for_branded_content
        //bool
        //Indicates whether the page is eligible for the branded content tool
        //
        //is_messenger_bot_get_started_enabled
        //bool
        //Indicates whether the page is a Messenger Platform Bot with Get Started button enabled
        //
        //is_messenger_platform_bot
        //bool
        //Indicates whether the page is a Messenger Platform Bot
        //
        //is_owned
        //bool
        //Indicates whether page is owned
        //
        //is_permanently_closed
        //bool
        //Whether the business corresponding to this Page is permanently closed
        //
        //is_published
        //bool
        //Indicates whether the Page is published and visible to non-admins
        //
        //is_unclaimed
        //bool
        //Indicates whether the Page is unclaimed
        //
        //is_verified
        //bool
        //Deprecated, use "verification_status". Pages with a large number of followers can be manually verified by Facebook as [having an authentic identity] (https://www.facebook.com/help/196050490547892). This field indicates whether the page is verified by this process
        //
        //Deprecated
        //is_webhooks_subscribed
        //bool
        //Indicates whether the application is subscribed for real time updates from this page
        //
        //keywords
        //null
        //Deprecated.Returns null
        //
        //Deprecated
        //leadgen_form_preview_details
        //LeadGenFormPreviewDetails
        //The details needed to generate an accurate preview of a lead gen form
        //
        //leadgen_has_crm_integration
        //bool
        //Indicates whether this page hasApp subscribes page leads realtime update
        //
        //leadgen_has_fat_ping_crm_integration
        //bool
        //Indicates whether this pagehas App that subscribes page leads realtime update via fat ping
        //
        //leadgen_tos_acceptance_time
        //datetime
        //Indicates the time when the TOS for running LeadGen Ads on the page was accepted
        //
        //leadgen_tos_accepted
        //bool
        //Indicates whether a user has accepted the TOS for running LeadGen Ads on the Page
        //
        //leadgen_tos_accepting_user
        //User
        //Indicates the user who accepted the TOS for running LeadGen Ads on the page
        //
        //link
        //string
        //The Page's Facebook URL
        //
        //Core
        //location
        //Location
        //The location of this place.Applicable to all Places
        //
        //members
        //string
        //Members of this org.Applicable to Pages representing Team Orgs
        //
        //merchant_id
        //string
        //The instant workflow merchant id associated with the Page
        //
        //merchant_review_status
        //enum
        //Review status of the Page against FB commerce policies, this status decides whether the Page can use component flow
        //
        //messenger_ads_default_icebreakers
        //list<string>
        //The default ice breakers for a certain page
        //
        //messenger_ads_default_page_welcome_message
        //MessengerDestinationPageWelcomeMessage
        //The default page welcome message for Click to Messenger Ads
        //
        //messenger_ads_default_quick_replies
        //list<string>
        //The default quick replies for a certain page
        //
        //messenger_ads_quick_replies_type
        //enum
        //Indicates what type this page is and we will generate different sets of quick replies based on it
        //
        //mission
        //string
        //The company mission. Applicable to Companies
        //
        //mpg
        //string
        //MPG of the vehicle.Applicable to Vehicles
        //
        //name
        //string
        //The name of the Page
        //
        //CoreDefault
        //name_with_location_descriptor
        //string
        //The name of the Page with its location and/or global brand descriptor. Only visible to a page admin. Non-page admins will get the same value as name.
        //
        //network
        //string
        //The TV network for the TV show.Applicable to TV Shows
        //
        //new_like_count
        //unsigned int32
        //The number of people who have liked the Page, since the last login. Only visible to a page admin
        //
        //offer_eligible
        //bool
        //Offer eligibility status. Only visible to a page admin
        //
        //overall_star_rating
        //float
        //Overall page rating based on rating survey from users on a scale of 1-5. This value is normalized and is not guaranteed to be a strict average of user ratings.
        //
        //page_token
        //string
        //page token
        //
        //parent_page
        //Page
        //Parent Page for this Page
        //
        //parking
        //PageParking
        //Parking information.Applicable to Businesses and Places
        //
        //payment_options
        //PagePaymentOptions
        //Payment options accepted by the business.Applicable to Restaurants or Nightlife
        //
        //personal_info
        //string
        //Personal information.Applicable to Pages representing People
        //
        //personal_interests
        //string
        //Personal interests.Applicable to Pages representing People
        //
        //pharma_safety_info
        //string
        //Pharmacy safety information. Applicable to Pharmaceutical companies
        //
        //phone
        //string
        //Phone number provided by a Page
        //
        //place_type
        //enum
        //For places, the category of the place
        //
        //plot_outline
        //string
        //The plot outline of the film.Applicable to Films
        //
        //preferred_audience
        //Targeting
        //Group of tags describing the preferred audienceof ads created for the Page
        //
        //press_contact
        //string
        //Press contact information of the band.Applicable to Bands
        //
        //price_range
        //string
        //Price range of the business.Applicable to Restaurants or Nightlife. Can be one of $, $$, $$$, $$$$ or Unspecified
        //
        //produced_by
        //string
        //The productor of the film. Applicable to Films
        //
        //products
        //string
        //The products of this company.Applicable to Companies
        //
        //promotion_eligible
        //bool
        //Boosted posts eligibility status.Only visible to a page admin
        //
        //promotion_ineligible_reason
        //string
        //Reason for which boosted posts are not eligible. Only visible to a page admin
        //
        //public_transit
        //string
        //Public transit to the business. Applicable to Restaurants or Nightlife
        //
        //publisher_space
        //PublisherSpace
        //Publisher Space for the page.
        //
        //rating_count
        //unsigned int32
        //Number of ratings for the page (limited to ratings that are publicly accessible)
        //
        //recipient
        //numeric string
        //Messenger page scope id associated with page and a user using account_linking_token
        //
        //record_label
        //string
        //Record label of the band.Applicable to Bands
        //
        //release_date
        //string
        //The film's release date. Applicable to Films
        //
        //restaurant_services
        //PageRestaurantServices
        //Services the restaurant provides. Applicable to Restaurants
        //
        //restaurant_specialties
        //PageRestaurantSpecialties
        //The restaurant's specialties. Applicable to Restaurants
        //
        //schedule
        //string
        //The air schedule of the TV show. Applicable to TV Shows
        //
        //screenplay_by
        //string
        //The screenwriter of the film. Applicable to Films
        //
        //season
        //string
        //The season information of the TV Show. Applicable to TV Shows
        //
        //single_line_address
        //string
        //The page address, if any, in a simple single line format.
        //
        //starring
        //string
        //The cast of the film. Applicable to Films
        //
        //start_info
        //PageStartInfo
        //Information about when the entity represented by the Page was started
        //
        //store_code
        //string
        //Unique store code for this location Page
        //
        //store_location_descriptor
        //string
        //Location Page's store location descriptor
        //
        //store_number
        //unsigned int32
        //Unique store number for this location Page
        //
        //studio
        //string
        //The studio for the film production.Applicable to Films
        //
        //supports_instant_articles
        //bool
        //Indicates whether this Page supports Instant Articles
        //
        //talking_about_count
        //unsigned int32
        //The number of people talking about this Page
        //
        //unread_message_count
        //unsigned int32
        //Unread message count for the Page. Only visible to a page admin
        //
        //unread_notif_count
        //unsigned int32
        //Number of unread notifications.Only visible to a page admin
        //
        //unseen_message_count
        //unsigned int32
        //Unseen message count for the Page. Only visible to a page admin
        //
        //username
        //string
        //The alias of the Page. For example, for www.facebook.com/platform the username is 'platform'
        //
        //Core
        //verification_status
        //string
        //Showing whether this Page is verified and in what color e.g.blue verified, gray verified or not verified
        //
        //voip_info
        //VoipInfo
        //Voip info
        //
        //website
        //string
        //The URL of the Page's website
        //
        //were_here_count
        //unsigned int32
        //The number of visits to this Page's location. If the Page setting Show map, check-ins and star ratings on the Page (under Page Settings > Page Info > Address) is disabled, then this value will also be disabled
        //
        //whatsapp_number
        //string
        //whatsapp number
        //
        //written_by
        //string
        //The writer of the TV show.Applicable to TV Shows

        //created_time
        //datetime
        //Time the object liked the page
        //
        //Default
        public string id { get; set; }
        public string about { get; set; }
        public string access_token { get; set; }
        public string affiliation { get; set; }
        public string app_id { get; set; }
        public string artists_we_like { get; set; }
        public string attire { get; set; }
        public string awards { get; set; }
        public string band_interests { get; set; }
        public string band_members { get; set; }
        public string bio { get; set; }
        public string birthday { get; set; }
        public string booking_agent { get; set; }
        public string built { get; set; }
        public string category { get; set; }
        public string company_overview { get; set; }
        public string culinary_team { get; set; }
        public string current_location { get; set; }
        public string description { get; set; }
        public string description_html { get; set; }
        public string directed_by { get; set; }
        public string display_subtext { get; set; }
        public string displayed_message_response_time { get; set; }
        public string features { get; set; }
        public string founded { get; set; }
        public string general_info { get; set; }
        public string general_manager { get; set; }
        public string genre { get; set; }
        public string global_brand_root_id { get; set; }
        public string hometown { get; set; }
        public string impressum { get; set; }
        public string influences { get; set; }
        public string link { get; set; }
        public string members { get; set; }
        public string mission { get; set; }
        public string mpg { get; set; }
        public string name { get; set; }
        public string name_with_location_descriptor { get; set; }
        public string network { get; set; }
        public string page_token { get; set; }
        public string personal_info { get; set; }
        public string personal_interests { get; set; }
        public string pharma_safety_info { get; set; }
        public string phone { get; set; }
        public string plot_outline { get; set; }
        public string press_contact { get; set; }
        public string price_range { get; set; }
        public string produced_by { get; set; }
        public string products { get; set; }
        public string promotion_eligible { get; set; }
        public string promotion_ineligible_reason { get; set; }
        public string public_transit { get; set; }
        public string record_label { get; set; }
        public string release_date { get; set; }
        public string schedule { get; set; }
        public string screenplay_by { get; set; }
        public string season { get; set; }
        public string single_line_address { get; set; }
        public string starring { get; set; }
        public string store_location_descriptor { get; set; }
        public string studio { get; set; }
        public string username { get; set; }
        public string verification_status { get; set; }
        public string website { get; set; }
        public string whatsapp_number { get; set; }
        public string written_by { get; set; }

        public bool can_checkin { get; set; }
        public bool can_post { get; set; }
        public bool has_added_app { get; set; }
        public bool has_whatsapp_number { get; set; }
        public bool is_always_open { get; set; }
        public bool is_chain { get; set; }
        public bool is_community_page { get; set; }
        public bool is_eligible_for_branded_content { get; set; }
        public bool is_messenger_bot_get_started_enabled { get; set; }
        public bool is_messenger_platform_bot { get; set; }
        public bool is_owned { get; set; }
        public bool is_permanently_closed { get; set; }
        public bool is_published { get; set; }
        public bool is_unclaimed { get; set; }
        public bool is_webhooks_subscribed { get; set; }
        public bool leadgen_has_crm_integration { get; set; }
        public bool leadgen_has_fat_ping_crm_integration { get; set; }
        public bool offer_eligible { get; set; }
        public bool supports_instant_articles { get; set; }

        public int checkins { get; set; }
        public int country_page_likes { get; set; }
        public int fan_count { get; set; }
        public int new_like_count { get; set; }
        public int rating_count { get; set; }
        public int store_number { get; set; }
        public int talking_about_count { get; set; }
        public int unread_message_count { get; set; }
        public int unread_notif_count { get; set; }
        public int unseen_message_count { get; set; }
        public int were_here_count { get; set; }

        public float overall_star_rating { get; set; }

        public string[] perms { get; set; }

        public DateTime created_time { get; set; } //for Likes when was liked
        public DateTime leadgen_tos_acceptance_time { get; set; }

        public Video featured_video { get; set; }


        //public string store_code { get; set; } causes fail when no data
        //public string merchant_id { get; set; } causes fail when no data

        //public Page best_page { get; set; } Page Public Content Access
        //public Page parent_page { get; set; } Page Public Content Access

        //public string global_brand_page_name { get; set; } Only visible to the Page admin
        //public string business { get; set; } page access token
        //public string recipient { get; set; } Messenger page scope id associated with page and a user using account_linking_token

        //ad_campaign AdCampaign
        //app_links AppLinks
        //category_list list<PageCategory>
        //connected_instagram_account ShadowIGUser
        //contact_address MailingAddress
        //context OpenGraphContext
        //copyright_attribution_insights CopyrightAttributionInsights
        //copyright_whitelisted_ig_partners list<string>
        //cover CoverPhoto
        //emails list<string>
        //engagement Engagement
        //food_styles list<string>
        //hours map<string, string>
        //instagram_business_account ShadowIGUser
        //instant_articles_review_status enum
        //leadgen_form_preview_details LeadGenFormPreviewDetails
        //leadgen_tos_accepting_user User
        //location Location
        //merchant_review_status enum
        //messenger_ads_default_icebreakers list<string>
        //messenger_ads_default_page_welcome_message MessengerDestinationPageWelcomeMessage
        //messenger_ads_default_quick_replies list<string>
        //messenger_ads_quick_replies_type enum
        //parking PageParking
        //payment_options PagePaymentOptions
        //place_type enum
        //preferred_audience Targeting
        //publisher_space PublisherSpace
        //restaurant_services PageRestaurantServices
        //restaurant_specialties PageRestaurantSpecialties
        //start_info PageStartInfo
        //voip_info VoipInfo

        public DefaultDataResponse<Post> feed { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/v3.0/group
    //https://developers.facebook.com/docs/graph-api/reference/user/groups/
    //groups_access_member_info — Enables your app to receive member-related data on group content.
    //publish_to_group — Enables your app to post content into a group on behalf of a user.
    //user_managed_groups — Enables your app to read the Groups of which a person is an admin.
    public class Group : IHavePosts
    {
        //id
        //
        //The group ID
        //
        //string
        //
        //cover
        //
        //Information about the group's cover photo.
        //
        //CoverPhoto
        //
        //description
        //
        //A brief description of the group.
        //
        //string
        //
        //email
        //
        //The email address to upload content to the group.Only current members of the group can use this.
        //
        //string
        //
        //icon
        //
        //The URL for the group's icon.
        //
        //string
        //
        //member_request_count
        //
        //The number of pending member requests. Requires an access token of an Admin of the Group.
        //
        //int
        //
        //name
        //
        //The name of the group.
        //
        //string
        //
        //owner
        //
        //This field was deprecated on April 4th, 2018.
        //
        //User | Page
        //
        //parent
        //
        //The parent of this group, if it exists.
        //
        //Group | Page
        //
        //privacy
        //
        //The privacy setting of the Group. Possible values are CLOSED, OPEN, and SECRET. Requires an access token of an Admin of the Group.
        //
        //string
        //
        //updated_time
        //
        //The last time the Group was updated (includes changes Group properties, Posts, and Comments). Requires an access token of an Admin of the Group.
        //
        //datetime

        //administrator
        //bool
        //Shown if this person is an admin of the group.

        //bookmark_order
        //unsigned int32
        //This group's place in the list of bookmarks on this person's Facebook homepage.

        //unread
        //unsigned int32
        //A count of the number of unread posts in that group.

        public string id { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string privacy { get; set; }

        public bool administrator { get; set; }

        public int bookmark_order { get; set; }
        public int unread { get; set; }

        public DateTime created_time { get; set; }

        //Requires an access token of an Admin of the Group.
        public int member_count { get; set; }
        public int member_request_count { get; set; }
        public DateTime updated_time { get; set; }

        //cover CoverPhoto
        //parent Group | Page

        public DefaultDataResponse<Post> feed { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/event/
    // user_events
    public class Event
    {
        //id
        //numeric string
        //The event ID
        //
        //attending_count
        //int32
        //Number of people attending the event
        //
        //can_guests_invite
        //bool
        //Can guests invite friends.Requires an access token of an Admin of the Event
        //
        //category
        //string
        //The category of the event
        //
        //cover
        //CoverPhoto
        //Cover picture
        //
        //declined_count
        //int32
        //Number of people who declined the event
        //
        //description
        //string
        //Long-form description
        //
        //Default
        //discount_code_enabled
        //bool
        //Is discount code enabled for this event
        //
        //end_time
        //string
        //End time, if one has been set
        //
        //Default
        //event_times
        //list<ChildEvent>
        //Array of times of a multi-instance event
        //
        //Default
        //guest_list_enabled
        //bool
        //Can see guest list.Requires an access token of an Admin of the Event
        //
        //interested_count
        //int32
        //Number of people interested in the event
        //
        //is_canceled
        //bool
        //Whether or not the event has been marked as canceled
        //
        //is_draft
        //bool
        //Whether the event is in draft mode or published. Requires an access token of an Admin of the Event
        //
        //is_page_owned
        //bool
        //Whether the event is created by page or not
        //
        //maybe_count
        //int32
        //Number of people who maybe going to the event
        //
        //name
        //string
        //Event name
        //
        //Default
        //noreply_count
        //int32
        //Number of people who did not reply to the event
        //
        //owner
        //The profile that created the event
        //
        //parent_group
        //Group
        //The group the event belongs to
        //
        //place
        //Place
        //Event Place information
        //
        //Default
        //scheduled_publish_time
        //string
        //Time when event is scheduled to be published
        //
        //start_time
        //string
        //Start time
        //
        //Default
        //ticket_uri
        //string
        //The link users can visit to buy a ticket to this event
        //
        //ticket_uri_start_sales_time
        //string
        //Time when tickets go on sale
        //
        //ticketing_privacy_uri
        //string
        //URI to seller's privacy policy for ticket purchases
        //
        //ticketing_terms_uri
        //string
        //URI to seller's terms of service for ticket purchases
        //
        //timezone
        //enum
        //Timezone
        //
        //type
        //enum {private, public, group, community
        //    }
        //    The type of the event
        //
        //    updated_time
        //    datetime
        //Last update time(ISO 8601 formatted)

        public string id { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public string end_time { get; set; }
        public string name { get; set; }
        //public string scheduled_publish_time { get; set; }(#200) Permissions error
        public string start_time { get; set; }
        public string ticket_uri { get; set; }
        public string ticket_uri_start_sales_time { get; set; }
        public string ticketing_terms_uri { get; set; }
        public string timezone { get; set; }
        public string type { get; set; }

        //public bool can_guests_invite { get; set; }(#200) Permissions error
        public bool discount_code_enabled { get; set; }
        //public bool guest_list_enabled { get; set; }(#200) Permissions error
        //public bool is_canceled { get; set; }(#200) Permissions error
        //public bool is_draft { get; set; }(#200) Permissions error
        //public bool is_page_owned { get; set; }(#200) Permissions error

        public int attending_count { get; set; }
        public int declined_count { get; set; }
        public int interested_count { get; set; }
        public int maybe_count { get; set; }
        public int noreply_count { get; set; }

        public DateTime updated_time { get; set; }

        //public Group parent_group { get; set; }(#200) Permissions error

        //cover CoverPhoto
        //event_times list<ChildEvent>
        //owner
        //place Place
    }

    //https://developers.facebook.com/docs/graph-api/reference/v3.0/album
    //user_photos 
    public class Album
    {
        //id
        //
        //The album ID.
        //
        //string
        //
        //can_upload
        //
        //Whether the viewer can upload photos to this album.
        //
        //boolean
        //
        //count
        //
        //The approximate number of photos in the album. This is not necessarily an exact count
        //
        //int
        //
        //cover_photo
        //
        //The ID of the album's cover photo.
        //
        //Photo
        //
        //created_time
        //
        //The time the album was initially created.
        //
        //datetime
        //
        //description
        //
        //The description of the album.
        //
        //string
        //
        //event
        //
        //The event associated with this album.
        //
        //Event
        //
        //from
        //
        //The current user, if the current user created the album.
        //
        //User
        //
        //link
        //
        //A link to this album on Facebook.
        //
        //string
        //
        //location
        //
        //The textual location of the album.
        //
        //string
        //
        //name
        //
        //The title of the album.
        //
        //string
        //
        //place
        //
        //The place associated with this album.
        //
        //Page
        //
        //privacy
        //
        //The privacy settings for the album.
        //
        //string
        //
        //type
        //
        //The type of the album.
        //
        //enum{ app, cover, profile, mobile, wall, normal, album }
        //
        //updated_time
        //
        //The last time the album was updated.
        //
        //datetime
        public string id { get; set; }
        public bool can_upload { get; set; }
        public int count { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public string privacy { get; set; }
        public string caption { get; set; }

        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }

        //enum{ app, cover, profile, mobile, wall, normal, album }
        public string type { get; set; }

        //cover_photo Photo
        public Photo cover_photo { get; set; }

        //event Event
        //from User
        //place Page

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/photo
    // user_photos or user_posts
    public class Photo
    {
        //id
        //numeric string
        //The photo ID
        //
        //album
        //Album
        //The album this photo is in
        //
        //backdated_time
        //datetime
        //backdated_time
        //
        //backdated_time_granularity
        //enum
        //backdated_time_granularity
        //
        //can_backdate
        //bool
        //Indicates whether the viewer can backdate the photo
        //
        //can_delete
        //bool
        //Indicates whether the viewer can delete the photo
        //
        //can_tag
        //bool
        //Indicates whether the viewer can tag the photo
        //
        //created_time
        //datetime
        //The time this photo was published
        //
        //Default
        //event
        //Event
        //event
        //
        //from
        //User|Page
        //The profile(user or page) that uploaded this photo
        //
        //height
        //unsigned int32
        //The height of this photo in pixels
        //
        //icon
        //string
        //The icon that Facebook displays when photos are published to News Feed
        //
        //images
        //list<PlatformImageSource>
        //The different stored representations of the photo. Can vary in number based upon the size of the original photo.
        //
        //link
        //string
        //A link to the photo on Facebook
        //
        //name
        //string
        //The user-provided caption given to this photo.Corresponds to caption when creating photos
        //
        //Default
        //name_tags
        //list<EntityAtTextRange>
        //An array containing an array of objects mentioned in the name field which contain the id, name, and type of each object as well as the offset and length which can be used to match it up with its corresponding string in the name field
        //
        //page_story_id
        //string
        //ID of the page story this corresponds to. May not be on all photos. Applies only to published photos
        //
        //picture
        //string
        //Link to the 100px wide representation of this photo
        //
        //place
        //Place
        //Location associated with the photo, if any
        //
        //position
        //unsigned int32
        //Deprecated.Returns 0
        //
        //Deprecated
        //source
        //string
        //Deprecated. Use images instead
        //
        //Deprecated
        //target
        //Profile
        //The target this photo is published to
        //
        //updated_time
        //datetime
        //The last time the photo was updated
        //
        //webp_images
        //list<PlatformImageSource>
        //The different stored representations of the photo in webp format. Can vary in number based upon the size of the original photo.
        //
        //width
        //unsigned int32
        //The width of this photo in pixels

        public string id { get; set; }
        public string icon { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string page_story_id { get; set; }
        public string picture { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        public bool can_backdate { get; set; }
        public bool can_delete { get; set; }
        public bool can_tag { get; set; }

        public DateTime backdated_time { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }

        public Album album { get; set; }

        //backdated_time_granularity enum
        //event Event
        //from  User|Page
        //images list<PlatformImageSource>
        //name_tags list<EntityAtTextRange>
        //place Place
        //target Profile
        //webp_images list<PlatformImageSource>

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/video/
    //user_videos or user_posts
    public class Video
    {
        //ad_breaks
        //list<integer>
        //Time offsets of ad breaks in milliseconds.Ad breaks are short ads that play within a video.
        //
        //backdated_time
        //datetime
        //The time when the video post was created.
        //
        //backdated_time_granularity
        //enum
        //Accuracy of the backdated time.
        //
        //content_category
        //enum
        //The content category of this video.
        //
        //content_tags
        //list<numeric string>
        //Tags that describe the contents of the video.
        //
        //description
        //string
        //The description of the video.
        //
        //Default
        //id
        //numeric string
        //The video ID.
        //
        //created_time
        //datetime
        //The time the video was initially published.
        //
        //custom_labels
        //list<string>
        //Labels used to describe the video. Unlike content tags, custom labels are not published and only appear in insights data.
        //
        //embed_html
        //string
        //The HTML element that may be embedded in a Web page to play the video.
        //
        //embeddable
        //bool
        //Whether the video is embeddable.
        //
        //event
        //Event
        //event
        //
        //format
        //list<VideoFormat>
        //The different formats of the video.
        //
        //from
        //User|Page
        //The profile that created the video.
        //
        //icon
        //string
        //The icon that Facebook displays when videos are published to the feed.
        //
        //is_crosspost_video
        //bool
        //Whether the video is crossposted from other page.
        //
        //is_crossposting_eligible
        //bool
        //Specifies if the video is eligible for crossposting.Page access-token is required to query this field.
        //
        //is_episode
        //bool
        //Whether this video is episodie or not.
        //
        //is_instagram_eligible
        //bool
        //Whether the video is eligible to be promoted on Instagram
        //
        //is_reference_only
        //bool
        //Whether the video is exclusively used for copyright monitoring
        //
        //length
        //float
        //Duration of this video in seconds.
        //
        //live_status
        //enum
        //The live status of the video
        //
        //music_video_copyright
        //MusicVideoCopyright
        //The music video copyright of this video
        //
        //permalink_url
        //string
        //URL to the permalink page of the video.
        //
        //picture
        //string
        //The URL for the thumbnail picture of the video.
        //
        //place
        //Place
        //Location associated with the video, if any.
        //
        //privacy
        //Privacy
        //Privacy setting for the video.
        //
        //published
        //bool
        //Whether a post about this video is published.The post is not scheduled, draft, or ads_post.
        //
        //scheduled_publish_time
        //datetime
        //The time that the video is scheduled to publish.
        //
        //source
        //string
        //A URL to the raw, playable video file.
        //
        //status
        //VideoStatus
        //Object describing the status attributes of a video.
        //
        //title
        //string
        //The video title or caption.
        //
        //universal_video_id
        //string
        //The publishers asset management code for this video.
        //
        //updated_time
        //datetime
        //The last time the video or its caption was updated.
        //
        //Default

        public DateTime backdated_time { get; set; }
        public DateTime created_time { get; set; }
        public DateTime scheduled_publish_time { get; set; }
        public DateTime updated_time { get; set; }

        public string content_category { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string embed_html { get; set; }
        public string icon { get; set; }
        public string permalink_url { get; set; }
        public string picture { get; set; }
        public string source { get; set; }
        public string title { get; set; }
        public string universal_video_id { get; set; }

        public float length { get; set; }

        public bool is_crosspost_video { get; set; }
        public bool is_crossposting_eligible { get; set; }
        public bool is_episode { get; set; }
        public bool is_instagram_eligible { get; set; }
        public bool is_reference_only { get; set; }
        public bool published { get; set; }
        public bool embeddable { get; set; }

        //ad_breaks list<integer>
        //backdated_time_granularity enum
        //content_tags list<numeric string>
        //custom_labels list<string>
        //event Event
        //format list<VideoFormat>
        //from User|Page
        //live_status enum
        //music_video_copyright MusicVideoCopyright
        //place Place
        //privacy Privacy
        //status VideoStatus        

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }
    }

    //https://developers.facebook.com/docs/graph-api/reference/v3.0/post
    //user_posts
    public class Post
    {
        public string caption { get; set; }
        public string description { get; set; }
        public string message { get; set; }
        public string name { get; set; }

        public string id { get; set; }
        public string object_id { get; set; }
        public string parent_id { get; set; }

        public bool is_instagram_eligible { get; set; }
        public bool is_published { get; set; }

        //URL to the permalink page of the post.
        public string permalink_url { get; set; }
        //The link attached to this post.
        public string link { get; set; }
        //A URL to any Flash movie or video file attached to the post.
        public string source { get; set; }

        //URL to a full-sized version of the Photo published in the Post or scraped from a link in the Post. If the photo's largest dimension exceeds 720 pixels, it will be resized, with the largest dimension set to 720.
        public string full_picture { get; set; }
        //A link to an icon representing the type of this post.
        public string icon { get; set; }
        //URL to a resized version of the Photo published in the Post or scraped from a link in the Post. If the photo's largest dimension exceeds 130 pixels, it will be resized, with the largest dimension set to 130.
        public string picture { get; set; }

        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }

        //enum{mobile_status_update, created_note, added_photos, added_video, shared_story, created_group, created_event, wall_post, app_created_story, published_story, tagged_in_photo, approved_friend}
        public string status_type { get; set; }
        //enum{link, status, photo, video, offer}
        public string type { get; set; }

        public string[] with_tags { get; set; }

        public shares shares { get; set; }
        public from from { get; set; }

        public properties[] properties { get; set; }

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }
    }



    //TODO:expand

    //live_videos{id,status,title,permalink_url,live_views}
    //video_broadcasts{id,status,title,permalink_url,live_views}
    //https://developers.facebook.com/docs/graph-api/reference/live-video/
    //user_videos or user_posts
    public class live_videosData
    {
        public string id { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string permalink_url { get; set; }
        public int live_views { get; set; }

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }
    }

    //likes{name,id,created_time,talking_about_count,fan_count,link,overall_star_rating,rating_count,website,category}
    //https://developers.facebook.com/docs/graph-api/reference/user/likes/
    //user_likes
    public class likesData
    {
        public string created_time { get; set; }

        public string id { get; set; }
        public string name { get; set; }
        public string category { get; set; }

        public string link { get; set; }
        public string website { get; set; }

        public int fan_count { get; set; }
        public int talking_about_count { get; set; }
        public int rating_count { get; set; }

        public float overall_star_rating { get; set; }

        //public paging paging { get; set; }
    }

    //sharedposts{id,link,story,permalink_url,shares}
    public class sharedpostsData
    {
        public string id { get; set; }
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public string link { get; set; }
        public string story { get; set; }
        public string permalink_url { get; set; }

        public shares shares { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }

        public paging paging { get; set; }
    }
    
    #region custom responses

    public interface IHavePosts
    {
        string id { get; set; }
        DefaultDataResponse<Post> feed { get; set; }
    }

    public class DefaultDataResponse<T>
    {
        public T[] data { get; set; }

        public paging paging { get; set; }

        public request_with_summary summary { get; set; }
    }
    #endregion

    //name,id,friends
    //user_friends
    public class userResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public request_with_summary friends { get; set; }
    }

    //accounts{id,name,talking_about_count,fan_count,instagram_business_account,link,overall_star_rating,rating_count,unread_message_count,unread_notif_count,unseen_message_count,website,access_token,likes,category}
    //TODO: instagram_business_account
    public class accountsData 
    {
        public string id { get; set; }
        public string name { get; set; }
        public string talking_about_count { get; set; }
        public string fan_count { get; set; }
        //TODO: public string instagram_business_account { get; set; }
        public string link { get; set; }
        public string overall_star_rating { get; set; }
        public string rating_count { get; set; }
        public string unread_message_count { get; set; }
        public string unread_notif_count { get; set; }
        public string unseen_message_count { get; set; }
        public string website { get; set; }
        public string access_token { get; set; }
        public string category { get; set; }

        public DefaultDataResponse<likesData> likes { get; set; }

        public DefaultDataResponse<feedData> feed { get; set; }

        public paging paging { get; set; }
    }

    //groups{name,id,administrator,created_time,email,link,purpose,unread}
    public class groupsData 
    {
        public string id { get; set; }
        public string name { get; set; }
        public string administrator { get; set; }
        public string created_time { get; set; }
        public string email { get; set; }
        public string link { get; set; }
        public string purpose { get; set; }
        public string unread { get; set; }

        //owned groups only feilds
        public string member_count { get; set; }
        public string member_request_count { get; set; }

        public DefaultDataResponse<feedData> feed { get; set; }

        public paging paging { get; set; }
    }

    //events{id,name,attending_count,declined_count,end_time,interested_count,maybe_count,noreply_count,start_time,timezone,type,updated_time,rsvp_status,ticket_uri,place,cover}
    public class eventsData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string attending_count { get; set; }
        public string declined_count { get; set; }
        public string end_time { get; set; }
        public string interested_count { get; set; }
        public string maybe_count { get; set; }
        public string noreply_count { get; set; }
        public string start_time { get; set; }
        public string timezone { get; set; }
        public string type { get; set; }
        public string updated_time { get; set; }
        public string rsvp_status { get; set; }
        public string ticket_uri { get; set; }
        //TODO: public string place { get; set; }
        //TODO: public string cover { get; set; }

        public paging paging { get; set; }
    }

    //feed{id,name,created_time,updated_time,is_expired,is_hidden,is_live_audio,is_popular,is_published,link,permalink_url,scheduled_publish_time,shares,source,status_type,subscribed,target,timeline_visibility,type}
    //user_posts
    public class feedData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public string is_expired { get; set; }
        public string is_hidden { get; set; }
        public string is_live_audio { get; set; }
        public string is_popular { get; set; }
        public string is_published { get; set; }
        public string link { get; set; }
        public string permalink_url { get; set; }
        public string scheduled_publish_time { get; set; }
        public string source { get; set; }
        public string status_type { get; set; }
        public string subscribed { get; set; }
        public string timeline_visibility { get; set; }
        public string type { get; set; }


        public from from { get; set; }
        public properties[] properties { get; set; }

        public string message { get; set; }

        public target target { get; set; }

        public shares shares { get; set; }
        
        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }

        public paging paging { get; set; }
    }

    //albums{id,name,link,created_time,updated_time,count,video_count,photo_count}
    public class albumsData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public string count { get; set; }
        public string video_count { get; set; }
        public string photo_count { get; set; }

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }

        public paging paging { get; set; }
    }

    //videos{id,title,created_time,updated_time,content_category,permalink_url,published,scheduled_publish_time}
    public class videosData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public string content_category { get; set; }
        public string permalink_url { get; set; }
        public string published { get; set; }
        public string scheduled_publish_time { get; set; }

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }

        public paging paging { get; set; }
    }

    //photos{id,name,created_time,updated_time,link}
    public class photosData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public string link { get; set; }

        public DefaultDataResponse<sharedpostsData> sharedposts { get; set; }

        public request_with_summary likes { get; set; }
        public request_with_summary comments { get; set; }
        public request_with_summary reactions { get; set; }

        public paging paging { get; set; }
    }

    //likes.limit(0).summary(true),comments.limit(0).summary(true),reactions.limit(0).summary(true)
    public class request_with_summary
    {
        public summary summary { get; set; }
    }
    public class summary
    {
        public int total_count { get; set; }
    }

    public class shares
    {
        public int count { get; set; }
    }

    public class from
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class target
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class properties
    {
        public string name { get; set; }
        public string text { get; set; }
        public string href { get; set; }
    }

    public class like
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created_time { get; set; }

        public paging paging { get; set; }
    }



    //public class picture
    //{
    //    public int total_count { get; set; }

    //    public class data
    //    {
    //        public string height { get; set; }
    //        public string is_silhouette { get; set; }
    //        public string url { get; set; }
    //        public string width { get; set; }
    //    }
    //}

    public class paging
    {
        public string previous { get; set; }
        public string next { get; set; }
    }

    #region insights
    //InsightsResult
    public class InsightsResultBase<T>
    {
        public string id { get; set; }
        public string name { get; set; }
        public string period { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string end_time { get; set; }

        public T[] values { get; set; }
    }

    //Value
    public class InsightsValue
    {
        public int value { get; set; }
    }

    //Page Like Sources
    public class LikeSourceType
    {
        public int Ads { get; set; }
        [JsonProperty("News Feed")]
        public int NewsFeed { get; set; }
        [JsonProperty("Page Suggestions")]
        public int PageSuggestions { get; set; }
        [JsonProperty("Restored Likes from Reactivated Accounts")]
        public int RestoredLikesfromReactivatedAccounts { get; set; }
        public int Search { get; set; }
        [JsonProperty("Your Page")]
        public int YourPage { get; set; }
    }

    //page_negative_feedback_by_type
    public class NegativeFeedbackType
    {
        public int hide_clicks { get; set; }
        public int hide_all_clicks { get; set; }
        public int report_spam_clicks { get; set; }
        public int unlike_page_clicks { get; set; }
    }

    //page_positive_feedback_by_type
    public class PositiveFeedbackType
    {
        public int like { get; set; }
        public int comment { get; set; }
        public int link { get; set; }
        public int answer { get; set; }
        public int claim { get; set; }
        public int rsvp { get; set; }
    }

    //Page Story Types
    public class InsightsPageStoryType
    {
        public int checkin { get; set; }
        public int coupon { get; set; }
        [JsonProperty("event")]
        public int mEvent { get; set; }
        public int fan { get; set; }
        public int mention { get; set; }
        [JsonProperty("page post")]
        public int pagePost { get; set; }
        public int question { get; set; }
        [JsonProperty("user post")]
        public int userPost { get; set; }
        public int other { get; set; }
    }

    //TODO:Tab Types
    #endregion
}
