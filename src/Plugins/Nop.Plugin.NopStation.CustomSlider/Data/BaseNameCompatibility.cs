using System;
using System.Collections.Generic;
using System.Text;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.CustomSlider.Domains;

namespace Nop.Plugin.NopStation.CustomSlider.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(Slider), "NS_Slider" },
            { typeof(SliderItem), "NS_SliderItem" }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}
