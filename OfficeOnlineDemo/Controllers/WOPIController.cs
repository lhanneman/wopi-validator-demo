using OfficeOnlineDemo.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Controllers
{
    public class WOPIController : Controller
    {

        /// <summary>
        /// Simplified Lock info storage.
        /// A real lock implementation would use persised storage for locks.
        /// </summary>
        private static readonly Dictionary<string, LockInfo> Locks = new Dictionary<string, LockInfo>();

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            // WOPI ProofKey validation is an optional way that a WOPI host can ensure that the request
            // is coming from the Office Online server that they expect to be talking to.
            if (!ValidateWopiProofKey(Request))
            {
                HandleResponse(new WopiResponse() { ResponseType = WopiResponseType.ServerError, Message = "Invalid Proof Key" });
                context.Result = new HttpStatusCodeResult(Response.StatusCode, Response.StatusDescription);
            }

            var requestData = WopiRequest.ParseRequest(Request);
            if (!FB.ValidateAccessToken(Request.QueryString["access_token"], Convert.ToInt64(requestData.Id)))
            {
                HandleResponse(new WopiResponse() { ResponseType = WopiResponseType.InvalidToken, Message = "Invalid Token" });
                context.Result = new HttpStatusCodeResult(Response.StatusCode, Response.StatusDescription);
            }

            base.OnActionExecuting(context);
        }

        // required endpoints:
        // https://wopi.readthedocs.io/en/latest/wopi_requirements.html#requirements

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

        public ActionResult Post(string file_id, string access_token, string access_token_ttl)
        {
            var wopiOverride = Request.Headers["X-WOPI-Override"];
            switch (wopiOverride)
            {
                case "PUT_RELATIVE":
                    //HandleJsonResponse(new Handlers.PutRelative(Request).Handle());
                    var response = new Handlers.PutRelative(Request).Handle();
                    Response.StatusCode = response.StatusCode;

                    if (response.StatusCode != 200)
                    {

                        break;
                    }
                    else
                    {
                        return Json(response.Json, JsonRequestBehavior.AllowGet);
                    }
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
                default:
                    // CheckFileInfo: https://wopirest.readthedocs.io/en/latest/files/CheckFileInfo.html#checkfileinfo
                    return Json(new CheckFileInfoResponse().InitializeValidatorParams(file_id, access_token, access_token_ttl), JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        /// <summary>
        /// Validate WOPI ProofKey to make sure request came from the expected Office Web Apps Server.
        /// </summary>
        /// <param name="request">Request information</param>
        /// <returns>true when WOPI ProofKey validation succeeded, false otherwise.</returns>
        private static bool ValidateWopiProofKey(HttpRequestBase request)
        {
            // TODO: WOPI proof key validation is not implemented in this sample.
            // For more details on proof keys, see the documentation
            // https://wopi.readthedocs.io/en/latest/scenarios/proofkeys.html

            // The proof keys are returned by WOPI Discovery. For more details, see
            // https://wopi.readthedocs.io/en/latest/discovery.html

            var proof = request.Headers["X-WOPI-Proof"];
            var proofOld = request.Headers["X-WOPI-ProofOld"];
            var timestamp = Convert.ToInt64(request.Headers["X-WOPI-TimeStamp"]);
            //var accessToken = request.Headers["Authorization"].Replace("Bearer ", "");
            //var url = request.Url.ToString();
            //var currentKeyInfo = new KeyInfo();
            //var oldKeyInfo = new KeyInfo();
            //var helper = new ProofKeysHelper(currentKeyInfo, oldKeyInfo);
            //var input = new ProofKeyValidationInput(accessToken, timestamp, url, proof, proofOld);
            //return helper.Validate(input);
            //return true;

            return proof != proofOld && !WopiRequest.TimestampOlderThan20Min(timestamp);
        }

        private void HandleResponse(WopiResponse response)
        {
            switch (response.ResponseType) {
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
    }
}