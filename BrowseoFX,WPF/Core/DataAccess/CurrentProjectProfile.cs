using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.DataAccess
{
    [Serializable]
    public class PersonDataTEST
    {
        public PersonDataTEST()
        {
            SexList = new ObservableCollection<string>();
            SexList.Add("MALE");
            SexList.Add("FEMALE");

            PBNOptions = new ObservableCollection<string>();
            PBNOptions.Add("Wordpress");
            PBNOptions.Add("Drupal");

            DayList = new ObservableCollection<string>();
            for (int i = 1; i < 32; i++)
            {
                DayList.Add("" + i);
            }
            MonthList = new ObservableCollection<string>();
            for (int i = 1; i < 13; i++)
            {
                MonthList.Add("" + i);
            }

            Profiles = new ObservableCollection<PersonDataTEST>();
            SelectedProfile = this;
        }
        
        public string ProjectName { get; set; }
        
        public string ProfileName { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public string ProxyIP { get; set; }
        
        public string ProxyPort { get; set; }
        
        public string ProxyUsername { get; set; }
        
        public string ProxyPassword { get; set; }

        private ObservableCollection<string> sexList;
        public ObservableCollection<string> SexList
        {
            get { return sexList; }
            set { sexList = value; }
        }
        public int CmbSelectedIndexSex { get; set; }

        private ObservableCollection<string> dayList;
        public ObservableCollection<string> DayList
        {
            get { return dayList; }
            set { dayList = value; }
        }
        public int CmbSelectedIndexDay { get; set; }

        private ObservableCollection<string> monthList;
        public ObservableCollection<string> MonthList
        {
            get { return monthList; }
            set { monthList = value; }
        }
        public int CmbSelectedIndexMonth { get; set; }
        

        public int BirthdayYear { get; set; }
        
        public string Street { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string Zip { get; set; }
        
        public string Country { get; set; }
        
        public string Notes { get; set; }
        
        public string WebAddress { get; set; }
        
        public string Children { get; set; }
        
        private ObservableCollection<string> pbnOptions;
        public ObservableCollection<string> PBNOptions
        {
            get { return pbnOptions; }
            set { pbnOptions = value; }
        }
        public int SIPBNType { get; set; }

        public bool InPBNVault { get; set; }

        public bool InMonney { get; set; }

        public string Dir { get; set; }
        public string FilePath { get; set; }
        public string ProjectDir { get; set; }
        public bool BIADefault { get; set; }

        public ObservableCollection<PersonDataTEST> Profiles { get; set; }
        public PersonDataTEST SelectedProfile { get; set; }
        public string ProfOrProjName
        {
            get
            {
                return ProfileName.IsNullOrSpace() ? ProjectName : ProfileName;
            }
        }

    }
}
