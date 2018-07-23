using Gecko;
using Gecko.Interfaces;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.Browseo
{
    public class MacroPromptService : DefaultPromptService, nsIAuthPrompt, nsIPrompt
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

        public bool showAny = true;

        public MacroPromptService() : base()
        {
            OnErrorButton = ButtonState_YES;
            OnErrorContinue = ButtonState_YES;
            OnLoginRetry = ButtonState_YES;

            OnLoginUsername = OnLoginPass = "";
        }

        public override void Alert(string dialogTitle, string text)
        {
            if (!showAny) return;

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
                return base.ConfirmEx(dialogTitle, text, buttonFlags, button0Title, button1Title, button2Title, checkMsg, ref checkValue);
            }
        }

        //public override bool Prompt(string dialogTitle, string text, string passwordRealm, uint savePassword, string defaultText, ref string result)
        //{
        //    POS++;
        //    return base.Prompt(dialogTitle, text, passwordRealm, savePassword, defaultText, ref result);
        //}

        //public override bool Select(string dialogTitle, string text, uint count, IntPtr[] selectList, ref int outSelection)
        //{
        //    POS++;
        //    return base.Select(dialogTitle, text, count, selectList, ref outSelection);
        //}
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
