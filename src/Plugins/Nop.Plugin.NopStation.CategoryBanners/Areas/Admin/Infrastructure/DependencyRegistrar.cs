using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Factories;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CategoryBannerModelFactory>().As<ICategoryBannerModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
