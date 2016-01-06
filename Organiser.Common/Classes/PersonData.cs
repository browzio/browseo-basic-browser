using System;
using System.Collections.ObjectModel;
using System.ComponentModel; 

namespace SocialOrganizer.Models
{
    [Serializable]
    public class PersonData : INotifyPropertyChanged
    {
        public PersonData()
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
        }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProjectName"));
                }
            }
        }

        private string profileName;
        public string ProfileName
        {
            get { return profileName; }
            set
            {
                profileName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProfileName"));
                }
            }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PhoneNumber"));
                }
            }
        }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));
                }
            }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LastName"));
                }
            }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Username"));
                }
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Email"));
                }
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Password"));
                }
            }
        }

        private string proxyIP;
        public string ProxyIP
        {
            get { return proxyIP; }
            set
            {
                proxyIP = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProxyIP"));
                }
            }
        }

        private string proxyPort;
        public string ProxyPort
        {
            get { return proxyPort; }
            set
            {
                proxyPort = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProxyPort"));
                }
            }
        }


        private string proxyUsername;
        public string ProxyUsername
        {
            get { return proxyUsername; }
            set
            {
                proxyUsername = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProxyUsername"));
                }
            }
        }

        private string proxyPassword;
        public string ProxyPassword
        {
            get { return proxyPassword; }
            set
            {
                proxyPassword = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProxyPassword"));
                }
            }
        }

        private int cmbSelectedIndexSex;
        public int CmbSelectedIndexSex
        {
            get { return cmbSelectedIndexSex; }
            set
            {
                cmbSelectedIndexSex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CmbSelectedIndexSex"));
                }
            }
        }


        private ObservableCollection<string> sexList;
        public ObservableCollection<string> SexList
        {
            get { return sexList; }
            set { sexList = value; }
        }

        private int cmbSelectedIndexDay;
        public int CmbSelectedIndexDay
        {
            get { return cmbSelectedIndexDay; }
            set
            {
                cmbSelectedIndexDay = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CmbSelectedIndexDay"));
                }
            }
        }

        private ObservableCollection<string> dayList;
        public ObservableCollection<string> DayList
        {
            get { return dayList; }
            set { dayList = value; }
        }

        private int cmbSelectedIndexMonth;
        public int CmbSelectedIndexMonth
        {
            get { return cmbSelectedIndexMonth; }
            set
            {
                cmbSelectedIndexMonth = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CmbSelectedIndexMonth"));
                }
            }
        }

        private ObservableCollection<string> monthList;
        public ObservableCollection<string> MonthList
        {
            get { return monthList; }
            set { monthList = value; }
        }

        private int birthdayYear;
        public int BirthdayYear
        {
            get { return birthdayYear; }
            set
            {
                birthdayYear = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BirthdayYear"));
                }
            }
        }

        //private string address;
        //public string Address
        //{
        //    get { return address; }
        //    set { address = value; }
        //}

        private string street;
        public string Street
        {
            get { return street; }
            set
            {
                street = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Street"));
                }
            }
        }

        private string city;
        public string City
        {
            get { return city; }
            set
            {
                city = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("City"));
                }
            }
        }

        private string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("State"));
                }
            }
        }

        private string zip;
        public string Zip
        {
            get { return zip; }
            set
            {
                zip = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Zip"));
                }
            }
        }

        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                country = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Country"));
                }
            }
        }

        private string notes;
        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Notes"));
                }
            }
        }

        private string webAddress;
        public string WebAddress
        {
            get { return webAddress; }
            set
            {
                webAddress = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("WebAddress"));
                }
            }
        }


        private string children;
        public string Children
        {
            get { return children; }
            set
            {
                children = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Children"));
                }
            }
        }

        private bool inVault;
        public bool InPBNVault
        {
            get { return inVault; }
            set
            {
                inVault = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("InPBNVault"));
                }
            }
        }

        private bool inMonney;
        public bool InMonney
        {
            get { return inMonney; }
            set
            {
                inMonney = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("InMonney"));
                }
            }
        }

        public int SIPBNType { get; set; }

        private ObservableCollection<string> pbnOptions;

        public ObservableCollection<string> PBNOptions
        {
            get { return pbnOptions; }
            set { pbnOptions = value; }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public string Dir { get; set; }
        public string FilePath { get; set; }
        public string ProjectDir { get; set; }
    }
} 
