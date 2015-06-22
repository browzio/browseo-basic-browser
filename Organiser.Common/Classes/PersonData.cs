using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SocialOrganizer.Models
{
    public class PersonData
    {
        public PersonData()
        {
            SexList = new ObservableCollection<string>();
            SexList.Add("MALE");
            SexList.Add("FEMALE");
            DayList = new ObservableCollection<string>();
            for (int i = 1; i < 32; i++)
            {
                DayList.Add("" + i);
            }
            MonthList = new ObservableCollection<string>();
            MonthList.Add("January");
            MonthList.Add("Febuary");
            MonthList.Add("March");
            MonthList.Add("April");
            MonthList.Add("May");
            MonthList.Add("June");
            MonthList.Add("July");
            MonthList.Add("Augest");
            MonthList.Add("September");
            MonthList.Add("October");
            MonthList.Add("November");
            MonthList.Add("December");
        }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        private string profileName;
        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string proxyIP;
        public string ProxyIP
        {
            get { return proxyIP; }
            set { proxyIP = value; }
        }

        private string proxyPort;
        public string ProxyPort
        {
            get { return proxyPort; }
            set { proxyPort = value; }
        }


        private string proxyUsername;
        public string ProxyUsername
        {
            get { return proxyUsername; }
            set { proxyUsername = value; }
        }

        private string proxyPassword;
        public string ProxyPassword
        {
            get { return proxyPassword; }
            set { proxyPassword = value; }
        }

        private int cmbSelectedIndexSex;
        public int CmbSelectedIndexSex
        {
            get { return cmbSelectedIndexSex; }
            set { cmbSelectedIndexSex = value; }
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
            set { cmbSelectedIndexDay = value; }
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
            set { cmbSelectedIndexMonth = value; }
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
            set { birthdayYear = value; }
        }

        private string street;
        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        private string state;
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        private string zip;
        public string Zip
        {
            get { return zip; }
            set { zip = value; }
        }

        private string country;
        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        private string webAddress;
        public string WebAddress
        {
            get { return webAddress; }
            set { webAddress = value; }
        }

        private string notes;
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
    }
}
