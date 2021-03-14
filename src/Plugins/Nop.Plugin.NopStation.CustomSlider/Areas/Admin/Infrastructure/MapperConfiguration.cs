using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models;
using Nop.Plugin.NopStation.CustomSlider.Domains;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public MapperConfiguration()
        {
            CreateMap<CustomSliderSettings, ConfigurationModel>()
                    .ForMember(model => model.EnableSlider_OverrideForStore, options => options.Ignore())
                    .ForMember(model => model.CustomProperties, options => options.Ignore())
                    .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
            CreateMap<ConfigurationModel, CustomSliderSettings>();

            CreateMap<Slider, SliderModel>()
                    .ForMember(model => model.AvailableStores, options => options.Ignore())
                    .ForMember(model => model.AvailableWidgetZones, options => options.Ignore())
                    .ForMember(model => model.WidgetZoneStr, options => options.Ignore())
                    .ForMember(model => model.CreatedOn, options => options.Ignore())
                    .ForMember(model => model.UpdatedOn, options => options.Ignore())
                    .ForMember(model => model.SliderItemSearchModel, options => options.Ignore())
                    .ForMember(model => model.CustomProperties, options => options.Ignore())
                    .ForMember(model => model.SelectedStoreIds, options => options.Ignore());

            CreateMap<SliderModel, Slider>()
                    .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                    .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<SliderItem, SliderItemModel>()
                    .ForMember(model => model.CustomProperties, options => options.Ignore())
                    .ForMember(model => model.FullPictureUrl, options => options.Ignore())
                    .ForMember(model => model.PictureUrl, options => options.Ignore())
                    .ForMember(model => model.MobileFullPictureUrl, options => options.Ignore())
                    .ForMember(model => model.MobilePictureUrl, options => options.Ignore());
            CreateMap<SliderItemModel, SliderItem>();
        }
    }
}
