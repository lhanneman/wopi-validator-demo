using OfficeOnlineDemo.Interfaces;
using OfficeOnlineDemo.Models;
using System;
using System.IO;
using System.Web;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Handlers
{
    public class PutFile : IWopiHandler
    {
        private readonly HttpRequestBase _request;

        public PutFile(HttpRequestBase request)
        {
            _request = request;
        }

        public WopiResponse Handle()
        {
            var requestData = WopiRequest.ParseRequest(_request);

            try
            {
                var documentId = Convert.ToInt64(requestData.Id);
                var document = FB.GetDocument(documentId);

                if (document == null)
                {
                    return new WopiResponse() { ResponseType = WopiResponseType.FileUnknown, Message = "File not found" };
                }

                string newLock = _request.Headers[WopiHeaders.Lock];

                var currentLockInfo = FB.GetLockInfo(document);
                var currentLock = !string.IsNullOrEmpty(currentLockInfo)
                                    ? Newtonsoft.Json.JsonConvert.DeserializeObject<LockInfo>(currentLockInfo)
                                    : null;

                if (currentLock != null && (currentLock.Lock != newLock))
                {
                    return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = currentLock.Lock };
                }

                // FileInfo putTargetFileInfo = new FileInfo(requestData.FullPath);

                // The WOPI spec allows for a PutFile to succeed on a non-locked file if the file is currently zero bytes in length.
                // This allows for a more efficient Create New File flow that saves the Lock roundtrips.
                //if (!hasExistingLock && putTargetFileInfo.Length != 0)
                //{
                //    // With no lock and a non-zero file, a PutFile could potentially result in data loss by clobbering
                //    // existing content.  Therefore, return a lock mismatch error.
                //    ReturnLockMismatch(context.Response, reason: "PutFile on unlocked file with current size != 0");
                //}

                // Either the file has a valid lock that matches the lock in the request, or the file is unlocked
                // and is zero bytes.  Either way, proceed with the PutFile.

                _request.InputStream.Position = 0;
                byte[] binary;
                using (var ms = new MemoryStream())
                {
                    _request.InputStream.CopyTo(ms);
                    binary = ms.ToArray();
                }

                FB.SaveDocument(document, binary);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new WopiResponse() { ResponseType = WopiResponseType.FileUnknown, Message = uex.Message };
            }
            catch (Exception ex)
            {
                return new WopiResponse() { ResponseType = WopiResponseType.ServerError, Message = ex.Message };
            }

            return new WopiResponse() { ResponseType = WopiResponseType.Success };
        }
    }
}