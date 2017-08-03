using Newtonsoft.Json;
using System;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Models
{

    // https://wopirest.readthedocs.io/en/latest/files/CheckFileInfo.html#checkfileinfo
    public class CheckFileInfoResponse
    {

        public CheckFileInfoResponse InitializeValidatorParams(string file_id, string access_token, string access_token_ttl)
        {
            var documentId = Convert.ToInt64(file_id);
            var document = FB.GetDocument(documentId);

            BaseFileName = "document.wopitest";
            OwnerId = "1";
            Size = document.BinaryData.Length;
            UserId = "1";
            Version = "1.0";
            UserFriendlyName = "Lloyd";
            UserInfo = JsonConvert.SerializeObject(new { userName = "Lloyd Hanneman", userId = 1 });

            //SupportedShareUrlTypes = new string[] { "ReadWrite" }; //  is another option

            ReadOnly = false;
            RestrictedWebViewOnly = false;
           // UserCanAttend = true;
            //UserCanPresent = true;
            UserCanNotWriteRelative = false;
            UserCanRename = true;
            UserCanWrite = true;

            SupportsUpdate = true;
            SupportsLocks = true;
            SupportsExtendedLockLength = true;
            SupportsGetLock = true;
            SupportsRename = true;
            SupportsUserInfo = true;
            SupportsDeleteFile = true;

            IsAnonymousUser = false;
            return this;
        }

        #region required properties 

        // The string name of the file, including extension, without a path. 
        // Used for display in user interface (UI), and determining the extension of the file.
        public string BaseFileName { get; set; }

        // The string name of the file, including extension, without a path. 
        // Used for display in user interface (UI), and determining the extension of the file.
        public string OwnerId { get; set; }

        // The size of the file in bytes, expressed as a long, a 64-bit signed integer.
        public long Size { get; set; }

        // A string value uniquely identifying the user currently accessing the file.
        public string UserId { get; set; }

        // The current version of the file based on the server’s file version schema, as a string. 
        // This value must change when the file changes, and version values must never repeat for a given file.
        public string Version { get; set; }

        #endregion

        #region wopi host capabilities

       // public string[] SupportedShareUrlTypes { get; set; }
        public bool SupportsCobalt { get; set; }
        public bool SupportsContainers { get; set; }
        public bool SupportsDeleteFile { get; set; }
        public bool SupportsEcosystem { get; set; }
        public bool SupportsExtendedLockLength { get; set; }
        public bool SupportsFolders { get; set; }
       // public bool SupportsGetFileWopiSrc { get; set; }
        public bool SupportsGetLock { get; set; }
        public bool SupportsLocks { get; set; }
        public bool SupportsRename { get; set; }
        public bool SupportsUpdate { get; set; }
        public bool SupportsUserInfo { get; set; }

        #endregion

        #region user permissions

        // indicates that, for this user, the file cannot be changed
        public bool ReadOnly { get; set; }

        // tells the WOPI client it should restrict what actions
        // the user can perform on the file - dependent on WOPI client
        public bool RestrictedWebViewOnly { get; set; }

        // indicates whether the user has permission to view a broadcast of the file
        /* A broadcast is a special Office Online scenario where navigation through a document is driven by one or more presenters. A set of attendees can follow along with the presenter remotely. */
        public bool UserCanAttend { get; set; }

        // indicates the user does not have sufficient permission to create new files on the wopi server
        public bool UserCanNotWriteRelative { get; set; }

        // indicates this user has permission to broadcast this file to a set of users who have permission to broadcast or view a broadcast of the current file
        public bool UserCanPresent { get; set; }

        // indicates the user can rename the file
        public bool UserCanRename { get; set; }

        // indicates teh user has permission to alter the file. setting to TRUE, tells the WOPI client it can call PutFile on behalf of the user
        public bool UserCanWrite { get; set; }

        #endregion

        #region user metadata

        public bool IsAnonymousUser { get; set; }
        public bool IsEduUser { get; set; }
        public bool LicenseCheckForEditIsEnabled { get; set; }
        public string UserFriendlyName { get; set; }
        public string UserInfo { get; set; }
        #endregion

    }
}