using Newtonsoft.Json;
using OfficeOnlineDemo.Models;
using System;
using System.Text;
using System.Web.Mvc;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Controllers
{
    public class WOPIController : Controller
    {
        // required endpoints:
        // https://wopi.readthedocs.io/en/latest/wopi_requirements.html#requirements

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestData = WopiRequest.ParseRequest(Request);
            if (!FB.ValidateAccessToken(Request.QueryString["access_token"], Convert.ToInt64(requestData.Id)))
            {
                HandleResponse(new WopiResponse() { ResponseType = WopiResponseType.InvalidToken, Message = "Invalid Token" });
                context.Result = new HttpStatusCodeResult(Response.StatusCode, Response.StatusDescription);
            }

            base.OnActionExecuting(context);
        }

        // GetFile: https://wopirest.readthedocs.io/en/latest/files/GetFile.html#getfile
        [HttpGet]
        public FileContentResult GetFile(long file_id, string access_token, string access_token_ttl)
        {
            var document = FB.GetDocument(file_id);
            return File(document.BinaryData, FB.GetContentType(document.Extension));
        }

        // PutFile: https://wopirest.readthedocs.io/en/latest/files/PutFile.html#putfile
        [HttpPost]
        public void PutFile(string file_id, string access_token, string access_token_ttl)
        {
            HandleResponse(new Handlers.PutFile(Request).Handle());
        }

        public ContentResult Post(string file_id, string access_token, string access_token_ttl)
        {
            var wopiOverride = Request.Headers["X-WOPI-Override"];
            switch (wopiOverride)
            {
                case "PUT_RELATIVE":
                    return HandleJsonResponse(new Handlers.PutRelative(Request).Handle());
                case "GET_SHARE_URL":
                    //https://wopirest.readthedocs.io/en/latest/files/GetShareUrl.html?highlight=getshareurl
                    Response.StatusCode = 501;
                    //return Json(new { ShareUrl = "http://wopi-api.azurewebsites.net/oos/share" });
                    break;

                case "LOCK":
                    HandleResponse(new Handlers.Lock(Request).Handle());
                    break;
                case "UNLOCK":
                    HandleResponse(new Handlers.Unlock(Request).Handle());
                    break;
                case "REFRESH_LOCK":
                    HandleResponse(new Handlers.Lock(Request).HandleRefresh());
                    break;
                case "DELETE":
                    HandleResponse(new Handlers.Delete(Request).Handle());
                    break;
                case "RENAME_FILE":
                    return HandleJsonResponse(new Handlers.Rename(Request).Handle());
                default:
                    // CheckFileInfo: https://wopirest.readthedocs.io/en/latest/files/CheckFileInfo.html#checkfileinfo
                    return GetJson(new CheckFileInfoResponse().InitializeValidatorParams(file_id, access_token, access_token_ttl));
            }

            return new ContentResult() { Content = "did not handle action: " + wopiOverride, ContentType = "text/plain" };
        }

        private ContentResult HandleJsonResponse(WopiJsonResponse jsonResponse)
        {
            Response.StatusCode = jsonResponse.StatusCode;

            if (jsonResponse.Headers.Count > 0)
            {
                Response.Headers.Add(jsonResponse.Headers);
            }

            if (jsonResponse.StatusCode >= 500)
            {
                return new ContentResult() { Content = jsonResponse.ErrorMessage, ContentType = "text/plain" };
            }

            return GetJson(jsonResponse.Json);
        }

        private void HandleResponse(WopiResponse response)
        {
            switch (response.ResponseType)
            {
                case WopiResponseType.ServerError:
                    ReturnStatus(500, response.Message);
                    break;
                case WopiResponseType.FileUnknown:
                    ReturnStatus(404, "File Unknown/User Unauthorized");
                    break;
                case WopiResponseType.InvalidToken:
                    ReturnStatus(401, response.Message);
                    break;
                case WopiResponseType.LockMismatch:
                    Response.Headers[WopiHeaders.Lock] = response.Message ?? String.Empty;
                    //if (!String.IsNullOrEmpty(reason))
                    //{
                    //    Response.Headers[WopiHeaders.LockFailureReason] = reason;
                    //}
                    ReturnStatus(409, "Lock mismatch/Locked by another interface");
                    break;
                case WopiResponseType.Success:

                    break;
            }

        }

        private void ReturnStatus(int code, string description)
        {
            Response.StatusCode = code;
            Response.StatusDescription = description;
        }

        private ContentResult GetJson(object data)
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(data),
                ContentEncoding = Encoding.UTF8,
            };
        }
    }
}