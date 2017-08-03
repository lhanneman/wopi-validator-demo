using OfficeOnlineDemo.Interfaces;
using System;
using System.Web;
using OfficeOnlineDemo.Models;
using FB = FileBoundHelper.Helper;

namespace OfficeOnlineDemo.Handlers
{
    public class Delete : IWopiHandler
    {
        private readonly HttpRequestBase _request;

        public Delete(HttpRequestBase request)
        {
            _request = request;
        }

        public WopiResponse Handle()
        {
            var requestData = WopiRequest.ParseRequest(_request);

            try
            {
                var documentId = Convert.ToInt64(requestData.Id);
                FB.DeleteDocument(documentId);
            }
            catch (Exception ex)
            {
                return new WopiResponse() { ResponseType = WopiResponseType.ServerError, Message = ex.Message };
            }

            return new WopiResponse() { ResponseType = WopiResponseType.Success };
        }
    }
}