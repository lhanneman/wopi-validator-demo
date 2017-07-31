using OfficeOnlineDemo.Interfaces;
using System;
using System.Web;
using OfficeOnlineDemo.Models;
using OfficeOnlineDemo.Helpers;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Handlers
{
    public class Lock : IWopiHandler
    {
        private readonly HttpRequestBase _request;

        public Lock(HttpRequestBase request)
        {
            _request = request;
        }

        public WopiResponse Handle()
        {
            var requestData = WopiRequest.ParseRequest(_request);
            if (!ValidationHelper.ValidateAccess(requestData.AccessToken, writeAccessRequired: true))
            {
                return new WopiResponse() { ResponseType = WopiResponseType.InvalidToken, Message = "Invalid Token" };
            }

            var documentId = Convert.ToInt64(requestData.Id);
            var document = FB.GetDocument(documentId);

            if (document == null)
            {
                return new WopiResponse() { ResponseType = WopiResponseType.FileUnknown, Message = "File not found" };
            }

            var newLock = new LockInfo() { DateCreated = DateTime.UtcNow, Lock = _request.Headers[WopiHeaders.Lock] };
            var oldLock = _request.Headers[WopiHeaders.OldLock];

            var currentLockInfo = FB.GetLockInfo(document);
            var currentLock = !string.IsNullOrEmpty(currentLockInfo)
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<LockInfo>(currentLockInfo)
                : null;

            if (currentLock == null)
            {
                // The file is not currently locked or the lock has already expired
                FB.UpdateLockInfo(document, Newtonsoft.Json.JsonConvert.SerializeObject(newLock));
                return new WopiResponse() { ResponseType = WopiResponseType.Success };
            }

            lock (currentLock)
            {
                // unlock & relock:
                if (!string.IsNullOrEmpty(oldLock))
                {
                    if (oldLock == currentLock.Lock && oldLock != newLock.Lock)
                    {
                        FB.UpdateLockInfo(document, Newtonsoft.Json.JsonConvert.SerializeObject(newLock));
                        return new WopiResponse() { ResponseType = WopiResponseType.Success };
                    } 

                    if (oldLock != currentLock.Lock)
                    {
                        return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = currentLock.Lock };
                    }
                }

                if (currentLock.Lock != newLock.Lock)
                {
                    // trying to lock again with a different lock? YEAH RIGHT
                    return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = currentLock.Lock };
                }

                // locking with the same lock? validator says this should be ok:
                return new WopiResponse() { ResponseType = WopiResponseType.Success }; 
            }
        }

        public WopiResponse HandleRefresh()
        {
            var requestData = WopiRequest.ParseRequest(_request);
            if (!ValidationHelper.ValidateAccess(requestData.AccessToken, writeAccessRequired: true))
            {
                return new WopiResponse() { ResponseType = WopiResponseType.InvalidToken, Message = "Invalid Token" };
            }

            var documentId = Convert.ToInt64(requestData.Id);
            var document = FB.GetDocument(documentId);

            if (document == null)
            {
                return new WopiResponse() { ResponseType = WopiResponseType.FileUnknown, Message = "File not found" };
            }

            var newLock = new LockInfo() { DateCreated = DateTime.UtcNow, Lock = _request.Headers[WopiHeaders.Lock] };

            var currentLockInfo = FB.GetLockInfo(document);
            var currentLock = !string.IsNullOrEmpty(currentLockInfo)
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<LockInfo>(currentLockInfo)
                : null;

            if (currentLock == null)
            {
                // on refresh, there should already be a lock
                return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = "" };
            }

            lock (currentLock)
            {
                if (currentLock.Lock != newLock.Lock)
                {
                    // trying to lock again with a different lock? YEAH RIGHT
                    return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = currentLock.Lock };
                }

                // update the lock (refresh) and return success:
                FB.UpdateLockInfo(document, Newtonsoft.Json.JsonConvert.SerializeObject(newLock));
                return new WopiResponse() { ResponseType = WopiResponseType.Success };
            }
        }
    }
}