﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models
{
    public class SliderSearchModel : BaseSearchModel
    {
        public SliderSearchModel()
        {
            AvailableWidgetZones = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableActiveOptions = new List<SelectListItem>();
            SearchWidgetZones = new List<int>() { 0 };
        }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.List.SearchWidgetZones")]
        public IList<int> SearchWidgetZones { get; set; }
        
        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.List.SearchActive")]
        public int SearchActiveId { get; set; }

        public IList<SelectListItem> AvailableWidgetZones { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableActiveOptions { get; set; }
    }
}
