using OfficeOnlineDemo.Interfaces;
using OfficeOnlineDemo.Models;
using System.Web;
using FB = FileBoundHelper.Helper;
using System.IO;
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
            // can be called on locked file, so lock header is not present

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
                responseData.Name = Utf7Encode(relativeTarget); // extension should already be here, we just need to get it for below

                var overwriteExisting = !string.IsNullOrEmpty(overwriteRelative) && overwriteRelative.ToLower().Equals("true");

                try
                {
                    if (!overwriteExisting && FB.DocumentExists(responseData.Name, extension, documentId))
                    {
                        return new WopiJsonResponse()
                        {
                            StatusCode = 409,
                            Json = new PutRelativeResponse()
                        };

                    }
                }
                catch (Exception ex)
                {
                    var i = 0;
                }
            }
            else
            {
                // suggested mode:
                extension = suggestedTarget.Substring(suggestedTarget.LastIndexOf(".") + 1);
                responseData.Name = "wopitest_putrelative." + extension;


                // save the file with whatever name we want, and return that name:
            }

            _request.InputStream.Position = 0;
            byte[] binary;
            using (var ms = new MemoryStream())
            {
                _request.InputStream.CopyTo(ms);
                binary = ms.ToArray();
            }


            var newDocumentId = FB.SaveNewDocument(binary, extension, responseData.Name, documentId);
            //$"&WOPIsrc={Constants.WopiApiUrl}wopi/files/" + documentId.ToString();
            var access_code = FB.GetAccessToken(newDocumentId);
            responseData.Url = $"{Constants.WopiApiUrl}wopi/files/{newDocumentId}?access_code={access_code}";

            return new WopiJsonResponse()
            {
                StatusCode = 200,
                Json = responseData
            };
        }

        private string Utf7Encode(string value)
        {
            return System.Text.Encoding.UTF7.GetString(System.Text.Encoding.ASCII.GetBytes(value));
        }

        public enum PutRelativeMode
        {
            Specific,
            Suggested
        }
    }
}