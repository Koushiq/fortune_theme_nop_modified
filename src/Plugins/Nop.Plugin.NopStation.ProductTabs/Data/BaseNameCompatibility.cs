using System;
using System.Collections.Generic;
using System.Text;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ProductTab), "NS_ProductTab" },
            { typeof(ProductTabItem), "NS_ProductTabItem" },
            { typeof(ProductTabItemProduct), "NS_ProductTabItemProduct" }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}
