using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.MegaMenu.Factories;
using Nop.Plugin.NopStation.MegaMenu.Services;

namespace Nop.Plugin.NopStation.MegaMenu.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<MegaMenuModelFactory>().As<IMegaMenuModelFactory>().InstancePerLifetimeScope();

            builder.RegisterType<MegaMenuCoreService>().As<IMegaMenuCoreService>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
