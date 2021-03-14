using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;

namespace Nop.Plugin.NopStation.AnywhereSlider.Data
{
    public class SliderItemBuilder : NopEntityBuilder<SliderItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SliderItem.SliderId)).AsInt32().ForeignKey<Slider>();
        }
    }
}
