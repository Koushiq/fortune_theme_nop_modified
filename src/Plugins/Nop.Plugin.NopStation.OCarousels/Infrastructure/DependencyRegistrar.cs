using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.OCarousels.Factories;
using Nop.Plugin.NopStation.OCarousels.Services;

namespace Nop.Plugin.NopStation.OCarousels.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "Nop.plugin.nopstation.ocarousels_object_context";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<OCarouselService>().As<IOCarouselService>().InstancePerLifetimeScope();

            builder.RegisterType<OCarouselModelFactory>().As<IOCarouselModelFactory>().InstancePerLifetimeScope();

        }

        public int Order => 1;
    }
}
