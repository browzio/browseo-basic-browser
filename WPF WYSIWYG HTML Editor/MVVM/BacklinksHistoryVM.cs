using Organiser.Common.Classes;
using Organiser.Common.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_WYSIWYG_HTML_Editor.Helpers;
using WPF_WYSIWYG_HTML_Editor.Models;

namespace WPF_WYSIWYG_HTML_Editor.MVVM
{
    public class BacklinksHistoryVM : ViewModelBase
    {
        public ICommand ContextMenuClick { get; set; }
        public ICommand AddNewClick { get; set; }
        public ICommand ViewChartClick { get; set; }

        private ObservableCollection<BacklinkHistoryLine> fullBaklinkHistoroy;
        public ObservableCollection<BacklinkHistoryLine> FullBaklinkHistoroy
        {
            get { return fullBaklinkHistoroy; }
            set { fullBaklinkHistoroy = value; }
        }

        private string backlinkText;
        public string BacklinkText
        {
            get { return backlinkText; }
            set
            {
                backlinkText = value;
                RaisePropertyChanged("BacklinkText");
            }
        }

        private string moneySite;
        public string MoneySite
        {
            get { return moneySite; }
            set
            {
                moneySite = value;
                RaisePropertyChanged("MoneySite");
            }
        }

        //Site
        private string site;
        public string Site
        {
            get { return site; }
            set
            {
                site = value;
                RaisePropertyChanged("Site");
            }
        }

        public int SIBacklinkHistory { get; set; }

        private PersonData thisProfData;

        public BacklinksHistoryVM()
        {
            ContextMenuClick = new RelayCommand(OnContextMenuClick);
            AddNewClick = new RelayCommand(OnAddNewClick);
            ViewChartClick = new RelayCommand(OnViewChartClick);

            FullBaklinkHistoroy = new ObservableCollection<BacklinkHistoryLine>();
        }


        public void FillHistoryList(PersonData profile)
        {
            string historyDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault", "BacklinksHistory", profile.ProjectName);
            if (!Directory.Exists(historyDir)) Directory.CreateDirectory(historyDir);

            string historyFile = Path.Combine(historyDir, "log.txt");
            if (!File.Exists(historyFile)) File.Create(historyFile);
            try
            {
                string[] fileLines = File.ReadAllLines(historyFile);
                foreach (var line in fileLines)
                {
                    string[] lineInfo = line.Split(',');
                    FullBaklinkHistoroy.Add(new BacklinkHistoryLine()
                    {
                        Site = lineInfo[0],
                        MoneySite = lineInfo[1],
                        BacklinkText = lineInfo[2]
                    });
                }
            }
            catch { }

            thisProfData = profile;
        }

        private void OnAddNewClick(object obj)
        {
            if (thisProfData == null) return;
            string param = obj as string;
            if (param == null) return;
            else
            {
                if(param == "Import")
                {
                    RssFeedsLinksMultiWindow mw = new RssFeedsLinksMultiWindow();
                    mw.Title = "One per line seperate with coma ex: Backlink,Money Site,Site";
                    mw.ShowDialog();
                    if (!mw.OKClicked) return;
                    try
                    {
                        string[] lines = mw.tbInputedText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            string[] linkData = line.Split(',');
                            FullBaklinkHistoroy.Add(new BacklinkHistoryLine()
                            {
                                Site = linkData[2],
                                MoneySite = linkData[1],
                                BacklinkText = linkData[0]
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to import backlink history. Reason: " +ex.Message);
                    }
                }
                else
                {
                    FullBaklinkHistoroy.Add(new BacklinkHistoryLine()
                    {
                        Site = Site,
                        MoneySite = MoneySite,
                        BacklinkText = BacklinkText
                    });
                }
            }
            saveAllFromList();
        }

        private void OnContextMenuClick(object obj)
        {
            switch ((string)obj)
            {
                case "Delete":
                    if (thisProfData == null) return;
                    FullBaklinkHistoroy.RemoveAt(SIBacklinkHistory);
                    saveAllFromList();
                    break;
                default:
                    break;
            }
        }

        private void OnViewChartClick(object obj)
        {
            try
            {
                WPFPieChart.Window1 win = new WPFPieChart.Window1();
                win.InitDataContext(FullBaklinkHistoroy.ToList());
                win.Show();
            }
            catch { }
        }

        private void saveAllFromList()
        {
            Task.Factory.StartNew(() =>
            {
                string historyDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault", "BacklinksHistory", thisProfData.ProjectName);
                if (!Directory.Exists(historyDir)) Directory.CreateDirectory(historyDir);

                string content = "";
                foreach (BacklinkHistoryLine line in FullBaklinkHistoroy)
                {
                    content += line.Site + ',' + line.MoneySite + ',' + line.BacklinkText + Environment.NewLine;
                }

                string historyFile = Path.Combine(historyDir, "log.txt");
                File.WriteAllText(historyFile, content);
            });
        }

        internal static void SaveLink(PersonData moneyProfile, string link, string text, string pbnSiteUrl)
        {
            string historyDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault", "BacklinksHistory", moneyProfile.ProjectName);
            if (!Directory.Exists(historyDir)) Directory.CreateDirectory(historyDir);

            string historyFile = Path.Combine(historyDir, "log.txt");

            File.AppendAllText(historyFile, pbnSiteUrl + ',' + link + ',' + text + Environment.NewLine);
        }
    }
}
