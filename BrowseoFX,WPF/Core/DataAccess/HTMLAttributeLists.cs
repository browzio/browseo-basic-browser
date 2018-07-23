using BrowseoFX_WPF.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.DataAccess
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
    /// </summary>
    public class HTMLElementsEventsLists : SingletonBase<HTMLElementsEventsLists>
    {
        /// <summary>
        /// HTMLElement Events	
        /// </summary>
        public List<string> HTMLElementEvents
        {
            get
            {
                return new List<string>(new string[] {
                     "abort",
                     "autocomplete",
                     "autocompleteerror",
                     "DOMContentLoaded",
                     "afterprint",
                     "afterscriptexecute",
                     "beforeprint",
                     "beforescriptexecute",
                     "beforeunload",
                     "blur",
                     "cancel",
                     "change",
                     "click",
                     "close",
                     "connect",
                     "contextmenu",
                     "error",
                     "focus",
                     "hashchange",
                     "input",
                     "invalid",
                     "languagechange",
                     "load",
                     "loadend",
                     "loadstart",
                     "message",
                     "offline",
                     "online",
                     "open",
                     "pagehide",
                     "pageshow",
                     "popstate",
                     "progress",
                     "readystatechange",
                     "reset",
                     "select",
                     "show",
                     "sort",
                     "storage",
                     "submit",
                     "toggle",
                     "unload",
                     "loadeddata",
                     "loadedmetadata",
                     "canplay",
                     "playing",
                     "play",
                     "canplaythrough",
                     "seeked",
                     "seeking",
                     "stalled",
                     "suspend",
                     "timeupdate",
                     "volumechange",
                     "waiting",
                     "durationchange",
                     "emptied",
                     "unhandledrejection",
                     "rejectionhandled",
                });
            }
        }
    
        /// <summary>
        /// HTMLFormElement Events	
        /// </summary>
        public List<string> HTMLFormElementEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "abort",
                    "autocomplete",
                    "autocompleteerror",
                    "DOMContentLoaded",
                    "afterprint",
                    "afterscriptexecute",
                    "beforeprint",
                    "beforescriptexecute",
                    "beforeunload",
                    "blur",
                    "cancel",
                    "change",
                    "click",
                    "close",
                    "connect",
                    "contextmenu",
                    "error",
                    "focus",
                    "hashchange",
                    "input",
                    "invalid",
                    "languagechange",
                    "load",
                    "loadend",
                    "loadstart",
                    "message",
                    "offline",
                    "online",
                    "open",
                    "pagehide",
                    "pageshow",
                    "popstate",
                    "progress",
                    "readystatechange",
                    "reset",
                    "select",
                    "show",
                    "sort",
                    "storage",
                    "submit",
                    "toggle",
                    "unload",
                    "loadeddata",
                    "loadedmetadata",
                    "canplay",
                    "playing",
                    "play",
                    "canplaythrough",
                    "seeked",
                    "seeking",
                    "stalled",
                    "suspend",
                    "timeupdate",
                    "volumechange",
                    "waiting",
                    "durationchange",
                    "emptied",
                    "unhandledrejection",
                    "rejectionhandled",
                });
            }
        }
    }

    public class HTMLEventAttributeLists : SingletonBase<HTMLEventAttributeLists>
    {
        /// <summary>
        /// Window Event Attributes	
        /// </summary>
        public List<string> WindowEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "onafterprint",
                    "onbeforeprint",
                    "onbeforeunload",
                    "onerror",
                    "onhashchange",
                    "onload",
                    "onmessage",
                    "onoffline",
                    "ononline",
                    "onpagehide",
                    "onpageshow",
                    "onpopstate",
                    "onresize",
                    "onstorage",
                    "onunload",
                });
            }
        }

        /// <summary>
        /// Form Event Attributes	
        /// </summary>
        public List<string> FormEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "onblur",
                    "onchange",
                    "oncontextmenu",
                    "onfocus",
                    "oninput",
                    "oninvalid",
                    "onreset",
                    "onsearch",
                    "onselect",
                    "onsubmit",
                });
            }
        }

        /// <summary>
        /// Keyboard Events Attributes	
        /// </summary>
        public List<string> KeyboardEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "onkeydown",
                    "onkeypress",
                    "onkeyup",
                });
            }
        }

        /// <summary>
        /// Mouse Events Attributes	
        /// </summary>
        public List<string> MouseEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "onclick",
                    "ondblclick",
                    "onmousedown",
                    "onmousemove",
                    "onmouseout",
                    "onmouseover",
                    "onmouseup",
                    "onmousewheel",
                    "onwheel",
                });
            }
        }

        /// <summary>
        /// Drag Events Attributes	
        /// </summary>
        public List<string> DragEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "ondrag",
                    "ondragend",
                    "ondragenter",
                    "ondragleave",
                    "ondragover",
                    "ondragstart",
                    "ondrop",
                    "onscroll",
                });
            }
        }

        /// <summary>
        /// Clipboard Events Attributes	
        /// </summary>
        public List<string> ClipboardEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "oncopy",
                    "oncut",
                    "onpaste",
                });
            }
        }

        /// <summary>
        /// Media Events
        /// Events triggered by medias like videos, images and audio(applies to all HTML elements, but is most common in media elements, like<audio>, <embed>, <img>, <object>, and<video>).
        /// </summary>
        public List<string> MediaEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "onabort",
                    "oncanplay",
                    "oncanplaythrough",
                    "oncuechange",
                    "ondurationchange",
                    "onemptied",
                    "onended",
                    "onerror",
                    "onloadeddata",
                    "onloadedmetadata",
                    "onloadstart",
                    "onpause",
                    "onplay",
                    "onplaying",
                    "onprogress",
                    "onratechange",
                    "onseeked",
                    "onseeking",
                    "onstalled",
                    "onsuspend",
                    "ontimeupdate",
                    "onvolumechange",
                    "onwaiting",
                });
            }
        }

        /// <summary>
        /// Misc Events Attributes	
        /// </summary>
        public List<string> MiscEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "onshow",
                    "ontoggle",
                });
            }
        }
    }

    public class HTMLTagAttributesLists : SingletonBase<HTMLTagAttributesLists>
    {

        /// <summary>
        /// HTML Global Attributes
        /// </summary>
        public List<string> HTMLGlobalAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "accesskey",
                    "class",
                    "contenteditable",
                    "contextmenu",
                    "data-*",
                    "dir",
                    "draggable",
                    "dropzone",
                    "hidden",
                    "id",
                    "lang",
                    "spellcheck",
                    "style",
                    "tabindex",
                    "title",
                    "translate",
                });
            }
        }

        /// <summary>
        /// Form Attributes
        /// </summary>
        public List<string> FormAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "accept",
                    "accept-charset",
                    "action",
                    "autocomplete",
                    "enctype",
                    "method",
                    "name",
                    "novalidate",
                    "target",
                });
            }
        }

        /// <summary>
        /// Input Attributes
        /// </summary>
        public List<string> InputAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "accept",
                    "align",
                    "alt",
                    "autocomplete",
                    "autofocus",
                    "checked",
                    "dirname",
                    "disabled",
                    "form",
                    "formaction",
                    "formenctype",
                    "formmethod",
                    "formnovalidate",
                    "formtarget",
                    "height",
                    "list",
                    "max",
                    "maxlength",
                    "min",
                    "multiple",
                    "name",
                    "pattern",
                    "placeholder",
                    "readonly",
                    "required",
                    "size",
                    "src",
                    "step",
                    "type",
                    "value",
                    "width",
                });
            }
        }
        /// <summary>
        /// Input Types
        /// </summary>
        public List<string> InputTypes
        {
            get
            {
                return new List<string>(new string[] {
                    "text",
                    "email",
                    "password",
                    "search",
                    "tel",
                    "url",
                });
            }
        }

        /// <summary>
        /// Textarea Attributes
        /// </summary>
        public List<string> TextareaAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "autofocus",
                    "cols",
                    "dirname",
                    "disabled",
                    "form",
                    "maxlength",
                    "name",
                    "placeholder",
                    "readonly",
                    "required",
                    "rows",
                    "wrap",
                });
            }
        }

        /// <summary>
        /// Button Attributes
        /// </summary>
        public List<string> ButtonAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "autofocus",
                    "disabled",
                    "form",
                    "formaction",
                    "formenctype",
                    "formmethod",
                    "formnovalidate",
                    "formtarget",
                    "name",
                    "type",
                    "value",
                });
            }
        }

        /// <summary>
        /// Select Attributes
        /// </summary>
        public List<string> SelectAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "autofocus",
                    "disabled",
                    "form",
                    "multiple",
                    "name",
                    "required",
                    "size",
                });
            }
        }

        /// <summary>
        /// Option Attributes
        /// </summary>
        public List<string> OptionAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "disabled",
                    "label",
                    "selected",
                    "value",
                });
            }
        }

        /// <summary>
        /// Optgroup Attributes
        /// </summary>
        public List<string> OptgroupAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "disabled",
                    "label",
                });
            }
        }

        /// <summary>
        /// Fieldset Attributes
        /// </summary>
        public List<string> FieldsetAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "disabled",
                    "form",
                    "name",
                });
            }
        }

        /// <summary>
        /// Label Attributes
        /// </summary>
        public List<string> LabelAttributes
        {
            get
            {
                return new List<string>(new string[] {
                    "for",
                    "form",
                });
            }
        }
    }

    //public class HTMLTagTypesLists : SingletonBase<HTMLTagAttributesLists>
    //{
    //    /// <summary>
    //    /// Input Types
    //    /// </summary>
    //    public List<string> InputTypes
    //    {
    //        get
    //        {
    //            return new List<string>(new string[] {
    //                "text",
    //                "email",
    //                "password",
    //                "search",
    //                "tel",
    //                "url",
    //            });
    //        }
    //    }
    //}
}