//using OfficeOnlineDemo.Models;
//using System;
//using System.Web.Mvc;

//namespace OfficeOnlineDemo.Controllers
//{
//    public class OOSController : Controller
//    {
//        // GET: OOS
//        [HttpGet]
//        [Route("oos/edit")]
//        public ActionResult Edit()
//        {

//            // discovery xml:
//            // https://onenote.officeapps-df.live.com/hosting/discovery
//            // action url for Edit Docx:
//            var actionUrl = "https://word-edit.officeapps-df.live.com/we/wordeditorframe.aspx?<ui=UI_LLCC&><rs=DC_LLCC&><dchat=DISABLE_CHAT&><hid=HOST_SESSION_ID&><showpagestats=PERFSTATS&><IsLicensedUser=BUSINESS_USER&>";

//            var tokenExpires = Convert.ToInt64(DateTime.UtcNow.AddDays(1).ToUniversalTime().Subtract(
//                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
//            ).TotalMilliseconds);

//            return View(new EditDocumentModel()
//            {
//                AccessToken = "asdfasdfasdf",
//                AccessTokenExpires = tokenExpires,
//                ActionUrl = GetTestUrl()
//            });
//        }

//        private string GetTestUrl()
//        {

//            var wopi_api = "http://wopi-api.azurewebsites.net/";

//            var business_user = 1; // 1 means user is a business user
//            var ui_llcc = "en-US";
//            var dc_llcc = "en-US";
//            var disable_chat = 1;
//            var host_session_id = Guid.NewGuid().ToString();
//            var test_category = "OfficeOnline"; // could be All or OfficeNativeClient for iOS stuff

//            //var testUrl = $"https://onenote.officeapps-df.live.com/hosting/WopiTestFrame.aspx?<ui={ui_llcc}&><rs={dc_llcc}&><dchat={disable_chat}&><hid={host_session_id}&><IsLicensedUser={business_user}&><testcategory={test_category}&>";
//            var testUrl = $"https://onenote.officeapps-df.live.com/hosting/WopiTestFrame.aspx?ui={ui_llcc}&rs={dc_llcc}&dchat={disable_chat}&hid={host_session_id}&IsLicensedUser={business_user}&testcategory={test_category}";

//            //testUrl += "&WOPISrc=http://localhost:49856/wopi/files/1";
//            testUrl += $"&WOPIsrc={wopi_api}wopi/files/1";

//            return testUrl;
//        }
//    }
//}