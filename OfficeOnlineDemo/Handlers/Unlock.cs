using OfficeOnlineDemo.Interfaces;
using System;
using System.Web;
using OfficeOnlineDemo.Models;
using FB = FileBoundHelper.Helper;


namespace OfficeOnlineDemo.Handlers
{
    public class Unlock : IWopiHandler
    {
        private readonly HttpRequestBase _request;

        public Unlock(HttpRequestBase request)
        {
            _request = request;
        }

        public WopiResponse Handle()
        {
            var requestData = WopiRequest.ParseRequest(_request);
            var documentId = Convert.ToInt64(requestData.Id);
            var document = FB.GetDocument(documentId);

            try
            {
                if (document == null)
                {
                    return new WopiResponse() { ResponseType = WopiResponseType.FileUnknown, Message = "File not found" };
                }

                var newLock = _request.Headers[WopiHeaders.Lock];

                var currentLockInfo = FB.GetLockInfo(document);
                var currentLock = !string.IsNullOrEmpty(currentLockInfo)
                    ? Newtonsoft.Json.JsonConvert.DeserializeObject<LockInfo>(currentLockInfo)
                    : null;

                if (currentLock == null)
                {
                    return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch };
                }

                lock (currentLock)
                {
                    if (currentLock.Lock == newLock)
                    {
                        // There is a valid lock on the file and the existing lock matches the provided one
                        FB.UpdateLockInfo(document, string.Empty);
                        return new WopiResponse() { ResponseType = WopiResponseType.Success };
                    }
                    else
                    {
                        // The existing lock doesn't match the requested one.  Return a lock mismatch error
                        return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = currentLock.Lock };
                    }
                }
            }
            catch (Exception ex)
            {
                FB.UpdateLockInfo(document, ex.Message);
                return new WopiResponse() { ResponseType = WopiResponseType.ServerError, Message = ex.Message };
            }
        }
    }
}