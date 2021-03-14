using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.NopStation.MegaMenu.Services
{
    public interface IMegaMenuCoreService
    {
        IList<Category> GetCategoriesByIds(int storeId = 0, List<int> selectedIds = null, int pageSize = int.MaxValue, bool showHidden = false);

        IList<Manufacturer> GetManufacturersByIds(int storeId = 0, List<int> selectedIds = null, int pageSize = int.MaxValue, bool showHidden = false);
    }
}