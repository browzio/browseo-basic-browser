using Gecko;
using Gecko.CustomMarshalers;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    public class nsToolkitProfile : nsIToolkitProfile, nsISupports
    {
        public uint AddRef()
        {
            throw new NotImplementedException();
        }
        public IntPtr QueryInterface(ref Guid uuid)
        {
            throw new NotImplementedException();
        }

        public uint Release()
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIFile GetLocalDirAttribute()
        {
            return FXServices.DirectoryServiceProvider.NewLocalFile(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath);
        }

        public void GetNameAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase result)
        {
            result.SetData(BrowseoFXManager.Instance.Project.ProjectName);
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIFile GetRootDirAttribute()
        {
            return FXServices.DirectoryServiceProvider.NewLocalFile(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath);
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIProfileLock Lock([MarshalAs(UnmanagedType.Interface)] out nsIProfileUnlocker aUnlocker)
        {
            aUnlocker = null;
            return null;
        }

        public void Remove([MarshalAs(UnmanagedType.U1)] bool removeFiles)
        {
        }

        public void SetNameAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase value)
        {
        }
    }

    public class nsToolkitProfileService : nsIToolkitProfileService
    {
        private static nsToolkitProfileService _instancs;
        public static nsToolkitProfileService Instance
        {
            get
            {
                if (_instancs == null) _instancs = new nsToolkitProfileService();
                return _instancs;
            }
        }


        ProfileEnumarator SimpleProfileEnumarator = new ProfileEnumarator();
        nsToolkitProfile Profile = new nsToolkitProfile();

        [return: MarshalAs(UnmanagedType.U1)]
        public bool GetStartWithLastProfileAttribute()
        {
            return false;
        }

        public void SetStartWithLastProfileAttribute([MarshalAs(UnmanagedType.U1)] bool value)
        {

        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool GetStartOfflineAttribute()
        {
            return false;
        }

        public void SetStartOfflineAttribute([MarshalAs(UnmanagedType.U1)] bool value)
        {
        }


        [return: MarshalAs(UnmanagedType.Interface)]
        public nsISimpleEnumerator GetProfilesAttribute()
        {
            var spe = new ProfileEnumarator();
            return spe;
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIToolkitProfile GetSelectedProfileAttribute()
        {
            return Profile;
        }

        public void SetSelectedProfileAttribute([MarshalAs(UnmanagedType.Interface)] nsIToolkitProfile value)
        {

        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIToolkitProfile GetDefaultProfileAttribute()
        {
            return Profile;
        }

        public void SetDefaultProfileAttribute([MarshalAs(UnmanagedType.Interface)] nsIToolkitProfile value)
        {
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIToolkitProfile GetProfileByName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase aName)
        {
            return Profile;
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIProfileLock LockProfilePath([MarshalAs(UnmanagedType.Interface)] nsIFile aDirectory, [MarshalAs(UnmanagedType.Interface)] nsIFile aTempDirectory)
        {
            return null;
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIToolkitProfile CreateProfile([MarshalAs(UnmanagedType.Interface)] nsIFile aRootDir, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase aName)
        {
            return null;
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIToolkitProfile CreateDefaultProfileForApp([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase aProfileName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase aAppName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase aVendorName)
        {
            return null;
        }

        public uint GetProfileCountAttribute()
        {
            return 1;
        }

        public void Flush()
        {

        }
    }

    public class ProfileEnumarator : nsISimpleEnumerator
    {
        bool checkedIt = false;
        [return: MarshalAs(UnmanagedType.Interface)]
        public nsISupports GetNext()
        {
            var pr = nsToolkitProfileService.Instance.GetSelectedProfileAttribute();
            return pr as nsISupports;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool HasMoreElements()
        {
            if(checkedIt)
             return false;
            else
            {
                checkedIt = true;
                return true;
            }
        }
    }
}
