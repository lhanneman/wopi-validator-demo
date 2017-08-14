using System.Web.Mvc;
using System.Web.Routing;

namespace OfficeOnlineDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("editWord", "edit/word/{id}", new { controller = "Edit", action = "EditWord" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("editExcel", "edit/excel/{id}", new { controller = "Edit", action = "EditExcel" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("editPpt", "edit/powerpoint/{id}", new { controller = "Edit", action = "EditPowerpoint" }, new { httpMethod = new HttpMethodConstraint("GET") });


            routes.MapRoute("validator", "validator/{id}", new { controller = "Validator", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("get_file_contents", "wopi/files/{file_id}/contents", new { controller = "WOPI", action = "GetFile" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("put_file", "wopi/files/{file_id}/contents", new { controller = "WOPI", action = "PutFile" }, new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapRoute("post", "wopi/files/{file_id}", new { controller = "WOPI", action = "Post" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
