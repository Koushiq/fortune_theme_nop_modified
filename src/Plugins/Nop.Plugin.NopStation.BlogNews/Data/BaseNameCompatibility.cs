using System;
using System.Collections.Generic;
using System.Text;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.BlogNews.Domains;

namespace Nop.Plugin.NopStation.BlogNews.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(BlogNewsPicture), "NS_BlogNewsPicture" },
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}