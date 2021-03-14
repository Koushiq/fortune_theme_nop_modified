using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.NopStation.CategoryBanners.Domains;
using Nop.Plugin.NopStation.CategoryBanners.Services;

namespace Nop.Plugin.NopStation.CategoryBanners.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "Nop.plugin.nopstation.categorybanners_object_context";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CategoryBannerService>().As<ICategoryBannerService>().InstancePerLifetimeScope();
            //data context
        }

        public int Order => 1;
    }
}
