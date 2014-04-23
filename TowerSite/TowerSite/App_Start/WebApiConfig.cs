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

            //custom action
            ActionConfiguration getScenario = builder.Entity<GameSession>().Collection.Action("Scenario");
            getScenario.Returns<string>();

            //game version
            ActionConfiguration getGameVersion = builder.Entity<GameSession>().Collection.Action("CurrentVersion");
            getGameVersion.Returns<string>();

            //get that ELORank
            ActionConfiguration getELORank = builder.Entity<GameSession>().Collection.Action("ELORank");
            getELORank.Returns<string>();

            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));

        }
    }
}