using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Data
{
    public class ProductTabBuilder : NopEntityBuilder<ProductTab>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(ProductTab.Name))
                 .AsString(400);
        }
    }
}
