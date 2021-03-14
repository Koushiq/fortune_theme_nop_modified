using Nop.Core;
using Nop.Data;
using Nop.Plugin.NopStation.MegaMenu.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.MegaMenu.Services
{
    public class CategoryIconService : ICategoryIconService
    {
        #region Fields

        private readonly IRepository<CategoryIcon> _categoryIconRepository;

        #endregion

        #region Ctor

        public CategoryIconService(IRepository<CategoryIcon> categoryIconRepository)
        {
            _categoryIconRepository = categoryIconRepository;
        }

        #endregion

        #region Methods

        public void DeleteCategoryIcon(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconRepository.Delete(categoryIcon);
        }

        public void InsertCategoryIcon(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconRepository.Insert(categoryIcon);
        }

        public void UpdateCategoryIcon(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconRepository.Update(categoryIcon);
        }

        public CategoryIcon GetCategoryIconById(int categoryIconId)
        {
            if (categoryIconId == 0)
                return null;

            return _categoryIconRepository.GetById(categoryIconId);
        }

        public CategoryIcon GetCategoryIconByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return _categoryIconRepository.Table.FirstOrDefault(x => x.CategoryId == categoryId);
        }

        public IPagedList<CategoryIcon> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var categoryIcons = _categoryIconRepository.Table;

            categoryIcons = categoryIcons.OrderByDescending(e => e.Id);

            return new PagedList<CategoryIcon>(categoryIcons, pageIndex, pageSize);
        }

        public IList<CategoryIcon> GetCategoryIconByIds(int[] categoryIconIds)
        {
            if (categoryIconIds == null || categoryIconIds.Length == 0)
                return new List<CategoryIcon>();

            var query = _categoryIconRepository.Table.Where(x => categoryIconIds.Contains(x.Id));

            return query.ToList();
        }

        public void DeleteCategoryIcons(List<CategoryIcon> categoryIcons)
        {
            if (categoryIcons == null)
                throw new ArgumentNullException(nameof(categoryIcons));

            _categoryIconRepository.Delete(categoryIcons);
        }

        #endregion
    }
}
