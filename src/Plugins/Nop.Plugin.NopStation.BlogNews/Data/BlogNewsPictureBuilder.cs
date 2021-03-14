using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.BlogNews.Domains;

namespace Nop.Plugin.NopStation.BlogNews.Data
{
    public class BlogNewsPictureBuilder : NopEntityBuilder<BlogNewsPicture>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.
              WithColumn(nameof(BlogNewsPicture.ShowInStore))
              .AsBoolean();
        }
    }
}
