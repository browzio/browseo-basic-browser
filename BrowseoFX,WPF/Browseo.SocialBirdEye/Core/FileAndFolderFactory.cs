using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Twitter;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Windows;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Core
{
    public class FileAndFolderFactory
    {
        #region singleton
        private FileAndFolderFactory() { }
        private static FileAndFolderFactory instance;
        public static FileAndFolderFactory Instance
        {
            get
            {
                if (instance == null) instance = new FileAndFolderFactory();
                return instance;
            }
        }
        #endregion

        public string SaveDirectory { get { return Path.Combine(MyFilesDatabase.GetBaseDir(), "BirdsEyeKeys", GloableProfData.PData.ProjectName); } }

        public void OpenOrSaveApiKeys(IHaveApiKeys apisController)
        {
            var keysContext = new ApiKeysBase()
            {
                ClientID = apisController.ClientKeys.ClientID,
                ClientSecret = apisController.ClientKeys.ClientSecret,
                RedirectUrl = apisController.ClientKeys.RedirectUrl
            };
            ApiKeysSettingsWindow apiKeySettingsWindow = new ApiKeysSettingsWindow();
            apiKeySettingsWindow.DataContext = keysContext; //googleApisController.ClientKeys;
            apiKeySettingsWindow.ShowDialog();
            if (apiKeySettingsWindow.DialogResult == true)
            {
                apisController.ClientKeys.ClientID = keysContext.ClientID;
                apisController.ClientKeys.ClientSecret = keysContext.ClientSecret;
                apisController.ClientKeys.RedirectUrl = keysContext.RedirectUrl;
                Task.Run(() =>
                {
                    if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);

                    File.WriteAllLines(apisController.ClientKeys.SaveFileKeys, new string[] 
                    {
                        apisController.ClientKeys.ClientID.Trim(),
                        apisController.ClientKeys.ClientSecret.Trim(),
                        apisController.ClientKeys.RedirectUrl.Trim(),
                    });
                });
            }
        }

        public void OpenOrSaveApiKeys(TwitterApisController twitterController)
        {
            var keysContext = new TwitterApisClientKeys()
            {
                OauthConsumerKey = twitterController.ClientKeys.OauthConsumerKey,
                OauthConsumerSecret = twitterController.ClientKeys.OauthConsumerSecret,
                OauthToken = twitterController.ClientKeys.OauthToken,
                OauthTokenSecret = twitterController.ClientKeys.OauthTokenSecret,
                ScreenName = twitterController.ClientKeys.ScreenName,
            };
            ApiKeysSettingsWindow apiKeySettingsWindow = new ApiKeysSettingsWindow();
            apiKeySettingsWindow.DataContext = keysContext; 
            apiKeySettingsWindow.googleApisKeys.Visibility = System.Windows.Visibility.Collapsed;
            apiKeySettingsWindow.twitterApisKeys.Visibility = System.Windows.Visibility.Visible;
            apiKeySettingsWindow.ShowDialog();
            if (apiKeySettingsWindow.DialogResult == true)
            {
                twitterController.ClientKeys = keysContext;
                Task.Run(() =>
                {
                    if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);

                    File.WriteAllLines(twitterController.ClientKeys.SaveFile, new string[] 
                    {
                        keysContext.OauthToken,
                        keysContext.OauthTokenSecret,
                        keysContext.OauthConsumerKey,
                        keysContext.OauthConsumerSecret,
                        keysContext.ScreenName
                    });
                });
            }
        }

        internal void SaveTokenInfo(string tokenJsonResponse, IHaveApiKeys apisController)
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);

                File.WriteAllText(apisController.ClientKeys.SaveFileToken, tokenJsonResponse);
            });
        }

        public async void LoadKeys(IHaveApiKeys apisController)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(SaveDirectory)) return;

                ApiKeysBase apiKeys = apisController.ClientKeys;
                if (File.Exists(apiKeys.SaveFileKeys))
                {
                    var fileContents = File.ReadAllLines(apiKeys.SaveFileKeys);
                    if (fileContents.Length >= 3)
                    {
                        apiKeys.ClientID = fileContents[0];
                        apiKeys.ClientSecret = fileContents[1];
                        apiKeys.RedirectUrl = fileContents[2];
                    }
                }
                if (File.Exists(apiKeys.SaveFileToken))
                {
                    var fileContents = File.ReadAllText(apiKeys.SaveFileToken);
                    apisController.AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenInfoBase>(fileContents);
                }
            });
        }

        public void LoadKeys(TwitterApisController twitterController)
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(SaveDirectory)) return;
                if (!File.Exists(twitterController.ClientKeys.SaveFile)) return;

                var fileContents = File.ReadAllLines(twitterController.ClientKeys.SaveFile);
                if (fileContents.Length < 5) return;

                twitterController.ClientKeys.OauthToken = fileContents[0];
                twitterController.ClientKeys.OauthTokenSecret = fileContents[1];
                twitterController.ClientKeys.OauthConsumerKey = fileContents[2];
                twitterController.ClientKeys.OauthConsumerSecret = fileContents[3];
                twitterController.ClientKeys.ScreenName = fileContents[4];
            });
        }
    }
}
