using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;

namespace Nop.Plugin.NopStation.AnywhereSlider.Data
{
    public class SliderBuilder : NopEntityBuilder<Slider>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Slider.Name)).AsString(1000).NotNullable();
        }
    }
}
