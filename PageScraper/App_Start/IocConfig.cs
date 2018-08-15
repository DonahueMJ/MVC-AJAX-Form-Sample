using Autofac;
using Autofac.Integration.Mvc;
using PageScraper.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PageScraper.App_Start
{
    public static class IocConfig
    {
        public static void RegisterDependencies()
        {
            #region Create the builder
            var builder = new ContainerBuilder();
            #endregion
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<PageBuilder>().As<IPageBuilder>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

    }
}