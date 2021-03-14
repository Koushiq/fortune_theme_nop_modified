using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Factories;
using Nop.Plugin.NopStation.MegaMenu.Services;

namespace Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CategoryIconService>().As<ICategoryIconService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryIconModelFactory>().As<ICategoryIconModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
