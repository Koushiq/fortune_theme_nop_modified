using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Factories;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<SliderModelFactory>().As<ISliderModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
