using OfficeOnlineDemo.Helpers;
using OfficeOnlineDemo.Models;
using System;
using System.Web.Mvc;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Controllers
{
    public class EditController : Controller
    {
        // discovery xml:
        // https://onenote.officeapps-df.live.com/hosting/discovery

        public ActionResult EditWord(long id)
        {
            return View("WopiFrame", new EditDocumentModel()
            {
                AccessToken = FB.GetAccessToken(id),
                AccessTokenExpires = GetTokenExpires(),
                ActionUrl = GetEditWordUrl(id)
            });
        }

        public ActionResult EditExcel(long id)
        {
            return View("WopiFrame", new EditDocumentModel()
            {
                AccessToken = FB.GetAccessToken(id),
                AccessTokenExpires = GetTokenExpires(),
                ActionUrl = GetEditExcelUrl(id)
            });
        }

        private string GetEditWordUrl(long documentId)
        {
            var business_user = 1; // 1 means user is a business user
            var ui_llcc = "en-US";
            var dc_llcc = "en-US";
            var disable_chat = 1;
            var host_session_id = Guid.NewGuid().ToString();
            var test_category = "OfficeOnline"; // could be All or OfficeNativeClient for iOS stuff

            var url = $"https://word-edit.officeapps-df.live.com/we/wordeditorframe.aspx?ui={ui_llcc}&rs={dc_llcc}&dchat={disable_chat}&hid={host_session_id}&IsLicensedUser={business_user}&testcategory={test_category}";

            url += $"&WOPIsrc={Constants.WopiApiUrl}wopi/files/" + documentId.ToString();

            return url;
        }

        private string GetEditExcelUrl(long documentId)
        {
            var business_user = 1; // 1 means user is a business user
            var ui_llcc = "en-US";
            var dc_llcc = "en-US";
            var disable_chat = 1;
            var host_session_id = Guid.NewGuid().ToString();
            var test_category = "OfficeOnline"; // could be All or OfficeNativeClient for iOS stuff

            var url = $"https://excel.officeapps-df.live.com/x/_layouts/xlviewerinternal.aspx?edit=1&ui={ui_llcc}&rs={dc_llcc}&dchat={disable_chat}&hid={host_session_id}&IsLicensedUser={business_user}&testcategory={test_category}";
            //var url = $"https://excel.officeapps-df.live.com/x/_layouts/xlviewerinternal.aspx?edit=1&<ui=UI_LLCC&><rs=DC_LLCC&><dchat=DISABLE_CHAT&><hid=HOST_SESSION_ID&><IsLicensedUser=BUSINESS_USER&>"

            url += $"&WOPIsrc={Constants.WopiApiUrl}wopi/files/" + documentId.ToString();

            return url;
        }

        private long GetTokenExpires()
        {
            return Convert.ToInt64(DateTime.UtcNow.AddDays(1).ToUniversalTime().Subtract(
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds);
        }
    }
}