using OfficeOnlineDemo.Models;
using System.Web;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Helpers
{
    public class LockHelper
    {
        public static bool IsLockMismatch(HttpRequestBase request, FileBound.Document document, out string currentLock)
        {
            var lockInfo = FB.GetLockInfo(document);
            var currentLockInfo = !string.IsNullOrEmpty(lockInfo)
                                ? Newtonsoft.Json.JsonConvert.DeserializeObject<LockInfo>(lockInfo)
                                : null;

            currentLock = currentLockInfo?.Lock ?? string.Empty;
            return currentLockInfo != null && (currentLockInfo.Lock != request.Headers[WopiHeaders.Lock]);
        }
    }
}