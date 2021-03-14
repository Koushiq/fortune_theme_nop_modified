using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.NopStation.CategoryBanners.Domains;
using Nop.Plugin.NopStation.CategoryBanners.Infrastructure.Cache;

namespace Nop.Plugin.NopStation.CategoryBanners.Services
{
    public class CategoryBannerService : ICategoryBannerService
    {
        #region Fields

        private readonly IRepository<CategoryBanner> _categoryBannerRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public CategoryBannerService(IRepository<CategoryBanner> categoryBannerRepository,
            IStaticCacheManager cacheManager)
        {
            _categoryBannerRepository = categoryBannerRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public void DeleteCategoryBanner(CategoryBanner categoryBanner)
        {
            if (categoryBanner == null)
                throw new ArgumentNullException(nameof(categoryBanner));

            _categoryBannerRepository.Delete(categoryBanner);
            _cacheManager.RemoveByPrefix(ModelCacheDefaults.CategoryBannerPicturePrefixCacheKey);
        }

        public void InsertCategoryBanner(CategoryBanner categoryBanner)
        {
            if (categoryBanner == null)
                throw new ArgumentNullException(nameof(categoryBanner));

            _categoryBannerRepository.Insert(categoryBanner);
            _cacheManager.RemoveByPrefix(ModelCacheDefaults.CategoryBannerPicturePrefixCacheKey);
        }

        public void UpdateCategoryBanner(CategoryBanner categoryBanner)
        {
            if (categoryBanner == null)
                throw new ArgumentNullException(nameof(categoryBanner));

            _categoryBannerRepository.Update(categoryBanner);
            _cacheManager.RemoveByPrefix(ModelCacheDefaults.CategoryBannerPicturePrefixCacheKey);
        }

        public CategoryBanner GetCategoryBannerById(int categoryBannerId)
        {
            if (categoryBannerId == 0)
                return null;

            return _categoryBannerRepository.GetById(categoryBannerId);
        }

        public IList<CategoryBanner> GetCategoryBannersByCategoryId(int categoryId, bool? mobileDevice = null)
        {
            var query = _categoryBannerRepository.Table.Where(x => x.CategoryId == categoryId);

            if(mobileDevice.HasValue)
                query = query.Where(x => x.ForMobile == mobileDevice.Value);

            query = query.OrderBy(e => e.DisplayOrder);

            return query.ToList();
        }

        #endregion
    }
}
