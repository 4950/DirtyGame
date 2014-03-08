using TowerSite.Models;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Batch;

namespace TowerSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Filters.Add(new AuthorizeAttribute());

            config.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}/{id}", defaults: new { id = RouteParameter.Optional });

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<GameEventModel>("GameEvent");
            builder.EntitySet<GameSession>("GameSession");
            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
        }
    }
}