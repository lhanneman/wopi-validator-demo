using OfficeOnlineDemo.Interfaces;
using System;
using System.Web;
using OfficeOnlineDemo.Models;
using FB = FileBoundHelper.Helper;
using System.Collections.Specialized;
using OfficeOnlineDemo.Helpers;

namespace OfficeOnlineDemo.Handlers
{
    public class Rename : IWopiJsonHandler
    {
        private readonly HttpRequestBase _request;

        public Rename(HttpRequestBase request)
        {
            _request = request;
        }

        public WopiJsonResponse Handle()
        {
            var requestData = WopiRequest.ParseRequest(_request);
            var newName = _request.Headers[WopiHeaders.RequestedName];

            try
            {
                // if the file name is illegal then
                // return 400
                // add response header WopiHeaders.InvalidFileNameError - describing the reason the rename operation could not be completed
                if (string.IsNullOrEmpty(newName))
                {
                    return new WopiJsonResponse()
                    {
                        StatusCode = 400,
                        Headers = new NameValueCollection { { WopiHeaders.InvalidFileNameError, "Invalid filename for rename" } },
                    };
                }

                newName = IOHelper.Utf7Encode(newName);

                var documentId = Convert.ToInt64(requestData.Id);
                var document = FB.GetDocument(documentId);

                if (document == null)
                {
                    return new WopiJsonResponse() { StatusCode = 404, Json = new RenameResponse() };
                }

                var currentLock = "";
                if (LockHelper.IsLockMismatch(_request, document, out currentLock))
                {
                    return new WopiJsonResponse() { StatusCode = 409, Json = new RenameResponse() };
                }

                // rename the document:
                FB.RenameDocument(document, newName);

            }
            catch (Exception ex)
            {
                //return new WopiResponse() { ResponseType = WopiResponseType.ServerError, Message = ex.Message };
                return new WopiJsonResponse() { StatusCode = 500, Json = new { Message = ex.Message } };
            }

            //return new WopiResponse() { ResponseType = WopiResponseType.Success };
            return new WopiJsonResponse() { StatusCode = 200, Json = new RenameResponse() { Name = newName } };
        }
    }
}