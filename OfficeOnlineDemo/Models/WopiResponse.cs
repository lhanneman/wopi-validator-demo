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