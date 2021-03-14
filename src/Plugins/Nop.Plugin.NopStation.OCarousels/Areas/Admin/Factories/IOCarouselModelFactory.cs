using Nop.Plugin.NopStation.OCarousels.Areas.Admin.Models;
using Nop.Plugin.NopStation.OCarousels.Domains;

namespace Nop.Plugin.NopStation.OCarousels.Areas.Admin.Factories
{
    public interface IOCarouselModelFactory
    {
        ConfigurationModel PrepareConfigurationModel();

        OCarouselSearchModel PrepareOCarouselSearchModel(OCarouselSearchModel searchModel);

        OCarouselListModel PrepareOCarouselListModel(OCarouselSearchModel searchModel);

        OCarouselModel PrepareOCarouselModel(OCarouselModel model, OCarousel carousel, 
            bool excludeProperties = false);

        OCarouselItemListModel PrepareOCarouselItemListModel(OCarouselItemSearchModel searchModel, OCarousel carousel);

        AddProductToCarouselSearchModel PrepareAddProductToOCarouselSearchModel(AddProductToCarouselSearchModel searchModel);

        AddProductToCarouselListModel PrepareAddProductToOCarouselListModel(AddProductToCarouselSearchModel searchModel);
    }
}