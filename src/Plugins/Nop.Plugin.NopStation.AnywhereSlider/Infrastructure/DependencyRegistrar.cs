using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.AnywhereSlider.Factories;
using Nop.Plugin.NopStation.AnywhereSlider.Services;

namespace Nop.Plugin.NopStation.AnywhereSlider.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "Nop.plugin.nopstation.anywhereslider_object_context";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<SliderService>().As<ISliderService>().InstancePerLifetimeScope();

            builder.RegisterType<SliderModelFactory>().As<ISliderModelFactory>().InstancePerLifetimeScope();

        }

        public int Order => 1;
    }
}
