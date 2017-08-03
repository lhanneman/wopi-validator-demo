using OfficeOnlineDemo.Interfaces;
using OfficeOnlineDemo.Models;
using System.Web;
using FB = FileBoundHelper.Helper;
using OfficeOnlineDemo.Helpers;
using System;

namespace OfficeOnlineDemo.Handlers
{
    public class PutRelative : IWopiJsonHandler
    {
        private readonly HttpRequestBase _request;

        public PutRelative(HttpRequestBase request)
        {
            _request = request;
        }

        public WopiJsonResponse Handle()
        {
            var requestData = WopiRequest.ParseRequest(_request);

            var responseData = new PutRelativeResponse();

            var documentId = Convert.ToInt64(requestData.Id);
            var relativeTarget = _request.Headers[WopiHeaders.RelativeTarget];
            var suggestedTarget = _request.Headers[WopiHeaders.SuggestedTarget];

            // make sure we don't have both headers present:
            if (!string.IsNullOrEmpty(relativeTarget) && !string.IsNullOrEmpty(suggestedTarget))
            {
                return new WopiJsonResponse()
                {
                    StatusCode = 501,
                    Json = new PutRelativeResponse()
                };
            }

            var overwriteRelative = _request.Headers[WopiHeaders.OverwriteRelativeTarget];
            string extension;

            if (!string.IsNullOrEmpty(relativeTarget))
            {
                // check if we have a file matching the target name
                // and if so, return 409 conflict w/ lock response
                extension = relativeTarget.Substring(relativeTarget.LastIndexOf(".") + 1);
                responseData.Name = IOHelper.Utf7Encode(relativeTarget); // extension should already be here, we just need to get it for below

                var overwriteExisting = !string.IsNullOrEmpty(overwriteRelative) && overwriteRelative.ToLower().Equals("true");
                var relativeDocument = FB.GetDocumentByNameAndExtension(responseData.Name, extension, documentId);

                // does this document already exist?
                if (relativeDocument != null)
                {
                    // lock check - make sure the existing document isn't locked:
                    var currentLock = "";
                    if (LockHelper.IsLockMismatch(_request, relativeDocument, out currentLock))
                    {
                        return new WopiJsonResponse() { StatusCode = 409, Json = responseData };
                    }

                    // not locked - but the document exists, so make sure the overwrite existing header is set:
                    if (!overwriteExisting)
                    {
                        return new WopiJsonResponse()
                        {
                            StatusCode = 409,
                            Json = responseData
                        };
                    }

                }
            }
            else
            {
                // suggested mode:
                // save the file with whatever name we want, and return that name:
                extension = suggestedTarget.Substring(suggestedTarget.LastIndexOf(".") + 1);
                responseData.Name = "wopitest_putrelative." + extension;
            }

            var binary = IOHelper.StreamToBytes(_request.InputStream);
            var newDocumentId = FB.SaveNewDocument(binary, extension, responseData.Name, documentId);
            var newToken = FB.GetAccessToken(newDocumentId);
            responseData.Url = $"{Constants.WopiApiUrl}wopi/files/{newDocumentId}?access_token={newToken}";

            return new WopiJsonResponse()
            {
                StatusCode = 200,
                Json = responseData
            };
        }

        public enum PutRelativeMode
        {
            Specific,
            Suggested
        }
    }
}