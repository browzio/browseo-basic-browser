using BrowseoFX_WPF.Core.BrowserListeners.Base;
using BrowseoFX_WPF.Core.DataAccess;
using Gecko;
using Gecko.DOM;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.BrowserListeners
{
    public class ContextMenuListener : nsDOMEventListenerBase
    {
        public ContextMenuListenerStates State { get; set; }

        public ContextMenuListener(ContextMenuListenerStates state, GeckoXULElement xulElement, string eventName) :
            base(xulElement, eventName)
        {
            State = state;
        }

        public override void OnHandleGeckoDomEvent(GeckoDOMEventArgs args)
        {
            switch (args.Type)
            {
                case "command":
                    switch (State)
                    {
                        case ContextMenuListenerStates.Default:
                            break;
                        case ContextMenuListenerStates.menuitem_ToSocialEngager:
                            var url = BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.ActiveElement.ToString();
                            string sitename = url.Replace("http://", "");
                            sitename = sitename.Replace("https://", "");
                            sitename = sitename.Replace("www.", "");
                            if (sitename.Contains("."))
                            {
                                sitename = sitename.Remove(sitename.IndexOf("."));
                            }

                            BrowseoFXManager.Instance.RaiseOnSentForSeo(sitename, url);
                            break;
                        case ContextMenuListenerStates.menuitem_curaste:
                        case ContextMenuListenerStates.menuitem_curate:
                            var selected = BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DefaultView.Selection.ToString();
                           // var div = BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.CreateElement("div");
                           // div.AppendChild((selected));
                           // var divelm = div as GeckoHTMLElement;
                           //var html = divelm.InnerHtml;

                            if (State == ContextMenuListenerStates.menuitem_curate)
                            {
                               BrowseoFXManager.Instance.RaiseOnCurateToPBN(selected, BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.Uri);
                            }
                            else
                            {
                                string thecontent = "<blockquote>" + selected + "<br />";
                                thecontent += "<a href=\"" + BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.Uri + " \" > " + BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.Uri + " </a>";
                                thecontent += "</blockquote>";
                                MyFilesDatabase.SetClipboardText(thecontent);
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case "popupshown":
                    var xulConxtMenuElement = (args.Target.CastToGeckoElement() as GeckoXULElement);
                        foreach (var attr in xulConxtMenuElement.Attributes)
                        {

                        }
                    foreach (var n in xulConxtMenuElement.ChildNodes)
                    {
                        var xulNode = n as GeckoXULElement;
                        if (xulNode.GetAttribute("label").Contains("Open Link in"))
                        {
                            (BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("menuitem_ToSocialEngager") as GeckoXULElement).SetAttribute("hidden", xulNode.GetAttribute("hidden"));
                        }
                        else if (xulNode.GetAttribute("label").Contains("Selection"))
                        {
                            (BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("menuitem_curaste") as GeckoXULElement).SetAttribute("hidden", xulNode.GetAttribute("hidden"));
                            (BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("menuitem_curate") as GeckoXULElement).SetAttribute("hidden", xulNode.GetAttribute("hidden"));
                        }
                        //foreach (var nn in xulNode.ChildNodes)
                        //{
                        //    var xulNnn = n as GeckoXULElement;
                        //    Console.WriteLine(xulNnn.GetAttribute("label"));

                        //    if (xulNnn.GetAttribute("label").Contains("Open Link In"))
                        //    {

                        //    }
                        //}
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
