using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.NopStation.QuickView.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private static string CONTEXT_NAME = "Nop.Plugin.NopStation.QuickView_object_context";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //builder.RegisterType<CustomService>().As<ICustomAttributeService>().InstancePerLifetimeScope();

            //builder.RegisterType<CustomModelFactory>().As<ICustomModelFactory>().InstancePerLifetimeScope();

            ////data context
            //builder.RegisterPluginDataContext<CustomObjectContext>(CONTEXT_NAME);

            ////override required repository with our custom context
            //builder.RegisterType<EfRepository<CustomTable>>()
            //    .As<IRepository<CustomTable>>()
            //    .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            //    .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
