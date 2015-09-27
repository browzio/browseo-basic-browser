using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Organiser.Common.Classes;

namespace WPFPieChart
{
    public class AssetClass : INotifyPropertyChanged
    {
        private String myClass;

        public String Class
        {
            get { return myClass; }
            set {
                myClass = value;
                RaisePropertyChangeEvent("Class");
            }
        }

        private double fund;

        public double Anchor
        {
            get { return fund; }
            set {
                fund = value;
                RaisePropertyChangeEvent("Anchor");
            }
        }

        private double total;

        public double Total
        {
            get { return total; }
            set {
                total = value;
                RaisePropertyChangeEvent("Total");
            }
        }

        private double benchmark;

        public double Benchmark
        {
            get { return benchmark; }
            set {
                benchmark = value;
                RaisePropertyChangeEvent("Benchmark");
            }
        }

        private string pBNSite;
        public string PBNSite
        {
            get { return pBNSite; }
            set
            {
                pBNSite = value;
                RaisePropertyChangeEvent("PBNSite");
            }
        }

        private string moneySite;
        public string MoneySite
        {
            get { return moneySite; }
            set
            {
                moneySite = value;
                RaisePropertyChangeEvent("MoneySite");
            }
        }


        public static List<AssetClass> ConstructTestData(List<BacklinkHistoryLine> lineData)
        {
            List<AssetClass> assetClasses = new List<AssetClass>();

            foreach (BacklinkHistoryLine line in lineData)
            {
                AssetClass assetClass = new AssetClass();
                assetClass.Class = line.BacklinkText;
                assetClass.PBNSite = line.Site;
                assetClass.MoneySite = line.MoneySite;
                int totalLines = 0;
                foreach (BacklinkHistoryLine lineBacklink in lineData)
                {
                    if (lineBacklink.BacklinkText == line.BacklinkText && lineBacklink.MoneySite == line.MoneySite)
                        totalLines += 1;
                }

                assetClass.Anchor = totalLines;
                assetClass.Total = totalLines;
                assetClasses.Add(assetClass);
            }
            //assetClasses.Add(new AssetClass(){Class="Cash", Projects=1.56, Total=1.56, Benchmark=4.82});
            //assetClasses.Add(new AssetClass(){Class="Bonds", Projects=2.92, Total=2.92, Benchmark=17.91});
            //assetClasses.Add(new AssetClass(){Class="Real Estate", Projects=13.24, Total=2.40, Benchmark=0.04});
            //assetClasses.Add(new AssetClass(){Class="Foreign Currency", Projects=16.44, Total=16.44, Benchmark=8.05});
            //assetClasses.Add(new AssetClass(){Class="Stocks; Domestic", Projects=27.57, Total=27.57, Benchmark=38.24});
            //assetClasses.Add(new AssetClass(){Class="Stocks; Foreign", Projects=50.03, Total=50.03, Benchmark=30.93});

            return assetClasses;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged!=null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            
        }

        #endregion
    }
}
