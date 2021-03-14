using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.NopStation.ProductTabs.Domains;
using Nop.Plugin.NopStation.ProductTabs.Models;

namespace Nop.Plugin.NopStation.ProductTabs.Factories
{
    public interface IProductTabModelFactory
    {
        IList<ProductTabModel> PrepareProductTabListModel(List<ProductTab> productTabs);
        IList<ProductTabModel> PrepareProductTabListModel(string widgetZone);
    }
}
