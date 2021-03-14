using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.ProductRibbon.Models;

namespace Nop.Plugin.NopStation.ProductRibbon.Factories
{
    public interface IProductRibbonModelFactory
    {
        ProductRibbonModel PrepareProductRibbonModel(Product product);
    }
}