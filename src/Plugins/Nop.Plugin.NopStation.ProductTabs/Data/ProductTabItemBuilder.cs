using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Data
{
    public class ProductTabItemBuilder : NopEntityBuilder<ProductTabItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
              .WithColumn(nameof(ProductTabItem.ProductTabId)).AsInt32().ForeignKey<ProductTab>().NotNullable();
        }
    }
}
