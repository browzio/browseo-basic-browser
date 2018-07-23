using Gecko;
using Gecko.DOM.HTML;
using Gecko.Interfaces;
using Gecko.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using Gecko.DOM;
using BrowseoFX_WPF.Core.DataAccess;
using BrowseoFX_WPF.Core.BrowserListeners;
using BrowseoFX_WPF.Core.BrowserListeners.Base;
using BrowseoFX_WPF.Core.Base;

namespace BrowseoFX_WPF.Core.Services
{
    public class FormAutoFillContracts
    {
        public const string Satchel_Inputlist_Autocomplete = "@mozilla.org/satchel/inputlist-autocomplete;1";
        public const string Satchel_Form_History_Startup = "@mozilla.org/satchel/form-history-startup;1";
        public const string Satchel_Form_Autocomplete = "@mozilla.org/satchel/form-autocomplete;1";
        public const string Satchel_Form_Fill_Controller = "@mozilla.org/satchel/form-fill-controller;1";
        public const string Satchel_Form_History = "@mozilla.org/satchel/form-history;1";
        
        public const string AutoComplete_Controller = "@mozilla.org/autocomplete/controller;1";
        public const string AutoComplete_Search = "@mozilla.org/autocomplete/search;1?name=autofill-profiles";
    }

    /// <summary>
    /// https://dxr.mozilla.org/mozilla-central/source/toolkit/components/satchel
    /// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input
    ///  https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#autofill
    ///  
    /// in js
    /// nsIFormAutoComplete nsFormAutoComplete.js
    /// nsIInputListAutoComplete
    /// formSubmitListener.js in satchel
    /// 
    /// in c++
    /// nsIFormFillController
    /// nsIAutoCompleteInput
    /// nsIAutoCompleteSearch
    /// nsIFormAutoCompleteObserver
    /// 
    /// in jsm
    /// nsIAutoCompletePopup
    /// nsIFormHistory2
    /// nsIAutoCompleteResult
    /// 
    /// 
    /// 
    /// nsIFormAutofillContentService
    /// nsIAutoCompleteController
    /// nsIAutoCompleteObserver
    /// nsIAutoCompleteSearchDescriptor
    /// nsIAutoCompleteSimpleResult
    /// 
    /// mozIPlacesAutoComplete
    /// 
    /// nsIDOMHTMLFormElement(some methods)
    /// nsIDOMHTMLInputElement(some methods)
    /// nsILoginManager(some methods)
    /// nsIAccessibleRole:nsIAccessibleRoleConsts.ROLE_AUTOCOMPLETE
    ///
    /// Tags forms contain
    /// <input>
    /// <textarea>
    /// <button>
    /// <select>
    /// <option>
    /// <optgroup>
    /// <fieldset>
    /// <label>
    /// 
    /// </summary>
    public class FormsManagerService
    {
        #region singletone
        private FormsManagerService() { }
        private static FormsManagerService instance;
        public static FormsManagerService Instance
        {
            get
            {
                if (instance == null) instance = new FormsManagerService();
                return instance;
            }
        }
        #endregion

        #region xpcom
        private FormFillController _formFillController;
        private InputListAutoComplete _inputListAutoComplete;
        private FormAutoComplete _formAutoComplete;
        private FormHistory _formHistory;

        public FormFillController FormFillController
        {
            get
            {
                if (_formFillController == null) _formFillController = new FormFillController();
                return _formFillController;
            }
        }
        public InputListAutoComplete InputListAutoComplete
        {
            get
            {
                if (_inputListAutoComplete == null) _inputListAutoComplete = new InputListAutoComplete();
                return _inputListAutoComplete;
            }
        }
        public FormAutoComplete FormAutoComplete
        {
            get
            {
                if (_formAutoComplete == null) _formAutoComplete = new FormAutoComplete();
                return _formAutoComplete;
            }
        }
        public FormHistory FormHistory
        {
            get
            {
                if (_formHistory == null) _formHistory = new FormHistory();
                return _formHistory;
            }
        }
        #endregion

        #region FormFillService

        public IEnumerable<GeckoHTMLFormElement> GetForms(GeckoDocument doc)
        {
            var formElements = doc.GetElementsByTagName("form");
            foreach (var geForm in formElements)
            {
                var form = geForm as GeckoHTMLFormElement;
                if (form == null) continue;
                yield return form;
            }
        }

        public IEnumerable<GeckoHTMLElement> GetFormsElements(GeckoHTMLFormElement form)
        {
            foreach (var geElement in form.Elements)
            {
                var input = geElement as GeckoHTMLElement;
                if (input == null) continue;
                yield return input;
            }
        }

        public IEnumerable<GeckoHTMLElement> GetFormsElements(GeckoDocument doc)
        {
            foreach (var form in GetForms(doc))
            {
                foreach (var input in GetFormsElements(form))
                {
                    yield return input;
                }
            }
        }

        public void AddFormsEventListeners(GeckoDocument contentDocument)
        {
            foreach (var form in GetForms(contentDocument))
            {
                FormElementEventListener.Instance.AddListener(form.Instance, HTMLElementsEventsLists.Instance.HTMLFormElementEvents); 

                foreach (var input in GetFormsElements(form))
                {
                    FormElementEventListener.Instance.AddListener(input.Instance, HTMLElementsEventsLists.Instance.HTMLElementEvents);
                }
            }
        }




















        /// <summary>
        /// need to find and display options for
        /// ProfOrProjName
        /// PhoneNumber
        /// FirstName
        /// LastName
        /// Username
        /// Email
        /// Password
        /// CmbSelectedIndexSex
        /// CmbSelectedIndexDay
        /// CmbSelectedIndexMonth
        /// BirthdayYear
        /// Street
        /// City
        /// State
        /// Zip
        /// Country
        /// WebAddress
        /// 
        /// Within forms can be 
        /// <input>
        /// <textarea>
        /// <button>
        /// <select>
        /// <option>
        /// <optgroup>
        /// <fieldset>
        /// <label>
        /// </summary>
        /// <param name="selectedContentDocument"></param>
        internal void CheckFormsAndDisplayOptions(GeckoDocument document)
        {
            var selectedProfile = BrowseoFXManager.Instance.Project;
            foreach (var form in GetForms(document))
            {
                foreach (var formElement in GetFormsElements(form))
                {
                    if (formElement.GetAttribute("type") == "hidden" || formElement.TagName.IsNullOrSpace()) continue;

                    var nodeNameToFind = "type";
                    var elementTagType = formElement.TagName.ToLower();
                    switch (elementTagType)
                    {
                        case "input":
                            nodeNameToFind = "type";
                            break;

                        case "textarea":
                            break;

                        case "button":
                            break;

                        case "select":
                            break;

                        case "option":
                            break;

                        case "optgroup":
                            break;

                        case "fieldset":
                            break;

                        case "label":
                            break;

                        default:
                            break;
                    }

                    var valToFill = "";
                    foreach (var attribute in formElement.Attributes)
                    {
                        if (attribute.NodeName == nodeNameToFind)
                        {
                            if (attribute.NodeValue == "email")
                            {
                                valToFill = selectedProfile.Email;
                            }
                            //else if (attribute.NodeValue == "password")
                            //{
                            //    valToFill = selectedProfile.Password;
                            //}
                        }

                        if (!valToFill.IsNullOrSpace())
                            break;
                    }

                    if (!valToFill.IsNullOrSpace())
                    {

                        formElement.Focus();
                        FormFillController.Instance.GetFocusedInputAttribute();
                        FormFillController.Instance.ShowPopup();
                    }
                }
            }
        }

        /// <summary>
        /// saves profiles information for the next use of the form
        /// </summary>
        /// <param name="selectedContentDocument"></param>
        public void SaveProjectsProfilesToFormHistory(GeckoDocument selectedContentDocument)
        {
            foreach (var form in GetForms(selectedContentDocument))
            {
                var formsAttributes = new Dictionary<string, string>();
                foreach (var att in HTMLTagAttributesLists.Instance.FormAttributes)
                {
                    var formsAtribValue = form.GetAttribute(att);
                    if (formsAtribValue.IsNullOrSpace()) continue;

                    formsAttributes.Add(att, formsAtribValue);
                }


                foreach (var formElement in GetFormsElements(form))
                {
                    var attribCheckList = HTMLTagAttributesLists.Instance.InputAttributes;

                    if (formElement is GeckoHTMLTextAreaElement)
                    {
                        attribCheckList = HTMLTagAttributesLists.Instance.TextareaAttributes;
                    }
                    else if (formElement is GeckoHTMLButtonElement)
                    {
                        attribCheckList = HTMLTagAttributesLists.Instance.ButtonAttributes;
                    }
                    else if (formElement is GeckoHTMLSelectElement)
                    {
                        attribCheckList = HTMLTagAttributesLists.Instance.SelectAttributes;
                    }
                    else if (formElement is GeckoHTMLOptGroupElement)
                    {
                        attribCheckList = HTMLTagAttributesLists.Instance.OptgroupAttributes;
                    }
                    else if (formElement is GeckoHTMLFieldSetElement)
                    {
                        attribCheckList = HTMLTagAttributesLists.Instance.FieldsetAttributes;
                    }
                    else if (formElement is GeckoHTMLLabelElement)
                    {
                        attribCheckList = HTMLTagAttributesLists.Instance.LabelAttributes;
                    }

                    var elementsAttributes = new Dictionary<string, string>();
                    foreach (var att in attribCheckList)
                    {
                        var elementsAtribValue = formElement.GetAttribute(att);
                        if (elementsAtribValue.IsNullOrSpace()) continue;

                        elementsAttributes.Add(att, elementsAtribValue);
                    }

                    if (!elementsAttributes.ContainsKey("type") || elementsAttributes["type"] == "hidden") continue;

                    foreach (var profile in BrowseoFXManager.Instance.Project.Profiles)
                    {

                    }
                }
            }

            foreach (var htmlElement in GetFormsElements(selectedContentDocument))
            {
                //FormHistory.AddEntry()

            }
        }

        internal void ReadAllForms(GeckoDocument contentDocument)
        {
            var formElements = contentDocument.GetElementsByTagName("form");
            foreach (var geForm in formElements)
            {
                var form = geForm as GeckoHTMLFormElement;
                if (form == null) continue;

                foreach (var geElement in form.Elements)
                {
                    var input = geElement as GeckoHTMLInputElement;
                    if (input == null) continue;

                    string attrValFor_class = input.GetAttribute("class"),
                           attrValFor_name = input.GetAttribute("name"),
                           attrValFor_id = input.GetAttribute("id"), 
                           attrValFor_type = input.GetAttribute("type");

                    bool added = false;
                    if (!attrValFor_type.IsNullOrSpace())
                    {
                        foreach (var profile in BrowseoFXManager.Instance.Project.Profiles)
                        {
                            switch (attrValFor_type)
                            {
                                case "email":
                                    added = FormHistory.AddedEntry(attrValFor_type, profile.Email);
                                    break;

                                case "password":
                                    added = FormHistory.AddedEntry(attrValFor_type, profile.Password);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    

                    //foreach (var attribute in input.Attributes)
                    //{
                    //    if (attribute == null || attribute.NodeName.IsNullOrSpace() || attribute.NodeValue.IsNullOrSpace()) continue;

                    //    //switch (attribute.NodeName)
                    //    //{
                    //    //    case "name":
                    //    //        attrValFor_name = attribute.NodeValue;
                    //    //        break;

                    //    //    case "id":
                    //    //        attrValFor_id = attribute.NodeValue;
                    //    //        break;
                    //    //    default:
                    //    //        break;
                    //    //}
                    //}
                    //var formHistoryName = input.Id.IsNullOrSpace() ? input.Name : input.Id;

                    //foreach (var attribute in InputContentAttributes)
                    //{
                    //    var attributeValue = input.GetAttribute(attribute);
                    //    if (attributeValue == null) continue;


                    //}
                    // var acResult = InputListAutoComplete.AutoCompleteSearch(input.Value, input);



                }
            }
        }
        #endregion

        #region FormsHistory
        internal void SaveProjectsProfilesToFormHistory()
        {
            foreach (var profile in BrowseoFXManager.Instance.Project.Profiles)
            {

            }
        }
        #endregion

        #region in .cpp
        //nsFormFillController.cpp\\\

        /// <summary>
        ///[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ///[return: System.Runtime.InteropServices.MarshalAs(UnmanagedType.Interface)]
        ///nsIDOMHTMLInputElement GetFocusedInputAttribute();
        ///
        ///[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        ///void AttachToBrowser([MarshalAs(UnmanagedType.Interface)] nsIDocShell docShell, [MarshalAs(UnmanagedType.Interface)] nsIAutoCompletePopup popup);
        ///
        ///[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        ///void DetachFromBrowser([MarshalAs(UnmanagedType.Interface)] nsIDocShell docShell);
        ///
        ///[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        ///void MarkAsLoginManagerField([MarshalAs(UnmanagedType.Interface)] nsIDOMHTMLInputElement aInput);
        ///
        ///[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        ///void ShowPopup();
        /// </summary>


        public ComObject<nsIAutoCompleteInput> AutoCompleteInput
        {
            get
            {
                return new ComObject<nsIAutoCompleteInput>(FormFillController.Instance.GetFocusedInputAttribute().AsComObject().QueryInterface<nsIAutoCompleteInput>());
                //return Xpcom.GetService2<nsIAutoCompleteInput>(Guid("b068e70f-f82c-4c12-ad87-82e271c5c180"));
            }
        }

        public ComObject<nsIAutoCompleteSearch> AutoCompleteSearch
        {
            get
            {
                return Xpcom.GetService2<nsIAutoCompleteSearch>(FormAutoFillContracts.AutoComplete_Search);
            }
        }

        public ComObject<nsIFormAutoCompleteObserver> FormAutoCompleteObserver
        {
            get
            {
                return Xpcom.GetService2<nsIFormAutoCompleteObserver>(new Guid("604419ab-55a0-4831-9eca-1b9e67cc4751"));
            }
        }

       

        #endregion

        #region in .jsm

        public ComObject<nsIAutoCompletePopup> AutoCompletePopup
        {
            get
            {
                return Xpcom.GetService2<nsIAutoCompletePopup>(new Guid("bd3c2662-a988-41ab-8c94-c15ed0e6ac7d"));
                //or
                //return Xpcom.CreateInstance2<nsIAutoCompletePopup>()
            }
        }

        public ComObject<nsIAutoCompleteResult> FormAutoCompleteResult
        {
            get
            {
                return Xpcom.GetService2<nsIAutoCompleteResult>(new Guid("5d7d84d1-9798-4016-bf61-a32acf09b29d"));
            }
        }


        #endregion
    }


    #region event listeners
    public class FormElementEventListener : BasicEventListenerBase
    {
        #region singletone
        private FormElementEventListener() { }
        private static FormElementEventListener instance;
        public static FormElementEventListener Instance
        {
            get
            {
                if (instance == null) instance = new FormElementEventListener();
                return instance;
            }
        }
        #endregion

        public override void OnHandleGeckoDomEvent(GeckoDOMEventArgs args)
        {
            Console.WriteLine("[FormElementEventListener.OnHandleGeckoDomEvent] 'args.Type': " + args.Type);
            switch (args.Type)
            {
                case "input":
                    break;
                case "change":
                    break;

                default:
                    break;
            }
        }
    }
    #endregion

    #region FormFillController
    /// <summary>
    /// For a Focused Input Element to find saved value from the text or selection previously saved
    /// If an <input> is focused, check if it has a list="<datalist>" which can
    /// provide the list of suggestions.
    /// </summary>
    public class FormFillController : ComObject<nsIFormFillController>
    {
        public FormFillController() : base(Xpcom.GetService2<nsIFormFillController>(FormAutoFillContracts.Satchel_Form_Fill_Controller).Instance)
        {
        }
    }
    #endregion

    #region FormHistory
    ///<summary>
    /// * FormHistory
    /// *
    /// * Used to store values that have been entered into forms which may later
    /// * be used to automatically fill in the values when the form is visited again.
    /// *
    /// * search(terms, queryData, callback)
    /// *   Look up values that have been previously stored.
    /// *     terms - array of terms to return data for
    /// *     queryData - object that contains the query terms
    /// *       The query object contains properties for each search criteria to match, where the value
    /// *       of the property specifies the value that term must have. For example,
    /// *       { term1: value1, term2: value2 }
    /// *     callback - callback that is called when results are available or an error occurs.
    /// *       The callback is passed a result array containing each found entry. Each element in
    /// *       the array is an object containing a property for each search term specified by 'terms'.
    /// * count(queryData, callback)
    /// *   Find the number of stored entries that match the given criteria.
    /// *     queryData - array of objects that indicate the query. See the search method for details.
    /// *     callback - callback that is called when results are available or an error occurs.
    /// *       The callback is passed the number of found entries.
    /// * update(changes, callback)
    /// *    Write data to form history storage.
    /// *      changes - an array of changes to be made. If only one change is to be made, it
    /// *                may be passed as an object rather than a one-element array.
    /// *        Each change object is of the form:
    /// *          { op: operation, term1: value1, term2: value2, ... }
    /// *        Valid operations are:
    /// *          add - add a new entry
    /// *          update - update an existing entry
    /// *          remove - remove an entry
    /// *          bump - update the last accessed time on an entry
    /// *        The terms specified allow matching of one or more specific entries. If no terms
    /// *        are specified then all entries are matched. This means that { op: "remove" } is
    /// *        used to remove all entries and clear the form history.
    /// *      callback - callback that is called when results have been stored.
    /// * getAutoCompeteResults(searchString, params, callback)
    /// *   Retrieve an array of form history values suitable for display in an autocomplete list.
    /// *   Returns an mozIStoragePendingStatement that can be used to cancel the operation if
    /// *   needed.
    /// *     searchString - the string to search for, typically the entered value of a textbox
    /// *     params - zero or more filter arguments:
    /// *       fieldname - form field name
    /// *       agedWeight
    /// *       bucketSize
    /// *       expiryDate
    /// *       maxTimeGroundings
    /// *       timeGroupingSize
    /// *       prefixWeight
    /// *       boundaryWeight
    /// *     callback - callback that is called with the array of results. Each result in the array
    /// *                is an object with four arguments:
    /// *                  text, textLowerCase, frecency, totalScore
    /// * schemaVersion
    /// *   This property holds the version of the database schema
    /// *
    /// * Terms:
    /// *  guid - entry identifier. For 'add', a guid will be generated.
    /// *  fieldname - form field name
    /// *  value - form value
    /// *  timesUsed - the number of times the entry has been accessed
    /// *  firstUsed - the time the the entry was first created
    /// *  lastUsed - the time the entry was last accessed
    /// *  firstUsedStart - search for entries created after or at this time
    /// *  firstUsedEnd - search for entries created before or at this time
    /// *  lastUsedStart - search for entries last accessed after or at this time
    /// *  lastUsedEnd - search for entries last accessed before or at this time
    /// *  newGuid - a special case valid only for 'update' and allows the guid for
    /// *            an existing record to be updated. The 'guid' term is the only
    /// *            other term which can be used (ie, you can not also specify a
    /// *            fieldname, value etc) and indicates the guid of the existing
    /// *            record that should be updated.
    /// *
    /// * In all of the above methods, the callback argument should be an object with
    /// * handleResult(result), handleFailure(error) and handleCompletion(reason) functions.
    /// * For search and getAutoCompeteResults, result is an object containing the desired
    /// * properties. For count, result is the integer count. For, update, handleResult is
    /// * not called. For handleCompletion, reason is either 0 if successful or 1 if
    /// * an error occurred.
    /// */
    /// </summary>
    public class FormHistory : ComObject<nsIFormHistory2>
    {
        public FormHistory() : base(Xpcom.GetService2<nsIFormHistory2>(FormAutoFillContracts.Satchel_Form_History).Instance)
        {
        }

        /// <summary>
		/// check if and add entry 
        /// if it does it wont add it and return false 
        /// if it doesnt itl add it and return true
		/// </summary>
        public bool AddedEntry(string name, string value)
        {
            AddEntry(name, value);
            return EntryExists(name, value);
        }

        /// <summary>
		/// Adds a name and value pair to the form history.
		/// </summary>
        public void AddEntry(string name, string value)
        {
            using (nsAString nameVal = new nsAString(name),
                             valueVal = new nsAString(value))
            {
                Instance.AddEntry(nameVal, valueVal);
            }
        }

        /// <summary>
        /// Gets whether a name and value pair exists in the form history.
        /// </summary>
        public bool EntryExists(string name, string value)
        {
            var exists = false;

            using (nsAString nameVal = new nsAString(name),
                             valueVal = new nsAString(value))
            {
                exists = Instance.EntryExists(nameVal, valueVal);
            }

            return exists;
        }

        /// <summary>
		/// Returns true if there is no entry that is paired with a name.
		/// </summary>
        public bool NameExists(string name)
        {
            using (nsAString nameVal = new nsAString(name))
            {
                return Instance.NameExists(nameVal);
            }
        }
    }
    #endregion

    #region FormAutoComplete
    public class FormAutoComplete : ComObject<nsIFormAutoComplete>, nsIFormAutoCompleteObserver
    {
        public event Action OnSearchCompletionResult;

        public ComObject<nsIAutoCompleteResult> PreviousResult { get; private set; }
        public ComObject<nsIAutoCompleteResult> DatalistResult { get; private set; }



        //public Task AutoCompleteSearchTask { get; set; }
        //public CancellationTokenSource AutoCompleteSearchTaskCancellationSource { get; set; }

        public FormAutoComplete() : base(Xpcom.GetService2<nsIFormAutoComplete>(FormAutoFillContracts.Satchel_Form_Autocomplete).Instance)
        {
        }

        public void OnSearchCompletion(nsIAutoCompleteResult result)
        {
            PreviousResult = new ComObject<nsIAutoCompleteResult>(result);
            OnSearchCompletionResult?.Invoke();
        }


        /// <summary>
        /// Generate results for a form input autocomplete menu asynchronously.
        /// 
        /// aInputName    -- |name| attribute from the form input being autocompleted.
        /// aUntrimmedSearchString -- current value of the input
        /// aField -- nsIDOMHTMLInputElement being autocompleted(may be null if from chrome)
        /// aPreviousResult -- previous search result, if any.
        /// aDatalistResult -- results from list=datalist for aField.
        /// aListener -- nsIFormAutoCompleteObserver that listens for the nsIAutoCompleteResult
        ///              that may be returned asynchronously.
        ///
        /// </summary>
        public void AutoCompleteSearchAsync(string aInputName, string aSearchString, GeckoHTMLInputElement aField, nsIAutoCompleteResult aPreviousResult, nsIAutoCompleteResult aDatalistResult, nsIFormAutoCompleteObserver aListener)
        {
            using (nsAString inputName = new nsAString(aInputName),
                             searchString = new nsAString(aSearchString))
            {
                Instance.AutoCompleteSearchAsync(inputName, searchString, aField.Instance, aPreviousResult == null ? PreviousResult == null ? null : PreviousResult.Instance : aPreviousResult, aDatalistResult, aListener == null ? this : aListener);

                //AutoCompleteSearchTaskCancellationSource = new CancellationTokenSource();
                //AutoCompleteSearchTask = Task.Run(() =>
                //{

                //}, AutoCompleteSearchTaskCancellationSource.Token);
            }
        }

        /// <summary>
        /// If a search is in progress, stop it. Otherwise, do nothing. This is used
        /// to cancel an existing search, for example, in preparation for a new search.
        /// </summary>
        public void StopAutoCompleteSearch()
        {
            Instance.StopAutoCompleteSearch();
            //AutoCompleteSearchTaskCancellationSource.Cancel();
        }

        /// <summary>
        /// Since the controller is disconnecting, any related data must be cleared.
        /// </summary>
        public void StopControllingInput(GeckoHTMLInputElement aField)
        {
            Instance.StopControllingInput(aField.Instance);
            //AutoCompleteSearchTaskCancellationSource.Cancel();
        }

    }
    #endregion

    #region InputListAutoComplete
    /// <summary>
    /// For a Focused Input Element to find saved value from the text or selection previously saved
    /// If an <input> is focused, check if it has a list="<datalist>" which can
    /// provide the list of suggestions.
    /// </summary>
    public class InputListAutoComplete : ComObject<nsIInputListAutoComplete>
    {
        public InputListAutoComplete() : base(Xpcom.GetService2<nsIInputListAutoComplete>(FormAutoFillContracts.Satchel_Inputlist_Autocomplete).Instance)
        {
        }

        /// <summary>
        /// Generate results for a form input autocomplete menu.
        /// <summary>
        public ComObject<nsIAutoCompleteResult> AutoCompleteSearch(string aSearchString, GeckoHTMLInputElement aField)
        {
            using (nsAString searchString = new nsAString(aSearchString))
            {
                return new ComObject<nsIAutoCompleteResult>(Instance.AutoCompleteSearch(searchString, aField.Instance));
            }
        }
    }
    #endregion
}

//#region html element attributes
//// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input
//// https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#autofill

//public IList<string> GlobalAttributes
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "accesskey",
//                    "contenteditable",
//                    "dir",
//                    "draggable",
//                    "hidden",
//                    "is",
//                    "itemid",
//                    "itemprop",
//                    "itemref",
//                    "itemscope",
//                    "itemtype",
//                    "lang",
//                    "spellcheck",
//                    "style",
//                    "tabindex",
//                    "title",
//                    "translate",
//                });
//    }
//}

//#region autocomplete
////1. Optionally, a token whose first eight characters are an ASCII case-insensitive match for the string "section-", meaning that the field belongs to the named group.
//public const string AutocompleteNamedGroup = "section-";

////2. Optionally, a token that is an ASCII case-insensitive match for one of the following strings:
//public const string AutocompleteNamedGroupShipping = "shipping";
//public const string AutocompleteNamedGroupBilling = "billing";

////3. Either of the following two options:
////A token that is an ASCII case-insensitive match for one of the following autofill field names, excluding those that are inappropriate for the control:
//public IList<string> AutocompleteFeilds
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "name",
//                    "honorific-prefix",
//                    "given-name",
//                    "additional-name",
//                    "family-name",
//                    "honorific-suffix",
//                    "nickname",
//                    "username",
//                    "new-password",
//                    "current-password",
//                    "organization-title",
//                    "organization",
//                    "street-address",
//                    "address-line1",
//                    "address-line2",
//                    "address-line3",
//                    "address-level4",
//                    "address-level3",
//                    "address-level2",
//                    "address-level1",
//                    "country",
//                    "country-name",
//                    "postal-code",
//                    "cc-name",
//                    "cc-given-name",
//                    "cc-additional-name",
//                    "cc-family-name",
//                    "cc-number",
//                    "cc-exp",
//                    "cc-exp-month",
//                    "cc-exp-year",
//                    "cc-csc",
//                    "cc-type",
//                    "transaction-currency",
//                    "transaction-amount",
//                    "language",
//                    "bday",
//                    "bday-day",
//                    "bday-month",
//                    "bday-year",
//                    "sex",
//                    "url",
//                    "photo",
//                });
//    }
//}


////// Or The following, in the given order:
////1. Optionally, a token that is an ASCII case-insensitive match for one of the following strings:
////Option "home"
//public const string AutocompleteOptionHome = "home";
////Option "work"
//public const string AutocompleteOptionWork = "work";
////Option "mobile"
//public const string AutocompleteOptionMobile = "mobile";
////Option "fax"
//public const string AutocompleteOptionFax = "fax";
////Option "pager"
//public const string AutocompleteOptionPager = "pager";

////2. A token that is an ASCII case-insensitive match for one of the following autofill field names, excluding those that are inappropriate for the control:
//public IList<string> AutocompleteFeildNames
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "tel",
//                    "tel-country-code",
//                    "tel-national",
//                    "tel-area-code",
//                    "tel-local",
//                    "tel-local-prefix",
//                    "tel-local-suffix",
//                    "tel-extension",
//                    "email",
//                    "impp",
//                });
//    }
//}
//#endregion


////input types\\
//public IList<string> InputTypes
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "hidden",
//                    "text",
//                    "search",
//                    "tel",
//                    "url",
//                    "email",
//                    "password",
//                    "date",
//                    "month",
//                    "week",
//                    "time",
//                    "datetime-local",
//                    "number",
//                    "range",
//                    "color",
//                    "checkbox",
//                    "radio",
//                    "file",
//                    "submit",
//                    "image",
//                    "reset",
//                    "button",
//                });
//    }
//}
//public IList<string> InputContentAttributes
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "accept",
//                    "alt",
//                    "autocomplete",
//                    "autofocus",
//                    "checked",
//                    "dirname",
//                    "disabled",
//                    "form",
//                    "formaction",
//                    "formenctype",
//                    "formmethod",
//                    "formnovalidate",
//                    "formtarget",
//                    "height",
//                    "id",
//                    "Indeterminate",
//                    "inputmode",
//                    "list",
//                    "max",
//                    "maxlength",
//                    "min",
//                    "minlength",
//                    "multiple",
//                    "name",
//                    "pattern",
//                    "placeholder",
//                    "readonly",
//                    "required",
//                    "size",
//                    "src",
//                    "step",
//                    "type",
//                    "width",
//                });
//    }
//}
//public IList<string> InputIDLAttributesAndMethods
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "checked",
//                    "files",
//                    "value",
//                    "valueAsDate",
//                    "valueAsNumber",
//                    "list",
//                    "select()",
//                    "selectionStart",
//                    "selectionEnd",
//                    "selectionDirection",
//                    "setRangeText()",
//                    "setSelectionRange()",
//                    "stepDown()",
//                    "stepUp()",
//                });
//    }
//}
//public IList<string> InputEvents
//{
//    get
//    {
//        return new List<string>(new string[] {
//                    "input",
//                    "change",
//                });
//    }
//}
//#endregion