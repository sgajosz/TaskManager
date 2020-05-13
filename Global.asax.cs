using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TaskManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<TaskManager.Models.UserContext>(null);
            Database.SetInitializer<TaskManager.Models.ProjectContext>(null);
            Database.SetInitializer<TaskManager.Models.TaskContext>(null);
            Database.SetInitializer<TaskManager.Models.ProjectUserJoinContext>(null);
            Database.SetInitializer<TaskManager.Models.TechnologyContext>(null);
            Database.SetInitializer<TaskManager.Models.ErrandContext>(null);
            Database.SetInitializer<TaskManager.Models.TypeContext>(null);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
