using System.Collections.Generic;
using Nop.Plugin.NopStation.CategoryBanners.Domains;

namespace Nop.Plugin.NopStation.CategoryBanners.Services
{
    public interface ICategoryBannerService
    {
        void DeleteCategoryBanner(CategoryBanner categoryBanner);

        void InsertCategoryBanner(CategoryBanner categoryBanner);

        void UpdateCategoryBanner(CategoryBanner categoryBanner);

        CategoryBanner GetCategoryBannerById(int categoryBannerId);

        IList<CategoryBanner> GetCategoryBannersByCategoryId(int categoryId, bool? mobileDevice = null);
    }
}