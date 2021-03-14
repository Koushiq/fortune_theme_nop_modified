using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public MapperConfiguration()
        {
            CreateMap<ProductRibbonSettings, ConfigurationModel>()
                    .ForMember(model => model.EnableBestSellerRibbon_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.EnableDiscountRibbon_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.EnableNewRibbon_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.ProductDetailsPageWidgetZone_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.ProductOverviewBoxWidgetZone_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.CustomProperties, options => options.Ignore())
                    .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
            CreateMap<ConfigurationModel, ProductRibbonSettings>();
        }
    }
}
