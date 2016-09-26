using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace IMacroMultyLayout.Models
{
    //[KnownType(typeof(Organiser.Common.Classes.RelayCommand))]
    //[DataContract(Namespace = "GoViral.Models")]
    //public class ProjectImported : ViewModelBase
    //{
    //    public event Action<ProjectImported, bool> OnCheckedFolder = delegate { };
    //    public event Action<ProjectImported, bool> OnClickedExpand = delegate { };

    //    [XmlIgnore]
    //    public ICommand OnCommandFromView { get; set; }

    //    public ProjectImported()
    //    {
    //        OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
    //        RaiseChecked = true;
    //    }

    //    private void OnCommandFromView_Raised(object obj)
    //    {
    //        string param = obj as string;
    //        if (param == null) return;

    //        switch (param)
    //        {
    //            case "ClickedExpand":
    //                if(AngleTransformImage == 0)
    //                {
    //                    AngleTransformImage = -180;
    //                    OnClickedExpand(this, true);
    //                }
    //                else
    //                {
    //                    AngleTransformImage = 0;
    //                    OnClickedExpand(this, false);
    //                }
    //                break;

    //            default:
    //                break;
    //        }
    //    }

    //    private string projectName;
    //    public string Name
    //    {
    //        get { return projectName; }
    //        set { projectName = value; RaisePropertyChanged("Name"); }
    //    }

    //    private bool isProjectChecked;
    //    public bool IsChecked
    //    {
    //        get { return isProjectChecked; }
    //        set
    //        {
    //            isProjectChecked = value;
    //            if (IsFolder && RaiseChecked)
    //            {
    //                OnCheckedFolder(this, value);
    //            }
    //            RaisePropertyChanged("IsChecked");
    //        }
    //    }
    //    public bool RaiseChecked { get; set; }

    //    private bool isFolder;
    //    public bool IsFolder
    //    {
    //        get { return isFolder; }
    //        set { isFolder = value; RaisePropertyChanged("IsFolder"); }
    //    }

    //    private Thickness tabMargin = new Thickness(5);
    //    public Thickness TabMargin
    //    {
    //        get { return tabMargin; }
    //        set { tabMargin = value; RaisePropertyChanged("TabMargin"); }
    //    }
    //    private double angleTransformImage;
    //    public double AngleTransformImage
    //    {
    //        get { return angleTransformImage; }
    //        set { angleTransformImage = value; RaisePropertyChanged("AngleTransformImage"); }
    //    }

    //    private Visibility projVisible = Visibility.Visible;
    //    public Visibility ProjVisible
    //    {
    //        get { return projVisible; }
    //        set { projVisible = value; RaisePropertyChanged("ProjVisible"); }
    //    }
    //    private Visibility visibleHasNext = Visibility.Collapsed;
    //    public Visibility VisibleHasNext
    //    {
    //        get { return visibleHasNext; }
    //        set { visibleHasNext = value; RaisePropertyChanged("VisibleHasNext"); }
    //    }


    //    public string FilePath { get; set; }    
    //}
}
