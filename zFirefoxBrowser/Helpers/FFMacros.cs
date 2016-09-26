using Gecko;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zFirefoxBrowser.Helpers
{
    public class MacroFilePickerFactory : nsIFactory
    {
        MacroFilePicker instance;
        public MacroFilePickerFactory(MacroFilePicker _instance)
        {
            instance = _instance;
        }
        public IntPtr CreateInstance(nsISupports aOuter, ref Guid iid)
        {
            if (aOuter != null)
                Marshal.ThrowExceptionForHR(GeckoError.NS_ERROR_NO_AGGREGATION);

            IntPtr pvv;
            IntPtr pUnk = Marshal.GetIUnknownForObject(instance);
            try
            {
                Marshal.ThrowExceptionForHR(Marshal.QueryInterface(pUnk, ref iid, out pvv));
            }
            finally
            {
                Marshal.Release(pUnk);
            }
            return pvv;
        }

        public void LockFactory(bool @lock)
        {

        }
    }
    public class MacroFilePicker : nsIFilePicker
    {
        private static MacroFilePicker instatnce;
        public static MacroFilePicker Instance
        {
            get
            {
                if (instatnce == null)
                {
                    instatnce = new MacroFilePicker();
                }
                return instatnce;
            }
        }

        public nsIFilePicker fp { get; set; }
        public nsIFilePickerShownCallback aFilePickerShownCallback { get; set; }
        public string FilePath { get; set; }
       public  nsIDOMWindowUtils utils;

        //public bool RunningInMacroMode { get; set; }

        private MacroFilePicker()
        {
            //fp = Xpcom.GetService<nsIFilePicker>("@mozilla.org/filepicker;1");
            //var fc = Xpcom.QueryInterface<nsIFilePicker>(fp);
            //Marshal.ReleaseComObject(fp);

            //C:\Users\eli\Pictures\1.jpg
            fp = Xpcom.CreateInstance<nsIFilePicker>("@mozilla.org/filepicker;1");
        }
        //...
        public void AppendFilter(nsAStringBase title, nsAStringBase filter)
        {
            fp.AppendFilter(title, filter);
        }

        public void AppendFilters(int filterMask)
        {
            fp.AppendFilters(filterMask);
        }

        public bool GetAddToRecentDocsAttribute()
        {
            return fp.GetAddToRecentDocsAttribute();
        }

        public void GetDefaultExtensionAttribute(nsAStringBase aDefaultExtension)
        {
            fp.GetDefaultExtensionAttribute(aDefaultExtension);
        }

        public void GetDefaultStringAttribute(nsAStringBase aDefaultString)
        {
            fp.GetDefaultStringAttribute(aDefaultString);
        }

        public nsIFile GetDisplayDirectoryAttribute()
        {
            return fp.GetDisplayDirectoryAttribute();
        }

        public nsISupports GetDomfileAttribute()
        {
            // return fp.GetDomfileAttribute();

            //var nsfile = Xpcom.CreateInstance<nsILocalFile>("@mozilla.org/file/local;1");
            //nsfile.InitWithPath(new nsAString(FilePath));
            //var reply1 = (nsISupports)nsfile;

            //fp.SetDisplayDirectoryAttribute(nsfile);
            //var yo = fp.GetDomfileAttribute();

          //  return reply1;

            if (!MacroManger.AnyRunning)
            {
                return fp.GetDomfileAttribute();
            }
            else
            {
                //var chromeDir = (nsIFile)Xpcom.NewNativeLocalFile(FilePath);
                //var chromeFile = chromeDir.Clone();
                //var replydom1 = (nsISupports)chromeFile;
                //return replydom1;

                var reply = (nsIFile)Xpcom.NewNativeLocalFile(FilePath);
                if (utils != null) return utils.WrapDOMFile(reply);
                else
                {
                    var replydom = (nsISupports)reply;
                    return replydom;
                }
            }
        }

        public nsISimpleEnumerator GetDomfilesAttribute()
        {
            return fp.GetDomfilesAttribute();
        }

        public nsIFile GetFileAttribute()
        {
            return fp.GetFileAttribute();
        }

        public nsISimpleEnumerator GetFilesAttribute()
        {
            return fp.GetFilesAttribute();
        }

        public nsIURI GetFileURLAttribute()
        {
            return fp.GetFileURLAttribute();
        }

        public int GetFilterIndexAttribute()
        {
            return fp.GetFilterIndexAttribute();
        }

        public short GetModeAttribute()
        {
            return fp.GetModeAttribute();
        }

        public void Init(nsIDOMWindow parent, nsAStringBase title, short mode)
        {
            fp.Init(parent, title, mode);
        }

        public void Open(nsIFilePickerShownCallback aFilePickerShownCallback)
        {
            //fp.Open(aFilePickerShownCallback);
            // fp.Show();
            //this.Show();
            //aFilePickerShownCallback.Done(nsIFilePickerConsts.returnOK);
            this.aFilePickerShownCallback = aFilePickerShownCallback;
            if (!MacroManger.AnyRunning) fp.Open(aFilePickerShownCallback);
        }

        public void SetAddToRecentDocsAttribute(bool aAddToRecentDocs)
        {
            fp.SetAddToRecentDocsAttribute(aAddToRecentDocs);
        }

        public void SetDefaultExtensionAttribute(nsAStringBase aDefaultExtension)
        {
            fp.SetDefaultExtensionAttribute(aDefaultExtension);
        }

        public void SetDefaultStringAttribute(nsAStringBase aDefaultString)
        {
            fp.SetDefaultStringAttribute(aDefaultString);
        }

        public void SetDisplayDirectoryAttribute(nsIFile aDisplayDirectory)
        {
            fp.SetDisplayDirectoryAttribute(aDisplayDirectory);
        }

        public void SetFilterIndexAttribute(int aFilterIndex)
        {
            fp.SetFilterIndexAttribute(aFilterIndex);
        }

        public short Show()
        {
            //if (!MacroManger.AnyRunning) return fp.Show();
             return nsIFilePickerConsts.returnOK;
        }
    }

    public class DirectoryServiceProvider : nsIDirectoryServiceProvider
    {
        public static string FileToSet = "";

        public nsIFile GetFile(string prop, ref bool persistent)
        {
            if (FileToSet != "")
            {
                switch (prop)
                {
                    case "GetFile":
                        return (nsIFile)Xpcom.NewNativeLocalFile(FileToSet);

                    default:
                        Console.WriteLine("Gecko.Xpcom.DirectoryServiceProvider.GetFile: not implemented: " + prop);
                        return null;
                }
            }
            else
            {
                return null;
            }
        }
    }


    public class MacroPromptService : PromptService, nsIAuthPrompt, nsIPrompt
    {
        public const string ButtonState_YES = "YES";
        public const string ButtonState_OK = "OK";
        public const string ButtonState_NO = "NO";
        public const string ButtonState_CANCEL = "CANCEL";
        public string ButtonState { get; set; }

        public string CONTENT { get; set; }

        public int POS { get; set; }
        public int AtPos { get; set; }


        public string OnErrorButton { get; set; }
        public string OnErrorContinue { get; set; }

        public string OnLoginUsername { get; set; }
        public string OnLoginPass { get; set; }
        public string OnLoginRetry { get; set; }
        public int OnLoginDidRetryCount { get; set; }

        public MacroPromptService() : base()
        {
            OnErrorButton = ButtonState_YES;
            OnErrorContinue = ButtonState_YES;
            OnLoginRetry = ButtonState_YES;

            OnLoginUsername = OnLoginPass = "";
        }

        public override void Alert(string dialogTitle, string text)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                if (ButtonState == "OK") return;
                else base.Alert(dialogTitle, text);
            }
            else
            {
                base.Alert(dialogTitle, text);
            }
        }

        public override void AlertCheck(string dialogTitle, string text, string checkMsg, ref bool checkValue)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                if (ButtonState == "OK")
                {
                    checkValue = true;
                    return;
                }
                else base.AlertCheck(dialogTitle, text, checkMsg, ref checkValue);
            }
            else
            {
                base.AlertCheck(dialogTitle, text, checkMsg, ref checkValue);
            }
        }

        public override bool PromptUsernameAndPassword(string dialogTitle, string text, string passwordRealm, uint savePassword, ref string user, ref string pwd)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                if (OnLoginUsername == "" && OnLoginPass == "")
                    return base.PromptUsernameAndPassword(dialogTitle, text, passwordRealm, savePassword, ref user, ref pwd);
                else
                {
                    if (OnLoginDidRetryCount == 0 || (OnLoginDidRetryCount < 2 && OnLoginRetry.ToUpper() == ButtonState_YES))
                    {
                        OnLoginDidRetryCount++;
                        user = OnLoginUsername;
                        pwd = OnLoginPass;
                        return true;
                    }
                    else
                    {
                        return base.PromptUsernameAndPassword(dialogTitle, text, passwordRealm, savePassword, ref user, ref pwd);
                    }
                }
            }
            else
            {
                return base.PromptUsernameAndPassword(dialogTitle, text, passwordRealm, savePassword, ref user, ref pwd);
            }
        }

        public override bool Confirm(string dialogTitle, string text)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                if (POS == AtPos)
                {
                    switch (ButtonState)
                    {
                        case ButtonState_YES:
                        case ButtonState_OK:
                            return true;

                        case ButtonState_NO:
                        case ButtonState_CANCEL:
                            return false;

                        default:
                            break;
                    }
                }
                return base.Confirm(dialogTitle, text);
            }
            else
            {
                return base.Confirm(dialogTitle, text);
            }
        }

        public override bool ConfirmCheck(string dialogTitle, string text, string checkMsg, ref bool checkValue)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                return base.ConfirmCheck(dialogTitle, text, checkMsg, ref checkValue);
            }
            else
            {
                return base.ConfirmCheck(dialogTitle, text, checkMsg, ref checkValue);
            }
        }

        public override bool Prompt(string dialogTitle, string text, ref string value, string checkMsg, ref bool checkValue)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                if (POS == AtPos)
                {
                    value = CONTENT;
                    switch (ButtonState)
                    {
                        case ButtonState_YES:
                        case ButtonState_OK:
                            return true;

                        case ButtonState_NO:
                        case ButtonState_CANCEL:
                            return false;

                        default:
                            break;
                    }
                }
                return base.Prompt(dialogTitle, text, ref value, checkMsg, ref checkValue);
            }
            else
            {
                return base.Prompt(dialogTitle, text, ref value, checkMsg, ref checkValue);
            }
        }

        public override int ConfirmEx(string dialogTitle, string text, uint buttonFlags, string button0Title, string button1Title, string button2Title, string checkMsg, ref bool checkValue)
        {
            if (MacroManger.AnyRunning)
            {
                POS++;
                checkValue = (OnErrorButton.ToUpper() == ButtonState_YES);

                if (OnErrorContinue.ToUpper() == ButtonState_YES)
                    return 0;
                else
                    return 2;
            }
            else
            {
                return base.ConfirmEx( dialogTitle,  text,  buttonFlags,  button0Title,  button1Title,  button2Title,  checkMsg, ref  checkValue);
            }
        }

        public override bool Prompt(string dialogTitle, string text, string passwordRealm, uint savePassword, string defaultText, ref string result)
        {
            POS++;
            return base.Prompt(dialogTitle, text, passwordRealm, savePassword, defaultText, ref result);
        }

        public override bool Select(string dialogTitle, string text, uint count, IntPtr[] selectList, ref int outSelection)
        {
            POS++;
            return base.Select(dialogTitle, text, count, selectList, ref outSelection);
        }
        public override nsICancelable AsyncPromptAuth(nsIChannel aChannel, nsIAuthPromptCallback aCallback, nsISupports aContext, uint level, nsIAuthInformation authInfo)
        {
            POS++;
            return base.AsyncPromptAuth(aChannel, aCallback, aContext, level, authInfo);
        }

        public override bool PromptAuth(nsIChannel aChannel, uint level, nsIAuthInformation authInfo)
        {
            POS++;
            return base.PromptAuth(aChannel, level, authInfo);
        }

        public override bool PromptPassword(string dialogTitle, string text, ref string password, string checkMsg, ref bool checkValue)
        {
            POS++;
            return base.PromptPassword(dialogTitle, text, ref password, checkMsg, ref checkValue);
        }

        public override bool PromptPassword(string dialogTitle, string text, string passwordRealm, uint savePassword, ref string pwd)
        {
            POS++;
            return base.PromptPassword(dialogTitle, text, passwordRealm, savePassword, ref pwd);
        }

        public override bool PromptUsernameAndPassword(string dialogTitle, string text, ref string username, ref string password, string checkMsg, ref bool checkValue)
        {
            POS++;
            return base.PromptUsernameAndPassword(dialogTitle, text, ref username, ref password, checkMsg, ref checkValue);
        }
    }
}
