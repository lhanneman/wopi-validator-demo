using OfficeOnlineDemo.Helpers;
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

                var currentLock = "";
                if (LockHelper.IsLockMismatch(_request, document, out currentLock))
                {
                    return new WopiResponse() { ResponseType = WopiResponseType.LockMismatch, Message = currentLock };
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

                var binary = IOHelper.StreamToBytes(_request.InputStream);
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