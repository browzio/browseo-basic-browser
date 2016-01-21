using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Organiser.Common.Controlls
{
    public class ExpanderDoubleClickExpand : Expander
    {
        private static readonly DependencyPropertyKey IsMouseDoubleClickedPropertyKey = DependencyProperty.RegisterReadOnly(
          "IsMouseDoubleClicked",
          typeof(Boolean),
          typeof(ExpanderDoubleClickExpand),
          new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsMouseDoubleClickedProperty = IsMouseDoubleClickedPropertyKey.DependencyProperty;

        public Boolean IsMouseDoubleClicked
        {
            get { return (Boolean)GetValue(IsMouseDoubleClickedProperty); }
        }

        static ExpanderDoubleClickExpand()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderDoubleClickExpand), new FrameworkPropertyMetadata(typeof(ExpanderDoubleClickExpand)));
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentControl contentControl = base.GetTemplateChild("HeaderSite") as ContentControl;
            if (contentControl != null)
            {
                contentControl.AddHandler(ContentControl.MouseDoubleClickEvent, new MouseButtonEventHandler(ExpanderHeader_MouseDoubleClick), true);
            }
        }

        private void ExpanderHeader_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            base.SetValue(IsMouseDoubleClickedPropertyKey, !IsMouseDoubleClicked);
            if (IsMouseDoubleClicked)
            {
                //base.OnExpanded();
                IsExpanded = true;
            }
            else
            {
                //base.OnCollapsed();
                IsExpanded = false;
            }
        }
    }
}
