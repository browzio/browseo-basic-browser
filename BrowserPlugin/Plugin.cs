using BrowserAndFeatures;
using IKriv.WpfHost.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrowserPlugin
{
    public class Plugin : PluginBase
    {
        FeatureCallage fc;

        public override FrameworkElement CreateControl()
        {
            fc = new FeatureCallage();
            return fc;
        }

        public override void Dispose()
        {
            if (fc != null)
                fc.CloseAll();
            base.Dispose();
        }
    }
}
