using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.BlogNews.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.BlogNews.Areas.Admin.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public MapperConfiguration()
        {
            CreateMap<BlogNewsSettings, ConfigurationModel>()
                    .ForMember(model => model.BlogPostPictureSize_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.NewsItemPictureSize_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.NumberOfNewsItemsToShow_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.NumberOfBlogPostsToShow_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.ShowBlogsInStore_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.WidgetZone_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.ShowNewsInStore_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.CustomProperties, options => options.Ignore())
                    .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
            CreateMap<ConfigurationModel, BlogNewsSettings>();
        }
    }
}
