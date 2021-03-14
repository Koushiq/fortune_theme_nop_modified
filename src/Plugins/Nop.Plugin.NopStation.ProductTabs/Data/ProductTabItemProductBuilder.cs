using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Data
{
    public class ProductTabItemProductBuilder : NopEntityBuilder<ProductTabItemProduct>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
              .WithColumn(nameof(ProductTabItemProduct.ProductTabItemId)).AsInt32().ForeignKey<ProductTabItem>().NotNullable();
        }
    }
}
