using System;
using System.Web;

namespace OfficeOnlineDemo.Models
{
    public enum RequestType
    {
        None,

        CheckFileInfo,
        PutRelativeFile,

        Lock,
        Unlock,
        RefreshLock,
        UnlockAndRelock,

        ExecuteCobaltRequest,

        DeleteFile,
        ReadSecureStore,
        GetRestrictedLink,
        RevokeRestrictedLink,

        CheckFolderInfo,

        GetFile,
        PutFile,

        EnumerateChildren,
    }

    static class WopiHeaders
    {
        public const string RequestType = "X-WOPI-Override";

        public const string Lock = "X-WOPI-Lock";
        public const string OldLock = "X-WOPI-OldLock";
        public const string LockFailureReason = "X-WOPI-LockFailureReason";
        public const string LockedByOtherInterface = "X-WOPI-LockedByOtherInterface";

        public const string SuggestedTarget = "X-WOPI-SuggestedTarget";
        public const string RelativeTarget = "X-WOPI-RelativeTarget";
        public const string OverwriteRelativeTarget = "X-WOPI-OverwriteRelativeTarget";

        public const string RequestedName = "X-WOPI-RequestedName";
        public const string InvalidFileNameError = "X-WOPI-InvalidFileNameError ";
    }

    public class WopiRequest
    {
        public RequestType Type { get; set; }

        public string AccessToken { get; set; }

        public string Id { get; set; }

        //public string FullPath
        //{
        //    get { return Path.Combine(WopiHandler.LocalStoragePath, Id); }
        //}

        private const string WopiPath = @"/wopi/";
        private const string FilesRequestPath = @"files/";
        private const string FoldersRequestPath = @"folders/";
        private const string ContentsRequestPath = @"/contents";
        private const string ChildrenRequestPath = @"/children";
        //internal const string LocalStoragePath = @"d:\WopiStorage\";

        public static bool TimestampOlderThan20Min(long ticks)
        {
            var currentDate = DateTime.UtcNow;
            var elapsedTicks = Math.Abs(currentDate.Ticks - ticks);
            var elapsedSpan = new TimeSpan(elapsedTicks);
            return elapsedSpan.Minutes > 20;
        }

        public static WopiRequest ParseRequest(HttpRequestBase request)
        {
            // Initilize wopi request data object with default values
            WopiRequest requestData = new WopiRequest()
            {
                Type = RequestType.None,
                AccessToken = request.QueryString["access_token"],
                Id = ""
            };

            // request.Url pattern:
            // http(s)://server/<...>/wopi/[files|folders]/<id>?access_token=<token>
            // or
            // https(s)://server/<...>/wopi/files/<id>/contents?access_token=<token>
            // or
            // https(s)://server/<...>/wopi/folders/<id>/children?access_token=<token>

            // Get request path, e.g. /<...>/wopi/files/<id>
            string requestPath = request.Url.AbsolutePath;
            // remove /<...>/wopi/
            string wopiPath = requestPath.Substring(WopiPath.Length);

            if (wopiPath.StartsWith(FilesRequestPath))
            {
                // A file-related request

                // remove /files/ from the beginning of wopiPath
                string rawId = wopiPath.Substring(FilesRequestPath.Length);

                if (rawId.EndsWith(ContentsRequestPath))
                {
                    // The rawId ends with /contents so this is a request to read/write the file contents

                    // Remove /contents from the end of rawId to get the actual file id
                    requestData.Id = rawId.Substring(0, rawId.Length - ContentsRequestPath.Length);

                    if (request.HttpMethod == "GET")
                        requestData.Type = RequestType.GetFile;
                    if (request.HttpMethod == "POST")
                        requestData.Type = RequestType.PutFile;
                }
                else
                {
                    requestData.Id = rawId;

                    if (request.HttpMethod == "GET")
                    {
                        // a GET to the file is always a CheckFileInfo request
                        requestData.Type = RequestType.CheckFileInfo;
                    }
                    else if (request.HttpMethod == "POST")
                    {
                        // For a POST to the file we need to use the X-WOPI-Override header to determine the request type
                        string wopiOverride = request.Headers[WopiHeaders.RequestType];

                        switch (wopiOverride)
                        {
                            case "PUT_RELATIVE":
                                requestData.Type = RequestType.PutRelativeFile;
                                break;
                            case "LOCK":
                                // A lock could be either a lock or an unlock and relock, determined based on whether
                                // the request sends an OldLock header.
                                if (request.Headers[WopiHeaders.OldLock] != null)
                                    requestData.Type = RequestType.UnlockAndRelock;
                                else
                                    requestData.Type = RequestType.Lock;
                                break;
                            case "UNLOCK":
                                requestData.Type = RequestType.Unlock;
                                break;
                            case "REFRESH_LOCK":
                                requestData.Type = RequestType.RefreshLock;
                                break;
                            case "COBALT":
                                requestData.Type = RequestType.ExecuteCobaltRequest;
                                break;
                            case "DELETE":
                                requestData.Type = RequestType.DeleteFile;
                                break;
                            case "READ_SECURE_STORE":
                                requestData.Type = RequestType.ReadSecureStore;
                                break;
                            case "GET_RESTRICTED_LINK":
                                requestData.Type = RequestType.GetRestrictedLink;
                                break;
                            case "REVOKE_RESTRICTED_LINK":
                                requestData.Type = RequestType.RevokeRestrictedLink;
                                break;
                        }
                    }
                }
            }
            else if (wopiPath.StartsWith(FoldersRequestPath))
            {
                // A folder-related request.

                // remove /folders/ from the beginning of wopiPath
                string rawId = wopiPath.Substring(FoldersRequestPath.Length);

                if (rawId.EndsWith(ChildrenRequestPath))
                {
                    // rawId ends with /children, so it's an EnumerateChildren request.

                    // remove /children from the end of rawId
                    requestData.Id = rawId.Substring(0, rawId.Length - ChildrenRequestPath.Length);
                    requestData.Type = RequestType.EnumerateChildren;
                }
                else
                {
                    // rawId doesn't end with /children, so it's a CheckFolderInfo.

                    requestData.Id = rawId;
                    requestData.Type = RequestType.CheckFolderInfo;
                }
            }
            else
            {
                // An unknown request.
                requestData.Type = RequestType.None;
            }

            return requestData;
        }

    }


}