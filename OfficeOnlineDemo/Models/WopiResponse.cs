using System.Collections.Specialized;

namespace OfficeOnlineDemo.Models
{
    public class WopiResponse
    {
        public WopiResponseType ResponseType { get; set; }
        public string Message { get; set; }
    }

    public class WopiJsonResponse
    {
        public int StatusCode { get; set; }
        public object Json { get; set; }
        public string ErrorMessage { get; set; }
        public NameValueCollection Headers { get; set; }

        public WopiJsonResponse()
        {
            StatusCode = 200;
            Json = new { };
            ErrorMessage = "Error";
            Headers = new NameValueCollection();
        }
    }

    public enum WopiResponseType
    {
        Success,
        ServerError,
        InvalidToken,
        FileUnknown,
        LockMismatch
    }

}