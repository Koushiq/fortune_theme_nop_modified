using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.ProductRibbon.Factories;

namespace Nop.Plugin.NopStation.ProductRibbon.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ProductRibbonModelFactory>().As<IProductRibbonModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
