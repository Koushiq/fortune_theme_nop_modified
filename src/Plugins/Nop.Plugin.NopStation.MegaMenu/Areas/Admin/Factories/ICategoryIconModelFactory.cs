using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Factories
{
    public interface ICategoryIconModelFactory
    {
        CategoryIconSearchModel PrepareCategoryIconSearchModel(CategoryIconSearchModel searchModel);

        CategoryIconListModel PrepareCategoryIconListModel(CategoryIconSearchModel searchModel);

        CategoryIconModel PrepareCategoryIconModel(CategoryIconModel model, Category category);
    }
}