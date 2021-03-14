using System;
using System.Collections.Generic;
using System.Text;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.CategoryBanners.Domains;

namespace Nop.Plugin.NopStation.CategoryBanners.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(CategoryBanner), "NS_CategoryBanner" }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}
