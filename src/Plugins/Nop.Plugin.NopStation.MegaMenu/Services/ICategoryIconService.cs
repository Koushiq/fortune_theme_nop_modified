using Nop.Core;
using Nop.Plugin.NopStation.MegaMenu.Domains;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.MegaMenu.Services
{
    public interface ICategoryIconService
    {
        void DeleteCategoryIcon(CategoryIcon categoryIcon);

        void InsertCategoryIcon(CategoryIcon categoryIcon);

        void UpdateCategoryIcon(CategoryIcon categoryIcon);

        CategoryIcon GetCategoryIconById(int categoryIconId);

        IList<CategoryIcon> GetCategoryIconByIds(int[] categoryIconIds);

        CategoryIcon GetCategoryIconByCategoryId(int categoryId);

        IPagedList<CategoryIcon> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue);

        void DeleteCategoryIcons(List<CategoryIcon> categoryIcons);
    }
}