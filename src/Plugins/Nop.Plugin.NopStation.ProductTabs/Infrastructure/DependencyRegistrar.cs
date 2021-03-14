using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.ProductTabs.Factories;
using Nop.Plugin.NopStation.ProductTabs.Services;

namespace Nop.Plugin.NopStation.ProductTabs.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "Nop.plugin.nopstation.producttabs_object_context";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ProductTabService>().As<IProductTabService>().InstancePerLifetimeScope();

            builder.RegisterType<ProductTabModelFactory>().As<IProductTabModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
