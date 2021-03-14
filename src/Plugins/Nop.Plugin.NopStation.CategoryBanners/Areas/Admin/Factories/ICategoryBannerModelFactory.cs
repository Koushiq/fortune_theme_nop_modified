using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Factories
{
    public interface ICategoryBannerModelFactory
    {
        CategoryBannerListModel PrepareProductPictureListModel(CategoryBannerSearchModel searchModel, Category category);
    }
}