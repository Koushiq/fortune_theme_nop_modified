using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.BlogNews.Services;

namespace Nop.Plugin.NopStation.BlogNews.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_blog_news";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BlogNewsPictureService>().As<IBlogNewsPictureService>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
