using BrowseoFX_WPF.Core;
using Gecko;
using Gecko.DOM;
using Gecko.GUI;
using Gecko.Interfaces;
using Gecko.Interop;
using Gecko.Windows;
using Gecko.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrowseoFX_WPF.ViewModels
{
    public class BaseBrowserViewModel : IWebviewListenerService
    {
        public event Action OnMainWebviewLoaded;
        #region IWebviewListenerService
        public void BuiltWindowCore(GeckoXULWindow _widget)
        {
        }

        public void GloableBrowserDocumentLoaded()
        {
            BrowseoFXManager.Instance.PanelUIHandler.CreateDefaultWindowView(MainWebView);
            OnMainWebviewLoaded?.Invoke();
        }

        public void GloableViewLoadDone()
        {
            OnMainWebviewLoaded?.Invoke();
        }
        #endregion

        public WebView MainWebView { get; set; }
        public WebView StartMainWebView()
        {
            MainWebView = new WebView();
            MainWebView.WebviewListenerService = this;
            MainWebView.Loaded += MainWebView_Loaded;
            return MainWebView;
        }

        private void MainWebView_Loaded(object sender, RoutedEventArgs e)
        {
            OnMainWebviewLoaded?.Invoke();
        }

        private GeckoXULElement tabbrowser;
        /// <summary>
        /// <tabbrowser id="content"
        ///             flex="1" 
        ///             contenttooltip="aHTMLTooltip"
        ///             tabcontainer="tabbrowser-tabs"
        ///             contentcontextmenu="contentAreaContextMenu"
        ///             autocompletepopup="PopupAutoComplete"
        ///             selectmenulist="ContentSelectDropdown"
        ///             datetimepicker="DateTimePickerPanel"/>
        /// </summary>
        public GeckoXULElement Tabbrowser
        {
            get
            {
                if (tabbrowser == null) tabbrowser = MainWebView.Widget.View.Document.GetElementById("content") as GeckoXULElement;
                return tabbrowser;
            }
        }

        /// <summary>
        /// returns the selected browser xul element
        /// </summary>
        public GeckoXULElement SelectedBrowser
        {
            get
            {
                return GeckoNode.Create(Tabbrowser.GetProperty<nsIDOMNode>("selectedBrowser")) as GeckoXULElement;
            }
        }

        /// <summary>
        /// returns the selected browsers html contentDocument
        /// </summary>
        public GeckoDocument SelectedContentDocument
        {
            get
            {
                return GeckoDocument.Create(SelectedBrowser.GetProperty<nsIDOMDocument>("contentDocument"));
            }
        }

        public void SelectedTabNavigate(string query)
        {
            SelectedContentDocument.DefaultView.Content.Location.Replace(query);
        }

        public void StartDownload(string link)
        {

        }
    }
}
