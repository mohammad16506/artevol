using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Http;


namespace artevol
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			CreateRoles();
		}
		private void CreateRoles()
		{
			// Ensure that the required roles are created
			if (!Roles.RoleExists("admin"))
			{
				Roles.CreateRole("admin");
			}

			if (!Roles.RoleExists("user1"))
			{
				Roles.CreateRole("user1");
			}

			if (!Roles.RoleExists("user2"))
			{
				Roles.CreateRole("user2");
			}
		}
	}
}
