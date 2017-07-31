using System;
using System.Web;

namespace OfficeOnlineDemo.Models
{
    public class WopiResponse
    {
        public WopiResponseType ResponseType { get; set; }
        public string Message { get; set; }
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