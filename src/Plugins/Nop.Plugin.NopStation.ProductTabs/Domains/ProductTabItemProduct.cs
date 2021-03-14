﻿using Nop.Core;

namespace Nop.Plugin.NopStation.ProductTabs.Domains
{
    public class ProductTabItemProduct : BaseEntity
    {
        public int ProductTabItemId { get; set; }

        public int ProductId { get; set; }

        public int DisplayOrder { get; set; }

        public virtual ProductTabItem ProductTabItem { get; set; }
    }
}
