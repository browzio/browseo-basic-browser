using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prospector.Models
{
    public class DWQuerySelectableOptions
    {
        public string Type { get; set; }
        public string Tooltip { get; set; }

        public ObservableCollection<DWQueryOption> DWSelectedSiteOptions { get; set; }

        public ICommand OnCommandFromView { get; set; }
        public DWQuerySelectableOptions()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            DWSelectedSiteOptions = new ObservableCollection<DWQueryOption>();
        }

        private void OnCommandFromView_Raised(object obj)
        {
                            var so = new DWQueryOption();
                            so.OnCloseMe += So_OnCloseMe;
            switch (obj as string)
            {
               case "DWADD":
                    switch (Type)
                    {
                        case "Site":
                        case "Title":
                            so.Type = Type.ToLower();
                            break;

                        case "External Links":
                            so.Type = "external_links";
                            break;

                        //For Example: thread.title:Blockchain -thread.title:Bitcoin
                        case "With Thread Title":
                            so.Type = "thread.title";
                            break;

                        case "Without Thread Title":
                            so.Type = "-thread.title";
                            break;

                        case "Thread Section Title":
                            so.Type = "thread.section_title";
                            break;

                        case "Thread URL":
                            so.Type = "thread.url";
                            break;

                        default:
                            break;
                    }
                break;

                default:
                    break;
            }
                            DWSelectedSiteOptions.Add(so);
        }

        private void So_OnCloseMe(DWQueryOption obj)
        {
            DWSelectedSiteOptions.Remove(obj);
        }
    }
}
